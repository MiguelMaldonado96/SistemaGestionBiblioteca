using System.Configuration;
using Microsoft.Data.SqlClient;

namespace SistemaGestionBiblioteca.Datos;

public sealed class ConexionBaseDatos
{
    public SqlConnection CrearConexion()
    {
        ConnectionStringSettings configuracionConexion =
            ConfigurationManager.ConnectionStrings["Biblioteca"]
            ?? throw new ConfigurationErrorsException(
                "No se encontró la cadena de conexión 'Biblioteca' en App.config.");

        return new SqlConnection(configuracionConexion.ConnectionString);
    }

    public async Task ProbarConexionAsync(CancellationToken tokenCancelacion = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
    }
}
