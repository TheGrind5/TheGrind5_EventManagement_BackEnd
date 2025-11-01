using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models;

public class EventQuestion
{
    public int QuestionId { get; set; }

    public int EventId { get; set; }

    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = string.Empty;

    public string QuestionType { get; set; } = "Text"; // "Text", "Number", "Email", "Phone", "Date", "Radio", "Checkbox", "Dropdown"

    public bool IsRequired { get; set; } = true;

    public string? Options { get; set; } // JSON array for Radio/Checkbox/Dropdown options
    
    public string? Placeholder { get; set; }

    public string? ValidationRules { get; set; } // JSON for validation rules

    public int DisplayOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Event Event { get; set; }
}

