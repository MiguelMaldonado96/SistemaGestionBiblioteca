# Informe de pruebas

**Proyecto:** Sistema de Gestión de Biblioteca

**Fecha de ejecución:** 8 de noviembre de 2026

**Entorno:** Windows, .NET SDK 10.0.403, SQL Server 2022 (`localhost`, versión 16.0.1000.6)

**Base de datos:** `SistemaGestionBiblioteca`

## Objetivo

Registrar verificaciones reproducibles de la compilación, la búsqueda de libros, la conexión local y la coherencia entre el catálogo, los autores, las existencias y los préstamos. Las capturas guardadas en `Documentacion/Capturas/` complementan la evidencia visual de módulos y operaciones.

## Resultados

| ID | Prueba | Resultado esperado | Resultado observado | Estado |
|---|---|---|---|---|
| P01 | Compilar `SistemaGestionBiblioteca.slnx` sin reemplazar el ejecutable abierto. | Build correcto sin errores ni advertencias. | Build en una ruta temporal: 0 errores y 0 advertencias. | Aprobada |
| P02 | Buscar con criterio vacío. | Mostrar el catálogo completo. | El filtro devuelve todos los elementos. | Aprobada |
| P03 | Buscar con un criterio de solo espacios. | Mostrar el catálogo completo. | El filtro devuelve todos los elementos. | Aprobada |
| P04 | Buscar un código, ignorando mayúsculas/minúsculas. | Encontrar el libro por código. | `col-001` encuentra `COL-001`. | Aprobada |
| P05 | Buscar una parte del título. | Encontrar el libro cuyo título contiene el texto. | `soledad` encuentra *Cien años de soledad*. | Aprobada |
| P06 | Buscar por nombre o apellido de autor. | Encontrar libros relacionados con el autor. | `caicedo` encuentra *¡Que viva la música!* de Andrés Caicedo. | Aprobada |
| P07 | Buscar por ISBN. | Encontrar el libro con el ISBN correspondiente. | El ISBN completo encuentra el libro esperado en el caso aislado de prueba. | Aprobada |
| P08 | Buscar un texto sin coincidencias. | Mostrar cero resultados y un mensaje informativo. | El filtro devuelve cero elementos para `no-existe`. | Aprobada |
| P09 | Buscar con espacios antes y después del criterio. | Ignorar espacios externos. | `  soledad  ` encuentra el libro esperado. | Aprobada |
| P10 | Conectar a la base local con autenticación integrada. | Consultar la base de desarrollo. | La conexión a `localhost` respondió y permitió leer los conteos de la base. | Aprobada |
| P11 | Consultar integridad del inventario y relaciones. | Libros con autor y cantidades disponibles coherentes. | 0 libros sin autor y 0 libros con diferencia entre disponibles y total menos préstamos activos. | Aprobada |
| P12 | Verificar volumen de datos de demostración. | 20 usuarios y 20 libros de demostración, sin duplicados. | 20 usuarios con documento `DEMO-*` y 20 libros con código `COL-*`; la base también conserva el usuario y el libro iniciales. | Aprobada |
| P13 | Conectar al nombre de instancia que pide la consigna. | Acceder a `.\SQL2022`. | La conexión falló con “Error Locating Server/Instance Specified”; solo se verificó la instancia local predeterminada `localhost`. | Pendiente de entorno |
| P14 | Revisar paquetes con vulnerabilidades conocidas. | Sin paquetes reportados como vulnerables en el proyecto. | `dotnet list ... package --vulnerable --include-transitive` no reportó paquetes vulnerables. | Aprobada |

## Casos de búsqueda ejecutados

Los casos P02–P09 se ejecutaron con un pequeño harness temporal que invocó el mismo método `LibroServicio.Buscar` utilizado por el formulario. El harness creó dos libros de muestra y verificó cada resultado con aserciones; los ocho casos terminaron en `PASS`. El harness se guardó fuera del repositorio y no se añadió al proyecto de producción.

El formulario actualiza el filtro con cada cambio de texto, permite limpiar el criterio y muestra el número de coincidencias. El build confirmó que la interfaz que contiene ese control compila correctamente. No se automatizó la interacción visual de teclado y ratón en esta ejecución.

## Verificación de base de datos

Las consultas de solo lectura ejecutadas en la base de desarrollo arrojaron:

| Comprobación | Resultado |
|---|---:|
| Usuarios totales | 21 |
| Libros totales | 21 |
| Préstamos activos | 1 |
| Préstamos devueltos | 1 |
| Libros sin autor relacionado | 0 |
| Libros con inventario inconsistente | 0 |
| Usuarios de demostración | 20 |
| Libros de demostración | 20 |

La devolución se confirma también en las capturas `20-Devolucion-registrada.png` y `21-Reporte-historial-devolucion.png`.

## Incidencias y limitaciones

- `.\SQL2022` no está disponible en este equipo. La aplicación conserva `localhost` como conexión de desarrollo. La prueba de `.\SQL2022` debe repetirse después de instalar o confirmar esa instancia en el entorno de entrega.
- La restauración del harness temporal mostró avisos NuGet `NU1902` y `NU1903` relativos a `System.Security.Cryptography.Pkcs 9.0.18`. La auditoría de paquetes del proyecto real no reportó paquetes vulnerables; los avisos corresponden al restore del harness temporal y no se incorporó ese proyecto al repositorio.
- Las pruebas de búsqueda validan la lógica usada por la interfaz, pero no sustituyen una prueba de aceptación manual del formulario en el equipo de entrega.
- La matriz documenta las verificaciones ejecutadas para esta iteración; antes de una entrega definitiva se recomienda repetir los flujos CRUD, préstamos y devoluciones desde la interfaz conectada al servidor final.

## Conclusión

La compilación, los casos de búsqueda y las verificaciones de consistencia de la base local pasaron. La principal condición pendiente de entorno es confirmar una instancia `.\SQL2022` accesible y validar allí la aplicación antes de la sustentación.
