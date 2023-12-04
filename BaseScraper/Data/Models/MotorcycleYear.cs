using System.ComponentModel.DataAnnotations;

namespace BaseScraper.Data.Models
{
    public class MotorcycleYear
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Year { get; set; }
    }
}
