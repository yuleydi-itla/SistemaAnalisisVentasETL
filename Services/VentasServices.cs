
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;
using SistemaAnalisisVentas.ETL.Entities.dbo;
using SistemaAnalisisVentas.ETL.Entities.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace SistemaAnalisisVentas.ETL.Services
{
    public class VentasServices
    {
        private readonly SistemaAnalisisVentasDBAContext _context;

        public VentasServices(SistemaAnalisisVentasDBAContext context)
        {
            _context = context;
        }

        private CsvReader ConfigurarCsv(StreamReader reader)
        {
            return new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header?.Trim().ToLower(),
                MissingFieldFound = null,
                HeaderValidated = null
            });
        }

        // ================= CLIENTES =================
        public void ProcesarClientes(string rutaCsv)
        {
            using var reader = new StreamReader(rutaCsv);
            using var csv = ConfigurarCsv(reader);

            var clientes = csv.GetRecords<dynamic>(); //  usamos dynamic para poder leer country y city

            var idsExistentes = _context.Customers.Select(c => c.CustomerId).ToHashSet();

            int contador = 0;

            foreach (var row in clientes)
            {
                int customerId = int.Parse(row.customerid);
                string email = row.email;
                string firstName = row.firstname;
                string lastName = row.lastname;
                string phone = row.phone;
                string countryName = row.country;
                string cityName = row.city;

                if (string.IsNullOrWhiteSpace(email)) continue;
                if (idsExistentes.Contains(customerId)) continue;
                if (string.IsNullOrWhiteSpace(countryName)) continue;
                if (string.IsNullOrWhiteSpace(cityName)) continue;

                // ================= COUNTRY =================
                var country = _context.Countries
                    .FirstOrDefault(c => c.CountryName == countryName);

                if (country == null)
                {
                    country = new Country { CountryName = countryName };
                    _context.Countries.Add(country);
                    _context.SaveChanges();
                }

                // ================= CITY =================
                var city = _context.Cities
                    .FirstOrDefault(c => c.CityName == cityName && c.CountryId == country.CountryId);

                if (city == null)
                {
                    city = new City
                    {
                        CityName = cityName,
                        CountryId = country.CountryId
                    };
                    _context.Cities.Add(city);
                    _context.SaveChanges();
                }

                // ================= CUSTOMER =================
                var cliente = new Customer
                {
                    CustomerId = customerId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    CityId = city.CityId
                };

                _context.Customers.Add(cliente);
                contador++;

                if (contador % 1000 == 0)
                {
                    _context.SaveChanges();
                    _context.ChangeTracker.Clear();
                }
            }

            _context.SaveChanges();
            Console.WriteLine($"Clientes cargados: {contador}");
        }

        // ================= PRODUCTOS =================
        public void ProcesarProductos(string rutaCsv)
        {
            using var reader = new StreamReader(rutaCsv);
            using var csv = ConfigurarCsv(reader);

            var productos = csv.GetRecords<dynamic>(); // usamos dynamic para leer category
            var idsExistentes = _context.Products.Select(p => p.ProductId).ToHashSet();

            int contador = 0;

            foreach (var row in productos)
            {
                int productId = int.Parse(row.productid);
                string productName = row.productname;
                decimal price = decimal.Parse(row.price);
                int stock = int.Parse(row.stock);
                string categoryName = row.category;

                if (price <= 0) continue;
                if (string.IsNullOrWhiteSpace(productName)) continue;
                if (idsExistentes.Contains(productId)) continue;

                
                if (string.IsNullOrWhiteSpace(categoryName))
                    categoryName = "Sin categoría";

                // ================= CATEGORY =================
                var category = _context.Categories
                    .FirstOrDefault(c => c.CategoryName == categoryName);

                if (category == null)
                {
                    category = new Category
                    {
                        CategoryName = categoryName
                    };

                    _context.Categories.Add(category);
                    _context.SaveChanges();
                }

                // ================= PRODUCT =================
                var producto = new Product
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Stock = stock,
                    CategoryId = category.CategoryId
                };

                _context.Products.Add(producto);
                contador++;

                if (contador % 1000 == 0)
                {
                    _context.SaveChanges();
                    _context.ChangeTracker.Clear();
                }
            }

            _context.SaveChanges();
            Console.WriteLine($"Productos cargados: {contador}");
        }

        // ================= ORDENES =================
        public void ProcesarOrdenes(string rutaCsv)
        {
            using var reader = new StreamReader(rutaCsv);
            using var csv = ConfigurarCsv(reader);

            var ordenes = csv.GetRecords<Order>();
            var idsExistentes = _context.Orders.Select(o => o.OrderId).ToHashSet();

            int contador = 0;

            foreach (var orden in ordenes)
            {
                if (orden.OrderDate == default) continue;
                if (idsExistentes.Contains(orden.OrderId)) continue;

                // evitar conflicto de Customer
                var customerExiste = _context.Customers
                    .AsNoTracking()
                    .FirstOrDefault(c => c.CustomerId == orden.CustomerId);

                if (customerExiste == null) continue;

                // SOLO asignamos el ID, NO el objeto completo
                var nuevaOrden = new Order
                {
                    OrderId = orden.OrderId,
                    OrderDate = orden.OrderDate,
                    CustomerId = orden.CustomerId
                };

                _context.Orders.Add(nuevaOrden);
                contador++;

                if (contador % 1000 == 0)
                {
                    _context.SaveChanges();
                    _context.ChangeTracker.Clear();
                }
            }

            _context.SaveChanges();
            Console.WriteLine($"Órdenes cargadas: {contador}");
        }
        // ================= ORDER DETAILS =================
        public void ProcesarOrderDetails(string rutaCsv)
        {
            using var reader = new StreamReader(rutaCsv);
            using var csv = ConfigurarCsv(reader);
           
            _context.ChangeTracker.Clear();

            var detalles = csv.GetRecords<OrderDetail>();
            var idsExistentes = _context.OrderDetails
                .AsNoTracking()
                .Select(d => d.OrderDetailId)
                .ToHashSet();

            int contador = 0;

            foreach (var detalle in detalles)
            {
                if (detalle.Quantity <= 0) continue;
                if (detalle.TotalPrice <= 0) continue;
                if (idsExistentes.Contains(detalle.OrderDetailId)) continue;

                detalle.Order = null;
                detalle.Product = null;

                _context.OrderDetails.Add(detalle);
                contador++;

                if (contador % 1000 == 0)
                {
                    _context.SaveChanges();
                    _context.ChangeTracker.Clear();
                }
            }

            _context.SaveChanges();
            Console.WriteLine($"Detalles cargados: {contador}");
        }
    }
}