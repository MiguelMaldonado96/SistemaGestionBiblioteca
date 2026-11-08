#nullable enable
namespace SistemaGestionBiblioteca.Presentacion;

partial class FormularioPrincipal
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? componentes;

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
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Text = "Sistema de Gestión de Biblioteca";
    }

    #endregion
}
