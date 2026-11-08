using System.Data;
using Microsoft.Data.SqlClient;

namespace SistemaGestionBiblioteca.Datos;

public sealed class ReporteRepositorio
{
    private readonly ConexionBaseDatos conexionBaseDatos;

    public ReporteRepositorio(ConexionBaseDatos conexionBaseDatos)
    {
        this.conexionBaseDatos = conexionBaseDatos;
    }

    public Task<DataTable> ObtenerResumenAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT Indicador, Cantidad
            FROM
            (
                SELECT N'Libros registrados' AS Indicador, COUNT(*) AS Cantidad FROM dbo.Libros
                UNION ALL
                SELECT N'Ejemplares disponibles', COALESCE(SUM(CantidadDisponible), 0) FROM dbo.Libros
                UNION ALL
                SELECT N'Autores registrados', COUNT(*) FROM dbo.Autores
                UNION ALL
                SELECT N'Editoriales registradas', COUNT(*) FROM dbo.Editoriales
                UNION ALL
                SELECT N'Usuarios registrados', COUNT(*) FROM dbo.Usuarios
                UNION ALL
                SELECT N'Préstamos activos', COUNT(*) FROM dbo.Prestamos WHERE FechaDevolucionReal IS NULL
                UNION ALL
                SELECT N'Préstamos devueltos', COUNT(*) FROM dbo.Prestamos WHERE FechaDevolucionReal IS NOT NULL
            ) AS resumen
            ORDER BY Indicador;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    public Task<DataTable> ObtenerLibrosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT l.Codigo, l.Titulo, l.ISBN, l.AnioPublicacion AS Anio,
                   e.Nombre AS Editorial, autores.NombresAutores AS Autores,
                   l.CantidadTotal AS Ejemplares,
                   l.CantidadDisponible AS Disponibles
            FROM dbo.Libros AS l
            INNER JOIN dbo.Editoriales AS e ON e.EditorialId = l.EditorialId
            OUTER APPLY
            (
                SELECT STRING_AGG(
                    CONVERT(NVARCHAR(MAX), CONCAT(a.Nombre, N' ', a.Apellido)),
                    N', '
                ) AS NombresAutores
                FROM dbo.LibroAutores AS la
                INNER JOIN dbo.Autores AS a ON a.AutorId = la.AutorId
                WHERE la.LibroId = l.LibroId
            ) AS autores
            ORDER BY l.Titulo;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    public Task<DataTable> ObtenerAutoresAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT a.Nombre, a.Apellido, COUNT(la.LibroId) AS LibrosRelacionados
            FROM dbo.Autores AS a
            LEFT JOIN dbo.LibroAutores AS la ON la.AutorId = a.AutorId
            GROUP BY a.Nombre, a.Apellido
            ORDER BY a.Apellido, a.Nombre;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    public Task<DataTable> ObtenerEditorialesAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT e.Nombre, e.Pais, COUNT(l.LibroId) AS LibrosRegistrados
            FROM dbo.Editoriales AS e
            LEFT JOIN dbo.Libros AS l ON l.EditorialId = e.EditorialId
            GROUP BY e.Nombre, e.Pais
            ORDER BY e.Nombre;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    public Task<DataTable> ObtenerUsuariosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT u.Nombre, u.Apellido, u.Documento, u.Telefono, u.Correo,
                   COUNT(p.PrestamoId) AS PrestamosRegistrados
            FROM dbo.Usuarios AS u
            LEFT JOIN dbo.Prestamos AS p ON p.UsuarioId = u.UsuarioId
            GROUP BY u.Nombre, u.Apellido, u.Documento, u.Telefono, u.Correo
            ORDER BY u.Apellido, u.Nombre;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    public Task<DataTable> ObtenerPrestamosActivosAsync(CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT u.Nombre + N' ' + u.Apellido AS Usuario,
                   l.Codigo AS CodigoLibro,
                   l.Titulo AS Libro,
                   p.FechaPrestamo,
                   p.FechaDevolucionPrevista,
                   CASE
                       WHEN p.FechaDevolucionPrevista < CONVERT(DATE, GETDATE())
                       THEN DATEDIFF(DAY, p.FechaDevolucionPrevista, CONVERT(DATE, GETDATE()))
                       ELSE 0
                   END AS DiasDeRetraso
            FROM dbo.Prestamos AS p
            INNER JOIN dbo.Usuarios AS u ON u.UsuarioId = p.UsuarioId
            INNER JOIN dbo.Libros AS l ON l.LibroId = p.LibroId
            WHERE p.FechaDevolucionReal IS NULL
            ORDER BY p.FechaDevolucionPrevista, l.Titulo;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    public Task<DataTable> ObtenerHistorialDevolucionesAsync(
        CancellationToken tokenCancelacion = default)
    {
        const string consulta = """
            SELECT u.Nombre + N' ' + u.Apellido AS Usuario,
                   l.Codigo AS CodigoLibro,
                   l.Titulo AS Libro,
                   p.FechaPrestamo,
                   p.FechaDevolucionPrevista,
                   p.FechaDevolucionReal
            FROM dbo.Prestamos AS p
            INNER JOIN dbo.Usuarios AS u ON u.UsuarioId = p.UsuarioId
            INNER JOIN dbo.Libros AS l ON l.LibroId = p.LibroId
            WHERE p.FechaDevolucionReal IS NOT NULL
            ORDER BY p.FechaDevolucionReal DESC, l.Titulo;
            """;

        return EjecutarConsultaAsync(consulta, tokenCancelacion);
    }

    private async Task<DataTable> EjecutarConsultaAsync(
        string consulta,
        CancellationToken tokenCancelacion)
    {
        await using var conexion = conexionBaseDatos.CrearConexion();
        await conexion.OpenAsync(tokenCancelacion);
        await using var comando = new SqlCommand(consulta, conexion);
        await using var lector = await comando.ExecuteReaderAsync(tokenCancelacion);

        var tabla = new DataTable();
        for (int indiceColumna = 0; indiceColumna < lector.FieldCount; indiceColumna++)
        {
            tabla.Columns.Add(lector.GetName(indiceColumna), lector.GetFieldType(indiceColumna));
        }

        while (await lector.ReadAsync(tokenCancelacion))
        {
            DataRow fila = tabla.NewRow();
            for (int indiceColumna = 0; indiceColumna < lector.FieldCount; indiceColumna++)
            {
                fila[indiceColumna] = lector.IsDBNull(indiceColumna)
                    ? DBNull.Value
                    : lector.GetValue(indiceColumna);
            }

            tabla.Rows.Add(fila);
        }

        return tabla;
    }
}
