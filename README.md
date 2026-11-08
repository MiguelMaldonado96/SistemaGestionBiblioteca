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

## Base de datos

El script inicial y el modelo entidad-relación con diccionario de datos están en:

- `BaseDatos/01_CrearBaseDatos.sql`
- `Documentacion/ModeloDeDatos.md`

Ejecuta el script desde SQL Server Management Studio para crear la base de datos e insertar registros de prueba.

## Arquitectura

La interfaz, la lógica de negocio y el acceso a datos se mantendrán separados en sus respectivas carpetas. La base de datos relacional almacenará las entidades y sus relaciones, con claves y restricciones de integridad.

## Convención de nombres

Los nombres de variables y controles serán descriptivos, en español y con formato `camelCase`.

## Requisitos para ejecutar

- Windows
- .NET 10 SDK
- SQL Server 2022 Database Engine instalado como instancia `SQL2022`
- SQL Server Management Studio (SSMS)

SSMS es el cliente gráfico; para conectarse se necesita también el motor de SQL Server. La aplicación usa la cadena `Biblioteca` de `SistemaGestionBiblioteca/App.config`, configurada para la instancia local `.\SQL2022`, autenticación de Windows, cifrado obligatorio y certificado de servidor de confianza. No almacena credenciales.

En SSMS configura la conexión así:

1. Nombre del servidor: `.\SQL2022`.
2. Autenticación: **Autenticación de Windows**.
3. En las opciones de conexión, establece **Cifrar** en **Obligatorio** y marca **Certificado de servidor de confianza**.
4. Selecciona **Conectar**.

La aplicación incluye un botón **Probar conexión** para verificar esos mismos parámetros. Ejecuta `BaseDatos/01_CrearBaseDatos.sql` en SSMS para crear la base de datos antes de implementar las operaciones.

Para compilar la solución:

```powershell
dotnet build SistemaGestionBiblioteca.slnx
```

Para ejecutar la aplicación:

```powershell
dotnet run --project SistemaGestionBiblioteca/SistemaGestionBiblioteca.csproj
```

## Estado

En desarrollo. La estructura inicial de la solución y el modelo relacional están definidos. La interfaz, las operaciones de negocio, la conexión a SQL Server y la documentación final se completarán en los siguientes pasos.
