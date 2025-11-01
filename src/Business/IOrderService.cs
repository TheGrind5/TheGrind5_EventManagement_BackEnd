using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    public interface IOrderService
    {
        // Tạo order mới
        Task<CreateOrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request, int customerId);
        
        // Lấy order theo ID
        Task<OrderDTO?> GetOrderByIdAsync(int orderId);
        
        // Lấy orders của user - original method (backward compatibility)
        Task<List<OrderDTO>> GetUserOrdersAsync(int userId);
        
        // Lấy orders của user - paginated
        Task<PagedResponse<OrderDTO>> GetUserOrdersAsync(int userId, PagedRequest request);
        
        // Lấy order theo ID (trả về Model)
        Task<Order?> GetOrderModelByIdAsync(int orderId);
        
        // Cập nhật order
        Task<bool> UpdateOrderAsync(int orderId, UpdateOrderRequest request);
        
        // Cập nhật order status
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        
        // Order cleanup methods
        Task<List<Order>> GetExpiredOrdersAsync();
        Task<int> CleanupExpiredOrdersAsync();
        
        // Inventory methods
        Task<object> GetTicketTypeInventoryAsync(int ticketTypeId);
        
        // User validation methods
        Task<bool> ValidateUserExistsAsync(int userId);
        
        // Mapping methods
        OrderDTO MapToOrderDto(Order order);
        Order MapFromCreateOrderRequest(CreateOrderRequestDTO request, int customerId);
    }

    // Helper DTO for UpdateOrder
    public record UpdateOrderRequest
    {
        public string? OrderAnswers { get; init; }
        public string? RecipientName { get; init; }
        public string? RecipientPhone { get; init; }
        public string? RecipientEmail { get; init; }
    }
}
