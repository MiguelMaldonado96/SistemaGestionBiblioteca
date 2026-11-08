using Microsoft.Data.SqlClient;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;
using SistemaGestionBiblioteca.Negocio;

namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Registra préstamos y devoluciones y muestra sus movimientos.
/// </summary>
public sealed class FormularioPrestamos : Form
{
    private readonly PrestamoServicio prestamoServicio =
        new(new PrestamoRepositorio(new ConexionBaseDatos()));
    private readonly UsuarioRepositorio usuarioRepositorio = new(new ConexionBaseDatos());
    private readonly LibroRepositorio libroRepositorio = new(new ConexionBaseDatos());
    private readonly DataGridView cuadriculaPrestamos = new();
    private readonly ComboBox listaUsuarios = new();
    private readonly ComboBox listaLibros = new();
    private readonly DateTimePicker selectorFechaPrestamo = new();
    private readonly DateTimePicker selectorFechaDevolucionPrevista = new();
    private readonly Button botonGuardar = new();
    private readonly Button botonActualizar = new();
    private readonly Button botonDevolver = new();
    private readonly Button botonEliminar = new();
    private readonly Label etiquetaEstado = new();
    private int? prestamoIdSeleccionado;

    public FormularioPrestamos()
    {
        ConfigurarVentana();
        ConstruirInterfaz();
        Load += FormularioPrestamos_Load;
    }

