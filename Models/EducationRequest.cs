using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class EducationRequest
    {
        [Required]
        public required string Degree { get; set; }

        [Required]
        [StringLength(120)]
        public required string Institution { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string StartDate { get; set; }

        public string? EndDate { get; set; }

        [Required]
        public required int UserProfileId { get; set; }
    }
}
