using BaseScraper.Calculations;
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

        public async Task SoldMotorcyclesReport(MotoContext context, SaleReport saleReport)
        {
            int totalNumber = context.MotocrossEntries.Count();
            List<MotocrossSoldEntry> soldEntries = context.MotocrossSoldEntries.ToList();
            List<MotocrossMarketPrice> marketPrices = context.MotocrossMarketPrices.ToList();

            HashSet<MotocrossSoldEntry> soldEntriesSet = new(soldEntries.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year));
            HashSet<MotocrossMarketPrice > marketPricesSet = new(marketPrices.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year));

            StreamWriter saleReportWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "SaleReport.csv"));
            saleReportWriter.WriteLine(DateTime.Now);
            saleReportWriter.WriteLine($"There are currently {totalNumber} Motocross announcements.");
            saleReportWriter.WriteLine($"Make, Year, CC, Price Sold, Avg Market Price, Date Listed, Date Sold");

            saleReport.SoldMotorcyclesList(soldEntriesSet, marketPricesSet, saleReportWriter);
            saleReport.CalculateAbsoluteAverages(soldEntriesSet, saleReportWriter);
            saleReport.EngineDisplacementCount(soldEntriesSet, saleReportWriter);
            saleReport.CountOfSalesPerMake(soldEntriesSet, saleReportWriter);
            saleReport.CountOfSalesPerDay(soldEntriesSet, saleReportWriter);

            saleReportWriter.Dispose();

            //TODO - SALE REPORT - LIST BELOW:
            //How to correctly calculate announcement's uptime period, more data?
        }
    }
}