    private void ConfigurarVentana()
    {
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1040, 690);
        MinimumSize = new Size(900, 590);
        Name = "FormularioPrestamos";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Préstamos y devoluciones";
    }

    private void ConstruirInterfaz()
    {
        var etiquetaTitulo = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(24, 18),
            Text = "Préstamos y devoluciones"
        };

        var etiquetaUsuario = CrearEtiqueta("Usuario", 24, 52);
        listaUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
        listaUsuarios.Location = new Point(24, 78);
        listaUsuarios.Name = "listaUsuarios";
        listaUsuarios.Size = new Size(450, 27);

        var etiquetaLibro = CrearEtiqueta("Libro", 500, 52);
        listaLibros.DropDownStyle = ComboBoxStyle.DropDownList;
        listaLibros.Location = new Point(500, 78);
        listaLibros.Name = "listaLibros";
        listaLibros.Size = new Size(500, 27);

        var etiquetaFechaPrestamo = CrearEtiqueta("Fecha del préstamo", 24, 122);
        selectorFechaPrestamo.Format = DateTimePickerFormat.Short;
        selectorFechaPrestamo.Location = new Point(24, 146);
        selectorFechaPrestamo.MaxDate = DateTime.Today;
        selectorFechaPrestamo.Name = "selectorFechaPrestamo";
        selectorFechaPrestamo.Size = new Size(220, 27);
        selectorFechaPrestamo.Value = DateTime.Today;

        var etiquetaFechaDevolucion = CrearEtiqueta("Fecha prevista de devolución", 270, 122);
        selectorFechaDevolucionPrevista.Format = DateTimePickerFormat.Short;
        selectorFechaDevolucionPrevista.Location = new Point(270, 146);
        selectorFechaDevolucionPrevista.Name = "selectorFechaDevolucionPrevista";
        selectorFechaDevolucionPrevista.Size = new Size(220, 27);
        selectorFechaDevolucionPrevista.Value = DateTime.Today.AddDays(14);

        botonGuardar.Location = new Point(24, 190);
        botonGuardar.Name = "botonGuardar";
        botonGuardar.Size = new Size(220, 38);
        botonGuardar.Text = "Registrar préstamo";
        botonGuardar.Click += BotonGuardar_Click;

        botonActualizar.Location = new Point(254, 190);
        botonActualizar.Name = "botonActualizar";
        botonActualizar.Size = new Size(120, 38);
        botonActualizar.Text = "Actualizar";
        botonActualizar.Enabled = false;
        botonActualizar.Click += BotonActualizar_Click;

        botonDevolver.Location = new Point(384, 190);
        botonDevolver.Name = "botonDevolver";
        botonDevolver.Size = new Size(220, 38);
        botonDevolver.Text = "Registrar devolución";
        botonDevolver.Enabled = false;
        botonDevolver.Click += BotonDevolver_Click;

        botonEliminar.Location = new Point(614, 190);
        botonEliminar.Name = "botonEliminar";
        botonEliminar.Size = new Size(120, 38);
        botonEliminar.Text = "Eliminar";
        botonEliminar.Enabled = false;
        botonEliminar.Click += BotonEliminar_Click;

        var botonLimpiar = new Button
        {
            Location = new Point(744, 190),
            Name = "botonLimpiar",
            Size = new Size(120, 38),
            Text = "Limpiar"
        };
        botonLimpiar.Click += (_, _) => LimpiarSeleccion();

        etiquetaEstado.AutoSize = true;
        etiquetaEstado.Location = new Point(24, 244);
        etiquetaEstado.Name = "etiquetaEstado";
        etiquetaEstado.Text = "Los préstamos descuentan existencias; las devoluciones las restauran.";

        cuadriculaPrestamos.AllowUserToAddRows = false;
        cuadriculaPrestamos.AllowUserToDeleteRows = false;
        cuadriculaPrestamos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        cuadriculaPrestamos.Location = new Point(24, 274);
        cuadriculaPrestamos.MultiSelect = false;
        cuadriculaPrestamos.Name = "cuadriculaPrestamos";
        cuadriculaPrestamos.ReadOnly = true;
        cuadriculaPrestamos.RowHeadersVisible = false;
        cuadriculaPrestamos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        cuadriculaPrestamos.Size = new Size(992, 372);
        cuadriculaPrestamos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cuadriculaPrestamos.SelectionChanged += CuadriculaPrestamos_SelectionChanged;

        Controls.AddRange(
        [
            etiquetaTitulo,
            etiquetaUsuario,
            listaUsuarios,
            etiquetaLibro,
            listaLibros,
            etiquetaFechaPrestamo,
            selectorFechaPrestamo,
            etiquetaFechaDevolucion,
            selectorFechaDevolucionPrevista,
            botonGuardar,
            botonActualizar,
            botonDevolver,
            botonEliminar,
            botonLimpiar,
            etiquetaEstado,
            cuadriculaPrestamos
        ]);
        EstiloInterfazBiblioteca.Aplicar(this, cuadriculaPrestamos);
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

    private async void FormularioPrestamos_Load(object? remitente, EventArgs argumentos)
    {
        await CargarDatosAsync();
    }

    private async Task CargarDatosAsync()
    {
        try
        {
            List<Usuario> usuarios = await usuarioRepositorio.ObtenerTodosAsync();
            List<Libro> libros = await libroRepositorio.ObtenerTodosAsync();

            listaUsuarios.Items.Clear();
            foreach (Usuario usuario in usuarios)
            {
                listaUsuarios.Items.Add(new UsuarioSeleccionable(usuario));
            }

            listaLibros.Items.Clear();
            foreach (Libro libro in libros)
            {
                listaLibros.Items.Add(new LibroSeleccionable(libro));
            }

            if (listaUsuarios.Items.Count > 0)
            {
                listaUsuarios.SelectedIndex = 0;
            }

            if (listaLibros.Items.Count > 0)
            {
                listaLibros.SelectedIndex = 0;
            }

            botonGuardar.Enabled = usuarios.Count > 0 && libros.Count > 0;
            if (usuarios.Count == 0 || libros.Count == 0)
            {
                etiquetaEstado.Text = "Registra al menos un usuario y un libro antes de crear préstamos.";
            }

            await CargarPrestamosAsync();
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible cargar los usuarios o los libros.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonGuardar_Click(object? remitente, EventArgs argumentos)
    {
        if (!TryObtenerDatos(out DatosFormularioPrestamo datos))
        {
            return;
        }

        try
        {
            await prestamoServicio.RegistrarAsync(
                datos.UsuarioId,
                datos.LibroId,
                selectorFechaPrestamo.Value,
                selectorFechaDevolucionPrevista.Value);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El préstamo se registró correctamente.");
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
            MostrarError($"No se pudo registrar el préstamo.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonActualizar_Click(object? remitente, EventArgs argumentos)
    {
        if (prestamoIdSeleccionado is not int prestamoId)
        {
            MostrarError("Selecciona un préstamo activo de la lista.");
            return;
        }

        if (!TryObtenerDatos(out DatosFormularioPrestamo datos))
        {
            return;
        }

        try
        {
            await prestamoServicio.ActualizarAsync(
                prestamoId,
                datos.UsuarioId,
                datos.LibroId,
                selectorFechaPrestamo.Value,
                selectorFechaDevolucionPrevista.Value);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El préstamo se actualizó correctamente.");
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
            MostrarError($"No se pudo actualizar el préstamo.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonDevolver_Click(object? remitente, EventArgs argumentos)
    {
        if (prestamoIdSeleccionado is not int prestamoId)
        {
            MostrarError("Selecciona un préstamo activo de la lista.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            this,
            "¿Registrar la devolución del libro con fecha de hoy?",
            "Confirmar devolución",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await prestamoServicio.RegistrarDevolucionAsync(prestamoId, DateTime.Today);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("La devolución se registró y la existencia del libro se actualizó.");
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
            MostrarError($"No se pudo registrar la devolución.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private async void BotonEliminar_Click(object? remitente, EventArgs argumentos)
    {
        if (prestamoIdSeleccionado is not int prestamoId)
        {
            MostrarError("Selecciona un préstamo de la lista.");
            return;
        }

        DialogResult confirmacion = MessageBox.Show(
            this,
            "¿Deseas eliminar el préstamo seleccionado? Si está activo, su existencia se restaurará.",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmacion != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await prestamoServicio.EliminarAsync(prestamoId);
            await CargarDatosAsync();
            LimpiarSeleccion();
            MostrarMensaje("El préstamo se eliminó correctamente.");
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
            MostrarError($"No se pudo eliminar el préstamo.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private bool TryObtenerDatos(out DatosFormularioPrestamo datos)
    {
        datos = default;
        if (listaUsuarios.SelectedItem is not UsuarioSeleccionable usuario ||
            listaLibros.SelectedItem is not LibroSeleccionable libro)
        {
            MostrarError("Selecciona el usuario y el libro del préstamo.");
            return false;
        }

        datos = new DatosFormularioPrestamo(usuario.UsuarioId, libro.LibroId);
        return true;
    }

    private async Task CargarPrestamosAsync()
    {
        try
        {
            List<Prestamo> prestamos = await prestamoServicio.ObtenerTodosAsync();
            cuadriculaPrestamos.DataSource = prestamos;
            etiquetaEstado.Text = $"Préstamos registrados: {prestamos.Count}.";

            OcultarColumna(nameof(Prestamo.UsuarioId));
            OcultarColumna(nameof(Prestamo.LibroId));
            CambiarTituloColumna(nameof(Prestamo.PrestamoId), "Código");
            CambiarTituloColumna(nameof(Prestamo.UsuarioNombre), "Usuario");
            CambiarTituloColumna(nameof(Prestamo.CodigoLibro), "Código del libro");
            CambiarTituloColumna(nameof(Prestamo.LibroTitulo), "Libro");
            CambiarTituloColumna(nameof(Prestamo.FechaPrestamo), "Préstamo");
            CambiarTituloColumna(nameof(Prestamo.FechaDevolucionPrevista), "Devolución prevista");
            CambiarTituloColumna(nameof(Prestamo.FechaDevolucionReal), "Devolución real");
            CambiarTituloColumna(nameof(Prestamo.Estado), "Estado");
            cuadriculaPrestamos.ClearSelection();
            prestamoIdSeleccionado = null;
            botonActualizar.Enabled = false;
            botonDevolver.Enabled = false;
            botonEliminar.Enabled = false;
            botonGuardar.Enabled = listaUsuarios.Items.Count > 0 && listaLibros.Items.Count > 0;
        }
        catch (SqlException errorSql)
        {
            etiquetaEstado.Text = "No fue posible consultar los préstamos.";
            MostrarError($"Verifica la conexión con SQL Server.{Environment.NewLine}{errorSql.Message}");
        }
    }

    private void OcultarColumna(string nombreColumna)
    {
        if (cuadriculaPrestamos.Columns[nombreColumna] is DataGridViewColumn columna)
        {
            columna.Visible = false;
        }
    }

    private void CambiarTituloColumna(string nombreColumna, string titulo)
    {
        if (cuadriculaPrestamos.Columns[nombreColumna] is DataGridViewColumn columna)
        {
            columna.HeaderText = titulo;
        }
    }

    private void CuadriculaPrestamos_SelectionChanged(object? remitente, EventArgs argumentos)
    {
        if (cuadriculaPrestamos.SelectedRows.Count != 1 ||
            cuadriculaPrestamos.SelectedRows[0].DataBoundItem is not Prestamo prestamo)
        {
            return;
        }

        prestamoIdSeleccionado = prestamo.PrestamoId;
        listaUsuarios.SelectedItem = listaUsuarios.Items
            .OfType<UsuarioSeleccionable>()
            .FirstOrDefault(usuario => usuario.UsuarioId == prestamo.UsuarioId);
        listaLibros.SelectedItem = listaLibros.Items
            .OfType<LibroSeleccionable>()
            .FirstOrDefault(libro => libro.LibroId == prestamo.LibroId);
        selectorFechaPrestamo.Value = prestamo.FechaPrestamo;
        selectorFechaDevolucionPrevista.Value = prestamo.FechaDevolucionPrevista;

        bool prestamoActivo = prestamo.FechaDevolucionReal is null;
        botonActualizar.Enabled = prestamoActivo;
        botonDevolver.Enabled = prestamoActivo;
        botonEliminar.Enabled = true;
        botonGuardar.Enabled = false;
    }

    private void LimpiarSeleccion()
    {
        prestamoIdSeleccionado = null;
        if (listaUsuarios.Items.Count > 0)
        {
            listaUsuarios.SelectedIndex = 0;
        }

        if (listaLibros.Items.Count > 0)
        {
            listaLibros.SelectedIndex = 0;
        }

        selectorFechaPrestamo.Value = DateTime.Today;
        selectorFechaDevolucionPrevista.Value = DateTime.Today.AddDays(14);
        cuadriculaPrestamos.ClearSelection();
        botonActualizar.Enabled = false;
        botonDevolver.Enabled = false;
        botonEliminar.Enabled = false;
        botonGuardar.Enabled = listaUsuarios.Items.Count > 0 && listaLibros.Items.Count > 0;
        etiquetaEstado.Text = "Los préstamos descuentan existencias; las devoluciones las restauran.";
    }

    private void MostrarMensaje(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MostrarError(string mensaje)
    {
        MessageBox.Show(this, mensaje, "Préstamos", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private sealed class UsuarioSeleccionable
    {
        public int UsuarioId { get; }
        private readonly string nombreCompleto;

        public UsuarioSeleccionable(Usuario usuario)
        {
            UsuarioId = usuario.UsuarioId;
            nombreCompleto = $"{usuario.Apellido}, {usuario.Nombre} ({usuario.Documento})";
        }

        public override string ToString()
        {
            return nombreCompleto;
        }
    }

    private sealed class LibroSeleccionable
    {
        public int LibroId { get; }
        private readonly string descripcion;

        public LibroSeleccionable(Libro libro)
        {
            LibroId = libro.LibroId;
            descripcion = $"{libro.Codigo} - {libro.Titulo} (disponibles: {libro.CantidadDisponible})";
        }

        public override string ToString()
        {
            return descripcion;
        }
    }

    private readonly record struct DatosFormularioPrestamo(int UsuarioId, int LibroId);
}
