#nullable enable
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Models;

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

    protected override void OnModelCreating(ModelBuilder b)
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

        // TicketType -> Event : required, không cascade
        b.Entity<TicketType>()
         .HasOne(tt => tt.Event)
         .WithMany(e => e.TicketTypes)
         .HasForeignKey(tt => tt.EventId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
