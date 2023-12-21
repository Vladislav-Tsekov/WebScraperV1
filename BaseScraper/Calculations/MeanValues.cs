namespace BaseScraper.Calculations
{
    public class MeanValues
    {
        public const double trimPercentage = 0.20;
        public const double deviationThreshold = 1;

        public static double Dev(IEnumerable<double> prices, double deviationThreshold)
        {
            double[] pricesArray = prices.ToArray();
            double mean = pricesArray.Average();

            double standardDeviation = Math.Sqrt(pricesArray.Select(x => Math.Pow(x - mean, 2)).Average());
            double deviationLimit = deviationThreshold * standardDeviation;

            double[] trimmedData = pricesArray.Where(x => Math.Abs(x - mean) <= deviationLimit).ToArray();
            double trimmedMean = trimmedData.Average();

            return trimmedMean;
        }

        public static double MeanTrim(IEnumerable<double> prices, double trimPercentage)
        {
            double[] pricesArray = prices.ToArray();
            double[] sortedPrices = pricesArray.OrderBy(x => x).ToArray();

            int trimCount = (int)(pricesArray.Length * trimPercentage);

            double[] trimmedData = sortedPrices.Skip(trimCount).Take(pricesArray.Length - 2 * trimCount).ToArray();
            double trimmedMean = trimmedData.Average();

            return trimmedMean;
        }

        //TODO - TEST, POLISH THEN IMPLEMENT - Middle or median value. Case handling - single motorcycle.
        public static double Median(IEnumerable<double> prices)
        {
            double[] pricesArray = prices.ToArray();
            Array.Sort(pricesArray);

            if (pricesArray.Length % 2 == 0)
            {
                int middleIndex1 = pricesArray.Length / 2 - 1;
                int middleIndex2 = pricesArray.Length / 2;
                return (pricesArray[middleIndex1] + pricesArray[middleIndex2]) / 2.0;
            }
            else
            {
                int middleIndex = pricesArray.Length / 2;
                return pricesArray[middleIndex];
            }
        }
    }
}
