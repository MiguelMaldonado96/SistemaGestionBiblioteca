-- Agrega registros de demostracion para completar las capturas de pantalla.
-- Los usuarios son ficticios y sus correos usan el dominio reservado .invalid.
USE [SistemaGestionBiblioteca];
GO

SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

DECLARE @usuariosDemostracion TABLE
(
    Nombre NVARCHAR(80) NOT NULL,
    Apellido NVARCHAR(80) NOT NULL,
    Documento NVARCHAR(30) NOT NULL,
    Correo NVARCHAR(254) NOT NULL
);

INSERT INTO @usuariosDemostracion (Nombre, Apellido, Documento, Correo)
VALUES
    (N'Camila', N'Prueba', N'DEMO-0001', N'usuario01@demo.invalid'),
    (N'Santiago', N'Prueba', N'DEMO-0002', N'usuario02@demo.invalid'),
    (N'Valentina', N'Prueba', N'DEMO-0003', N'usuario03@demo.invalid'),
    (N'Mateo', N'Prueba', N'DEMO-0004', N'usuario04@demo.invalid'),
    (N'Isabella', N'Prueba', N'DEMO-0005', N'usuario05@demo.invalid'),
    (N'Samuel', N'Prueba', N'DEMO-0006', N'usuario06@demo.invalid'),
    (N'Mariana', N'Prueba', N'DEMO-0007', N'usuario07@demo.invalid'),
    (N'Sebastian', N'Prueba', N'DEMO-0008', N'usuario08@demo.invalid'),
    (N'Gabriela', N'Prueba', N'DEMO-0009', N'usuario09@demo.invalid'),
    (N'Daniel', N'Prueba', N'DEMO-0010', N'usuario10@demo.invalid'),
    (N'Luciana', N'Prueba', N'DEMO-0011', N'usuario11@demo.invalid'),
    (N'Emiliano', N'Prueba', N'DEMO-0012', N'usuario12@demo.invalid'),
    (N'Antonella', N'Prueba', N'DEMO-0013', N'usuario13@demo.invalid'),
    (N'Gabriel', N'Prueba', N'DEMO-0014', N'usuario14@demo.invalid'),
    (N'Julieta', N'Prueba', N'DEMO-0015', N'usuario15@demo.invalid'),
    (N'Benjamin', N'Prueba', N'DEMO-0016', N'usuario16@demo.invalid'),
    (N'Veronica', N'Prueba', N'DEMO-0017', N'usuario17@demo.invalid'),
    (N'Andres', N'Prueba', N'DEMO-0018', N'usuario18@demo.invalid'),
    (N'Natalia', N'Prueba', N'DEMO-0019', N'usuario19@demo.invalid'),
    (N'Felipe', N'Prueba', N'DEMO-0020', N'usuario20@demo.invalid');

INSERT INTO dbo.Usuarios (Nombre, Apellido, Documento, Correo)
SELECT usuario.Nombre, usuario.Apellido, usuario.Documento, usuario.Correo
FROM @usuariosDemostracion AS usuario
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Usuarios AS usuarioExistente
    WHERE usuarioExistente.Documento = usuario.Documento
);

-- Corrige el texto de la editorial asociada al catálogo de demostración.
UPDATE editorial
SET Nombre = N'Editorial de demostración',
    Pais = N'Colombia'
FROM dbo.Editoriales AS editorial
WHERE editorial.EditorialId =
(
    SELECT TOP (1) libro.EditorialId
    FROM dbo.Libros AS libro
    WHERE libro.Codigo = N'COL-001'
);

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Editoriales
    WHERE Nombre = N'Editorial de demostración'
)
BEGIN
    INSERT INTO dbo.Editoriales (Nombre, Pais)
    VALUES (N'Editorial de demostración', N'Colombia');
END;

DECLARE @librosDemostracion TABLE
(
    Codigo NVARCHAR(20) NOT NULL,
    Titulo NVARCHAR(200) NOT NULL,
    AnioPublicacion SMALLINT NOT NULL,
    NombreAutor NVARCHAR(80) NOT NULL,
    ApellidoAutor NVARCHAR(80) NOT NULL
);

INSERT INTO @librosDemostracion
    (Codigo, Titulo, AnioPublicacion, NombreAutor, ApellidoAutor)
VALUES
    (N'COL-001', N'Cien años de soledad', 1967, N'Gabriel', N'García Márquez'),
    (N'COL-002', N'El amor en los tiempos del cólera', 1985, N'Gabriel', N'García Márquez'),
    (N'COL-003', N'Crónica de una muerte anunciada', 1981, N'Gabriel', N'García Márquez'),
    (N'COL-004', N'El coronel no tiene quien le escriba', 1961, N'Gabriel', N'García Márquez'),
    (N'COL-005', N'La vorágine', 1924, N'José Eustasio', N'Rivera'),
    (N'COL-006', N'María', 1867, N'Jorge', N'Isaacs'),
    (N'COL-007', N'La tejedora de coronas', 1982, N'Germán', N'Espinosa'),
    (N'COL-008', N'Cóndores no entierran todos los días', 1972, N'Gustavo Álvarez', N'Gardeazábal'),
    (N'COL-009', N'¡Que viva la música!', 1977, N'Andrés', N'Caicedo'),
    (N'COL-010', N'El olvido que seremos', 2006, N'Héctor Abad', N'Faciolince'),
    (N'COL-011', N'Delirio', 2004, N'Laura', N'Restrepo'),
    (N'COL-012', N'Rosario Tijeras', 1999, N'Jorge', N'Franco'),
    (N'COL-013', N'Satanás', 2002, N'Mario', N'Mendoza'),
    (N'COL-014', N'El ruido de las cosas al caer', 2011, N'Juan Gabriel', N'Vásquez'),
    (N'COL-015', N'Los ejércitos', 2007, N'Evelio', N'Rosero'),
    (N'COL-016', N'La perra', 2017, N'Pilar', N'Quintana'),
    (N'COL-017', N'La casa de las dos palmas', 1988, N'Manuel Mejía', N'Vallejo'),
    (N'COL-018', N'La multitud errante', 2001, N'Laura', N'Restrepo'),
    (N'COL-019', N'Plegarias nocturnas', 2012, N'Santiago', N'Gamboa'),
    (N'COL-020', N'El día del odio', 1952, N'José Antonio Osorio', N'Lizarazo');

