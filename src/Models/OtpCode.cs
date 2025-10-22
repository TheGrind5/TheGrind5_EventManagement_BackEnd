using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.Models;

public class OtpCode
{
    [Key]
    public int OtpId { get; set; }
    
    [Required]
    [StringLength(6)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    public bool IsUsed { get; set; } = false;
    
    public DateTime? UsedAt { get; set; }
}
