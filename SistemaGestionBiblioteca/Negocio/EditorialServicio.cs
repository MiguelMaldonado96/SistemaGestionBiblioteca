using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

/// <summary>
/// Valida y organiza las operaciones de editoriales antes de guardarlas.
/// </summary>
public sealed class EditorialServicio
{
    private readonly EditorialRepositorio editorialRepositorio;

    /// <summary>Crea el servicio y le entrega el repositorio de editoriales.</summary>
    public EditorialServicio(EditorialRepositorio editorialRepositorio)
    {
        this.editorialRepositorio = editorialRepositorio;
    }

    /// <summary>Obtiene todas las editoriales registradas.</summary>
    public Task<List<Editorial>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return editorialRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

    /// <summary>Revisa los datos y registra una editorial nueva.</summary>
    public Task CrearAsync(string nombre, string? pais, CancellationToken tokenCancelacion = default)
    {
        ValidarDatos(nombre, pais);
        return editorialRepositorio.CrearAsync(
            new Editorial
            {
                Nombre = nombre.Trim(),
                Pais = NormalizarPais(pais)
            },
            tokenCancelacion);
    }

    /// <summary>Revisa los datos y guarda los cambios de la editorial indicada.</summary>
    public Task ActualizarAsync(
        int editorialId,
        string nombre,
        string? pais,
        CancellationToken tokenCancelacion = default)
    {
        if (editorialId <= 0)
        {
            throw new ArgumentException("Selecciona una editorial válida.");
        }

        ValidarDatos(nombre, pais);
        return editorialRepositorio.ActualizarAsync(
            new Editorial
            {
                EditorialId = editorialId,
                Nombre = nombre.Trim(),
                Pais = NormalizarPais(pais)
            },
            tokenCancelacion);
    }

    /// <summary>Elimina la editorial si no publica libros registrados.</summary>
    public Task EliminarAsync(int editorialId, CancellationToken tokenCancelacion = default)
    {
        if (editorialId <= 0)
        {
            throw new ArgumentException("Selecciona una editorial válida.");
        }

        return editorialRepositorio.EliminarAsync(editorialId, tokenCancelacion);
    }

    /// <summary>Comprueba que el nombre y el país respeten los campos obligatorios y sus límites.</summary>
    private static void ValidarDatos(string nombre, string? pais)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("Escribe el nombre de la editorial.");
        }

        if (nombre.Trim().Length > 120 || (pais?.Trim().Length ?? 0) > 80)
        {
            throw new ArgumentException(
                "El nombre admite máximo 120 caracteres y el país máximo 80 caracteres.");
        }
    }

    /// <summary>Convierte un país vacío en un valor nulo y quita espacios sobrantes.</summary>
    private static string? NormalizarPais(string? pais)
    {
        return string.IsNullOrWhiteSpace(pais) ? null : pais.Trim();
    }
}
