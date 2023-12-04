using System.ComponentModel.DataAnnotations;

namespace BaseScraper.Data.Models
{
    public class MarketPrice
    {

        //Make, Year, Average Price, Mean Price, StdDev Price, Combined Price, Count

        [Key]
        public int Id { get; set; }

        public string Make { get; set; }

        public int Year { get; set; }

        public double AvgPrice { get; set; }

        public double MeanPrice { get; set; }

        public double StdDevPrice { get; set; }

        public double CombinedPrice { get; set; }
        public int MotorcyclesCount { get; set; }
    }
}
