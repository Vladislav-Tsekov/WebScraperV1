using BaseScraper.Data.Models;
using System.Text;

namespace BaseScraper.Calculations
{
    public class MarketOverview
    {
        public static void MarketShareByEngineDisplacement(HashSet<MotocrossEntry> entriesSet, StreamWriter marketWriter) 
        {
            int totalEntries = entriesSet.Where(m => m.Cc == 250 || m.Cc == 350 || m.Cc == 450).Count();
            Dictionary<int,int> ccCounts = entriesSet
                            .Where(m => m.Cc == 250 || m.Cc == 350 || m.Cc == 450)
                            .GroupBy(m => m.Cc)
                            .OrderByDescending(m => m.Key)
                            .ToDictionary(g => g.Key, g => g.Count());

            StringBuilder sb = new();
            sb.AppendLine($"There are currently {totalEntries} Motocross announcements.");

            foreach (var ccEntry in ccCounts)
            {
                string cc = ccEntry.Key.ToString();
                double percentage = (double)ccEntry.Value / totalEntries * 100;
                sb.AppendLine($"{ccEntry.Value} out of {totalEntries} are {cc} cc. ({percentage:f2}%)");
            }

            marketWriter.Write(sb.ToString());
        }

        public static void MarketShareByMakeAndYear(List<MotocrossMarketPrice> pricesList, StreamWriter marketWriter)
        {
            SortedDictionary<string, int> makeCountPairs = new();
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

            StringBuilder sb = new();

            foreach (var kvp in makeCountPairs)
            {
                sb.AppendLine($"{kvp.Key.ToUpper()}, {kvp.Value}");
            }

            foreach (var kvp in yearCountPairs)
            {
                sb.AppendLine($"{kvp.Key}, {kvp.Value}");
            }

            marketWriter.Write(sb.ToString());
        }
    }
}
