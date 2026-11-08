using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

public sealed class AutorServicio
{
    private readonly AutorRepositorio autorRepositorio;

    public AutorServicio(AutorRepositorio autorRepositorio)
    {
        this.autorRepositorio = autorRepositorio;
    }

    public Task<List<Autor>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return autorRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

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

    public Task EliminarAsync(int autorId, CancellationToken tokenCancelacion = default)
    {
        if (autorId <= 0)
        {
            throw new ArgumentException("Selecciona un autor válido.");
        }

        return autorRepositorio.EliminarAsync(autorId, tokenCancelacion);
    }

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
