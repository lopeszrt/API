using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SkillRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int UserProfileId { get; set; }
    }
}
