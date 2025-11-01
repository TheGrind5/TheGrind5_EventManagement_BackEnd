using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace TheGrind5_EventManagement.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventMapper _eventMapper;
    private readonly IFileManagementService _fileManagementService;
    private readonly EventDBContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<EventService> _logger;

    public EventService(
        IEventRepository eventRepository, 
        IEventMapper eventMapper, 
        IFileManagementService fileManagementService, 
        EventDBContext context,
        IMemoryCache cache,
        ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _eventMapper = eventMapper;
        _fileManagementService = fileManagementService;
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // Original method - kept for backward compatibility
    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllEventsAsync();
    }

    // New paginated method
    public async Task<PagedResponse<Event>> GetAllEventsAsync(PagedRequest request)
    {
        var query = _context.Events
            .Include(e => e.TicketTypes)
            .Include(e => e.Campus) // Include Campus để có thể lấy campus name
            // Loại bỏ Include Host để tránh phụ thuộc vào các cột mới (IsBanned, BannedAt, BanReason)
            .OrderByDescending(e => e.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        
        var events = await query
            .Paginate(request)
            .ToListAsync();

        return new PagedResponse<Event>(events, totalCount, request.Page, request.PageSize);
    }

    public async Task<Event?> GetEventByIdAsync(int eventId)
    {
        // Try get from cache first
        var cacheKey = $"event_{eventId}";
        
        if (_cache.TryGetValue(cacheKey, out Event? cachedEvent))
        {
            return cachedEvent;
        }

        // If not in cache, get from database
        var eventData = await _eventRepository.GetEventByIdAsync(eventId);
        
        if (eventData != null)
        {
            // Cache for 10 minutes
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            
            _cache.Set(cacheKey, eventData, cacheOptions);
        }

        return eventData;
    }

    public async Task<Event?> CreateEventAsync(Event eventData)
    {
        return await _eventRepository.CreateEventAsync(eventData);
    }

    public async Task<Event?> UpdateEventAsync(int eventId, Event eventData)
    {
        var result = await _eventRepository.UpdateEventAsync(eventId, eventData);
        
        // Invalidate cache after update
        if (result != null)
        {
            var cacheKey = $"event_{eventId}";
            _cache.Remove(cacheKey);
        }
        
        return result;
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        try
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

            // Lấy tất cả TicketTypes của event
            var ticketTypes = await _context.TicketTypes
                .Where(tt => tt.EventId == eventId)
                .Select(tt => tt.TicketTypeId)
                .ToListAsync();

            if (ticketTypes.Any())
            {
                // XÓA THEO THỨ TỰ (vì tất cả đều dùng DeleteBehavior.Restrict):
                
                // 1. Xóa Tickets (liên quan đến TicketTypes)
                var tickets = await _context.Tickets
                    .Where(t => ticketTypes.Contains(t.TicketTypeId))
                    .ToListAsync();
                if (tickets.Any())
                {
                    _context.Tickets.RemoveRange(tickets);
                    await _context.SaveChangesAsync();
                }

                // 2. Xóa Wishlists (liên quan đến TicketTypes)
                var wishlists = await _context.Wishlists
                    .Where(w => ticketTypes.Contains(w.TicketTypeId))
                    .ToListAsync();
                if (wishlists.Any())
                {
                    _context.Wishlists.RemoveRange(wishlists);
                    await _context.SaveChangesAsync();
                }

                // 3. Xóa OrderItems (liên quan đến TicketTypes)
                var orderItems = await _context.OrderItems
                    .Where(oi => ticketTypes.Contains(oi.TicketTypeId))
                    .ToListAsync();
                if (orderItems.Any())
                {
                    // Lấy các OrderIds để xóa Orders sau
                    var orderIds = orderItems.Select(oi => oi.OrderId).Distinct().ToList();
                    
                    // Xóa Payments trước (vì Payment -> Order: Restrict)
                    var payments = await _context.Payments
                        .Where(p => orderIds.Contains(p.OrderId))
                        .ToListAsync();
                    if (payments.Any())
                    {
                        _context.Payments.RemoveRange(payments);
                        await _context.SaveChangesAsync();
                    }

                    // Xóa OrderItems
                    _context.OrderItems.RemoveRange(orderItems);
                    await _context.SaveChangesAsync();

                    // 4. Xóa Orders (sau khi đã xóa OrderItems và Payments)
                    var orders = await _context.Orders
                        .Where(o => orderIds.Contains(o.OrderId))
                        .ToListAsync();
                    if (orders.Any())
                    {
                        _context.Orders.RemoveRange(orders);
                        await _context.SaveChangesAsync();
                    }
                }

                // 5. Cuối cùng mới xóa TicketTypes
                var ticketTypesToDelete = await _context.TicketTypes
                    .Where(tt => tt.EventId == eventId)
                    .ToListAsync();
                if (ticketTypesToDelete.Any())
                {
                    _context.TicketTypes.RemoveRange(ticketTypesToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            // 6. Cuối cùng mới xóa Event
            var result = await _eventRepository.DeleteEventAsync(eventId);
            
            if (result)
            {
                // Invalidate cache after deletion
                var cacheKey = $"event_{eventId}";
                _cache.Remove(cacheKey);
                
                // Xóa ảnh liên quan
                if (imagesToDelete.Any())
                {
                    await _fileManagementService.DeleteEventImagesAsync(imagesToDelete);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting event {eventId}: {ex.Message}");
            _logger.LogError($"StackTrace: {ex.StackTrace}");
            throw;
        }
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


