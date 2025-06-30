using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using API.Services;

namespace API.Models
{
    public class ImageUploadRequest
    {
        [Required(ErrorMessage = "Image file is required.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)] // 5MB limit (custom attribute)
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })] // Custom attribute
        public IFormFile Image { get; set; }
    }
}
