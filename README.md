# Sistema de Análisis de Ventas ETL

## Descripción
Este proyecto lo desarrollé como parte de mi práctica de ETL. Mi objetivo fue construir un flujo que permitiera leer archivos CSV, procesarlos y cargarlos en una base de datos SQL Server. Todo lo que ves aquí lo organicé en carpetas y código para que quede claro cómo funciona el pipeline.

## Flujo de Trabajo
1. **Extracción**: leo los archivos CSV desde la carpeta `Data`.
2. **Transformación**: aplico limpieza y validación de los datos.
3. **Carga**: inserto los registros en las tablas de SQL Server usando Entity Framework Core.
4. **Verificación**: reviso que los datos estén completos y hago consultas para validar la carga.
   
## Tecnologías
- C#
- Entity Framework Core
- SQL Server
- CsvHelper

## Tablas
- Customers
- Products
- Orders
- Order_Details
- Categories
- Cities
- Countries
