using Microsoft.Extensions.Configuration;

namespace BaseScraper.Data
{
    public class DatabaseSettings
    {
        public static string ConnectionString { get; private set; }

        public DatabaseSettings(IConfiguration configuration)
        {
            ConnectionString = configuration.GetSection("ScraperSettings:ConnectionString").Value;
        }

    }
}
