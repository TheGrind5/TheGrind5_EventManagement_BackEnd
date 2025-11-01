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
                user.Phone ?? string.Empty,
                user.Role,
                user.WalletBalance,
                user.Avatar
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
                CreatedAt = DateTime.UtcNow,
                Avatar = request.Avatar,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            };
        }

        public ProfileDTOs.ProfileDetailDto MapToProfileDetailDto(User user)
        {
            return new ProfileDTOs.ProfileDetailDto(
                user.UserId,
                user.Username,
                user.FullName,
                user.Email,
                user.Phone ?? string.Empty,
                user.Role,
                user.CreatedAt,
                user.UpdatedAt,
                user.Avatar,
                user.DateOfBirth,
                user.Gender
            );
        }
    }
}


