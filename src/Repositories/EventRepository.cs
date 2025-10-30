using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventDBContext _context;

        public EventRepository(EventDBContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Host)
                .Include(e => e.TicketTypes)
                .Where(e => e.Status == "Open")
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            return await _context.Events
                .Include(e => e.Host)
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task<Event?> CreateEventAsync(Event eventData)
        {
            _context.Events.Add(eventData);
            await _context.SaveChangesAsync();
            return eventData;
        }

        public async Task<Event?> UpdateEventAsync(int eventId, Event eventData)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null) return null;

            // Cập nhật các field cơ bản
            existingEvent.Title = eventData.Title;
            existingEvent.Description = eventData.Description;
            existingEvent.StartTime = eventData.StartTime;
            existingEvent.EndTime = eventData.EndTime;
            existingEvent.Location = eventData.Location;
            existingEvent.Category = eventData.Category;
            existingEvent.Status = eventData.Status;
            existingEvent.EventMode = eventData.EventMode;
            existingEvent.EventType = eventData.EventType;
            
            // QUAN TRỌNG: Cập nhật EventDetails (chứa EventImage, BackgroundImage, images array)
            existingEvent.EventDetails = eventData.EventDetails;
            
            // Cập nhật các JSON fields khác
            existingEvent.TermsAndConditions = eventData.TermsAndConditions;
            existingEvent.OrganizerInfo = eventData.OrganizerInfo;
            existingEvent.VenueLayout = eventData.VenueLayout;
            
            // QUAN TRỌNG: Luôn cập nhật UpdatedAt để trigger cache bust ở frontend
            existingEvent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingEvent;
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var eventData = await _context.Events.FindAsync(eventId);
            if (eventData == null) return false;

            _context.Events.Remove(eventData);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Event>> GetEventsByHostAsync(int hostId)
        {
            return await _context.Events
                .Include(e => e.Host)
                .Include(e => e.TicketTypes)
                .Where(e => e.HostId == hostId)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }
    }
}


