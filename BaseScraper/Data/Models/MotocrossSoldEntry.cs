using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BaseScraper.Data.Models
{
    [Comment("A table of all sold motorcycles")]
    public class MotocrossSoldEntry
    {
        [Key]
        public int Id { get; set; }

        [Comment("Motorcycle's Make - Manufacturer")]
        [ForeignKey(nameof(MakeId))]
        public MotoMake Make { get; set; }
        public int MakeId { get; set; }

        [Comment("Motorcycle's Year of manufacture")]
        [ForeignKey(nameof(YearId))]
        public MotoYear Year { get; set; }
        public int YearId { get; set; }

        [Comment("Motorcycle's engine displacement")]
        public string Cc { get; set; }

        [Comment("Motorcycle's price")]
        [Required]
        public double Price { get; set; }

        [Comment("Date of announcement's addition to the database")]
        [Required]
        public DateTime DateAdded { get; set; }

        [Comment("Date of announcement's removal from the website")]
        [Required]
        public DateTime DateSold { get; set; }
    }
}
