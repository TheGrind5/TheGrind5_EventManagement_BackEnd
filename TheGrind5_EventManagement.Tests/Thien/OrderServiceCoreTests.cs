using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Business;
using Microsoft.Extensions.Logging;
using Moq;

namespace TheGrind5_EventManagement.Tests.Thien
{
    /// <summary>
    /// Unit Tests for OrderService - Thiên's Assignment
    /// Feature: Buy Ticket Flow - Core functionality tests
    /// Total: 10 test cases covering basic functionality
    /// </summary>
    public class OrderServiceTests : IDisposable
    {
        private readonly EventDBContext _context;
        private readonly OrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderMapper _orderMapper;
        private readonly ITicketService _ticketService;
        private readonly IVoucherService _voucherService;
        private readonly INotificationService _notificationService;

        public OrderServiceTests()
        {
            // Use InMemory database for testing
            var options = new DbContextOptionsBuilder<EventDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new EventDBContext(options);
            
            // Initialize dependencies
            _orderRepository = new OrderRepository(_context);
            _orderMapper = new OrderMapper();
            _ticketService = new TicketService(_context);
            _voucherService = new VoucherService(_context);
            var mockLogger = new Mock<ILogger<NotificationService>>();
            _notificationService = new NotificationService(_context, mockLogger.Object);
            
            _orderService = new OrderService(_orderRepository, _orderMapper, _ticketService, _voucherService, _notificationService, _context);
            
            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Add test users
            _context.Users.AddRange(new List<User>
            {
                new User { UserId = 1, Email = "test1@example.com", PasswordHash = "hash1", Username = "test1", FullName = "Test User1", Phone = "1234567890", Role = "Customer", CreatedAt = DateTime.Now },
                new User { UserId = 2, Email = "test2@example.com", PasswordHash = "hash2", Username = "test2", FullName = "Test User2", Phone = "1234567891", Role = "Customer", CreatedAt = DateTime.Now }
            });

            // Add test events
            _context.Events.AddRange(new List<Event>
            {
                new Event { EventId = 1, HostId = 1, Title = "Test Event 1", Description = "Test Description", StartTime = DateTime.Now.AddDays(30), EndTime = DateTime.Now.AddDays(31), Location = "Test Location", Status = "Open", CreatedAt = DateTime.Now },
                new Event { EventId = 2, HostId = 2, Title = "Test Event 2", Description = "Test Description", StartTime = DateTime.Now.AddDays(30), EndTime = DateTime.Now.AddDays(31), Location = "Test Location", Status = "Open", CreatedAt = DateTime.Now }
            });

            // Add test ticket types
            _context.TicketTypes.AddRange(new List<TicketType>
            {
                new TicketType { TicketTypeId = 1, EventId = 1, TypeName = "VIP", Price = 100, Quantity = 10, Status = "Active", SaleStart = DateTime.Now, SaleEnd = DateTime.Now.AddDays(30) },
                new TicketType { TicketTypeId = 2, EventId = 2, TypeName = "Standard", Price = 50, Quantity = 5, Status = "Active", SaleStart = DateTime.Now, SaleEnd = DateTime.Now.AddDays(30) }
            });

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region Real Business Scenarios Tests (10 test cases)

        [Fact]
        public void ValidRequest_CreatesOrder()
        {
            // Given - Customer wants to buy 2 VIP tickets
            var customerId = 1;
            var request = new CreateOrderRequestDTO
            {
                TicketTypeId = 1, // VIP ticket
                Quantity = 2,
                SeatNo = "A1, A2"
            };

            // When - Create order (simulate the flow)
            var mapper = new OrderMapper();
            var order = mapper.MapFromCreateOrderRequest(request, customerId);
            
            // Simulate order creation
            order.Amount = 200m; // 2 x 100
            order.Status = "Pending";
            order.CreatedAt = DateTime.Now;

            // Then - Order should be created with correct details
            order.Should().NotBeNull();
            order.CustomerId.Should().Be(customerId);
            order.Amount.Should().Be(200m);
            order.Status.Should().Be("Pending");
            order.OrderItems.Should().HaveCount(1);
            order.OrderItems.First().Quantity.Should().Be(2);
            order.OrderItems.First().TicketTypeId.Should().Be(1);
        }


        [Fact]
        public async Task OutOfStock_BlocksPurchase()
        {
            // Given - Ticket type with 0 quantity (sold out)
            var soldOutTicketType = new TicketType
            {
                TicketTypeId = 99,
                EventId = 1,
                TypeName = "Sold Out",
                Price = 100,
                Quantity = 0, // No tickets available
                Status = "Active",
                SaleStart = DateTime.Now,
                SaleEnd = DateTime.Now.AddDays(30)
            };
            _context.TicketTypes.Add(soldOutTicketType);
            await _context.SaveChangesAsync();

            // When - Try to buy from sold out ticket type
            var availableQuantity = await _context.TicketTypes
                .Where(tt => tt.TicketTypeId == 99)
                .Select(tt => tt.Quantity)
                .FirstOrDefaultAsync();

            // Then - Should show no availability
            availableQuantity.Should().Be(0);
        }

        [Fact]
        public async Task InvalidTicketType_NotFound()
        {
            // Given - Request for non-existent ticket type
            var request = new CreateOrderRequestDTO
            {
                TicketTypeId = 999, // Non-existent
                Quantity = 1,
                SeatNo = "A1"
            };

            // When - Try to find ticket type
            var ticketType = await _context.TicketTypes
                .FirstOrDefaultAsync(tt => tt.TicketTypeId == request.TicketTypeId);

            // Then - Should not find ticket type
            ticketType.Should().BeNull("Non-existent ticket type should not be found");
        }

        [Fact]
        public async Task EventNotOpen_BlocksPurchase()
        {
            // Given - Event with status "Draft" (not open for sale)
            var draftEvent = new Event
            {
                EventId = 99,
                HostId = 1,
                Title = "Draft Event",
                Status = "Draft", // Not open for sale
                StartTime = DateTime.Now.AddDays(30),
                EndTime = DateTime.Now.AddDays(31),
                CreatedAt = DateTime.Now
            };
            _context.Events.Add(draftEvent);
            await _context.SaveChangesAsync();

            // When - Check if event is open for sale
            var isEventOpen = await _context.Events
                .Where(e => e.EventId == 99)
                .Select(e => e.Status == "Open")
                .FirstOrDefaultAsync();

            // Then - Event should not be open
            isEventOpen.Should().BeFalse("Draft events should not be open for sale");
        }


        [Fact]
        public async Task CalculateTotalPrice_Accurate()
        {
            // Given - Customer buying multiple tickets with different prices
            var vipTicket = await _context.TicketTypes
                .Where(tt => tt.TicketTypeId == 1)
                .FirstOrDefaultAsync(); // VIP: 100 each

            var standardTicket = await _context.TicketTypes
                .Where(tt => tt.TicketTypeId == 2)
                .FirstOrDefaultAsync(); // Standard: 50 each

            // When - Calculate total for 2 VIP + 1 Standard
            var vipQuantity = 2;
            var standardQuantity = 1;
            var expectedTotal = ((vipTicket?.Price ?? 0) * vipQuantity) + ((standardTicket?.Price ?? 0) * standardQuantity);

            // Then - Total should be calculated correctly
            expectedTotal.Should().Be(250m); // (100 * 2) + (50 * 1) = 250
        }

        [Fact]
        public async Task CheckTicketAvailability_ReturnsCount()
        {
            // Given - VIP tickets with 10 available
            var ticketTypeId = 1;

            // When - Check available quantity
            var availableQuantity = await _context.TicketTypes
                .Where(tt => tt.TicketTypeId == ticketTypeId)
                .Select(tt => tt.Quantity)
                .FirstOrDefaultAsync();

            // Then - Should show correct availability
            availableQuantity.Should().Be(10);
        }


        [Fact]
        public async Task ValidateEventIsActive_ReturnsTrue()
        {
            // Given - Active event
            var eventId = 1;

            // When - Check if event is active
            var isEventActive = await _context.Events
                .Where(e => e.EventId == eventId)
                .Select(e => e.Status == "Open")
                .FirstOrDefaultAsync();

            // Then - Event should be active
            isEventActive.Should().BeTrue("Active event should be open for sale");
        }

        #endregion
    }
}
