namespace SistemaGestionBiblioteca.Presentacion;

/// <summary>
/// Inicia la aplicación de Windows Forms.
/// </summary>
static class Program
{
    /// <summary>
    /// Prepara Windows Forms y abre la pantalla principal.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormularioPrincipal());
    }
}