using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Services
{
    // Helper class for raw SQL query result mapping
    public class UsedQuantityResult
    {
        public int UsedQuantity { get; set; }
    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderMapper _orderMapper;
        private readonly ITicketService _ticketService;
        private readonly IVoucherService _voucherService;
        private readonly EventDBContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderMapper orderMapper,
            ITicketService ticketService,
            IVoucherService voucherService,
            EventDBContext context)
        {
            _orderRepository = orderRepository;
            _orderMapper = orderMapper;
            _ticketService = ticketService;
            _voucherService = voucherService;
            _context = context;
        }

        public async Task<CreateOrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request, int customerId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate request
                if (request.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                // Check if ticket type exists before lock
                var ticketTypeExists = await _context.TicketTypes
                    .AnyAsync(tt => tt.TicketTypeId == request.TicketTypeId);
                
                if (!ticketTypeExists)
                {
                    throw new ArgumentException($"Ticket type {request.TicketTypeId} not found in database");
                }

                // 🔒 CRITICAL FIX: Lock ticket type row để tránh race condition
                var ticketType = await _context.TicketTypes
                    .FromSqlRaw("SELECT * FROM TicketType WITH (UPDLOCK, ROWLOCK) WHERE TicketTypeId = {0}", request.TicketTypeId)
                    .Include(tt => tt.Event)
                    .FirstOrDefaultAsync();

                
                if (ticketType == null)
                    throw new ArgumentException("Ticket type not found");

                // Kiểm tra event có tồn tại không
                if (ticketType.Event == null)
                    throw new ArgumentException("Event not found");

                // BUSINESS VALIDATION - Kiểm tra event status
                if (ticketType.Event.Status != "Open")
                    throw new ArgumentException($"Event is not available for booking. Current status: {ticketType.Event.Status}");

                // BUSINESS VALIDATION - Kiểm tra ticket type status
                if (ticketType.Status != "Active")
                    throw new ArgumentException("Ticket type is not active");

                // BUSINESS VALIDATION - Kiểm tra thời gian bán vé
                var now = DateTime.Now;
                if (now < ticketType.SaleStart)
                    throw new ArgumentException($"Ticket sales have not started yet. Sales start at: {ticketType.SaleStart:yyyy-MM-dd HH:mm}");
                
                if (now > ticketType.SaleEnd)
                    throw new ArgumentException($"Ticket sales have ended. Sales ended at: {ticketType.SaleEnd:yyyy-MM-dd HH:mm}");

                // BUSINESS VALIDATION - Kiểm tra MinOrder/MaxOrder
                if (ticketType.MinOrder.HasValue && request.Quantity < ticketType.MinOrder.Value)
                    throw new ArgumentException($"Minimum order quantity is {ticketType.MinOrder.Value}");

                if (ticketType.MaxOrder.HasValue && request.Quantity > ticketType.MaxOrder.Value)
                    throw new ArgumentException($"Maximum order quantity is {ticketType.MaxOrder.Value}");

                // 🔒 CRITICAL FIX: Kiểm tra inventory với lock để tránh race condition
                var availableQuantity = await GetAvailableQuantityWithLockAsync(ticketType.TicketTypeId);
                if (request.Quantity > availableQuantity)
                    throw new ArgumentException($"Not enough tickets available. Available: {availableQuantity}, Requested: {request.Quantity}");

                // Tính toán giá (subtotal)
                var unitPrice = ticketType.Price;
                var subTotalAmount = unitPrice * request.Quantity;

                // Validate và tính discount nếu có voucher
                var discountAmount = 0m;
                if (!string.IsNullOrWhiteSpace(request.VoucherCode))
                {
                    var voucherRequest = new VoucherValidationRequest
                    {
                        VoucherCode = request.VoucherCode,
                        OriginalAmount = subTotalAmount
                    };
                    
                    var voucherValidation = await _voucherService.ValidateVoucherAsync(voucherRequest);
                    
                    if (!voucherValidation.IsValid)
                        throw new ArgumentException(voucherValidation.Message);
                    
                    discountAmount = voucherValidation.DiscountAmount;
                }

                // Tính tổng cuối cùng = subtotal - discount
                var totalAmount = subTotalAmount - discountAmount;
                if (totalAmount < 0) totalAmount = 0;

                // Tạo order từ request
                var order = _orderMapper.MapFromCreateOrderRequest(request, customerId);
                
                // Set giá và voucher cho order
                order.Amount = totalAmount;
                order.DiscountAmount = discountAmount;
                order.VoucherCode = request.VoucherCode;

                // 🔒 CRITICAL FIX: Tạo order trong transaction với lock
                var createdOrder = await _orderRepository.CreateOrderAsync(order);

                // 🔒 CRITICAL FIX: Final inventory check trước khi commit
                // Lưu ý: Không cần kiểm tra lại inventory vì order đã được tạo và inventory đã được reserve
                // Việc kiểm tra inventory đã được thực hiện trước khi tạo order

                // Load ticket type info cho response
                await _context.Entry(createdOrder)
                    .Collection(o => o.OrderItems)
                    .LoadAsync();
                    
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

                // Commit transaction
                await transaction.CommitAsync();

                // Map thành response DTO
                return _orderMapper.MapToCreateOrderResponse(createdOrder);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

        // Original method - backward compatibility
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

        // New paginated method
        public async Task<PagedResponse<OrderDTO>> GetUserOrdersAsync(int userId, PagedRequest request)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.TicketType)
                            .ThenInclude(tt => tt.Event)
                    .Where(o => o.CustomerId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .AsQueryable();

                var totalCount = await query.CountAsync();
                
                var orders = await query
                    .Paginate(request)
                    .ToListAsync();

                var orderDtos = orders.Select(_orderMapper.MapToOrderDto).ToList();

                return new PagedResponse<OrderDTO>(orderDtos, totalCount, request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting paginated user orders: {ex.Message}", ex);
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

                var result = await _orderRepository.UpdateOrderStatusAsync(orderId, status);
                
                // If order is paid, create tickets
                if (result && status == "Paid")
                {
                    await CreateTicketsForOrderAsync(orderId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order status: {ex.Message}", ex);
            }
        }

        private async Task CreateTicketsForOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.TicketType)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    throw new ArgumentException("Order not found");

                foreach (var orderItem in order.OrderItems)
                {
                    await _ticketService.CreateTicketsForOrderItemAsync(
                        orderItem.OrderItemId, 
                        orderItem.Quantity, 
                        orderItem.TicketTypeId
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating tickets for order: {ex.Message}", ex);
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

        /// <summary>
        /// Tính số lượng vé còn lại cho một ticket type với LOCK để tránh race condition
        /// Bao gồm cả vé đã bán (Paid) và vé đang được reserve (Pending)
        /// </summary>
        private async Task<int> GetAvailableQuantityWithLockAsync(int ticketTypeId)
        {
            try
            {
                // 🔒 CRITICAL FIX: Sử dụng raw SQL với UPDLOCK để lock row
                var ticketType = await _context.TicketTypes
                    .FromSqlRaw("SELECT * FROM TicketType WITH (UPDLOCK, ROWLOCK) WHERE TicketTypeId = {0}", ticketTypeId)
                    .FirstOrDefaultAsync();
                
                if (ticketType == null)
                    return 0;

                // 🔒 CRITICAL FIX: Đếm số vé đã sử dụng với lock - sử dụng raw SQL để đảm bảo consistency
                var usedQuantityResult = await _context.Database
                    .SqlQueryRaw<UsedQuantityResult>("SELECT ISNULL(SUM(oi.Quantity), 0) AS UsedQuantity FROM OrderItem oi INNER JOIN [Order] o ON oi.OrderId = o.OrderId WHERE oi.TicketTypeId = {0} AND o.Status IN ('Paid', 'Pending')", ticketTypeId)
                    .FirstOrDefaultAsync();
                
                var usedQuantity = usedQuantityResult?.UsedQuantity ?? 0;

                // Tính số vé còn lại
                var availableQuantity = ticketType.Quantity - usedQuantity;
                return Math.Max(0, availableQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating available quantity with lock: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính số lượng vé còn lại cho một ticket type (DEPRECATED - chỉ dùng cho read-only operations)
        /// Bao gồm cả vé đã bán (Paid) và vé đang được reserve (Pending)
        /// </summary>
        [Obsolete("Use GetAvailableQuantityWithLockAsync for order creation to prevent race conditions")]
        private async Task<int> GetAvailableQuantityAsync(int ticketTypeId)
        {
            try
            {
                // Lấy tổng số lượng vé của ticket type
                var ticketType = await _context.TicketTypes
                    .FirstOrDefaultAsync(tt => tt.TicketTypeId == ticketTypeId);
                
                if (ticketType == null)
                    return 0;

                // Đếm số vé đã bán (Paid) + số vé đang được reserve (Pending)
                var usedQuantity = await _context.OrderItems
                    .Where(oi => oi.TicketTypeId == ticketTypeId)
                    .Where(oi => oi.Order.Status == "Paid" || oi.Order.Status == "Pending")
                    .SumAsync(oi => oi.Quantity);

                // Tính số vé còn lại
                var availableQuantity = ticketType.Quantity - usedQuantity;
                return Math.Max(0, availableQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating available quantity: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách orders đã hết hạn (Pending quá 15 phút)
        /// </summary>
        public async Task<List<Order>> GetExpiredOrdersAsync()
        {
            try
            {
                var expiredTime = DateTime.Now.AddMinutes(-15); // 15 phút trước
                
                var expiredOrders = await _context.Orders
                    .Where(o => o.Status == "Pending" && o.CreatedAt < expiredTime)
                    .ToListAsync();

                return expiredOrders;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting expired orders: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cleanup expired orders - tự động cancel orders đã hết hạn
        /// </summary>
        public async Task<int> CleanupExpiredOrdersAsync()
        {
            try
            {
                var expiredOrders = await GetExpiredOrdersAsync();
                var cleanedCount = 0;

                foreach (var order in expiredOrders)
                {
                    var result = await UpdateOrderStatusAsync(order.OrderId, "Cancelled");
                    if (result)
                    {
                        cleanedCount++;
                    }
                }

                return cleanedCount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cleaning up expired orders: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate user exists in database
        /// </summary>
        public async Task<bool> ValidateUserExistsAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                return user != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validating user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin inventory chi tiết cho một ticket type
        /// </summary>
        public async Task<object> GetTicketTypeInventoryAsync(int ticketTypeId)
        {
            try
            {
                var ticketType = await _context.TicketTypes
                    .FirstOrDefaultAsync(tt => tt.TicketTypeId == ticketTypeId);
                
                if (ticketType == null)
                    throw new ArgumentException("Ticket type not found");

                // Đếm số vé đã bán (Paid)
                var soldQuantity = await _context.OrderItems
                    .Where(oi => oi.TicketTypeId == ticketTypeId)
                    .Where(oi => oi.Order.Status == "Paid")
                    .SumAsync(oi => oi.Quantity);

                // Đếm số vé đang được reserve (Pending)
                var reservedQuantity = await _context.OrderItems
                    .Where(oi => oi.TicketTypeId == ticketTypeId)
                    .Where(oi => oi.Order.Status == "Pending")
                    .SumAsync(oi => oi.Quantity);

                // Tính số vé còn lại
                var availableQuantity = ticketType.Quantity - soldQuantity - reservedQuantity;

                return new
                {
                    ticketTypeId = ticketTypeId,
                    totalQuantity = ticketType.Quantity,
                    soldQuantity = soldQuantity,
                    reservedQuantity = reservedQuantity,
                    availableQuantity = Math.Max(0, availableQuantity),
                    isAvailable = availableQuantity > 0,
                    utilizationRate = ticketType.Quantity > 0 ? (double)(soldQuantity + reservedQuantity) / ticketType.Quantity * 100 : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting ticket type inventory: {ex.Message}", ex);
            }
        }
    }
}
