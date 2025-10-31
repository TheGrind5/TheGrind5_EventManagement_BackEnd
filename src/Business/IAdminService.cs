using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    /// <summary>
    /// Interface cho Admin Service - Quản lý người dùng
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Lấy danh sách users với filter và pagination
        /// </summary>
        Task<AdminDTOs.GetUsersResponse> GetAllUsersAsync(AdminDTOs.GetUsersRequest request);

        /// <summary>
        /// Lấy thống kê tổng quan về users
        /// </summary>
        Task<AdminDTOs.UserStatisticsDto> GetUserStatisticsAsync();

        /// <summary>
        /// Lấy thông tin chi tiết của một user
        /// </summary>
        Task<AdminDTOs.UserManagementDto?> GetUserByIdAsync(int userId);
    }
}


