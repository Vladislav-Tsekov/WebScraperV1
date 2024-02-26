using BaseScraper.Data.Models;

namespace BaseScraper.Calculations
{
    public  class SaleReport
    {
        public void SoldMotorcyclesList(HashSet<MotocrossSoldEntry> soldEntriesSet, HashSet<MotocrossMarketPrice> marketPricesSet, StreamWriter saleReportWriter)
        {
            foreach (var entry in soldEntriesSet)
            {
                var currentPrice = marketPricesSet
                    .FirstOrDefault(m => m.Make.Make == entry.Make.Make && m.Year.Year == entry.Year.Year);

                if (currentPrice != null)
                {
                    saleReportWriter.WriteLine($"{entry.Make.Make}, {entry.Year.Year}, {entry.Cc}, {entry.Price}, {currentPrice.AvgPrice:f0}, {entry.DateAdded:d}, {entry.DateSold:d}");
                }
            }
        }

        public void CalculateAbsoluteAverages(HashSet<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
        {
            var averagePrice = soldEntriesSet.Average(m => m.Price);
            var averageYear = soldEntriesSet.Average(m => m.Year.Year);

            saleReportWriter.WriteLine($"The average price for all sold entries is: {averagePrice:f2}");
            saleReportWriter.WriteLine($"The average year for all sold entries is: {Math.Round(averageYear)}");
        }

        public void EngineDisplacementCount(HashSet<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
        {
            double countOf250 = soldEntriesSet.Count(m => m.Cc == 250);
            double countOf350 = soldEntriesSet.Count(m => m.Cc == 350);
            double countOf450 = soldEntriesSet.Count(m => m.Cc == 450);

            double totalCount = soldEntriesSet.Count(m => m.Cc == 250 || m.Cc == 350 || m.Cc == 450);

            saleReportWriter.WriteLine($"250cc: {countOf250} out of {totalCount}. Ratio of {(countOf250 / totalCount) * 100:f2}%");
            saleReportWriter.WriteLine($"350cc: {countOf350} out of {totalCount}. Ratio of {(countOf350 / totalCount) * 100:f2}%");
            saleReportWriter.WriteLine($"450cc: {countOf450} out of {totalCount}. Ratio of {(countOf450 / totalCount) * 100:f2}%");
        }

        public void CountOfSalesPerMake(HashSet<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter) 
        {
            Dictionary<string, int> makeCountPairs = new Dictionary<string, int>();

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

            foreach (var kvp in makeCountPairs)
            {
                saleReportWriter.WriteLine($"A total of {kvp.Value} {kvp.Key} sold!");
            }
        }

        public void CountOfSalesPerDay(HashSet<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
        {
            SortedDictionary<DateTime, int> salesPerDay = new();

            foreach (var entry in soldEntriesSet)
            {
                if (!salesPerDay.ContainsKey(entry.DateSold.Date))
                {
                    salesPerDay.Add(entry.DateSold.Date, 0);
                }

                salesPerDay[entry.DateSold.Date] += 1;
            }

            foreach (var kvp in salesPerDay)
            {
                saleReportWriter.WriteLine($"On {kvp.Key:d}, {kvp.Value} motorcycles were sold.");
            }

            double totalSales = soldEntriesSet.Count;
            double totalDays = salesPerDay.Count;
            double averageSalesPerDay = totalSales / totalDays;

            saleReportWriter.WriteLine($"The average motorcycles sold per day is: {averageSalesPerDay:f1}");
        }
    }
}
