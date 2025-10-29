using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    public interface IEventService
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int eventId);
        Task<Event?> CreateEventAsync(Event eventData);
        Task<Event?> UpdateEventAsync(int eventId, Event eventData);
        Task<bool> DeleteEventAsync(int eventId);
        Task<List<Event>> GetEventsByHostAsync(int hostId);
        Task<bool> CheckHasTicketsSoldAsync(int eventId);
        Task<bool> CheckHasPaidTicketsAsync(int eventId);
        Task<bool> DeleteTicketTypesForEventAsync(int eventId);
        object MapToEventDto(Event eventData);
        object MapToEventDetailDto(Event eventData);
        Event MapFromCreateEventRequest(CreateEventRequest request, int userId);
    }
}

