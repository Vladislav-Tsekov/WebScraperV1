using System.ComponentModel.DataAnnotations;

namespace BaseScraper.Data.Models
{
    public class MotorcycleMake
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Make { get; set; }
    }
}
