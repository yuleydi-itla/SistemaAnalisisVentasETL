
namespace SistemaAnalisisVentas.ETL.Entities.dbo
{
    public sealed class Order
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? Status { get; set; }

        public Customer? Customer { get; set; }

    }
}