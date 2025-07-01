using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class LanguageRequest
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }
        [Required, StringLength(100)]
        public required string Proficiency { get; set; }
        [Required]
        public required int UserProfileId { get; set; }
    }
}
