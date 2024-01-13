using System.ComponentModel.DataAnnotations;

namespace BaseScraper.Data.Models
{
    public class MotoYear
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Year { get; set; }

        public virtual ICollection<MotocrossMarketPrice> MotocrossMarketPrices { get; set; } = new List<MotocrossMarketPrice>();
        public virtual ICollection<MotocrossEntry> MotocrossEntries { get; set; } = new List<MotocrossEntry>();
        public virtual ICollection<MotocrossSoldEntry> MotocrossSoldEntries { get; set; } = new List<MotocrossSoldEntry>();
    }
}