-- Repara nombres que pudieron guardarse con una codificación incorrecta.
UPDATE libro
SET Titulo = demostracion.Titulo
FROM dbo.Libros AS libro
INNER JOIN @librosDemostracion AS demostracion
    ON demostracion.Codigo = libro.Codigo;

UPDATE autor
SET Nombre = demostracion.NombreAutor,
    Apellido = demostracion.ApellidoAutor
FROM dbo.Autores AS autor
INNER JOIN dbo.LibroAutores AS relacion ON relacion.AutorId = autor.AutorId
INNER JOIN dbo.Libros AS libro ON libro.LibroId = relacion.LibroId
INNER JOIN @librosDemostracion AS demostracion
    ON demostracion.Codigo = libro.Codigo;

INSERT INTO dbo.Autores (Nombre, Apellido)
SELECT DISTINCT libro.NombreAutor, libro.ApellidoAutor
FROM @librosDemostracion AS libro
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Autores AS autorExistente
    WHERE autorExistente.Nombre = libro.NombreAutor
      AND autorExistente.Apellido = libro.ApellidoAutor
);

DECLARE @editorialId INT =
(
    SELECT EditorialId
    FROM dbo.Editoriales
    WHERE Nombre = N'Editorial de demostración'
);

INSERT INTO dbo.Libros
    (Codigo, Titulo, AnioPublicacion, EditorialId, CantidadTotal, CantidadDisponible)
SELECT libro.Codigo, libro.Titulo, libro.AnioPublicacion, @editorialId, 3, 3
FROM @librosDemostracion AS libro
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Libros AS libroExistente
    WHERE libroExistente.Codigo = libro.Codigo
);

INSERT INTO dbo.LibroAutores (LibroId, AutorId)
SELECT libroExistente.LibroId, autorExistente.AutorId
FROM @librosDemostracion AS libro
INNER JOIN dbo.Libros AS libroExistente ON libroExistente.Codigo = libro.Codigo
INNER JOIN dbo.Autores AS autorExistente
    ON autorExistente.Nombre = libro.NombreAutor
   AND autorExistente.Apellido = libro.ApellidoAutor
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.LibroAutores AS relacionExistente
    WHERE relacionExistente.LibroId = libroExistente.LibroId
      AND relacionExistente.AutorId = autorExistente.AutorId
);

-- Agrega un préstamo devuelto de muestra sin modificar las existencias.
INSERT INTO dbo.Prestamos
    (UsuarioId, LibroId, FechaPrestamo, FechaDevolucionPrevista, FechaDevolucionReal)
SELECT
    usuario.UsuarioId,
    libro.LibroId,
    DATEADD(DAY, -14, CONVERT(DATE, GETDATE())),
    DATEADD(DAY, -7, CONVERT(DATE, GETDATE())),
    DATEADD(DAY, -7, CONVERT(DATE, GETDATE()))
FROM dbo.Usuarios AS usuario
CROSS JOIN dbo.Libros AS libro
WHERE usuario.Documento = N'DEMO-0001'
  AND libro.Codigo = N'COL-001'
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.Prestamos AS prestamo
      WHERE prestamo.UsuarioId = usuario.UsuarioId
        AND prestamo.LibroId = libro.LibroId
        AND prestamo.FechaDevolucionReal IS NOT NULL
  );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;
GO

-- Muestra un resumen para comprobar que se cargaron los datos.
SELECT
    (SELECT COUNT(*) FROM dbo.Usuarios WHERE Documento LIKE N'DEMO-%') AS UsuariosDemostracion,
    (SELECT COUNT(*) FROM dbo.Libros WHERE Codigo LIKE N'COL-%') AS LibrosDemostracion,
    (SELECT COUNT(*) FROM dbo.Prestamos WHERE FechaDevolucionReal IS NULL) AS PrestamosActivos,
    (SELECT COUNT(*) FROM dbo.Prestamos WHERE FechaDevolucionReal IS NOT NULL) AS PrestamosDevueltos,
    (SELECT COUNT(*) FROM dbo.Autores AS autor
     WHERE EXISTS
     (
         SELECT 1
         FROM dbo.LibroAutores AS relacion
         INNER JOIN dbo.Libros AS libro ON libro.LibroId = relacion.LibroId
         WHERE relacion.AutorId = autor.AutorId AND libro.Codigo LIKE N'COL-%'
     )) AS AutoresRelacionados;
