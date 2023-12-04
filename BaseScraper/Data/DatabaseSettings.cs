using Microsoft.Extensions.Configuration;

namespace BaseScraper.Data
{
    public class DatabaseSettings
    {
        public const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=MotoMarketPrices;Integrated Security=True";

        //public static string TestConnectionString { get; private set; }

        //public DatabaseSettings(IConfiguration configuration)
        //{
        //    TestConnectionString = configuration.GetSection("ScraperSettings:ConnectionString").Value;
        //}
    }
}
