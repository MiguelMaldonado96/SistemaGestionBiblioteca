using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Negocio;

namespace SistemaGestionBiblioteca.Presentacion;

public sealed class FormularioReportes : Form
{
    private readonly ReporteServicio reporteServicio =
        new(new ReporteRepositorio(new ConexionBaseDatos()));
    private readonly DataGridView cuadriculaResumen = CrearCuadricula();
    private readonly DataGridView cuadriculaLibros = CrearCuadricula();
    private readonly DataGridView cuadriculaAutores = CrearCuadricula();
    private readonly DataGridView cuadriculaEditoriales = CrearCuadricula();
    private readonly DataGridView cuadriculaUsuarios = CrearCuadricula();
    private readonly DataGridView cuadriculaPrestamosActivos = CrearCuadricula();
    private readonly DataGridView cuadriculaHistorialDevoluciones = CrearCuadricula();
    private readonly Button botonActualizar = new();
    private readonly Label etiquetaEstado = new();

    public FormularioReportes()
    {
        ConfigurarVentana();
        ConstruirInterfaz();
        Load += FormularioReportes_Load;
    }

    private void ConfigurarVentana()
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 700);
        MinimumSize = new Size(840, 560);
        Name = "FormularioReportes";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Consultas y reportes";
    }

    private void ConstruirInterfaz()
    {
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(24, 18),
            Text = "Consultas y reportes"
        };

        botonActualizar.Location = new Point(24, 48);
        botonActualizar.Name = "botonActualizar";
        botonActualizar.Size = new Size(140, 32);
        botonActualizar.Text = "Actualizar reportes";
        botonActualizar.Click += BotonActualizar_Click;

        etiquetaEstado.AutoSize = true;
        etiquetaEstado.Location = new Point(182, 57);
        etiquetaEstado.Name = "etiquetaEstado";
        etiquetaEstado.Text = "Información consolidada de la biblioteca.";

        var pestanasReportes = new TabControl
        {
            Location = new Point(24, 92),
            Name = "pestanasReportes",
            Size = new Size(952, 570),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        pestanasReportes.TabPages.Add(CrearPestana("Resumen", cuadriculaResumen));
        pestanasReportes.TabPages.Add(CrearPestana("Libros", cuadriculaLibros));
        pestanasReportes.TabPages.Add(CrearPestana("Autores", cuadriculaAutores));
        pestanasReportes.TabPages.Add(CrearPestana("Editoriales", cuadriculaEditoriales));
        pestanasReportes.TabPages.Add(CrearPestana("Usuarios", cuadriculaUsuarios));
        pestanasReportes.TabPages.Add(CrearPestana("Préstamos activos", cuadriculaPrestamosActivos));
        pestanasReportes.TabPages.Add(CrearPestana("Historial de devoluciones", cuadriculaHistorialDevoluciones));

        Controls.Add(etiquetaTitulo);
        Controls.Add(botonActualizar);
        Controls.Add(etiquetaEstado);
        Controls.Add(pestanasReportes);
    }

    private static DataGridView CrearCuadricula()
    {
        return new DataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
    }

    private static TabPage CrearPestana(string titulo, DataGridView cuadricula)
    {
        var pestana = new TabPage(titulo);
        pestana.Controls.Add(cuadricula);
        return pestana;
    }

    private async void FormularioReportes_Load(object? remitente, EventArgs argumentos)
    {
        await CargarReportesAsync();
    }

    private async void BotonActualizar_Click(object? remitente, EventArgs argumentos)
    {
        await CargarReportesAsync();
    }

    private async Task CargarReportesAsync()
    {
        botonActualizar.Enabled = false;
        etiquetaEstado.Text = "Actualizando reportes...";

        try
        {
            cuadriculaResumen.DataSource = await reporteServicio.ObtenerResumenAsync();
            cuadriculaLibros.DataSource = await reporteServicio.ObtenerLibrosAsync();
            cuadriculaAutores.DataSource = await reporteServicio.ObtenerAutoresAsync();
            cuadriculaEditoriales.DataSource = await reporteServicio.ObtenerEditorialesAsync();
            cuadriculaUsuarios.DataSource = await reporteServicio.ObtenerUsuariosAsync();
            cuadriculaPrestamosActivos.DataSource =
                await reporteServicio.ObtenerPrestamosActivosAsync();
            cuadriculaHistorialDevoluciones.DataSource =
                await reporteServicio.ObtenerHistorialDevolucionesAsync();

            etiquetaEstado.Text = $"Reportes actualizados: {DateTime.Now:dd/MM/yyyy HH:mm}.";
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible actualizar los reportes.";
            MessageBox.Show(
                this,
                $"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}",
                "Error en reportes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            botonActualizar.Enabled = true;
        }
    }
}
