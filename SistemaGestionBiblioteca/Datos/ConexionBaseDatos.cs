using System.Configuration;
using Microsoft.Data.SqlClient;

namespace SistemaGestionBiblioteca.Datos;

/// <summary>
/// Crea conexiones a la base de datos usando la configuración de App.config.
/// </summary>
public sealed class ConexionBaseDatos
{
    /// <summary>Crea una conexión nueva que todavía no está abierta.</summary>
    public SqlConnection CrearConexion()
    {
        return new SqlConnection(ObtenerCadenaConexion());
    }

    /// <summary>Indica qué servidor está configurado para la aplicación.</summary>
    public string ObtenerServidorConfigurado()
    {
        return new SqlConnectionStringBuilder(ObtenerCadenaConexion()).DataSource;
    }

    /// <summary>Lee la cadena Biblioteca y avisa si falta la configuración.</summary>
    private static string ObtenerCadenaConexion()
    {
        ConnectionStringSettings configuracionConexion =
            ConfigurationManager.ConnectionStrings["Biblioteca"]
            ?? throw new ConfigurationErrorsException(
                "No se encontró la cadena de conexión 'Biblioteca' en App.config.");

        return configuracionConexion.ConnectionString;
    }

    /// <summary>Abre una conexión para comprobar que la base de datos responde.</summary>
    public async Task ProbarConexionAsync(CancellationToken tokenCancelacion = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
    }
}
