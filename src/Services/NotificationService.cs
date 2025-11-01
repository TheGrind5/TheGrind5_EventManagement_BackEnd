using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services;

public class NotificationService : INotificationService
{
    private readonly EventDBContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(EventDBContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request)
    {
        try
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RelatedEventId = request.RelatedEventId,
                RelatedOrderId = request.RelatedOrderId,
                RelatedTicketId = request.RelatedTicketId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created notification {notification.NotificationId} of type {notification.Type} for user {request.UserId}");

            return MapToNotificationResponse(notification);
        }
        catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Message.Contains("Invalid object name") || sqlEx.Number == 208)
        {
            _logger.LogWarning($"Không thể tạo notification vì bảng Notification chưa tồn tại. Vui lòng chạy script CREATE_NOTIFICATION_TABLE.sql. Lỗi: {sqlEx.Message}");
            // Trả về một notification response mặc định để không làm fail toàn bộ process
            return new NotificationResponse
            {
                NotificationId = 0,
                UserId = request.UserId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RelatedEventId = request.RelatedEventId,
                RelatedOrderId = request.RelatedOrderId,
                RelatedTicketId = request.RelatedTicketId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo notification cho user {UserId}", request.UserId);
            throw;
        }
    }

    public async Task<NotificationListResponse> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10)
    {
        // 🔧 FIX: Try-catch để handle trường hợp bảng chưa tồn tại
        try
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            var totalCount = await query.CountAsync();
            var unreadCount = await query.CountAsync(n => !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var notificationResponses = notifications.Select(MapToNotificationResponse).ToList();

            return new NotificationListResponse
            {
                Notifications = notificationResponses,
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Message.Contains("Invalid object name") || sqlEx.Number == 208)
        {
            // Bảng Notification chưa tồn tại, trả về empty list
            _logger.LogWarning("Bảng Notification chưa tồn tại trong database. Vui lòng chạy migration hoặc script CREATE_NOTIFICATION_TABLE.sql để tạo bảng.");
            return new NotificationListResponse
            {
                Notifications = new List<NotificationResponse>(),
                TotalCount = 0,
                UnreadCount = 0,
                Page = page,
                PageSize = pageSize,
                TotalPages = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy notifications cho user {UserId}", userId);
            throw;
        }
    }

    public async Task<NotificationResponse?> GetNotificationByIdAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        return notification != null ? MapToNotificationResponse(notification) : null;
    }

    public async Task<NotificationStatsResponse> GetNotificationStatsAsync(int userId)
    {
        try
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            var totalNotifications = notifications.Count;
            var unreadNotifications = notifications.Count(n => !n.IsRead);
            
            var notificationsByType = notifications
                .GroupBy(n => n.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            return new NotificationStatsResponse
            {
                TotalNotifications = totalNotifications,
                UnreadNotifications = unreadNotifications,
                NotificationsByType = notificationsByType
            };
        }
        catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Message.Contains("Invalid object name") || sqlEx.Number == 208)
        {
            _logger.LogWarning("Bảng Notification chưa tồn tại trong database.");
            return new NotificationStatsResponse
            {
                TotalNotifications = 0,
                UnreadNotifications = 0,
                NotificationsByType = new Dictionary<string, int>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy stats notifications cho user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Marked notification {notificationId} as read for user {userId}");
        return true;
    }

    public async Task<MarkAllNotificationsReadResponse> MarkAllNotificationsAsReadAsync(int userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        var now = DateTime.UtcNow;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        var markedCount = unreadNotifications.Count;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Marked {markedCount} notifications as read for user {userId}");

        return new MarkAllNotificationsReadResponse
        {
            MarkedCount = markedCount,
            Message = $"Đã đánh dấu {markedCount} thông báo là đã đọc"
        };
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null)
            return false;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Deleted notification {notificationId} for user {userId}");
        return true;
    }

    // Auto-create notification methods
    public async Task CreateEventReminderNotificationAsync(int userId, int eventId, DateTime eventStartTime)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null) return;

        var hoursUntilEvent = (eventStartTime - DateTime.UtcNow).TotalHours;
        var title = hoursUntilEvent > 24 
            ? $"Nhắc nhở sự kiện: {@event.Title}" 
            : $"Sự kiện bắt đầu sớm: {@event.Title}";

        var content = $"Sự kiện '{@event.Title}' sẽ diễn ra vào {eventStartTime:dd/MM/yyyy HH:mm}. " +
                     $"Hãy sắp xếp thời gian để tham gia!";

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            UserId = userId,
            Title = title,
            Content = content,
            Type = "EventReminder",
            RelatedEventId = eventId
        });
    }

    public async Task CreateEventUpdateNotificationAsync(int userId, int eventId, string updateMessage)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null) return;

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            UserId = userId,
            Title = $"Cập nhật sự kiện: {@event.Title}",
            Content = updateMessage,
            Type = "EventUpdate",
            RelatedEventId = eventId
        });
    }

    public async Task CreateEventCancelledNotificationAsync(int userId, int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null) return;

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            UserId = userId,
            Title = $"Sự kiện đã bị hủy: {@event.Title}",
            Content = "Sự kiện bạn đã đăng ký đã bị tổ chức viên hủy. Tiền sẽ được hoàn lại trong vòng 3-5 ngày làm việc.",
            Type = "EventCancelled",
            RelatedEventId = eventId
        });
    }

    public async Task CreateOrderConfirmationNotificationAsync(int userId, int orderId)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.TicketType)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return;

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            UserId = userId,
            Title = "Xác nhận đơn hàng thành công",
            Content = $"Đơn hàng #{orderId} của bạn đã được xác nhận. Tổng giá trị: {order.Amount:N0} VND",
            Type = "OrderConfirmation",
            RelatedOrderId = orderId
        });
    }

    public async Task CreatePaymentSuccessNotificationAsync(int userId, int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return;

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            UserId = userId,
            Title = "Thanh toán thành công",
            Content = $"Đơn hàng #{orderId} đã được thanh toán thành công với số tiền {order.Amount:N0} VND",
            Type = "PaymentSuccess",
            RelatedOrderId = orderId
        });
    }

    public async Task CreateRefundNotificationAsync(int userId, int orderId, decimal refundAmount)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return;

        await CreateNotificationAsync(new CreateNotificationRequest
        {
            UserId = userId,
            Title = "Hoàn tiền đã được xử lý",
            Content = $"Số tiền {refundAmount:N0} VND đã được hoàn về ví của bạn từ đơn hàng #{orderId}",
            Type = "Refund",
            RelatedOrderId = orderId
        });
    }

    private static NotificationResponse MapToNotificationResponse(Notification notification)
    {
        return new NotificationResponse
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Title = notification.Title,
            Content = notification.Content,
            Type = notification.Type,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            RelatedEventId = notification.RelatedEventId,
            RelatedOrderId = notification.RelatedOrderId,
            RelatedTicketId = notification.RelatedTicketId
        };
    }
}

