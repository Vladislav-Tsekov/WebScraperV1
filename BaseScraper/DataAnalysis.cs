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
        public async Task MarketOverviewReport(MotoContext context, MarketOverview marketOverview) 
        {
            StreamWriter marketReport = new(Path.Combine(ScraperSettings.OutputFolderPath, "MarketOverview.csv"));

            marketOverview.MarketShareByEngineDisplacement();
            marketOverview.MarketShareByMakeAndYear();


        }

        public async Task UnusualValuesReport(MotoContext context, StreamWriter marketOutliers)
        {
            //TODO - ADD EVEN MORE STATS TO FOLLOW

            List<MotocrossMarketPrice> highVariance = context.MotocrossMarketPrices.Where(m => m.PriceVariance > 0.19m).ToList();
            List<MotocrossMarketPrice> extremeRange = context.MotocrossMarketPrices.Where(m => m.PriceRange > 2000).ToList();

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
        }

        public async Task SoldMotorcyclesReport(MotoContext context, SaleReport saleReport)
        {
            List<MotocrossSoldEntry> soldEntries = context.MotocrossSoldEntries.ToList();
            List<MotocrossMarketPrice> marketPrices = context.MotocrossMarketPrices.ToList();

            HashSet<MotocrossSoldEntry> soldEntriesSet = new(soldEntries.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year));
            HashSet<MotocrossMarketPrice > marketPricesSet = new(marketPrices.OrderBy(m => m.Make.Make).ThenBy(m => m.Year.Year));

            StreamWriter salesReport = new(Path.Combine(ScraperSettings.OutputFolderPath, "SaleReport.csv"));
            salesReport.WriteLine(DateTime.Now);
            salesReport.WriteLine($"Make, Year, CC, Price Sold, Avg Market Price, Date Listed, Date Sold");

            saleReport.SoldMotorcyclesList(soldEntriesSet, marketPricesSet, salesReport);
            saleReport.CalculateAbsoluteAverages(soldEntriesSet, salesReport);
            saleReport.EngineDisplacementCount(soldEntriesSet, salesReport);
            saleReport.CountOfSalesPerMake(soldEntriesSet, salesReport);
            saleReport.CountOfSalesPerDay(soldEntriesSet, salesReport);

            salesReport.Dispose();

            //TODO - SALE REPORT - LIST BELOW:
            //How to correctly calculate announcement's uptime period, more data?
        }
    }
}
