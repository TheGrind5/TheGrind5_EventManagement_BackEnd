using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateToken(User user, DateTime expiresAt);
    }
}

