using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;
using SistemaGestionBiblioteca.Negocio;

namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Permite consultar y administrar las editoriales registradas.
/// </summary>
public sealed class FormularioEditoriales : Form
{
    private readonly EditorialServicio editorialServicio =
        new(new EditorialRepositorio(new ConexionBaseDatos()));
    private readonly DataGridView cuadriculaEditoriales = new();
    private readonly TextBox campoNombre = new();
    private readonly TextBox campoPais = new();
    private readonly Button botonGuardar = new();
    private readonly Button botonActualizar = new();
    private readonly Button botonEliminar = new();
    private readonly Label etiquetaEstado = new();
    private int? editorialIdSeleccionada;

    public FormularioEditoriales()
    {
        ConfigurarVentana();
        ConstruirInterfaz();
        Load += FormularioEditoriales_Load;
    }

    private void ConfigurarVentana()
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 540);
        MinimumSize = new Size(700, 480);
        Name = "FormularioEditoriales";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gestión de editoriales";
    }

    private void ConstruirInterfaz()
    {
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(24, 20),
            Text = "Editoriales"
        };

        var etiquetaNombre = CrearEtiqueta("Nombre", 24, 68);
        campoNombre.Location = new Point(24, 94);
        campoNombre.MaxLength = 120;
        campoNombre.Name = "campoNombre";
        campoNombre.Size = new Size(320, 27);

        var etiquetaPais = CrearEtiqueta("País (opcional)", 366, 68);
        campoPais.Location = new Point(366, 94);
        campoPais.MaxLength = 80;
        campoPais.Name = "campoPais";
        campoPais.Size = new Size(320, 27);

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
        etiquetaEstado.Text = "Selecciona una editorial para editarla o eliminarla.";

        cuadriculaEditoriales.AllowUserToAddRows = false;
        cuadriculaEditoriales.AllowUserToDeleteRows = false;
        cuadriculaEditoriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        cuadriculaEditoriales.Location = new Point(24, 222);
        cuadriculaEditoriales.MultiSelect = false;
        cuadriculaEditoriales.Name = "cuadriculaEditoriales";
        cuadriculaEditoriales.ReadOnly = true;
        cuadriculaEditoriales.RowHeadersVisible = false;
        cuadriculaEditoriales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        cuadriculaEditoriales.Size = new Size(740, 288);
        cuadriculaEditoriales.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cuadriculaEditoriales.SelectionChanged += CuadriculaEditoriales_SelectionChanged;

        Controls.AddRange(
        [
            etiquetaTitulo,
            etiquetaNombre,
            campoNombre,
            etiquetaPais,
            campoPais,
            botonGuardar,
            botonActualizar,
            botonEliminar,
            botonLimpiar,
            etiquetaEstado,
            cuadriculaEditoriales
        ]);
        EstiloInterfazBiblioteca.Aplicar(this, cuadriculaEditoriales);
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

    private async void FormularioEditoriales_Load(object? remitente, EventArgs argumentos)
    {
        await CargarEditorialesAsync();
    }

    private async void BotonGuardar_Click(object? remitente, EventArgs argumentos)
    {
        try
        {
            await editorialServicio.CrearAsync(campoNombre.Text, campoPais.Text);
            await CargarEditorialesAsync();
            LimpiarSeleccion();
            MostrarMensaje("La editorial se guardó correctamente.");
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
        if (editorialIdSeleccionada is not int editorialId)
        {
            MostrarError("Selecciona una editorial de la lista.");
            return;
        }

        try
        {
            await editorialServicio.ActualizarAsync(editorialId, campoNombre.Text, campoPais.Text);
            await CargarEditorialesAsync();
            LimpiarSeleccion();
            MostrarMensaje("La editorial se actualizó correctamente.");
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
        if (editorialIdSeleccionada is not int editorialId)
        {
            MostrarError("Selecciona una editorial de la lista.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            this,
            "¿Deseas eliminar la editorial seleccionada?",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await editorialServicio.EliminarAsync(editorialId);
            await CargarEditorialesAsync();
            LimpiarSeleccion();
            MostrarMensaje("La editorial se eliminó correctamente.");
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

    private async Task CargarEditorialesAsync()
    {
        try
        {
            List<Editorial> editoriales = await editorialServicio.ObtenerTodosAsync();
            cuadriculaEditoriales.DataSource = editoriales;
            etiquetaEstado.Text = $"Editoriales registradas: {editoriales.Count}.";

            if (cuadriculaEditoriales.Columns[nameof(Editorial.EditorialId)] is DataGridViewColumn columnaId)
            {
                columnaId.HeaderText = "Código";
            }

            if (cuadriculaEditoriales.Columns[nameof(Editorial.Nombre)] is DataGridViewColumn columnaNombre)
            {
                columnaNombre.HeaderText = "Nombre";
            }

            if (cuadriculaEditoriales.Columns[nameof(Editorial.Pais)] is DataGridViewColumn columnaPais)
            {
                columnaPais.HeaderText = "País";
            }

            cuadriculaEditoriales.ClearSelection();
            editorialIdSeleccionada = null;
            botonActualizar.Enabled = false;
            botonEliminar.Enabled = false;
            botonGuardar.Enabled = true;
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible consultar las editoriales.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private void CuadriculaEditoriales_SelectionChanged(object? remitente, EventArgs argumentos)
    {
        if (cuadriculaEditoriales.SelectedRows.Count != 1 ||
            cuadriculaEditoriales.SelectedRows[0].DataBoundItem is not Editorial editorial)
        {
            return;
        }

        editorialIdSeleccionada = editorial.EditorialId;
        campoNombre.Text = editorial.Nombre;
        campoPais.Text = editorial.Pais ?? string.Empty;
        botonActualizar.Enabled = true;
        botonEliminar.Enabled = true;
        botonGuardar.Enabled = false;
    }

    private void LimpiarSeleccion()
    {
        editorialIdSeleccionada = null;
        campoNombre.Clear();
        campoPais.Clear();
        cuadriculaEditoriales.ClearSelection();
        botonActualizar.Enabled = false;
        botonEliminar.Enabled = false;
        botonGuardar.Enabled = true;
        etiquetaEstado.Text = "Selecciona una editorial para editarla o eliminarla.";
    }

    private void MostrarErrorSql(SqlException errorSql, string operacion)
    {
        if (errorSql.Number is 2601 or 2627)
        {
            MostrarError("Ya existe una editorial con ese nombre.");
            return;
        }

        MostrarError($"No se pudo {operacion} la editorial.{Environment.NewLine}{errorSql.Message}");
    }

    private void MostrarMensaje(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Editoriales", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MostrarError(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Editoriales", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
