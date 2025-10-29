using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Controllers;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;
using TheGrind5.Tests.TestHelpers;

namespace TheGrind5.Tests.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IWalletService> _mockWalletService;
    private readonly Mock<ILogger<OrderController>> _mockLogger;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _mockWalletService = new Mock<IWalletService>();
        _mockLogger = new Mock<ILogger<OrderController>>();
        
        _controller = new OrderController(_mockOrderService.Object, _mockWalletService.Object, _mockLogger.Object);
        
        // Setup controller context for authorization
        SetupControllerContext();
    }

    private void SetupControllerContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    #region CreateOrder Tests

    [Fact]
    public async Task CreateOrder_Success_ReturnsOk()
    {
        // Arrange
        var request = new CreateOrderRequestDTO
        {
            EventId = 1,
            TicketTypeId = 1,
            Quantity = 2,
            SeatNo = "A1"
        };

        var expectedResponse = new CreateOrderResponseDTO
        {
            OrderId = 1,
            EventId = 1,
            CustomerId = 1,
            TotalAmount = 200,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
            .ReturnsAsync(true);
        _mockOrderService.Setup(x => x.CreateOrderAsync(request, 1))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        
        Assert.NotNull(response);
        _mockOrderService.Verify(x => x.ValidateUserExistsAsync(1), Times.Once);
        _mockOrderService.Verify(x => x.CreateOrderAsync(request, 1), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateOrderRequestDTO
        {
            EventId = 1,
            TicketTypeId = 1,
            Quantity = 2
        };

        // Setup controller context without user claims
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal()
            }
        };

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value;
        Assert.Contains("Token không hợp lệ", response.ToString());
    }

    [Fact]
    public async Task CreateOrder_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequestDTO
        {
            EventId = 0, // Invalid EventId
            TicketTypeId = 1,
            Quantity = 2
        };

        _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value;
        Assert.Contains("Dữ liệu order không hợp lệ", response.ToString());
    }

    [Fact]
    public async Task CreateOrder_UserNotExists_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateOrderRequestDTO
        {
            EventId = 1,
            TicketTypeId = 1,
            Quantity = 2
        };

        _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = unauthorizedResult.Value;
        Assert.Contains("Người dùng không tồn tại trong hệ thống", response.ToString());
    }

    [Fact]
    public async Task CreateOrder_ConcurrentPurchase_HandlesRaceCondition()
    {
        // Arrange
        var request = new CreateOrderRequestDTO
        {
            EventId = 1,
            TicketTypeId = 1,
            Quantity = 1
        };

        _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
            .ReturnsAsync(true);

        // Simulate race condition - first call succeeds, second fails
        _mockOrderService.SetupSequence(x => x.CreateOrderAsync(request, 1))
            .ReturnsAsync(new CreateOrderResponseDTO 
            { 
                OrderId = 1, 
                EventId = 1, 
                CustomerId = 1, 
                TotalAmount = 100, 
                Status = "Pending" 
            })
            .ThrowsAsync(new InvalidOperationException("Ticket no longer available"));

        // Act - First call should succeed
        var result1 = await _controller.CreateOrder(request);
        var okResult1 = Assert.IsType<OkObjectResult>(result1);

        // Act - Second call should handle the race condition gracefully
        var result2 = await _controller.CreateOrder(request);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result2);

        // Assert
        Assert.NotNull(okResult1.Value);
        Assert.Contains("Có lỗi xảy ra khi tạo order", badRequestResult.Value.ToString());
    }

    #endregion

    #region ProcessPayment Tests

    [Fact]
    public async Task ProcessPayment_Wallet_Success()
    {
        // Arrange
        var orderId = 1;
        var request = new PaymentRequest
        {
            PaymentMethod = "wallet",
            TransactionId = ""
        };

        var order = new OrderDTO
        {
            OrderId = orderId,
            CustomerId = 1,
            Amount = 100,
            Status = "Pending"
        };

        var walletTransaction = new WalletTransaction
        {
            TransactionId = 1,
            UserId = 1,
            Amount = 100,
            BalanceAfter = 400,
            TransactionType = "Payment",
            Status = "Completed",
            CreatedAt = DateTime.UtcNow
        };

        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(order);
        _mockWalletService.Setup(x => x.HasSufficientBalanceAsync(1, 100))
            .ReturnsAsync(true);
        _mockWalletService.Setup(x => x.ProcessPaymentAsync(1, 100, orderId, It.IsAny<string>()))
            .ReturnsAsync(walletTransaction);
        _mockOrderService.Setup(x => x.UpdateOrderStatusAsync(orderId, "Paid"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ProcessPayment(orderId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        
        Assert.NotNull(response);
        _mockWalletService.Verify(x => x.HasSufficientBalanceAsync(1, 100), Times.Once);
        _mockWalletService.Verify(x => x.ProcessPaymentAsync(1, 100, orderId, It.IsAny<string>()), Times.Once);
        _mockOrderService.Verify(x => x.UpdateOrderStatusAsync(orderId, "Paid"), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_InsufficientBalance_ReturnsBadRequest()
    {
        // Arrange
        var orderId = 1;
        var request = new PaymentRequest
        {
            PaymentMethod = "wallet",
            TransactionId = ""
        };

        var order = new OrderDTO
        {
            OrderId = orderId,
            CustomerId = 1,
            Amount = 1000, // High amount
            Status = "Pending"
        };

        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(order);
        _mockWalletService.Setup(x => x.HasSufficientBalanceAsync(1, 1000))
            .ReturnsAsync(false);
        _mockWalletService.Setup(x => x.GetWalletBalanceAsync(1))
            .ReturnsAsync(500); // Insufficient balance

        // Act
        var result = await _controller.ProcessPayment(orderId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value;
        
        Assert.Contains("Số dư ví không đủ để thanh toán", response.ToString());
        _mockWalletService.Verify(x => x.HasSufficientBalanceAsync(1, 1000), Times.Once);
        _mockWalletService.Verify(x => x.GetWalletBalanceAsync(1), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_OrderNotFound_ReturnsNotFound()
    {
        // Arrange
        var orderId = 999;
        var request = new PaymentRequest
        {
            PaymentMethod = "wallet",
            TransactionId = ""
        };

        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync((OrderDTO)null);

        // Act
        var result = await _controller.ProcessPayment(orderId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = notFoundResult.Value;
        Assert.Contains("Không tìm thấy order", response.ToString());
    }

    [Fact]
    public async Task ProcessPayment_UnauthorizedUser_ReturnsForbid()
    {
        // Arrange
        var orderId = 1;
        var request = new PaymentRequest
        {
            PaymentMethod = "wallet",
            TransactionId = ""
        };

        var order = new OrderDTO
        {
            OrderId = orderId,
            CustomerId = 2, // Different user
            Amount = 100,
            Status = "Pending"
        };

        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _controller.ProcessPayment(orderId, request);

        // Assert
        var forbidResult = Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task ProcessPayment_OrderNotPending_ReturnsBadRequest()
    {
        // Arrange
        var orderId = 1;
        var request = new PaymentRequest
        {
            PaymentMethod = "wallet",
            TransactionId = ""
        };

        var order = new OrderDTO
        {
            OrderId = orderId,
            CustomerId = 1,
            Amount = 100,
            Status = "Paid" // Already paid
        };

        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _controller.ProcessPayment(orderId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value;
        Assert.Contains("Chỉ có thể thanh toán order đang Pending", response.ToString());
    }

    [Fact]
    public async Task ProcessPayment_InvalidOrderId_ReturnsBadRequest()
    {
        // Arrange
        var invalidOrderId = 0;
        var request = new PaymentRequest
        {
            PaymentMethod = "wallet",
            TransactionId = ""
        };

        // Act
        var result = await _controller.ProcessPayment(invalidOrderId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value;
        Assert.Contains("ID order không hợp lệ", response.ToString());
    }

    [Fact]
    public async Task ProcessPayment_UnsupportedPaymentMethod_ReturnsBadRequest()
    {
        // Arrange
        var orderId = 1;
        var request = new PaymentRequest
        {
            PaymentMethod = "credit_card", // Unsupported method
            TransactionId = ""
        };

        var order = new OrderDTO
        {
            OrderId = orderId,
            CustomerId = 1,
            Amount = 100,
            Status = "Pending"
        };

        _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _controller.ProcessPayment(orderId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = badRequestResult.Value;
        Assert.Contains("Phương thức thanh toán không được hỗ trợ", response.ToString());
    }

    #endregion
}
