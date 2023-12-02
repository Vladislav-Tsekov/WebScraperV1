using Microsoft.Extensions.Configuration;

namespace BaseScraper.Config
{
    public class ScraperSettings
    {
        public const int MaxPages = 1;
        public string OutputFolderPath { get => OutputConfig(); }

        public string OutputConfig() 
        {
            string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            ScraperSettings? settings = new();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath)
                .Build();

            settings = configuration.GetSection("ScraperSettings").Get<ScraperSettings>();

            string outputFolderPath = settings!.OutputFolderPath;
            return outputFolderPath;
        }
    }
}
