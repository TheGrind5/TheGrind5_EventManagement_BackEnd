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

        public OrderController(IOrderService orderService, IWalletService walletService)
        {
            _orderService = orderService;
            _walletService = walletService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                if (!IsValidCreateOrderRequest(request))
                    return BadRequest(new { message = "Dữ liệu order không hợp lệ" });

                var result = await _orderService.CreateOrderAsync(request, userId.Value);
                
                return Ok(new { 
                    message = "Tạo order thành công", 
                    order = result 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
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
            return request != null &&
                   request.EventId > 0 &&
                   request.TicketTypeId > 0 &&
                   request.Quantity > 0;
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
