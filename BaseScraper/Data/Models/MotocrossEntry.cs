using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    [Comment("Table of all Motocross announcements")]
    public class MotocrossEntry
    {
        [Comment("Each announcement link is unique, therefore used as a key")]
        [Key]
        public string Link { get; set; }

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

        [Comment("Motorcycle's actual price")]
        [Required]
        public double Price { get; set; }

        [Comment("Announcement's number of price changes")]
        [Required]
        public int PriceChanges { get; set; } = 0;

        [Comment("The price's value before it was changed")]
        public double OldPrice { get; set; }

        [Comment("Date of announcement's addition to the database")]
        [Required]
        public DateTime DateAdded { get; set; }

        [Comment("Keeps track whether the motorcycle has been sold")]
        [Required]
        public bool IsSold { get; set; }
    }
}
