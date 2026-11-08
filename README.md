# Sistema de Gestión de Biblioteca

Aplicación de escritorio para administrar la información y los procesos de una biblioteca educativa. El proyecto se desarrolla como trabajo final de Programación Avanzada.

## Tecnologías

- C# y Windows Forms sobre .NET 10
- SQL Server
- Git y GitHub

## Estructura

La aplicación inicia como un único proyecto Windows Forms organizado por carpetas:

```text
SistemaGestionBiblioteca/
├── Presentacion/
├── Negocio/
├── Datos/
└── Entidades/
```

- `Presentacion`: formularios y navegación de la aplicación.
- `Negocio`: reglas y validaciones del sistema.
- `Datos`: conexión y operaciones con SQL Server.
- `Entidades`: clases que representan los datos del sistema.

## Módulos previstos

- Libros
- Autores
- Editoriales
- Usuarios
- Préstamos y devoluciones
- Consultas y reportes

## Arquitectura

La interfaz, la lógica de negocio y el acceso a datos se mantendrán separados en sus respectivas carpetas. La base de datos relacional almacenará las entidades y sus relaciones, con claves y restricciones de integridad.

## Convención de nombres

Los nombres de variables y controles serán descriptivos, en español y con formato `camelCase`.

## Requisitos para ejecutar

- Windows
- .NET 10 SDK
- SQL Server (se configurará en una etapa posterior)

Para compilar la solución:

```powershell
dotnet build SistemaGestionBiblioteca.slnx
```

Para ejecutar la aplicación:

```powershell
dotnet run --project SistemaGestionBiblioteca/SistemaGestionBiblioteca.csproj
```

## Estado

En desarrollo. La estructura de la solución, el modelo de datos y las instrucciones de configuración se completarán durante la implementación.
