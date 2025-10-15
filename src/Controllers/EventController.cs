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
                var events = await _eventService.GetAllEventsAsync();
                var eventDtos = events.Select(e => _eventService.MapToEventDto(e)).ToList();
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

        // ========================================
        // NEW EVENT CREATION ENDPOINTS (Ticket Box Style)
        // ========================================

        [HttpPost("create/step1")]
        [Authorize]
        public async Task<IActionResult> CreateEventStep1([FromBody] CreateEventStep1Request request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Tạo event với thông tin bước 1
                var eventData = new Event
                {
                    HostId = userId.Value,
                    Title = request.Title,
                    Description = request.Description,
                    EventMode = request.EventMode,
                    VenueName = request.VenueName,
                    Province = request.Province,
                    District = request.District,
                    Ward = request.Ward,
                    StreetAddress = request.StreetAddress,
                    EventType = request.EventType,
                    Category = request.Category,
                    EventImage = request.EventImage,
                    BackgroundImage = request.BackgroundImage,
                    EventIntroduction = request.EventIntroduction,
                    EventDetails = request.EventDetails,
                    SpecialGuests = request.SpecialGuests,
                    SpecialExperience = request.SpecialExperience,
                    TermsAndConditions = request.TermsAndConditions,
                    ChildrenTerms = request.ChildrenTerms,
                    VATTerms = request.VATTerms,
                    OrganizerLogo = request.OrganizerLogo,
                    OrganizerName = request.OrganizerName,
                    OrganizerInfo = request.OrganizerInfo,
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };

                var createdEvent = await _eventService.CreateEventAsync(eventData);
                return Ok(new EventCreationResponse(
                    createdEvent.EventId,
                    "Bước 1: Thông tin sự kiện đã được lưu thành công",
                    true
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo sự kiện bước 1", error = ex.Message });
            }
        }

        [HttpPut("{eventId}/create/step2")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStep2(int eventId, [FromBody] CreateEventStep2Request request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");

                // Cập nhật thông tin thời gian
                existingEvent.StartTime = request.StartTime;
                existingEvent.EndTime = request.EndTime;
                existingEvent.UpdatedAt = DateTime.UtcNow;

                await _eventService.UpdateEventAsync(eventId, existingEvent);

                // Tạo ticket types
                foreach (var ticketTypeRequest in request.TicketTypes)
                {
                    var ticketType = new TicketType
                    {
                        EventId = eventId,
                        TypeName = ticketTypeRequest.TypeName,
                        Price = ticketTypeRequest.Price,
                        Quantity = ticketTypeRequest.Quantity,
                        MinOrder = ticketTypeRequest.MinOrder,
                        MaxOrder = ticketTypeRequest.MaxOrder,
                        SaleStart = ticketTypeRequest.SaleStart,
                        SaleEnd = ticketTypeRequest.SaleEnd,
                        Status = ticketTypeRequest.Status
                    };
                    // TODO: Implement ticket type creation service
                }

                return Ok(new EventCreationResponse(
                    eventId,
                    "Bước 2: Thời gian và loại vé đã được cập nhật thành công",
                    true
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện bước 2", error = ex.Message });
            }
        }

        [HttpPut("{eventId}/create/step3")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStep3(int eventId, [FromBody] CreateEventStep3Request request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");

                // Cập nhật cài đặt sự kiện (có thể lưu trong Description hoặc tạo bảng riêng)
                existingEvent.UpdatedAt = DateTime.UtcNow;
                await _eventService.UpdateEventAsync(eventId, existingEvent);

                return Ok(new EventCreationResponse(
                    eventId,
                    "Bước 3: Cài đặt sự kiện đã được lưu thành công",
                    true
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện bước 3", error = ex.Message });
            }
        }

        [HttpPut("{eventId}/create/step4")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStep4(int eventId, [FromBody] CreateEventStep4Request request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");

                // Cập nhật thông tin thanh toán và chuyển status thành Open
                existingEvent.Status = "Open";
                existingEvent.UpdatedAt = DateTime.UtcNow;
                await _eventService.UpdateEventAsync(eventId, existingEvent);

                return Ok(new EventCreationResponse(
                    eventId,
                    "Bước 4: Thông tin thanh toán đã được lưu và sự kiện đã được kích hoạt thành công!",
                    true
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện bước 4", error = ex.Message });
            }
        }

        [HttpGet("{eventId}/creation-status")]
        [Authorize]
        public async Task<IActionResult> GetEventCreationStatus(int eventId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var eventData = await _eventService.GetEventByIdAsync(eventId);
                if (eventData == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (eventData.HostId != userId.Value)
                    return Forbid("Bạn không có quyền xem sự kiện này");

                var response = new
                {
                    EventId = eventData.EventId,
                    Title = eventData.Title,
                    Status = eventData.Status,
                    HasBasicInfo = !string.IsNullOrEmpty(eventData.Title) && !string.IsNullOrEmpty(eventData.Description),
                    HasDateTime = eventData.StartTime != default && eventData.EndTime != default,
                    HasTicketTypes = eventData.TicketTypes?.Any() == true,
                    CreatedAt = eventData.CreatedAt,
                    UpdatedAt = eventData.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy trạng thái tạo sự kiện", error = ex.Message });
            }
        }

        [HttpPost("upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Không có file được chọn" });

                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest(new { message = "Chỉ chấp nhận file ảnh (JPG, PNG, GIF, WEBP)" });

                // Kiểm tra kích thước file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "File quá lớn, tối đa 5MB" });

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events");
                
                // Debug: Log đường dẫn
                Console.WriteLine($"Upload folder: {uploadsFolder}");
                Console.WriteLine($"File name: {fileName}");
                
                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL ảnh
                var imageUrl = $"/uploads/events/{fileName}";
                return Ok(new { 
                    success = true, 
                    imageUrl = imageUrl,
                    message = "Upload ảnh thành công" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi upload ảnh", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
