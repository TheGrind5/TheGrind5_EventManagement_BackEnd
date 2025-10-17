using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                Console.WriteLine("Getting all events...");
                var events = await _eventService.GetAllEventsAsync();
                Console.WriteLine($"Found {events.Count} events");
                
                var eventDtos = events.Select(e => _eventService.MapToEventDto(e)).ToList();
                Console.WriteLine($"Mapped {eventDtos.Count} event DTOs");
                
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllEvents: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sự kiện", error = ex.Message });
            }
        }
   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID sự kiện không hợp lệ" });

                var eventData = await _eventService.GetEventByIdAsync(id);
                if (eventData == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                return Ok(_eventService.MapToEventDetailDto(eventData));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin sự kiện", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var eventData = _eventService.MapFromCreateEventRequest(request, userId.Value);
                var createdEvent = await _eventService.CreateEventAsync(eventData);

                return Ok(new { message = "Tạo sự kiện thành công", eventId = createdEvent?.EventId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo sự kiện", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
        {
            try
            {
                var eventData = new Event
                {
                    EventId = id,
                    Title = request.Title,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Location = request.Location,
                    Category = request.Category,
                    Status = "Open"
                };

                var updatedEvent = await _eventService.UpdateEventAsync(id, eventData);
                if (updatedEvent == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                return Ok(new { message = "Cập nhật sự kiện thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _eventService.DeleteEventAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                return Ok(new { message = "Xóa sự kiện thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi xóa sự kiện", error = ex.Message });
            }
        }

        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetEventsByHost(int hostId)
        {
            try
            {
                var events = await _eventService.GetEventsByHostAsync(hostId);
                var eventDtos = events.Select(e => _eventService.MapToEventDto(e)).ToList();
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sự kiện của host", error = ex.Message });
            }
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedSampleEvents()
        {
            try
            {
                // This endpoint is kept for development/testing purposes only
                // In production, this should be removed or restricted to admin users only
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Only allow seeding if no events exist (first time setup)
                var existingEvents = await _eventService.GetAllEventsAsync();
                if (existingEvents.Any())
                {
                    return BadRequest(new { message = "Sample data seeding is only allowed when no events exist" });
                }

                // Tạo một số sự kiện mẫu cho development
                var sampleEvents = new List<Event>
                {
                    new Event
                    {
                        Title = "Tech Conference 2024",
                        Description = "Hội nghị công nghệ lớn nhất năm",
                        StartTime = DateTime.Now.AddDays(7),
                        EndTime = DateTime.Now.AddDays(7).AddHours(8),
                        Location = "Hà Nội",
                        Category = "Technology",
                        Status = "Open",
                        HostId = userId.Value
                    },
                    new Event
                    {
                        Title = "Music Festival",
                        Description = "Lễ hội âm nhạc với nhiều nghệ sĩ nổi tiếng",
                        StartTime = DateTime.Now.AddDays(14),
                        EndTime = DateTime.Now.AddDays(14).AddHours(12),
                        Location = "TP.HCM",
                        Category = "Music",
                        Status = "Open",
                        HostId = userId.Value
                    },
                    new Event
                    {
                        Title = "Startup Pitch Competition",
                        Description = "Cuộc thi thuyết trình ý tưởng khởi nghiệp",
                        StartTime = DateTime.Now.AddDays(21),
                        EndTime = DateTime.Now.AddDays(21).AddHours(6),
                        Location = "Đà Nẵng",
                        Category = "Business",
                        Status = "Open",
                        HostId = userId.Value
                    }
                };

                foreach (var eventData in sampleEvents)
                {
                    await _eventService.CreateEventAsync(eventData);
                }

                return Ok(new { 
                    message = "Đã tạo thành công các sự kiện mẫu cho development", 
                    count = sampleEvents.Count,
                    note = "This is for development purposes only"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo sự kiện mẫu", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
