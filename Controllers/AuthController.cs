using API.Services;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using API.Models;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly DatabaseCalls _db;
        private readonly JwtService _jwtService;
        private const string UserTable = "User";

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = new Dictionary<string, object>
            {
                { "Username", request.Username }
            };

            var result = await _db.GetFromTableFilteredAsync(UserTable, data);

            if (result.Rows.Count == 0)
            {
                return Unauthorized(new
                {
                    error = "Invalid username or password."
                });
            }

            var user = LoginRequest.CreateFromDataRow(result.Rows[0]);
            var userId = result.Rows[0]["id"].ToString();

            if (!user.checkHashed(request.Password))
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
        public IActionResult TokenLogin()
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

            var data = new Dictionary<string, object>
            {
                { "Username", user.Username }
            };

            var res = await _db.GetFromTableFilteredAsync(UserTable, data);

            if (res.Rows.Count > 0)
            {
                return BadRequest(new
                {
                    error = "Username already Exists"
                });
            }

            data = new Dictionary<string, object>
            {
                { "@Username", user.Username },
                { "@Password", LoginRequest.hashPassword(user.Password) }
            };
            var success = await _db.InsertAsync(UserTable, data);
            if (success == -1)
            {
                return BadRequest(new
                {
                    error = "Failed to register user."
                });
            }

            string token = _jwtService.GenerateToken(success.ToString(), user.Username);

            return Ok(new
            {
                message = "User registered successfully.",
                token = token
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.OldPassword))
            {
                return BadRequest(new { error = "Old and new passwords are required." });
            }
            var userId = User.FindFirst(CustomClaimTypes.UserId)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid token." });
            }
            var data = new Dictionary<string, object>
            {
                { "id", userId }
            };
            var result = await _db.GetFromTableFilteredAsync(UserTable, data);

            if (result.Rows.Count == 0)
            {
                return NotFound(new { error = "User not found." });
            }

            var loginRequest = LoginRequest.CreateFromDataRow(result.Rows[0]);
            if (!loginRequest.checkHashed(request.OldPassword))
            {
                return Unauthorized(new { error = "Old password is incorrect." });
            }

            data = new Dictionary<string, object>
            {
                { "@Password", LoginRequest.hashPassword(request.NewPassword) }
            };

            var success = await _db.UpdateAsync(UserTable, userId,data);

            if (!success)
            {
                return BadRequest(new { error = "Failed to update password." });
            }

            return Ok(new { message = "Password updated successfully." });
        }

        public AuthController(DatabaseCalls db, JwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;
        }
    }
}