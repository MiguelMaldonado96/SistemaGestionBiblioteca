using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Datos;

public sealed class PrestamoRepositorio
{
    private readonly ConexionBaseDatos conexionBaseDatos;

    public PrestamoRepositorio(ConexionBaseDatos conexionBaseDatos)
    {
        this.conexionBaseDatos = conexionBaseDatos;
    }

    public async Task<List<Prestamo>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT p.PrestamoId, p.UsuarioId, p.LibroId,
                   u.Nombre, u.Apellido, l.Codigo, l.Titulo,
                   p.FechaPrestamo, p.FechaDevolucionPrevista,
                   p.FechaDevolucionReal, p.Estado
            FROM dbo.Prestamos AS p
            INNER JOIN dbo.Usuarios AS u ON u.UsuarioId = p.UsuarioId
            INNER JOIN dbo.Libros AS l ON l.LibroId = p.LibroId
            ORDER BY p.FechaPrestamo DESC, p.PrestamoId DESC;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(tokenCancelacion);

        var prestamos = new List<Prestamo>();
        while (await lector.ReadAsync(tokenCancelacion))
        {
            prestamos.Add(new Prestamo
            {
                PrestamoId = lector.GetInt32(0),
                UsuarioId = lector.GetInt32(1),
                LibroId = lector.GetInt32(2),
                UsuarioNombre = $"{lector.GetString(3)} {lector.GetString(4)}",
                CodigoLibro = lector.GetString(5),
                LibroTitulo = lector.GetString(6),
                FechaPrestamo = lector.GetDateTime(7),
                FechaDevolucionPrevista = lector.GetDateTime(8),
                FechaDevolucionReal = lector.IsDBNull(9) ? null : lector.GetDateTime(9),
                Estado = lector.GetString(10)
            });
        }

