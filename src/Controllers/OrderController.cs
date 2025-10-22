using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IWalletService _walletService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IWalletService walletService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _walletService = walletService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            try
            {
                _logger.LogInformation("CreateOrder request received: {Request}", request);
                
                var userId = GetUserIdFromToken();
                _logger.LogInformation("Extracted userId from token: {UserId}", userId);
                
                if (userId == null)
                {
                    _logger.LogWarning("No userId found in token");
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                // Validate user exists in database
                var userExists = await _orderService.ValidateUserExistsAsync(userId.Value);
                if (!userExists)
                {
                    _logger.LogWarning("User {UserId} does not exist in database", userId.Value);
                    return Unauthorized(new { message = "Người dùng không tồn tại trong hệ thống" });
                }

                if (!IsValidCreateOrderRequest(request))
                {
                    _logger.LogWarning("Invalid create order request: {Request}", request);
                    return BadRequest(new { message = "Dữ liệu order không hợp lệ" });
                }

                _logger.LogInformation("Creating order for user {UserId} with request {Request}", userId.Value, request);
                
                var result = await _orderService.CreateOrderAsync(request, userId.Value);
                
                _logger.LogInformation("Order created successfully: {OrderId}", result.OrderId);
                
                return Ok(new { 
                    message = "Tạo order thành công", 
                    order = result 
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument exception in CreateOrder: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId} with event {EventId}", 
                    GetUserIdFromToken(), request?.EventId);
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo order", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID order không hợp lệ" });

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Không tìm thấy order" });

                // Kiểm tra quyền truy cập - chỉ owner hoặc admin mới xem được
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                if (order.CustomerId != userId.Value)
                    return Forbid("Bạn chỉ có thể xem order của mình");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin order", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            try
            {
                var currentUserId = GetUserIdFromToken();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Chỉ cho phép xem orders của chính mình
                if (currentUserId.Value != userId)
                    return Forbid("Bạn chỉ có thể xem orders của mình");

                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(new { orders });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách orders", error = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var orders = await _orderService.GetUserOrdersAsync(userId.Value);
                return Ok(new { orders });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách orders", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID order không hợp lệ" });

                if (string.IsNullOrWhiteSpace(request.Status))
                    return BadRequest(new { message = "Status không được để trống" });

                var result = await _orderService.UpdateOrderStatusAsync(id, request.Status);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy order" });

                return Ok(new { message = "Cập nhật status thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật status", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID order không hợp lệ" });

                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu order
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Không tìm thấy order" });

                if (order.CustomerId != userId.Value)
                    return Forbid("Bạn chỉ có thể hủy order của mình");

                // Chỉ cho phép hủy order đang Pending
                if (order.Status != "Pending")
                    return BadRequest(new { message = "Chỉ có thể hủy order đang Pending" });

                var result = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
                if (!result)
                    return NotFound(new { message = "Không tìm thấy order" });

                return Ok(new { message = "Hủy order thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi hủy order", error = ex.Message });
            }
        }

        [HttpPost("{id}/payment")]
        [Authorize]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] PaymentRequest request)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID order không hợp lệ" });

                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu order
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = "Không tìm thấy order" });

                if (order.CustomerId != userId.Value)
                    return Forbid("Bạn chỉ có thể thanh toán order của mình");

                // Chỉ cho phép thanh toán order đang Pending
                if (order.Status != "Pending")
                    return BadRequest(new { message = "Chỉ có thể thanh toán order đang Pending" });

                // Process wallet payment
                if (request.PaymentMethod.ToLower() == "wallet")
                {
                    // Check if user has sufficient balance
                    var hasSufficientBalance = await _walletService.HasSufficientBalanceAsync(userId.Value, order.Amount);
                    if (!hasSufficientBalance)
                    {
                        var currentBalance = await _walletService.GetWalletBalanceAsync(userId.Value);
                        return BadRequest(new { 
                            message = "Số dư ví không đủ để thanh toán",
                            currentBalance = currentBalance,
                            requiredAmount = order.Amount,
                            shortfall = order.Amount - currentBalance
                        });
                    }

                    // Process payment from wallet
                    var walletTransaction = await _walletService.ProcessPaymentAsync(
                        userId.Value, 
                        order.Amount, 
                        id, 
                        $"Payment for order #{id}");

                    // Update order status to Paid
                    var result = await _orderService.UpdateOrderStatusAsync(id, "Paid");
                    if (!result)
                        return NotFound(new { message = "Không tìm thấy order" });

                    return Ok(new { 
                        message = "Thanh toán thành công", 
                        paymentMethod = request.PaymentMethod,
                        amount = order.Amount,
                        walletTransactionId = walletTransaction.TransactionId,
                        newWalletBalance = walletTransaction.BalanceAfter
                    });
                }
                else
                {
                    // TODO: Implement other payment methods (credit card, bank transfer, etc.)
                    return BadRequest(new { message = "Phương thức thanh toán không được hỗ trợ. Hiện tại chỉ hỗ trợ thanh toán qua ví." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi xử lý thanh toán", error = ex.Message });
            }
        }

        private bool IsValidCreateOrderRequest(CreateOrderRequestDTO request)
        {
            if (request == null)
            {
                _logger.LogWarning("CreateOrderRequest is null");
                return false;
            }

            if (request.EventId <= 0)
            {
                _logger.LogWarning("Invalid EventId: {EventId}", request.EventId);
                return false;
            }

            if (request.TicketTypeId <= 0)
            {
                _logger.LogWarning("Invalid TicketTypeId: {TicketTypeId}", request.TicketTypeId);
                return false;
            }

            if (request.Quantity <= 0)
            {
                _logger.LogWarning("Invalid Quantity: {Quantity}", request.Quantity);
                return false;
            }

            // 🔧 FIX: Thêm validation cho business rules
            if (request.Quantity > 100) // Reasonable limit
            {
                _logger.LogWarning("Quantity too high: {Quantity}", request.Quantity);
                return false;
            }

            // Validate SeatNo if provided
            if (!string.IsNullOrEmpty(request.SeatNo) && request.SeatNo.Length > 50)
            {
                _logger.LogWarning("SeatNo too long: {SeatNo}", request.SeatNo);
                return false;
            }

            return true;
        }

        [HttpPost("cleanup-expired")]
        [Authorize]
        public async Task<IActionResult> CleanupExpiredOrders()
        {
            try
            {
                var cleanedCount = await _orderService.CleanupExpiredOrdersAsync();
                
                return Ok(new { 
                    message = $"Đã cleanup {cleanedCount} orders hết hạn",
                    cleanedCount = cleanedCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cleanup orders", error = ex.Message });
            }
        }

        [HttpGet("inventory/{ticketTypeId}")]
        public async Task<IActionResult> GetTicketTypeInventory(int ticketTypeId)
        {
            try
            {
                if (ticketTypeId <= 0)
                    return BadRequest(new { message = "Ticket type ID không hợp lệ" });

                var inventory = await _orderService.GetTicketTypeInventoryAsync(ticketTypeId);
                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin inventory", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }

    // Helper DTOs cho requests
    public record UpdateOrderStatusRequest
    {
        public string Status { get; init; } = string.Empty;
    }

    public record PaymentRequest
    {
        public string PaymentMethod { get; init; } = string.Empty;
        public string TransactionId { get; init; } = string.Empty;
    }
}
