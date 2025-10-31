using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> IsEmailExistsAsync(string email);
        
        // Wallet methods
        Task<bool> UpdateWalletBalanceAsync(int userId, decimal newBalance);
        Task<decimal> GetWalletBalanceAsync(int userId);
        
        // Admin methods - với các parameter filter
        Task<List<User>> GetAllUsersAsync(string? role = null, string? searchTerm = null, string sortBy = "CreatedAt", string sortOrder = "desc", int skip = 0, int take = int.MaxValue);
        Task<int> GetTotalUsersCountAsync(string? role = null, string? searchTerm = null);
        Task<Dictionary<string, int>> GetUserStatisticsAsync();
    }
}


