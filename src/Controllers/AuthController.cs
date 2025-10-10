using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTOs.LoginRequest request)
        {
            if (!IsValidLoginRequest(request))
                return BadRequest(new { message = "Email và mật khẩu không hợp lệ" });

            var result = await _authService.LoginAsync(request.Email!, request.Password!);
            
            if (result == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });

            return Ok(CreateLoginResponse(result));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (await _userRepository.IsEmailExistsAsync(request.Email))
                    return BadRequest(new { message = "Email này đã được sử dụng" });

                var result = await _authService.RegisterAsync(request);
                return Ok(CreateRegisterResponse(result));
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
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy user" });

                return Ok(CreateUserDto(user));
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
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy user" });

                return Ok(CreateUserDto(user));
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
                var existingAdmin = await _userRepository.GetUserByEmailAsync("admin@test.com");
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

                await _userRepository.CreateUserAsync(adminUser);

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

        private bool IsValidLoginRequest(AuthDTOs.LoginRequest request)
        {
            return request != null && 
                   !string.IsNullOrWhiteSpace(request.Email) && 
                   !string.IsNullOrWhiteSpace(request.Password) &&
                   request.Email.Contains("@");
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        private object CreateUserDto(User user)
        {
            return new
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };
        }

        private object CreateLoginResponse(AuthDTOs.LoginResponse result)
        {
            return new
            {
                User = new
                {
                    UserId = result.User.UserId,
                    FullName = result.User.FullName,
                    Email = result.User.Email,
                    Phone = result.User.Phone,
                    Role = result.User.Role
                },
                AccessToken = result.AccessToken,
                ExpiresAt = result.ExpiresAt
            };
        }

        private object CreateRegisterResponse(AuthDTOs.UserReadDto result)
        {
            return new
            {
                Message = "Đăng ký thành công",
                UserId = result.UserId,
                FullName = result.FullName,
                Email = result.Email,
                Phone = result.Phone,
                Role = result.Role
            };
        }
    }
}
