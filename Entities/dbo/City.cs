namespace SistemaAnalisisVentas.ETL.Entities.dbo
{
    public sealed class City
    {
        public int CityId { get; set; }

        public string? CityName { get; set; }

        public int CountryId { get; set; }

        public Country? Country { get; set; }

    }
}