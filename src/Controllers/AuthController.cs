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
        private readonly ILogger<AuthController> _logger; 
       

        public AuthController(IAuthService authService, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTOs.LoginRequest request)
        {
            try
            {
                if (!IsValidLoginRequest(request))
                {
                    _logger.LogWarning("Invalid login request for email: {Email}", request.Email);
                    return BadRequest(new { message = "Email và mật khẩu không hợp lệ" });
                }

                var result = await _authService.LoginAsync(request.Email!, request.Password!);
                
                if (result == null)
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
                }

                _logger.LogInformation("Successful login for user: {Email}", request.Email);
                return Ok(CreateLoginResponse(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Banned user login attempt for email: {Email}", request.Email);
                return StatusCode(403, new { message = ex.Message, isBanned = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return BadRequest(new { message = "Có lỗi xảy ra khi đăng nhập", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                    return BadRequest(new { message = "Email không hợp lệ" });

                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                    return BadRequest(new { message = "Mật khẩu phải có ít nhất 8 ký tự" });

                if (string.IsNullOrWhiteSpace(request.Username))
                    return BadRequest(new { message = "Username không được để trống" });

                if (string.IsNullOrWhiteSpace(request.FullName))
                    return BadRequest(new { message = "Họ tên không được để trống" });

                // Check email exists
                if (await _userRepository.IsEmailExistsAsync(request.Email))
                    return BadRequest(new { message = "Email này đã được sử dụng" });

                var result = await _authService.RegisterAsync(request);
                return Ok(CreateRegisterResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with email: {Email}", request.Email);
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

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy user" });

                return Ok(CreateProfileDetailDto(user));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileDTOs.UpdateProfileRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy user" });

                // Cập nhật thông tin nếu có
                if (!string.IsNullOrWhiteSpace(request.fullName))
                    user.FullName = request.fullName;
                
                if (!string.IsNullOrWhiteSpace(request.phone))
                    user.Phone = request.phone;

                if (!string.IsNullOrWhiteSpace(request.avatar))
                    user.Avatar = request.avatar;

                if (request.dateOfBirth.HasValue)
                    user.DateOfBirth = request.dateOfBirth;

                if (!string.IsNullOrWhiteSpace(request.gender))
                    user.Gender = request.gender;

                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateUserAsync(user);

                return Ok(new { 
                    message = "Cập nhật profile thành công",
                    user = new {
                        userId = user.UserId,
                        fullName = user.FullName,
                        email = user.Email,
                        phone = user.Phone,
                        role = user.Role,
                        avatar = user.Avatar,
                        walletBalance = user.WalletBalance,
                        dateOfBirth = user.DateOfBirth,
                        gender = user.Gender
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật profile", error = ex.Message });
            }
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                if (avatar == null || avatar.Length == 0)
                    return BadRequest(new { message = "Không có file được upload" });

                // Kiểm tra loại file
                if (!avatar.ContentType.StartsWith("image/"))
                    return BadRequest(new { message = "Chỉ được upload file ảnh" });

                // Kiểm tra kích thước file (max 5MB)
                if (avatar.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "Kích thước file không được vượt quá 5MB" });

                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Tạo thư mục assets/images/avatars nếu chưa tồn tại
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "assets", "images", "avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Xóa file avatar cũ (nếu có) - XÓA TRƯỚC
                var oldFiles = Directory.GetFiles(uploadsFolder, $"user_{userId}.*");
                foreach (var oldFile in oldFiles)
                {
                    try
                    {
                        System.IO.File.Delete(oldFile);
                        Console.WriteLine($"Đã xóa file cũ: {oldFile}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể xóa file cũ {oldFile}: {ex.Message}");
                    }
                }

                // Tạo tên file cố định
                var fileExtension = Path.GetExtension(avatar.FileName);
                var fileName = $"user_{userId}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file mới - LƯU SAU
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }

                // Cập nhật DB
                var avatarUrl = $"/assets/images/avatars/{fileName}";
                var user = await _userRepository.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    user.Avatar = avatarUrl;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateUserAsync(user);
                }

                return Ok(new { 
                    message = "Upload avatar thành công", 
                    avatarUrl = avatarUrl,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi upload avatar", error = ex.Message });
            }
        }

        [HttpGet("avatar/{fileName}")]
        public IActionResult GetAvatar(string fileName)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "images", "avatars", fileName);
                
                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "Avatar not found" });

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = GetContentType(fileName);
                
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error serving avatar", error = ex.Message });
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                var userDtos = users.Select(user => new
                {
                    userId = user.UserId,
                    username = user.Username,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    role = user.Role,
                    walletBalance = user.WalletBalance,
                    avatar = user.Avatar,
                    dateOfBirth = user.DateOfBirth,
                    gender = user.Gender,
                    createdAt = user.CreatedAt,
                    updatedAt = user.UpdatedAt,
                    isBanned = user.IsBanned,
                    bannedAt = user.BannedAt,
                    banReason = user.BanReason
                }).ToList();

                return Ok(new { 
                    success = true,
                    data = userDtos,
                    totalCount = userDtos.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách users", error = ex.Message });
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
                userId = user.UserId,
                fullName = user.FullName,
                email = user.Email,
                phone = user.Phone,
                role = user.Role,
                avatar = user.Avatar,
                walletBalance = user.WalletBalance
            };
        }

        private object CreateLoginResponse(AuthDTOs.LoginResponse result)
        {
            return new
            {
                user = new
                {
                    userId = result.User.UserId,
                    fullName = result.User.FullName,
                    email = result.User.Email,
                    phone = result.User.Phone,
                    role = result.User.Role,
                    avatar = result.User.Avatar,
                    walletBalance = result.User.WalletBalance
                },
                accessToken = result.AccessToken,
                expiresAt = result.ExpiresAt
            };
        }

        private object CreateRegisterResponse(AuthDTOs.UserReadDto result)
        {
            return new
            {
                message = "Đăng ký thành công",
                userId = result.UserId,
                fullName = result.FullName,
                email = result.Email,
                phone = result.Phone,
                role = result.Role,
                walletBalance = result.WalletBalance
            };
        }

        private ProfileDTOs.ProfileDetailDto CreateProfileDetailDto(User user)
        {
            return new ProfileDTOs.ProfileDetailDto(
                user.UserId,
                user.Username,
                user.FullName,
                user.Email,
                user.Phone ?? string.Empty,
                user.Role,
                user.CreatedAt,
                user.UpdatedAt,
                user.Avatar,
                user.DateOfBirth,
                user.Gender
            );
        }

        // Ban/Unban User Endpoints
        [HttpPost("users/{userId}/ban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser(int userId, [FromBody] BanUserRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                if (user.Role == "Admin")
                {
                    return BadRequest(new { message = "Không thể ban tài khoản Admin" });
                }

                if (user.IsBanned)
                {
                    return BadRequest(new { message = "Tài khoản đã bị cấm trước đó" });
                }

                user.IsBanned = true;
                user.BannedAt = DateTime.UtcNow;
                user.BanReason = request.Reason ?? "Vi phạm chính sách";

                await _userRepository.UpdateUserAsync(user);

                _logger.LogInformation("User {UserId} has been banned by admin. Reason: {Reason}", userId, user.BanReason);

                return Ok(new
                {
                    success = true,
                    message = "Đã cấm tài khoản thành công",
                    data = new
                    {
                        userId = user.UserId,
                        isBanned = user.IsBanned,
                        bannedAt = user.BannedAt,
                        banReason = user.BanReason
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error banning user {UserId}", userId);
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cấm tài khoản" });
            }
        }

        [HttpPost("users/{userId}/unban")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                if (!user.IsBanned)
                {
                    return BadRequest(new { message = "Tài khoản không bị cấm" });
                }

                user.IsBanned = false;
                user.BannedAt = null;
                user.BanReason = null;

                await _userRepository.UpdateUserAsync(user);

                _logger.LogInformation("User {UserId} has been unbanned by admin", userId);

                return Ok(new
                {
                    success = true,
                    message = "Đã mở cấm tài khoản thành công",
                    data = new
                    {
                        userId = user.UserId,
                        isBanned = user.IsBanned
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unbanning user {UserId}", userId);
                return StatusCode(500, new { message = "Có lỗi xảy ra khi mở cấm tài khoản" });
            }
        }
    }

    // DTO for ban request
    public class BanUserRequest
    {
        public string? Reason { get; set; }
    }
}
