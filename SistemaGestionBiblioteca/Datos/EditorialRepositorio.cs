using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Datos;

public sealed class EditorialRepositorio
{
    private readonly ConexionBaseDatos conexionBaseDatos;

    public EditorialRepositorio(ConexionBaseDatos conexionBaseDatos)
    {
        this.conexionBaseDatos = conexionBaseDatos;
    }

    public async Task<List<Editorial>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT EditorialId, Nombre, Pais
            FROM dbo.Editoriales
            ORDER BY Nombre;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(tokenCancelacion);

        var editoriales = new List<Editorial>();
        while (await lector.ReadAsync(tokenCancelacion))
        {
            editoriales.Add(new Editorial
            {
                EditorialId = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Pais = lector.IsDBNull(2) ? null : lector.GetString(2)
            });
        }

        return editoriales;
    }

    public async Task CrearAsync(Editorial editorial, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            INSERT INTO dbo.Editoriales (Nombre, Pais)
            VALUES (@Nombre, @Pais);
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 120).Value = editorial.Nombre;
        comando.Parameters.Add("@Pais", System.Data.SqlDbType.NVarChar, 80).Value =
            (object?)editorial.Pais ?? DBNull.Value;
        await comando.ExecuteNonQueryAsync(tokenCancelacion);
    }

    public async Task ActualizarAsync(Editorial editorial, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            UPDATE dbo.Editoriales
            SET Nombre = @Nombre, Pais = @Pais
            WHERE EditorialId = @EditorialId;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@EditorialId", System.Data.SqlDbType.Int).Value = editorial.EditorialId;
        comando.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 120).Value = editorial.Nombre;
        comando.Parameters.Add("@Pais", System.Data.SqlDbType.NVarChar, 80).Value =
            (object?)editorial.Pais ?? DBNull.Value;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException("No se encontró la editorial que deseas actualizar.");
        }
    }

    public async Task EliminarAsync(int editorialId, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            DELETE FROM dbo.Editoriales
            WHERE EditorialId = @EditorialId
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.Libros
                  WHERE EditorialId = @EditorialId
              );
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@EditorialId", System.Data.SqlDbType.Int).Value = editorialId;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException(
                "No se puede eliminar la editorial: no existe o está relacionada con uno o más libros.");
        }
    }
}
