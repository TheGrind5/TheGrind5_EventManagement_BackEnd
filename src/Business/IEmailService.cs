namespace TheGrind5_EventManagement.Business;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string toEmail, string otpCode);
}
