using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    public interface ITicketService
    {
        // Original method - backward compatibility
        Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(int userId);
        
        // New paginated method
        Task<PagedResponse<Ticket>> GetTicketsByUserIdAsync(int userId, PagedRequest request);
        
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByEventIdAsync(int eventId);
        Task<IEnumerable<TicketType>> GetTicketTypesByEventIdAsync(int eventId);
        Task<Ticket> CheckInTicketAsync(int ticketId);
        Task<Ticket> CreateTicketAsync(int ticketTypeId, int orderItemId, string serialNumber);
        Task<IEnumerable<Ticket>> CreateTicketsForOrderItemAsync(int orderItemId, int quantity, int ticketTypeId);
        Task<Ticket> RefundTicketAsync(int ticketId);
        Task<bool> IsTicketValidAsync(int ticketId);
        Task<string> GenerateTicketSerialNumberAsync(int eventId, int ticketTypeId);
        Task<TicketType> GetTicketTypeByIdAsync(int ticketTypeId);
        Task<int> GetSoldTicketsCountAsync(int ticketTypeId);
    }
}
