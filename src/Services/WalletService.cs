using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;

namespace TheGrind5_EventManagement.Services
{
    public class WalletService : IWalletService
    {
        private readonly EventDBContext _context;
        private readonly IUserRepository _userRepository;

        public WalletService(EventDBContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<decimal> GetWalletBalanceAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user?.WalletBalance ?? 0;
        }

        public async Task<bool> UpdateWalletBalanceAsync(int userId, decimal newBalance)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.WalletBalance = newBalance;
            user.UpdatedAt = DateTime.UtcNow;
            
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<WalletTransaction> CreateTransactionAsync(WalletTransaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.Status = "Pending";
            
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            
            return transaction;
        }

        public async Task<WalletTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.WalletTransactions
                .Include(wt => wt.User)
                .FirstOrDefaultAsync(wt => wt.TransactionId == transactionId);
        }

        public async Task<List<WalletTransaction>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 10)
        {
            return await _context.WalletTransactions
                .Where(wt => wt.UserId == userId)
                .OrderByDescending(wt => wt.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<WalletTransaction> DepositAsync(int userId, decimal amount, string? description = null, string? referenceId = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than 0");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var balanceBefore = user.WalletBalance;
                var balanceAfter = balanceBefore + amount;

                // Update user wallet balance
                user.WalletBalance = balanceAfter;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);

                // Create transaction record
                var walletTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    TransactionType = "Deposit",
                    Status = "Completed",
                    Description = description ?? "Wallet deposit",
                    ReferenceId = referenceId,
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter
                };

                await CreateTransactionAsync(walletTransaction);
                await transaction.CommitAsync();

                return walletTransaction;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WalletTransaction> WithdrawAsync(int userId, decimal amount, string? description = null, string? referenceId = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than 0");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.WalletBalance < amount)
                throw new InvalidOperationException("Insufficient wallet balance");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var balanceBefore = user.WalletBalance;
                var balanceAfter = balanceBefore - amount;

                // Update user wallet balance
                user.WalletBalance = balanceAfter;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);

                // Create transaction record
                var walletTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    TransactionType = "Withdraw",
                    Status = "Completed",
                    Description = description ?? "Wallet withdrawal",
                    ReferenceId = referenceId,
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter
                };

                await CreateTransactionAsync(walletTransaction);
                await transaction.CommitAsync();

                return walletTransaction;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WalletTransaction> ProcessPaymentAsync(int userId, decimal amount, int orderId, string? description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (user.WalletBalance < amount)
                throw new InvalidOperationException("Insufficient wallet balance for payment");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var balanceBefore = user.WalletBalance;
                var balanceAfter = balanceBefore - amount;

                // Update user wallet balance
                user.WalletBalance = balanceAfter;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);

                // Create transaction record
                var walletTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    TransactionType = "Payment",
                    Status = "Completed",
                    Description = description ?? $"Payment for order #{orderId}",
                    ReferenceId = orderId.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter
                };

                await CreateTransactionAsync(walletTransaction);
                await transaction.CommitAsync();

                return walletTransaction;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WalletTransaction> ProcessRefundAsync(int userId, decimal amount, int orderId, string? description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Refund amount must be greater than 0");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var balanceBefore = user.WalletBalance;
                var balanceAfter = balanceBefore + amount;

                // Update user wallet balance
                user.WalletBalance = balanceAfter;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateUserAsync(user);

                // Create transaction record
                var walletTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    TransactionType = "Refund",
                    Status = "Completed",
                    Description = description ?? $"Refund for order #{orderId}",
                    ReferenceId = orderId.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter
                };

                await CreateTransactionAsync(walletTransaction);
                await transaction.CommitAsync();

                return walletTransaction;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> HasSufficientBalanceAsync(int userId, decimal amount)
        {
            var balance = await GetWalletBalanceAsync(userId);
            return balance >= amount;
        }

        public async Task<bool> ValidateTransactionAsync(WalletTransaction transaction)
        {
            if (transaction == null) return false;
            if (transaction.Amount <= 0) return false;
            if (string.IsNullOrEmpty(transaction.TransactionType)) return false;
            if (transaction.UserId <= 0) return false;

            var user = await _userRepository.GetUserByIdAsync(transaction.UserId);
            return user != null;
        }
    }
}
