using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Datos;

public sealed class LibroRepositorio
{
    private readonly ConexionBaseDatos conexionBaseDatos;

    public LibroRepositorio(ConexionBaseDatos conexionBaseDatos)
    {
        this.conexionBaseDatos = conexionBaseDatos;
    }

    public async Task<List<Libro>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT l.LibroId, l.Codigo, l.ISBN, l.Titulo, l.AnioPublicacion,
                   l.EditorialId, e.Nombre, l.CantidadTotal, l.CantidadDisponible,
                   la.AutorId, a.Nombre, a.Apellido
            FROM dbo.Libros AS l
            INNER JOIN dbo.Editoriales AS e ON e.EditorialId = l.EditorialId
            LEFT JOIN dbo.LibroAutores AS la ON la.LibroId = l.LibroId
            LEFT JOIN dbo.Autores AS a ON a.AutorId = la.AutorId
            ORDER BY l.Titulo, a.Apellido, a.Nombre;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(tokenCancelacion);

        var librosPorId = new Dictionary<int, Libro>();
        while (await lector.ReadAsync(tokenCancelacion))
        {
            int libroId = lector.GetInt32(0);
            if (!librosPorId.TryGetValue(libroId, out Libro? libro))
            {
                libro = new Libro
                {
                    LibroId = libroId,
                    Codigo = lector.GetString(1),
                    ISBN = lector.IsDBNull(2) ? null : lector.GetString(2),
                    Titulo = lector.GetString(3),
                    AnioPublicacion = lector.IsDBNull(4) ? null : lector.GetInt16(4),
                    EditorialId = lector.GetInt32(5),
                    EditorialNombre = lector.GetString(6),
                    CantidadTotal = lector.GetInt32(7),
                    CantidadDisponible = lector.GetInt32(8)
                };
                librosPorId.Add(libroId, libro);
            }

            if (!lector.IsDBNull(9))
            {
                int autorId = lector.GetInt32(9);
                if (!libro.AutoresIds.Contains(autorId))
                {
                    libro.AutoresIds.Add(autorId);
                    libro.AutoresNombres.Add($"{lector.GetString(10)} {lector.GetString(11)}");
                }
            }
        }

