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
        
        // Lấy orders của user
        Task<List<OrderDTO>> GetUserOrdersAsync(int userId);
        
        // Lấy order theo ID (trả về Model)
        Task<Order?> GetOrderModelByIdAsync(int orderId);
        
        // Cập nhật order status
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        
        // Mapping methods
        OrderDTO MapToOrderDto(Order order);
        Order MapFromCreateOrderRequest(CreateOrderRequestDTO request, int customerId);
    }
}
