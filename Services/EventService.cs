using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services;

public class EventService
{
    private readonly EventDBContext _context;

    public EventService(EventDBContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await _context.Events
            .Include(e => e.Host)
            .Include(e => e.TicketTypes)
            .Where(e => e.Status == "Active")
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

        existingEvent.Title = eventData.Title;
        existingEvent.Description = eventData.Description;
        existingEvent.StartTime = eventData.StartTime;
        existingEvent.EndTime = eventData.EndTime;
        existingEvent.Location = eventData.Location;
        existingEvent.Category = eventData.Category;
        existingEvent.Status = eventData.Status;
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

    public async Task SeedSampleEventsAsync()
    {
        if (await _context.Events.AnyAsync()) return;

        var admin = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        if (admin == null) return;

        var sampleEvents = new List<Event>
        {
            new Event
            {
                HostId = admin.UserId,
                Title = "Tech Conference 2024",
                Description = "Annual technology conference featuring the latest innovations",
                StartTime = DateTime.UtcNow.AddDays(30),
                EndTime = DateTime.UtcNow.AddDays(30).AddHours(8),
                Location = "Convention Center, Ho Chi Minh City",
                Category = "Technology",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            },
            new Event
            {
                HostId = admin.UserId,
                Title = "Music Festival Summer",
                Description = "Outdoor music festival with top artists",
                StartTime = DateTime.UtcNow.AddDays(45),
                EndTime = DateTime.UtcNow.AddDays(47),
                Location = "Saigon Zoo and Botanical Gardens",
                Category = "Music",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            },
            new Event
            {
                HostId = admin.UserId,
                Title = "Business Networking Event",
                Description = "Connect with industry leaders and entrepreneurs",
                StartTime = DateTime.UtcNow.AddDays(15),
                EndTime = DateTime.UtcNow.AddDays(15).AddHours(4),
                Location = "Lotte Center, District 7",
                Category = "Business",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Events.AddRange(sampleEvents);
        await _context.SaveChangesAsync();
    }
}
