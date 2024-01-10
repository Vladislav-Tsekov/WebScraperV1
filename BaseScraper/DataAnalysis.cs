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
            output.AppendLine(StringsConstants.ListingsByMake);

            foreach (var kvp in makeCountPairs)
            {
                output.AppendLine($"{kvp.Key.ToUpper()} -> {kvp.Value}");
            }

            //TODO - OUTPUT MUST BE EXPORTED TO A CSV FILE
            //TODO - MUST ALSO ADD DATE IN ORDER TO KNOW WHEN WAS THE REPORT GENERATED

            Console.WriteLine(output.ToString().TrimEnd());
        }

        public async Task TotalMotorcyclesCountByYear(MotoContext context) 
        {
            List<MotocrossMarketPrice> pricesList = context.MotocrossMarketPrices.ToList();

            Dictionary<int, int> yearCountPairs = new();

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
            output.AppendLine(StringsConstants.ListingsByYear);

            foreach (var kvp in yearCountPairs)
            {
                output.AppendLine($"{kvp.Key} -> {kvp.Value}");
            }

            //TODO - OUTPUT MUST BE EXPORTED TO A CSV FILE
            //TODO - MUST ALSO ADD DATE IN ORDER TO KNOW WHEN WAS THE REPORT GENERATED

            Console.WriteLine(output.ToString().TrimEnd());
        }

        public async Task MotorcyclesWithUnusualVariance(MotoContext context) 
        {
            //TODO - TAKE ALL MOTORCYCLES WITH VARIANCE IN BOTH EXTREMES, CHECK WHETHER THE DATA CALCULATIONS ARE CORRECT
            throw new NotImplementedException();
        }

        public async Task MotorcyclesWithUnusualPriceRange(MotoContext context)
        {
            //TODO - TAKE ALL MOTORCYCLES WITH SUSPICIOUS RANGE VALUES, CHECK WHETHER THE DATA CALCULATIONS ARE CORRECT
            throw new NotImplementedException();
        }

        public async Task SoldMotorcycles(MotoContext context)
        {
            //TODO - LIST, USED TO ANALYZE SOLD MOTORCYCLES AND THE DATA INTEGRITY
            throw new NotImplementedException();
        }
    }
}
