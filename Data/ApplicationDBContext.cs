using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Models.Entities;
namespace TheGrind5_EventManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        public DbSet<User> User{ get; set; }
    }
}
