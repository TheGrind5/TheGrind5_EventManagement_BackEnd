using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Tests.Fixtures;

/// <summary>
/// Builder class để tạo test data objects dễ dàng
/// </summary>
public class TestDataBuilder
{
    private static int _userIdCounter = 1;
    private static int _eventIdCounter = 1;
    private static int _orderIdCounter = 1;
    private static int _ticketTypeIdCounter = 1;
    private static int _ticketIdCounter = 1;
    private static int _campusIdCounter = 1;

    public static void ResetCounters()
    {
        _userIdCounter = 1;
        _eventIdCounter = 1;
        _orderIdCounter = 1;
        _ticketTypeIdCounter = 1;
        _ticketIdCounter = 1;
        _campusIdCounter = 1;
    }

    #region User Builder

    public static User CreateUser(
        string? username = null,
        string? email = null,
        string? fullName = null,
        string role = "Customer",
        bool isBanned = false,
        decimal walletBalance = 0)
    {
        var userId = _userIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new User
        {
            UserId = userId,
            Username = username ?? $"testuser{userId}",
            FullName = fullName ?? $"Test User {userId}",
            Email = email ?? $"testuser{userId}@example.com",
            PasswordHash = "hashed_password_123",
            Phone = "0123456789",
            Role = role,
            CreatedAt = timestamp,
            UpdatedAt = null,
            Avatar = null,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            IsBanned = isBanned,
            BannedAt = isBanned ? timestamp : null,
            BanReason = isBanned ? "Test ban reason" : null,
            WalletBalance = walletBalance,
            CampusId = null,
            Events = new List<Event>(),
            Orders = new List<Order>(),
            WalletTransactions = new List<WalletTransaction>(),
            Wishlists = new List<Wishlist>(),
            Notifications = new List<Notification>(),
            AISuggestions = new List<AISuggestion>()
        };
    }

    public static User CreateAdminUser()
    {
        return CreateUser(role: "Admin");
    }

    public static User CreateHostUser()
    {
        return CreateUser(role: "Host");
    }

    public static User CreateBannedUser()
    {
        return CreateUser(isBanned: true);
    }

    #endregion

    #region Campus Builder

    public static Campus CreateCampus(
        string? name = null,
        string? code = null)
    {
        var campusId = _campusIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new Campus
        {
            CampusId = campusId,
            Name = name ?? $"Campus {campusId}",
            Code = code,
            CreatedAt = timestamp,
            UpdatedAt = null,
            Users = new List<User>(),
            Events = new List<Event>()
        };
    }

    #endregion

    #region Event Builder

    public static Event CreateEvent(
        int? hostId = null,
        string? title = null,
        string? description = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? location = null,
        string? eventType = null,
        string eventMode = "Offline",
        string? category = null,
        string status = "Open")
    {
        var eventId = _eventIdCounter++;
        var timestamp = DateTime.UtcNow;

        var start = startTime ?? timestamp.AddDays(7);
        var end = endTime ?? start.AddHours(3);

        return new Event
        {
            EventId = eventId,
            HostId = hostId ?? 1,
            Title = title ?? $"Test Event {eventId}",
            Description = description ?? $"Description for event {eventId}",
            StartTime = start,
            EndTime = end,
            Location = location ?? $"Location {eventId}",
            EventType = eventType ?? "Workshop",
            EventMode = eventMode,
            Category = category ?? "Education",
            Status = status,
            EventDetails = null,
            TermsAndConditions = null,
            OrganizerInfo = null,
            VenueLayout = null,
            CreatedAt = timestamp,
            UpdatedAt = null,
            Host = null!,
            TicketTypes = new List<TicketType>(),
            EventQuestions = new List<EventQuestion>(),
            CampusId = null
        };
    }

    public static Event CreateDraftEvent(int? hostId = null)
    {
        return CreateEvent(hostId: hostId, status: "Draft");
    }

    public static Event CreateCancelledEvent(int? hostId = null)
    {
        return CreateEvent(hostId: hostId, status: "Cancelled");
    }

    public static Event CreateClosedEvent(int? hostId = null)
    {
        return CreateEvent(hostId: hostId, status: "Closed");
    }

    public static Event CreateOnlineEvent(int? hostId = null)
    {
        return CreateEvent(hostId: hostId, eventMode: "Online");
    }

    #endregion

    #region TicketType Builder

