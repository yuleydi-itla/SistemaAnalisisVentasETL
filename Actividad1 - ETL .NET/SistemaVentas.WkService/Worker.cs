using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaVentas.WkService.Proccess;
using SistemaVentas.Data.Entities.Csv;
using SistemaVentas.Data.Entities.Db;

namespace SistemaVentas.WkService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("=== Worker ETL iniciado: {Time} ===", DateTimeOffset.Now);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        var csvFilePath = _configuration["Extraction:CsvFilePath"]!;
        var apiBaseUrl = _configuration["Extraction:ApiBaseUrl"]!;
        var operationalDb = _configuration.GetConnectionString("OperationalDb")!;

        var csvExtractor = new CsvExtractor(csvFilePath,
            LoggerFactory.Create(b => b.AddConsole()).CreateLogger<CsvExtractor>());

        var dbExtractor = new DbExtractor(operationalDb,
            LoggerFactory.Create(b => b.AddConsole()).CreateLogger<DbExtractor>());

        var apiExtractor = new ApiExtractor(new HttpClient(), apiBaseUrl,
            LoggerFactory.Create(b => b.AddConsole()).CreateLogger<ApiExtractor>());

        _logger.LogInformation("Ejecutando extracción en paralelo de las 3 fuentes...");

        var csvTask = csvExtractor.ExtractAsync();
        var dbTask = dbExtractor.ExtractAsync();
        var apiTask = apiExtractor.ExtractAsync();

        await Task.WhenAll(csvTask, dbTask, apiTask);

        var csvRecords = await csvTask;
        var dbRecords = await dbTask;
        var apiRecords = await apiTask;

        _logger.LogInformation("CSV: {CsvCount} registros", csvRecords?.Count() ?? 0);
        _logger.LogInformation("BD: {DbCount} registros", dbRecords?.Count() ?? 0);
        _logger.LogInformation("API: {ApiCount} registros", apiRecords?.Count() ?? 0);

        sw.Stop();
        _logger.LogInformation("=== Extracción completada en {Ms}ms ===", sw.ElapsedMilliseconds);
    }
}
