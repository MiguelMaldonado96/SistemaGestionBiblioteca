namespace SistemaGestionBiblioteca.Entidades;

/// <summary>
/// Guarda la información de una persona que puede pedir libros prestados.
/// </summary>
public sealed class Usuario
{
    /// <summary>Identificador asignado al usuario en la base de datos.</summary>
    public int UsuarioId { get; set; }
    /// <summary>Nombre del usuario.</summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Apellido del usuario.</summary>
    public string Apellido { get; set; } = string.Empty;
    /// <summary>Documento único usado para identificar al usuario.</summary>
    public string Documento { get; set; } = string.Empty;
    /// <summary>Teléfono de contacto; puede no informarse.</summary>
    public string? Telefono { get; set; }
    /// <summary>Correo electrónico de contacto; puede no informarse.</summary>
    public string? Correo { get; set; }
}
