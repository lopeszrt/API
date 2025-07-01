using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserProfileRequest
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }

        [Required, EmailAddress, StringLength(300)]
        public required string Email { get; set; }

        [Required, Phone, StringLength(100)]
        public required string Phone { get; set; }

        [Required, StringLength(500)]
        public required string Location { get; set; }

        [Required, StringLength(300)]
        public required string LinkedIn { get; set; }

        [Required, StringLength(300)]
        public required string GitHub { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public required string Route { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public required bool PublicPhone { get; set; }

        [Required]
        public required bool PublicEmail { get; set; }

        [Required]
        public required string Description { get; set; }
    }
}
