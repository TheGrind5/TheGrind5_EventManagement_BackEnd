#nullable enable
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Data;

public partial class EventDBContext : DbContext
{
    public EventDBContext(DbContextOptions<EventDBContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Configure table names to match existing database (singular names)
        b.Entity<User>().ToTable("User");
        b.Entity<Event>().ToTable("Event");
        b.Entity<TicketType>().ToTable("TicketType");
        b.Entity<Order>().ToTable("Order");
        b.Entity<OrderItem>().ToTable("OrderItem");
        b.Entity<Ticket>().ToTable("Ticket");

        b.Entity<TicketType>().ToTable("TicketType");
        b.Entity<WalletTransaction>().ToTable("WalletTransaction");

        b.Entity<Payment>().ToTable("Payment");
        
        // Configure column mappings for User table
        b.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Ignore(e => e.Username); // Database doesn't have Username column
        });


        
        ConfigureUserRelationships(b);
        ConfigureEventRelationships(b);
        ConfigureOrderRelationships(b);
        ConfigureTicketRelationships(b);
        ConfigurePaymentRelationships(b);
        ConfigureWalletRelationships(b);
        ConfigureDecimalPrecision(b);
    }

    private void ConfigureUserRelationships(ModelBuilder b)
    {
        // Event -> Host (User) : required, KHÔNG cascade xóa Event khi xóa Host
        b.Entity<Event>()
         .HasOne(e => e.Host)
         .WithMany(u => u.Events)
         .HasForeignKey(e => e.HostId)
         .OnDelete(DeleteBehavior.Restrict);

        // Order -> Customer (User) : required, không cascade
        b.Entity<Order>()
         .HasOne(o => o.Customer)
         .WithMany(u => u.Orders)
         .HasForeignKey(o => o.CustomerId)
         .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureEventRelationships(ModelBuilder b)
    {
        // TicketType -> Event : required, không cascade
        b.Entity<TicketType>()
         .HasOne(tt => tt.Event)
         .WithMany(e => e.TicketTypes)
         .HasForeignKey(tt => tt.EventId)
         .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureOrderRelationships(ModelBuilder b)
    {
        // OrderItem -> Order : required, không cascade
        b.Entity<OrderItem>()
         .HasOne(oi => oi.Order)
         .WithMany(o => o.OrderItems)
         .HasForeignKey(oi => oi.OrderId)
         .OnDelete(DeleteBehavior.Restrict);

        // OrderItem -> TicketType : required, không cascade
        b.Entity<OrderItem>()
         .HasOne(oi => oi.TicketType)
         .WithMany(tt => tt.OrderItems)
         .HasForeignKey(oi => oi.TicketTypeId)
         .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureTicketRelationships(ModelBuilder b)
    {
        // Ticket -> TicketType : required, không cascade
        b.Entity<Ticket>()
         .HasOne(t => t.TicketType)
         .WithMany(tt => tt.Tickets)
         .HasForeignKey(t => t.TicketTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        // Ticket -> OrderItem : optional, nếu xóa OrderItem thì set null
        b.Entity<Ticket>()
         .HasOne(t => t.OrderItem)
         .WithMany(oi => oi.Tickets)
         .HasForeignKey(t => t.OrderItemId)
         .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigurePaymentRelationships(ModelBuilder b)
    {
        // Payment -> Order : required, không cascade
        b.Entity<Payment>()
         .HasOne(p => p.Order)
         .WithMany(o => o.Payments)
         .HasForeignKey(p => p.OrderId)
         .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình Primary Key cho Payment
        b.Entity<Payment>()
         .HasKey(p => p.PaymentId);
    }

    private void ConfigureWalletRelationships(ModelBuilder b)
    {
        // WalletTransaction -> User : required, không cascade
        b.Entity<WalletTransaction>()
         .HasOne(wt => wt.User)
         .WithMany(u => u.WalletTransactions)
         .HasForeignKey(wt => wt.UserId)
         .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình Primary Key cho WalletTransaction
        b.Entity<WalletTransaction>()
         .HasKey(wt => wt.TransactionId);
    }

    private void ConfigureDecimalPrecision(ModelBuilder b)
    {
        b.Entity<Order>()
         .Property(o => o.Amount)
         .HasPrecision(18, 2);

        b.Entity<Payment>()
         .Property(p => p.Amount)
         .HasPrecision(18, 2);

        b.Entity<TicketType>()
         .Property(tt => tt.Price)
         .HasPrecision(18, 2);

        b.Entity<User>()
         .Property(u => u.WalletBalance)
         .HasPrecision(18, 2);

        b.Entity<WalletTransaction>()
         .Property(wt => wt.Amount)
         .HasPrecision(18, 2);

        b.Entity<WalletTransaction>()
         .Property(wt => wt.BalanceBefore)
         .HasPrecision(18, 2);

        b.Entity<WalletTransaction>()
         .Property(wt => wt.BalanceAfter)
         .HasPrecision(18, 2);
    }
}


