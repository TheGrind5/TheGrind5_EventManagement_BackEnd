using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventDBContext _context;

        public EventController(EventDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Host)
                    .Include(e => e.TicketTypes)
                    .Where(e => e.Status == "Active")
                    .Select(e => new
                    {
                        e.EventId,
                        e.Title,
                        e.Description,
                        e.StartTime,
                        e.EndTime,
                        e.Location,
                        e.Category,
                        e.Status,
                        HostName = e.Host.FullName,
                        TicketTypes = e.TicketTypes.Select(tt => new
                        {
                            tt.TicketTypeId,
                            tt.TypeName,
                            tt.Price,
                            tt.Quantity,
                            tt.Status
                        })
                    })
                    .ToListAsync();

                return Ok(events);
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
                var eventData = await _context.Events
                    .Include(e => e.Host)
                    .Include(e => e.TicketTypes)
                    .Where(e => e.EventId == id)
                    .Select(e => new
                    {
                        e.EventId,
                        e.Title,
                        e.Description,
                        e.StartTime,
                        e.EndTime,
                        e.Location,
                        e.Category,
                        e.Status,
                        e.CreatedAt,
                        HostName = e.Host.FullName,
                        HostEmail = e.Host.Email,
                        TicketTypes = e.TicketTypes.Select(tt => new
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
                    })
                    .FirstOrDefaultAsync();

                if (eventData == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                return Ok(eventData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin sự kiện", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] DTOs.CreateEventRequest request)
        {
            try
            {
                // Tạm thời sử dụng user ID = 1 (admin) làm host
                // Sau này sẽ lấy từ authentication token
                var eventData = new Event
                {
                    HostId = 1, // Tạm thời
                    Title = request.Title,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Location = request.Location,
                    Category = request.Category,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Events.Add(eventData);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tạo sự kiện thành công", eventId = eventData.EventId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo sự kiện", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] DTOs.UpdateEventRequest request)
        {
            try
            {
                var eventData = await _context.Events.FindAsync(id);
                if (eventData == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                eventData.Title = request.Title;
                eventData.Description = request.Description;
                eventData.StartTime = request.StartTime;
                eventData.EndTime = request.EndTime;
                eventData.Location = request.Location;
                eventData.Category = request.Category;
                eventData.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật sự kiện thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var eventData = await _context.Events.FindAsync(id);
                if (eventData == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                eventData.Status = "Deleted";
                eventData.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa sự kiện thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi xóa sự kiện", error = ex.Message });
            }
        }
    }
}
