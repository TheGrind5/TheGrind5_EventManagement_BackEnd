using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            return GenerateToken(user, DateTime.UtcNow.AddDays(7));
        }

        public string GenerateToken(User user, DateTime expiresAt)
        {
            var jwtConfig = GetJwtConfiguration();
            var credentials = CreateSigningCredentials(jwtConfig.Key);
            var claims = CreateUserClaims(user);
            
            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private (string Key, string Issuer, string Audience) GetJwtConfiguration()
        {
            return (
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"),
                _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured"),
                _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured")
            );
        }

        private SigningCredentials CreateSigningCredentials(string key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        private Claim[] CreateUserClaims(User user)
        {
            return new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };
        }
    }
}

