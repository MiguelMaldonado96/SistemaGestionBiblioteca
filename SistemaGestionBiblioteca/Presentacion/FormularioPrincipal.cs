using System.Configuration;
using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;

namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Presenta el inicio, el menú y los accesos a los módulos de la biblioteca.
/// </summary>
public partial class FormularioPrincipal : Form
{
    private readonly ConexionBaseDatos conexionBaseDatos = new();
    private readonly Panel panelNavegacion = new();
    private readonly Panel panelContenido = new();
    private readonly Label etiquetaEstadoConexion = new();
    private readonly Button botonProbarConexion = new();

    public FormularioPrincipal()
    {
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = EstiloInterfazBiblioteca.fondoAplicacion;
        ClientSize = new Size(1180, 760);
        Font = new Font("Segoe UI", 9F);
        ForeColor = EstiloInterfazBiblioteca.tinta;
        MinimumSize = new Size(900, 600);
        Name = "FormularioPrincipal";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sistema de Gestión de Biblioteca";
        WindowState = FormWindowState.Maximized;

        ConstruirNavegacion();
        ConstruirPanelInicio();

        Controls.Add(panelContenido);
        Controls.Add(panelNavegacion);
    }

    private void ConstruirNavegacion()
    {
        panelNavegacion.BackColor = EstiloInterfazBiblioteca.azulOscuro;
        panelNavegacion.Dock = DockStyle.Left;
        panelNavegacion.Name = "panelNavegacion";
        panelNavegacion.Padding = new Padding(18, 22, 18, 18);
        panelNavegacion.Size = new Size(248, ClientSize.Height);

        var etiquetaMarca = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 18F),
            ForeColor = Color.White,
            Location = new Point(20, 24),
            Text = "BIBLIOTECA"
        };
        var etiquetaSubtituloMarca = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.FromArgb(190, 205, 225),
            Location = new Point(22, 67),
            Text = "Sistema de gestión"
        };
        var etiquetaMenu = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 8F),
            ForeColor = Color.FromArgb(147, 167, 193),
            Location = new Point(22, 112),
            Text = "MENÚ PRINCIPAL"
        };

        var panelOpciones = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            Location = new Point(16, 140),
            Name = "panelOpciones",
            Size = new Size(216, 410),
            WrapContents = false,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        panelOpciones.Controls.Add(CrearBotonNavegacion("Inicio", (_, _) => { }, true));
        panelOpciones.Controls.Add(CrearBotonNavegacion("Libros", BotonGestionarLibros_Click));
        panelOpciones.Controls.Add(CrearBotonNavegacion("Préstamos", BotonGestionarPrestamos_Click));
        panelOpciones.Controls.Add(CrearBotonNavegacion("Usuarios", BotonGestionarUsuarios_Click));
        panelOpciones.Controls.Add(CrearBotonNavegacion("Autores", BotonGestionarAutores_Click));
        panelOpciones.Controls.Add(CrearBotonNavegacion("Editoriales", BotonGestionarEditoriales_Click));
        panelOpciones.Controls.Add(CrearBotonNavegacion("Reportes", BotonGestionarReportes_Click));

        var etiquetaConexion = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 8F),
            ForeColor = Color.FromArgb(190, 205, 225),
            Location = new Point(22, 5),
            Text = "SQL SERVER"
        };
        var etiquetaServidor = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.White,
            Location = new Point(22, 25),
            Text = conexionBaseDatos.ObtenerServidorConfigurado()
        };
        var panelPieNavegacion = new Panel
        {
            BackColor = EstiloInterfazBiblioteca.azulOscuro,
            Dock = DockStyle.Bottom,
            Height = 60,
            Name = "panelPieNavegacion"
        };
        etiquetaEstadoConexion.Text = $"Servidor: {conexionBaseDatos.ObtenerServidorConfigurado()}";

        panelPieNavegacion.Controls.Add(etiquetaConexion);
        panelPieNavegacion.Controls.Add(etiquetaServidor);
        panelNavegacion.Controls.Add(etiquetaMarca);
        panelNavegacion.Controls.Add(etiquetaSubtituloMarca);
        panelNavegacion.Controls.Add(etiquetaMenu);
        panelNavegacion.Controls.Add(panelOpciones);
        panelNavegacion.Controls.Add(panelPieNavegacion);
    }

    private Button CrearBotonNavegacion(string texto, EventHandler accion, bool estaActivo = false)
    {
        var boton = new Button
        {
            BackColor = estaActivo
                ? Color.FromArgb(43, 69, 108)
                : EstiloInterfazBiblioteca.azulOscuro,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            Height = 46,
            Margin = new Padding(0, 0, 0, 5),
            Name = $"boton{texto.Replace(" ", string.Empty)}",
            Padding = new Padding(16, 0, 8, 0),
            Size = new Size(216, 46),
            Text = texto,
            TextAlign = ContentAlignment.MiddleLeft,
            UseVisualStyleBackColor = false
        };
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(43, 69, 108);
        boton.Click += accion;
        return boton;
    }

    private void ConstruirPanelInicio()
    {
        panelContenido.BackColor = EstiloInterfazBiblioteca.fondoAplicacion;
        panelContenido.Dock = DockStyle.Fill;
        panelContenido.Name = "panelContenido";
        panelContenido.Size = new Size(ClientSize.Width - panelNavegacion.Width, ClientSize.Height);

        var panelEncabezado = new Panel
        {
            BackColor = Color.White,
            Dock = DockStyle.Top,
            Height = 90,
            Name = "panelEncabezado",
            Width = panelContenido.ClientSize.Width
        };
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 19F),
            ForeColor = EstiloInterfazBiblioteca.tinta,
            Location = new Point(32, 12),
            Text = "Panel de inicio"
        };
        var etiquetaDescripcion = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            ForeColor = EstiloInterfazBiblioteca.tintaSuave,
            Location = new Point(34, 62),
            Text = "Administra la información y las operaciones de tu biblioteca."
        };

        etiquetaEstadoConexion.Font = new Font("Segoe UI Semibold", 9F);
        etiquetaEstadoConexion.ForeColor = EstiloInterfazBiblioteca.tintaSuave;
        etiquetaEstadoConexion.Margin = new Padding(0, 8, 18, 0);
        etiquetaEstadoConexion.Name = "etiquetaEstadoConexion";
        etiquetaEstadoConexion.Size = new Size(200, 20);
        etiquetaEstadoConexion.TextAlign = ContentAlignment.MiddleRight;

        botonProbarConexion.BackColor = EstiloInterfazBiblioteca.azul;
        botonProbarConexion.FlatStyle = FlatStyle.Flat;
        botonProbarConexion.FlatAppearance.BorderSize = 0;
        botonProbarConexion.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        botonProbarConexion.Font = new Font("Segoe UI Semibold", 9F);
        botonProbarConexion.ForeColor = Color.White;
        botonProbarConexion.Margin = Padding.Empty;
        botonProbarConexion.Name = "botonProbarConexion";
        botonProbarConexion.Size = new Size(170, 38);
        botonProbarConexion.Text = "Probar conexión";
        botonProbarConexion.UseVisualStyleBackColor = false;
        botonProbarConexion.Click += BotonProbarConexion_Click;

        var panelAccionesEncabezado = new FlowLayoutPanel
        {
            BackColor = Color.White,
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.LeftToRight,
            Name = "panelAccionesEncabezado",
            Padding = new Padding(0, 26, 22, 0),
            Size = new Size(420, 90),
            WrapContents = false
        };
        panelAccionesEncabezado.Controls.Add(etiquetaEstadoConexion);
        panelAccionesEncabezado.Controls.Add(botonProbarConexion);

        panelEncabezado.Controls.Add(etiquetaTitulo);
        panelEncabezado.Controls.Add(etiquetaDescripcion);
        panelEncabezado.Controls.Add(panelAccionesEncabezado);

        var panelCuerpo = new Panel
        {
            BackColor = EstiloInterfazBiblioteca.fondoAplicacion,
            Dock = DockStyle.Fill,
            Name = "panelCuerpo",
            AutoScroll = true,
            Size = new Size(panelContenido.ClientSize.Width, panelContenido.ClientSize.Height - panelEncabezado.Height)
        };
        ConstruirTarjetaBienvenida(panelCuerpo);
        ConstruirAccesosRapidos(panelCuerpo);

        panelContenido.Controls.Add(panelCuerpo);
        panelContenido.Controls.Add(panelEncabezado);
    }

    private void ConstruirTarjetaBienvenida(Panel contenedor)
    {
        var tarjeta = new Panel
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackColor = EstiloInterfazBiblioteca.azulOscuro,
            Location = new Point(32, 30),
            Name = "tarjetaBienvenida",
            Size = new Size(878, 210)
        };

        var etiquetaBienvenida = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 25F),
            ForeColor = Color.White,
            Location = new Point(28, 28),
            Text = "Tu biblioteca, en orden."
        };
        var etiquetaDetalle = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(210, 221, 237),
            Location = new Point(31, 88),
            Text = "Gestiona libros, personas, préstamos y devoluciones desde un solo lugar."
        };
        var botonAbrirPrestamos = new Button
        {
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9F),
            ForeColor = EstiloInterfazBiblioteca.azulOscuro,
            Location = new Point(31, 140),
            Name = "botonAbrirPrestamos",
            Size = new Size(190, 40),
            Text = "Ir a préstamos",
            UseVisualStyleBackColor = false
        };
        botonAbrirPrestamos.FlatAppearance.BorderSize = 0;
        botonAbrirPrestamos.Click += BotonGestionarPrestamos_Click;

        var etiquetaPrivacidad = new Label
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Font = new Font("Segoe UI Semibold", 8.5F),
            ForeColor = Color.FromArgb(210, 221, 237),
            Location = new Point(609, 165),
            Size = new Size(240, 24),
            Text = "Conexión local · Windows",
            TextAlign = ContentAlignment.MiddleRight
        };

        tarjeta.Controls.Add(etiquetaBienvenida);
        tarjeta.Controls.Add(etiquetaDetalle);
        tarjeta.Controls.Add(botonAbrirPrestamos);
        tarjeta.Controls.Add(etiquetaPrivacidad);
        contenedor.Controls.Add(tarjeta);
    }

    private void ConstruirAccesosRapidos(Panel contenedor)
    {
        var etiquetaAccesos = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 15F),
            ForeColor = EstiloInterfazBiblioteca.tinta,
            Location = new Point(32, 266),
            Text = "Accesos rápidos"
        };
        var etiquetaAccesosDetalle = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            ForeColor = EstiloInterfazBiblioteca.tintaSuave,
            Location = new Point(34, 302),
            Text = "Abre las tareas que realizas con más frecuencia."
        };
        var panelAccesos = new FlowLayoutPanel
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BackColor = EstiloInterfazBiblioteca.fondoAplicacion,
            FlowDirection = FlowDirection.LeftToRight,
            Location = new Point(32, 338),
            Name = "panelAccesos",
            Size = new Size(Math.Max(360, contenedor.ClientSize.Width - 54), 140),
            AutoScroll = true,
            WrapContents = true
        };

        panelAccesos.Controls.Add(CrearTarjetaAcceso("Libros", BotonGestionarLibros_Click));
        panelAccesos.Controls.Add(CrearTarjetaAcceso("Préstamos", BotonGestionarPrestamos_Click));
        panelAccesos.Controls.Add(CrearTarjetaAcceso("Usuarios", BotonGestionarUsuarios_Click));
        panelAccesos.Controls.Add(CrearTarjetaAcceso("Reportes", BotonGestionarReportes_Click));

        contenedor.Controls.Add(etiquetaAccesos);
        contenedor.Controls.Add(etiquetaAccesosDetalle);
        contenedor.Controls.Add(panelAccesos);
    }

    private static Button CrearTarjetaAcceso(string titulo, EventHandler accion)
    {
        var boton = new Button
        {
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 12F),
            ForeColor = EstiloInterfazBiblioteca.tinta,
            Margin = new Padding(0, 0, 14, 14),
            Padding = new Padding(18),
            Size = new Size(200, 100),
            Text = titulo,
            TextAlign = ContentAlignment.MiddleLeft,
            UseVisualStyleBackColor = false
        };
        boton.FlatAppearance.BorderColor = EstiloInterfazBiblioteca.borde;
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.MouseOverBackColor = EstiloInterfazBiblioteca.azulClaro;
        boton.Cursor = Cursors.Hand;
        boton.Click += accion;
        return boton;
    }

    private void BotonGestionarAutores_Click(object? remitente, EventArgs argumentos)
    {
        using var formularioAutores = new FormularioAutores();
        formularioAutores.ShowDialog(this);
    }

    private void BotonGestionarEditoriales_Click(object? remitente, EventArgs argumentos)
    {
        using var formularioEditoriales = new FormularioEditoriales();
        formularioEditoriales.ShowDialog(this);
    }

    private void BotonGestionarUsuarios_Click(object? remitente, EventArgs argumentos)
    {
        using var formularioUsuarios = new FormularioUsuarios();
        formularioUsuarios.ShowDialog(this);
    }

    private void BotonGestionarLibros_Click(object? remitente, EventArgs argumentos)
    {
        using var formularioLibros = new FormularioLibros();
        formularioLibros.ShowDialog(this);
    }

    private void BotonGestionarPrestamos_Click(object? remitente, EventArgs argumentos)
    {
        using var formularioPrestamos = new FormularioPrestamos();
        formularioPrestamos.ShowDialog(this);
    }

    private void BotonGestionarReportes_Click(object? remitente, EventArgs argumentos)
    {
        using var formularioReportes = new FormularioReportes();
        formularioReportes.ShowDialog(this);
    }

    private async void BotonProbarConexion_Click(object? remitente, EventArgs argumentos)
    {
        botonProbarConexion.Enabled = false;
        etiquetaEstadoConexion.ForeColor = EstiloInterfazBiblioteca.tintaSuave;
        etiquetaEstadoConexion.Text = "Conectando...";

        try
        {
            await conexionBaseDatos.ProbarConexionAsync();
            etiquetaEstadoConexion.ForeColor = EstiloInterfazBiblioteca.verde;
            etiquetaEstadoConexion.Text =
                $"Conectado · {conexionBaseDatos.ObtenerServidorConfigurado()}";
        }
        catch (SqlException errorSql)
        {
            etiquetaEstadoConexion.ForeColor = EstiloInterfazBiblioteca.rojo;
            etiquetaEstadoConexion.Text = "Sin conexión a SQL Server";
            MessageBox.Show(
                this,
                $"No fue posible conectarse a «{conexionBaseDatos.ObtenerServidorConfigurado()}».{Environment.NewLine}{Environment.NewLine}" +
                errorSql.Message,
                "Error de conexión",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (ConfigurationErrorsException errorConfiguracion)
        {
            etiquetaEstadoConexion.ForeColor = EstiloInterfazBiblioteca.rojo;
            etiquetaEstadoConexion.Text = "Configuración no válida";
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
