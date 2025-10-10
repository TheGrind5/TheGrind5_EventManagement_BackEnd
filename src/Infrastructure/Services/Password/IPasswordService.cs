namespace TheGrind5_EventManagement.Infrastructure.Services.Password
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
