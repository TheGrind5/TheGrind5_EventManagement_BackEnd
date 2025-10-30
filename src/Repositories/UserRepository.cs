using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventDBContext _context;

        public UserRepository(EventDBContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var normalizedEmail = email.ToLowerInvariant().Trim();
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        public async Task<List<User>> GetAllUsersAsync(string? role = null, string? searchTerm = null, string sortBy = "CreatedAt", string sortOrder = "desc", int skip = 0, int take = int.MaxValue)
        {
            var query = _context.Users.AsQueryable();

            // Filter by role
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(searchLower) || 
                                         u.Email.ToLower().Contains(searchLower));
            }

            // Sorting
            query = sortBy.ToLower() switch
            {
                "fullname" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(u => u.FullName) 
                    : query.OrderByDescending(u => u.FullName),
                "email" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(u => u.Email) 
                    : query.OrderByDescending(u => u.Email),
                "walletbalance" => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(u => u.WalletBalance) 
                    : query.OrderByDescending(u => u.WalletBalance),
                _ => sortOrder.ToLower() == "asc" 
                    ? query.OrderBy(u => u.CreatedAt) 
                    : query.OrderByDescending(u => u.CreatedAt)
            };

            // Pagination
            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var normalizedEmail = email.ToLowerInvariant().Trim();
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        public async Task<bool> UpdateWalletBalanceAsync(int userId, decimal newBalance)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.WalletBalance = newBalance;
                user.UpdatedAt = DateTime.UtcNow;
                
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<decimal> GetWalletBalanceAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.WalletBalance ?? 0;
        }

        public async Task<int> GetTotalUsersCountAsync(string? role = null, string? searchTerm = null)
        {
            var query = _context.Users.AsQueryable();

            // Filter by role
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(searchLower) || 
                                         u.Email.ToLower().Contains(searchLower));
            }

            return await query.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetUserStatisticsAsync()
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            var totalUsers = await _context.Users.CountAsync();
            var totalHosts = await _context.Users.CountAsync(u => u.Role == "Host");
            var totalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer");
            var totalAdmins = await _context.Users.CountAsync(u => u.Role == "Admin");
            var newUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= firstDayOfMonth);

            return new Dictionary<string, int>
            {
                { "TotalUsers", totalUsers },
                { "TotalHosts", totalHosts },
                { "TotalCustomers", totalCustomers },
                { "TotalAdmins", totalAdmins },
                { "NewUsersThisMonth", newUsersThisMonth }
            };
        }
    }
}


