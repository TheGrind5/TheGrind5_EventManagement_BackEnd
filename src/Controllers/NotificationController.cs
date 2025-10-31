using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications for current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var response = await _notificationService.GetUserNotificationsAsync(userId.Value, page, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user");
            return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Get notification statistics for current user
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetNotificationStats()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var stats = await _notificationService.GetNotificationStatsAsync(userId.Value);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification stats for user");
            return BadRequest(new { message = "Có lỗi xảy ra khi lấy thống kê thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotification(int notificationId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var notification = await _notificationService.GetNotificationByIdAsync(notificationId, userId.Value);
            if (notification == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {NotificationId}", notificationId);
            return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var success = await _notificationService.MarkNotificationAsReadAsync(notificationId, userId.Value);
            if (!success)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            return Ok(new { message = "Đã đánh dấu thông báo là đã đọc" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            return BadRequest(new { message = "Có lỗi xảy ra khi đánh dấu thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var response = await _notificationService.MarkAllNotificationsAsReadAsync(userId.Value);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user");
            return BadRequest(new { message = "Có lỗi xảy ra khi đánh dấu tất cả thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var success = await _notificationService.DeleteNotificationAsync(notificationId, userId.Value);
            if (!success)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            return Ok(new { message = "Đã xóa thông báo" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", notificationId);
            return BadRequest(new { message = "Có lỗi xảy ra khi xóa thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new notification (Admin only or system use)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

            var notification = await _notificationService.CreateNotificationAsync(request);
            return CreatedAtAction(
                nameof(GetNotification), 
                new { notificationId = notification.NotificationId }, 
                notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            return BadRequest(new { message = "Có lỗi xảy ra khi tạo thông báo", error = ex.Message });
        }
    }

    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : null;
    }
}

