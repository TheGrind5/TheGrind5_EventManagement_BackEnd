using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Mappers
{
    public class OrderMapper : IOrderMapper
    {
        public OrderDTO MapToOrderDto(Order order)
        {
            try
            {
                return new OrderDTO
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.FullName ?? string.Empty,
                    CustomerEmail = order.Customer?.Email ?? string.Empty,
                    Amount = order.Amount,
                    VoucherCode = order.VoucherCode,
                    DiscountAmount = order.DiscountAmount,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod ?? string.Empty,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    OrderItems = order.OrderItems?.Select(MapToOrderItemDto).ToList() ?? new List<OrderItemDTO>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error mapping order to DTO: {ex.Message}", ex);
            }
        }

        public OrderDetailDto MapToOrderDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                CustomerEmail = order.Customer?.Email ?? string.Empty,
                Amount = order.Amount,
                VoucherCode = order.VoucherCode,
                DiscountAmount = order.DiscountAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod ?? string.Empty,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems?.Select(MapToOrderItemDto).ToList() ?? new List<OrderItemDTO>(),
                Payments = new List<PaymentDTO>()
            };
        }

        public Order MapFromCreateOrderRequest(CreateOrderRequestDTO request, int customerId)
        {
            var order = new Order
            {
                CustomerId = customerId,
                Amount = 0, // Sẽ được tính toán trong service
                Status = "Pending",
                PaymentMethod = string.Empty,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            // Tạo OrderItem từ request
            var orderItem = new OrderItem
            {
                TicketTypeId = request.TicketTypeId,
                Quantity = request.Quantity,
                SeatNo = request.SeatNo ?? string.Empty, // Đảm bảo không null
                Status = "Reserved"
            };

            order.OrderItems.Add(orderItem);

            return order;
        }

        public OrderItemDTO MapToOrderItemDto(OrderItem orderItem)
        {
            try
            {
                // Tính toán giá từ TicketType nếu không có UnitPrice
                var unitPrice = orderItem.TicketType?.Price ?? 0;
                var totalPrice = unitPrice * orderItem.Quantity;
                
                return new OrderItemDTO
                {
                    OrderItemId = orderItem.OrderItemId,
                    TicketTypeId = orderItem.TicketTypeId,
                    TicketTypeName = orderItem.TicketType?.TypeName ?? string.Empty,
                    EventTitle = orderItem.TicketType?.Event?.Title ?? string.Empty,
                    Quantity = orderItem.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    SeatNo = orderItem.SeatNo,
                    Status = orderItem.Status
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error mapping order item to DTO: {ex.Message}", ex);
            }
        }

        public CreateOrderResponseDTO MapToCreateOrderResponse(Order order)
        {
            try
            {
                var firstOrderItem = order.OrderItems?.FirstOrDefault();
                var ticketType = firstOrderItem?.TicketType;

                var unitPrice = ticketType?.Price ?? 0;
                var quantity = firstOrderItem?.Quantity ?? 0;
                var subtotal = unitPrice * quantity;

                return new CreateOrderResponseDTO
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    EventId = firstOrderItem?.TicketType?.EventId ?? 0,
                    EventTitle = ticketType?.Event?.Title ?? string.Empty,
                    TicketTypeName = ticketType?.TypeName ?? string.Empty,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    SubTotalAmount = subtotal,
                    VoucherCode = order.VoucherCode,
                    DiscountAmount = order.DiscountAmount,
                    TotalAmount = order.Amount,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    Message = "Order created successfully"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error mapping to CreateOrderResponse: {ex.Message}", ex);
            }
        }
    }
}