        return prestamos;
    }

    public async Task RegistrarAsync(
        Prestamo prestamo,
        CancellationToken tokenCancelacion = default)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var transaccion =
            (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable, tokenCancelacion);

        const string consultaActualizarExistencias = """
            UPDATE dbo.Libros
            SET CantidadDisponible = CantidadDisponible - 1
            WHERE LibroId = @LibroId AND CantidadDisponible > 0;
            """;

        await using (var comandoExistencias = new SqlCommand(
            consultaActualizarExistencias,
            conexion,
            transaccion))
        {
            comandoExistencias.Parameters.Add("@LibroId", SqlDbType.Int).Value = prestamo.LibroId;
            if (await comandoExistencias.ExecuteNonQueryAsync(tokenCancelacion) != 1)
            {
                throw new InvalidOperationException(
                    "El libro no existe o no tiene ejemplares disponibles para préstamo.");
            }
        }

        const string consultaRegistrar = """
            INSERT INTO dbo.Prestamos
                (UsuarioId, LibroId, FechaPrestamo, FechaDevolucionPrevista)
            VALUES
                (@UsuarioId, @LibroId, @FechaPrestamo, @FechaDevolucionPrevista);
            """;

        await using (var comandoPrestamo = new SqlCommand(consultaRegistrar, conexion, transaccion))
        {
            AgregarParametros(comandoPrestamo, prestamo);
            await comandoPrestamo.ExecuteNonQueryAsync(tokenCancelacion);
        }

        await transaccion.CommitAsync(tokenCancelacion);
    }

    public async Task ActualizarAsync(
        Prestamo prestamo,
        CancellationToken tokenCancelacion = default)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var transaccion =
            (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable, tokenCancelacion);

        int libroAnteriorId;
        const string consultaAnterior = """
            SELECT LibroId
            FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
            WHERE PrestamoId = @PrestamoId AND FechaDevolucionReal IS NULL;
            """;

        await using (var comandoAnterior = new SqlCommand(consultaAnterior, conexion, transaccion))
        {
            comandoAnterior.Parameters.Add("@PrestamoId", SqlDbType.Int).Value = prestamo.PrestamoId;
            await using var lector = await comandoAnterior.ExecuteReaderAsync(tokenCancelacion);
            if (!await lector.ReadAsync(tokenCancelacion))
            {
                throw new InvalidOperationException(
                    "No se encontró el préstamo activo que deseas actualizar.");
            }

            libroAnteriorId = lector.GetInt32(0);
        }

        if (libroAnteriorId != prestamo.LibroId)
        {
            await ActualizarExistenciasAsync(
                conexion,
                transaccion,
                libroAnteriorId,
                aumentarExistencias: true,
                tokenCancelacion);
            await ActualizarExistenciasAsync(
                conexion,
                transaccion,
                prestamo.LibroId,
                aumentarExistencias: false,
                tokenCancelacion);
        }

        const string consultaActualizar = """
            UPDATE dbo.Prestamos
            SET UsuarioId = @UsuarioId,
                LibroId = @LibroId,
                FechaPrestamo = @FechaPrestamo,
                FechaDevolucionPrevista = @FechaDevolucionPrevista
            WHERE PrestamoId = @PrestamoId AND FechaDevolucionReal IS NULL;
            """;

        await using (var comandoActualizar = new SqlCommand(consultaActualizar, conexion, transaccion))
        {
            comandoActualizar.Parameters.Add("@PrestamoId", SqlDbType.Int).Value = prestamo.PrestamoId;
            AgregarParametros(comandoActualizar, prestamo);

            if (await comandoActualizar.ExecuteNonQueryAsync(tokenCancelacion) != 1)
            {
                throw new InvalidOperationException("No se encontró el préstamo activo que deseas actualizar.");
            }
        }

        await transaccion.CommitAsync(tokenCancelacion);
    }

    public async Task RegistrarDevolucionAsync(
        int prestamoId,
        DateTime fechaDevolucionReal,
        CancellationToken tokenCancelacion = default)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var transaccion =
            (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable, tokenCancelacion);

        int libroId;
        DateTime fechaPrestamo;
        const string consultaPrestamo = """
            SELECT LibroId, FechaPrestamo
            FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
            WHERE PrestamoId = @PrestamoId AND FechaDevolucionReal IS NULL;
            """;

        await using (var comandoPrestamo = new SqlCommand(consultaPrestamo, conexion, transaccion))
        {
            comandoPrestamo.Parameters.Add("@PrestamoId", SqlDbType.Int).Value = prestamoId;
            await using var lector = await comandoPrestamo.ExecuteReaderAsync(tokenCancelacion);
            if (!await lector.ReadAsync(tokenCancelacion))
            {
                throw new InvalidOperationException(
                    "El préstamo no existe o ya fue devuelto.");
            }

            libroId = lector.GetInt32(0);
            fechaPrestamo = lector.GetDateTime(1);
        }

        if (fechaDevolucionReal.Date < fechaPrestamo.Date)
        {
            throw new InvalidOperationException(
                "La fecha de devolución no puede ser anterior a la fecha del préstamo.");
        }

        const string consultaDevolver = """
            UPDATE dbo.Prestamos
            SET FechaDevolucionReal = @FechaDevolucionReal
            WHERE PrestamoId = @PrestamoId AND FechaDevolucionReal IS NULL;
            """;

        await using (var comandoDevolver = new SqlCommand(consultaDevolver, conexion, transaccion))
        {
            comandoDevolver.Parameters.Add("@PrestamoId", SqlDbType.Int).Value = prestamoId;
            comandoDevolver.Parameters.Add("@FechaDevolucionReal", SqlDbType.Date).Value =
                fechaDevolucionReal.Date;
            if (await comandoDevolver.ExecuteNonQueryAsync(tokenCancelacion) != 1)
            {
                throw new InvalidOperationException("No se pudo actualizar el estado del préstamo.");
            }
        }

        await ActualizarExistenciasAsync(
            conexion,
            transaccion,
            libroId,
            aumentarExistencias: true,
            tokenCancelacion);
        await transaccion.CommitAsync(tokenCancelacion);
    }

    public async Task EliminarAsync(int prestamoId, CancellationToken tokenCancelacion = default)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var transaccion =
            (SqlTransaction)await conexion.BeginTransactionAsync(IsolationLevel.Serializable, tokenCancelacion);

        int libroId;
        bool yaFueDevuelto;
        const string consultaPrestamo = """
            SELECT LibroId, FechaDevolucionReal
            FROM dbo.Prestamos WITH (UPDLOCK, HOLDLOCK)
            WHERE PrestamoId = @PrestamoId;
            """;

        await using (var comandoPrestamo = new SqlCommand(consultaPrestamo, conexion, transaccion))
        {
            comandoPrestamo.Parameters.Add("@PrestamoId", SqlDbType.Int).Value = prestamoId;
            await using var lector = await comandoPrestamo.ExecuteReaderAsync(tokenCancelacion);
            if (!await lector.ReadAsync(tokenCancelacion))
            {
                throw new InvalidOperationException("No se encontró el préstamo que deseas eliminar.");
            }

            libroId = lector.GetInt32(0);
            yaFueDevuelto = !lector.IsDBNull(1);
        }

        if (!yaFueDevuelto)
        {
            await ActualizarExistenciasAsync(
                conexion,
                transaccion,
                libroId,
                aumentarExistencias: true,
                tokenCancelacion);
        }

        const string consultaEliminar = "DELETE FROM dbo.Prestamos WHERE PrestamoId = @PrestamoId;";
        await using (var comandoEliminar = new SqlCommand(consultaEliminar, conexion, transaccion))
        {
            comandoEliminar.Parameters.Add("@PrestamoId", SqlDbType.Int).Value = prestamoId;
            if (await comandoEliminar.ExecuteNonQueryAsync(tokenCancelacion) != 1)
            {
                throw new InvalidOperationException("No se pudo eliminar el préstamo.");
            }
        }

        await transaccion.CommitAsync(tokenCancelacion);
    }

    private static void AgregarParametros(SqlCommand comando, Prestamo prestamo)
    {
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = prestamo.UsuarioId;
        comando.Parameters.Add("@LibroId", SqlDbType.Int).Value = prestamo.LibroId;
        comando.Parameters.Add("@FechaPrestamo", SqlDbType.Date).Value = prestamo.FechaPrestamo.Date;
        comando.Parameters.Add("@FechaDevolucionPrevista", SqlDbType.Date).Value =
            prestamo.FechaDevolucionPrevista.Date;
    }

    private static async Task ActualizarExistenciasAsync(
        SqlConnection conexion,
        SqlTransaction transaccion,
        int libroId,
        bool aumentarExistencias,
        CancellationToken tokenCancelacion)
    {
        string consulta = aumentarExistencias
            ? """
              UPDATE dbo.Libros
              SET CantidadDisponible = CantidadDisponible + 1
              WHERE LibroId = @LibroId AND CantidadDisponible < CantidadTotal;
              """
            : """
              UPDATE dbo.Libros
              SET CantidadDisponible = CantidadDisponible - 1
              WHERE LibroId = @LibroId AND CantidadDisponible > 0;
              """;

        await using var comando = new SqlCommand(consulta, conexion, transaccion);
        comando.Parameters.Add("@LibroId", SqlDbType.Int).Value = libroId;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            string detalle = aumentarExistencias
                ? "No se pudo restaurar la existencia del libro."
                : "El libro no tiene ejemplares disponibles.";
            throw new InvalidOperationException(detalle);
        }
    }
}
