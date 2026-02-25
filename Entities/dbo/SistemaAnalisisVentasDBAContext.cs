using Microsoft.EntityFrameworkCore;
using SistemaAnalisisVentas.ETL.Entities.dbo;


namespace SistemaAnalisisVentas.ETL.Entities.Db.Context
{
    public class SistemaAnalisisVentasDBAContext : DbContext
    {
        public SistemaAnalisisVentasDBAContext(DbContextOptions<SistemaAnalisisVentasDBAContext> options)
        : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>().ToTable("Order_Details");
        }
    }
}
        
