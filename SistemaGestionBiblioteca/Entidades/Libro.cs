namespace SistemaGestionBiblioteca.Entidades;

public sealed class Libro
{
    public int LibroId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public short? AnioPublicacion { get; set; }
    public int EditorialId { get; set; }
    public string EditorialNombre { get; set; } = string.Empty;
    public int CantidadTotal { get; set; }
    public int CantidadDisponible { get; set; }
    public List<int> AutoresIds { get; set; } = [];
    public List<string> AutoresNombres { get; set; } = [];
    public string AutoresTexto => string.Join(", ", AutoresNombres);
}
