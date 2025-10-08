using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Respositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventDBContext db;
        public UserRepository(EventDBContext db)
        {
            this.db = db;
        }
        public Task<User?> GetByEmailAsync(string email) => db.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User> AddAsync(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
