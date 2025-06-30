using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class HobbyRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int UserProfileId { get; set; }
    }
}
