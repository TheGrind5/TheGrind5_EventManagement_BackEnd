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
            var organizerInfo = eventData.GetOrganizerInfo(); // Lấy OrganizerInfo từ JSON
            
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
                // QUAN TRỌNG: Ưu tiên EventImage/BackgroundImage trực tiếp từ EventDetails, không fallback nếu empty
                // Chỉ fallback về images array nếu EventImage/BackgroundImage là null (không phải empty string)
                eventImage = !string.IsNullOrEmpty(eventDetails?.EventImage) 
                    ? eventDetails.EventImage 
                    : (eventDetails?.images != null && eventDetails.images.Length > 0) 
                        ? eventDetails.images.FirstOrDefault() 
                        : null,
                backgroundImage = !string.IsNullOrEmpty(eventDetails?.BackgroundImage) 
                    ? eventDetails.BackgroundImage 
                    : (eventDetails?.images != null && eventDetails.images.Length > 1) 
                        ? eventDetails.images.Skip(1).FirstOrDefault() 
                        : null,
                // Trả về organizerInfo để frontend có thể sử dụng
                organizerInfo = organizerInfo != null ? new { 
                    organizerName = organizerInfo.OrganizerName ?? "", 
                    organizerInfo = organizerInfo.OrganizerInfo ?? "", 
                    organizerLogo = organizerInfo.OrganizerLogo ?? "" 
                } : new { organizerName = "", organizerInfo = "", organizerLogo = "" },
                ticketTypes = CreateTicketTypeDtos(eventData.TicketTypes),
                createdAt = eventData.CreatedAt,
                updatedAt = eventData.UpdatedAt // QUAN TRỌNG: Trả về UpdatedAt để frontend cache bust
            };
        }

        public object MapToEventDetailDto(Event eventData)
        {
            var eventDetails = eventData.GetEventDetails();
            var venueLayout = eventData.GetVenueLayout();
            var organizerInfo = eventData.GetOrganizerInfo(); // QUAN TRỌNG: Lấy OrganizerInfo từ JSON
            
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
                updatedAt = eventData.UpdatedAt, // QUAN TRỌNG: Trả về UpdatedAt để frontend cache bust
                hostId = eventData.HostId,
                hostName = eventData.Host?.FullName,
                hostEmail = eventData.Host?.Email,
                eventDetails = eventDetails,
                // QUAN TRỌNG: Ưu tiên EventImage/BackgroundImage trực tiếp từ EventDetails, không fallback nếu empty
                // Chỉ fallback về images array nếu EventImage/BackgroundImage là null (không phải empty string)
                eventImage = !string.IsNullOrEmpty(eventDetails?.EventImage) 
                    ? eventDetails.EventImage 
                    : (eventDetails?.images != null && eventDetails.images.Length > 0) 
                        ? eventDetails.images.FirstOrDefault() 
                        : null,
                backgroundImage = !string.IsNullOrEmpty(eventDetails?.BackgroundImage) 
                    ? eventDetails.BackgroundImage 
                    : (eventDetails?.images != null && eventDetails.images.Length > 1) 
                        ? eventDetails.images.Skip(1).FirstOrDefault() 
                        : null,
                venueLayout = venueLayout,
                // QUAN TRỌNG: Trả về organizerInfo riêng biệt để frontend có thể load khi chỉnh sửa
                organizerInfo = organizerInfo != null ? new { 
                    organizerName = organizerInfo.OrganizerName ?? "", 
                    organizerInfo = organizerInfo.OrganizerInfo ?? "", 
                    organizerLogo = organizerInfo.OrganizerLogo ?? "" 
                } : new { organizerName = "", organizerInfo = "", organizerLogo = "" },
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


