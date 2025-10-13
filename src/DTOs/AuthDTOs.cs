namespace TheGrind5_EventManagement.DTOs
{
    public class AuthDTOs
    {
        public record LoginRequest(string? Email, string? Password);
        public record UserReadDto(int UserId, string FullName, string Email, string Phone, string Role, decimal WalletBalance);
        public record LoginResponse(string AccessToken, DateTime ExpiresAt, UserReadDto User);
        
        // Wallet DTOs
        public record WalletResponse(decimal Balance, string Currency = "VND");
        public record WalletTransactionDto(int TransactionId, decimal Amount, string Type, string Description, DateTime CreatedAt);
    }

    public record RegisterRequest(
        string Username,
        string Email,
        string Password,
        string FullName,
        string? Phone = null
    );
}
