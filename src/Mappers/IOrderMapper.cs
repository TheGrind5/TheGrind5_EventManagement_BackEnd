using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Mappers
{
    public interface IOrderMapper
    {
        // Map Order entity thành OrderDTO
        OrderDTO MapToOrderDto(Order order);
        
        // Map Order entity thành OrderDetailDto (bao gồm OrderItems)
        OrderDetailDto MapToOrderDetailDto(Order order);
        
        // Map CreateOrderRequestDTO thành Order entity
        Order MapFromCreateOrderRequest(CreateOrderRequestDTO request, int customerId);
        
        // Map OrderItem entity thành OrderItemDTO
        OrderItemDTO MapToOrderItemDto(OrderItem orderItem);
        
        // Map CreateOrderResponseDTO từ Order entity
        CreateOrderResponseDTO MapToCreateOrderResponse(Order order);
    }
}
