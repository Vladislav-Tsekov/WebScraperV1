using BaseScraper.Calculations;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using Microsoft.EntityFrameworkCore;
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
            List<MotocrossMarketPrice> pricesList = context.MotocrossMarketPrices.ToList();

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
            List<MotocrossSoldEntry> soldEntries = context.MotocrossSoldEntries.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year).ToList();
            List<MotocrossMarketPrice> marketPrices = context.MotocrossMarketPrices.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year).ToList();

            if (soldEntries.Count < 1)
            {
                return Task.CompletedTask;
            }

            StreamWriter salesWriter = new(Path.Combine(OutputFolderPath, SaleReportCsv));
            salesWriter.WriteLine(DateTime.Now);
            salesWriter.WriteLine($"Make, Year, CC, Price Sold, Avg Market Price, Date Listed, Date Sold");

            SaleReport.SoldMotorcyclesList(soldEntries, marketPrices, salesWriter);
            SaleReport.CalculateAbsoluteAverages(soldEntries, salesWriter);
            SaleReport.EngineDisplacementCount(soldEntries, salesWriter);
            SaleReport.CountOfSalesPerMake(soldEntries, salesWriter);
            SaleReport.CountOfSalesPerDay(soldEntries, salesWriter);

            salesWriter.Dispose();

            return Task.CompletedTask;

            //TODO - SALE REPORT - LIST BELOW:
            //How to correctly calculate announcement's uptime period, more data?
        }
    }
}
