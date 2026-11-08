using System.Net.Mail;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

/// <summary>
/// Valida y organiza las operaciones de usuarios antes de guardarlas.
/// </summary>
public sealed class UsuarioServicio
{
    private readonly UsuarioRepositorio usuarioRepositorio;

    /// <summary>Crea el servicio y le entrega el repositorio de usuarios.</summary>
    public UsuarioServicio(UsuarioRepositorio usuarioRepositorio)
    {
        this.usuarioRepositorio = usuarioRepositorio;
    }

    /// <summary>Obtiene todos los usuarios registrados.</summary>
    public Task<List<Usuario>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return usuarioRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

    /// <summary>Valida los datos y registra a un usuario nuevo.</summary>
    public Task CrearAsync(
        string nombre,
        string apellido,
        string documento,
        string? telefono,
        string? correo,
        CancellationToken tokenCancelacion = default)
    {
        ValidarDatos(nombre, apellido, documento, telefono, correo);
        return usuarioRepositorio.CrearAsync(
            CrearUsuario(nombre, apellido, documento, telefono, correo),
            tokenCancelacion);
    }

    /// <summary>Valida los datos y guarda los cambios del usuario indicado.</summary>
    public Task ActualizarAsync(
        int usuarioId,
        string nombre,
        string apellido,
        string documento,
        string? telefono,
        string? correo,
        CancellationToken tokenCancelacion = default)
    {
        if (usuarioId <= 0)
        {
            throw new ArgumentException("Selecciona un usuario válido.");
        }

        ValidarDatos(nombre, apellido, documento, telefono, correo);
        Usuario usuario = CrearUsuario(nombre, apellido, documento, telefono, correo);
        usuario.UsuarioId = usuarioId;
        return usuarioRepositorio.ActualizarAsync(usuario, tokenCancelacion);
    }

    /// <summary>Elimina el usuario si no tiene préstamos relacionados.</summary>
    public Task EliminarAsync(int usuarioId, CancellationToken tokenCancelacion = default)
    {
        if (usuarioId <= 0)
        {
            throw new ArgumentException("Selecciona un usuario válido.");
        }

        return usuarioRepositorio.EliminarAsync(usuarioId, tokenCancelacion);
    }

    /// <summary>Prepara los datos del usuario y limpia los espacios sobrantes.</summary>
    private static Usuario CrearUsuario(
        string nombre,
        string apellido,
        string documento,
        string? telefono,
        string? correo)
    {
        return new Usuario
        {
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Documento = documento.Trim(),
            Telefono = NormalizarOpcional(telefono),
            Correo = NormalizarOpcional(correo)
        };
    }

    /// <summary>Comprueba los datos obligatorios, sus longitudes y el formato del correo.</summary>
    private static void ValidarDatos(
        string nombre,
        string apellido,
        string documento,
        string? telefono,
        string? correo)
    {
        if (string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(apellido) ||
            string.IsNullOrWhiteSpace(documento))
        {
            throw new ArgumentException("Nombre, apellido y documento son obligatorios.");
        }

        if (nombre.Trim().Length > 80 ||
            apellido.Trim().Length > 80 ||
            documento.Trim().Length > 30 ||
            (telefono?.Trim().Length ?? 0) > 30 ||
            (correo?.Trim().Length ?? 0) > 254)
        {
            throw new ArgumentException("Uno o más campos superan la longitud permitida.");
        }

        if (!string.IsNullOrWhiteSpace(correo) &&
            (!MailAddress.TryCreate(correo.Trim(), out MailAddress? direccionCorreo) ||
             direccionCorreo.Address != correo.Trim()))
        {
            throw new ArgumentException("Escribe un correo electrónico válido.");
        }
    }

    /// <summary>Convierte un dato opcional vacío en nulo y quita espacios sobrantes.</summary>
    private static string? NormalizarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
