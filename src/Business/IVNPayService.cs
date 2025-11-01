using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business;

public interface IVNPayService
{
    Task<CreateVNPayPaymentResponse> CreatePaymentAsync(int orderId, string returnUrl);
    Task<bool> ProcessWebhookAsync(VNPayWebhookData webhookData);
    Task<PaymentStatusResponse?> GetPaymentStatusAsync(int paymentId);
    Task<bool> CancelPaymentAsync(int paymentId);
    bool ValidateSignature(VNPayWebhookData webhookData);
}

