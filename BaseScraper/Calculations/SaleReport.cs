using BaseScraper.Data.Models;
using System.Text;

namespace BaseScraper.Calculations
{
    public  class SaleReport
    {
        public static void SoldMotorcyclesList(List<MotocrossSoldEntry> soldEntriesSet, List<MotocrossMarketPrice> marketPricesSet, StreamWriter saleReportWriter)
        {
            foreach (var entry in soldEntriesSet)
            {
                if (entry.Make?.Make != null && entry.Year?.Year != 0)
                {
                    var currentPrice = marketPricesSet.FirstOrDefault(m => m.Make?.Make == entry.Make.Make && m.Year?.Year == entry.Year.Year);

                    if (currentPrice != null)
                    {
                        saleReportWriter.WriteLine($"{entry.Make.Make}, {entry.Year.Year}, {entry.Cc}, {entry.Price}, {currentPrice.AvgPrice:f0}, {entry.DateAdded:d}, {entry.DateSold:d}");
                    }
                }
            }
        }

        public static void CalculateAbsoluteAverages(List<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
        {
            var averagePrice = soldEntriesSet.Average(m => m.Price);
            var averageYear = soldEntriesSet.Average(m => m.Year.Year);

            saleReportWriter.WriteLine($"The average price for all sold entries is: {averagePrice:f2}");
            saleReportWriter.WriteLine($"The average year for all sold entries is: {Math.Round(averageYear)}");
        }

        public static void EngineDisplacementCount(List<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
        {
            int[] engineDisplacements = { 250, 350, 450 };

            double totalCount = soldEntriesSet.Count(m => engineDisplacements.Contains(m.Cc));

            StringBuilder sb = new();

            foreach (int cc in engineDisplacements)
            {
                double count = soldEntriesSet.Count(m => m.Cc == cc);
                double ratio = (count / totalCount) * 100.0;

                sb.AppendLine($"{cc}cc: {count} out of {totalCount}. Ratio of {ratio:f2}%");
            }

            saleReportWriter.Write(sb.ToString());
        }

        public static void CountOfSalesPerMake(List<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
        {
            var makeCountPairs = new Dictionary<string, int>();

            foreach (var motorcycle in soldEntriesSet)
            {
                var make = motorcycle.Make?.Make;
                if (make != null)
                {
                    if (!makeCountPairs.ContainsKey(make))
                    {
                        makeCountPairs.Add(make, 0);
                    }
                }
            }

            foreach (var motorcycle in soldEntriesSet)
            {
                var make = motorcycle.Make?.Make;
                if (make != null)
                {
                    makeCountPairs[make]++;
                }
            }

            foreach (var kvp in makeCountPairs)
            {
                saleReportWriter.WriteLine($"A total of {kvp.Value} {kvp.Key} sold!");
            }
        }

        public static void CountOfSalesPerDay(List<MotocrossSoldEntry> soldEntriesSet, StreamWriter saleReportWriter)
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
