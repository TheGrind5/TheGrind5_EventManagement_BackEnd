using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Mappers
{
    public class EventMapper : IEventMapper
    {
        public object MapToEventDto(Event eventData)
        {
            var eventDetails = eventData.GetEventDetails();
            return new
            {
                EventId = eventData.EventId,
                Title = eventData.Title,
                Description = eventData.Description,
                StartTime = eventData.StartTime,
                EndTime = eventData.EndTime,
                Location = eventData.Location,
                Category = eventData.Category,
                Status = eventData.Status,
                HostName = eventData.Host?.FullName,
                HostEmail = eventData.Host?.Email,
                EventDetails = eventDetails,
                EventImage = eventDetails.EventImage,
                BackgroundImage = eventDetails.BackgroundImage,
                TicketTypes = CreateTicketTypeDtos(eventData.TicketTypes)
            };
        }

        public object MapToEventDetailDto(Event eventData)
        {
            var eventDetails = eventData.GetEventDetails();
            return new
            {
                EventId = eventData.EventId,
                Title = eventData.Title,
                Description = eventData.Description,
                StartTime = eventData.StartTime,
                EndTime = eventData.EndTime,
                Location = eventData.Location,
                EventMode = eventData.EventMode,
                EventType = eventData.EventType,
                Category = eventData.Category,
                Status = eventData.Status,
                CreatedAt = eventData.CreatedAt,
                HostId = eventData.HostId,
                HostName = eventData.Host?.FullName,
                HostEmail = eventData.Host?.Email,
                EventDetails = eventDetails,
                EventImage = eventDetails.EventImage,
                BackgroundImage = eventDetails.BackgroundImage,
                TicketTypes = CreateTicketTypeDtos(eventData.TicketTypes)
            };
        }

        public Event MapFromCreateEventRequest(CreateEventRequest request, int userId)
        {
            return new Event
            {
                HostId = userId,
                Title = request.Title,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Location = request.Location,
                Category = request.Category,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };
        }

        private object CreateTicketTypeDtos(ICollection<TicketType>? ticketTypes)
        {
            if (ticketTypes == null)
                return new List<object>();
                
            return ticketTypes.Where(tt => tt.Status == "Active").Select(tt => new
            {
                TicketTypeId = tt.TicketTypeId,
                TypeName = tt.TypeName,
                Price = tt.Price,
                Quantity = tt.Quantity,
                MinOrder = tt.MinOrder,
                MaxOrder = tt.MaxOrder,
                SaleStart = tt.SaleStart,
                SaleEnd = tt.SaleEnd,
                Status = tt.Status
            });
        }
    }
}


