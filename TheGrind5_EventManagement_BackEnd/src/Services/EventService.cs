using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Business;

namespace TheGrind5_EventManagement.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventMapper _eventMapper;
    private readonly IFileManagementService _fileManagementService;

    public EventService(IEventRepository eventRepository, IEventMapper eventMapper, IFileManagementService fileManagementService)
    {
        _eventRepository = eventRepository;
        _eventMapper = eventMapper;
        _fileManagementService = fileManagementService;
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


