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
    
    // DTO cho order detail (bao gồm items)
    public record OrderDetailDto
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Status { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<OrderItemDTO> OrderItems { get; init; } = new();
        public List<PaymentDTO> Payments { get; init; } = new();
    }
    
    // DTO cho payment info
    public record PaymentDTO
    {
        public int PaymentId { get; init; }
        public int OrderId { get; init; }
        public decimal Amount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public string TransactionId { get; init; } = string.Empty;
    }
}
