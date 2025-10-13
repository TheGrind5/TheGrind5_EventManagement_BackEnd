using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(int userId);
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByEventIdAsync(int eventId);
        Task<Ticket> CheckInTicketAsync(int ticketId);
        Task<Ticket> CreateTicketAsync(int ticketTypeId, int orderItemId, string serialNumber);
        Task<IEnumerable<Ticket>> CreateTicketsForOrderItemAsync(int orderItemId, int quantity, int ticketTypeId);
        Task<Ticket> RefundTicketAsync(int ticketId);
        Task<bool> IsTicketValidAsync(int ticketId);
        Task<string> GenerateTicketSerialNumberAsync(int eventId, int ticketTypeId);
    }
}
