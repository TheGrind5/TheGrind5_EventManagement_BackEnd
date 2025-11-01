using System;
using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models;

public partial class AISuggestion
{
    public int SuggestionId { get; set; }

    public int UserId { get; set; }

    [Required]
    [RegularExpression("^(EventRecommendation|ChatbotQA|PricingSuggestion|ContentGeneration)$", 
        ErrorMessage = "SuggestionType must be EventRecommendation, ChatbotQA, PricingSuggestion, or ContentGeneration")]
    public string SuggestionType { get; set; } = string.Empty;

    public string? RequestData { get; set; } // JSON

    public string? ResponseData { get; set; } // JSON

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
}

