using System.Configuration;
using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;

namespace SistemaGestionBiblioteca.Presentacion;

public partial class FormularioPrincipal : Form
{
    private readonly ConexionBaseDatos conexionBaseDatos = new();

    public FormularioPrincipal()
    {
        InitializeComponent();
        botonProbarConexion.Click += BotonProbarConexion_Click;
    }

    private async void BotonProbarConexion_Click(object? remitente, EventArgs argumentos)
    {
        botonProbarConexion.Enabled = false;
        etiquetaEstadoConexion.Text = "Conectando a .\\SQL2022...";

        try
        {
            await conexionBaseDatos.ProbarConexionAsync();
            etiquetaEstadoConexion.Text = "Conexión exitosa con SQL Server.";
            MessageBox.Show(
                this,
                "La conexión con la base de datos se realizó correctamente.",
                "Conexión establecida",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (SqlException errorSql)
        {
            etiquetaEstadoConexion.Text = "No fue posible conectar con SQL Server.";
            MessageBox.Show(
                this,
                $"No se pudo conectar con .\\SQL2022.{Environment.NewLine}{Environment.NewLine}" +
                $"{errorSql.Message}{Environment.NewLine}{Environment.NewLine}" +
                "Verifica que el motor de SQL Server esté instalado y que el servicio " +
                "de la instancia SQL2022 esté iniciado.",
                "Error de conexión",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (ConfigurationErrorsException errorConfiguracion)
        {
            etiquetaEstadoConexion.Text = "La configuración de la conexión es incorrecta.";
            MessageBox.Show(
                this,
                errorConfiguracion.Message,
                "Error de configuración",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            botonProbarConexion.Enabled = true;
        }
    }
}
