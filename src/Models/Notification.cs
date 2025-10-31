using System;
using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    [Required]
    [RegularExpression("^(EventReminder|EventUpdate|PaymentSuccess|Refund|OrderConfirmation|EventCancelled|NewMessage)$", 
        ErrorMessage = "Type must be one of: EventReminder, EventUpdate, PaymentSuccess, Refund, OrderConfirmation, EventCancelled, NewMessage")]
    public string Type { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    // Related entity references (optional)
    public int? RelatedEventId { get; set; }
    
    public int? RelatedOrderId { get; set; }
    
    public int? RelatedTicketId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}

