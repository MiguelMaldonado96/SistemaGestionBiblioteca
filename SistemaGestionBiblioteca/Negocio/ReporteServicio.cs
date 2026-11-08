using System.Data;
using SistemaGestionBiblioteca.Datos;

namespace SistemaGestionBiblioteca.Negocio;

/// <summary>
/// Solicita los datos que la pantalla de reportes muestra a la persona usuaria.
/// </summary>
public sealed class ReporteServicio
{
    private readonly ReporteRepositorio reporteRepositorio;

    /// <summary>Crea el servicio y le entrega el repositorio de reportes.</summary>
    public ReporteServicio(ReporteRepositorio reporteRepositorio)
    {
        this.reporteRepositorio = reporteRepositorio;
    }

    /// <summary>Obtiene los conteos principales de la biblioteca.</summary>
    public Task<DataTable> ObtenerResumenAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerResumenAsync(tokenCancelacion);
    }

    /// <summary>Obtiene el catálogo de libros y sus existencias.</summary>
    public Task<DataTable> ObtenerLibrosAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerLibrosAsync(tokenCancelacion);
    }

    /// <summary>Obtiene la cantidad de libros relacionados con cada autor.</summary>
    public Task<DataTable> ObtenerAutoresAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerAutoresAsync(tokenCancelacion);
    }

    /// <summary>Obtiene las editoriales y el número de libros de cada una.</summary>
    public Task<DataTable> ObtenerEditorialesAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerEditorialesAsync(tokenCancelacion);
    }

    /// <summary>Obtiene los usuarios y el número de préstamos registrados a su nombre.</summary>
    public Task<DataTable> ObtenerUsuariosAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerUsuariosAsync(tokenCancelacion);
    }

    /// <summary>Obtiene los préstamos que todavía no se han devuelto.</summary>
    public Task<DataTable> ObtenerPrestamosActivosAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerPrestamosActivosAsync(tokenCancelacion);
    }

    /// <summary>Obtiene los préstamos que ya tienen registrada su devolución.</summary>
    public Task<DataTable> ObtenerHistorialDevolucionesAsync(
        CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerHistorialDevolucionesAsync(tokenCancelacion);
    }
}
