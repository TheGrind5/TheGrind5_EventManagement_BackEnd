namespace TheGrind5_EventManagement.DTOs;

public class VoucherValidationRequest
{
    public string VoucherCode { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
}

public class VoucherValidationResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string VoucherCode { get; set; } = string.Empty;
}

public class VoucherDTO
{
    public int VoucherId { get; set; }
    public string VoucherCode { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VoucherCreateDTO
{
    public string VoucherCode { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}
