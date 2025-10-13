namespace TheGrind5_EventManagement.DTOs
{
    public class ProfileDTOs
    {
        public record ProfileDetailDto(
            int UserId,
            string Username,
            string FullName,
            string Email,
            string Phone,
            string Role,
            DateTime CreatedAt,
            DateTime? UpdatedAt
        );

        public record UpdateProfileRequest(
            string? FullName = null,
            string? Phone = null
        );

        public record UpdateProfileResponse(
            string Message,
            ProfileDetailDto User
        );
    }
}
