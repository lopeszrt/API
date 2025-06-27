using API.Services;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly Database _db;
        private readonly JwtService _jwtService;

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new
                {
                    error = "Password and Username are required"
                });
            }

            var query = "SELECT id, Password FROM User WHERE Username = @Username";

            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username }
            };

            var result = await _db.ExecuteQueryAsync(query, parameters);

            if (result == null || result.Rows.Count == 0)
            {
                return Unauthorized(new
                {
                    error = "Invalid username or password."
                });
            }

            var userId = result.Rows[0]["id"].ToString();
            var hashedPassword = result.Rows[0]["Password"].ToString() ?? "";

            if (!user.checkHashed(hashedPassword))
            {
                return Unauthorized(new
                {
                    error = "Invalid username or password."
                });
            }

            var tokenString = _jwtService.GenerateToken(userId, user.Username);

            return Ok(new { token = tokenString, userId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TokenLogin()
        {
            var userId = User.FindFirst(CustomClaimTypes.UserId)?.Value;
            var username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { error = "Invalid token." });
            }

            return Ok(new { userId });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new
                {
                    error = "Password and Username are required"
                });
            }

            var checkQuery = "SELECT COUNT(*) FROM User WHERE Username = @Username";
            var checkParameters = new Dictionary<string, object>
            {
                { "@Username", user.Username }
            };

            var count = await _db.ExecuteScalarAsync(checkQuery, checkParameters);

            if (Convert.ToInt32(count) > 0)
            {
                return BadRequest(new
                {
                    error = "Username already Exists"
                });
            }

            var query = "INSERT INTO User (Username, Password) VALUES (@Username, @Password)";
            var parameters = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", LoginRequest.hashPassword(user.Password) }
            };
            var success = await _db.ExecuteInsertAsync(query, parameters);
            if (success == 0)
            {
                return BadRequest(new { 
                    error= "Failed to register user."
                });
            }

            string token = _jwtService.GenerateToken(success.ToString(), user.Username);

            return Ok(new
            {
                message = "User registered successfully.",
                token = token
            });
        }

        public AuthController(Database db, JwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }
    }
}