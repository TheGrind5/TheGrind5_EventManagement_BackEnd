using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<PagedResponse<Event>> SearchEventsAsync(EventSearchRequest request);
        Task<Event?> GetEventByIdAsync(int eventId);
        Task<Event?> CreateEventAsync(Event eventData);
        Task<Event?> UpdateEventAsync(int eventId, Event eventData);
        Task<bool> DeleteEventAsync(int eventId);
        Task<List<Event>> GetEventsByHostAsync(int hostId);
    }
}


