using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

public sealed class LibroServicio
{
    private readonly LibroRepositorio libroRepositorio;

    public LibroServicio(LibroRepositorio libroRepositorio)
    {
        this.libroRepositorio = libroRepositorio;
    }

    public Task<List<Libro>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return libroRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

    public Task CrearAsync(
        string codigo,
        string? isbn,
        string titulo,
        short? anioPublicacion,
        int editorialId,
        int cantidadTotal,
        IReadOnlyCollection<int> autoresIds,
        CancellationToken tokenCancelacion = default)
    {
        ValidarDatos(codigo, isbn, titulo, anioPublicacion, editorialId, cantidadTotal, autoresIds);
        return libroRepositorio.CrearAsync(
            CrearLibro(codigo, isbn, titulo, anioPublicacion, editorialId, cantidadTotal, autoresIds),
            tokenCancelacion);
    }

    public Task ActualizarAsync(
        int libroId,
        string codigo,
        string? isbn,
        string titulo,
        short? anioPublicacion,
        int editorialId,
        int cantidadTotal,
        IReadOnlyCollection<int> autoresIds,
        CancellationToken tokenCancelacion = default)
    {
        if (libroId <= 0)
        {
            throw new ArgumentException("Selecciona un libro válido.");
        }

        ValidarDatos(codigo, isbn, titulo, anioPublicacion, editorialId, cantidadTotal, autoresIds);
        Libro libro = CrearLibro(codigo, isbn, titulo, anioPublicacion, editorialId, cantidadTotal, autoresIds);
        libro.LibroId = libroId;
        return libroRepositorio.ActualizarAsync(libro, tokenCancelacion);
    }

    public Task EliminarAsync(int libroId, CancellationToken tokenCancelacion = default)
    {
        if (libroId <= 0)
        {
            throw new ArgumentException("Selecciona un libro válido.");
        }

        return libroRepositorio.EliminarAsync(libroId, tokenCancelacion);
    }

    private static Libro CrearLibro(
        string codigo,
        string? isbn,
        string titulo,
        short? anioPublicacion,
        int editorialId,
        int cantidadTotal,
        IReadOnlyCollection<int> autoresIds)
    {
        return new Libro
        {
            Codigo = codigo.Trim(),
            ISBN = NormalizarOpcional(isbn),
            Titulo = titulo.Trim(),
            AnioPublicacion = anioPublicacion,
            EditorialId = editorialId,
            CantidadTotal = cantidadTotal,
            CantidadDisponible = cantidadTotal,
            AutoresIds = autoresIds.Distinct().ToList()
        };
    }

    private static void ValidarDatos(
        string codigo,
        string? isbn,
        string titulo,
        short? anioPublicacion,
        int editorialId,
        int cantidadTotal,
        IReadOnlyCollection<int> autoresIds)
    {
        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("El código y el título del libro son obligatorios.");
        }

        if (codigo.Trim().Length > 20 ||
            (isbn?.Trim().Length ?? 0) > 20 ||
            titulo.Trim().Length > 200)
        {
            throw new ArgumentException("El código admite 20, el ISBN 20 y el título 200 caracteres como máximo.");
        }

        if (anioPublicacion is < 1000 or > 2200)
        {
            throw new ArgumentException("El año de publicación debe estar entre 1000 y 2200.");
        }

        if (editorialId <= 0)
        {
            throw new ArgumentException("Selecciona una editorial.");
        }

        if (cantidadTotal < 1)
        {
            throw new ArgumentException("El libro debe tener al menos un ejemplar.");
        }

        if (autoresIds.Count == 0 || autoresIds.Any(autorId => autorId <= 0))
        {
            throw new ArgumentException("Selecciona al menos un autor válido.");
        }
    }

    private static string? NormalizarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