        return librosPorId.Values.ToList();
    }

    public async Task CrearAsync(Libro libro, CancellationToken tokenCancelacion = default)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var transaccion =
            (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable, tokenCancelacion);

        const string consulta = """
            INSERT INTO dbo.Libros
                (Codigo, ISBN, Titulo, AnioPublicacion, EditorialId, CantidadTotal, CantidadDisponible)
            OUTPUT INSERTED.LibroId
            VALUES
                (@Codigo, @ISBN, @Titulo, @AnioPublicacion, @EditorialId, @CantidadTotal, @CantidadTotal);
            """;

        await using var comando = new SqlCommand(consulta, conexion, transaccion);
        AgregarParametros(comando, libro);
        int libroId = (int)(await comando.ExecuteScalarAsync(tokenCancelacion)
            ?? throw new InvalidOperationException("No se pudo registrar el libro."));

        await ReemplazarAutoresAsync(conexion, transaccion, libroId, libro.AutoresIds, tokenCancelacion);
        await transaccion.CommitAsync(tokenCancelacion);
    }

    public async Task ActualizarAsync(Libro libro, CancellationToken tokenCancelacion = default)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var transaccion =
            (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable, tokenCancelacion);

        const string consultaPrestamos = """
            SELECT COUNT(*)
            FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
            WHERE LibroId = @LibroId AND FechaDevolucionReal IS NULL;
            """;

        await using var comandoPrestamos = new SqlCommand(consultaPrestamos, conexion, transaccion);
        comandoPrestamos.Parameters.Add("@LibroId", SqlDbType.Int).Value = libro.LibroId;
        int prestamosActivos = Convert.ToInt32(
            await comandoPrestamos.ExecuteScalarAsync(tokenCancelacion),
            System.Globalization.CultureInfo.InvariantCulture);

        if (libro.CantidadTotal < prestamosActivos)
        {
            throw new InvalidOperationException(
                $"El libro tiene {prestamosActivos} ejemplar(es) prestado(s); no puedes reducir el total a {libro.CantidadTotal}.");
        }

        const string consultaActualizacion = """
            UPDATE dbo.Libros
            SET Codigo = @Codigo,
                ISBN = @ISBN,
                Titulo = @Titulo,
                AnioPublicacion = @AnioPublicacion,
                EditorialId = @EditorialId,
                CantidadTotal = @CantidadTotal,
                CantidadDisponible = @CantidadTotal - @PrestamosActivos
            WHERE LibroId = @LibroId;
            """;

        await using var comandoActualizacion = new SqlCommand(consultaActualizacion, conexion, transaccion);
        comandoActualizacion.Parameters.Add("@LibroId", SqlDbType.Int).Value = libro.LibroId;
        comandoActualizacion.Parameters.Add("@PrestamosActivos", SqlDbType.Int).Value = prestamosActivos;
        AgregarParametros(comandoActualizacion, libro);

        if (await comandoActualizacion.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException("No se encontró el libro que deseas actualizar.");
        }

        await ReemplazarAutoresAsync(
            conexion,
            transaccion,
            libro.LibroId,
            libro.AutoresIds,
            tokenCancelacion);
        await transaccion.CommitAsync(tokenCancelacion);
    }

    public async Task EliminarAsync(int libroId, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            DELETE FROM dbo.Libros
            WHERE LibroId = @LibroId
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.Prestamos
                  WHERE LibroId = @LibroId
              );
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@LibroId", SqlDbType.Int).Value = libroId;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException(
                "No se puede eliminar el libro: no existe o tiene préstamos registrados.");
        }
    }

    private static void AgregarParametros(SqlCommand comando, Libro libro)
    {
        comando.Parameters.Add("@Codigo", SqlDbType.NVarChar, 20).Value = libro.Codigo;
        comando.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = (object?)libro.ISBN ?? DBNull.Value;
        comando.Parameters.Add("@Titulo", SqlDbType.NVarChar, 200).Value = libro.Titulo;
        comando.Parameters.Add("@AnioPublicacion", SqlDbType.SmallInt).Value =
            (object?)libro.AnioPublicacion ?? DBNull.Value;
        comando.Parameters.Add("@EditorialId", SqlDbType.Int).Value = libro.EditorialId;
        comando.Parameters.Add("@CantidadTotal", SqlDbType.Int).Value = libro.CantidadTotal;
    }

    private static async Task ReemplazarAutoresAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        int libroId,
        IReadOnlyCollection<int> autoresIds,
        CancellationToken tokenCancelacion)
    {
        const string consultaEliminar = "DELETE FROM dbo.LibroAutores WHERE LibroId = @LibroId;";
        await using (var comandoEliminar = new SqlCommand(consultaEliminar, conexion, transaccion))
        {
            comandoEliminar.Parameters.Add("@LibroId", SqlDbType.Int).Value = libroId;
            await comandoEliminar.ExecuteNonQueryAsync(tokenCancelacion);
        }

        const string consultaInsertar = """
            INSERT INTO dbo.LibroAutores (LibroId, AutorId)
            VALUES (@LibroId, @AutorId);
            """;
        await using var comandoInsertar = new SqlCommand(consultaInsertar, conexion, transaccion);
        comandoInsertar.Parameters.Add("@LibroId", SqlDbType.Int).Value = libroId;
        SqlParameter parametroAutorId = comandoInsertar.Parameters.Add("@AutorId", SqlDbType.Int);

        foreach (int autorId in autoresIds)
        {
            parametroAutorId.Value = autorId;
            await comandoInsertar.ExecuteNonQueryAsync(tokenCancelacion);
        }
    }
}
