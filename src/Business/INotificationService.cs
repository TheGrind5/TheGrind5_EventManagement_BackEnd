using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Business;

public interface INotificationService
{
    // Create notification
    Task<NotificationResponse> CreateNotificationAsync(CreateNotificationRequest request);
    
    // Get notifications
    Task<NotificationListResponse> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10);
    Task<NotificationResponse?> GetNotificationByIdAsync(int notificationId, int userId);
    Task<NotificationStatsResponse> GetNotificationStatsAsync(int userId);
    
    // Update notifications
    Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
    Task<MarkAllNotificationsReadResponse> MarkAllNotificationsAsReadAsync(int userId);
    
    // Delete notifications
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
    
    // Auto-create notifications for system events
    Task CreateEventReminderNotificationAsync(int userId, int eventId, DateTime eventStartTime);
    Task CreateEventUpdateNotificationAsync(int userId, int eventId, string updateMessage);
    Task CreateEventCancelledNotificationAsync(int userId, int eventId);
    Task CreateOrderConfirmationNotificationAsync(int userId, int orderId);
    Task CreatePaymentSuccessNotificationAsync(int userId, int orderId);
    Task CreateRefundNotificationAsync(int userId, int orderId, decimal refundAmount);
}