    public static TicketType CreateTicketType(
        int? eventId = null,
        string? typeName = null,
        decimal? price = null,
        int quantity = 100,
        int? minOrder = 1,
        int? maxOrder = 10,
        DateTime? saleStart = null,
        DateTime? saleEnd = null,
        string status = "Active")
    {
        var ticketTypeId = _ticketTypeIdCounter++;
        var timestamp = DateTime.UtcNow;
        var start = saleStart ?? timestamp.AddDays(-1);
        var end = saleEnd ?? timestamp.AddDays(30);

        return new TicketType
        {
            TicketTypeId = ticketTypeId,
            EventId = eventId ?? 1,
            TypeName = typeName ?? $"Type {ticketTypeId}",
            Price = price ?? 100.00m,
            Quantity = quantity,
            MinOrder = minOrder,
            MaxOrder = maxOrder,
            SaleStart = start,
            SaleEnd = end,
            Status = status,
            Event = null!,
            Tickets = new List<Ticket>(),
            OrderItems = new List<OrderItem>()
        };
    }

    #endregion

    #region Order Builder

    public static Order CreateOrder(
        int? customerId = null,
        int eventId = 1,
        decimal amount = 100.00m,
        string status = "Pending",
        string paymentMethod = "Wallet")
    {
        var orderId = _orderIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new Order
        {
            OrderId = orderId,
            CustomerId = customerId ?? 1,
            EventId = eventId,
            Amount = amount,
            Status = status,
            PaymentMethod = paymentMethod,
            CreatedAt = timestamp,
            UpdatedAt = null,
            Customer = null!,
            OrderItems = new List<OrderItem>(),
            Payments = new List<Payment>()
        };
    }

    public static Order CreatePaidOrder(int? customerId = null)
    {
        return CreateOrder(customerId: customerId, status: "Paid");
    }

    public static Order CreateCancelledOrder(int? customerId = null)
    {
        return CreateOrder(customerId: customerId, status: "Cancelled");
    }

    #endregion

    #region OrderItem Builder

    public static OrderItem CreateOrderItem(
        int? orderId = null,
        int? ticketTypeId = null,
        int quantity = 1,
        string? seatNo = null,
        string status = "Confirmed")
    {
        var orderItemId = _orderIdCounter++;

        return new OrderItem
        {
            OrderItemId = orderItemId,
            OrderId = orderId ?? 1,
            TicketTypeId = ticketTypeId ?? 1,
            Quantity = quantity,
            SeatNo = seatNo,
            Status = status,
            Order = null!,
            TicketType = null!,
            Tickets = new List<Ticket>()
        };
    }

    #endregion

    #region Ticket Builder

    public static Ticket CreateTicket(
        int? ticketTypeId = null,
        int? orderItemId = null,
        string? serialNumber = null,
        string status = "Assigned")
    {
        var ticketId = _ticketIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new Ticket
        {
            TicketId = ticketId,
            TicketTypeId = ticketTypeId ?? 1,
            OrderItemId = orderItemId,
            SerialNumber = serialNumber ?? $"SN{ticketId:D10}",
            Status = status,
            IssuedAt = timestamp,
            UsedAt = status == "Used" ? timestamp : null,
            RefundedAt = status == "Refunded" ? timestamp : null,
            TicketType = null!,
            OrderItem = null
        };
    }

    public static Ticket CreateUsedTicket(int? ticketTypeId = null)
    {
        return CreateTicket(ticketTypeId: ticketTypeId, status: "Used");
    }

    #endregion

    #region Voucher Builder

    public static Voucher CreateVoucher(
        string? code = null,
        decimal discountPercentage = 10.00m,
        DateTime? validFrom = null,
        DateTime? validTo = null,
        bool isActive = true)
    {
        var voucherId = _orderIdCounter++;
        var timestamp = DateTime.UtcNow;
        var from = validFrom ?? timestamp.AddDays(-1);
        var to = validTo ?? timestamp.AddDays(30);

        return new Voucher
        {
            VoucherId = voucherId,
            VoucherCode = code ?? $"VOUCHER{voucherId}",
            DiscountPercentage = discountPercentage,
            ValidFrom = from,
            ValidTo = to,
            IsActive = isActive,
            CreatedAt = timestamp,
            UpdatedAt = null
        };
    }

    public static Voucher CreateExpiredVoucher()
    {
        return CreateVoucher(
            validFrom: DateTime.UtcNow.AddDays(-30),
            validTo: DateTime.UtcNow.AddDays(-1));
    }

