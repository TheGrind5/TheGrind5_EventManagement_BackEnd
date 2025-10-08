using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                
                var eventDtos = events.Select(e => new
                {
                    e.EventId,
                    e.Title,
                    e.Description,
                    e.StartTime,
                    e.EndTime,
                    e.Location,
                    e.Category,
                    e.Status,
                    HostName = e.Host?.FullName,
                    TicketTypes = e.TicketTypes?.Where(tt => tt.Status == "Active").Select(tt => new
                    {
                        tt.TicketTypeId,
                        tt.TypeName,
                        tt.Price,
                        tt.Quantity,
                        tt.Status
                    })
                }).ToList();

                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sự kiện", error = ex.Message });
            }
        }
   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var eventData = await _eventService.GetEventByIdAsync(id);

                if (eventData == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                var eventDto = new
                {
                    eventData.EventId,
                    eventData.Title,
                    eventData.Description,
                    eventData.StartTime,
                    eventData.EndTime,
                    eventData.Location,
                    eventData.Category,
                    eventData.Status,
                    eventData.CreatedAt,
                    HostName = eventData.Host?.FullName,
                    HostEmail = eventData.Host?.Email,
                    TicketTypes = eventData.TicketTypes?.Where(tt => tt.Status == "Active").Select(tt => new
                    {
                        tt.TicketTypeId,
                        tt.TypeName,
                        tt.Price,
                        tt.Quantity,
                        tt.MinOrder,
                        tt.MaxOrder,
                        tt.SaleStart,
                        tt.SaleEnd,
                        tt.Status
                    })
                };

                return Ok(eventDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin sự kiện", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] DTOs.CreateEventRequest request)
        {
            try
            {
                // Lấy user ID từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var eventData = new Event
                {
                    HostId = userId,
                    Title = request.Title,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Location = request.Location,
                    Category = request.Category,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

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
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] DTOs.UpdateEventRequest request)
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
                    Status = "Active"
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
                
                var eventDtos = events.Select(e => new
                {
                    e.EventId,
                    e.Title,
                    e.Description,
                    e.StartTime,
                    e.EndTime,
                    e.Location,
                    e.Category,
                    e.Status,
                    HostName = e.Host?.FullName,
                    TicketTypes = e.TicketTypes?.Select(tt => new
                    {
                        tt.TicketTypeId,
                        tt.TypeName,
                        tt.Price,
                        tt.Quantity,
                        tt.Status
                    })
                }).ToList();

                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sự kiện của host", error = ex.Message });
            }
        }

    }
}
