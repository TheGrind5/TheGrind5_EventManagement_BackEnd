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
    }
}


