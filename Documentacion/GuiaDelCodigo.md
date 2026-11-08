# Guía sencilla del código

Esta guía explica dónde está cada parte del sistema y cómo se relacionan. La idea es poder seguir una operación desde la pantalla hasta la base de datos sin tener que conocer cada instrucción de memoria.

## Cómo funciona una operación

1. La persona usa un formulario de `Presentacion`.
2. El formulario recoge los datos y llama a un servicio de `Negocio`.
3. El servicio revisa que los datos cumplan las reglas del sistema.
4. El repositorio de `Datos` prepara una consulta SQL con parámetros y se conecta a SQL Server.
5. SQL Server guarda o devuelve la información. El resultado vuelve a la pantalla.

Los préstamos y devoluciones también actualizan las existencias. Esas dos acciones se guardan juntas en una transacción para evitar que el inventario quede incompleto si una parte falla.

## Carpetas y archivos

### Entidades: los datos que maneja el sistema

- `SistemaGestionBiblioteca/Entidades/Autor.cs`: identificador, nombre y apellido de un autor.
- `SistemaGestionBiblioteca/Entidades/Editorial.cs`: identificador, nombre y país opcional de una editorial.
- `SistemaGestionBiblioteca/Entidades/Usuario.cs`: identificación y datos de contacto de una persona usuaria.
- `SistemaGestionBiblioteca/Entidades/Libro.cs`: código, título, editorial, autores y cantidades del inventario. `AutoresTexto` reúne los nombres para mostrarlos en una sola columna.
- `SistemaGestionBiblioteca/Entidades/Prestamo.cs`: libro y usuario relacionados, fechas y estado del préstamo.

### Negocio: reglas y validaciones

- `SistemaGestionBiblioteca/Negocio/AutorServicio.cs`: verifica que nombre y apellido estén completos y respeta sus límites de caracteres.
- `SistemaGestionBiblioteca/Negocio/EditorialServicio.cs`: comprueba los datos de la editorial y deja el país vacío como valor nulo.
- `SistemaGestionBiblioteca/Negocio/UsuarioServicio.cs`: comprueba nombre, apellido, documento, longitudes y formato del correo.
- `SistemaGestionBiblioteca/Negocio/LibroServicio.cs`: revisa código, título, año, editorial, existencias y selección de autores; también filtra el catálogo por código, título, ISBN o autor.
- `SistemaGestionBiblioteca/Negocio/PrestamoServicio.cs`: revisa que haya usuario y libro, y que las fechas del préstamo y la devolución sean válidas.
- `SistemaGestionBiblioteca/Negocio/ReporteServicio.cs`: solicita cada conjunto de datos que se presenta en los reportes.

Cada servicio recibe su repositorio en el constructor. Así, la pantalla no tiene que ejecutar consultas SQL ni decidir las reglas de negocio.

### Datos: acceso a SQL Server

- `SistemaGestionBiblioteca/Datos/ConexionBaseDatos.cs`: lee la conexión `Biblioteca` de `App.config`, crea conexiones y permite probarlas.
- `SistemaGestionBiblioteca/Datos/AutorRepositorio.cs`: consulta, inserta, actualiza y elimina autores; no permite borrar autores relacionados con libros.
- `SistemaGestionBiblioteca/Datos/EditorialRepositorio.cs`: administra editoriales y no permite eliminar una editorial que tenga libros.
- `SistemaGestionBiblioteca/Datos/UsuarioRepositorio.cs`: administra usuarios y no permite borrar usuarios con préstamos.
- `SistemaGestionBiblioteca/Datos/LibroRepositorio.cs`: administra libros y su relación con autores. Al actualizar, conserva las unidades que ya están prestadas.
- `SistemaGestionBiblioteca/Datos/PrestamoRepositorio.cs`: registra, actualiza y devuelve préstamos, manteniendo sincronizado el inventario.
- `SistemaGestionBiblioteca/Datos/ReporteRepositorio.cs`: ejecuta las consultas de resumen, catálogo, personas, préstamos activos e historial de devoluciones.

