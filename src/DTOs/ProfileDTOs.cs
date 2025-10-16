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
        );

        public record UpdateProfileRequest(
            string? fullName = null,
            string? phone = null
        );

        public record UpdateProfileResponse(
            string message,
            ProfileDetailDto user
        );
    }
}
