using TheGrind5_EventManagement.Constants;

namespace TheGrind5_EventManagement.DTOs;

// Request để tạo VNPay payment
public record CreateVNPayPaymentRequest
{
    public int OrderId { get; init; }
    public string? ReturnUrl { get; init; }
}

// Response sau khi tạo payment
public record CreateVNPayPaymentResponse
{
    public int PaymentId { get; init; }
    public string PaymentUrl { get; init; }
    public string QrCodeUrl { get; init; }
    public DateTime ExpiredAt { get; init; }
}

// Webhook data từ VNPay
public record VNPayWebhookData
{
    public string vnp_TmnCode { get; init; } = string.Empty;
    public long vnp_Amount { get; init; }
    public string vnp_BankCode { get; init; } = string.Empty;
    public string? vnp_BankTranNo { get; init; }
    public string? vnp_CardType { get; init; }
    public string vnp_PayDate { get; init; } = string.Empty;
    public string vnp_OrderInfo { get; init; } = string.Empty;
    public string vnp_TransactionNo { get; init; } = string.Empty;
    public string vnp_ResponseCode { get; init; } = string.Empty;
    public string vnp_TransactionStatus { get; init; } = string.Empty;
    public string vnp_TxnRef { get; init; } = string.Empty;
    public string vnp_SecureHash { get; init; } = string.Empty;
    public string? vnp_SecureHashType { get; init; }
    public string? vnp_CreateDate { get; init; }
    public string? vnp_IpAddr { get; init; }
    public string? vnp_CurrCode { get; init; }
}

// Payment status response
public record PaymentStatusResponse
{
    public int PaymentId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
}

// Configuration binding model
public class VNPaySettings
{
    public string TmnCode { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string IpnUrl { get; set; } = string.Empty;
    public string QueryUrl { get; set; } = string.Empty;
    public string Command { get; set; } = VNPayConstants.COMMAND;
    public string CurrCode { get; set; } = VNPayConstants.CURRENCY;
    public string Version { get; set; } = VNPayConstants.VERSION;
    public string Locale { get; set; } = VNPayConstants.LOCALE;
    public string TimeZoneId { get; set; } = VNPayConstants.TIMEZONE;
    public string OrderType { get; set; } = VNPayConstants.ORDER_TYPE;
}

