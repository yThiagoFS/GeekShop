using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Model.Context
{
    public class SQLContext : DbContext
    {
        public SQLContext() {}

        public SQLContext(DbContextOptions<SQLContext> opts) : base(opts) {}

        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedNever();
        }
    }
}
