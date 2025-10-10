using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int eventId);
        Task<Event?> CreateEventAsync(Event eventData);
        Task<Event?> UpdateEventAsync(int eventId, Event eventData);
        Task<bool> DeleteEventAsync(int eventId);
        Task<List<Event>> GetEventsByHostAsync(int hostId);
    }
}


