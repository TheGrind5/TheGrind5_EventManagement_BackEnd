using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Constants;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Helpers;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;

namespace TheGrind5_EventManagement.Services;

public class VNPayService : IVNPayService
{
    private readonly VNPaySettings _settings;
    private readonly IOrderService _orderService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<VNPayService> _logger;

    public VNPayService(
        IConfiguration configuration,
        IOrderService orderService,
        IPaymentRepository paymentRepository,
        ILogger<VNPayService> logger)
    {
        _settings = configuration.GetSection("VNPay").Get<VNPaySettings>() 
            ?? throw new InvalidOperationException("VNPay settings not found");
        _orderService = orderService;
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<CreateVNPayPaymentResponse> CreatePaymentAsync(int orderId, string returnUrl)
    {
        _logger.LogInformation("Creating VNPay payment for order {OrderId}", orderId);

        // Get order
        var order = await _orderService.GetOrderModelByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException($"Order {orderId} not found");

        if (order.Status != "Pending")
            throw new InvalidOperationException($"Order {orderId} is not in Pending status");

        // Generate TxnRef
        var txnRef = VNPayHelper.GenerateTxnRef(orderId);

        // Create payment record
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = order.Amount - order.DiscountAmount,
            Method = "VNPay",
            Status = VNPayConstants.STATUS_INITIATED,
            PaymentDate = DateTime.UtcNow,
            VnpTxnRef = txnRef,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.CreatePaymentAsync(payment);

        // Build payment URL
        var paymentUrl = CreatePaymentUrl(order, txnRef, returnUrl);

        // Generate QR code
        var qrCodeUrl = GenerateQrCode(paymentUrl);

        _logger.LogInformation("VNPay payment created: PaymentId={PaymentId}, TxnRef={TxnRef}", 
            payment.PaymentId, txnRef);

        return new CreateVNPayPaymentResponse
        {
            PaymentId = payment.PaymentId,
            PaymentUrl = paymentUrl,
            QrCodeUrl = qrCodeUrl,
            ExpiredAt = DateTime.UtcNow.AddMinutes(15) // VNPay default timeout
        };
    }

    public async Task<bool> ProcessWebhookAsync(VNPayWebhookData webhookData)
    {
        _logger.LogInformation("Processing VNPay webhook for TxnRef={TxnRef}", webhookData.vnp_TxnRef);

        try
        {
            // Validate signature first
            if (!ValidateSignature(webhookData))
            {
                _logger.LogWarning("Invalid signature for TxnRef={TxnRef}", webhookData.vnp_TxnRef);
                return false;
            }

            // Extract order ID
            var orderId = VNPayHelper.ExtractOrderIdFromTxnRef(webhookData.vnp_TxnRef);
            if (!orderId.HasValue)
            {
                _logger.LogError("Cannot extract OrderId from TxnRef={TxnRef}", webhookData.vnp_TxnRef);
                return false;
            }

            // Check if transaction already processed (idempotency)
            var existingPayment = await _paymentRepository.GetPaymentByTransactionIdAsync(webhookData.vnp_TransactionNo);
            if (existingPayment != null)
            {
                _logger.LogInformation("Transaction {TransactionId} already processed", webhookData.vnp_TransactionNo);
                return true;
            }

            // Get order
            var order = await _orderService.GetOrderModelByIdAsync(orderId.Value);
            if (order == null)
            {
                _logger.LogError("Order {OrderId} not found", orderId.Value);
                return false;
            }

            // Get payment
            var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId.Value);
            if (payment == null)
            {
                _logger.LogError("Payment not found for Order {OrderId}", orderId.Value);
                return false;
            }

            // Update payment
            payment.TransactionId = webhookData.vnp_TransactionNo;
            payment.ResponseCode = webhookData.vnp_ResponseCode;
            payment.TransactionStatus = webhookData.vnp_TransactionStatus;
            payment.UpdatedAt = DateTime.UtcNow;

            // Check if success
            var isSuccess = VNPayHelper.IsTransactionSuccess(
                webhookData.vnp_ResponseCode, 
                webhookData.vnp_TransactionStatus);

            if (isSuccess)
            {
                payment.Status = VNPayConstants.STATUS_SUCCEEDED;
                
                // Update order status
                await _orderService.UpdateOrderStatusAsync(orderId.Value, "Paid");
                
                _logger.LogInformation("Payment succeeded for Order {OrderId}", orderId.Value);
            }
            else
            {
                payment.Status = VNPayConstants.STATUS_FAILED;
                
                // Update order status
                await _orderService.UpdateOrderStatusAsync(orderId.Value, "Failed");
                
                _logger.LogWarning("Payment failed for Order {OrderId}, ResponseCode={ResponseCode}", 
                    orderId.Value, webhookData.vnp_ResponseCode);
            }

            await _paymentRepository.UpdatePaymentStatusAsync(
                payment.PaymentId, 
                payment.Status, 
                payment.TransactionId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook for TxnRef={TxnRef}", webhookData.vnp_TxnRef);
            return false;
        }
    }

    public async Task<PaymentStatusResponse?> GetPaymentStatusAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
        if (payment == null)
            return null;

        return new PaymentStatusResponse
        {
            PaymentId = payment.PaymentId,
            Status = payment.Status,
            TransactionId = payment.TransactionId
        };
    }

    public async Task<bool> CancelPaymentAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
        if (payment == null)
            return false;

        return await _paymentRepository.UpdatePaymentStatusAsync(
            paymentId, 
            VNPayConstants.STATUS_FAILED, 
            null);
    }

    public bool ValidateSignature(VNPayWebhookData webhookData)
    {
        return VNPayHelper.ValidateSignature(webhookData, _settings.HashSecret);
    }

    #region Private Methods

    private string CreatePaymentUrl(Order order, string txnRef, string returnUrl)
    {
        var parameters = new Dictionary<string, string>
        {
            { "vnp_Version", _settings.Version },
            { "vnp_Command", _settings.Command },
            { "vnp_TmnCode", _settings.TmnCode },
            { "vnp_Amount", VNPayHelper.ConvertAmountToVnPayFormat(order.Amount - order.DiscountAmount).ToString() },
            { "vnp_CreateDate", VNPayHelper.GetVnPayDateFormat() },
            { "vnp_CurrCode", _settings.CurrCode },
            { "vnp_IpAddr", "127.0.0.1" }, // TODO: Get actual IP from HttpContext
            { "vnp_Locale", _settings.Locale },
            { "vnp_OrderInfo", $"Thanh toan don hang #{order.OrderId}" },
            { "vnp_OrderType", _settings.OrderType },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_TxnRef", txnRef }
        };

        var queryString = VNPayHelper.BuildQueryString(parameters, _settings.HashSecret);
        return $"{_settings.PaymentUrl}?{queryString}";
    }

    private string GenerateQrCode(string paymentUrl)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            var base64String = Convert.ToBase64String(qrCodeBytes);
            return $"data:image/png;base64,{base64String}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating QR code");
            return string.Empty;
        }
    }

    #endregion
}

