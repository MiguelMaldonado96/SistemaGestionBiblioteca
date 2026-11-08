using System.Net.Mail;
using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

public sealed class UsuarioServicio
{
    private readonly UsuarioRepositorio usuarioRepositorio;

    public UsuarioServicio(UsuarioRepositorio usuarioRepositorio)
    {
        this.usuarioRepositorio = usuarioRepositorio;
    }

    public Task<List<Usuario>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return usuarioRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

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

    public Task EliminarAsync(int usuarioId, CancellationToken tokenCancelacion = default)
    {
        if (usuarioId <= 0)
        {
            throw new ArgumentException("Selecciona un usuario válido.");
        }

        return usuarioRepositorio.EliminarAsync(usuarioId, tokenCancelacion);
    }

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

    private static string? NormalizarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
