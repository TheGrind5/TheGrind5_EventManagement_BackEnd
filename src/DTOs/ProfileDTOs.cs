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
            DateTime? UpdatedAt,
            string? Avatar,
            DateTime? DateOfBirth,
            string? Gender
        );

        public record UpdateProfileRequest(
            string? FullName = null,
            string? Phone = null,
            string? Avatar = null,
            DateTime? DateOfBirth = null,
            string? Gender = null
        );

        public record UpdateProfileResponse(
            string Message,
            ProfileDetailDto User
        );
    }
}
