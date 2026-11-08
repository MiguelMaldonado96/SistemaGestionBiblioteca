using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;
using SistemaGestionBiblioteca.Negocio;

namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Permite consultar y administrar libros, autores, editoriales y existencias.
/// </summary>
public sealed class FormularioLibros : Form
{
    private readonly LibroServicio libroServicio = new(new LibroRepositorio(new ConexionBaseDatos()));
    private readonly AutorServicio autorServicio = new(new AutorRepositorio(new ConexionBaseDatos()));
    private readonly EditorialServicio editorialServicio =
        new(new EditorialRepositorio(new ConexionBaseDatos()));
    private readonly DataGridView cuadriculaLibros = new();
    private readonly TextBox campoCodigo = new();
    private readonly TextBox campoIsbn = new();
    private readonly TextBox campoTitulo = new();
    private readonly TextBox campoAnio = new();
    private readonly TextBox campoCantidad = new();
    private readonly ComboBox listaEditoriales = new();
    private readonly CheckedListBox listaAutores = new();
    private readonly Button botonGuardar = new();
    private readonly Button botonActualizar = new();
    private readonly Button botonEliminar = new();
    private readonly Label etiquetaEstado = new();
    private int? libroIdSeleccionado;

    public FormularioLibros()
    {
        ConfigurarVentana();
        ConstruirInterfaz();
        Load += FormularioLibros_Load;
    }

