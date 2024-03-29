﻿using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using BaseScraper.Models;
using static BaseScraper.Config.StringsConstants;

namespace BaseScraper
{
    public class DataExport
    {
        public static async Task UpdateMakesTable(List<string> distinctMakes, MotoContext context)
        {
            List<string> existingMakes = context.Makes.Select(m => m.Make).ToList();
            HashSet<MotoMake> makesToAdd = new();

            foreach (var make in distinctMakes)
            {
                if (!existingMakes.Contains(make))
                {
                    var currentMake = new MotoMake { Make = make };
                    makesToAdd.Add(currentMake);
                } 
            }

            if (makesToAdd.Count > 0)
            {
                await context.Makes.AddRangeAsync(makesToAdd);
                await context.SaveChangesAsync();
            }
        }

        public static async Task UpdateYearsTable(List<int> distinctYears, MotoContext context)
        {
            List<int> existingYears = context.Years.Select(m => m.Year).ToList();
            HashSet<MotoYear> yearsToAdd = new();

            foreach (var year in distinctYears)
            {
                if (!existingYears.Contains(year))
                {
                    var currentYear = new MotoYear { Year = (year) };
                    yearsToAdd.Add(currentYear);
                } 
            }

            if (yearsToAdd.Count > 0)
            {
                await context.Years.AddRangeAsync(yearsToAdd);
                await context.SaveChangesAsync();
            }  
        }

        public static async Task AddMotorcycleEntries(ICollection<Motocross> scrapedMoto, MotoContext context)
        {
            using StreamWriter motoWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, AllLinksCsv));
            motoWriter.WriteLine(AllLinksTitles);

            var dbEntries = context.MotocrossEntries.ToList();

            HashSet<MotocrossEntry> entries = new();
            HashSet<MotocrossEntry> existingEntries = new(dbEntries);

            foreach (var moto in scrapedMoto)
            {
                MotoMake make = context.Makes.FirstOrDefault(m => m.Make == moto.Make);
                MotoYear year = context.Years.FirstOrDefault(y => y.Year == moto.Year);

                if (make is null || year is null)
                {
                    continue;
                }

                motoWriter.WriteLine(String.Format(AllLinksMotoInfo, moto.Make, moto.CC, moto.Year, moto.Price, moto.Link));

                if (!dbEntries.Any(e => e.Link == moto.Link))
                {
                    var entry = new MotocrossEntry()
                    {
                        Make = make,
                        Year = year,
                        Cc = moto.CC,
                        Price = moto.Price,
                        Link = moto.Link,
                        DateAdded = DateTime.Now
                    };

                    entries.Add(entry);
                }
                else
                {
                    var currentEntry = existingEntries.FirstOrDefault(e => e.Link == moto.Link);

                    if (currentEntry != null)
                    {
                        if (currentEntry.Price != moto.Price)
                        {
                            Console.WriteLine(String.Format(AllLinksPriceChange, moto.Link, currentEntry.Price, moto.Price));

                            currentEntry.OldPrice = currentEntry.Price;
                            currentEntry.Price = moto.Price;
                            currentEntry.PriceChanges += 1;
                        }
                        else
                        {
                            Console.WriteLine(String.Format(AllLinksEntryExists, moto.Link));
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
                }
            }

            await context.MotocrossEntries.AddRangeAsync(entries);
            await context.SaveChangesAsync();
        }

        public static async Task AddMarketPrices(ICollection<Motocross> filteredMoto, MotoContext context)
        {
            var averagePrices = filteredMoto
            .GroupBy(m => new { m.Make, m.Year })
            .Select(group => new
            {
                group.Key.Make,
                group.Key.Year,
                AveragePrice = group.Average(m => m.Price),
                MeanPrice = MeanValues.MeanTrim(group.Select(m => m.Price)),
                DevPrice = MeanValues.StdDev(group.Select(m => m.Price)),
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

            StreamWriter priceWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, MarketPriceCsv));
            priceWriter.WriteLine(MarketPriceTitles);

            HashSet<MotocrossMarketPrice> marketPrices = new();

            foreach (var m in averagePrices)
            {
                MotocrossMarketPrice motoExists = context.MotocrossMarketPrices
                    .FirstOrDefault(record => record.Make.Make == m.Make && record.Year.Year == m.Year);

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
                    MotoYear year = context.Years.FirstOrDefault(yExists => yExists.Year == m.Year);

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

                    marketPrices.Add(entity);
                }

                priceWriter.WriteLine(String.Format(MarketPriceMotoInfo, 
                                                    m.Make,
                                                    m.Year, 
                                                    m.MotorcycleCount, 
                                                    m.AveragePrice, 
                                                    m.MeanPrice, 
                                                    m.DevPrice, 
                                                    m.MedianPrice, 
                                                    m.PriceVariance));
            }

            priceWriter.Dispose();

            await context.MotocrossMarketPrices.AddRangeAsync(marketPrices);
            await context.SaveChangesAsync();
        }

        public static async Task TransferSoldEntries(MotoContext context)
        {
            var dbEntries = context.MotocrossEntries.Where(e => e.IsSold == true).ToList();

            HashSet<MotocrossEntry> soldEntries = new(dbEntries);
            HashSet<MotocrossSoldEntry> transferEntries = new();

            foreach (var entry in soldEntries)
            {
                MotoMake newMake = new();
                MotoYear newYear = new();

                try
                {
                    newMake = await context.Makes.FindAsync(entry.MakeId);
                    newYear = await context.Years.FindAsync(entry.YearId);
                }
                catch (Exception)
                {
                    Console.WriteLine("Make or Year non-existent.");
                    continue;
                }

                MotocrossSoldEntry newSoldEntry = new() 
                {
                    Make = newMake,
                    Year = newYear,
                    Cc = entry.Cc,
                    Price = entry.Price,
                    DateAdded = entry.DateAdded,
                    DateSold = DateTime.Now
                };

                if (newSoldEntry.Make is not null && newSoldEntry.Year is not null)
                {
                    transferEntries.Add(newSoldEntry);
                }
            }

            context.MotocrossEntries.RemoveRange(soldEntries);

            await context.MotocrossSoldEntries.AddRangeAsync(transferEntries);
            await context.SaveChangesAsync();
        }
    }
}
