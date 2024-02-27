using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using static BaseScraper.Config.ScraperSettings;
using static BaseScraper.Config.StringsConstants;

namespace BaseScraper
{
    public class DataAnalysis
    {
        //TODO - MUST FIND A WAY TO INDENTIFY TRENDS
        //TODO - IDEAS FOR DATA INTERPRETATION OF SOLD ENTRIES

        public static Task MarketOverviewReport(MotoContext context) 
        {
            List<MotocrossEntry> entriesList = context.MotocrossEntries.ToList();
            List<MotocrossMarketPrice> pricesList = context.MotocrossMarketPrices.Where(m => m.Year.Year != 0).ToList();

            HashSet<MotocrossEntry> entriesSet = new(entriesList);

            StreamWriter marketWriter = new(Path.Combine(OutputFolderPath, MarketOverviewCsv));
            marketWriter.WriteLine(DateTime.Now);

            MarketOverview.MarketShareByEngineDisplacement(entriesSet, marketWriter);
            MarketOverview.MarketShareByMakeAndYear(pricesList, marketWriter);

            marketWriter.Dispose();

            return Task.CompletedTask;
        }

        public static Task UnusualValuesReport(MotoContext context)
        {
            //TODO - ADD EVEN MORE STATS TO FOLLOW

            List<MotocrossMarketPrice> highVariance = context.MotocrossMarketPrices.Where(m => m.PriceVariance > 0.19m).ToList();
            List<MotocrossMarketPrice> extremeRange = context.MotocrossMarketPrices.Where(m => m.PriceRange > 2000).ToList();

            StreamWriter marketOutliers = new(Path.Combine(OutputFolderPath, MarketOutliersCsv));
            marketOutliers.WriteLine($"{DateTime.Now:d}");

            marketOutliers.WriteLine("Make,Year,Count,Variance");

            foreach (var entity in highVariance)
            {
                marketOutliers.WriteLine($"{entity.Make.Make},{entity.Year.Year},{entity.MotoCount},{entity.PriceVariance:f3}");
            }

            marketOutliers.WriteLine("Make,Year,Count,Range");

            foreach (var entity in extremeRange)
            {
                marketOutliers.WriteLine($"{entity.Make.Make},{entity.Year.Year},{entity.MotoCount},{entity.PriceRange:f0}");
            }

            marketOutliers.Dispose();

            return Task.CompletedTask;
        }

        public static Task SoldMotorcyclesReport(MotoContext context)
        {
            List<MotocrossSoldEntry> soldEntries = context.MotocrossSoldEntries.ToList();
            List<MotocrossMarketPrice> marketPrices = context.MotocrossMarketPrices.ToList();

            HashSet<MotocrossSoldEntry> soldEntriesSet = new(soldEntries.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year));
            HashSet<MotocrossMarketPrice > marketPricesSet = new(marketPrices.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year));

            if (soldEntriesSet.Count < 1)
            {
                return Task.FromCanceled(CancellationToken.None);
            }

            StreamWriter salesWriter = new(Path.Combine(OutputFolderPath, SaleReportCsv));
            salesWriter.WriteLine(DateTime.Now);
            salesWriter.WriteLine($"Make, Year, CC, Price Sold, Avg Market Price, Date Listed, Date Sold");

            SaleReport.SoldMotorcyclesList(soldEntriesSet, marketPricesSet, salesWriter);
            SaleReport.CalculateAbsoluteAverages(soldEntriesSet, salesWriter);
            SaleReport.EngineDisplacementCount(soldEntriesSet, salesWriter);
            SaleReport.CountOfSalesPerMake(soldEntriesSet, salesWriter);
            SaleReport.CountOfSalesPerDay(soldEntriesSet, salesWriter);

            salesWriter.Dispose();

            return Task.CompletedTask;

            //TODO - SALE REPORT - LIST BELOW:
            //How to correctly calculate announcement's uptime period, more data?
        }
    }
}
