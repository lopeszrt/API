using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class JobExperienceRequest
    {
        [Required, StringLength(120)]
        public string Title { get; set; }
        [Required, StringLength(120)]
        public string Company { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public int UserProfileId { get; set; }

        [Required]
        public string StartDate { get; set; }

        public string? EndDate { get; set; }
    }
}
