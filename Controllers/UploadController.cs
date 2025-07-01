using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UploadController : Controller
    {
        private readonly string _profilePath;
        private readonly string _projectPath;
        private readonly string _publicBaseUrl;
        private readonly DatabaseCalls _db;

        public UploadController(DatabaseCalls db, IConfiguration configuration)
        {
            _db = db;

            var uploadPath = configuration["UPLOAD_PATH"];
            _publicBaseUrl = configuration["PUBLIC_BASE_URL"] ?? "";

            if (string.IsNullOrWhiteSpace(uploadPath) || string.IsNullOrWhiteSpace(_publicBaseUrl))
                throw new InvalidOperationException("UPLOAD_PATH or PUBLIC_BASE_URL is not configured.");

            // Set final storage paths
            _profilePath = Path.Combine(uploadPath, "profile");
            _projectPath = Path.Combine(uploadPath, "project");

            // Ensure directories exist
            Directory.CreateDirectory(_profilePath);
            Directory.CreateDirectory(_projectPath);
        }

        [HttpPost("profile/{profileId}")]
        public async Task<IActionResult> UploadProfileImage(int profileId, [FromForm] ImageUploadRequest image)
        {
            if (image?.Image == null || image.Image.Length == 0)
                return BadRequest(new { error = "Image file is required." });

            if (profileId <= 0 || !ModelState.IsValid)
                return BadRequest(new { error = "Invalid profile ID or model state.", ModelState });

            var (success, url) = await UploadImageAndUpdateAsync(
                image.Image,
                _profilePath,
                "User_Profile",
                profileId,
                imageUrl => new Dictionary<string, object>
                {
                    { "@ImageUrl", imageUrl }
                },
                _db.UpdateAsync
            );

            if (!success)
                return NotFound(new { error = $"User Profile with ID {profileId} not found." });

            return Ok(new { message = "Profile image uploaded successfully.", data = new { ImageUrl = url } });
        }

        [HttpPost("project/{projectId}")]
        public async Task<IActionResult> UploadProjectImage(int projectId, [FromForm] ImageUploadRequest image)
        {
            if (image?.Image == null || image.Image.Length == 0)
                return BadRequest(new { error = "Image file is required." });
            if (projectId <= 0 || !ModelState.IsValid)
                return BadRequest(new { error = "Invalid project ID or model state.", ModelState });
            var (success, url) = await UploadImageAndUpdateAsync(
                image.Image,
                _projectPath,
                "Project",
                projectId,
                imageUrl => new Dictionary<string, object>
                {
                    { "@ImageUrl", imageUrl }
                },
                _db.UpdateAsync
            );
            if (!success)
                return NotFound(new { error = $"Project with ID {projectId} not found." });
            return Ok(new { message = "Project image uploaded successfully.", data = new { ImageUrl = url } });
        }

        // Helper to save file to disk
        private async Task<string?> SaveFileAsync(IFormFile file, string directoryPath)
        {
            if (file == null || file.Length == 0)
                return null;

            var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(directoryPath, filename);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filename;
        }

        // Helper to save image and update database
        private async Task<(bool success, string? url)> UploadImageAndUpdateAsync(
            IFormFile file,
            string directoryPath,
            string tableName,
            int id,
            Func<string, Dictionary<string, object>> createDataDict,
            Func<string, string, Dictionary<string, object>, Task<bool>> dbUpdateFunc)
        {
            var filename = await SaveFileAsync(file, directoryPath);
            if (filename == null)
                return (false, null);

            // Build public URL
            var relativePath = $"/images/{Path.GetFileName(directoryPath)}/{filename}";
            var fullUrl = $"{_publicBaseUrl}{relativePath}";

            var data = createDataDict(fullUrl);
            var success = await dbUpdateFunc(tableName, id.ToString(), data);
            return (success, fullUrl);
        }
    }
}
