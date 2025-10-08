using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly EventDBContext _context;

        public AuthController(AuthService authService, EventDBContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTOs.LoginRequest request)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Login attempt - Email: '{request?.Email}', Password: '{request?.Password}'");
                
                // Check if request is null
                if (request == null)
                {
                    Console.WriteLine("Request is null");
                    return BadRequest(new { message = "Request body không hợp lệ" });
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    Console.WriteLine($"Validation failed - Email: '{request.Email}', Password: '{request.Password}'");
                    return BadRequest(new { message = "Email và mật khẩu không được để trống" });
                }

                // Basic email validation
                if (!request.Email.Contains("@") || !request.Email.Contains("."))
                {
                    Console.WriteLine($"Email validation failed: '{request.Email}'");
                    return BadRequest(new { message = "Email không hợp lệ" });
                }

                var result = await _authService.LoginAsync(request.Email, request.Password);
                
                if (result == null)
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
                }

                // Trả về format phù hợp với frontend
                return Ok(new {
                    user = new {
                        userId = result.User.UserId,
                        fullName = result.User.FullName,
                        email = result.User.Email,
                        phone = result.User.Phone,
                        role = result.User.Role
                    },
                    AccessToken = result.AccessToken,
                    ExpiresAt = result.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                // Log the exception (in production, use proper logging)
                Console.WriteLine($"Login error: {ex.Message}");
                return BadRequest(new { message = "Có lỗi xảy ra khi đăng nhập", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Email này đã được sử dụng" });
                }

                var result = await _authService.RegisterAsync(request);
                
                return Ok(new { 
                    message = "Đăng ký thành công", 
                    userId = result.UserId,
                    fullName = result.FullName,
                    email = result.Email,
                    phone = result.Phone,
                    role = result.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi đăng ký", error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Lấy user ID từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy user" });
                }

                var userDto = new
                {
                    userId = user.UserId,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    role = user.Role
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy user" });
                }

                var userDto = new
                {
                    userId = user.UserId,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    role = user.Role
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpPost("seed-admin")]
        public async Task<IActionResult> SeedAdmin()
        {
            try
            {
                // Kiểm tra xem admin đã tồn tại chưa
                var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
                if (existingAdmin != null)
                {
                    return Ok(new { 
                        message = "Admin user already exists", 
                        email = existingAdmin.Email,
                        password = "admin123"
                    });
                }

                // Tạo admin user
                var adminUser = new User
                {
                    Username = "admin",
                    FullName = "Administrator",
                    Email = "admin@test.com",
                    PasswordHash = HashPassword("admin123"),
                    Phone = "0123456789",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Admin user created successfully", 
                    email = adminUser.Email,
                    password = "admin123"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo admin user", error = ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
