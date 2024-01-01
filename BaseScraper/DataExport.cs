using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using BaseScraper.Models;

namespace BaseScraper
{
    public class DataExport
    {
        public async Task AddMotorcycleEntries(ICollection<Motorcycle> filteredMoto)
        {
            using StreamWriter motoWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "MotocrossData.csv"));
            motoWriter.Write($"Make, CC, Year, Price, Link{Environment.NewLine}");

            using MotoContext entryTableData = new();
            HashSet<MotocrossEntry> entriesCollection = new();

            foreach (var m in filteredMoto)
            {
                var entry = new MotocrossEntry()
                {
                    Price = m.Price,
                    Link = m.Link,
                };

                if (int.TryParse(m.Year, out int parsedYear))
                {
                    MotoYear year = entryTableData.Years.FirstOrDefault(yExists => yExists.Year == parsedYear);

                    if (year == null)
                    {
                        //TODO - Troubleshoot or find another way to populate table
                        year = new MotoYear { Year = parsedYear };
                        entryTableData.Years.Add(year);
                    }

                    entry.Year = year;
                }
                else
                {
                    Console.WriteLine($"Invalid year format: {m.Year}");
                    continue;
                }

                MotoMake make = entryTableData.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);

                if (make == null)
                {
                    //TODO - Troubleshoot or find another way to populate table
                    make = new MotoMake { Make = m.Make };
                    entryTableData.Makes.Add(make);
                }

                entry.Make = make;

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

            await entryTableData.MotocrossEntries.AddRangeAsync(entriesCollection);
            await entryTableData.SaveChangesAsync();
            await entryTableData.DisposeAsync();
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
            .ThenBy(m => m.AveragePrice);



            using StreamWriter priceWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "AvgPriceMotocross.csv"));
            priceWriter.Write($"Make, Year, Average Price, Mean Price, StdDev Price, Combined Price, Count{Environment.NewLine}");

            using MotoContext pricesTableData = new();
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

                MotoMake make = pricesTableData.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);

                if (int.TryParse(m.Year, out int parsedYear))
                {
                    MotoYear year = pricesTableData.Years.FirstOrDefault(yExists => yExists.Year == parsedYear);

                    if (make == null)
                    {
                        //TODO - Troubleshoot or find another way to populate table
                        make = new MotoMake { Make = m.Make };
                        pricesTableData.Makes.Add(make);
                    }
                    if (year == null)
                    {
                        //TODO - Troubleshoot or find another way to populate table
                        year = new MotoYear { Year = parsedYear };
                        pricesTableData.Years.Add(year);
                    }

                    entity.Make = make;
                    entity.Year = year;

                    pricesTableData.Add(entity);

                    Console.WriteLine($"{entity.GetType().Name} motorcycle added successfully to the database.");
                }
                else
                {
                    Console.WriteLine($"Error: Unable to parse year '{m.Year}' to an integer for motorcycle {m.Make}.");
                }
            }

            priceWriter.Dispose();

            await pricesTableData.MotocrossMarketPrices.AddRangeAsync(pricesCollection);
            await pricesTableData.SaveChangesAsync();
            await pricesTableData.DisposeAsync();
        }
    }
}
