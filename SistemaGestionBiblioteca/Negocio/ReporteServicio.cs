using System.Data;
using SistemaGestionBiblioteca.Datos;

namespace SistemaGestionBiblioteca.Negocio;

public sealed class ReporteServicio
{
    private readonly ReporteRepositorio reporteRepositorio;

    public ReporteServicio(ReporteRepositorio reporteRepositorio)
    {
        this.reporteRepositorio = reporteRepositorio;
    }

    public Task<DataTable> ObtenerResumenAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerResumenAsync(tokenCancelacion);
    }

    public Task<DataTable> ObtenerLibrosAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerLibrosAsync(tokenCancelacion);
    }

    public Task<DataTable> ObtenerAutoresAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerAutoresAsync(tokenCancelacion);
    }

    public Task<DataTable> ObtenerEditorialesAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerEditorialesAsync(tokenCancelacion);
    }

    public Task<DataTable> ObtenerUsuariosAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerUsuariosAsync(tokenCancelacion);
    }

    public Task<DataTable> ObtenerPrestamosActivosAsync(CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerPrestamosActivosAsync(tokenCancelacion);
    }

    public Task<DataTable> ObtenerHistorialDevolucionesAsync(
        CancellationToken tokenCancelacion = default)
    {
        return reporteRepositorio.ObtenerHistorialDevolucionesAsync(tokenCancelacion);
    }
}
