using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string userId, string username)
        {
            var section = _config.GetSection("JwtSettings");

            var secretKey = section["SecretKey"] ?? throw new Exception("Missing JWT SecretKey.");
            var issuer = section["Issuer"] ?? "default_issuer";
            var audience = section["Audience"] ?? "default_audience";
            var ExpiryMinutes = int.Parse(section["ExpiryMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(CustomClaimTypes.UserId, userId),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private DateTime? GetExpiryFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token)) return null;

            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo.ToUniversalTime();
        }

        public string? RefreshToken(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var user = context.User;
            var userid = user.FindFirst(CustomClaimTypes.UserId)?.Value;
            var username = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var exp = GetExpiryFromToken(token);

            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token) || exp == null)
                throw new UnauthorizedAccessException("Invalid token");

            return exp <= DateTime.UtcNow.AddMinutes(10) ? GenerateToken(userid, username) : null;
        }


    }

    public static class CustomClaimTypes
    {
        public const string UserId = "id";
    }
}