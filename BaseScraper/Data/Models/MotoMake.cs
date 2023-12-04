using System.ComponentModel.DataAnnotations;

namespace BaseScraper.Data.Models
{
    public class MotoMake
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Make { get; set; }
    }
}
