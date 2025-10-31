namespace TheGrind5_EventManagement.DTOs
{
    /// <summary>
    /// DTOs cho Admin Management - Quản lý người dùng
    /// </summary>
    public class AdminDTOs
    {
        /// <summary>
        /// DTO hiển thị thông tin user trong danh sách Admin
        /// </summary>
        public record UserManagementDto(
            int UserId,
            string Username,
            string FullName,
            string Email,
            string? Phone,
            string Role,
            decimal WalletBalance,
            DateTime CreatedAt,
            DateTime? UpdatedAt,
            string? Avatar,
            DateTime? DateOfBirth,
            string? Gender
        );

        /// <summary>
        /// Request để lấy danh sách users với filter và pagination
        /// </summary>
        public record GetUsersRequest(
            string? Role = null,          // Filter theo role: "Host", "Customer", null = all
            string? SearchTerm = null,     // Tìm kiếm theo name/email
            int PageNumber = 1,
            int PageSize = 10,
            string SortBy = "CreatedAt",   // CreatedAt, FullName, Email, WalletBalance
            string SortOrder = "desc"      // asc hoặc desc
        );

        /// <summary>
        /// Response trả về danh sách users với pagination
        /// </summary>
        public record GetUsersResponse(
            List<UserManagementDto> Users,
            int TotalCount,
            int PageNumber,
            int PageSize,
            int TotalPages
        );

        /// <summary>
        /// Statistics cho Admin Dashboard
        /// </summary>
        public record UserStatisticsDto(
            int TotalUsers,
            int TotalHosts,
            int TotalCustomers,
            int TotalAdmins,
            int NewUsersThisMonth,
            decimal TotalWalletBalance
        );

        /// <summary>
        /// Request để update user status (active/inactive)
        /// </summary>
        public record UpdateUserStatusRequest(
            int UserId,
            bool IsActive
        );

        /// <summary>
        /// Response khi update thành công
        /// </summary>
        public record UpdateUserStatusResponse(
            bool Success,
            string Message
        );
    }
}


