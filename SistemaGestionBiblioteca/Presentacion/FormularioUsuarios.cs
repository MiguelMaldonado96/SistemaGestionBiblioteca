using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;
using SistemaGestionBiblioteca.Negocio;

namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Permite consultar y administrar las personas usuarias de la biblioteca.
/// </summary>
public sealed class FormularioUsuarios : Form
{
    private readonly UsuarioServicio usuarioServicio =
        new(new UsuarioRepositorio(new ConexionBaseDatos()));
    private readonly DataGridView cuadriculaUsuarios = new();
    private readonly TextBox campoNombre = new();
    private readonly TextBox campoApellido = new();
    private readonly TextBox campoDocumento = new();
    private readonly TextBox campoTelefono = new();
    private readonly TextBox campoCorreo = new();
    private readonly Button botonGuardar = new();
    private readonly Button botonActualizar = new();
    private readonly Button botonEliminar = new();
    private readonly Label etiquetaEstado = new();
    private int? usuarioIdSeleccionado;

    public FormularioUsuarios()
    {
        ConfigurarVentana();
        ConstruirInterfaz();
        Load += FormularioUsuarios_Load;
    }

    private void ConfigurarVentana()
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 640);
        MinimumSize = new Size(700, 560);
        Name = "FormularioUsuarios";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gestión de usuarios";
    }

    private void ConstruirInterfaz()
    {
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(24, 20),
            Text = "Usuarios"
        };

        var etiquetaNombre = CrearEtiqueta("Nombre", 24, 68);
        PrepararCampo(campoNombre, "campoNombre", 24, 92, 320, 80);

        var etiquetaApellido = CrearEtiqueta("Apellido", 366, 68);
        PrepararCampo(campoApellido, "campoApellido", 366, 92, 320, 80);

        var etiquetaDocumento = CrearEtiqueta("Documento", 24, 134);
        PrepararCampo(campoDocumento, "campoDocumento", 24, 158, 320, 30);

        var etiquetaTelefono = CrearEtiqueta("Teléfono (opcional)", 366, 134);
        PrepararCampo(campoTelefono, "campoTelefono", 366, 158, 320, 30);

        var etiquetaCorreo = CrearEtiqueta("Correo electrónico (opcional)", 24, 200);
        PrepararCampo(campoCorreo, "campoCorreo", 24, 224, 320, 254);

        botonGuardar.Location = new Point(24, 270);
        botonGuardar.Name = "botonGuardar";
        botonGuardar.Size = new Size(120, 34);
        botonGuardar.Text = "Guardar";
        botonGuardar.Click += BotonGuardar_Click;

        botonActualizar.Location = new Point(156, 270);
        botonActualizar.Name = "botonActualizar";
        botonActualizar.Size = new Size(120, 34);
        botonActualizar.Text = "Actualizar";
        botonActualizar.Enabled = false;
        botonActualizar.Click += BotonActualizar_Click;

        botonEliminar.Location = new Point(288, 270);
        botonEliminar.Name = "botonEliminar";
        botonEliminar.Size = new Size(120, 34);
        botonEliminar.Text = "Eliminar";
        botonEliminar.Enabled = false;
        botonEliminar.Click += BotonEliminar_Click;

        var botonLimpiar = new Button
        {
            Location = new Point(420, 270),
            Name = "botonLimpiar",
            Size = new Size(120, 34),
            Text = "Limpiar"
        };
        botonLimpiar.Click += (_, _) => LimpiarSeleccion();

        etiquetaEstado.AutoSize = true;
        etiquetaEstado.Location = new Point(24, 320);
        etiquetaEstado.Name = "etiquetaEstado";
        etiquetaEstado.Text = "Selecciona un usuario para editarlo o eliminarlo.";

        cuadriculaUsuarios.AllowUserToAddRows = false;
        cuadriculaUsuarios.AllowUserToDeleteRows = false;
        cuadriculaUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        cuadriculaUsuarios.Location = new Point(24, 350);
        cuadriculaUsuarios.MultiSelect = false;
        cuadriculaUsuarios.Name = "cuadriculaUsuarios";
        cuadriculaUsuarios.ReadOnly = true;
        cuadriculaUsuarios.RowHeadersVisible = false;
        cuadriculaUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        cuadriculaUsuarios.Size = new Size(740, 256);
        cuadriculaUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cuadriculaUsuarios.SelectionChanged += CuadriculaUsuarios_SelectionChanged;

        Controls.AddRange(
        [
            etiquetaTitulo,
            etiquetaNombre,
            campoNombre,
            etiquetaApellido,
            campoApellido,
            etiquetaDocumento,
            campoDocumento,
            etiquetaTelefono,
            campoTelefono,
            etiquetaCorreo,
            campoCorreo,
            botonGuardar,
            botonActualizar,
            botonEliminar,
            botonLimpiar,
            etiquetaEstado,
            cuadriculaUsuarios
        ]);
        EstiloInterfazBiblioteca.Aplicar(this, cuadriculaUsuarios);
    }

    private static Label CrearEtiqueta(string texto, int posicionX, int posicionY)
    {
        return new Label
        {
            AutoSize = true,
            Location = new Point(posicionX, posicionY),
            Text = texto
        };
    }

    private static void PrepararCampo(
        TextBox campo,
        string nombreControl,
        int posicionX,
        int posicionY,
        int ancho,
        int longitudMaxima)
    {
        campo.Location = new Point(posicionX, posicionY);
        campo.MaxLength = longitudMaxima;
        campo.Name = nombreControl;
        campo.Size = new Size(ancho, 27);
    }

    private async void FormularioUsuarios_Load(object? remitente, EventArgs argumentos)
    {
        await CargarUsuariosAsync();
    }

    private async void BotonGuardar_Click(object? remitente, EventArgs argumentos)
    {
        try
        {
            await usuarioServicio.CrearAsync(
                campoNombre.Text,
                campoApellido.Text,
                campoDocumento.Text,
                campoTelefono.Text,
                campoCorreo.Text);
            await CargarUsuariosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El usuario se guardó correctamente.");
        }
        catch (ArgumentException errorValidacion)
        {
            MostrarError(errorValidacion.Message);
        }
        catch (SqlException errorSql)
        {
            MostrarErrorSql(errorSql, "guardar");
        }
    }

    private async void BotonActualizar_Click(object? remitente, EventArgs argumentos)
    {
        if (usuarioIdSeleccionado is not int usuarioId)
        {
            MostrarError("Selecciona un usuario de la lista.");
            return;
        }

        try
        {
            await usuarioServicio.ActualizarAsync(
                usuarioId,
                campoNombre.Text,
                campoApellido.Text,
                campoDocumento.Text,
                campoTelefono.Text,
                campoCorreo.Text);
            await CargarUsuariosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El usuario se actualizó correctamente.");
        }
        catch (ArgumentException errorValidacion)
        {
            MostrarError(errorValidacion.Message);
        }
        catch (InvalidOperationException errorOperacion)
        {
            MostrarError(errorOperacion.Message);
        }
        catch (SqlException errorSql)
        {
            MostrarErrorSql(errorSql, "actualizar");
        }
    }

    private async void BotonEliminar_Click(object? remitente, EventArgs argumentos)
    {
        if (usuarioIdSeleccionado is not int usuarioId)
        {
            MostrarError("Selecciona un usuario de la lista.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            this,
            "¿Deseas eliminar el usuario seleccionado?",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await usuarioServicio.EliminarAsync(usuarioId);
            await CargarUsuariosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El usuario se eliminó correctamente.");
        }
        catch (ArgumentException errorValidacion)
        {
            MostrarError(errorValidacion.Message);
        }
        catch (InvalidOperationException errorOperacion)
        {
            MostrarError(errorOperacion.Message);
        }
        catch (SqlException errorSql)
        {
            MostrarErrorSql(errorSql, "eliminar");
        }
    }

    private async Task CargarUsuariosAsync()
    {
        try
        {
            List<Usuario> usuarios = await usuarioServicio.ObtenerTodosAsync();
            cuadriculaUsuarios.DataSource = usuarios;
            etiquetaEstado.Text = $"Usuarios registrados: {usuarios.Count}.";

            if (cuadriculaUsuarios.Columns[nameof(Usuario.UsuarioId)] is DataGridViewColumn columnaId)
            {
                columnaId.HeaderText = "Código";
            }

            if (cuadriculaUsuarios.Columns[nameof(Usuario.Nombre)] is DataGridViewColumn columnaNombre)
            {
                columnaNombre.HeaderText = "Nombre";
            }

            if (cuadriculaUsuarios.Columns[nameof(Usuario.Apellido)] is DataGridViewColumn columnaApellido)
            {
                columnaApellido.HeaderText = "Apellido";
            }

            if (cuadriculaUsuarios.Columns[nameof(Usuario.Documento)] is DataGridViewColumn columnaDocumento)
            {
                columnaDocumento.HeaderText = "Documento";
            }

            if (cuadriculaUsuarios.Columns[nameof(Usuario.Telefono)] is DataGridViewColumn columnaTelefono)
            {
                columnaTelefono.HeaderText = "Teléfono";
            }

            if (cuadriculaUsuarios.Columns[nameof(Usuario.Correo)] is DataGridViewColumn columnaCorreo)
            {
                columnaCorreo.HeaderText = "Correo";
            }

            cuadriculaUsuarios.ClearSelection();
            usuarioIdSeleccionado = null;
            botonActualizar.Enabled = false;
            botonEliminar.Enabled = false;
            botonGuardar.Enabled = true;
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible consultar los usuarios.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private void CuadriculaUsuarios_SelectionChanged(object? remitente, EventArgs argumentos)
    {
        if (cuadriculaUsuarios.SelectedRows.Count != 1 ||
            cuadriculaUsuarios.SelectedRows[0].DataBoundItem is not Usuario usuario)
        {
            return;
        }

        usuarioIdSeleccionado = usuario.UsuarioId;
        campoNombre.Text = usuario.Nombre;
        campoApellido.Text = usuario.Apellido;
        campoDocumento.Text = usuario.Documento;
        campoTelefono.Text = usuario.Telefono ?? string.Empty;
        campoCorreo.Text = usuario.Correo ?? string.Empty;
        botonActualizar.Enabled = true;
        botonEliminar.Enabled = true;
        botonGuardar.Enabled = false;
    }

    private void LimpiarSeleccion()
    {
        usuarioIdSeleccionado = null;
        campoNombre.Clear();
        campoApellido.Clear();
        campoDocumento.Clear();
        campoTelefono.Clear();
        campoCorreo.Clear();
        cuadriculaUsuarios.ClearSelection();
        botonActualizar.Enabled = false;
        botonEliminar.Enabled = false;
        botonGuardar.Enabled = true;
        etiquetaEstado.Text = "Selecciona un usuario para editarlo o eliminarlo.";
    }

    private void MostrarErrorSql(SqlException errorSql, string operacion)
    {
        if (errorSql.Number is 2601 or 2627)
        {
            MostrarError("El documento o correo ya está registrado.");
            return;
        }

        MostrarError($"No se pudo {operacion} el usuario.{Environment.NewLine}{errorSql.Message}");
    }

    private void MostrarMensaje(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MostrarError(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
