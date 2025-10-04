using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace TheGrind5_EventManagement.Services
{
    public class SeedService
    {
        private readonly EventDBContext _context;

        public SeedService(EventDBContext context)
        {
            _context = context;
        }

        public async Task SeedAdminUserAsync()
        {
            // Kiểm tra xem đã có admin user chưa
            var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@test.com");
            if (existingAdmin != null)
            {
                return; // Đã có admin rồi, không cần tạo lại
            }

            // Tạo admin user
            var adminUser = new User
            {
                Username = "admin",
                FullName = "Administrator",
                Email = "admin@test.com",
                PasswordHash = HashPassword("admin123"), // Mật khẩu mặc định
                Phone = "0123456789",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            // Sử dụng SHA256 để hash password (trong thực tế nên dùng BCrypt hoặc Argon2)
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
