using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using BaseScraper.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseScraper
{
    public class DataExport
    {
        public async Task PopulateMakesTable(List<string> distinctMakes, MotoContext context)
        {
            var dbMakes = context.Makes.ToList();

            HashSet<MotoMake> makes = new();
            HashSet<MotoMake> existingMakes = new(dbMakes);

            makes.IntersectWith(existingMakes);

            if (makes.Count > 0)
            {
                foreach (var make in distinctMakes)
                {
                    var currentMake = new MotoMake { Make = make };
                    makes.Add(currentMake);
                }

                await context.Makes.AddRangeAsync(makes);
                await context.SaveChangesAsync();
            }
        }

        public async Task PopulateYearsTable(List<int> distinctYears, MotoContext context)
        {
            var dbYears = context.Years.ToList();

            HashSet<MotoYear> years = new();
            HashSet<MotoYear> existingYears = new(dbYears);

            years.IntersectWith(existingYears);

            if (years.Count > 0)
            {
                foreach (var year in distinctYears)
                {
                    var currentYear = new MotoYear { Year = (year) };
                    years.Add(currentYear);
                }

                await context.Years.AddRangeAsync(years);
                await context.SaveChangesAsync();
            } 
        }

        public async Task AddMotorcycleEntries(ICollection<Motorcycle> scrapedMoto, MotoContext context)
        {
            using StreamWriter motoWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "MotocrossData.csv"));
            motoWriter.Write($"Make, CC, Year, Price, Link{Environment.NewLine}");

            var dbEntries = context.MotocrossEntries.ToList();

            HashSet<MotocrossEntry> entries = new();
            HashSet<MotocrossEntry> existingEntries = new(dbEntries);

            foreach (var m in scrapedMoto)
            {
                MotoMake make = context.Makes.FirstOrDefault(mExists => mExists.Make == m.Make);
                MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == int.Parse(m.Year));

                if (!dbEntries.Any(dbEntry => dbEntry.Link == m.Link))
                {
                    var entry = new MotocrossEntry()
                    {
                        Price = m.Price,
                        Link = m.Link,
                        Make = make,
                        Year = year,
                        DateAdded = DateTime.Now
                    };

                    if (m.CC == StringsConstants.NotAvailable)
                        entry.Cc = null;
                    else
                        entry.Cc = m.CC;

                    entries.Add(entry);
                    motoWriter.Write($"{m.Make}, {m.CC}, {m.Year}, {m.Price}, {m.Link}{Environment.NewLine}");
                }
                else
                {
                    Console.WriteLine($"Entry with link {m.Link} already exists.");
                }
            }

            motoWriter.Dispose();

            foreach (var existingEntry in dbEntries)
            {
                if (!scrapedMoto.Any(m => m.Link == existingEntry.Link))
                {
                    existingEntry.IsSold = true;
                    existingEntry.DateSold = DateTime.Now;

                    //TODO - CASE NOT TESTED, MUST COLLECT NEW DATA TO TEST FUNCTIONALITY
                }
            }

            //TODO - MOVE SOLD MOTORCYCLES TO A DIFFERENT TABLE AND REMOVE THEM FROM PREVIOUS ONE!?
            //OR FIND A WAY TO TRACK SALES AND OTHER TRENDS

            await context.MotocrossEntries.AddRangeAsync(entries);
            await context.SaveChangesAsync();
        }

        public async Task CalculateMarketPrices(ICollection<Motorcycle> filteredMoto, MotoContext context)
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

            HashSet<MotocrossMarketPrice> pricesCollection = new();

            foreach (var m in averagePrices)
            {
                MotocrossMarketPrice motoExists = context.MotocrossMarketPrices
                    .FirstOrDefault(record => record.Make.Make == m.Make && record.Year.Year == int.Parse(m.Year));

                double customPrice = (m.AveragePrice + m.DevPrice + m.MeanPrice + m.ModePrice + m.MedianPrice) / 5;

                if (motoExists != null)
                {
                    motoExists.AvgPrice = (decimal)m.AveragePrice;
                    motoExists.MeanTrimPrice = (decimal)m.MeanPrice;
                    motoExists.StdDevPrice = (decimal)m.DevPrice;
                    motoExists.MedianPrice = (decimal)m.MedianPrice;
                    motoExists.ModePrice = (decimal)m.ModePrice;
                    motoExists.PriceVariance = (decimal)m.PriceVariance;
                    motoExists.PriceRange = (decimal)m.PriceRange;
                    motoExists.FinalPrice = (decimal)customPrice;
                    motoExists.MotoCount = m.MotorcycleCount;
                }
                else
                {
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

                priceWriter.Write($"{m.Make}, {m.Year}, {m.AveragePrice:f2}, {m.MeanPrice:f2}, {m.DevPrice:f2}, {customPrice:f2}, {m.MotorcycleCount}{Environment.NewLine}");
            }

            priceWriter.Dispose();

            await context.MotocrossMarketPrices.AddRangeAsync(pricesCollection);
            await context.SaveChangesAsync();
        }
    }
}
