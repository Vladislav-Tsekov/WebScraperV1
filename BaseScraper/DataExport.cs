using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using BaseScraper.Models;

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
            motoWriter.WriteLine($"Make, CC, Year, Price, Link");

            var dbEntries = context.MotocrossEntries.ToList();

            HashSet<MotocrossEntry> entries = new();
            HashSet<MotocrossEntry> existingEntries = new(dbEntries);

            foreach (var moto in scrapedMoto)
            {
                MotoMake make = context.Makes.FirstOrDefault(mExists => mExists.Make == moto.Make);
                MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == int.Parse(moto.Year));

                motoWriter.WriteLine($"{moto.Make},{moto.CC},{moto.Year},{moto.Price},{moto.Link}");

                if (!dbEntries.Any(e => e.Link == moto.Link))
                {
                    var entry = new MotocrossEntry()
                    {
                        Price = moto.Price,
                        Link = moto.Link,
                        Make = make,
                        Year = year,
                        DateAdded = DateTime.Now
                    };

                    if (moto.CC == StringsConstants.NotAvailable)
                        entry.Cc = null;
                    else
                        entry.Cc = moto.CC;

                    entries.Add(entry);

                }
                else
                {
                    var currentEntry = existingEntries.FirstOrDefault(e => e.Link == moto.Link);

                    if (currentEntry != null)
                    {
                        if (currentEntry.Price != moto.Price)
                        {
                            Console.WriteLine($"Updating entry with link {moto.Link}. Old Price: {currentEntry.Price}, New Price: {moto.Price}");

                            currentEntry.Price = moto.Price;

                            //TODO - ADD AN OPTION TO TRACK PRICE CHANGES? COUNTER MAYBE? 
                            //currentEntry.DateAdded = DateTime.Now;
                        }
                        else
                        {
                            Console.WriteLine($"Entry with link {moto.Link} already exists, and prices match.");
                        }
                    }
                }
            }

            motoWriter.Dispose();

            foreach (var existingEntry in dbEntries)
            {
                if (!scrapedMoto.Any(m => m.Link == existingEntry.Link))
                {
                    existingEntry.IsSold = true;
                    existingEntry.DateSold = DateTime.Now;
                }
            }

            await context.MotocrossEntries.AddRangeAsync(entries);
            await context.SaveChangesAsync();
        }

        public async Task AddMarketPrices(ICollection<Motorcycle> filteredMoto, MotoContext context)
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

                if (motoExists != null)
                {
                    motoExists.AvgPrice = (decimal)m.AveragePrice;
                    motoExists.MeanTrimPrice = (decimal)m.MeanPrice;
                    motoExists.StdDevPrice = (decimal)m.DevPrice;
                    motoExists.MedianPrice = (decimal)m.MedianPrice;
                    motoExists.ModePrice = (decimal)m.ModePrice;
                    motoExists.PriceVariance = (decimal)m.PriceVariance;
                    motoExists.PriceRange = (decimal)m.PriceRange;
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
                        MotoCount = m.MotorcycleCount,
                    };

                    pricesCollection.Add(entity);
                }

                priceWriter.WriteLine($"{m.Make}, {m.Year}, {m.AveragePrice:f2}, {m.MeanPrice:f2}, {m.DevPrice:f2}, {m.MotorcycleCount}");
            }

            priceWriter.Dispose();

            await context.MotocrossMarketPrices.AddRangeAsync(pricesCollection);
            await context.SaveChangesAsync();
        }

        public async Task TransferSoldEntries(MotoContext context) 
        {
            var dbEntries = context.MotocrossEntries.Where(e => e.IsSold == true).ToList();

            HashSet<MotocrossEntry> soldEntries = new(dbEntries);
            HashSet<MotocrossSoldEntry> transferEntries = new();

            foreach (var entry in soldEntries)
            {
                MotocrossSoldEntry newSoldEntry = new() 
                {
                    Make = entry.Make,
                    Year = entry.Year,
                    Cc = entry.Cc,
                    Price = entry.Price,
                    DateAdded = entry.DateAdded,
                    DateSold = entry.DateSold
                };

                transferEntries.Add(newSoldEntry);
            }

            context.MotocrossEntries.RemoveRange(soldEntries);

            await context.MotocrossSoldEntries.AddRangeAsync(transferEntries);
            await context.SaveChangesAsync();
        }
    }
}
