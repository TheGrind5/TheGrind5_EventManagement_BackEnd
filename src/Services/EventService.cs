using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventMapper _eventMapper;
    private readonly IFileManagementService _fileManagementService;
    private readonly EventDBContext _context;

    public EventService(IEventRepository eventRepository, IEventMapper eventMapper, IFileManagementService fileManagementService, EventDBContext context)
    {
        _eventRepository = eventRepository;
        _eventMapper = eventMapper;
        _fileManagementService = fileManagementService;
        _context = context;
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllEventsAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int eventId)
    {
        return await _eventRepository.GetEventByIdAsync(eventId);
    }

    public async Task<Event?> CreateEventAsync(Event eventData)
    {
        return await _eventRepository.CreateEventAsync(eventData);
    }

    public async Task<Event?> UpdateEventAsync(int eventId, Event eventData)
    {
        return await _eventRepository.UpdateEventAsync(eventId, eventData);
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        // Lấy thông tin sự kiện trước khi xóa để lấy ảnh
        var eventToDelete = await _eventRepository.GetEventByIdAsync(eventId);
        if (eventToDelete == null)
            return false;

        // Lấy danh sách ảnh cần xóa
        var imagesToDelete = new List<string>();
        var eventDetails = eventToDelete.GetEventDetails();
        
        if (!string.IsNullOrEmpty(eventDetails.EventImage))
            imagesToDelete.Add(eventDetails.EventImage);
        if (!string.IsNullOrEmpty(eventDetails.BackgroundImage))
            imagesToDelete.Add(eventDetails.BackgroundImage);

        // Xóa ticket types trước khi xóa event
        var ticketTypes = await _context.TicketTypes
            .Where(tt => tt.EventId == eventId)
            .ToListAsync();

        if (ticketTypes.Any())
        {
            _context.TicketTypes.RemoveRange(ticketTypes);
            await _context.SaveChangesAsync();
        }

        // Xóa sự kiện từ database
        var result = await _eventRepository.DeleteEventAsync(eventId);
        
        if (result && imagesToDelete.Any())
        {
            // Xóa ảnh liên quan
            await _fileManagementService.DeleteEventImagesAsync(imagesToDelete);
        }

        return result;
    }

    public async Task<List<Event>> GetEventsByHostAsync(int hostId)
    {
        return await _eventRepository.GetEventsByHostAsync(hostId);
    }

    public async Task<bool> CheckHasTicketsSoldAsync(int eventId)
    {
        try
        {
            // Lấy danh sách ticket types của event
            var eventData = await _eventRepository.GetEventByIdAsync(eventId);
            if (eventData == null || !eventData.TicketTypes.Any())
                return false;

            var ticketTypeIds = eventData.TicketTypes.Select(tt => tt.TicketTypeId).ToList();

            // Kiểm tra xem có order items nào với ticket types này không
            var hasTicketsSold = await _context.OrderItems
                .Where(oi => ticketTypeIds.Contains(oi.TicketTypeId) && oi.Status != "Cancelled")
                .AnyAsync();

            return hasTicketsSold;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CheckHasPaidTicketsAsync(int eventId)
    {
        try
        {
            // Lấy danh sách ticket types của event
            var eventData = await _eventRepository.GetEventByIdAsync(eventId);
            if (eventData == null || !eventData.TicketTypes.Any())
                return false;

            var ticketTypeIds = eventData.TicketTypes.Select(tt => tt.TicketTypeId).ToList();

            // Kiểm tra xem có order items nào với ticket types này VÀ Order có Status = "Paid" không
            var hasPaidTickets = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => ticketTypeIds.Contains(oi.TicketTypeId) 
                    && oi.Order.Status == "Paid"
                    && oi.Status != "Cancelled")
                .AnyAsync();

            return hasPaidTickets;
        }
        catch
        {
            return true; // Trả về true để an toàn, không cho xóa nếu có lỗi
        }
    }

    public async Task<bool> DeleteTicketTypesForEventAsync(int eventId)
    {
        try
        {
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.EventId == eventId)
                .ToListAsync();

            if (ticketTypes.Any())
            {
                _context.TicketTypes.RemoveRange(ticketTypes);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting ticket types: {ex.Message}");
            return false;
        }
    }

    public object MapToEventDto(Event eventData)
    {
        return _eventMapper.MapToEventDto(eventData);
    }

    public object MapToEventDetailDto(Event eventData)
    {
        return _eventMapper.MapToEventDetailDto(eventData);
    }

    public Event MapFromCreateEventRequest(CreateEventRequest request, int userId)
    {
        return _eventMapper.MapFromCreateEventRequest(request, userId);
    }

}


