using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGrind5_EventManagement.Models;

public class Voucher
{
    [Key]
    public int VoucherId { get; set; }

    [Required]
    [StringLength(50)]
    public string VoucherCode { get; set; } = string.Empty;

    [Required]
    [Range(1, 100, ErrorMessage = "Discount percentage must be between 1 and 100")]
    public decimal DiscountPercentage { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    [Required]
    public DateTime ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Helper method to check if voucher is valid
    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        return IsActive && now >= ValidFrom && now <= ValidTo;
    }

    // Helper method to calculate discount amount
    public decimal CalculateDiscountAmount(decimal originalAmount)
    {
        return originalAmount * (DiscountPercentage / 100);
    }

    // Helper method to calculate final amount after discount
    public decimal CalculateFinalAmount(decimal originalAmount)
    {
        return originalAmount - CalculateDiscountAmount(originalAmount);
    }
}
