using API.Services;
using API.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using API.Models;
using API.Structure;

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

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = new Dictionary<string, object>
            {
                { "Username", request.Username }
            };

            var result = await _db.GetFromTableFilteredAsync(TableName.User, data);

            if (result.Rows.Count == 0)
            {
                return Unauthorized(new
                {
                    error = "Invalid username or password."
                });
            }

            var user = LoginRequest.CreateFromDataRow(result.Rows[0]);
            var userId = result.Rows[0]["id"].ToString();

            var udata = new Dictionary<string, object>
            {
                { "UserId", userId }
            };

            var profileRes = await _db.GetFromTableFilteredAsync(TableName.UserProfile, udata);
            var profile = profileRes.Rows.Count > 0 ? UserProfile.CreateFromDataRow(profileRes.Rows[0]) : null;

            if (profile == null || !user.CheckHashed(request.Password))
            {
                return Unauthorized(new
                {
                    error = "Invalid username or password."
                });
            }

            var tokenString = _jwtService.GenerateToken(userId, user.Username, user.Role, profile.Id.ToString());

            return Ok(new { token = tokenString, userId });
        }

        [Authorize(Roles = "Admin, User")]
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

            var res = await _db.GetFromTableFilteredAsync(TableName.User, data);

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
                { "@Password", LoginRequest.HashPassword(user.Password) }
            };
            var success = await _db.InsertAsync(TableName.User, data);
            if (success == -1)
            {
                return BadRequest(new
                {
                    error = "Failed to register user."
                });
            }

            var udata = new Dictionary<string, object>
            {
                { "UserId", success.ToString() },
                { "Name", user.Username },
                { "Description", "" },
                { "Email", ""},
                { "Phone", "" },
                { "Location", "" },
                { "LinkedIn", "" }, 
                { "GitHub", "" },
                { "Route", Guid.NewGuid().ToString() }
            };

            var profileSuccess = await _db.InsertAsync(TableName.UserProfile, udata);
            if (profileSuccess == -1)
            {
                return StatusCode(503,new
                {
                    error = "Failed to create user profile."
                });
            }

            string token = _jwtService.GenerateToken(success.ToString(), user.Username, "User", profileSuccess.ToString());

            return Ok(new
            {
                message = "User registered successfully.",
                token
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
            var result = await _db.GetFromTableFilteredAsync(TableName.User, data);

            if (result.Rows.Count == 0)
            {
                return NotFound(new { error = "User not found." });
            }

            var loginRequest = LoginRequest.CreateFromDataRow(result.Rows[0]);
            if (!loginRequest.CheckHashed(request.OldPassword))
            {
                return Unauthorized(new { error = "Old password is incorrect." });
            }

            data = new Dictionary<string, object>
            {
                { "@Password", LoginRequest.HashPassword(request.NewPassword) }
            };

            var success = await _db.UpdateAsync(TableName.User, userId,data);

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