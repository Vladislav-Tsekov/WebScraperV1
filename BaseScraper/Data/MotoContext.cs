namespace BaseScraper.Data
{
    using BaseScraper.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class MotoContext : DbContext
    {
        public MotoContext() { }

        public MotoContext(DbContextOptions options) : base(options) { }

        public DbSet<MotoMake> Makes { get; set; }
        public DbSet<MotoYear> Years { get; set; }
        public DbSet<MotocrossMarketPrice> MotocrossMarketPrices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DatabaseSettings.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
