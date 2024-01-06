using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    public class MotocrossEntry
    {
        [Key]
        public string Link { get; set; }

        [ForeignKey(nameof(MakeId))]
        public MotoMake Make { get; set; }
        public int MakeId { get; set; }

        [ForeignKey(nameof(YearId))]
        public MotoYear Year { get; set; }
        public int YearId { get; set; }

        public string Cc { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public bool IsSold { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        public DateTime DateSold { get; set; }
    }
}
