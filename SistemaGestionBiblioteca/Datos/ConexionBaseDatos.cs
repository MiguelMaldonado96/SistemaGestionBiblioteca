using System.Configuration;
using Microsoft.Data.SqlClient;

namespace SistemaGestionBiblioteca.Datos;

public sealed class ConexionBaseDatos
{
    public async Task ProbarConexionAsync(CancellationToken tokenCancelacion = default)
    {
        ConnectionStringSettings configuracionConexion =
            ConfigurationManager.ConnectionStrings["Biblioteca"]
            ?? throw new ConfigurationErrorsException(
                "No se encontró la cadena de conexión 'Biblioteca' en App.config.");

        await using var conexion = new SqlConnection(configuracionConexion.ConnectionString);
        await conexion.OpenAsync(tokenCancelacion);
    }
}
