using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    public class MotocrossEntries
    {
        [Required]
        [ForeignKey(nameof(MotoMake))]
        public int MakeId { get; set; }

        [Required]
        [ForeignKey(nameof(MotoYear))]
        public int YearId { get; set; }

        public string Cc { get; set; }

        [Required]
        public double Price { get; set; }

        [Key]
        public string Link { get; set; }

        public virtual MotoMake Make { get; set; }
        public virtual MotoYear Year { get; set; }
    }
}
