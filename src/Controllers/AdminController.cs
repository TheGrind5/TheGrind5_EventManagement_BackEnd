using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    /// <summary>
    /// Controller cho Admin - Quản lý người dùng
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả users với filter và pagination
        /// GET: api/admin/users?role=Host&searchTerm=nguyen&pageNumber=1&pageSize=10
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? role = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            try
            {
                var adminId = GetCurrentUserId();
                _logger.LogInformation("Admin {AdminId} requested users list", adminId);

                var request = new AdminDTOs.GetUsersRequest(
                    Role: role,
                    SearchTerm: searchTerm,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    SortBy: sortBy,
                    SortOrder: sortOrder
                );

                var response = await _adminService.GetAllUsersAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách người dùng thành công",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users list");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy danh sách người dùng",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một user
        /// GET: api/admin/users/5
        /// </summary>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var adminId = GetCurrentUserId();
                _logger.LogInformation("Admin {AdminId} requested user {UserId} details", adminId, userId);

                var user = await _adminService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy người dùng"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin người dùng thành công",
                    data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId} details", userId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy thông tin người dùng",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan về users
        /// GET: api/admin/statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var adminId = GetCurrentUserId();
                _logger.LogInformation("Admin {AdminId} requested user statistics", adminId);

                var stats = await _adminService.GetUserStatisticsAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê thành công",
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy thống kê",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách Hosts
        /// GET: api/admin/hosts
        /// </summary>
        [HttpGet("hosts")]
        public async Task<IActionResult> GetHosts(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var request = new AdminDTOs.GetUsersRequest(
                    Role: "Host",
                    SearchTerm: searchTerm,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    SortBy: "CreatedAt",
                    SortOrder: "desc"
                );

                var response = await _adminService.GetAllUsersAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách Host thành công",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hosts list");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy danh sách Host",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách Customers
        /// GET: api/admin/customers
        /// </summary>
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var request = new AdminDTOs.GetUsersRequest(
                    Role: "Customer",
                    SearchTerm: searchTerm,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    SortBy: "CreatedAt",
                    SortOrder: "desc"
                );

                var response = await _adminService.GetAllUsersAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách Customer thành công",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers list");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy danh sách Customer",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả orders với filter và pagination
        /// GET: api/admin/orders?searchTerm=nguyen&pageNumber=1&pageSize=10
        /// </summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "desc")
        {
            try
            {
                var adminId = GetCurrentUserId();
                _logger.LogInformation("Admin {AdminId} requested orders list", adminId);

                var request = new AdminDTOs.GetOrdersRequest(
                    SearchTerm: searchTerm,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    SortBy: sortBy,
                    SortOrder: sortOrder
                );

                var response = await _adminService.GetAllOrdersAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách orders thành công",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders list");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy danh sách orders",
                    error = ex.Message
                });
            }
        }

        // Helper method để lấy UserId từ JWT token
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}


