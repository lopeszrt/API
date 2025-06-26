using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly Database _db;
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var query = "SELECT id FROM User WHERE Username = @Username AND Password = @Password";

            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", user.Password }
            };

            var result = await _db.ExecuteScalarAsync(query, parameters);

            if (result == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(new { UserId = result });

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Username and password are required.");
            }
            var query = "INSERT INTO User (Username, Password) VALUES (@Username, @Password)";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", user.Password }
            };
            var success = await _db.ExecuteNonQueryAsync(query, parameters);
            if (!success)
            {
                return BadRequest("Failed to register user.");
            }
            return Ok("User registered successfully.");
        }

        public AuthController(Database db)
        {
            _db = db;
        }
    }
}
