using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Datos;

public sealed class AutorRepositorio
{
    private readonly ConexionBaseDatos conexionBaseDatos;

    public AutorRepositorio(ConexionBaseDatos conexionBaseDatos)
    {
        this.conexionBaseDatos = conexionBaseDatos;
    }

    public async Task<List<Autor>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT AutorId, Nombre, Apellido
            FROM dbo.Autores
            ORDER BY Apellido, Nombre;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(tokenCancelacion);

        var autores = new List<Autor>();
        while (await lector.ReadAsync(tokenCancelacion))
        {
            autores.Add(new Autor
            {
                AutorId = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Apellido = lector.GetString(2)
            });
        }

        return autores;
    }

    public async Task CrearAsync(Autor autor, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            INSERT INTO dbo.Autores (Nombre, Apellido)
            VALUES (@Nombre, @Apellido);
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 80).Value = autor.Nombre;
        comando.Parameters.Add("@Apellido", System.Data.SqlDbType.NVarChar, 80).Value = autor.Apellido;
        await comando.ExecuteNonQueryAsync(tokenCancelacion);
    }

    public async Task ActualizarAsync(Autor autor, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            UPDATE dbo.Autores
            SET Nombre = @Nombre, Apellido = @Apellido
            WHERE AutorId = @AutorId;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@AutorId", System.Data.SqlDbType.Int).Value = autor.AutorId;
        comando.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 80).Value = autor.Nombre;
        comando.Parameters.Add("@Apellido", System.Data.SqlDbType.NVarChar, 80).Value = autor.Apellido;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException("No se encontró el autor que deseas actualizar.");
        }
    }

    public async Task EliminarAsync(int autorId, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            DELETE FROM dbo.Autores
            WHERE AutorId = @AutorId
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.LibroAutores
                  WHERE AutorId = @AutorId
              );
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@AutorId", System.Data.SqlDbType.Int).Value = autorId;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException(
                "No se puede eliminar el autor: no existe o está relacionado con uno o más libros.");
        }
    }
}
