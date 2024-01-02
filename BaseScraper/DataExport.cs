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

        public async Task AddMotorcycleEntries(ICollection<Motorcycle> filteredMoto)
        {
            using StreamWriter motoWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "MotocrossData.csv"));
            motoWriter.Write($"Make, CC, Year, Price, Link{Environment.NewLine}");

            using MotoContext context = new();

            HashSet<MotocrossEntry> entriesCollection = new();
            List<MotoMake> makes = new();
            List<MotoYear> years = new();

            foreach (var m in filteredMoto)
            {
                var entry = new MotocrossEntry()
                {
                    Price = m.Price,
                    Link = m.Link,
                };

                MotoMake make = context.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);

                if (make == null)
                {
                    make = new MotoMake { Make = m.Make };
                    makes.Add(make);
                }

                entry.Make = make;

                if (int.TryParse(m.Year, out int parsedYear))
                {
                    MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == parsedYear);

                    if (year == null)
                    {
                        year = new MotoYear { Year = parsedYear };
                        years.Add(year);
                    }

                    entry.Year = year;
                }
                else
                {
                    Console.WriteLine($"Error: Unable to parse year '{m.Year}' to an integer for motorcycle {m.Make}.");
                }

                entriesCollection.Add(entry);

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

            makes = makes.Distinct().ToList();
            years = years.Distinct().ToList();

            await context.Makes.AddRangeAsync(makes);
            await context.Years.AddRangeAsync(years);
            await context.SaveChangesAsync();

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

                var entity = new MotocrossMarketPrice
                {
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

                MotoMake make = context.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);

                if (make == null)
                {
                    //TODO - Troubleshoot or find another way to populate table
                    make = new MotoMake { Make = m.Make };
                    context.Makes.Add(make);
                }

                entity.Make = make;

                if (int.TryParse(m.Year, out int parsedYear))
                {
                    MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == parsedYear);

                    if (year == null)
                    {
                        //TODO - Troubleshoot or find another way to populate table
                        year = new MotoYear { Year = parsedYear };
                        context.Years.Add(year);
                    }

                    entity.Year = year;
                }
                else
                {
                    Console.WriteLine($"Error: Unable to parse year '{m.Year}' to an integer for motorcycle {m.Make}.");
                }

                pricesCollection.Add(entity);

                Console.WriteLine($"{entity.GetType().Name} motorcycle added successfully to the database.");
            }

            priceWriter.Dispose();

            await context.MotocrossMarketPrices.AddRangeAsync(pricesCollection);
            await context.SaveChangesAsync();
            await context.DisposeAsync();
        }
    }
}
