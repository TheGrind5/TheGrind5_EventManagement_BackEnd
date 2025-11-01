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
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<Campus> Campuses => Set<Campus>();

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
        b.Entity<OtpCode>().ToTable("OtpCode");
        b.Entity<WalletTransaction>().ToTable("WalletTransaction");
        b.Entity<Payment>().ToTable("Payment");
        b.Entity<Wishlist>().ToTable("Wishlist");
        b.Entity<Voucher>().ToTable("Voucher");
        b.Entity<Campus>().ToTable("Campus");
        
        // Configure OtpCode primary key to match database
        b.Entity<OtpCode>()
         .HasKey(o => o.Id);
        
        // Configure Voucher DiscountPercentage precision
        b.Entity<Voucher>()
         .Property(v => v.DiscountPercentage)
         .HasPrecision(5, 2);
        
        // Configure column mappings for User table
        b.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Username).HasColumnName("Username");
        });
        
        ConfigureUserRelationships(b);
        ConfigureEventRelationships(b);
        ConfigureOrderRelationships(b);
        ConfigureTicketRelationships(b);
        ConfigurePaymentRelationships(b);
        ConfigureWalletRelationships(b);
        ConfigureWishlistRelationships(b);
        ConfigureCampusRelationships(b);
        ConfigureDecimalPrecision(b);
        ConfigureCampusColumnMapping(b);
    }

    private void ConfigureCampusColumnMapping(ModelBuilder b)
    {
        // Map Campus model to database schema
        b.Entity<Campus>(entity =>
        {
            entity.Property(e => e.Name).HasColumnName("CampusName");
            entity.Property(e => e.Code).HasColumnName("Province");
        });
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

    private void ConfigureWishlistRelationships(ModelBuilder b)
    {
        // Wishlist -> User : required, cascade delete
        b.Entity<Wishlist>()
         .HasOne(w => w.User)
         .WithMany(u => u.Wishlists)
         .HasForeignKey(w => w.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        // Wishlist -> TicketType : required, không cascade
        b.Entity<Wishlist>()
         .HasOne(w => w.TicketType)
         .WithMany()
         .HasForeignKey(w => w.TicketTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint: mỗi user chỉ có 1 wishlist item cho mỗi TicketType
        b.Entity<Wishlist>()
         .HasIndex(w => new { w.UserId, w.TicketTypeId })
         .IsUnique();
    }

    private void ConfigureCampusRelationships(ModelBuilder b)
    {
        // Event -> Campus : optional
        b.Entity<Event>()
         .HasOne(e => e.Campus)
         .WithMany(c => c.Events)
         .HasForeignKey(e => e.CampusId)
         .OnDelete(DeleteBehavior.Restrict);

        // User -> Campus : optional
        b.Entity<User>()
         .HasOne(u => u.Campus)
         .WithMany(c => c.Users)
         .HasForeignKey(u => u.CampusId)
         .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureDecimalPrecision(ModelBuilder b)
    {
        b.Entity<Order>()
         .Property(o => o.Amount)
         .HasPrecision(18, 2);

        b.Entity<Order>()
         .Property(o => o.DiscountAmount)
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


