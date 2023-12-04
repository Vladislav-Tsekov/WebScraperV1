using BaseScraper.Data;
using BaseScraper.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace WebScraperV1.Data
{

    public class MotoContext : DbContext
    {
        public MotoContext(DbContextOptions options) : base(options) {}

        public DbSet<MotoMake> Makes { get; set; }
        public DbSet<MotoYear> Years { get; set; }
        public DbSet<MotocrossMarketPrice> ListOfPrices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(DatabaseSettings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
