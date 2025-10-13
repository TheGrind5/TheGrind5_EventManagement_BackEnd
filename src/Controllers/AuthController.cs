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
        // private readonly IEmailService _emailService;
        // private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
            // _emailService = emailService;
            // _otpService = otpService;
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

        [HttpGet("wallet")]
        [Authorize]
        public async Task<IActionResult> GetMyWallet()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy user" });

                var walletResponse = new AuthDTOs.WalletResponse(user.WalletBalance);
                return Ok(walletResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin ví", error = ex.Message });
            }
        }

        [HttpGet("wallet/balance")]
        [Authorize]
        public async Task<IActionResult> GetWalletBalance()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy user" });

                return Ok(new { balance = user.WalletBalance, currency = "VND" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy số dư ví", error = ex.Message });
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
                    CreatedAt = DateTime.UtcNow,
                    WalletBalance = 1000000
                };

                await _userRepository.CreateUserAsync(adminUser);

                // Tạo thêm user test
                var testUser = new User
                {
                    Username = "testuser",
                    FullName = "Test User",
                    Email = "test@test.com",
                    PasswordHash = HashPassword("123456"),
                    Phone = "0987654321",
                    Role = "Customer",
                    CreatedAt = DateTime.UtcNow,
                    WalletBalance = 500000
                };

                await _userRepository.CreateUserAsync(testUser);

                return Ok(new { 
                    message = "Test users created successfully", 
                    admin = new { email = "admin@test.com", password = "admin123" },
                    user = new { email = "test@test.com", password = "123456" }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo admin user", error = ex.Message });
            }
        }

        // Forgot password endpoints temporarily disabled
        /*
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] AuthDTOs.ForgotPasswordRequest request)
        {
            return BadRequest(new { message = "Tính năng này đang được phát triển" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] AuthDTOs.VerifyOtpRequest request)
        {
            return BadRequest(new { message = "Tính năng này đang được phát triển" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] AuthDTOs.ResetPasswordRequest request)
        {
            return BadRequest(new { message = "Tính năng này đang được phát triển" });
        }
        */

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
                Role = user.Role,
                WalletBalance = user.WalletBalance
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
                    Role = result.User.Role,
                    WalletBalance = result.User.WalletBalance
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
                Role = result.Role,
                WalletBalance = result.WalletBalance
            };
        }
    }
}
