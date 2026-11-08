IF DB_ID(N'SistemaGestionBiblioteca') IS NULL
BEGIN
    CREATE DATABASE [SistemaGestionBiblioteca];
END;
GO

USE [SistemaGestionBiblioteca];
GO

SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

IF OBJECT_ID(N'dbo.Autores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Autores
    (
        AutorId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Autores PRIMARY KEY,
        Nombre NVARCHAR(80) NOT NULL,
        Apellido NVARCHAR(80) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Editoriales', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Editoriales
    (
        EditorialId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Editoriales PRIMARY KEY,
        Nombre NVARCHAR(120) NOT NULL,
        Pais NVARCHAR(80) NULL,
        CONSTRAINT UQ_Editoriales_Nombre UNIQUE (Nombre)
    );
END;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        UsuarioId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
        Nombre NVARCHAR(80) NOT NULL,
        Apellido NVARCHAR(80) NOT NULL,
        Documento NVARCHAR(30) NOT NULL,
        Telefono NVARCHAR(30) NULL,
        Correo NVARCHAR(254) NULL,
        CONSTRAINT UQ_Usuarios_Documento UNIQUE (Documento)
    );
END;
GO

IF OBJECT_ID(N'dbo.Libros', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Libros
    (
        LibroId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Libros PRIMARY KEY,
        Codigo NVARCHAR(20) NOT NULL,
        ISBN NVARCHAR(20) NULL,
        Titulo NVARCHAR(200) NOT NULL,
        AnioPublicacion SMALLINT NULL,
        EditorialId INT NOT NULL,
        CantidadTotal INT NOT NULL CONSTRAINT DF_Libros_CantidadTotal DEFAULT (1),
        CantidadDisponible INT NOT NULL CONSTRAINT DF_Libros_CantidadDisponible DEFAULT (1),
        CONSTRAINT UQ_Libros_Codigo UNIQUE (Codigo),
        CONSTRAINT FK_Libros_Editoriales FOREIGN KEY (EditorialId)
            REFERENCES dbo.Editoriales (EditorialId),
        CONSTRAINT CK_Libros_AnioPublicacion CHECK (AnioPublicacion IS NULL OR AnioPublicacion BETWEEN 1000 AND 2200),
        CONSTRAINT CK_Libros_Cantidades CHECK
            (CantidadTotal >= 0 AND CantidadDisponible >= 0 AND CantidadDisponible <= CantidadTotal)
    );
END;
GO

IF OBJECT_ID(N'dbo.LibroAutores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LibroAutores
    (
        LibroId INT NOT NULL,
        AutorId INT NOT NULL,
        CONSTRAINT PK_LibroAutores PRIMARY KEY (LibroId, AutorId),
        CONSTRAINT FK_LibroAutores_Libros FOREIGN KEY (LibroId)
            REFERENCES dbo.Libros (LibroId) ON DELETE CASCADE,
        CONSTRAINT FK_LibroAutores_Autores FOREIGN KEY (AutorId)
            REFERENCES dbo.Autores (AutorId) ON DELETE CASCADE
    );
END;
GO

IF OBJECT_ID(N'dbo.Prestamos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Prestamos
    (
        PrestamoId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Prestamos PRIMARY KEY,
        UsuarioId INT NOT NULL,
        LibroId INT NOT NULL,
        FechaPrestamo DATE NOT NULL CONSTRAINT DF_Prestamos_FechaPrestamo DEFAULT (CONVERT(DATE, GETDATE())),
        FechaDevolucionPrevista DATE NOT NULL,
        FechaDevolucionReal DATE NULL,
        Estado AS (CONVERT(NVARCHAR(10), CASE
            WHEN FechaDevolucionReal IS NULL THEN N'Prestado'
            ELSE N'Devuelto'
        END)) PERSISTED,
        CONSTRAINT FK_Prestamos_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES dbo.Usuarios (UsuarioId),
        CONSTRAINT FK_Prestamos_Libros FOREIGN KEY (LibroId)
            REFERENCES dbo.Libros (LibroId),
        CONSTRAINT CK_Prestamos_FechaPrevista CHECK (FechaDevolucionPrevista >= FechaPrestamo),
        CONSTRAINT CK_Prestamos_FechaReal CHECK
            (FechaDevolucionReal IS NULL OR FechaDevolucionReal >= FechaPrestamo)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Libros_ISBN'
      AND object_id = OBJECT_ID(N'dbo.Libros')
)
BEGIN
    CREATE UNIQUE INDEX UX_Libros_ISBN ON dbo.Libros (ISBN) WHERE ISBN IS NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Usuarios_Correo'
      AND object_id = OBJECT_ID(N'dbo.Usuarios')
)
BEGIN
    CREATE UNIQUE INDEX UX_Usuarios_Correo ON dbo.Usuarios (Correo) WHERE Correo IS NOT NULL;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_LibroAutores_AutorId'
      AND object_id = OBJECT_ID(N'dbo.LibroAutores')
)
BEGIN
    CREATE INDEX IX_LibroAutores_AutorId ON dbo.LibroAutores (AutorId);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Prestamos_UsuarioId'
      AND object_id = OBJECT_ID(N'dbo.Prestamos')
)
BEGIN
    CREATE INDEX IX_Prestamos_UsuarioId ON dbo.Prestamos (UsuarioId);
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Prestamos_LibroId_Estado'
      AND object_id = OBJECT_ID(N'dbo.Prestamos')
)
BEGIN
    CREATE INDEX IX_Prestamos_LibroId_Estado ON dbo.Prestamos (LibroId, FechaDevolucionReal);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Autores WHERE Nombre = N'Ana' AND Apellido = N'Ejemplo')
