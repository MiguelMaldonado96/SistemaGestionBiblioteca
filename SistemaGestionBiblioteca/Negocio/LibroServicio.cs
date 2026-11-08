using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

/// <summary>
/// Valida los libros y prepara sus datos para el repositorio.
/// </summary>
public sealed class LibroServicio
{
    private readonly LibroRepositorio libroRepositorio;

    /// <summary>Crea el servicio y le entrega el repositorio de libros.</summary>
    public LibroServicio(LibroRepositorio libroRepositorio)
    {
        this.libroRepositorio = libroRepositorio;
    }

    /// <summary>Obtiene los libros con su editorial y autores relacionados.</summary>
    public Task<List<Libro>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return libroRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

    /// <summary>Valida los datos y registra un libro con sus autores.</summary>
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

    /// <summary>Valida los datos y guarda los cambios del libro indicado.</summary>
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

    /// <summary>Elimina el libro si no tiene préstamos relacionados.</summary>
    public Task EliminarAsync(int libroId, CancellationToken tokenCancelacion = default)
    {
        if (libroId <= 0)
        {
            throw new ArgumentException("Selecciona un libro válido.");
        }

        return libroRepositorio.EliminarAsync(libroId, tokenCancelacion);
    }

    /// <summary>Construye el libro y elimina autores repetidos de la selección.</summary>
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

    /// <summary>Comprueba los campos obligatorios, las cantidades y las relaciones del libro.</summary>
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

    /// <summary>Convierte un dato opcional vacío en nulo y quita espacios sobrantes.</summary>
    private static string? NormalizarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
