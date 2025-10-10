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

    public EventService(IEventRepository eventRepository, IEventMapper eventMapper)
    {
        _eventRepository = eventRepository;
        _eventMapper = eventMapper;
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
        return await _eventRepository.DeleteEventAsync(eventId);
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


