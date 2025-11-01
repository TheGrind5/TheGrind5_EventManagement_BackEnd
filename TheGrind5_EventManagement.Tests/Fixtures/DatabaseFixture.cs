using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TheGrind5_EventManagement.Data;

namespace TheGrind5_EventManagement.Tests.Fixtures;

/// <summary>
/// Database fixture cho integration tests với InMemory database
/// </summary>
public class DatabaseFixture : IDisposable
{
    private readonly string _databaseName;
    public EventDBContext Context { get; private set; }

    public DatabaseFixture(string? databaseName = null)
    {
        _databaseName = databaseName ?? $"TestDB_{Guid.NewGuid()}";
        
        var options = new DbContextOptionsBuilder<EventDBContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        Context = new EventDBContext(options);
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Ensure database is created
        Context.Database.EnsureCreated();
    }

    /// <summary>
    /// Clear all data từ database
    /// </summary>
    public async Task ClearAllDataAsync()
    {
        Context.Tickets.RemoveRange(Context.Tickets);
        Context.OrderItems.RemoveRange(Context.OrderItems);
        Context.Payments.RemoveRange(Context.Payments);
        Context.Orders.RemoveRange(Context.Orders);
        Context.WalletTransactions.RemoveRange(Context.WalletTransactions);
        Context.Wishlists.RemoveRange(Context.Wishlists);
        Context.Notifications.RemoveRange(Context.Notifications);
        Context.EventQuestions.RemoveRange(Context.EventQuestions);
        Context.AISuggestions.RemoveRange(Context.AISuggestions);
        Context.TicketTypes.RemoveRange(Context.TicketTypes);
        Context.Events.RemoveRange(Context.Events);
        Context.Users.RemoveRange(Context.Users);
        Context.Campuses.RemoveRange(Context.Campuses);
        Context.Vouchers.RemoveRange(Context.Vouchers);
        
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Seed test data vào database
    /// </summary>
    public async Task SeedTestDataAsync()
    {
        // Tạo test users
        var campus1 = TestDataBuilder.CreateCampus(name: "HCM Campus");
        var campus2 = TestDataBuilder.CreateCampus(name: "HN Campus");
        Context.Campuses.AddRange(campus1, campus2);
        await Context.SaveChangesAsync();

        var admin = TestDataBuilder.CreateAdminUser();
        var host1 = TestDataBuilder.CreateHostUser();
        var host2 = TestDataBuilder.CreateHostUser();
        var customer1 = TestDataBuilder.CreateUser(role: "Customer", walletBalance: 1000m);
        var customer2 = TestDataBuilder.CreateUser(role: "Customer", walletBalance: 500m);
        var bannedUser = TestDataBuilder.CreateBannedUser();
        
        admin.CampusId = campus1.CampusId;
        host1.CampusId = campus1.CampusId;
        host2.CampusId = campus2.CampusId;
        customer1.CampusId = campus1.CampusId;
        
        Context.Users.AddRange(admin, host1, host2, customer1, customer2, bannedUser);
        await Context.SaveChangesAsync();

        // Tạo test events
        var event1 = TestDataBuilder.CreateEvent(hostId: host1.UserId);
        var event2 = TestDataBuilder.CreateEvent(hostId: host2.UserId, status: "Draft");
        var event3 = TestDataBuilder.CreateEvent(hostId: host1.UserId, eventMode: "Online");
        event1.CampusId = campus1.CampusId;
        event2.CampusId = campus2.CampusId;
        event3.CampusId = campus1.CampusId;
        
        Context.Events.AddRange(event1, event2, event3);
        await Context.SaveChangesAsync();

        // Tạo test ticket types
        var ticketType1 = TestDataBuilder.CreateTicketType(eventId: event1.EventId, price: 100m, quantity: 100);
        var ticketType2 = TestDataBuilder.CreateTicketType(eventId: event1.EventId, price: 200m, quantity: 50);
        var ticketType3 = TestDataBuilder.CreateTicketType(eventId: event2.EventId, price: 150m, quantity: 75);
        var ticketType4 = TestDataBuilder.CreateTicketType(eventId: event3.EventId, price: 80m, quantity: 200);
        
        Context.TicketTypes.AddRange(ticketType1, ticketType2, ticketType3, ticketType4);
        await Context.SaveChangesAsync();

        // Tạo test vouchers
        var voucher1 = TestDataBuilder.CreateVoucher(discountPercentage: 10m);
        var voucher2 = TestDataBuilder.CreateExpiredVoucher();
        var voucher3 = TestDataBuilder.CreateVoucher(discountPercentage: 20m);
        
        Context.Vouchers.AddRange(voucher1, voucher2, voucher3);
        await Context.SaveChangesAsync();

        // Tạo test orders với items và tickets
        var (order1, items1, tickets1) = TestDataBuilder.CreateOrderWithItemsAndTickets(
            customerId: customer1.UserId,
            eventId: event1.EventId,
            itemCount: 1);
        order1.Status = "Paid";
        
        var order2 = TestDataBuilder.CreateOrder(customerId: customer2.UserId, eventId: event1.EventId, status: "Pending");
        
        Context.Orders.AddRange(order1, order2);
        await Context.SaveChangesAsync();

        // Tạo test wallet transactions
        var walletTransaction1 = TestDataBuilder.CreateWalletTransaction(
            userId: customer1.UserId,
            amount: 1000m,
            transactionType: "Deposit",
            description: "Initial deposit");
        var walletTransaction2 = TestDataBuilder.CreateWalletTransaction(
            userId: customer1.UserId,
            amount: -200m,
            transactionType: "Payment",
            description: "Payment for order",
            status: "Completed");
        
        Context.WalletTransactions.AddRange(walletTransaction1, walletTransaction2);
        await Context.SaveChangesAsync();

        // Tạo test wishlists
        var wishlist1 = TestDataBuilder.CreateWishlist(userId: customer1.UserId, ticketTypeId: ticketType1.TicketTypeId);
        var wishlist2 = TestDataBuilder.CreateWishlist(userId: customer2.UserId, ticketTypeId: ticketType2.TicketTypeId);
        
        Context.Wishlists.AddRange(wishlist1, wishlist2);
        await Context.SaveChangesAsync();

        // Tạo test notifications
        var notification1 = TestDataBuilder.CreateNotification(
            userId: customer1.UserId,
            title: "Order Confirmed",
            content: "Your order has been confirmed",
            type: "OrderConfirmation");
        var notification2 = TestDataBuilder.CreateNotification(
            userId: customer1.UserId,
            title: "Payment Success",
            content: "Your payment was successful",
            type: "PaymentSuccess");
        
        Context.Notifications.AddRange(notification1, notification2);
        await Context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}

/// <summary>
/// IClassFixture implementation cho integration tests
/// </summary>
public class IntegrationTestFixture : DatabaseFixture
{
    public IntegrationTestFixture() : base($"IntegrationTestDB_{Guid.NewGuid()}")
    {
    }
}

