using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using SistemaVentas.Data.Entities.Csv;
using System.Globalization;

namespace SistemaVentas.WkService.Proccess;

public class CsvExtractor : IExtractor<OrderCsvRecord>
{
    private readonly string _filePath;
    private readonly ILogger<CsvExtractor> _logger;

    public CsvExtractor(string filePath, ILogger<CsvExtractor> logger)
    {
        _filePath = filePath;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderCsvRecord>> ExtractAsync()
    {
        var records = new List<OrderCsvRecord>();

        try
        {
            _logger.LogInformation("Iniciando extracción CSV desde {FilePath}", _filePath);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null
            });

            await foreach (var record in csv.GetRecordsAsync<OrderCsvRecord>())
            {
                // Validación básica antes de agregar
                if (record.CustomerId <= 0)
                {
                    _logger.LogWarning("Registro inválido omitido: OrderId {OrderId}", record.OrderId);
                    continue;
                }

                records.Add(record);
            }

            sw.Stop();
            _logger.LogInformation("CSV completado: {Count} registros extraídos en {Ms}ms",
                records.Count, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer datos del CSV");
        }

        return records;
    }
}