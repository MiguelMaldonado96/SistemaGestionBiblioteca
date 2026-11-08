namespace SistemaGestionBiblioteca.Entidades;

public sealed class Editorial
{
    public int EditorialId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Pais { get; set; }
}
