using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserRequest
    {
        [Required, StringLength(100)]
        public required string Username { get; set; }

        [Required, StringLength(512)]
        public required string Password { get; set; }
    }
}
