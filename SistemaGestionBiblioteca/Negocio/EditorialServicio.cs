using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

public sealed class EditorialServicio
{
    private readonly EditorialRepositorio editorialRepositorio;

    public EditorialServicio(EditorialRepositorio editorialRepositorio)
    {
        this.editorialRepositorio = editorialRepositorio;
    }

    public Task<List<Editorial>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return editorialRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

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

    public Task EliminarAsync(int editorialId, CancellationToken tokenCancelacion = default)
    {
        if (editorialId <= 0)
        {
            throw new ArgumentException("Selecciona una editorial válida.");
        }

        return editorialRepositorio.EliminarAsync(editorialId, tokenCancelacion);
    }

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

    private static string? NormalizarPais(string? pais)
    {
        return string.IsNullOrWhiteSpace(pais) ? null : pais.Trim();
    }
}
