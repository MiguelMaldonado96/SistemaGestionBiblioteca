namespace SistemaGestionBiblioteca.Entidades;

/// <summary>
/// Representa un libro, su editorial, sus autores y las existencias disponibles.
/// </summary>
public sealed class Libro
{
    /// <summary>Identificador asignado al libro en la base de datos.</summary>
    public int LibroId { get; set; }
    /// <summary>Código único usado por la biblioteca para ubicar el libro.</summary>
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Número ISBN del libro; puede no informarse.</summary>
    public string? ISBN { get; set; }
    /// <summary>Título del libro.</summary>
    public string Titulo { get; set; } = string.Empty;
    /// <summary>Año de publicación; puede no informarse.</summary>
    public short? AnioPublicacion { get; set; }
    /// <summary>Identificador de la editorial que publica el libro.</summary>
    public int EditorialId { get; set; }
    /// <summary>Nombre de la editorial, incluido para mostrarlo en pantalla.</summary>
    public string EditorialNombre { get; set; } = string.Empty;
    /// <summary>Número total de ejemplares del libro.</summary>
    public int CantidadTotal { get; set; }
    /// <summary>Número de ejemplares que no están prestados.</summary>
    public int CantidadDisponible { get; set; }
    /// <summary>Identificadores de las personas autoras del libro.</summary>
    public List<int> AutoresIds { get; set; } = [];
    /// <summary>Nombres de las personas autoras, incluidos para mostrar en pantalla.</summary>
    public List<string> AutoresNombres { get; set; } = [];
    /// <summary>Une los nombres de los autores para mostrarlos en una sola columna.</summary>
    public string AutoresTexto => string.Join(", ", AutoresNombres);
}
