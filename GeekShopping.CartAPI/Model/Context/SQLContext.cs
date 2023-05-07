using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Model.Context
{
    public class SQLContext : DbContext
    {
        public SQLContext(DbContextOptions<SQLContext> opts) : base(opts) {}

        public DbSet<Product> Products { get; set; }

        public DbSet<CartDetail> CartDetails { get; set; }

        public DbSet<CartHeader> CartHeaders { get; set; }


    }
}
