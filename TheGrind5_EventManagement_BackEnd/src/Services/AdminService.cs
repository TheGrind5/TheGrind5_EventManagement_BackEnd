using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Repositories;

namespace TheGrind5_EventManagement.Services
{
    /// <summary>
    /// Service xử lý business logic cho Admin - Quản lý người dùng
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IUserRepository userRepository, ILogger<AdminService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AdminDTOs.GetUsersResponse> GetAllUsersAsync(AdminDTOs.GetUsersRequest request)
        {
            try
            {
                // Validation
                if (request.PageNumber < 1) request = request with { PageNumber = 1 };
                if (request.PageSize < 1) request = request with { PageSize = 10 };
                if (request.PageSize > 100) request = request with { PageSize = 100 }; // Max 100 items per page

                // Calculate skip
                var skip = (request.PageNumber - 1) * request.PageSize;

                // Get users from repository
                var users = await _userRepository.GetAllUsersAsync(
                    role: request.Role,
                    searchTerm: request.SearchTerm,
                    sortBy: request.SortBy,
                    sortOrder: request.SortOrder,
                    skip: skip,
                    take: request.PageSize
                );

                // Get total count
                var totalCount = await _userRepository.GetTotalUsersCountAsync(
                    role: request.Role,
                    searchTerm: request.SearchTerm
                );

                // Calculate total pages
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                // Map to DTO
                var userDtos = users.Select(u => new AdminDTOs.UserManagementDto(
                    UserId: u.UserId,
                    Username: u.Username,
                    FullName: u.FullName,
                    Email: u.Email,
                    Phone: u.Phone,
                    Role: u.Role,
                    WalletBalance: u.WalletBalance,
                    CreatedAt: u.CreatedAt,
                    UpdatedAt: u.UpdatedAt,
                    Avatar: u.Avatar,
                    DateOfBirth: u.DateOfBirth,
                    Gender: u.Gender
                )).ToList();

                return new AdminDTOs.GetUsersResponse(
                    Users: userDtos,
                    TotalCount: totalCount,
                    PageNumber: request.PageNumber,
                    PageSize: request.PageSize,
                    TotalPages: totalPages
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users list for admin");
                throw;
            }
        }

        public async Task<AdminDTOs.UserStatisticsDto> GetUserStatisticsAsync()
        {
            try
            {
                var stats = await _userRepository.GetUserStatisticsAsync();

                // Get total wallet balance (sum of all users' balance)
                var totalWalletBalance = await GetTotalWalletBalanceAsync();

                return new AdminDTOs.UserStatisticsDto(
                    TotalUsers: stats["TotalUsers"],
                    TotalHosts: stats["TotalHosts"],
                    TotalCustomers: stats["TotalCustomers"],
                    TotalAdmins: stats["TotalAdmins"],
                    NewUsersThisMonth: stats["NewUsersThisMonth"],
                    TotalWalletBalance: totalWalletBalance
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                throw;
            }
        }

        public async Task<AdminDTOs.UserManagementDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) return null;

                return new AdminDTOs.UserManagementDto(
                    UserId: user.UserId,
                    Username: user.Username,
                    FullName: user.FullName,
                    Email: user.Email,
                    Phone: user.Phone,
                    Role: user.Role,
                    WalletBalance: user.WalletBalance,
                    CreatedAt: user.CreatedAt,
                    UpdatedAt: user.UpdatedAt,
                    Avatar: user.Avatar,
                    DateOfBirth: user.DateOfBirth,
                    Gender: user.Gender
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId} details", userId);
                throw;
            }
        }

        // Helper method
        private async Task<decimal> GetTotalWalletBalanceAsync()
        {
            var allUsers = await _userRepository.GetAllUsersAsync(skip: 0, take: int.MaxValue);
            return allUsers.Sum(u => u.WalletBalance);
        }
    }
}

