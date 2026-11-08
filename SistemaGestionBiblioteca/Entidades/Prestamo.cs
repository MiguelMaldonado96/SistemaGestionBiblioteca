namespace SistemaGestionBiblioteca.Entidades;

public sealed class Prestamo
{
    public int PrestamoId { get; set; }
    public int UsuarioId { get; set; }
    public int LibroId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string CodigoLibro { get; set; } = string.Empty;
    public string LibroTitulo { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaDevolucionPrevista { get; set; }
    public DateTime? FechaDevolucionReal { get; set; }
    public string Estado { get; set; } = string.Empty;
}
