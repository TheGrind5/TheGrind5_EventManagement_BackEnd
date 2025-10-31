namespace TheGrind5_EventManagement.Business;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string toEmail, string otpCode);
    
    // Notification emails
    Task<bool> SendEventReminderEmailAsync(string toEmail, string eventName, DateTime eventStartTime);
    Task<bool> SendEventUpdateEmailAsync(string toEmail, string eventName, string updateMessage);
    Task<bool> SendEventCancelledEmailAsync(string toEmail, string eventName);
    Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int orderId, decimal amount);
    Task<bool> SendPaymentSuccessEmailAsync(string toEmail, int orderId, decimal amount);
    Task<bool> SendRefundEmailAsync(string toEmail, int orderId, decimal refundAmount);
}
