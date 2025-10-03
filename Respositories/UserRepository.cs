using TheGrind5_EventManagement.Models;


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

    }
}
