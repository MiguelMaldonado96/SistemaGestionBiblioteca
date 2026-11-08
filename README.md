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

## Módulos

- Autores: consultar, registrar, actualizar y eliminar, con validación de campos y protección de relaciones.
- Libros
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

SSMS es el cliente gráfico; para conectarse se necesita también el motor de SQL Server. La cadena `Biblioteca` de `SistemaGestionBiblioteca/App.config` usa autenticación de Windows, cifrado obligatorio y certificado de servidor de confianza. Para desarrollo en este equipo apunta temporalmente a `localhost`, que corresponde a la instancia instalada `MSSQLSERVER`. Cuando esté instalada la instancia nombrada solicitada por la docente, cambia el servidor a `.\SQL2022`. No se almacenan credenciales.

En SSMS configura la conexión así:

1. Nombre del servidor: `.\SQL2022`.
2. Autenticación: **Autenticación de Windows**.
3. En las opciones de conexión, establece **Cifrar** en **Obligatorio** y marca **Certificado de servidor de confianza**.
4. Selecciona **Conectar**.

La aplicación incluye un botón **Probar conexión** para verificar los parámetros configurados en `App.config`. Ejecuta `BaseDatos/01_CrearBaseDatos.sql` en SSMS para crear la base de datos.

Para compilar la solución:

```powershell
dotnet build SistemaGestionBiblioteca.slnx
```

Para ejecutar la aplicación:

```powershell
dotnet run --project SistemaGestionBiblioteca/SistemaGestionBiblioteca.csproj
```

## Estado

En desarrollo. La conexión local y la base de datos ya están verificadas; el CRUD de autores es el primer módulo implementado. Los módulos restantes y la documentación final se completarán por etapas.
