namespace BaseScraper.Calculations
{
    public class MeanValues
    {
        public const double trimPercentage = 0.20;
        public const double deviationThreshold = 1;

        public static double Dev(IEnumerable<double> data, double deviationThreshold)
        {
            double[] dataArray = data.ToArray();
            double mean = dataArray.Average();

            double standardDeviation = Math.Sqrt(dataArray.Select(x => Math.Pow(x - mean, 2)).Average());
            double deviationLimit = deviationThreshold * standardDeviation;

            double[] trimmedData = dataArray.Where(x => Math.Abs(x - mean) <= deviationLimit).ToArray();
            double trimmedMean = trimmedData.Average();

            return trimmedMean;
        }

        public static double Mean(IEnumerable<double> prices, double trimPercentage)
        {
            double[] pricesArray = prices.ToArray();
            double[] sortedPrices = pricesArray.OrderBy(x => x).ToArray();

            int trimCount = (int)(pricesArray.Length * trimPercentage);

            double[] trimmedData = sortedPrices.Skip(trimCount).Take(pricesArray.Length - 2 * trimCount).ToArray();
            double trimmedMean = trimmedData.Average();

            return trimmedMean;
        }
    }
}
