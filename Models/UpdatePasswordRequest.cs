using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UpdatePasswordRequest
    {
        [Required, StringLength(512)]
        public string OldPassword { get; set; }
        [Required, StringLength(512)]
        public string NewPassword { get; set; }
    }
}
