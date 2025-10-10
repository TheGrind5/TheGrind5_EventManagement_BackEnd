using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Infrastructure.Services.Mappers
{
    public interface IUserMapper
    {
        AuthDTOs.UserReadDto MapToUserReadDto(User user);
        User MapFromRegisterRequest(RegisterRequest request, string passwordHash);
    }
}
