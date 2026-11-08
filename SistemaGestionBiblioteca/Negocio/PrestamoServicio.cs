using SistemaGestionBiblioteca.Datos;
using SistemaGestionBiblioteca.Entidades;

namespace SistemaGestionBiblioteca.Negocio;

public sealed class PrestamoServicio
{
    private readonly PrestamoRepositorio prestamoRepositorio;

    public PrestamoServicio(PrestamoRepositorio prestamoRepositorio)
    {
        this.prestamoRepositorio = prestamoRepositorio;
    }

    public Task<List<Prestamo>> ObtenerTodosAsync(CancellationToken tokenCancelacion = default)
    {
        return prestamoRepositorio.ObtenerTodosAsync(tokenCancelacion);
    }

    public Task RegistrarAsync(
        int usuarioId,
        int libroId,
        DateTime fechaPrestamo,
        DateTime fechaDevolucionPrevista,
        CancellationToken tokenCancelacion = default)
    {
        ValidarDatos(usuarioId, libroId, fechaPrestamo, fechaDevolucionPrevista);
        return prestamoRepositorio.RegistrarAsync(
            new Prestamo
            {
                UsuarioId = usuarioId,
                LibroId = libroId,
                FechaPrestamo = fechaPrestamo.Date,
                FechaDevolucionPrevista = fechaDevolucionPrevista.Date
            },
            tokenCancelacion);
    }

    public Task ActualizarAsync(
        int prestamoId,
        int usuarioId,
        int libroId,
        DateTime fechaPrestamo,
        DateTime fechaDevolucionPrevista,
        CancellationToken tokenCancelacion = default)
    {
        if (prestamoId <= 0)
        {
            throw new ArgumentException("Selecciona un préstamo válido.");
        }

        ValidarDatos(usuarioId, libroId, fechaPrestamo, fechaDevolucionPrevista);
        return prestamoRepositorio.ActualizarAsync(
            new Prestamo
            {
                PrestamoId = prestamoId,
                UsuarioId = usuarioId,
                LibroId = libroId,
                FechaPrestamo = fechaPrestamo.Date,
                FechaDevolucionPrevista = fechaDevolucionPrevista.Date
            },
            tokenCancelacion);
    }

    public Task RegistrarDevolucionAsync(
        int prestamoId,
        DateTime fechaDevolucionReal,
        CancellationToken tokenCancelacion = default)
    {
        if (prestamoId <= 0)
        {
            throw new ArgumentException("Selecciona un préstamo válido.");
        }

        if (fechaDevolucionReal.Date > DateTime.Today)
        {
            throw new ArgumentException("La fecha de devolución no puede estar en el futuro.");
        }

        return prestamoRepositorio.RegistrarDevolucionAsync(
            prestamoId,
            fechaDevolucionReal.Date,
            tokenCancelacion);
    }

    public Task EliminarAsync(int prestamoId, CancellationToken tokenCancelacion = default)
    {
        if (prestamoId <= 0)
        {
            throw new ArgumentException("Selecciona un préstamo válido.");
        }

        return prestamoRepositorio.EliminarAsync(prestamoId, tokenCancelacion);
    }

    private static void ValidarDatos(
        int usuarioId,
        int libroId,
        DateTime fechaPrestamo,
        DateTime fechaDevolucionPrevista)
    {
        if (usuarioId <= 0 || libroId <= 0)
        {
            throw new ArgumentException("Selecciona un usuario y un libro válidos.");
        }

        if (fechaPrestamo.Date > DateTime.Today)
        {
            throw new ArgumentException("La fecha del préstamo no puede estar en el futuro.");
        }

        if (fechaDevolucionPrevista.Date < fechaPrestamo.Date)
        {
            throw new ArgumentException(
                "La fecha prevista de devolución no puede ser anterior a la fecha del préstamo.");
        }
    }
}
