using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Services
{
    public class AuthService
    {
        private readonly EventDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(EventDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthDTOs.LoginResponse?> LoginAsync(string email, string password)
        {
            try
            {
                // Normalize email to lowercase for consistent comparison
                var normalizedEmail = email.ToLowerInvariant().Trim();
                
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
                
                if (user == null)
                {
                    Console.WriteLine($"Login failed: User not found for email {email}");
                    return null;
                }

                // Kiểm tra password
                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    Console.WriteLine($"Login failed: Invalid password for email {email}");
                    return null;
                }

                // Tạo JWT token
                var token = GenerateJwtToken(user);

                var userDto = new AuthDTOs.UserReadDto(
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.Role
                );

                Console.WriteLine($"Login successful for user {user.Email}");
                return new AuthDTOs.LoginResponse(
                    token,
                    DateTime.UtcNow.AddDays(7), // Token expires in 7 days
                    userDto
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error for email {email}: {ex.Message}");
                throw;
            }
        }

        public async Task<AuthDTOs.UserReadDto> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    Phone = request.Phone ?? "",
                    Role = "Customer", // Default role
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new AuthDTOs.UserReadDto(
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.Role
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RegisterAsync: {ex.Message}", ex);
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "TheGrind5_EventManagement";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "TheGrind5_EventManagement_Users";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
