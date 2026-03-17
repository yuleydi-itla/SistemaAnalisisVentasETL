using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SistemaVentas.Data.Entities.Db;

namespace SistemaVentas.WkService.Proccess;

public class DbExtractor : IExtractor<OrderDetail>
{
    private readonly string _connectionString;
    private readonly ILogger<DbExtractor> _logger;

    public DbExtractor(string connectionString, ILogger<DbExtractor> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDetail>> ExtractAsync()
    {
        var records = new List<OrderDetail>();

        try
        {
            _logger.LogInformation("Iniciando extracción desde Base de Datos relacional");

            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Esta query trae el detalle de órdenes de tu BD de la Tarea 1
            const string query = @"
                SELECT 
                    od.OrderDetailID,
                    od.OrderID,
                    od.ProductID,
                    od.Quantity,
                    od.TotalPrice
                FROM Order_Details od
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                WHERE od.Quantity > 0
                  AND od.TotalPrice > 0";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                records.Add(new OrderDetail
                {
                    OrderDetailId = reader.GetInt32(0),
                    OrderId = reader.GetInt32(1),
                    ProductId = reader.GetInt32(2),
                    Quantity = reader.GetInt32(3),
                    TotalPrice = reader.GetDecimal(4)
                });
            }

            sw.Stop();
            _logger.LogInformation("BD completada: {Count} registros extraídos en {Ms}ms",
                records.Count, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer datos de la Base de Datos");
        }

        return records;
    }
}