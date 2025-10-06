using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

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
                var result = await _authService.LoginAsync(request.Email, request.Password);
                
                if (result == null)
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi đăng nhập", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
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
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi đăng ký", error = ex.Message });
            }
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                // Lấy user ID từ token (sẽ implement sau)
                // Tạm thời return null để frontend có thể test
                return Ok(new { message = "Endpoint này cần authentication token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra", error = ex.Message });
            }
        }
    }
}