    private void ConfigurarVentana()
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 760);
        MinimumSize = new Size(880, 680);
        Name = "FormularioLibros";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gestión de libros";
    }

    private void ConstruirInterfaz()
    {
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(24, 18),
            Text = "Libros"
        };

        var etiquetaCodigo = CrearEtiqueta("Código", 24, 52);
        PrepararCampo(campoCodigo, "campoCodigo", 24, 78, 200, 20);

        var etiquetaTituloLibro = CrearEtiqueta("Título", 250, 52);
        PrepararCampo(campoTitulo, "campoTitulo", 250, 78, 350, 200);

        var etiquetaIsbn = CrearEtiqueta("ISBN (opcional)", 626, 52);
        PrepararCampo(campoIsbn, "campoIsbn", 626, 78, 250, 20);

        var etiquetaEditorial = CrearEtiqueta("Editorial", 24, 126);
        listaEditoriales.DropDownStyle = ComboBoxStyle.DropDownList;
        listaEditoriales.Location = new Point(24, 150);
        listaEditoriales.Name = "listaEditoriales";
        listaEditoriales.Size = new Size(260, 27);

        var etiquetaAnio = CrearEtiqueta("Año de publicación", 310, 126);
        PrepararCampo(campoAnio, "campoAnio", 310, 150, 150, 4);

        var etiquetaCantidad = CrearEtiqueta("Ejemplares", 520, 126);
        PrepararCampo(campoCantidad, "campoCantidad", 520, 150, 110, 9);

        var etiquetaAutores = CrearEtiqueta("Autores (selecciona uno o varios)", 650, 126);
        listaAutores.CheckOnClick = true;
        listaAutores.Location = new Point(650, 150);
        listaAutores.Name = "listaAutores";
        listaAutores.Size = new Size(326, 105);
        listaAutores.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        botonGuardar.Location = new Point(24, 276);
        botonGuardar.Name = "botonGuardar";
        botonGuardar.Size = new Size(120, 34);
        botonGuardar.Text = "Guardar";
        botonGuardar.Click += BotonGuardar_Click;

        botonActualizar.Location = new Point(156, 276);
        botonActualizar.Name = "botonActualizar";
        botonActualizar.Size = new Size(120, 34);
        botonActualizar.Text = "Actualizar";
        botonActualizar.Enabled = false;
        botonActualizar.Click += BotonActualizar_Click;

        botonEliminar.Location = new Point(288, 276);
        botonEliminar.Name = "botonEliminar";
        botonEliminar.Size = new Size(120, 34);
        botonEliminar.Text = "Eliminar";
        botonEliminar.Enabled = false;
        botonEliminar.Click += BotonEliminar_Click;

        var botonLimpiar = new Button
        {
            Location = new Point(420, 276),
            Name = "botonLimpiar",
            Size = new Size(120, 34),
            Text = "Limpiar"
        };
        botonLimpiar.Click += (_, _) => LimpiarSeleccion();

        etiquetaEstado.AutoSize = true;
        etiquetaEstado.Location = new Point(24, 326);
        etiquetaEstado.Name = "etiquetaEstado";
        etiquetaEstado.Text = "Registra libros con su editorial, autores y existencias.";

        cuadriculaLibros.AllowUserToAddRows = false;
        cuadriculaLibros.AllowUserToDeleteRows = false;
        cuadriculaLibros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        cuadriculaLibros.Location = new Point(24, 356);
        cuadriculaLibros.MultiSelect = false;
        cuadriculaLibros.Name = "cuadriculaLibros";
        cuadriculaLibros.ReadOnly = true;
        cuadriculaLibros.RowHeadersVisible = false;
        cuadriculaLibros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        cuadriculaLibros.Size = new Size(952, 338);
        cuadriculaLibros.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cuadriculaLibros.SelectionChanged += CuadriculaLibros_SelectionChanged;

        Controls.AddRange(
        [
            etiquetaTitulo,
            etiquetaCodigo,
            campoCodigo,
            etiquetaTituloLibro,
            campoTitulo,
            etiquetaIsbn,
            campoIsbn,
            etiquetaEditorial,
            listaEditoriales,
            etiquetaAnio,
            campoAnio,
            etiquetaCantidad,
            campoCantidad,
            etiquetaAutores,
            listaAutores,
            botonGuardar,
            botonActualizar,
            botonEliminar,
            botonLimpiar,
            etiquetaEstado,
            cuadriculaLibros
        ]);
        EstiloInterfazBiblioteca.Aplicar(this, cuadriculaLibros);
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

    private async void FormularioLibros_Load(object? remitente, EventArgs argumentos)
    {
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        try
        {
            List<Autor> autores = await autorServicio.ObtenerTodosAsync();
            List<Editorial> editoriales = await editorialServicio.ObtenerTodosAsync();

            listaAutores.Items.Clear();
            foreach (Autor autor in autores)
            {
                listaAutores.Items.Add(new AutorSeleccionable(autor));
            }

            listaEditoriales.DataSource = editoriales;
            listaEditoriales.DisplayMember = nameof(Editorial.Nombre);
            listaEditoriales.ValueMember = nameof(Editorial.EditorialId);
            botonGuardar.Enabled = autores.Count > 0 && editoriales.Count > 0;

            if (autores.Count == 0 || editoriales.Count == 0)
            {
                etiquetaEstado.Text =
                    "Registra al menos un autor y una editorial antes de agregar libros.";
            }

            await CargarLibrosAsync();
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible cargar los datos de autores o editoriales.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonGuardar_Click(object? remitente, EventArgs argumentos)
    {
        if (!TryObtenerDatos(out DatosFormularioLibro datos))
        {
            return;
        }

        try
        {
            await libroServicio.CrearAsync(
                datos.Codigo,
                datos.Isbn,
                datos.Titulo,
                datos.AnioPublicacion,
                datos.EditorialId,
                datos.CantidadTotal,
                datos.AutoresIds);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El libro se guardó correctamente.");
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
        if (libroIdSeleccionado is not int libroId)
        {
            MostrarError("Selecciona un libro de la lista.");
            return;
        }

        if (!TryObtenerDatos(out DatosFormularioLibro datos))
        {
            return;
        }

        try
        {
            await libroServicio.ActualizarAsync(
                libroId,
                datos.Codigo,
                datos.Isbn,
                datos.Titulo,
                datos.AnioPublicacion,
                datos.EditorialId,
                datos.CantidadTotal,
                datos.AutoresIds);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El libro se actualizó correctamente.");
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
        if (libroIdSeleccionado is not int libroId)
        {
            MostrarError("Selecciona un libro de la lista.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            this,
            "¿Deseas eliminar el libro seleccionado?",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await libroServicio.EliminarAsync(libroId);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El libro se eliminó correctamente.");
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
            MostrarError($"No se pudo eliminar el libro.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private bool TryObtenerDatos(out DatosFormularioLibro datos)
    {
        datos = default;

        if (listaEditoriales.SelectedValue is not int editorialId)
        {
            MostrarError("Selecciona una editorial.");
            return false;
        }

        if (!int.TryParse(campoCantidad.Text, out int cantidadTotal))
        {
            MostrarError("Escribe un número válido de ejemplares.");
            return false;
        }

        short? anioPublicacion = null;
        if (!string.IsNullOrWhiteSpace(campoAnio.Text))
        {
            if (!short.TryParse(campoAnio.Text, out short anio))
            {
                MostrarError("Escribe un año de publicación válido.");
                return false;
            }

            anioPublicacion = anio;
        }

        List<int> autoresIds = listaAutores.CheckedItems
            .OfType<AutorSeleccionable>()
            .Select(autor => autor.AutorId)
            .ToList();

        if (autoresIds.Count == 0)
        {
            MostrarError("Selecciona al menos un autor.");
            return false;
        }

        datos = new DatosFormularioLibro(
            campoCodigo.Text,
            campoIsbn.Text,
            campoTitulo.Text,
            anioPublicacion,
            editorialId,
            cantidadTotal,
            autoresIds);
        return true;
    }

    private async Task CargarLibrosAsync()
    {
        try
        {
            List<Libro> libros = await libroServicio.ObtenerTodosAsync();
            cuadriculaLibros.DataSource = libros;
            etiquetaEstado.Text = $"Libros registrados: {libros.Count}.";

            OcultarColumna(nameof(Libro.LibroId));
            OcultarColumna(nameof(Libro.EditorialId));
            OcultarColumna(nameof(Libro.AutoresIds));
            OcultarColumna(nameof(Libro.AutoresNombres));
            CambiarTituloColumna(nameof(Libro.Codigo), "Código");
            CambiarTituloColumna(nameof(Libro.ISBN), "ISBN");
            CambiarTituloColumna(nameof(Libro.Titulo), "Título");
            CambiarTituloColumna(nameof(Libro.AnioPublicacion), "Año");
            CambiarTituloColumna(nameof(Libro.EditorialNombre), "Editorial");
            CambiarTituloColumna(nameof(Libro.CantidadTotal), "Ejemplares");
            CambiarTituloColumna(nameof(Libro.CantidadDisponible), "Disponibles");
            CambiarTituloColumna(nameof(Libro.AutoresTexto), "Autores");
            cuadriculaLibros.ClearSelection();
            libroIdSeleccionado = null;
            botonActualizar.Enabled = false;
            botonEliminar.Enabled = false;
            botonGuardar.Enabled = listaEditoriales.Items.Count > 0 && listaAutores.Items.Count > 0;
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible consultar los libros.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private void OcultarColumna(string nombreColumna)
    {
        if (cuadriculaLibros.Columns[nombreColumna] is DataGridViewColumn columna)
        {
            columna.Visible = false;
        }
    }

    private void CambiarTituloColumna(string nombreColumna, string titulo)
    {
        if (cuadriculaLibros.Columns[nombreColumna] is DataGridViewColumn columna)
        {
            columna.HeaderText = titulo;
        }
    }

    private void CuadriculaLibros_SelectionChanged(object? remitente, EventArgs argumentos)
    {
        if (cuadriculaLibros.SelectedRows.Count != 1 ||
            cuadriculaLibros.SelectedRows[0].DataBoundItem is not Libro libro)
        {
            return;
        }

        libroIdSeleccionado = libro.LibroId;
        campoCodigo.Text = libro.Codigo;
        campoIsbn.Text = libro.ISBN ?? string.Empty;
        campoTitulo.Text = libro.Titulo;
        campoAnio.Text = libro.AnioPublicacion?.ToString() ?? string.Empty;
        campoCantidad.Text = libro.CantidadTotal.ToString();
        listaEditoriales.SelectedValue = libro.EditorialId;

        for (int indiceAutor = 0; indiceAutor < listaAutores.Items.Count; indiceAutor++)
        {
            if (listaAutores.Items[indiceAutor] is AutorSeleccionable autorSeleccionable)
            {
                listaAutores.SetItemChecked(
                    indiceAutor,
                    libro.AutoresIds.Contains(autorSeleccionable.AutorId));
            }
        }

        botonActualizar.Enabled = true;
        botonEliminar.Enabled = true;
        botonGuardar.Enabled = false;
    }

    private void LimpiarSeleccion()
    {
        libroIdSeleccionado = null;
        campoCodigo.Clear();
        campoIsbn.Clear();
        campoTitulo.Clear();
        campoAnio.Clear();
        campoCantidad.Clear();
        listaEditoriales.SelectedIndex = listaEditoriales.Items.Count > 0 ? 0 : -1;

        for (int indiceAutor = 0; indiceAutor < listaAutores.Items.Count; indiceAutor++)
        {
            listaAutores.SetItemChecked(indiceAutor, false);
        }

        cuadriculaLibros.ClearSelection();
        botonActualizar.Enabled = false;
        botonEliminar.Enabled = false;
        botonGuardar.Enabled = listaEditoriales.Items.Count > 0 && listaAutores.Items.Count > 0;
        etiquetaEstado.Text = "Registra libros con su editorial, autores y existencias.";
    }

    private void MostrarErrorSql(SqlException errorSql, string operacion)
    {
        if (errorSql.Number is 2601 or 2627)
        {
            MostrarError("El código del libro o el ISBN ya está registrado.");
            return;
        }

        MostrarError($"No se pudo {operacion} el libro.{Environment.NewLine}{errorSql.Message}");
    }

    private void MostrarMensaje(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Libros", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MostrarError(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Libros", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private sealed class AutorSeleccionable
    {
        public int AutorId { get; }
        private readonly string nombreCompleto;

        public AutorSeleccionable(Autor autor)
        {
            AutorId = autor.AutorId;
            nombreCompleto = $"{autor.Apellido}, {autor.Nombre}";
        }

        public override string ToString()
        {
            return nombreCompleto;
        }
    }

    private readonly record struct DatosFormularioLibro(
        string Codigo,
        string? Isbn,
        string Titulo,
        short? AnioPublicacion,
        int EditorialId,
        int CantidadTotal,
        IReadOnlyCollection<int> AutoresIds);
}