    #endregion

    #region WalletTransaction Builder

    public static WalletTransaction CreateWalletTransaction(
        int? userId = null,
        decimal amount = 100.00m,
        string transactionType = "Deposit",
        string status = "Completed",
        string? description = "Test transaction")
    {
        var transactionId = _orderIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new WalletTransaction
        {
            TransactionId = transactionId,
            UserId = userId ?? 1,
            Amount = amount,
            TransactionType = transactionType,
            Status = status,
            BalanceBefore = 0.00m,
            BalanceAfter = amount,
            Description = description,
            ReferenceId = null,
            CreatedAt = timestamp,
            CompletedAt = status == "Completed" ? timestamp : null,
            User = null!
        };
    }

    #endregion

    #region Payment Builder

    public static Payment CreatePayment(
        int? orderId = null,
        decimal amount = 100.00m,
        string method = "Wallet",
        string status = "Succeeded",
        DateTime? paymentDate = null)
    {
        var paymentId = _orderIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new Payment
        {
            PaymentId = paymentId,
            OrderId = orderId ?? 1,
            Amount = amount,
            Method = method,
            Status = status,
            TransactionId = $"TXN{paymentId}",
            PaymentDate = paymentDate ?? timestamp,
            CreatedAt = timestamp,
            UpdatedAt = null,
            Order = null!
        };
    }

    #endregion

    #region Notification Builder

    public static Notification CreateNotification(
        int? userId = null,
        string? title = null,
        string? content = null,
        string type = "NewMessage",
        bool isRead = false)
    {
        var notificationId = _orderIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new Notification
        {
            NotificationId = notificationId,
            UserId = userId ?? 1,
            Title = title ?? $"Notification {notificationId}",
            Content = content ?? $"Test notification message {notificationId}",
            Type = type,
            IsRead = isRead,
            ReadAt = isRead ? timestamp : null,
            RelatedEventId = null,
            RelatedOrderId = null,
            RelatedTicketId = null,
            CreatedAt = timestamp,
            User = null!
        };
    }

    #endregion

    #region Wishlist Builder

    public static Wishlist CreateWishlist(
        int? userId = null,
        int? ticketTypeId = null,
        int quantity = 1)
    {
        var wishlistId = _orderIdCounter++;
        var timestamp = DateTime.UtcNow;

        return new Wishlist
        {
            Id = wishlistId,
            UserId = userId ?? 1,
            TicketTypeId = ticketTypeId ?? 1,
            Quantity = quantity,
            AddedAt = timestamp,
            UpdatedAt = null,
            User = null!,
            TicketType = null!
        };
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Create a complete event with ticket types
    /// </summary>
    public static (Event Event, List<TicketType> TicketTypes) CreateEventWithTicketTypes(
        int? hostId = null,
        int ticketTypeCount = 2)
    {
        var evt = CreateEvent(hostId: hostId);
        var ticketTypes = new List<TicketType>();

        for (int i = 0; i < ticketTypeCount; i++)
        {
            var ticketType = CreateTicketType(
                eventId: evt.EventId,
                price: 100.00m + (i * 50));
            ticketTypes.Add(ticketType);
        }

        evt.TicketTypes = ticketTypes;

        return (evt, ticketTypes);
    }

    /// <summary>
    /// Create a complete order with items and tickets
    /// </summary>
    public static (Order Order, List<OrderItem> Items, List<Ticket> Tickets) CreateOrderWithItemsAndTickets(
        int? customerId = null,
        int eventId = 1,
        int itemCount = 1)
    {
        var order = CreateOrder(customerId: customerId, eventId: eventId);
        var items = new List<OrderItem>();
        var tickets = new List<Ticket>();

        for (int i = 0; i < itemCount; i++)
        {
            var item = CreateOrderItem(
                orderId: order.OrderId,
                quantity: 2);
            items.Add(item);

            for (int j = 0; j < item.Quantity; j++)
            {
                var ticket = CreateTicket(
                    ticketTypeId: item.TicketTypeId,
                    orderItemId: item.OrderItemId);
                tickets.Add(ticket);
            }
        }

        order.OrderItems = items;
        foreach (var item in items)
        {
            item.Tickets = tickets.Where(t => t.OrderItemId == item.OrderItemId).ToList();
        }

        return (order, items, tickets);
    }

    #endregion
}

