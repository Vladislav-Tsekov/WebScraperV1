using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BaseScraper.Data.Models
{
    public class MotocrossSoldEntry
    {
        [Key]
        public int Id { get; set; }

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
        public DateTime DateAdded { get; set; }

        [Required]
        public DateTime DateSold { get; set; }
    }
}
