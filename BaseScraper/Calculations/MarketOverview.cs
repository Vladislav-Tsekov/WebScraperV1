using BaseScraper.Data.Models;
using System.Text;

namespace BaseScraper.Calculations
{
    public class MarketOverview
    {
        public static void MarketShareByEngineDisplacement(HashSet<MotocrossEntry> entriesSet, StreamWriter marketWriter) 
        {
            //double totalEntries = entriesSet.Count;
            //double countOf250s = entriesSet.Where(m => m.Cc == 250).Count();
            //double countOf350s = entriesSet.Where(m => m.Cc == 350).Count();
            //double countOf450s = entriesSet.Where(m => m.Cc == 450).Count();
            //double unknownCcCount = entriesSet.Where(m => m.Cc == 0).Count();
            //double existingCcCount = totalEntries - unknownCcCount;

            //marketWriter.WriteLine($"There are currently {totalEntries} Motocross announcements.");
            //marketWriter.WriteLine($"{countOf250s} out of {totalEntries} are 250 cc.");
            //marketWriter.WriteLine($"{countOf350s} out of {totalEntries} are 350 cc.");
            //marketWriter.WriteLine($"{countOf450s} out of {totalEntries} are 450 cc.");
            //marketWriter.WriteLine($"{unknownCcCount} out of {totalEntries} are neither of the above or do not contain engine displacement information.");
            //marketWriter.WriteLine($"Market Share (CC, %):");
            //marketWriter.WriteLine($"250, {(countOf250s / existingCcCount) * 100:f2}");
            //marketWriter.WriteLine($"350, {(countOf350s / existingCcCount) * 100:f2}");
            //marketWriter.WriteLine($"450, {(countOf450s / existingCcCount) * 100:f2}");

            int totalEntries = entriesSet.Count;
            var ccCounts = entriesSet.GroupBy(m => m.Cc).ToDictionary(g => g.Key, g => g.Count());

            StringBuilder sb = new();
            sb.AppendLine($"There are currently {totalEntries} Motocross announcements.");

            foreach (var ccEntry in ccCounts)
            {
                string cc = ccEntry.Key == 0 ? "unknown or missing" : ccEntry.Key.ToString();
                double percentage = (double)ccEntry.Value / totalEntries * 100;
                sb.AppendLine($"{ccEntry.Value} out of {totalEntries} are {cc} cc. ({percentage:f2}%)");
            }

            marketWriter.Write(sb.ToString());
        }

        public static void MarketShareByMakeAndYear(List<MotocrossMarketPrice> pricesList, StreamWriter marketWriter)
        {
            Dictionary<string, int> makeCountPairs = new();
            SortedDictionary<int, int> yearCountPairs = new();

            foreach (var motorcycle in pricesList)
            {
                if (!makeCountPairs.ContainsKey(motorcycle.Make.Make.ToString()))
                {
                    makeCountPairs.Add(motorcycle.Make.Make, 0);
                }

                if (!yearCountPairs.ContainsKey(motorcycle.Year.Year) && motorcycle.Year.Year != 0)
                {
                    yearCountPairs.Add(motorcycle.Year.Year, 0);
                }
            }

            foreach (var motorcycle in pricesList)
            {

                makeCountPairs[motorcycle.Make.Make.ToString()] += motorcycle.MotoCount;

                yearCountPairs[motorcycle.Year.Year] += motorcycle.MotoCount;
            }

            foreach (var kvp in makeCountPairs)
            {
                marketWriter.WriteLine($"{kvp.Key.ToUpper()}, {kvp.Value}");
            }

            foreach (var kvp in yearCountPairs)
            {
                marketWriter.WriteLine($"{kvp.Key}, {kvp.Value}");
            }
        }
    }
}
