using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Tests.Minh
{

public class TicketServiceTests : IDisposable
{
    private readonly EventDBContext _context;
    private readonly TicketService _ticketService;
    private readonly Mock<ILogger<TicketService>> _mockLogger;

    public TicketServiceTests()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<EventDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EventDBContext(options);
        _mockLogger = new Mock<ILogger<TicketService>>();
        _ticketService = new TicketService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region GetTicketsByUserIdAsync Tests

    [Fact]
    public async Task ValidUser_ReturnsTickets()
    {
        // Arrange
        var userId = 1;
        var eventId = 1;
        var ticketTypeId = 1;

        // Create test data
        var user = new User { UserId = userId, Email = "test@example.com", PasswordHash = "hash" };
        var eventData = new Event 
        { 
            EventId = eventId, 
            HostId = 2, 
            Title = "Test Event", 
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(2),
            Status = "Open"
        };
        var ticketType = new TicketType 
        { 
            TicketTypeId = ticketTypeId, 
            EventId = eventId, 
            TypeName = "VIP", 
            Quantity = 10,
            Status = "Active"
        };
        var order = new Order 
        { 
            OrderId = 1, 
            CustomerId = userId, 
            Amount = 100,
            Status = "Paid",
            CreatedAt = DateTime.UtcNow
        };
        var orderItem = new OrderItem 
        { 
            OrderItemId = 1, 
            OrderId = 1, 
            TicketTypeId = ticketTypeId, 
            Quantity = 1,
            Status = "Confirmed"
        };
        var ticket = new Ticket 
        { 
            TicketId = 1, 
            TicketTypeId = ticketTypeId, 
            OrderItemId = 1, 
            SerialNumber = "TICKET001",
            Status = "Assigned",
            IssuedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.Events.Add(eventData);
        _context.TicketTypes.Add(ticketType);
        _context.Orders.Add(order);
        _context.OrderItems.Add(orderItem);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Act
        var result = await _ticketService.GetTicketsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var ticketResult = result.First();
        Assert.Equal(1, ticketResult.TicketId);
        Assert.Equal("TICKET001", ticketResult.SerialNumber);
        Assert.Equal("Assigned", ticketResult.Status);
    }

    [Fact]
    public async Task Empty_ReturnsEmptyList()
    {
        // Arrange
        var userId = 999; // Non-existent user

        // Act
        var result = await _ticketService.GetTicketsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task InvalidUser_ReturnsEmptyList()
    {
        // Arrange
        var invalidUserId = -1; // Invalid user ID

        // Act
        var result = await _ticketService.GetTicketsByUserIdAsync(invalidUserId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetTicketByIdAsync Tests

    [Fact]
    public async Task ValidId_ReturnsTicket()
    {
        // Arrange
        var ticketId = 1;
        var eventId = 1;
        var ticketTypeId = 1;

        var eventData = new Event 
        { 
            EventId = eventId, 
            HostId = 1, 
            Title = "Test Event", 
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(2),
            Status = "Open"
        };
        var ticketType = new TicketType 
        { 
            TicketTypeId = ticketTypeId, 
            EventId = eventId, 
            TypeName = "VIP", 
            Quantity = 10,
            Status = "Active"
        };
        var order = new Order 
        { 
            OrderId = 1, 
            CustomerId = 1, 
            Amount = 100,
            Status = "Paid",
            CreatedAt = DateTime.UtcNow
        };
        var orderItem = new OrderItem 
        { 
            OrderItemId = 1, 
            OrderId = 1, 
            TicketTypeId = ticketTypeId, 
            Quantity = 1,
            Status = "Confirmed"
        };
        var ticket = new Ticket 
        { 
            TicketId = ticketId, 
            TicketTypeId = ticketTypeId, 
            OrderItemId = 1, 
            SerialNumber = "TICKET001",
            Status = "Assigned",
            IssuedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventData);
        _context.TicketTypes.Add(ticketType);
        _context.Orders.Add(order);
        _context.OrderItems.Add(orderItem);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Act
        var result = await _ticketService.GetTicketByIdAsync(ticketId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticketId, result.TicketId);
        Assert.Equal("TICKET001", result.SerialNumber);
    }

    [Fact]
    public async Task InvalidId_ReturnsNull()
    {
        // Arrange
        var invalidTicketId = 999;

        // Act
        var result = await _ticketService.GetTicketByIdAsync(invalidTicketId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region CheckInTicketAsync Tests

    [Fact]
    public async Task ValidTicket_UpdatesStatusToUsed()
    {
        // Arrange
        var ticketId = 1;
        var eventId = 1;
        var ticketTypeId = 1;

        var eventData = new Event 
        { 
            EventId = eventId, 
            HostId = 1, 
            Title = "Test Event", 
            StartTime = DateTime.UtcNow.AddMinutes(-30), // Event started 30 minutes ago
            EndTime = DateTime.UtcNow.AddHours(10),      // Event ends in 10 hours (plenty of time)
            Status = "Open"
        };
        var ticketType = new TicketType 
        { 
            TicketTypeId = ticketTypeId, 
            EventId = eventId, 
            TypeName = "VIP", 
            Quantity = 10,
            Status = "Active"
        };
        var ticket = new Ticket 
        { 
            TicketId = ticketId, 
            TicketTypeId = ticketTypeId, 
            OrderItemId = 1, 
            SerialNumber = "TICKET001",
            Status = "Assigned",
            IssuedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventData);
        _context.TicketTypes.Add(ticketType);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Act
        var result = await _ticketService.CheckInTicketAsync(ticketId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Used", result.Status);
        Assert.NotNull(result.UsedAt);
    }

    [Fact]
    public async Task EventNotStarted_ThrowsException()
    {
        // Arrange
        var ticketId = 1;
        var eventId = 1;
        var ticketTypeId = 1;

        var eventData = new Event 
        { 
            EventId = eventId, 
            HostId = 1, 
            Title = "Test Event", 
            StartTime = DateTime.UtcNow.AddHours(1), // Event starts in 1 hour
            EndTime = DateTime.UtcNow.AddHours(2),
            Status = "Open"
        };
        var ticketType = new TicketType 
        { 
            TicketTypeId = ticketTypeId, 
            EventId = eventId, 
            TypeName = "VIP", 
            Quantity = 10,
            Status = "Active"
        };
        var ticket = new Ticket 
        { 
            TicketId = ticketId, 
            TicketTypeId = ticketTypeId, 
            OrderItemId = 1, 
            SerialNumber = "TICKET001",
            Status = "Assigned",
            IssuedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventData);
        _context.TicketTypes.Add(ticketType);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _ticketService.CheckInTicketAsync(ticketId));
    }

    [Fact]
    public async Task EventEnded_ThrowsException()
    {
        // Arrange
        var ticketId = 1;
        var eventId = 1;
        var ticketTypeId = 1;

        var eventData = new Event 
        { 
            EventId = eventId, 
            HostId = 1, 
            Title = "Test Event", 
            StartTime = DateTime.UtcNow.AddHours(-2), // Event started 2 hours ago
            EndTime = DateTime.UtcNow.AddHours(-1),   // Event ended 1 hour ago
            Status = "Open"
        };
        var ticketType = new TicketType 
        { 
            TicketTypeId = ticketTypeId, 
            EventId = eventId, 
            TypeName = "VIP", 
            Quantity = 10,
            Status = "Active"
        };
        var ticket = new Ticket 
        { 
            TicketId = ticketId, 
            TicketTypeId = ticketTypeId, 
            OrderItemId = 1, 
            SerialNumber = "TICKET001",
            Status = "Assigned",
            IssuedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventData);
        _context.TicketTypes.Add(ticketType);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _ticketService.CheckInTicketAsync(ticketId));
    }

    // Removed test cases for methods that don't exist

    #endregion
}
}
