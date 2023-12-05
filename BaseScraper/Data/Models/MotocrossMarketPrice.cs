using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    public class MotocrossMarketPrice
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(MakeId))]
        public MotoMake Make { get; set; }
        public int MakeId { get; set; }

        [ForeignKey(nameof(YearId))]
        public MotoYear Year { get; set; }
        public int YearId { get; set; }

        [Required]
        public double AvgPrice { get; set; }

        [Required]
        public double MeanTrimPrice { get; set; }

        [Required]
        public double StdDevPrice { get; set; }

        [Required]
        public double FinalPrice { get; set; }

        public int MotoCount { get; set; }
    }
}
