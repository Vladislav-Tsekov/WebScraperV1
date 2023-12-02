using Microsoft.Extensions.Configuration;

namespace BaseScraper.Config
{
    public class ScraperSettings
    {
        public const int MaxPages = 100;
        public static string OutputFolderPath { get; private set; } = null!;

        public ScraperSettings(IConfiguration? configuration)
        {
            OutputFolderPath = configuration.GetSection("ScraperSettings:OutputFolderPath").Value;
        }
    }
}
