using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class JobExperienceRequest
    {
        [Required, StringLength(120)]
        public required string Title { get; set; }
        [Required, StringLength(120)]
        public required string Company { get; set; }
        [Required]
        public required string Description { get; set; }

        [Required]
        public required int UserProfileId { get; set; }

        [Required]
        public required string StartDate { get; set; }

        public string? EndDate { get; set; }
    }
}
