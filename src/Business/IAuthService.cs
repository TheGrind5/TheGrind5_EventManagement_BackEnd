using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    public interface IAuthService
    {
        Task<AuthDTOs.LoginResponse?> LoginAsync(string email, string password);
        Task<AuthDTOs.UserReadDto> RegisterAsync(RegisterRequest request);
    }
}

