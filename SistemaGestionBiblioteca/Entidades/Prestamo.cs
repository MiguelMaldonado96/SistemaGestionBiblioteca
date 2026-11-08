namespace SistemaGestionBiblioteca.Entidades;

/// <summary>
/// Describe un préstamo, el libro y usuario relacionados y sus fechas.
/// </summary>
public sealed class Prestamo
{
    /// <summary>Identificador asignado al préstamo en la base de datos.</summary>
    public int PrestamoId { get; set; }
    /// <summary>Identificador de la persona que recibe el libro.</summary>
    public int UsuarioId { get; set; }
    /// <summary>Identificador del libro que se presta.</summary>
    public int LibroId { get; set; }
    /// <summary>Nombre de la persona usuaria, incluido para mostrarlo en pantalla.</summary>
    public string UsuarioNombre { get; set; } = string.Empty;
    /// <summary>Código del libro, incluido para mostrarlo en pantalla.</summary>
    public string CodigoLibro { get; set; } = string.Empty;
    /// <summary>Título del libro, incluido para mostrarlo en pantalla.</summary>
    public string LibroTitulo { get; set; } = string.Empty;
    /// <summary>Día en que comenzó el préstamo.</summary>
    public DateTime FechaPrestamo { get; set; }
    /// <summary>Día acordado para devolver el libro.</summary>
    public DateTime FechaDevolucionPrevista { get; set; }
    /// <summary>Día en que se devolvió el libro; queda vacío mientras siga prestado.</summary>
    public DateTime? FechaDevolucionReal { get; set; }
    /// <summary>Estado calculado por la base de datos: prestado o devuelto.</summary>
    public string Estado { get; set; } = string.Empty;
}
