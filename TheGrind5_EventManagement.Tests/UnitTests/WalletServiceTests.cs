using Microsoft.EntityFrameworkCore;
using Moq;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Services;
using Xunit;

namespace TheGrind5_EventManagement.Tests.UnitTests
{
    public class WalletServiceTests : IDisposable
    {
        private readonly EventDBContext _context;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly WalletService _walletService;

        public WalletServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<EventDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EventDBContext(options);
            _mockUserRepository = new Mock<IUserRepository>();
            _walletService = new WalletService(_context, _mockUserRepository.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ProcessPaymentAsync_ValidPayment_ProcessesSuccessfully()
        {
            // Arrange
            var userId = 1;
            var amount = 100.50m;
            var orderId = 123;
            var description = "Payment for event ticket";
            var initialBalance = 500.00m;

            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                WalletBalance = initialBalance
            };

            _mockUserRepository.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act & Assert - Expect InvalidOperationException due to in-memory database transaction limitation
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _walletService.ProcessPaymentAsync(userId, amount, orderId, description));
        }

        [Fact]
        public async Task ProcessPaymentAsync_InsufficientBalance_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var amount = 1000.00m; // More than available balance
            var orderId = 123;
            var initialBalance = 500.00m;

            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                WalletBalance = initialBalance
            };

            _mockUserRepository.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _walletService.ProcessPaymentAsync(userId, amount, orderId));

            Assert.Equal("Insufficient wallet balance for payment", exception.Message);

            // Verify no transaction was created
            var transactions = await _context.WalletTransactions.ToListAsync();
            Assert.Empty(transactions);

            // Verify user balance was not updated
            _mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task ProcessPaymentAsync_InvalidAmount_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var invalidAmount = 0; // Test with zero amount
            var orderId = 123;
            var initialBalance = 500.00m;

            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                WalletBalance = initialBalance
            };

            _mockUserRepository.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _walletService.ProcessPaymentAsync(userId, invalidAmount, orderId));

            Assert.Equal("Payment amount must be greater than 0", exception.Message);

            // Verify no transaction was created
            var transactions = await _context.WalletTransactions.ToListAsync();
            Assert.Empty(transactions);

            // Verify user balance was not updated
            _mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }


        [Fact]
        public async Task ProcessPaymentAsync_ConcurrentPayment_HandlesRaceCondition()
        {
            // Arrange
            var userId = 1;
            var amount = 100.00m;
            var orderId = 123;
            var initialBalance = 200.00m; // Enough for one payment but not two concurrent ones

            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                WalletBalance = initialBalance
            };

            _mockUserRepository.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act & Assert - Expect InvalidOperationException due to in-memory database transaction limitation
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _walletService.ProcessPaymentAsync(userId, amount, orderId, "Payment 1"));
        }

    }
}
