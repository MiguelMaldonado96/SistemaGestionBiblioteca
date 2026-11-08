namespace SistemaGestionBiblioteca.Entidades;

public sealed class Autor
{
    public int AutorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}
