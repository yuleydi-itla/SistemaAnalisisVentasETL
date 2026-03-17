using CsvHelper.Configuration.Attributes;

namespace SistemaVentas.Data.Entities.Csv;

public class OrderCsvRecord
{
    [Name("OrderID")]
    public int OrderId { get; set; }

    [Name("CustomerID")]
    public int CustomerId { get; set; }

    [Name("OrderDate")]
    public string OrderDate { get; set; } = string.Empty;

    [Name("Status")]
    public string Status { get; set; } = string.Empty;
}