Las consultas usan parámetros para enviar los valores a SQL Server por separado del texto SQL. Esto evita construir consultas con los datos escritos en los formularios.

### Presentación: pantallas y controles

- `SistemaGestionBiblioteca/Presentacion/Program.cs`: configura Windows Forms y abre el formulario principal.
- `SistemaGestionBiblioteca/Presentacion/FormularioPrincipal.cs`: construye el inicio, el menú y los accesos a cada módulo; también permite probar la conexión.
- `SistemaGestionBiblioteca/Presentacion/FormularioPrincipal.Designer.cs`: declara la otra parte de la clase parcial del formulario principal. La interfaz actual se crea en código en `FormularioPrincipal.cs`.
- `SistemaGestionBiblioteca/Presentacion/FormularioAutores.cs`: muestra la lista y permite registrar, editar y eliminar autores.
- `SistemaGestionBiblioteca/Presentacion/FormularioEditoriales.cs`: muestra la lista y permite registrar, editar y eliminar editoriales.
- `SistemaGestionBiblioteca/Presentacion/FormularioUsuarios.cs`: muestra la lista y permite registrar, editar y eliminar usuarios.
- `SistemaGestionBiblioteca/Presentacion/FormularioLibros.cs`: muestra y busca libros por código, título, ISBN o autor, y permite editar su editorial, autores y existencias.
- `SistemaGestionBiblioteca/Presentacion/FormularioPrestamos.cs`: permite seleccionar usuario y libro, registrar el préstamo, registrar su devolución y consultar el historial.
- `SistemaGestionBiblioteca/Presentacion/FormularioReportes.cs`: contiene pestañas para ver el resumen y las consultas de la biblioteca.
- `SistemaGestionBiblioteca/Presentacion/EstiloInterfazBiblioteca.cs`: comparte colores, tipografía y formato de botones, campos, pestañas y tablas.

Los formularios cargan la información al abrirse. Al seleccionar una fila, llenan los campos para editarla. Los botones se habilitan o deshabilitan según la selección y la operación disponible.

### Base de datos

- `BaseDatos/01_CrearBaseDatos.sql`: crea la base `SistemaGestionBiblioteca`, sus tablas y relaciones; agrega restricciones e índices, y carga datos de ejemplo cuando todavía no existen.
- `BaseDatos/02_DatosDemostracionCapturas.sql`: agrega 20 usuarios ficticios, 20 libros de autores colombianos y un préstamo devuelto de muestra para las capturas. Se puede ejecutar más de una vez sin duplicar esos datos; la editorial indicada es solo de demostración.
- `Documentacion/ModeloDeDatos.md`: describe las tablas y las relaciones de la base de datos.
- `Documentacion/Capturas/`: contiene las imágenes de las pantallas y de las operaciones realizadas para la evidencia del proyecto.
- `Documentacion/DocumentoTecnico.md`: reúne los apartados académicos de la entrega.
- `Documentacion/InformeDePruebas.md`: registra las verificaciones ejecutadas y las limitaciones del entorno.

El script se puede volver a ejecutar sin volver a crear objetos o registros de ejemplo que ya existen.

## Reglas importantes

- El inventario disponible disminuye cuando se registra un préstamo y aumenta al devolverlo.
- El estado de un préstamo se calcula con la fecha de devolución real: sin fecha, sigue prestado; con fecha, está devuelto.
- Un libro conserva una existencia por cada préstamo activo, aunque se reduzca su cantidad total.
- No se eliminan autores, editoriales o usuarios que todavía estén relacionados con libros o préstamos.
- Los campos opcionales vacíos se guardan como `NULL`.
- El código fuente usa nombres descriptivos en español y las variables/controladores siguen `camelCase`.

## Compilar y ejecutar

Desde la carpeta del repositorio:

```powershell
dotnet build SistemaGestionBiblioteca.slnx
dotnet run --project SistemaGestionBiblioteca/SistemaGestionBiblioteca.csproj
```

La conexión a SQL Server se configura en `SistemaGestionBiblioteca/App.config`; no se escriben contraseñas en el código.
