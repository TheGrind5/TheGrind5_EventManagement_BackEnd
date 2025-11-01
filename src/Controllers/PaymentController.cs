using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IVNPayService _vnPayService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IVNPayService vnPayService,
        IOrderService orderService,
        ILogger<PaymentController> logger)
    {
        _vnPayService = vnPayService;
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("vnpay/create")]
    [Authorize]
    public async Task<IActionResult> CreateVNPayPayment([FromBody] CreateVNPayPaymentRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            // Validate order ownership
            var order = await _orderService.GetOrderByIdAsync(request.OrderId);
            if (order == null)
                return NotFound(new { message = "Không tìm thấy order" });

            if (order.CustomerId != userId.Value)
                return Forbid("Bạn chỉ có thể thanh toán order của mình");

            if (order.Status != "Pending")
                return BadRequest(new { message = "Chỉ có thể thanh toán order đang Pending" });

            // Create payment
            var response = await _vnPayService.CreatePaymentAsync(request.OrderId, request.ReturnUrl ?? "");

            return Ok(new { 
                message = "Tạo payment thành công", 
                payment = response 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating VNPay payment");
            return BadRequest(new { message = "Có lỗi xảy ra khi tạo payment", error = ex.Message });
        }
    }

    [HttpPost("vnpay/webhook")]
    [HttpGet("vnpay/webhook")] // VNPay có thể gọi GET hoặc POST
    public async Task<IActionResult> VNPayWebhook()
    {
        try
        {
            _logger.LogInformation("VNPay webhook called");

            // Parse query parameters
            var webhookData = ParseWebhookData(Request);
            
            if (webhookData == null)
            {
                _logger.LogWarning("Failed to parse webhook data");
                return Ok("FAIL"); // VNPay expects plain text response
            }

            // Process webhook
            var success = await _vnPayService.ProcessWebhookAsync(webhookData);

            return Ok(success ? "SUCCESS" : "FAIL");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay webhook");
            return Ok("FAIL");
        }
    }

    [HttpGet("{paymentId}/status")]
    [Authorize]
    public async Task<IActionResult> GetPaymentStatus(int paymentId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var status = await _vnPayService.GetPaymentStatusAsync(paymentId);
            if (status == null)
                return NotFound(new { message = "Không tìm thấy payment" });

            // TODO: Validate user has access to this payment

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status");
            return BadRequest(new { message = "Có lỗi xảy ra", error = ex.Message });
        }
    }

    [HttpPost("{paymentId}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelPayment(int paymentId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var cancelled = await _vnPayService.CancelPaymentAsync(paymentId);
            if (!cancelled)
                return NotFound(new { message = "Không tìm thấy payment" });

            return Ok(new { message = "Hủy payment thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling payment");
            return BadRequest(new { message = "Có lỗi xảy ra", error = ex.Message });
        }
    }

    #region Helper Methods

    private VNPayWebhookData? ParseWebhookData(HttpRequest request)
    {
        try
        {
            return new VNPayWebhookData
            {
                vnp_TmnCode = request.Query["vnp_TmnCode"].ToString(),
                vnp_Amount = long.TryParse(request.Query["vnp_Amount"], out var amount) ? amount : 0,
                vnp_BankCode = request.Query["vnp_BankCode"].ToString(),
                vnp_BankTranNo = request.Query["vnp_BankTranNo"].ToString(),
                vnp_CardType = request.Query["vnp_CardType"].ToString(),
                vnp_PayDate = request.Query["vnp_PayDate"].ToString(),
                vnp_OrderInfo = request.Query["vnp_OrderInfo"].ToString(),
                vnp_TransactionNo = request.Query["vnp_TransactionNo"].ToString(),
                vnp_ResponseCode = request.Query["vnp_ResponseCode"].ToString(),
                vnp_TransactionStatus = request.Query["vnp_TransactionStatus"].ToString(),
                vnp_TxnRef = request.Query["vnp_TxnRef"].ToString(),
                vnp_SecureHash = request.Query["vnp_SecureHash"].ToString(),
                vnp_SecureHashType = request.Query["vnp_SecureHashType"].ToString(),
                vnp_CreateDate = request.Query["vnp_CreateDate"].ToString(),
                vnp_IpAddr = request.Query["vnp_IpAddr"].ToString(),
                vnp_CurrCode = request.Query["vnp_CurrCode"].ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing webhook data");
            return null;
        }
    }

    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    #endregion
}

