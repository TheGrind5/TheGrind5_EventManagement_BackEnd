using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Business;

namespace TheGrind5_EventManagement.Services
{
    public class TicketService : ITicketService
    {
        private readonly EventDBContext _context;

        public TicketService(EventDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(int userId)
        {
            return await _context.Tickets
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Include(t => t.OrderItem)
                    .ThenInclude(oi => oi.Order)
                .Where(t => t.OrderItem.Order.CustomerId == userId)
                .OrderByDescending(t => t.IssuedAt)
                .ToListAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Include(t => t.OrderItem)
                    .ThenInclude(oi => oi.Order)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByEventIdAsync(int eventId)
        {
            return await _context.Tickets
                .Include(t => t.TicketType)
                .Include(t => t.OrderItem)
                    .ThenInclude(oi => oi.Order)
                .Where(t => t.TicketType.EventId == eventId)
                .OrderBy(t => t.IssuedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketType>> GetTicketTypesByEventIdAsync(int eventId)
        {
            return await _context.TicketTypes
                .Where(tt => tt.EventId == eventId && tt.Status == "Active")
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<Ticket> CheckInTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            if (ticket.Status != "Assigned")
                throw new InvalidOperationException("Ticket is not available for check-in");

            // Check if event has started
            if (DateTime.Now < ticket.TicketType.Event.StartTime)
                throw new InvalidOperationException("Event has not started yet");

            // Check if event has ended
            if (DateTime.Now > ticket.TicketType.Event.EndTime)
                throw new InvalidOperationException("Event has already ended");

            ticket.Status = "Used";
            ticket.UsedAt = DateTime.Now;
            ticket.TicketType.Event.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> CreateTicketAsync(int ticketTypeId, int orderItemId, string serialNumber)
        {
            var ticket = new Ticket
            {
                TicketTypeId = ticketTypeId,
                OrderItemId = orderItemId,
                SerialNumber = serialNumber,
                Status = "Assigned",
                IssuedAt = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task<IEnumerable<Ticket>> CreateTicketsForOrderItemAsync(int orderItemId, int quantity, int ticketTypeId)
        {
            var tickets = new List<Ticket>();
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.TicketType)
                    .ThenInclude(tt => tt.Event)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);

            if (orderItem == null)
                throw new ArgumentException("Order item not found");

            for (int i = 0; i < quantity; i++)
            {
                var serialNumber = await GenerateTicketSerialNumberAsync(
                    orderItem.TicketType.EventId, 
                    ticketTypeId
                );

                var ticket = new Ticket
                {
                    TicketTypeId = ticketTypeId,
                    OrderItemId = orderItemId,
                    SerialNumber = serialNumber,
                    Status = "Assigned",
                    IssuedAt = DateTime.Now
                };

                tickets.Add(ticket);
                _context.Tickets.Add(ticket);
            }

            await _context.SaveChangesAsync();
            return tickets;
        }

        public async Task<Ticket> RefundTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.OrderItem)
                    .ThenInclude(oi => oi.Order)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            if (ticket.Status == "Used")
                throw new InvalidOperationException("Cannot refund a used ticket");

            if (ticket.Status == "Refunded")
                throw new InvalidOperationException("Ticket has already been refunded");

            ticket.Status = "Refunded";
            ticket.RefundedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> IsTicketValidAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
                return false;

            if (ticket.Status != "Assigned")
                return false;

            // Check if event is still active
            var now = DateTime.Now;
            if (now < ticket.TicketType.Event.StartTime || now > ticket.TicketType.Event.EndTime)
                return false;

            return true;
        }

        public async Task<string> GenerateTicketSerialNumberAsync(int eventId, int ticketTypeId)
        {
            var eventData = await _context.Events.FindAsync(eventId);
            var ticketType = await _context.TicketTypes.FindAsync(ticketTypeId);

            if (eventData == null || ticketType == null)
                throw new ArgumentException("Event or ticket type not found");

            // Generate format: EVENT{eventId}-TYPE{ticketTypeId}-{timestamp}-{random}
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            
            var serialNumber = $"EVENT{eventId}-TYPE{ticketTypeId}-{timestamp}-{random}";

            // Ensure uniqueness
            while (await _context.Tickets.AnyAsync(t => t.SerialNumber == serialNumber))
            {
                random = new Random().Next(1000, 9999);
                serialNumber = $"EVENT{eventId}-TYPE{ticketTypeId}-{timestamp}-{random}";
            }

            return serialNumber;
        }

        public async Task<TicketType> GetTicketTypeByIdAsync(int ticketTypeId)
        {
            return await _context.TicketTypes
                .Include(tt => tt.Event)
                .FirstOrDefaultAsync(tt => tt.TicketTypeId == ticketTypeId);
        }

        public async Task<int> GetSoldTicketsCountAsync(int ticketTypeId)
        {
            return await _context.Tickets
                .Where(t => t.TicketTypeId == ticketTypeId && t.Status != "Refunded")
                .CountAsync();
        }
    }
}
