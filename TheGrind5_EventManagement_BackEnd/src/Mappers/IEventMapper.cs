using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Mappers
{
    public interface IEventMapper
    {
        object MapToEventDto(Event eventData);
        object MapToEventDetailDto(Event eventData);
        Event MapFromCreateEventRequest(CreateEventRequest request, int userId);
    }
}


