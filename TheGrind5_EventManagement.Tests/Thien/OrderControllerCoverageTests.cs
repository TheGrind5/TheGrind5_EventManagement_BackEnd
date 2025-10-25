using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Controllers;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Tests.Thien
{
    public class OrderControllerCoverageTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly Mock<ILogger<OrderController>> _mockLogger;
        private readonly OrderController _controller;

        public OrderControllerCoverageTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockWalletService = new Mock<IWalletService>();
            _mockLogger = new Mock<ILogger<OrderController>>();
            
            _controller = new OrderController(_mockOrderService.Object, _mockWalletService.Object, _mockLogger.Object);
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
        public async Task CreateOrder_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new CreateOrderRequestDTO
            {
                EventId = 1,
                TicketTypeId = 1,
                Quantity = 2
            };

            var response = new CreateOrderResponseDTO
            {
                OrderId = 1,
                CustomerId = 1,
                TotalAmount = 200.00m,
                Status = "Pending"
            };

            _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
                .ReturnsAsync(true);
            _mockOrderService.Setup(x => x.CreateOrderAsync(request, 1))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequestDTO
            {
                EventId = 0, // Invalid
                TicketTypeId = 1,
                Quantity = 2
            };

            _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateOrder_WithNullRequest_ShouldReturnBadRequest()
        {
            // Arrange
            CreateOrderRequestDTO request = null;

            _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidTicketTypeId_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequestDTO
            {
                EventId = 1,
                TicketTypeId = 0, // Invalid
                Quantity = 2
            };

            _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidQuantity_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequestDTO
            {
                EventId = 1,
                TicketTypeId = 1,
                Quantity = 0 // Invalid
            };

            _mockOrderService.Setup(x => x.ValidateUserExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        #endregion

        #region GetOrderById Tests

        [Fact]
        public async Task GetOrderById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var orderId = 1;
            var order = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = 1,
                Amount = 100.00m,
                Status = "Paid"
            };

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = okResult.Value as OrderDTO;
            Assert.Equal(orderId, responseData.OrderId);
        }

        #endregion

        #region GetUserOrders Tests

        [Fact]
        public async Task GetUserOrders_WithValidUserId_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            var orders = new List<OrderDTO>
            {
                new OrderDTO { OrderId = 1, CustomerId = userId, Amount = 100.00m },
                new OrderDTO { OrderId = 2, CustomerId = userId, Amount = 200.00m }
            };

            _mockOrderService.Setup(x => x.GetUserOrdersAsync(userId))
                .ReturnsAsync(orders);

            // Act
            var result = await _controller.GetUserOrders(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region UpdateOrderStatus Tests

        [Fact]
        public async Task UpdateOrderStatus_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var orderId = 1;
            var request = new UpdateOrderStatusRequest { Status = "Paid" };

            _mockOrderService.Setup(x => x.UpdateOrderStatusAsync(orderId, request.Status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }

        #endregion

        #region CancelOrder Tests

        [Fact]
        public async Task CancelOrder_WithValidOrder_ShouldReturnOk()
        {
            // Arrange
            var orderId = 1;
            var order = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = 1,
                Status = "Pending"
            };

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);
            _mockOrderService.Setup(x => x.UpdateOrderStatusAsync(orderId, "Cancelled"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CancelOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CancelOrder_WithNonPendingOrder_ShouldReturnBadRequest()
        {
            // Arrange
            var orderId = 1;
            var order = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = 1,
                Status = "Paid" // Not Pending
            };

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _controller.CancelOrder(orderId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CancelOrder_WithInvalidOrderId_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidOrderId = 0;

            // Act
            var result = await _controller.CancelOrder(invalidOrderId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CancelOrder_WithNonExistentOrder_ShouldReturnNotFound()
        {
            // Arrange
            var orderId = 999;

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((OrderDTO)null);

            // Act
            var result = await _controller.CancelOrder(orderId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = notFoundResult.Value as dynamic;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task CancelOrder_WithUnauthorizedUser_ShouldReturnForbid()
        {
            // Arrange
            var orderId = 1;
            var order = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = 2, // Different user
                Status = "Pending"
            };

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _controller.CancelOrder(orderId);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        #endregion

        #region ProcessPayment Tests - Removed (method not exists)

        #endregion

        #region CleanupExpiredOrders Tests

        [Fact]
        public async Task CleanupExpiredOrders_ShouldReturnOk()
        {
            // Arrange
            _mockOrderService.Setup(x => x.CleanupExpiredOrdersAsync())
                .ReturnsAsync(5);

            // Act
            var result = await _controller.CleanupExpiredOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
        }

        #endregion

        #region GetTicketTypeInventory Tests - Removed (method not exists)

        #endregion
    }
}