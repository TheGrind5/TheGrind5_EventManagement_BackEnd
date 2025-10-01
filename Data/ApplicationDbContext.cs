using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Models;
namespace TheGrind5_EventManagement.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; }
    }
}
