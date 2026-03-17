using Microsoft.Extensions.Logging;
using SistemaVentas.Data.Entities.Db;
using System.Text.Json;

namespace SistemaVentas.WkService.Proccess;

public class ApiExtractor : IExtractor<Order>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiExtractor> _logger;
    private readonly string _baseUrl;

    public ApiExtractor(HttpClient httpClient, string baseUrl, ILogger<ApiExtractor> logger)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _logger = logger;
    }

    public async Task<IEnumerable<Order>> ExtractAsync()
    {
        var records = new List<Order>();

        try
        {
            _logger.LogInformation("Iniciando extracción desde API REST: {Url}", _baseUrl);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var response = await _httpClient.GetAsync($"{_baseUrl}/carts");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var carts = JsonSerializer.Deserialize<List<JsonElement>>(json);

            if (carts == null)
            {
                _logger.LogWarning("La API no retornó datos");
                return records;
            }

            foreach (var cart in carts)
            {
                // Validación básica antes de agregar
                var cartId = cart.GetProperty("id").GetInt32();
                var userId = cart.GetProperty("userId").GetInt32();
                var dateStr = cart.GetProperty("date").GetString();

                if (cartId <= 0 || userId <= 0)
                {
                    _logger.LogWarning("Registro de API inválido omitido: CartId {CartId}", cartId);
                    continue;
                }

                records.Add(new Order
                {
                    OrderId = cartId,
                    CustomerId = userId,
                    OrderDate = DateTime.TryParse(dateStr, out var date) ? date : DateTime.Now,
                    Status = "API"
                });
            }

            sw.Stop();
            _logger.LogInformation("API completada: {Count} registros extraídos en {Ms}ms",
                records.Count, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer datos de la API REST");
        }

        return records;
    }
}
