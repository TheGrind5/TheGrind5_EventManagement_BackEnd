namespace TheGrind5_EventManagement.DTOs
{
    public class AuthDTOs
    {
        public record LoginRequest(string? Email, string? Password);
        public record UserReadDto(int UserId, string FullName, string Email, string Phone, string Role, string? Avatar = null,decimal WalletBalance);
        public record LoginResponse(string AccessToken, DateTime ExpiresAt, UserReadDto User);
        
        // Wallet DTOs
        public record WalletResponse(decimal Balance, string Currency = "VND");
        public record WalletTransactionDto(int TransactionId, decimal Amount, string Type, string Description, DateTime CreatedAt);
        
        // Forgot Password DTOs
        public record ForgotPasswordRequest(string Email);
        public record VerifyOtpRequest(string Email, string OtpCode);
        public record ResetPasswordRequest(string Email, string OtpCode, string NewPassword);
    }

    public record RegisterRequest(
        string Username,
        string Email,
        string Password,
        string FullName,
        string? Phone = null,
        string? Avatar = null,
        DateTime? DateOfBirth = null,
        string? Gender = null
    );
}
