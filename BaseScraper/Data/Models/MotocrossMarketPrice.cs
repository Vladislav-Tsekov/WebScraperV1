using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    public class MotocrossMarketPrice
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(MotorcycleMake))]
        public int MotorcycleMakeId { get; set; }

        [ForeignKey(nameof(MotorcycleYear))]
        public int MotorcycleYearId { get; set; }

        [Required]
        public double AvgPrice { get; set; }

        [Required]
        public double MeanTrimPrice { get; set; }

        [Required]
        public double StdDevPrice { get; set; }

        [Required]
        public double FinalPrice { get; set; }

        public int MotorcyclesCount { get; set; }

        public virtual MotorcycleMake Make { get; set; }

        public virtual MotorcycleYear Year { get; set; }
    }
}
