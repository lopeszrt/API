using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserRequest
    {
        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, StringLength(512)]
        public string Password { get; set; }
    }
}
