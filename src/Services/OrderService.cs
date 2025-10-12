using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderMapper _orderMapper;
        private readonly EventDBContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderMapper orderMapper,
            EventDBContext context)
        {
            _orderRepository = orderRepository;
            _orderMapper = orderMapper;
            _context = context;
        }

        public async Task<CreateOrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request, int customerId)
        {
            try
            {
                // Validate request
                if (request.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                // Lấy thông tin ticket type để tính giá
                var ticketType = await _context.TicketTypes
                    .Include(tt => tt.Event)
                    .FirstOrDefaultAsync(tt => tt.TicketTypeId == request.TicketTypeId);

                if (ticketType == null)
                    throw new ArgumentException("Ticket type not found");

                // Kiểm tra event có tồn tại không
                if (ticketType.Event == null)
                    throw new ArgumentException("Event not found");

                // Tính toán giá
                var unitPrice = ticketType.Price;
                var totalAmount = unitPrice * request.Quantity;

                // Tạo order từ request
                var order = _orderMapper.MapFromCreateOrderRequest(request, customerId);
                
                // Set giá cho order
                order.Amount = totalAmount;

                // Tạo order trong database
                var createdOrder = await _orderRepository.CreateOrderAsync(order);

                // Load ticket type info cho response
                if (createdOrder.OrderItems.Any()) // Kiểm tra nếu có order items
                {
                    var orderItem = createdOrder.OrderItems.First(); // Lấy order item đầu tiên
                    await _context.Entry(orderItem)
                        .Reference(oi => oi.TicketType)
                        .LoadAsync();
                        
                    await _context.Entry(orderItem.TicketType)
                        .Reference(tt => tt.Event)
                        .LoadAsync();
                }

                // Map thành response DTO
                return _orderMapper.MapToCreateOrderResponse(createdOrder);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating order: {ex.Message}", ex);
            }
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null)
                    return null;

                return _orderMapper.MapToOrderDto(order);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order by ID: {ex.Message}", ex);
            }
        }

        public async Task<List<OrderDTO>> GetUserOrdersAsync(int userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                return orders.Select(_orderMapper.MapToOrderDto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user orders: {ex.Message}", ex);
            }
        }

        public async Task<Order?> GetOrderModelByIdAsync(int orderId)
        {
            try
            {
                return await _orderRepository.GetOrderByIdAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order model by ID: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                // Validate status
                var validStatuses = new[] { "Pending", "Paid", "Failed", "Cancelled", "Refunded" };
                if (!validStatuses.Contains(status))
                    throw new ArgumentException($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");

                return await _orderRepository.UpdateOrderStatusAsync(orderId, status);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order status: {ex.Message}", ex);
            }
        }

        // Mapping methods từ interface
        public OrderDTO MapToOrderDto(Order order)
        {
            return _orderMapper.MapToOrderDto(order);
        }

        public Order MapFromCreateOrderRequest(CreateOrderRequestDTO request, int customerId)
        {
            return _orderMapper.MapFromCreateOrderRequest(request, customerId);
        }
    }
}
