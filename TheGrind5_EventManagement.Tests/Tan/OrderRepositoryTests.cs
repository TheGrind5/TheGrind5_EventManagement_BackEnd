using Microsoft.EntityFrameworkCore;
using Moq;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using Xunit;

namespace TheGrind5_EventManagement.Tests.UnitTests
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly EventDBContext _context;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<EventDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EventDBContext(options);
            _orderRepository = new OrderRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_CreatesOrder()
        {
            // Arrange
            var customerId = 1;
            var eventId = 1;
            var ticketTypeId = 1;

            // Create test data
            var customer = new User
            {
                UserId = customerId,
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };

            var eventEntity = new Event
            {
                EventId = eventId,
                Title = "Test Event",
                Description = "Test Description",
                StartTime = DateTime.UtcNow.AddDays(7),
                EndTime = DateTime.UtcNow.AddDays(8),
                Location = "Test Location",
                HostId = 1,
                CreatedAt = DateTime.UtcNow
            };

            var ticketType = new TicketType
            {
                TicketTypeId = ticketTypeId,
                EventId = eventId,
                TypeName = "VIP",
                Price = 100.00m,
                Quantity = 50,
                MinOrder = 1,
                MaxOrder = 5,
                SaleStart = DateTime.UtcNow.AddDays(-1),
                SaleEnd = DateTime.UtcNow.AddDays(6),
                Status = "Active"
            };

            // Add test data to context
            _context.Users.Add(customer);
            _context.Events.Add(eventEntity);
            _context.TicketTypes.Add(ticketType);
            await _context.SaveChangesAsync();

            var order = new Order
            {
                CustomerId = customerId,
                Amount = 200.00m,
                Status = "Pending",
                PaymentMethod = "Credit Card",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        TicketTypeId = ticketTypeId,
                        Quantity = 2,
                        SeatNo = "A1, A2",
                        Status = "Reserved"
                    }
                }
            };

            // Act
            var result = await _orderRepository.CreateOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.OrderId > 0); // OrderId should be assigned after creation
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(200.00m, result.Amount);
            Assert.Equal("Pending", result.Status);
            Assert.Equal("Credit Card", result.PaymentMethod);
            Assert.True(result.CreatedAt > DateTime.MinValue);

            // Verify OrderItems were created with correct OrderId
            Assert.Single(result.OrderItems);
            var orderItem = result.OrderItems.First();
            Assert.Equal(result.OrderId, orderItem.OrderId);
            Assert.Equal(ticketTypeId, orderItem.TicketTypeId);
            Assert.Equal(2, orderItem.Quantity);
            Assert.Equal("A1, A2", orderItem.SeatNo);
            Assert.Equal("Reserved", orderItem.Status);

            // Verify related data is loaded
            Assert.NotNull(result.Customer);
            Assert.Equal(customerId, result.Customer.UserId);
            Assert.Equal("testuser", result.Customer.Username);

            // Verify TicketType is loaded for OrderItem
            Assert.NotNull(orderItem.TicketType);
            Assert.Equal(ticketTypeId, orderItem.TicketType.TicketTypeId);
            Assert.Equal("VIP", orderItem.TicketType.TypeName);
            Assert.Equal(100.00m, orderItem.TicketType.Price);

            // Verify order was saved to database
            var savedOrder = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.TicketType)
                .FirstOrDefaultAsync(o => o.OrderId == result.OrderId);
            
            Assert.NotNull(savedOrder);
            Assert.Equal(result.OrderId, savedOrder.OrderId);
        }


        [Fact]
        public async Task CreateOrderAsync_TransactionRollback_OnError()
        {
            // Arrange
            var customerId = 1;
            var eventId = 1;
            var ticketTypeId = 1;

            // Create test data
            var customer = new User
            {
                UserId = customerId,
                Username = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };

            var eventEntity = new Event
            {
                EventId = eventId,
                Title = "Test Event",
                Description = "Test Description",
                StartTime = DateTime.UtcNow.AddDays(7),
                EndTime = DateTime.UtcNow.AddDays(8),
                Location = "Test Location",
                HostId = 1,
                CreatedAt = DateTime.UtcNow
            };

            var ticketType = new TicketType
            {
                TicketTypeId = ticketTypeId,
                EventId = eventId,
                TypeName = "VIP",
                Price = 100.00m,
                Quantity = 50,
                SaleStart = DateTime.UtcNow.AddDays(-1),
                SaleEnd = DateTime.UtcNow.AddDays(6),
                Status = "Active"
            };

            _context.Users.Add(customer);
            _context.Events.Add(eventEntity);
            _context.TicketTypes.Add(ticketType);
            await _context.SaveChangesAsync();

            var order = new Order
            {
                CustomerId = customerId,
                Amount = 200.00m,
                Status = "Pending",
                PaymentMethod = "Credit Card",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        TicketTypeId = ticketTypeId,
                        Quantity = 2,
                        SeatNo = "A1, A2",
                        Status = "Reserved"
                    }
                }
            };

            // Mock SaveChangesAsync to throw an exception
            var mockContext = new Mock<EventDBContext>(new DbContextOptionsBuilder<EventDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);
            
            var mockOrderRepository = new OrderRepository(mockContext.Object);

            // Setup the mock to throw exception on SaveChangesAsync
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => mockOrderRepository.CreateOrderAsync(order));

            Assert.Contains("Error creating order", exception.Message);

            // Verify that no order was actually saved to the real database
            var ordersInDb = await _context.Orders.ToListAsync();
            Assert.Empty(ordersInDb);
        }

    }
}
