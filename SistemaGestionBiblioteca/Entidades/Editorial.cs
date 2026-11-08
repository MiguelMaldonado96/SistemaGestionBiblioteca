namespace SistemaGestionBiblioteca.Entidades;

/// <summary>
/// Representa la editorial que publica uno o varios libros.
/// </summary>
public sealed class Editorial
{
    /// <summary>Identificador asignado a la editorial en la base de datos.</summary>
    public int EditorialId { get; set; }
    /// <summary>Nombre de la editorial.</summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>País donde se encuentra la editorial; puede no informarse.</summary>
    public string? Pais { get; set; }
}
