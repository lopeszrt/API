using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SkillRequest
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public required int UserProfileId { get; set; }
    }
}
