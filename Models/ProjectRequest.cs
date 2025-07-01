using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ProjectRequest
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        public string? Link { get; set; }

        [Required]
        public required int UserProfileId { get; set; }

        public string? ImageUrl { get; set; }
    }
}
