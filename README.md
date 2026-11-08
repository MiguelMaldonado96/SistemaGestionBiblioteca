# Sistema de Gestión de Biblioteca

Aplicación de escritorio para administrar una biblioteca educativa. Este proyecto es el trabajo final de Programación Avanzada y se desarrolla con C#, Windows Forms y SQL Server.

## Tecnologías

- C# y Windows Forms con .NET 10 (`net10.0-windows`).
- SQL Server y `Microsoft.Data.SqlClient`.
- Git y GitHub para el control de versiones.

## Estructura real del repositorio

La solución contiene un solo proyecto WinForms. Las carpetas `Presentacion`, `Negocio`, `Datos` y `Entidades` son capas dentro de ese proyecto; no son proyectos separados.

```text
.
├── SistemaGestionBiblioteca.slnx
├── SistemaGestionBiblioteca/
│   ├── SistemaGestionBiblioteca.csproj
│   ├── App.config
│   ├── Presentacion/
│   │   ├── Program.cs
│   │   ├── FormularioPrincipal.cs
│   │   ├── FormularioPrincipal.Designer.cs
│   │   ├── FormularioLibros.cs
│   │   ├── FormularioUsuarios.cs
│   │   ├── FormularioAutores.cs
│   │   ├── FormularioEditoriales.cs
│   │   ├── FormularioPrestamos.cs
│   │   ├── FormularioReportes.cs
│   │   └── EstiloInterfazBiblioteca.cs
│   ├── Negocio/
│   │   └── Servicios y validaciones
│   ├── Datos/
│   │   └── Conexión y repositorios SQL
│   └── Entidades/
│       └── Autor, Editorial, Libro, Prestamo y Usuario
├── BaseDatos/
│   ├── 01_CrearBaseDatos.sql
│   └── 02_DatosDemostracionCapturas.sql
├── Documentacion/
│   ├── Capturas/
│   ├── DocumentoTecnico.md
│   ├── InformeDePruebas.md
│   ├── GuiaDelCodigo.md
│   └── ModeloDeDatos.md
├── .gitignore
└── README.md
```

La presentación llama a los servicios de negocio, estos aplican validaciones y usan los repositorios para consultar o actualizar SQL Server. Las entidades representan los datos que pasan entre las capas.

## Funcionalidades implementadas

- **Inicio:** navegación entre Inicio, Libros, Usuarios, Autores, Editoriales, Préstamos y Reportes; incluye una prueba de conexión.
- **Libros:** registrar, listar, buscar por código, título, ISBN o autor, editar y eliminar libros; relacionarlos con autores y editoriales, y controlar existencias.
- **Usuarios:** registrar, listar, editar y eliminar usuarios, con validaciones de documento y correo y protección de registros relacionados con préstamos.
- **Autores:** registrar, listar, editar y eliminar autores, respetando las relaciones con libros.
- **Editoriales:** registrar, listar, editar y eliminar editoriales, respetando las relaciones con libros.
- **Préstamos y devoluciones:** registrar, consultar, actualizar y eliminar préstamos; registrar devoluciones y mantener sincronizadas las existencias mediante transacciones.
- **Reportes:** resumen general, libros, autores, editoriales, usuarios, préstamos activos e historial de devoluciones.

Las operaciones usan validaciones para los campos y las fechas, restricciones de unicidad para los datos definidos en la base y mensajes en la interfaz para informar errores y resultados.

## Base de datos y datos de demostración

La base de datos `SistemaGestionBiblioteca` contiene las tablas `Autores`, `Editoriales`, `Usuarios`, `Libros`, `LibroAutores` y `Prestamos`. Sus relaciones, restricciones, índices y datos iniciales están definidos en `BaseDatos/01_CrearBaseDatos.sql`.

Para poblar las pantallas con datos de demostración, `BaseDatos/02_DatosDemostracionCapturas.sql` agrega veinte usuarios ficticios, veinte libros de autores colombianos, un préstamo activo inicial y un préstamo devuelto de muestra. Los usuarios usan correos del dominio reservado `.invalid`. El script puede ejecutarse nuevamente sin duplicar esos registros ni alterar las existencias por el préstamo ya devuelto.

## Requisitos para ejecutar

- Windows.
- .NET 10 SDK.
- Motor de SQL Server y SQL Server Management Studio (SSMS).

SSMS es el cliente de administración; también se necesita el motor de SQL Server. La consigna indica conectar en SSMS a `.\SQL2022` con autenticación de Windows, **Cifrar: Obligatorio** y **Certificado de servidor de confianza**.

La conexión guardada en `SistemaGestionBiblioteca/App.config` apunta a `.\SQL2022`, que se verificó con autenticación de Windows. La instancia predeterminada `localhost` se conserva en paralelo y no se modificó. La cadena usa cifrado obligatorio y certificado de servidor de confianza; no contiene contraseñas.

## Preparar, compilar y ejecutar

1. Abre `BaseDatos/01_CrearBaseDatos.sql` en SSMS y ejecuta el script para crear la base y los datos iniciales.
2. Opcionalmente, ejecuta `BaseDatos/02_DatosDemostracionCapturas.sql` para cargar los datos de demostración.
3. La cadena de `SistemaGestionBiblioteca/App.config` ya apunta a `.\SQL2022`; ajústala solo si ejecutarás el proyecto contra otra instancia.
4. Desde la carpeta del repositorio, compila y ejecuta:

```powershell
dotnet build SistemaGestionBiblioteca.slnx
dotnet run --project SistemaGestionBiblioteca/SistemaGestionBiblioteca.csproj
```

## Documentación y capturas

- [Guía del código](Documentacion/GuiaDelCodigo.md): explica las capas y los archivos principales.
- [Modelo de datos](Documentacion/ModeloDeDatos.md): incluye el diagrama entidad-relación y el diccionario de datos.
- [Documento técnico (Markdown)](Documentacion/DocumentoTecnico.md): fuente editable del informe académico.
- [Documento técnico (PDF)](Documentacion/DocumentoTecnico.pdf): versión lista para revisar e imprimir, exportada con diagramas y número de página.
- [Informe de pruebas](Documentacion/InformeDePruebas.md): registra los casos ejecutados, resultados y condiciones pendientes del entorno.
- [Capturas de pantalla](Documentacion/Capturas/): evidencia de los módulos, reportes y registro de una devolución.

## Estado frente a la consigna

Están implementados los módulos principales, incluida la búsqueda de libros, la base de datos relacional, la arquitectura en capas dentro del proyecto, las validaciones, el manejo de errores en la interfaz y los reportes. El repositorio incluye el documento técnico en Markdown, el informe de pruebas, la guía del código, el modelo de datos y las capturas.

Configuración del entorno comprobada:

- La instancia nombrada `.\SQL2022` está instalada y la base de datos está creada con los datos de demostración. La instancia predeterminada `localhost` también continúa funcionando.

El repositorio contiene el código fuente, los scripts SQL, esta documentación, las capturas y el historial de commits.
