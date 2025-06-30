using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class LanguageRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(100)]
        public string Proficiency { get; set; }
        [Required]
        public int UserProfileId { get; set; }
    }
}
