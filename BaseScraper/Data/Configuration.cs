using Microsoft.Extensions.Configuration;

namespace BaseScraper.Data
{
    public class Configuration
    {
        public static string ConnectionString { get; private set; }

        public Configuration(IConfiguration configuration)
        {
            ConnectionString = configuration.GetSection("ScraperSettings:ConnectionString").Value;
        }

    }
}
