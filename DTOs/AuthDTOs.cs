namespace TheGrind5_EventManagement.DTOs
{
    public class AuthDTOs
    {
        public record LoginRequest(string Email, string Password);
        public record UserReadDto(int UserId, string FullName, string Email, string Phone, string Role);
        public record LoginResponse(string AccessToken, DateTime ExpiresAt, UserReadDto User);
    }

    public record RegisterRequest(
        string Username,
        string Email,
        string Password,
        string FullName,
        string? Phone = null
    );

    // Forgot Password DTOs
    public record ForgotPasswordRequest(string Email);
    public record ForgotPasswordResponse(string Message);
    
    public record ResetPasswordRequest(
        string Token,
        string NewPassword
    );
    public record ResetPasswordResponse(string Message);
}
