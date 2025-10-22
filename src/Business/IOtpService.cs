namespace TheGrind5_EventManagement.Business;

public interface IOtpService
{
    Task<string> GenerateOtpAsync(string email);
    Task<bool> VerifyOtpAsync(string email, string otpCode);
    Task<bool> IsOtpExpiredAsync(string email);
    Task CleanupExpiredOtpsAsync();
}
