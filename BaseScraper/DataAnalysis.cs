using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;

namespace BaseScraper
{
    public class DataAnalysis
    {
        //TODO - MUST FIND A WAY TO INDENTIFY TRENDS
        //TODO - IDEAS FOR DATA INTERPRETATION OF SOLD ENTRIES

        public async Task TotalMotorcyclesCountByMake(MotoContext context)
        {
            List<MotocrossMarketPrice> pricesList = context.MotocrossMarketPrices.ToList();

            Dictionary<string, int> makeCountPairs = new();

            foreach (var motorcycle in pricesList)
            {
                if (!makeCountPairs.ContainsKey(motorcycle.Make.Make.ToString()))
                {
                    makeCountPairs.Add(motorcycle.Make.Make, 0);
                }
            }

            foreach (var motorcycle in pricesList) 
            {
                makeCountPairs[motorcycle.Make.Make.ToString()] += motorcycle.MotoCount;
            }

            StreamWriter makeCountWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "CountByMake.csv"));

            makeCountWriter.WriteLine(DateTime.Now);

            foreach (var kvp in makeCountPairs)
            {
                makeCountWriter.WriteLine($"{kvp.Key.ToUpper()}, {kvp.Value}");
            }

            makeCountWriter.Dispose();
        }

        public async Task TotalMotorcyclesCountByYear(MotoContext context) 
        {
            List<MotocrossMarketPrice> pricesList = context.MotocrossMarketPrices.ToList();
            //TODO - LEARN WHY .ASNOTRACKING() CAUSES PROBLEMS HERE

            SortedDictionary<int, int> yearCountPairs = new();

            foreach (var motorcycle in pricesList)
            {
                if (!yearCountPairs.ContainsKey(motorcycle.Year.Year))
                {
                    yearCountPairs.Add(motorcycle.Year.Year, 0);
                }
            }

            foreach (var motorcycle in pricesList)
            {
                yearCountPairs[motorcycle.Year.Year] += motorcycle.MotoCount;
            }

            StreamWriter yearCountWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "CountByYear.csv"));
            
            yearCountWriter.WriteLine(DateTime.Now);

            foreach (var kvp in yearCountPairs)
            {
                yearCountWriter.WriteLine($"{kvp.Key}, {kvp.Value}");
            }

            yearCountWriter.Dispose();
        }

        public async Task MotorcyclesWithHighVariance(MotoContext context) 
        {
            List<MotocrossMarketPrice> highVariance = context.MotocrossMarketPrices.Where(m => m.PriceVariance > 0.19m).ToList();

            StreamWriter varianceWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "HighVariance.csv"));
            varianceWriter.WriteLine(DateTime.Now);
            varianceWriter.WriteLine("Make,Year,Count,Variance");

            foreach (var entity in highVariance)
            {
                varianceWriter.WriteLine($"{entity.Make.Make},{entity.Year.Year},{entity.MotoCount},{entity.PriceVariance:f3}");
            }

            varianceWriter.Dispose();
        }

        public async Task MotorcyclesWithHighPriceRange(MotoContext context)
        {
            List<MotocrossMarketPrice> extremeRange = context.MotocrossMarketPrices.Where(m => m.PriceVariance > 0.19m).ToList();

            StreamWriter rangeWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "ExtremeRange.csv"));
            rangeWriter.WriteLine(DateTime.Now);
            rangeWriter.WriteLine("Make,Year,Count,Range");

            foreach (var entity in extremeRange)
            {
                rangeWriter.WriteLine($"{entity.Make.Make},{entity.Year.Year},{entity.MotoCount},{entity.PriceRange:f0}");
            }

            rangeWriter.Dispose();
        }

        public async Task SoldMotorcyclesAnalysis(MotoContext context)
        {
            List<MotocrossSoldEntry> soldEntries = context.MotocrossSoldEntries.ToList();
            List<MotocrossMarketPrice> marketPrices = context.MotocrossMarketPrices.ToList();

            HashSet<MotocrossSoldEntry> soldEntriesSet = new(soldEntries);
            HashSet<MotocrossMarketPrice > marketPricesSet = new(marketPrices);

            StreamWriter saleReportWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "SaleReport.csv"));
            saleReportWriter.WriteLine(DateTime.Now);

            foreach (var entry in soldEntriesSet)
            {
                saleReportWriter.WriteLine($"{entry.Make.Make}, {entry.Year.Year}, {entry.Cc}, {entry.Price}, {entry.DateAdded.ToShortDateString()}, {entry.DateSold.ToShortDateString()}");

                var currentPrice = marketPricesSet.Where(m => m.Make.Make == entry.Make.Make &&
                                                                  m.Year.Year == entry.Year.Year).FirstOrDefault();

                saleReportWriter.WriteLine($"This motorcycle was sold for: {entry.Price}. Average Market Price: {currentPrice.AvgPrice}");
            }

            var averagePrice = soldEntries.Average(m => m.Price);
            var averageYear = soldEntries.Average(m => m.Year.Year);

            double countOf250 = soldEntriesSet.Where(m => m.Cc == "250").Count();
            double countOf350 = soldEntriesSet.Where(m => m.Cc == "350").Count();
            double countOf450 = soldEntriesSet.Where(m => m.Cc == "450").Count();

            double totalCount = soldEntriesSet.Where(m => m.Cc == "250" || m.Cc == "350" || m.Cc == "450").Count();

            saleReportWriter.WriteLine($"The average price for all sold entries is: {averagePrice:f2}");
            saleReportWriter.WriteLine($"The average year for all sold entries is: {Math.Round(averageYear)}");
            saleReportWriter.WriteLine($"250cc: {countOf250} out of {totalCount}. Ratio of {totalCount * 100:f2}%");
            saleReportWriter.WriteLine($"350cc: {countOf350} out of {totalCount}. Ratio of {totalCount * 100:f2}%");
            saleReportWriter.WriteLine($"450cc: {countOf450} out of {totalCount}. Ratio of {totalCount * 100:f2}%");

            Dictionary<string, int> makeCountPairs = new();

            foreach (var motorcycle in soldEntriesSet)
            {
                if (!makeCountPairs.ContainsKey(motorcycle.Make.Make.ToString()))
                {
                    makeCountPairs.Add(motorcycle.Make.Make, 0);
                }
            }

            foreach (var motorcycle in soldEntriesSet)
            {
                makeCountPairs[motorcycle.Make.Make.ToString()] += 1;
            }

            //TODO - BREAK DOWN AND ANALYZE FURTHER?
            foreach (var kvp in makeCountPairs)
            {
                saleReportWriter.WriteLine($"A total of {kvp.Value} {kvp.Key} sold!");
            }

            //Check whether the price is more or less than the market price

            saleReportWriter.Dispose();

            //TODO - SALE REPORT - LIST BELOW:
            //How to correctly calculate announcement's uptime period, more data?
        }
    }
}
