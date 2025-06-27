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
    }

    public static class CustomClaimTypes
    {
        public const string UserId = "id";
    }
}