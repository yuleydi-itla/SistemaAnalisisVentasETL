namespace SistemaVentas.Data.Entities.Dwh;

public class FactSales
{
    public int SalesId { get; set; }
    public int OrderId { get; set; }
    public int DateKey { get; set; }
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceAtSale { get; set; }
    public decimal TotalAmount { get; set; }
}