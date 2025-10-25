using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using TheGrind5_EventManagement.Controllers;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Tests.UnitTests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly Mock<ILogger<OrderController>> _mockLogger;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrderController>>();
            _mockWalletService = new Mock<IWalletService>();
            _controller = new OrderController(_mockOrderService.Object, _mockWalletService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetOrderById_Success_ReturnsOrder()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var expectedOrder = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = userId,
                CustomerName = "Test User",
                CustomerEmail = "test@example.com",
                Amount = 100.00m,
                Status = "Paid",
                PaymentMethod = "Credit Card",
                CreatedAt = DateTime.UtcNow
            };

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(expectedOrder);

            // Setup JWT token claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderDTO>(okResult.Value);
            Assert.Equal(expectedOrder.OrderId, returnedOrder.OrderId);
            Assert.Equal(expectedOrder.CustomerId, returnedOrder.CustomerId);
            Assert.Equal(expectedOrder.Amount, returnedOrder.Amount);
            Assert.Equal(expectedOrder.Status, returnedOrder.Status);
        }

        [Fact]
        public async Task GetOrderById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var orderId = 999;
            var userId = 1;

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync((OrderDTO?)null);

            // Setup JWT token claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetOrderById_UnauthorizedUser_ReturnsForbid()
        {
            // Arrange
            var orderId = 1;
            var orderOwnerId = 1;
            var unauthorizedUserId = 2;
            var expectedOrder = new OrderDTO
            {
                OrderId = orderId,
                CustomerId = orderOwnerId,
                CustomerName = "Test User",
                CustomerEmail = "test@example.com",
                Amount = 100.00m,
                Status = "Paid",
                PaymentMethod = "Credit Card",
                CreatedAt = DateTime.UtcNow
            };

            _mockOrderService.Setup(x => x.GetOrderByIdAsync(orderId))
                .ReturnsAsync(expectedOrder);

            // Setup JWT token claims for unauthorized user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, unauthorizedUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result);
            Assert.NotNull(forbidResult);
        }

    }
}
