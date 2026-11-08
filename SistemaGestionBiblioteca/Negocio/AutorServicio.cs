using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

/// <summary>
/// Valida y organiza las operaciones de autores antes de guardarlas.
/// </summary>
public sealed class AutorServicio
{
    private readonly AutorRepositorio autorRepositorio;

    /// <summary>Crea el servicio y le entrega el repositorio de autores.</summary>
    public AutorServicio(AutorRepositorio autorRepositorio)
    {
        this.autorRepositorio = autorRepositorio;
    }

    /// <summary>Obtiene todos los autores registrados.</summary>
    public Task<List<Autor>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return autorRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

    /// <summary>Revisa los datos y registra un autor nuevo.</summary>
    public Task CrearAsync(string nombre, string apellido, CancellationToken tokenCancelacion = default)
    {
        ValidarDatos(nombre, apellido);
        return autorRepositorio.CrearAsync(
            new Autor
            {
                Nombre = nombre.Trim(),
                Apellido = apellido.Trim()
            },
            tokenCancelacion);
    }

    /// <summary>Revisa los datos y guarda los cambios del autor indicado.</summary>
    public Task ActualizarAsync(
        int autorId,
        string nombre,
        string apellido,
        CancellationToken tokenCancelacion = default)
    {
        if (autorId <= 0)
        {
            throw new ArgumentException("Selecciona un autor válido.");
        }

        ValidarDatos(nombre, apellido);
        return autorRepositorio.ActualizarAsync(
            new Autor
            {
                AutorId = autorId,
                Nombre = nombre.Trim(),
                Apellido = apellido.Trim()
            },
            tokenCancelacion);
    }

    /// <summary>Elimina el autor si no está relacionado con libros.</summary>
    public Task EliminarAsync(int autorId, CancellationToken tokenCancelacion = default)
    {
        if (autorId <= 0)
        {
            throw new ArgumentException("Selecciona un autor válido.");
        }

        return autorRepositorio.EliminarAsync(autorId, tokenCancelacion);
    }

    /// <summary>Comprueba que el nombre y el apellido estén informados y no excedan su límite.</summary>
    private static void ValidarDatos(string nombre, string apellido)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
        {
            throw new ArgumentException("Escribe el nombre y el apellido del autor.");
        }

        if (nombre.Trim().Length > 80 || apellido.Trim().Length > 80)
        {
            throw new ArgumentException("El nombre y el apellido deben tener máximo 80 caracteres.");
        }
    }
}
