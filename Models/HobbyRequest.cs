using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class HobbyRequest
    {
        [Required, StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public required int UserProfileId { get; set; }
    }
}
