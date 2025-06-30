using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class EducationRequest
    {
        [Required]
        public string Degree { get; set; }

        [Required]
        [StringLength(120)]
        public string Institution { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string StartDate { get; set; }

        public string? EndDate { get; set; }

        [Required]
        public int UserProfileId { get; set; }
    }
}
