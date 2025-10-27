using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using System.Linq;
using System.Collections.Generic;

namespace TheGrind5_EventManagement.Mappers
{
    public class EventMapper : IEventMapper
    {
        public object MapToEventDto(Event eventData)
        {
            var eventDetails = eventData.GetEventDetails();
            
            // Construct location from address components if Location field is empty
            var location = eventData.Location;
            if (string.IsNullOrEmpty(location) && eventDetails != null)
            {
                var locationParts = new List<string>();
                if (!string.IsNullOrEmpty(eventDetails.VenueName)) locationParts.Add(eventDetails.VenueName);
                if (!string.IsNullOrEmpty(eventDetails.StreetAddress)) locationParts.Add(eventDetails.StreetAddress);
                if (!string.IsNullOrEmpty(eventDetails.Ward)) locationParts.Add(eventDetails.Ward);
                if (!string.IsNullOrEmpty(eventDetails.District)) locationParts.Add(eventDetails.District);
                if (!string.IsNullOrEmpty(eventDetails.Province)) locationParts.Add(eventDetails.Province);
                
                if (locationParts.Any())
                    location = string.Join(", ", locationParts);
            }
            
            return new
            {
                eventId = eventData.EventId,
                title = eventData.Title,
                description = eventData.Description,
                startTime = eventData.StartTime,
                endTime = eventData.EndTime,
                location = location,
                category = eventData.Category,
                status = eventData.Status,
                hostName = eventData.Host?.FullName,
                hostEmail = eventData.Host?.Email,
                eventDetails = eventDetails,
                eventImage = eventDetails?.images?.FirstOrDefault() ?? eventDetails?.EventImage,
                backgroundImage = eventDetails?.images?.Skip(1).FirstOrDefault() ?? eventDetails?.BackgroundImage,
                ticketTypes = CreateTicketTypeDtos(eventData.TicketTypes)
            };
        }

        public object MapToEventDetailDto(Event eventData)
        {
            var eventDetails = eventData.GetEventDetails();
            var venueLayout = eventData.GetVenueLayout();
            
            // Construct location from address components if Location field is empty
            var location = eventData.Location;
            if (string.IsNullOrEmpty(location) && eventDetails != null)
            {
                var locationParts = new List<string>();
                if (!string.IsNullOrEmpty(eventDetails.VenueName)) locationParts.Add(eventDetails.VenueName);
                if (!string.IsNullOrEmpty(eventDetails.StreetAddress)) locationParts.Add(eventDetails.StreetAddress);
                if (!string.IsNullOrEmpty(eventDetails.Ward)) locationParts.Add(eventDetails.Ward);
                if (!string.IsNullOrEmpty(eventDetails.District)) locationParts.Add(eventDetails.District);
                if (!string.IsNullOrEmpty(eventDetails.Province)) locationParts.Add(eventDetails.Province);
                
                if (locationParts.Any())
                    location = string.Join(", ", locationParts);
            }
            
            return new
            {
                eventId = eventData.EventId,
                title = eventData.Title,
                description = eventData.Description,
                startTime = eventData.StartTime,
                endTime = eventData.EndTime,
                location = location,
                eventMode = eventData.EventMode,
                eventType = eventData.EventType,
                category = eventData.Category,
                status = eventData.Status,
                createdAt = eventData.CreatedAt,
                hostId = eventData.HostId,
                hostName = eventData.Host?.FullName,
                hostEmail = eventData.Host?.Email,
                eventDetails = eventDetails,
                eventImage = eventDetails?.EventImage,
                backgroundImage = eventDetails?.BackgroundImage,
                venueLayout = venueLayout,
                ticketTypes = CreateTicketTypeDtos(eventData.TicketTypes)
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
                ticketTypeId = tt.TicketTypeId,
                typeName = tt.TypeName,
                price = tt.Price,
                quantity = tt.Quantity,
                minOrder = tt.MinOrder,
                maxOrder = tt.MaxOrder,
                saleStart = tt.SaleStart,
                saleEnd = tt.SaleEnd,
                status = tt.Status
            });
        }
    }
}


