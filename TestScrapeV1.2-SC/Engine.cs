using TestScrapeV1._2_SC;

class Engine
{
    static async Task Main(string[] args)
    {
        try
        {
            List<Motorcycle> motorcycles = await Scraper.ScrapeDataAsync();

            if (motorcycles != null)
            {
                // Filter, write to files, and perform other operations

                // For example, writing to CSV files
                WriteToCsv("../../../MotoData.csv", motorcycles);
                WriteAveragePrices("../../../AvgPriceModelYear.csv", motorcycles);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void WriteToCsv(string filePath, List<Motorcycle> motorcycles)
    {
        using StreamWriter writer = new(filePath);

        writer.Write($"Make, CC, Year, Price{Environment.NewLine}");

        foreach (var motorcycle in motorcycles)
        {
            writer.Write($"{motorcycle.Make}, {motorcycle.CC}, {motorcycle.Year}, {motorcycle.Price}{Environment.NewLine}");
        }
    }

    private static void WriteAveragePrices(string filePath, List<Motorcycle> motorcycles)
    {
        var averagePrices = motorcycles
            .Where(m => m.Price > 3000)
            .GroupBy(m => new { m.Make, m.Year })
            .Select(group => new
            {
                group.Key.Make,
                group.Key.Year,
                AveragePrice = group.Average(m => m.Price)
            })
            .OrderBy(m => m.Make)
            .ThenBy(m => m.Year)
            .ThenBy(m => m.AveragePrice);

        using StreamWriter writer = new(filePath);

        writer.Write($"Make, Year, Average Price{Environment.NewLine}");

        foreach (var moto in averagePrices)
        {
            writer.Write($"{moto.Make}, {moto.Year}, {moto.AveragePrice:f2}{Environment.NewLine}");
        }
    }
}