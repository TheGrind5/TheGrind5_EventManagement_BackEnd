using System;
using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models;

public class Feedback
{
    public int FeedbackId { get; set; }
    
    public int EventId { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public int? ParentFeedbackId { get; set; }
    
    // Navigation properties
    public virtual Event? Event { get; set; }
    
    public virtual User? User { get; set; }
    
    public virtual Feedback? ParentFeedback { get; set; }
    
    public virtual ICollection<Feedback> Replies { get; set; } = new List<Feedback>();
    
    public virtual ICollection<FeedbackReaction> Reactions { get; set; } = new List<FeedbackReaction>();
}

public class FeedbackReaction
{
    public int ReactionId { get; set; }
    
    public int FeedbackId { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [RegularExpression("^(Like|Dislike)$", ErrorMessage = "ReactionType must be Like or Dislike")]
    public string ReactionType { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual Feedback? Feedback { get; set; }
    
    public virtual User? User { get; set; }
}
