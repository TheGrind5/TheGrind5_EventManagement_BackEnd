namespace TheGrind5_EventManagement.DTOs
{
    public class ProfileDTOs
    {
        public record ProfileDetailDto(
            int userId,
            string username,
            string fullName,
            string email,
            string phone,
            string role,
            DateTime createdAt,
            DateTime? updatedAt
            string? avatar,
            DateTime? dateOfBirth,
            string? gender
        );

        public record UpdateProfileRequest(
            string? fullName = null,
            string? phone = null
            string? avatar = null,
            DateTime? dateOfBirth = null,
            string? gender = null
        );

        public record UpdateProfileResponse(
            string message,
            ProfileDetailDto user
        );
    }
}
