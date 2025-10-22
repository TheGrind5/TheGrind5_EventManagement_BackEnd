using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Business
{
    public interface IWalletService
    {
        // Wallet balance operations
        Task<decimal> GetWalletBalanceAsync(int userId);
        Task<bool> UpdateWalletBalanceAsync(int userId, decimal newBalance);
        
        // Transaction operations
        Task<WalletTransaction> CreateTransactionAsync(WalletTransaction transaction);
        Task<WalletTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<List<WalletTransaction>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 10);
        
        // Wallet operations
        Task<WalletTransaction> DepositAsync(int userId, decimal amount, string? description = null, string? referenceId = null);
        Task<WalletTransaction> WithdrawAsync(int userId, decimal amount, string? description = null, string? referenceId = null);
        Task<WalletTransaction> ProcessPaymentAsync(int userId, decimal amount, int orderId, string? description = null);
        Task<WalletTransaction> ProcessRefundAsync(int userId, decimal amount, int orderId, string? description = null);
        
        // Validation
        Task<bool> HasSufficientBalanceAsync(int userId, decimal amount);
        Task<bool> ValidateTransactionAsync(WalletTransaction transaction);
    }
}