BEGIN
    INSERT INTO dbo.Autores (Nombre, Apellido)
    VALUES (N'Ana', N'Ejemplo');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Autores WHERE Nombre = N'Luis' AND Apellido = N'Muestra')
BEGIN
    INSERT INTO dbo.Autores (Nombre, Apellido)
    VALUES (N'Luis', N'Muestra');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Editoriales WHERE Nombre = N'Editorial de Prueba')
BEGIN
    INSERT INTO dbo.Editoriales (Nombre, Pais)
    VALUES (N'Editorial de Prueba', N'Colombia');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Documento = N'PRUEBA-001')
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Apellido, Documento, Telefono, Correo)
    VALUES (N'Usuario', N'De Prueba', N'PRUEBA-001', NULL, N'usuario.prueba@ejemplo.com');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE Codigo = N'LIB-PRUEBA-001')
BEGIN
    INSERT INTO dbo.Libros
        (Codigo, ISBN, Titulo, AnioPublicacion, EditorialId, CantidadTotal, CantidadDisponible)
    VALUES
        (N'LIB-PRUEBA-001', NULL, N'Libro de ejemplo', NULL,
         (SELECT EditorialId FROM dbo.Editoriales WHERE Nombre = N'Editorial de Prueba'), 3, 3);
END;

DECLARE @libroId INT =
    (SELECT LibroId FROM dbo.Libros WHERE Codigo = N'LIB-PRUEBA-001');
DECLARE @primerAutorId INT =
    (SELECT AutorId FROM dbo.Autores WHERE Nombre = N'Ana' AND Apellido = N'Ejemplo');
DECLARE @segundoAutorId INT =
    (SELECT AutorId FROM dbo.Autores WHERE Nombre = N'Luis' AND Apellido = N'Muestra');
DECLARE @usuarioId INT =
    (SELECT UsuarioId FROM dbo.Usuarios WHERE Documento = N'PRUEBA-001');

IF NOT EXISTS (SELECT 1 FROM dbo.LibroAutores WHERE LibroId = @libroId AND AutorId = @primerAutorId)
BEGIN
    INSERT INTO dbo.LibroAutores (LibroId, AutorId) VALUES (@libroId, @primerAutorId);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.LibroAutores WHERE LibroId = @libroId AND AutorId = @segundoAutorId)
BEGIN
    INSERT INTO dbo.LibroAutores (LibroId, AutorId) VALUES (@libroId, @segundoAutorId);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Prestamos WHERE UsuarioId = @usuarioId AND LibroId = @libroId)
   AND EXISTS (SELECT 1 FROM dbo.Libros WHERE LibroId = @libroId AND CantidadDisponible > 0)
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Libros
        SET CantidadDisponible = CantidadDisponible - 1
        WHERE LibroId = @libroId AND CantidadDisponible > 0;

        IF @@ROWCOUNT = 1
        BEGIN
            INSERT INTO dbo.Prestamos (UsuarioId, LibroId, FechaDevolucionPrevista)
            VALUES (@usuarioId, @libroId, DATEADD(DAY, 14, CONVERT(DATE, GETDATE())));
        END;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        THROW;
    END CATCH;
END;
GO
