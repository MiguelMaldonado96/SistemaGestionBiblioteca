namespace SistemaGestionBiblioteca.Entidades;

/// <summary>
/// Guarda el código y los datos de identificación de un autor.
/// </summary>
public sealed class Autor
{
    /// <summary>Identificador asignado al autor en la base de datos.</summary>
    public int AutorId { get; set; }
    /// <summary>Nombre del autor.</summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Apellido del autor.</summary>
    public string Apellido { get; set; } = string.Empty;
}
