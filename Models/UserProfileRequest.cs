using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserProfileRequest
    {
        [Required, StringLength(100)] public string Name { get; set; }

        [Required, EmailAddress, StringLength(300)]
        public string Email { get; set; }

        [Required, Phone, StringLength(100)]
        public string Phone { get; set; }

        [Required, StringLength(500)]
        public string Location { get; set; }

        [Required, StringLength(300)]
        public string LinkedIn { get; set; }

        [Required, StringLength(300)]
        public string GitHub { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Route { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public bool PublicPhone { get; set; }

        [Required]
        public bool PublicEmail { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
