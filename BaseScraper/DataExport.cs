using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using BaseScraper.Models;

namespace BaseScraper
{
    public class DataExport
    {
        public async Task PopulateMakesTable(List<string> distinctMakes)
        {
            HashSet<MotoMake> makes = new();

            foreach (var make in distinctMakes)
            {
                MotoMake currentMake = new MotoMake { Make = make};
                makes.Add(currentMake);
            }

            using MotoContext context = new();
            await context.Makes.AddRangeAsync(makes);
            await context.SaveChangesAsync();
        }

        public async Task PopulateYearsTable(List<int> distinctYears)
        {
            HashSet<MotoYear> years = new();

            foreach (var year in distinctYears)
            {
                MotoYear currentYear = new MotoYear { Year = (year) };
                years.Add(currentYear);
            }

            using MotoContext context = new();
            await context.Years.AddRangeAsync(years);
            await context.SaveChangesAsync();
        }

        public async Task AddMotorcycleEntries(ICollection<Motorcycle> filteredMoto)
        {
            using StreamWriter motoWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "MotocrossData.csv"));
            motoWriter.Write($"Make, CC, Year, Price, Link{Environment.NewLine}");

            using MotoContext context = new();

            HashSet<MotocrossEntry> entriesCollection = new();

            foreach (var m in filteredMoto)
            {
                MotoMake make = context.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);
                MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == int.Parse(m.Year));

                var entry = new MotocrossEntry()
                {
                    Price = m.Price,
                    Link = m.Link,
                    Make = make,
                    Year = year
                };

                if (m.CC == "N/A")
                    entry.Cc = null;
                else
                    entry.Cc = m.CC;

                entriesCollection.Add(entry);
                motoWriter.Write($"{m.Make}, {m.CC}, {m.Year}, {m.Price}, {m.Link}{Environment.NewLine}");
            }

            motoWriter.Dispose();

            // TODO - ADD HASHSET TO COMPARE EXISTING DATA AND NEW ONE
            // PERHAPS A TRACKER - WHEN WAS THE VEHICLE SOLD / PUBLISHED / ETC.
            // .INTERSECT(); / .UNIONWITH();

            await context.MotocrossEntries.AddRangeAsync(entriesCollection);
            await context.SaveChangesAsync();
            await context.DisposeAsync();
        }

        public async Task CalculateMarketPrices(ICollection<Motorcycle> filteredMoto)
        {
            var averagePrices = filteredMoto
            .GroupBy(m => new { m.Make, m.Year })
            .Select(group => new
            {
                group.Key.Make,
                group.Key.Year,
                AveragePrice = group.Average(m => m.Price),
                MeanPrice = MeanValues.MeanTrim(group.Select(m => m.Price), MeanValues.trimPercentage),
                DevPrice = MeanValues.Dev(group.Select(m => m.Price), MeanValues.deviationThreshold),
                MedianPrice = MeanValues.Median(group.Select(m => m.Price)),
                ModePrice = MeanValues.Mode(group.Select(m => m.Price)),
                PriceVariance = MeanValues.Variance(group.Select(m => m.Price)),
                PriceRange = MeanValues.Range(group.Select(m => m.Price)),
                MotorcycleCount = group.Count(),
            })
            .OrderBy(m => m.Make)
            .ThenBy(m => m.Year)
            .ThenBy(m => m.AveragePrice)
            .ToList();

            using StreamWriter priceWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "AvgPriceMotocross.csv"));
            priceWriter.Write($"Make, Year, Average Price, Mean Price, StdDev Price, Combined Price, Count{Environment.NewLine}");

            using MotoContext context = new();

            HashSet<MotocrossMarketPrice> pricesCollection = new();

            foreach (var m in averagePrices)
            {
                double customPrice = (m.AveragePrice + m.DevPrice + m.MeanPrice + m.ModePrice + m.MedianPrice) / 5;

                priceWriter.Write($"{m.Make}, {m.Year}, {m.AveragePrice:f2}, {m.MeanPrice:f2}, {m.DevPrice:f2}, {customPrice:f2}, {m.MotorcycleCount}{Environment.NewLine}");

                MotoMake make = context.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);
                MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == int.Parse(m.Year));

                var entity = new MotocrossMarketPrice
                {
                    Make = make,
                    Year = year,
                    AvgPrice = (decimal)m.AveragePrice,
                    MeanTrimPrice = (decimal)m.MeanPrice,
                    StdDevPrice = (decimal)m.DevPrice,
                    MedianPrice = (decimal)m.MedianPrice,
                    ModePrice = (decimal)m.ModePrice,
                    PriceVariance = (decimal)m.PriceVariance,
                    PriceRange = (decimal)m.PriceRange,
                    FinalPrice = (decimal)customPrice,
                    MotoCount = m.MotorcycleCount,
                };

                pricesCollection.Add(entity);
            }

            priceWriter.Dispose();

            await context.MotocrossMarketPrices.AddRangeAsync(pricesCollection);
            await context.SaveChangesAsync();
            await context.DisposeAsync();
        }
    }
}
