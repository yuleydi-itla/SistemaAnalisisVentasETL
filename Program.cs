using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using SistemaAnalisisVentas.ETL.Entities.Db.Context;
using SistemaAnalisisVentas.ETL.Services;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("====================================");
            Console.WriteLine("   INICIANDO PIPELINE ETL");
            Console.WriteLine("====================================\n");

            var optionsBuilder = new DbContextOptionsBuilder<SistemaAnalisisVentasDBAContext>();

            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=SistemaAnalisisVentasDBA;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
            );

            using var context = new SistemaAnalisisVentasDBAContext(optionsBuilder.Options);

            // Asegura que la BD exista
            context.Database.EnsureCreated();

            var service = new VentasServices(context);

            // Ruta segura
            var basePath = Directory.GetCurrentDirectory();

            Console.WriteLine($"Ruta de archivos: {basePath}\n");

            // Rutas de archivos
            var customersPath = Path.Combine(basePath, "customers.csv");
            var productsPath = Path.Combine(basePath, "products.csv");
            var ordersPath = Path.Combine(basePath, "orders.csv");
            var orderDetailsPath = Path.Combine(basePath, "order_details.csv");

            // Validación de archivos
            ValidarArchivo(customersPath);
            ValidarArchivo(productsPath);
            ValidarArchivo(ordersPath);
            ValidarArchivo(orderDetailsPath);

            Console.WriteLine("Archivos validados correctamente.\n");

            // ORDEN CORRECTO DEL ETL
            Console.WriteLine("Procesando CLIENTES...");
            service.ProcesarClientes(customersPath);

            Console.WriteLine("\nProcesando PRODUCTOS...");
            service.ProcesarProductos(productsPath);

            Console.WriteLine("\nProcesando ORDENES...");
            service.ProcesarOrdenes(ordersPath);

            Console.WriteLine("\nProcesando DETALLES DE ORDEN...");
            service.ProcesarOrderDetails(orderDetailsPath);

            Console.WriteLine("\n====================================");
            Console.WriteLine("   ETL COMPLETADO CORRECTAMENTE");
            Console.WriteLine("====================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR EN EL PIPELINE:");
            Console.WriteLine(ex.Message);

            if (ex.InnerException != null)
            {
                Console.WriteLine("DETALLE REAL:");
                Console.WriteLine(ex.InnerException.Message);
            }
        }
    }

    // Método reutilizable
    static void ValidarArchivo(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"No se encontró el archivo: {path}");
        }
    }
}
