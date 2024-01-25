using BaseScraper.Data.Models;

namespace BaseScraper.Calculations
{
    public class MarketOverview
    {
        public void MarketShareByEngineDisplacement() 
        {
            List<MotocrossEntry> entriesList = context.MotocrossEntries.ToList();
            HashSet<MotocrossEntry> entriesSet = new(entriesList);

            double totalEntries = entriesSet.Count;
            double countOf250s = entriesSet.Where(m => m.Cc == "250").Count();
            double countOf350s = entriesSet.Where(m => m.Cc == "350").Count();
            double countOf450s = entriesSet.Where(m => m.Cc == "450").Count();
            double unknownCcCount = entriesSet.Where(m => m.Cc is null).Count();
            double existingCcCount = totalEntries - unknownCcCount;

            marketOverview.WriteLine($"There are currently {totalEntries} Motocross announcements.");

            marketOverview.WriteLine($"{countOf250s} out of {totalEntries} are 250 cc.");
            marketOverview.WriteLine($"{countOf350s} out of {totalEntries} are 350 cc.");
            marketOverview.WriteLine($"{countOf450s} out of {totalEntries} are 450 cc.");
            marketOverview.WriteLine($"{unknownCcCount} out of {totalEntries} are neither of the above or do not contain engine displacement information.");
            marketOverview.WriteLine($"Market Share (CC, %):");
            marketOverview.WriteLine($"250, {(countOf250s / existingCcCount) * 100:f2}");
            marketOverview.WriteLine($"350, {(countOf350s / existingCcCount) * 100:f2}");
            marketOverview.WriteLine($"450, {(countOf450s / existingCcCount) * 100:f2}");
        }

        public void MarketShareByMakeAndYear()
        {
            List<MotocrossMarketPrice> pricesList = context.MotocrossMarketPrices.ToList();

            Dictionary<string, int> makeCountPairs = new();
            SortedDictionary<int, int> yearCountPairs = new();

            foreach (var motorcycle in pricesList)
            {
                if (!makeCountPairs.ContainsKey(motorcycle.Make.Make.ToString()))
                {
                    makeCountPairs.Add(motorcycle.Make.Make, 0);
                }

                if (!yearCountPairs.ContainsKey(motorcycle.Year.Year))
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
                marketOverview.WriteLine($"{kvp.Key.ToUpper()}, {kvp.Value}");
            }

            foreach (var kvp in yearCountPairs)
            {
                marketOverview.WriteLine($"{kvp.Key}, {kvp.Value}");
            }
        }
    }
}
