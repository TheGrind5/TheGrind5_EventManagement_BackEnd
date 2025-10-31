using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs;

// Request DTOs
public record CreateNotificationRequest
{
    [Required]
    public int UserId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    public string? Content { get; init; }

    [Required]
    [RegularExpression("^(EventReminder|EventUpdate|PaymentSuccess|Refund|OrderConfirmation|EventCancelled|NewMessage)$")]
    public string Type { get; init; } = string.Empty;

    public int? RelatedEventId { get; init; }
    
    public int? RelatedOrderId { get; init; }
    
    public int? RelatedTicketId { get; init; }
}

public record UpdateNotificationReadStatusRequest
{
    [Required]
    public bool IsRead { get; init; }
}

// Response DTOs
public record NotificationResponse
{
    public int NotificationId { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Content { get; init; }
    public string Type { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ReadAt { get; init; }
    public int? RelatedEventId { get; init; }
    public int? RelatedOrderId { get; init; }
    public int? RelatedTicketId { get; init; }
}

public record NotificationListResponse
{
    public List<NotificationResponse> Notifications { get; init; } = new();
    public int TotalCount { get; init; }
    public int UnreadCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

public record NotificationStatsResponse
{
    public int TotalNotifications { get; init; }
    public int UnreadNotifications { get; init; }
    public Dictionary<string, int> NotificationsByType { get; init; } = new();
}

public record MarkAllNotificationsReadResponse
{
    public int MarkedCount { get; init; }
    public string Message { get; init; } = string.Empty;
}

