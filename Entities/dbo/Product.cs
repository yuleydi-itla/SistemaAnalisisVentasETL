namespace SistemaAnalisisVentas.ETL.Entities.dbo
{  

public sealed class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    }
  }