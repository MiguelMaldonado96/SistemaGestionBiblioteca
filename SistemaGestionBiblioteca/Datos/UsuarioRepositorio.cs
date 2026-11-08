using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Datos;

/// <summary>
/// Lee y guarda usuarios en la base de datos.
/// </summary>
public sealed class UsuarioRepositorio
{
    private readonly ConexionBaseDatos conexionBaseDatos;

    /// <summary>Crea el repositorio con el servicio de conexiones configurado.</summary>
    public UsuarioRepositorio(ConexionBaseDatos conexionBaseDatos)
    {
        this.conexionBaseDatos = conexionBaseDatos;
    }

    /// <summary>Devuelve los usuarios ordenados por apellido y nombre.</summary>
    public async Task<List<Usuario>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT UsuarioId, Nombre, Apellido, Documento, Telefono, Correo
            FROM dbo.Usuarios
            ORDER BY Apellido, Nombre;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(tokenCancelacion);

        var usuarios = new List<Usuario>();
        while (await lector.ReadAsync(tokenCancelacion))
        {
            usuarios.Add(new Usuario
            {
                UsuarioId = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Apellido = lector.GetString(2),
                Documento = lector.GetString(3),
                Telefono = lector.IsDBNull(4) ? null : lector.GetString(4),
                Correo = lector.IsDBNull(5) ? null : lector.GetString(5)
            });
        }

        return usuarios;
    }

    /// <summary>Inserta un usuario nuevo.</summary>
    public async Task CrearAsync(Usuario usuario, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            INSERT INTO dbo.Usuarios (Nombre, Apellido, Documento, Telefono, Correo)
            VALUES (@Nombre, @Apellido, @Documento, @Telefono, @Correo);
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        AgregarParametros(comando, usuario);
        await comando.ExecuteNonQueryAsync(tokenCancelacion);
    }

    /// <summary>Guarda los cambios del usuario indicado.</summary>
    public async Task ActualizarAsync(Usuario usuario, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            UPDATE dbo.Usuarios
            SET Nombre = @Nombre,
                Apellido = @Apellido,
                Documento = @Documento,
                Telefono = @Telefono,
                Correo = @Correo
            WHERE UsuarioId = @UsuarioId;
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@UsuarioId", System.Data.SqlDbType.Int).Value = usuario.UsuarioId;
        AgregarParametros(comando, usuario);

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException("No se encontró el usuario que deseas actualizar.");
        }
    }

    /// <summary>Elimina al usuario solo cuando no tiene préstamos relacionados.</summary>
    public async Task EliminarAsync(int usuarioId, CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            DELETE FROM dbo.Usuarios
            WHERE UsuarioId = @UsuarioId
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM dbo.Prestamos
                  WHERE UsuarioId = @UsuarioId
              );
            """;

        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.Add("@UsuarioId", System.Data.SqlDbType.Int).Value = usuarioId;

        if (await comando.ExecuteNonQueryAsync(tokenCancelacion) != 1)
        {
            throw new InvalidOperationException(
                "No se puede eliminar el usuario: no existe o tiene préstamos registrados.");
        }
    }

    /// <summary>Agrega los datos del usuario a la consulta SQL como parámetros.</summary>
    private static void AgregarParametros(SqlCommand comando, Usuario usuario)
    {
        comando.Parameters.Add("@Nombre", System.Data.SqlDbType.NVarChar, 80).Value = usuario.Nombre;
        comando.Parameters.Add("@Apellido", System.Data.SqlDbType.NVarChar, 80).Value = usuario.Apellido;
        comando.Parameters.Add("@Documento", System.Data.SqlDbType.NVarChar, 30).Value = usuario.Documento;
        comando.Parameters.Add("@Telefono", System.Data.SqlDbType.NVarChar, 30).Value =
            (object?)usuario.Telefono ?? DBNull.Value;
        comando.Parameters.Add("@Correo", System.Data.SqlDbType.NVarChar, 254).Value =
            (object?)usuario.Correo ?? DBNull.Value;
    }
}
