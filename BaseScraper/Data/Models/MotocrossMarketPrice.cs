﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseScraper.Data.Models
{
    public class MotocrossMarketPrice
    {
        [ForeignKey(nameof(MakeId))]
        public MotoMake Make { get; set; }
        public int MakeId { get; set; }

        [ForeignKey(nameof(YearId))]
        public MotoYear Year { get; set; }
        public int YearId { get; set; }

        public int MotoCount { get; set; }

        [Required]
        public decimal AvgPrice { get; set; }

        [Required]
        public decimal MeanTrimPrice { get; set; }

        [Required]
        public decimal StdDevPrice { get; set; }

        [Required]
        public decimal MedianPrice { get; set; }

        [Required]
        public decimal ModePrice { get; set; }

        [Required]
        public decimal PriceRange { get; set; }

        [Required]
        public decimal PriceVariance { get; set; }
    }
}
