namespace SistemaVentas.Data.Entities.Dwh;

public class DimProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
}