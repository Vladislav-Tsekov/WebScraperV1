using Microsoft.Extensions.Configuration;

namespace BaseScraper.Config
{
    public class ScraperSettings
    {
        public const int MaxPages = 150;
        public static string OutputFolderPath { get; private set; }

        public ScraperSettings(IConfiguration configuration)
        {
            OutputFolderPath = configuration.GetSection("ScraperSettings:OutputFolderPath").Value;
        }
    }
}
