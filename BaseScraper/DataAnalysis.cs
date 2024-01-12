using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using System.Text;

namespace BaseScraper
{
    public class DataAnalysis
    {
        //TODO - MUST FIND A WAY TO INDENTIFY TRENDS

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

            StringBuilder output = new();
            StreamWriter makeCountWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "CountByMake.csv"));

            output.AppendLine(StringsConstants.ListingsByMake);
            makeCountWriter.WriteLine(DateTime.Now);

            foreach (var kvp in makeCountPairs)
            {
                output.AppendLine($"{kvp.Key.ToUpper()} -> {kvp.Value}");
                makeCountWriter.WriteLine($"{kvp.Key.ToUpper()}, {kvp.Value}");
            }

            makeCountWriter.Dispose();

            Console.WriteLine(output.ToString().TrimEnd());
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

            StringBuilder output = new();
            StreamWriter yearCountWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "CountByYear.csv"));
            
            output.AppendLine(StringsConstants.ListingsByYear);
            yearCountWriter.WriteLine(DateTime.Now);

            foreach (var kvp in yearCountPairs)
            {
                output.AppendLine($"{kvp.Key} -> {kvp.Value}");
                yearCountWriter.WriteLine($"{kvp.Key}, {kvp.Value}");
            }

            yearCountWriter.Dispose();

            Console.WriteLine(output.ToString().TrimEnd());
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

        public async Task SoldMotorcycles(MotoContext context)
        {
            throw new NotImplementedException();
        }
    }
}
