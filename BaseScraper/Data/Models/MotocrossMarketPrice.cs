using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    public class MotocrossMarketPrice
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(MotoMake))]
        public int MotoMakeId { get; set; }

        [ForeignKey(nameof(MotoYear))]
        public int MotoYearId { get; set; }

        [Required]
        public double AvgPrice { get; set; }

        [Required]
        public double MeanTrimPrice { get; set; }

        [Required]
        public double StdDevPrice { get; set; }

        [Required]
        public double FinalPrice { get; set; }

        public int MotoCount { get; set; }

        public virtual MotoMake Make { get; set; }

        public virtual MotoYear Year { get; set; }
    }
}
