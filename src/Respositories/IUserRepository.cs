using TheGrind5_EventManagement.Models;
namespace TheGrind5_EventManagement.Respositories
{
    public interface IUserRepository
    {
        
        Task<User?> GetByEmailAsync(string email); 
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
    }
}
