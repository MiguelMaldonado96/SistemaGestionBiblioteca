using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;
using SistemaGestionBiblioteca.Negocio;

namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Permite consultar y administrar los autores registrados.
/// </summary>
public sealed class FormularioAutores : Form
{
    private readonly AutorServicio autorServicio = new(new AutorRepositorio(new ConexionBaseDatos()));
    private readonly DataGridView cuadriculaAutores = new();
    private readonly TextBox campoNombre = new();
    private readonly TextBox campoApellido = new();
    private readonly Button botonGuardar = new();
    private readonly Button botonActualizar = new();
    private readonly Button botonEliminar = new();
    private readonly Label etiquetaEstado = new();
    private int? autorIdSeleccionado;

    public FormularioAutores()
    {
        ConfigurarVentana();
        ConstruirInterfaz();
        Load += FormularioAutores_Load;
    }

    private void ConfigurarVentana()
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 540);
        MinimumSize = new Size(700, 480);
        Name = "FormularioAutores";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gestión de autores";
    }

    private void ConstruirInterfaz()
    {
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(24, 20),
            Text = "Autores"
        };

        var etiquetaNombre = CrearEtiqueta("Nombre", 24, 68);
        campoNombre.Location = new Point(24, 94);
        campoNombre.MaxLength = 80;
        campoNombre.Name = "campoNombre";
        campoNombre.Size = new Size(320, 27);

        var etiquetaApellido = CrearEtiqueta("Apellido", 366, 68);
        campoApellido.Location = new Point(366, 94);
        campoApellido.MaxLength = 80;
        campoApellido.Name = "campoApellido";
        campoApellido.Size = new Size(320, 27);

        botonGuardar.Location = new Point(24, 142);
        botonGuardar.Name = "botonGuardar";
        botonGuardar.Size = new Size(120, 34);
        botonGuardar.Text = "Guardar";
        botonGuardar.Click += BotonGuardar_Click;

        botonActualizar.Location = new Point(156, 142);
        botonActualizar.Name = "botonActualizar";
        botonActualizar.Size = new Size(120, 34);
        botonActualizar.Text = "Actualizar";
        botonActualizar.Enabled = false;
        botonActualizar.Click += BotonActualizar_Click;

        botonEliminar.Location = new Point(288, 142);
        botonEliminar.Name = "botonEliminar";
        botonEliminar.Size = new Size(120, 34);
        botonEliminar.Text = "Eliminar";
        botonEliminar.Enabled = false;
        botonEliminar.Click += BotonEliminar_Click;

        var botonLimpiar = new Button
        {
            Location = new Point(420, 142),
            Name = "botonLimpiar",
            Size = new Size(120, 34),
            Text = "Limpiar"
        };
        botonLimpiar.Click += (_, _) => LimpiarSeleccion();

        etiquetaEstado.AutoSize = true;
        etiquetaEstado.Location = new Point(24, 192);
        etiquetaEstado.Name = "etiquetaEstado";
        etiquetaEstado.Text = "Selecciona un autor de la lista para editarlo o eliminarlo.";

        cuadriculaAutores.AllowUserToAddRows = false;
        cuadriculaAutores.AllowUserToDeleteRows = false;
        cuadriculaAutores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        cuadriculaAutores.Location = new Point(24, 222);
        cuadriculaAutores.MultiSelect = false;
        cuadriculaAutores.Name = "cuadriculaAutores";
        cuadriculaAutores.ReadOnly = true;
        cuadriculaAutores.RowHeadersVisible = false;
        cuadriculaAutores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        cuadriculaAutores.Size = new Size(740, 288);
        cuadriculaAutores.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cuadriculaAutores.SelectionChanged += CuadriculaAutores_SelectionChanged;

        Controls.AddRange(
        [
            etiquetaTitulo,
            etiquetaNombre,
            campoNombre,
            etiquetaApellido,
            campoApellido,
            botonGuardar,
            botonActualizar,
            botonEliminar,
            botonLimpiar,
            etiquetaEstado,
            cuadriculaAutores
        ]);
        EstiloInterfazBiblioteca.Aplicar(this, cuadriculaAutores);
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

    private async void FormularioAutores_Load(object? remitente, EventArgs argumentos)
    {
        await CargarAutoresAsync();
    }

    private async void BotonGuardar_Click(object? remitente, EventArgs argumentos)
    {
        try
        {
            await autorServicio.CrearAsync(campoNombre.Text, campoApellido.Text);
            await CargarAutoresAsync();
            LimpiarSeleccion();
            MostrarMensaje("El autor se guardó correctamente.");
        }
        catch (ArgumentException errorValidacion)
        {
            MostrarError(errorValidacion.Message);
        }
        catch (SqlException errorSql)
        {
            MostrarError($"No se pudo guardar el autor.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonActualizar_Click(object? remitente, EventArgs argumentos)
    {
        if (autorIdSeleccionado is not int autorId)
        {
            MostrarError("Selecciona un autor de la lista.");
            return;
        }

        try
        {
            await autorServicio.ActualizarAsync(autorId, campoNombre.Text, campoApellido.Text);
            await CargarAutoresAsync();
            LimpiarSeleccion();
            MostrarMensaje("El autor se actualizó correctamente.");
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
            MostrarError($"No se pudo actualizar el autor.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonEliminar_Click(object? remitente, EventArgs argumentos)
    {
        if (autorIdSeleccionado is not int autorId)
        {
            MostrarError("Selecciona un autor de la lista.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            this,
            "¿Deseas eliminar el autor seleccionado?",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await autorServicio.EliminarAsync(autorId);
            await CargarAutoresAsync();
            LimpiarSeleccion();
            MostrarMensaje("El autor se eliminó correctamente.");
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
            MostrarError($"No se pudo eliminar el autor.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async Task CargarAutoresAsync()
    {
        try
        {
            List<Autor> autores = await autorServicio.ObtenerTodosAsync();
            cuadriculaAutores.DataSource = autores;
            etiquetaEstado.Text = $"Autores registrados: {autores.Count}.";

            if (cuadriculaAutores.Columns[nameof(Autor.AutorId)] is DataGridViewColumn columnaId)
            {
                columnaId.HeaderText = "Código";
            }

            if (cuadriculaAutores.Columns[nameof(Autor.Nombre)] is DataGridViewColumn columnaNombre)
            {
                columnaNombre.HeaderText = "Nombre";
            }

            if (cuadriculaAutores.Columns[nameof(Autor.Apellido)] is DataGridViewColumn columnaApellido)
            {
                columnaApellido.HeaderText = "Apellido";
            }

            cuadriculaAutores.ClearSelection();
            autorIdSeleccionado = null;
            botonActualizar.Enabled = false;
            botonEliminar.Enabled = false;
            botonGuardar.Enabled = true;
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible consultar los autores.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private void CuadriculaAutores_SelectionChanged(object? remitente, EventArgs argumentos)
    {
        if (cuadriculaAutores.SelectedRows.Count != 1 ||
            cuadriculaAutores.SelectedRows[0].DataBoundItem is not Autor autor)
        {
            return;
        }

        autorIdSeleccionado = autor.AutorId;
        campoNombre.Text = autor.Nombre;
        campoApellido.Text = autor.Apellido;
        botonActualizar.Enabled = true;
        botonEliminar.Enabled = true;
        botonGuardar.Enabled = false;
    }

    private void LimpiarSeleccion()
    {
        autorIdSeleccionado = null;
        campoNombre.Clear();
        campoApellido.Clear();
        cuadriculaAutores.ClearSelection();
        botonActualizar.Enabled = false;
        botonEliminar.Enabled = false;
        botonGuardar.Enabled = true;
        etiquetaEstado.Text = "Selecciona un autor de la lista para editarlo o eliminarlo.";
    }

    private void MostrarMensaje(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Autores", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MostrarError(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Autores", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
