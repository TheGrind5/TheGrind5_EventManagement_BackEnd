using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace TheGrind5_EventManagement.Services
{
    public class AuthService
    {
        private readonly EventDBContext _context;

        public AuthService(EventDBContext context)
        {
            _context = context;
        }

        public async Task<AuthDTOs.LoginResponse?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null)
                return null;

            // Kiểm tra password
            var hashedPassword = HashPassword(password);
            if (user.PasswordHash != hashedPassword)
                return null;

            // Tạo token (tạm thời dùng simple token, sau này sẽ dùng JWT)
            var token = GenerateSimpleToken(user.UserId, user.Email);

            var userDto = new AuthDTOs.UserReadDto(
                user.UserId,
                user.FullName,
                user.Email,
                user.Phone,
                user.Role
            );

            return new AuthDTOs.LoginResponse(
                token,
                DateTime.UtcNow.AddDays(7), // Token expires in 7 days
                userDto
            );
        }

        public async Task<AuthDTOs.UserReadDto> RegisterAsync(DTOs.RegisterRequest request)
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
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateSimpleToken(int userId, string email)
        {
            // Tạm thời tạo simple token, sau này sẽ dùng JWT
            var tokenData = $"{userId}:{email}:{DateTime.UtcNow.Ticks}";
            var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
