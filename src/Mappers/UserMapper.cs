using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Mappers
{
    public class UserMapper : IUserMapper
    {
        public AuthDTOs.UserReadDto MapToUserReadDto(User user)
        {
            return new AuthDTOs.UserReadDto(
                user.UserId,
                user.FullName,
                user.Email,
                user.Phone,
                user.Role,
                user.WalletBalance,
                user.AvatarUrl
            );
        }

        public User MapFromRegisterRequest(RegisterRequest request, string passwordHash)
        {
            return new User
            {
                Username = request.Username,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Phone = request.Phone ?? "",
                Role = "Customer", // Default role
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}


