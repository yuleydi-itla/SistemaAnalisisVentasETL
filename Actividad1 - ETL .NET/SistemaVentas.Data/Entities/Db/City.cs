namespace SistemaVentas.Data.Entities.Db;

public class City
{
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public int CountryId { get; set; }
}