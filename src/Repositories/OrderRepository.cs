using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EventDBContext _context;

        public OrderRepository(EventDBContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                // Đảm bảo CreatedAt được set
                if (order.CreatedAt == default)
                    order.CreatedAt = DateTime.UtcNow;
                
                // Đảm bảo Status được set
                if (string.IsNullOrEmpty(order.Status))
                    order.Status = "Pending";
                
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                
                // Set OrderId cho OrderItems sau khi order được tạo
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.OrderId = order.OrderId;
                }
                
                // Load related data
                await _context.Entry(order)
                    .Reference(o => o.Customer)
                    .LoadAsync();
                    
                await _context.Entry(order)
                    .Collection(o => o.OrderItems)
                    .LoadAsync();
                    
                // Load TicketType for each OrderItem
                foreach (var orderItem in order.OrderItems)
                {
                    await _context.Entry(orderItem)
                        .Reference(oi => oi.TicketType)
                        .LoadAsync();
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating order: {ex.Message}", ex);
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.TicketType)
                            .ThenInclude(tt => tt.Event)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order by ID: {ex.Message}", ex);
            }
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.TicketType)
                    .Where(o => o.CustomerId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting orders by user ID: {ex.Message}", ex);
            }
        }

        public async Task<Order?> UpdateOrderAsync(int orderId, Order order)
        {
            try
            {
                var existingOrder = await _context.Orders.FindAsync(orderId);
                if (existingOrder == null)
                    return null;

                existingOrder.Amount = order.Amount;
                existingOrder.Status = order.Status;
                existingOrder.PaymentMethod = order.PaymentMethod;
                existingOrder.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                // Reload with related data
                return await GetOrderByIdAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return false;

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting order: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return false;

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order status: {ex.Message}", ex);
            }
        }
    }
}
