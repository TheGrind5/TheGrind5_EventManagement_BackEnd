using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public interface IOrderRepository
    {
        // Tạo order mới
        Task<Order> CreateOrderAsync(Order order);
        
        // Lấy order theo ID
        Task<Order?> GetOrderByIdAsync(int orderId);
        
        // Lấy orders của user
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        
        // Cập nhật order
        Task<Order?> UpdateOrderAsync(int orderId, Order order);
        
        // Xóa order
        Task<bool> DeleteOrderAsync(int orderId);
        
        // Cập nhật order status
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        
        // Lấy tất cả orders với filter và pagination (cho Admin)
        Task<List<Order>> GetAllOrdersAsync(
            string? searchTerm = null,
            string sortBy = "CreatedAt",
            string sortOrder = "desc",
            int skip = 0,
            int take = 10);
        
        // Đếm tổng số orders với filter (cho Admin)
        Task<int> GetTotalOrdersCountAsync(string? searchTerm = null);
    }
}
