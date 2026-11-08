namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Define y aplica el estilo visual compartido por las pantallas.
/// </summary>
public static class EstiloInterfazBiblioteca
{
    public static readonly Color fondoAplicacion = Color.FromArgb(244, 247, 251);
    public static readonly Color blanco = Color.White;
    public static readonly Color tinta = Color.FromArgb(30, 41, 59);
    public static readonly Color tintaSuave = Color.FromArgb(100, 116, 139);
    public static readonly Color azul = Color.FromArgb(37, 99, 235);
    public static readonly Color azulOscuro = Color.FromArgb(24, 43, 73);
    public static readonly Color azulClaro = Color.FromArgb(235, 243, 255);
    public static readonly Color verde = Color.FromArgb(22, 126, 91);
    public static readonly Color rojo = Color.FromArgb(185, 56, 70);
    public static readonly Color borde = Color.FromArgb(226, 232, 240);

    /// <summary>
    /// Aplica colores y formatos a los controles de un formulario.
    /// </summary>
    /// <param name="formulario">Pantalla cuyos controles se van a estilizar.</param>
    /// <param name="cuadriculaPrincipal">Tabla opcional que se muestra como tarjetas.</param>
    public static void Aplicar(Form formulario, DataGridView? cuadriculaPrincipal = null)
    {
        formulario.BackColor = fondoAplicacion;
        formulario.ForeColor = tinta;
        formulario.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        formulario.StartPosition = FormStartPosition.CenterParent;

        foreach (Control control in ObtenerControles(formulario))
        {
            if (control is Label etiqueta)
            {
                EstilizarEtiqueta(etiqueta);
            }
            else if (control is Button boton)
            {
                EstilizarBoton(boton, formulario is FormularioReportes);
            }
            else if (control is DataGridView cuadricula)
            {
                EstilizarCuadricula(cuadricula);
            }
            else if (control is TextBox campo)
            {
                campo.BorderStyle = BorderStyle.FixedSingle;
                campo.BackColor = blanco;
                campo.ForeColor = tinta;
                campo.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            }
            else if (control is ComboBox lista)
            {
                lista.FlatStyle = FlatStyle.Flat;
                lista.BackColor = blanco;
                lista.ForeColor = tinta;
                lista.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            else if (control is CheckedListBox listaVerificable)
            {
                listaVerificable.BorderStyle = BorderStyle.FixedSingle;
                listaVerificable.BackColor = blanco;
                listaVerificable.ForeColor = tinta;
                listaVerificable.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            else if (control is DateTimePicker selectorFecha)
            {
                selectorFecha.CalendarForeColor = tinta;
                selectorFecha.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            else if (control is TabControl pestanas)
            {
                pestanas.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular);
            }
            else if (control is TabPage pestana)
            {
                pestana.BackColor = blanco;
                pestana.ForeColor = tinta;
            }
        }

        if (cuadriculaPrincipal is not null)
        {
            AgregarTarjetas(formulario, cuadriculaPrincipal);
        }
    }

    private static IEnumerable<Control> ObtenerControles(Control contenedor)
    {
        foreach (Control control in contenedor.Controls)
        {
            yield return control;
            foreach (Control controlAnidado in ObtenerControles(control))
            {
                yield return controlAnidado;
            }
        }
    }

    private static void EstilizarEtiqueta(Label etiqueta)
    {
        if (etiqueta.Name == "etiquetaTitulo")
        {
            etiqueta.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Regular);
            etiqueta.ForeColor = tinta;
        }
        else if (etiqueta.Name == "etiquetaEstado")
        {
            etiqueta.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            etiqueta.ForeColor = tintaSuave;
        }
        else
        {
            etiqueta.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular);
            etiqueta.ForeColor = tinta;
        }
    }

    private static void EstilizarBoton(Button boton, bool esFormularioReportes)
    {
        Color colorFondo;
        Color colorTexto;
        Color colorSobre;

        if (boton.Name == "botonEliminar")
        {
            colorFondo = Color.FromArgb(255, 241, 242);
            colorTexto = rojo;
            colorSobre = Color.FromArgb(254, 226, 226);
        }
        else if (boton.Name == "botonDevolver")
        {
            colorFondo = Color.FromArgb(230, 248, 240);
            colorTexto = verde;
            colorSobre = Color.FromArgb(209, 241, 224);
        }
        else if (boton.Name == "botonGuardar" ||
                 boton.Name == "botonProbarConexion" ||
                 (boton.Name == "botonActualizar" && esFormularioReportes))
        {
            colorFondo = azul;
            colorTexto = blanco;
            colorSobre = Color.FromArgb(29, 78, 216);
        }
        else
        {
            colorFondo = azulClaro;
            colorTexto = azulOscuro;
            colorSobre = Color.FromArgb(219, 234, 254);
        }

        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = colorSobre;
        boton.BackColor = boton.Enabled ? colorFondo : Color.FromArgb(226, 232, 240);
        boton.ForeColor = boton.Enabled ? colorTexto : tintaSuave;
        boton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular);
        boton.Cursor = boton.Enabled ? Cursors.Hand : Cursors.Default;
        boton.UseVisualStyleBackColor = false;
        boton.Height = Math.Max(boton.Height, 38);
    }

    private static void EstilizarCuadricula(DataGridView cuadricula)
    {
        cuadricula.BackgroundColor = blanco;
        cuadricula.BorderStyle = BorderStyle.None;
        cuadricula.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        cuadricula.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        cuadricula.EnableHeadersVisualStyles = false;
        cuadricula.GridColor = borde;
        cuadricula.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        cuadricula.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        cuadricula.ColumnHeadersHeight = 42;
        cuadricula.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        cuadricula.RowTemplate.Height = 36;
        cuadricula.AllowUserToResizeRows = false;
        cuadricula.DefaultCellStyle.BackColor = blanco;
        cuadricula.DefaultCellStyle.ForeColor = tinta;
        cuadricula.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        cuadricula.DefaultCellStyle.SelectionForeColor = azulOscuro;
        cuadricula.DefaultCellStyle.Padding = new Padding(8, 2, 8, 2);
        cuadricula.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        cuadricula.ColumnHeadersDefaultCellStyle.BackColor = azulOscuro;
        cuadricula.ColumnHeadersDefaultCellStyle.ForeColor = blanco;
        cuadricula.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F);
        cuadricula.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
        cuadricula.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
    }

    private static void AgregarTarjetas(Form formulario, DataGridView cuadricula)
    {
        Rectangle limitesCuadricula = cuadricula.Bounds;
        var tarjetaFormulario = new Panel
        {
            BackColor = blanco,
            Bounds = new Rectangle(
                12,
                12,
                formulario.ClientSize.Width - 24,
                Math.Max(170, limitesCuadricula.Top - 24)),
            Name = "tarjetaFormulario",
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        var tarjetaCuadricula = new Panel
        {
            BackColor = blanco,
            Bounds = new Rectangle(
                Math.Max(8, limitesCuadricula.Left - 12),
                Math.Max(8, limitesCuadricula.Top - 12),
                limitesCuadricula.Width + 24,
                limitesCuadricula.Height + 24),
            Name = "tarjetaCuadricula",
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        formulario.Controls.Add(tarjetaFormulario);
        formulario.Controls.Add(tarjetaCuadricula);
        tarjetaCuadricula.SendToBack();
        tarjetaFormulario.SendToBack();
    }
}
