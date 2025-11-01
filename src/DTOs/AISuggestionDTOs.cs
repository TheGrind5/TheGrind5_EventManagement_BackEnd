using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs;

// ==================== REQUEST DTOs ====================

/// <summary>
/// Request DTO for Event Recommendation
/// </summary>
public class EventRecommendationRequest
{
    [Required]
    public int UserId { get; set; }
}

/// <summary>
/// Request DTO for AI Chatbot
/// </summary>
public class ChatbotRequest
{
    [Required]
    public string Question { get; set; } = string.Empty;

    public int? EventId { get; set; }
}

/// <summary>
/// Request DTO for Pricing Suggestion
/// </summary>
public class PricingSuggestionRequest
{
    public string? Category { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public string? Location { get; set; }
}

/// <summary>
/// Request DTO for Content Generation
/// </summary>
public class ContentGenerationRequest
{
    public string? EventType { get; set; }
    
    public string? Title { get; set; }
    
    public string? Category { get; set; }
}

// ==================== RESPONSE DTOs ====================

/// <summary>
/// Response DTO for Event Recommendation
/// </summary>
public class EventRecommendationResponse
{
    public List<RecommendedEvent> Events { get; set; } = new();
    
    public string Reasoning { get; set; } = string.Empty;
}

/// <summary>
/// Recommended Event model
/// </summary>
public class RecommendedEvent
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? Category { get; set; }
    public string? EventMode { get; set; }
    public decimal? MinPrice { get; set; }
    public string? ImageUrl { get; set; }
    public double SimilarityScore { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for AI Chatbot
/// </summary>
public class ChatbotResponse
{
    public string Answer { get; set; } = string.Empty;
    
    public List<string> RelatedLinks { get; set; } = new();
    
    public string? Confidence { get; set; }
}

/// <summary>
/// Response DTO for Pricing Suggestion
/// </summary>
public class PricingSuggestionResponse
{
    public List<PriceRange> SuggestedPrices { get; set; } = new();
    
    public string Analysis { get; set; } = string.Empty;
    
    public string? Recommendation { get; set; }
}

/// <summary>
/// Price Range model for pricing suggestions
/// </summary>
public class PriceRange
{
    public string TicketType { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal RecommendedPrice { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for Content Generation
/// </summary>
public class ContentGenerationResponse
{
    public string? Description { get; set; }
    
    public string? Introduction { get; set; }
    
    public string? TermsAndConditions { get; set; }
    
    public string? SpecialExperience { get; set; }
}

/// <summary>
/// History response DTO
/// </summary>
public class AISuggestionHistoryResponse
{
    public int SuggestionId { get; set; }
    public string SuggestionType { get; set; } = string.Empty;
    public string? RequestData { get; set; }
    public DateTime CreatedAt { get; set; }
}

