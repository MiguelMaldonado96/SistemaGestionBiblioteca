#nullable enable
namespace SistemaGestionBiblioteca.Presentacion;

partial class FormularioPrincipal
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? componentes;
    private Button botonProbarConexion = null!;
    private Button botonGestionarAutores = null!;
    private Button botonGestionarEditoriales = null!;
    private Button botonGestionarUsuarios = null!;
    private Button botonGestionarLibros = null!;
    private Button botonGestionarPrestamos = null!;
    private Button botonGestionarReportes = null!;
    private Label etiquetaEstadoConexion = null!;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool liberandoRecursos)
    {
        if (liberandoRecursos && (componentes != null))
        {
            componentes.Dispose();
        }
        base.Dispose(liberandoRecursos);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        componentes = new System.ComponentModel.Container();
        botonProbarConexion = new Button();
        botonGestionarAutores = new Button();
        botonGestionarEditoriales = new Button();
        botonGestionarUsuarios = new Button();
        botonGestionarLibros = new Button();
        botonGestionarPrestamos = new Button();
        botonGestionarReportes = new Button();
        etiquetaEstadoConexion = new Label();
        SuspendLayout();
        botonProbarConexion.Location = new Point(32, 92);
        botonProbarConexion.Name = "botonProbarConexion";
        botonProbarConexion.Size = new Size(190, 36);
        botonProbarConexion.TabIndex = 0;
        botonProbarConexion.Text = "Probar conexión";
        botonProbarConexion.UseVisualStyleBackColor = true;
        botonGestionarAutores.Location = new Point(240, 92);
        botonGestionarAutores.Name = "botonGestionarAutores";
        botonGestionarAutores.Size = new Size(190, 36);
        botonGestionarAutores.TabIndex = 1;
        botonGestionarAutores.Text = "Gestionar autores";
        botonGestionarAutores.UseVisualStyleBackColor = true;
        botonGestionarEditoriales.Location = new Point(448, 92);
        botonGestionarEditoriales.Name = "botonGestionarEditoriales";
        botonGestionarEditoriales.Size = new Size(190, 36);
        botonGestionarEditoriales.TabIndex = 2;
        botonGestionarEditoriales.Text = "Gestionar editoriales";
        botonGestionarEditoriales.UseVisualStyleBackColor = true;
        botonGestionarUsuarios.Location = new Point(656, 92);
        botonGestionarUsuarios.Name = "botonGestionarUsuarios";
        botonGestionarUsuarios.Size = new Size(190, 36);
        botonGestionarUsuarios.TabIndex = 3;
        botonGestionarUsuarios.Text = "Gestionar usuarios";
        botonGestionarUsuarios.UseVisualStyleBackColor = true;
        botonGestionarLibros.Location = new Point(32, 148);
        botonGestionarLibros.Name = "botonGestionarLibros";
        botonGestionarLibros.Size = new Size(190, 36);
        botonGestionarLibros.TabIndex = 4;
        botonGestionarLibros.Text = "Gestionar libros";
        botonGestionarLibros.UseVisualStyleBackColor = true;
        botonGestionarPrestamos.Location = new Point(240, 148);
        botonGestionarPrestamos.Name = "botonGestionarPrestamos";
        botonGestionarPrestamos.Size = new Size(190, 36);
        botonGestionarPrestamos.TabIndex = 5;
        botonGestionarPrestamos.Text = "Préstamos y devoluciones";
        botonGestionarPrestamos.UseVisualStyleBackColor = true;
        botonGestionarReportes.Location = new Point(448, 148);
        botonGestionarReportes.Name = "botonGestionarReportes";
        botonGestionarReportes.Size = new Size(190, 36);
        botonGestionarReportes.TabIndex = 6;
        botonGestionarReportes.Text = "Consultas y reportes";
        botonGestionarReportes.UseVisualStyleBackColor = true;
        etiquetaEstadoConexion.AutoSize = true;
        etiquetaEstadoConexion.Location = new Point(32, 148);
        etiquetaEstadoConexion.Name = "etiquetaEstadoConexion";
        etiquetaEstadoConexion.Size = new Size(222, 15);
        etiquetaEstadoConexion.TabIndex = 1;
        etiquetaEstadoConexion.Text = "Servidor de desarrollo: localhost";
        AcceptButton = botonProbarConexion;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(880, 450);
        Controls.Add(etiquetaEstadoConexion);
        Controls.Add(botonGestionarEditoriales);
        Controls.Add(botonGestionarUsuarios);
        Controls.Add(botonGestionarAutores);
        Controls.Add(botonProbarConexion);
        Controls.Add(botonGestionarLibros);
        Controls.Add(botonGestionarPrestamos);
        Controls.Add(botonGestionarReportes);
        MinimumSize = new Size(880, 360);
        Name = "FormularioPrincipal";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sistema de Gestión de Biblioteca";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
