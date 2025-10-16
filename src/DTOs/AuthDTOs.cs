namespace TheGrind5_EventManagement.DTOs
{
    public class AuthDTOs
    {
        public record LoginRequest(string? Email, string? Password);
        public record UserReadDto(int UserId, string FullName, string Email, string Phone, string Role, string? Avatar = null);
        public record LoginResponse(string AccessToken, DateTime ExpiresAt, UserReadDto User);
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
