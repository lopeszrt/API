using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ProjectRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string? Link { get; set; }

        [Required]
        public int UserProfileId { get; set; }

        public string? ImageUrl { get; set; }
    }
}
