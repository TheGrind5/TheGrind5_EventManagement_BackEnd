using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Text.Json;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IFileManagementService _fileManagementService;

        public EventController(IEventService eventService, IFileManagementService fileManagementService)
        {
            _eventService = eventService;
            _fileManagementService = fileManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? category = null,
            [FromQuery] string? eventMode = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                Console.WriteLine($"Getting events - SearchTerm: {searchTerm}, Category: {category}, Page: {page}, PageSize: {pageSize}");
                
                // Create search request
                var searchRequest = new EventSearchRequest
                {
                    SearchTerm = searchTerm,
                    Category = category,
                    EventMode = eventMode,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = status,
                    Page = page,
                    PageSize = pageSize
                };
                
                var pagedEvents = await _eventService.SearchEventsAsync(searchRequest);
                Console.WriteLine($"Found {pagedEvents.TotalCount} total events, returning {pagedEvents.Data.Count} for page {page}");
                
                var eventDtos = pagedEvents.Data.Select(e => _eventService.MapToEventDto(e)).ToList();
                
                var response = new PagedResponse<object>(
                    eventDtos,
                    pagedEvents.TotalCount,
                    pagedEvents.Page,
                    pagedEvents.PageSize
                );
                
                return Ok(response);
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
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventStep1Request request)
        {
            try
            {
                Console.WriteLine($"=== UpdateEvent Debug ===");
                Console.WriteLine($"EventId: {id}");
                Console.WriteLine($"Title: {request.Title}");
                Console.WriteLine($"EventMode: {request.EventMode}");
                Console.WriteLine($"Request EventImage: {request.EventImage ?? "NULL"}");
                Console.WriteLine($"Request BackgroundImage: {request.BackgroundImage ?? "NULL"}");
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiá»ƒm tra quyá»n sá»Ÿ há»¯u event
                var existingEvent = await _eventService.GetEventByIdAsync(id);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");

                Console.WriteLine($"Found event: {existingEvent.Title}, HostId: {existingEvent.HostId}");

                // Cáº­p nháº­t thÃ´ng tin cÆ¡ báº£n cá»§a event
                existingEvent.Title = request.Title ?? existingEvent.Title;
                existingEvent.Description = request.Description ?? existingEvent.Description;
                existingEvent.Category = request.Category ?? existingEvent.Category;
                existingEvent.EventMode = request.EventMode ?? existingEvent.EventMode;
                existingEvent.Location = request.Location ?? existingEvent.Location;
                existingEvent.UpdatedAt = DateTime.UtcNow;

                // Cập nhật EventDetails - MERGE với data cũ để không mất thông tin
                var existingDetails = existingEvent.GetEventDetails();
                Console.WriteLine($"Existing EventImage: {existingDetails?.EventImage ?? "NULL"}");
                Console.WriteLine($"Existing BackgroundImage: {existingDetails?.BackgroundImage ?? "NULL"}");
                
                // Helper method để normalize đường dẫn ảnh: /uploads/events/ -> /assets/images/events/
                string NormalizeImagePath(string? imagePath)
                {
                    if (string.IsNullOrEmpty(imagePath)) return string.Empty;
                    // Chuyá»ƒn Ä‘á»•i tá»« /uploads/events/ thÃ nh /assets/images/events/
                    return imagePath.Replace("/uploads/events/", "/assets/images/events/")
                                   .Replace("/uploads/avatars/", "/assets/images/avatars/");
                }
                
                // Xá»­ lÃ½ images array - Æ°u tiÃªn EventImage vÃ  BackgroundImage tá»« request
                var imagesList = new List<string>();
                
                // QUAN TRá»ŒNG: Xá»­ lÃ½ EventImage - náº¿u cÃ³ giÃ¡ trá»‹ má»›i (dÃ¹ lÃ  empty string), dÃ¹ng giÃ¡ trá»‹ má»›i
                // Náº¿u request.EventImage lÃ  null hoáº·c empty, thÃ¬ giá»¯ giÃ¡ trá»‹ cÅ©
                var normalizedEventImage = string.Empty;
                if (request.EventImage != null) // CÃ³ field trong request (dÃ¹ cÃ³ thá»ƒ lÃ  empty)
                {
                    normalizedEventImage = NormalizeImagePath(request.EventImage);
                    Console.WriteLine($"Processing request EventImage (may be empty): '{request.EventImage}' -> '{normalizedEventImage}'");
                    if (!string.IsNullOrEmpty(normalizedEventImage))
                    {
                        imagesList.Add(normalizedEventImage);
                    }
                }
                else
                {
                    // Náº¿u request khÃ´ng cÃ³ EventImage (null), giá»¯ EventImage cÅ© (Ä‘Ã£ normalize)
                    var existingEventImage = NormalizeImagePath(existingDetails?.EventImage);
                    if (!string.IsNullOrEmpty(existingEventImage))
                    {
                        normalizedEventImage = existingEventImage;
                        imagesList.Add(existingEventImage);
                    }
                    else if (existingDetails?.images != null && existingDetails.images.Length > 0)
                    {
                        // Náº¿u cÃ³ images array, normalize vÃ  láº¥y pháº§n tá»­ Ä‘áº§u tiÃªn
                        normalizedEventImage = NormalizeImagePath(existingDetails.images[0]);
                        if (!string.IsNullOrEmpty(normalizedEventImage))
                        {
                            imagesList.Add(normalizedEventImage);
                        }
                    }
                }
                
                // QUAN TRá»ŒNG: Xá»­ lÃ½ BackgroundImage - náº¿u cÃ³ giÃ¡ trá»‹ má»›i (dÃ¹ lÃ  empty string), dÃ¹ng giÃ¡ trá»‹ má»›i
                var normalizedBackgroundImage = string.Empty;
                if (request.BackgroundImage != null) // CÃ³ field trong request (dÃ¹ cÃ³ thá»ƒ lÃ  empty)
                {
                    normalizedBackgroundImage = NormalizeImagePath(request.BackgroundImage);
                    Console.WriteLine($"Processing request BackgroundImage (may be empty): '{request.BackgroundImage}' -> '{normalizedBackgroundImage}'");
                    if (!string.IsNullOrEmpty(normalizedBackgroundImage))
                    {
                        imagesList.Add(normalizedBackgroundImage);
                    }
                }
                else
                {
                    // Náº¿u request khÃ´ng cÃ³ BackgroundImage (null), giá»¯ BackgroundImage cÅ© (Ä‘Ã£ normalize)
                    var existingBgImage = NormalizeImagePath(existingDetails?.BackgroundImage);
                    if (!string.IsNullOrEmpty(existingBgImage))
                    {
                        normalizedBackgroundImage = existingBgImage;
                        imagesList.Add(existingBgImage);
                    }
                    else if (existingDetails?.images != null && existingDetails.images.Length > 1)
                    {
                        // Náº¿u cÃ³ images array, normalize vÃ  láº¥y pháº§n tá»­ thá»© 2
                        normalizedBackgroundImage = NormalizeImagePath(existingDetails.images[1]);
                        if (!string.IsNullOrEmpty(normalizedBackgroundImage))
                        {
                            imagesList.Add(normalizedBackgroundImage);
                        }
                    }
                }
                
                // Äáº£m báº£o EventImage vÃ  BackgroundImage Ä‘Æ°á»£c sync vá»›i images array (Ä‘Ã£ normalize)
                // finalEventImage vÃ  finalBackgroundImage cÃ³ thá»ƒ lÃ  empty string náº¿u user xÃ³a áº£nh
                var finalEventImage = normalizedEventImage ?? NormalizeImagePath(existingDetails?.EventImage ?? (imagesList.Count > 0 ? imagesList[0] : null));
                var finalBackgroundImage = normalizedBackgroundImage ?? NormalizeImagePath(existingDetails?.BackgroundImage ?? (imagesList.Count > 1 ? imagesList[1] : null));
                
                // Cáº­p nháº­t imagesList Ä‘á»ƒ Ä‘áº£m báº£o sync (sau khi Ä‘Ã£ normalize)
                if (!string.IsNullOrEmpty(finalEventImage) && (imagesList.Count == 0 || imagesList[0] != finalEventImage))
                {
                    if (imagesList.Count > 0) imagesList[0] = finalEventImage;
                    else imagesList.Insert(0, finalEventImage);
                }
                
                if (!string.IsNullOrEmpty(finalBackgroundImage))
                {
                    if (imagesList.Count > 1) imagesList[1] = finalBackgroundImage;
                    else if (imagesList.Count == 1) imagesList.Add(finalBackgroundImage);
                    else
                    {
                        // Nếu không có eventImage, thêm empty string để giữ vị trí
                        if (imagesList.Count == 0) imagesList.Add("");
                        imagesList.Add(finalBackgroundImage);
                    }
                }
                
                // Normalize các ảnh còn lại trong images array (nếu có)
                for (int i = 2; i < existingDetails?.images?.Length; i++)
                {
                    var normalizedImg = NormalizeImagePath(existingDetails.images[i]);
                    if (!string.IsNullOrEmpty(normalizedImg) && !imagesList.Contains(normalizedImg))
                    {
                        imagesList.Add(normalizedImg);
                    }
                }
                
                var eventDetails = new EventDetailsData
                {
                    // Giữ nguyên data cũ nếu request không có
                    VenueName = request.VenueName ?? existingDetails?.VenueName,
                    Province = request.Province ?? existingDetails?.Province,
                    District = request.District ?? existingDetails?.District,
                    Ward = request.Ward ?? existingDetails?.Ward,
                    StreetAddress = request.StreetAddress ?? existingDetails?.StreetAddress,
                    // QUAN TRỌNG: Luôn cập nhật EventImage và BackgroundImage (đã normalize về /assets/images/events/)
                    EventImage = finalEventImage,
                    BackgroundImage = finalBackgroundImage,
                    // Cập nhật images array - đảm bảo sync với EventImage và BackgroundImage (đã normalize)
                    images = imagesList.Where(img => !string.IsNullOrEmpty(img)).ToArray(),
                    introduction = existingDetails?.introduction ?? request.EventIntroduction,
                    specialGuests = existingDetails?.specialGuests,
                    EventIntroduction = request.EventIntroduction ?? existingDetails?.EventIntroduction,
                    EventDetails = request.EventDetails ?? existingDetails?.EventDetails,
                    SpecialGuests = request.SpecialGuests ?? existingDetails?.SpecialGuests,
                    SpecialExperience = request.SpecialExperience ?? existingDetails?.SpecialExperience
                };
                
                Console.WriteLine($"Updated EventImage: {eventDetails.EventImage}");
                Console.WriteLine($"Updated BackgroundImage: {eventDetails.BackgroundImage}");
                Console.WriteLine($"Updated images array: [{string.Join(", ", eventDetails.images ?? new string[0])}]");

                existingEvent.SetEventDetails(eventDetails);
                
                // Cập nhật OrganizerInfo riêng
                if (!string.IsNullOrEmpty(request.OrganizerName) || !string.IsNullOrEmpty(request.OrganizerInfo) || !string.IsNullOrEmpty(request.OrganizerLogo))
                {
                    var organizerInfo = new OrganizerInfoData
                    {
                        OrganizerName = request.OrganizerName,
                        OrganizerInfo = request.OrganizerInfo,
                        OrganizerLogo = NormalizeImagePath(request.OrganizerLogo) // Normalize organizer logo
                    };
                    
                    existingEvent.SetOrganizerInfo(organizerInfo);
                }

                // Cập nhật event vào database
                var updatedEvent = await _eventService.UpdateEventAsync(id, existingEvent);
                if (updatedEvent == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                // Debug: Kiểm tra data sau khi update
                var verifyDetails = updatedEvent.GetEventDetails();
                Console.WriteLine($"=== After Update Verification ===");
                Console.WriteLine($"EventId: {updatedEvent.EventId}");
                Console.WriteLine($"EventImage in DB: {verifyDetails?.EventImage}");
                Console.WriteLine($"BackgroundImage in DB: {verifyDetails?.BackgroundImage}");
                Console.WriteLine($"UpdatedAt: {updatedEvent.UpdatedAt}");
                
                // Trả về event đã update để frontend có thể reload ngay
                var responseDto = _eventService.MapToEventDetailDto(updatedEvent);
                Console.WriteLine("Event updated successfully");

                return Ok(new { 
                    message = "Cập nhật sự kiện thành công",
                    eventData = responseDto // Trả về event data để frontend có thể update state ngay
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu event
                var eventData = await _eventService.GetEventByIdAsync(id);
                if (eventData == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (eventData.HostId != userId.Value)
                    return Forbid("Bạn không có quyền xóa sự kiện này");

                // Kiểm tra xem event có vé đã được bán thành công chưa (Status = "Paid")
                var hasTicketsSold = await _eventService.CheckHasPaidTicketsAsync(id);
                if (hasTicketsSold)
                {
                    return BadRequest(new { 
                        message = "Không thể xóa sự kiện đã có vé được mua thành công. Hãy liên hệ hỗ trợ nếu muốn hủy sự kiện." 
                    });
                }

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

        [HttpGet("my-events")]
        [Authorize]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var events = await _eventService.GetEventsByHostAsync(userId.Value);
                var eventDtos = events.Select(e => _eventService.MapToEventDto(e)).ToList();
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sự kiện của tôi", error = ex.Message });
            }
        }

        [HttpGet("{id}/edit-status")]
        [Authorize]
        public async Task<IActionResult> GetEventEditStatus(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var eventData = await _eventService.GetEventByIdAsync(id);
                if (eventData == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (eventData.HostId != userId.Value)
                    return Forbid("Bạn không có quyền xem sự kiện này");

                // Kiểm tra xem event có ticket đã được bán chưa
                var hasTicketsSold = await _eventService.CheckHasTicketsSoldAsync(id);

                    // Kiểm tra xem còn cách thời gian bắt đầu bao nhiêu giờ
                var hoursUntilStart = (eventData.StartTime - DateTime.UtcNow).TotalHours;
                var canEditTimeLocation = hoursUntilStart >= 48;
                var timeUntilEvent = hoursUntilStart > 0 ? Math.Round(hoursUntilStart, 2) : 0;

                return Ok(new
                {
                    eventId = id,
                    canEdit = !hasTicketsSold,
                    canEditTimeLocation = canEditTimeLocation,
                    hasTicketsSold = hasTicketsSold,
                    hoursUntilStart = timeUntilEvent,
                    message = hasTicketsSold 
                        ? "Không thể chỉnh sửa sự kiện đã có vé được bán" 
                        : !canEditTimeLocation
                            ? $"Chỉ có thể chỉnh sửa thời gian và địa điểm trước 48 giờ. Còn {Math.Round(hoursUntilStart, 1)} giờ đến sự kiện."
                            : "Có thể chỉnh sửa sự kiện"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi kiểm tra trạng thái chỉnh sửa", error = ex.Message });
            }
        }

        // ========================================
        // NEW EVENT CREATION ENDPOINTS (Ticket Box Style)
        // ========================================

        // IMPORTANT: create/complete phải đặt trước create/step1 để tránh route conflict
        // (Route cụ thể hơn phải đặt trước route generic hơn)

        [HttpPost("create/complete")]
        [Authorize]
        public async Task<IActionResult> CreateCompleteEvent([FromBody] CreateCompleteEventRequest request)
        {
            try
            {
                Console.WriteLine($"=== CreateCompleteEvent Debug ===");
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // VALIDATION: Kiểm tra tất cả 5 bước trước khi tạo event
                var validationErrors = new List<string>();

                // Bước 1: Kiểm tra thông tin cơ bản
                if (string.IsNullOrWhiteSpace(request.Title))
                    validationErrors.Add("Bước 1: Thiếu tên sự kiện (Title)");
                
                if (string.IsNullOrWhiteSpace(request.Description))
                    validationErrors.Add("Bước 1: Thiếu mô tả sự kiện (Description)");
                
                if (string.IsNullOrWhiteSpace(request.Category))
                    validationErrors.Add("Bước 1: Thiếu danh mục sự kiện (Category)");
                
                if (string.IsNullOrWhiteSpace(request.EventMode))
                    validationErrors.Add("Bước 1: Thiếu chế độ sự kiện (EventMode)");

                // Kiểm tra thông tin địa chỉ
                if (request.EventMode == "Offline")
                {
                    if (string.IsNullOrWhiteSpace(request.VenueName))
                        validationErrors.Add("Bước 1: Thiếu tên địa điểm (VenueName) cho sự kiện Offline");
                    
                    if (string.IsNullOrWhiteSpace(request.Province))
                        validationErrors.Add("Bước 1: Thiếu tỉnh/thành phố (Province) cho sự kiện Offline");
                    
                    if (string.IsNullOrWhiteSpace(request.StreetAddress))
                        validationErrors.Add("Bước 1: Thiếu địa chỉ đường (StreetAddress) cho sự kiện Offline");
                }
                else if (request.EventMode == "Online")
                {
                    if (string.IsNullOrWhiteSpace(request.Location))
                        validationErrors.Add("Bước 1: Thiếu link sự kiện (Location) cho sự kiện Online");
                }

                // Kiểm tra thông tin tổ chức
                if (string.IsNullOrWhiteSpace(request.OrganizerName))
                    validationErrors.Add("Bước 1: Thiếu tên tổ chức (OrganizerName)");
                
                if (string.IsNullOrWhiteSpace(request.OrganizerInfo))
                    validationErrors.Add("Bước 1: Thiếu thông tin tổ chức (OrganizerInfo)");

                // Bước 2: Kiểm tra thời gian và loại vé
                if (request.StartTime == default || request.StartTime == DateTime.MinValue)
                    validationErrors.Add("Bước 2: Thiếu thời gian bắt đầu (StartTime)");
                
                if (request.EndTime == default || request.EndTime == DateTime.MinValue)
                    validationErrors.Add("Bước 2: Thiếu thời gian kết thúc (EndTime)");
                
                if (request.StartTime != default && request.EndTime != default && request.StartTime >= request.EndTime)
                    validationErrors.Add("Bước 2: Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                
                // Kiểm tra ticket types
                if (request.TicketTypes == null || !request.TicketTypes.Any())
                    validationErrors.Add("Bước 2: Thiếu loại vé (cần ít nhất một loại vé)");
                else
                {
                    int ticketIndex = 0;
                    foreach (var ticketType in request.TicketTypes)
                    {
                        int i = ticketIndex++;
                        if (string.IsNullOrWhiteSpace(ticketType.TypeName))
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} thiếu tên (TypeName)");
                        
                        if (ticketType.Price < 0)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có giá không hợp lệ (Price)");
                        
                        if (ticketType.Quantity < 0)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có số lượng không hợp lệ (Quantity)");
                        
                        if (ticketType.MinOrder < 1)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có đơn hàng tối thiểu không hợp lệ (MinOrder)");
                        
                        if (ticketType.MaxOrder < 1)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có đơn hàng tối đa không hợp lệ (MaxOrder)");
                        
                        if (ticketType.MinOrder > ticketType.MaxOrder)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có MinOrder lớn hơn MaxOrder");
                    }
                }

                // Bước 5: Kiểm tra thông tin thanh toán
                if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                    validationErrors.Add("Bước 5: Thiếu phương thức thanh toán (PaymentMethod)");
                
                if (string.IsNullOrWhiteSpace(request.BankAccount))
                    validationErrors.Add("Bước 5: Thiếu thông tin tài khoản ngân hàng (BankAccount)");

                // Nếu có lỗi validation, không cho phép tạo event
                if (validationErrors.Any())
                {
                    Console.WriteLine("=== Validation Failed ===");
                    foreach (var error in validationErrors)
                    {
                        Console.WriteLine($"- {error}");
                    }
                    
                    return BadRequest(new { 
                        message = "Không thể tạo sự kiện. Vui lòng hoàn thành tất cả các bước bắt buộc.",
                        errors = validationErrors,
                        completed = false
                    });
                }

                // Tất cả validation đều pass - Tạo event hoàn chỉnh
                Console.WriteLine("=== All Validations Passed - Creating Complete Event ===");
                
                // Tạo location string
                string locationString = string.Empty;
                if (request.EventMode == "Online")
                {
                    locationString = request.Location ?? string.Empty;
                }
                else
                {
                    var addressParts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(request.StreetAddress)) addressParts.Add(request.StreetAddress);
                    if (!string.IsNullOrWhiteSpace(request.Ward)) addressParts.Add(request.Ward);
                    if (!string.IsNullOrWhiteSpace(request.District)) addressParts.Add(request.District);
                    if (!string.IsNullOrWhiteSpace(request.Province)) addressParts.Add(request.Province);
                    locationString = string.Join(", ", addressParts);
                }

                // Tạo event với tất cả thông tin
                var eventData = new Event
                {
                    HostId = userId.Value,
                    Title = request.Title,
                    Description = request.Description,
                    EventMode = request.EventMode,
                    EventType = request.EventType ?? "Public",
                    Category = request.Category,
                    Location = locationString,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = "Open", // Trực tiếp set Open vì đã hoàn thành 5 bước
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Set EventDetails
                var eventDetails = new EventDetailsData
                {
                    VenueName = request.VenueName,
                    Province = request.Province,
                    District = request.District,
                    Ward = request.Ward,
                    StreetAddress = request.StreetAddress,
                    EventImage = request.EventImage,
                    BackgroundImage = request.BackgroundImage,
                    EventIntroduction = request.EventIntroduction,
                    EventDetails = request.EventDetails,
                    SpecialGuests = request.SpecialGuests,
                    SpecialExperience = request.SpecialExperience
                };
                eventData.SetEventDetails(eventDetails);

                // Set TermsAndConditions
                var termsAndConditions = new TermsAndConditionsData
                {
                    TermsAndConditions = request.TermsAndConditions,
                    ChildrenTerms = request.ChildrenTerms,
                    VATTerms = request.VATTerms
                };
                eventData.SetTermsAndConditions(termsAndConditions);

                // Set OrganizerInfo
                var organizerInfo = new OrganizerInfoData
                {
                    OrganizerLogo = request.OrganizerLogo,
                    OrganizerName = request.OrganizerName,
                    OrganizerInfo = request.OrganizerInfo
                };
                eventData.SetOrganizerInfo(organizerInfo);

                // Set VenueLayout nếu có
                if (request.VenueLayout != null)
                {
                    eventData.SetVenueLayout(request.VenueLayout);
                }

                // Tạo event
                var createdEvent = await _eventService.CreateEventAsync(eventData);

                // Tạo ticket types
                if (request.TicketTypes != null && request.TicketTypes.Any())
                {
                    var ticketTypesToAdd = new List<TicketType>();
                    foreach (var ticketRequest in request.TicketTypes)
                    {
                        var saleStart = ticketRequest.SaleStart == default ? DateTime.UtcNow : ticketRequest.SaleStart;
                        var saleEnd = ticketRequest.SaleEnd == default || ticketRequest.SaleEnd <= saleStart 
                            ? saleStart.AddDays(30) 
                            : ticketRequest.SaleEnd;

                        var ticketType = new TicketType
                        {
                            EventId = createdEvent.EventId,
                            TypeName = ticketRequest.TypeName.Trim(),
                            Price = ticketRequest.Price,
                            Quantity = ticketRequest.Quantity,
                            MinOrder = ticketRequest.MinOrder > 0 ? ticketRequest.MinOrder : 1,
                            MaxOrder = ticketRequest.MaxOrder > 0 ? ticketRequest.MaxOrder : 10,
                            SaleStart = saleStart,
                            SaleEnd = saleEnd,
                            Status = "Active"
                        };
                        ticketTypesToAdd.Add(ticketType);
                    }

                    // Thêm ticket types vào event
                    if (createdEvent.TicketTypes == null)
                        createdEvent.TicketTypes = new List<TicketType>();
                    
                    foreach (var ticketType in ticketTypesToAdd)
                    {
                        createdEvent.TicketTypes.Add(ticketType);
                    }

                    // Update event để lưu ticket types
                    await _eventService.UpdateEventAsync(createdEvent.EventId, createdEvent);
                }

                // Thêm thông tin thanh toán vào Description
                var paymentInfo = $"Payment Method: {request.PaymentMethod}\n" +
                                $"Bank Account: {request.BankAccount}\n" +
                                $"Tax Info: {request.TaxInfo}";
                
                createdEvent.Description = createdEvent.Description + "\n\n" + paymentInfo;
                await _eventService.UpdateEventAsync(createdEvent.EventId, createdEvent);

                Console.WriteLine($"Complete event created successfully with ID: {createdEvent.EventId}");

                return Ok(new EventCreationResponse(
                    createdEvent.EventId,
                    "Sự kiện đã được tạo thành công!",
                    true
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating complete event: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo sự kiện hoàn chỉnh", error = ex.Message });
            }
        }

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
                    EventType = request.EventType,
                    Category = request.Category,
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };

                var createdEvent = await _eventService.CreateEventAsync(eventData);
                
                // Set JSON data using helper methods
                var eventDetails = new EventDetailsData
                {
                    VenueName = request.VenueName,
                    Province = request.Province,
                    District = request.District,
                    Ward = request.Ward,
                    StreetAddress = request.StreetAddress,
                    EventImage = request.EventImage,
                    BackgroundImage = request.BackgroundImage,
                    EventIntroduction = request.EventIntroduction,
                    EventDetails = request.EventDetails,
                    SpecialGuests = request.SpecialGuests,
                    SpecialExperience = request.SpecialExperience
                };
                createdEvent.SetEventDetails(eventDetails);
                
                var termsAndConditions = new TermsAndConditionsData
                {
                    TermsAndConditions = request.TermsAndConditions,
                    ChildrenTerms = request.ChildrenTerms,
                    VATTerms = request.VATTerms
                };
                createdEvent.SetTermsAndConditions(termsAndConditions);
                
                var organizerInfo = new OrganizerInfoData
                {
                    OrganizerLogo = request.OrganizerLogo,
                    OrganizerName = request.OrganizerName,
                    OrganizerInfo = request.OrganizerInfo
                };
                createdEvent.SetOrganizerInfo(organizerInfo);
                
                // Update the event with JSON data
                await _eventService.UpdateEventAsync(createdEvent.EventId, createdEvent);
                
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
                // Debug: Log request data
                Console.WriteLine($"=== UpdateEventStep2 Debug ===");
                Console.WriteLine($"EventId: {eventId}");
                Console.WriteLine($"StartTime: {request.StartTime} (Type: {request.StartTime.GetType()})");
                Console.WriteLine($"EndTime: {request.EndTime} (Type: {request.EndTime.GetType()})");
                Console.WriteLine($"TicketTypes count: {request.TicketTypes?.Count ?? 0}");
                
                // Validate StartTime and EndTime
                if (request.StartTime == DateTime.MinValue)
                {
                    Console.WriteLine("Error: StartTime is DateTime.MinValue");
                    return BadRequest(new { message = "Thời gian bắt đầu không hợp lệ" });
                }
                
                if (request.EndTime == DateTime.MinValue)
                {
                    Console.WriteLine("Error: EndTime is DateTime.MinValue");
                    return BadRequest(new { message = "Thời gian kết thúc không hợp lệ" });
                }
                
                if (request.StartTime >= request.EndTime)
                {
                    Console.WriteLine($"Error: StartTime ({request.StartTime}) >= EndTime ({request.EndTime})");
                    return BadRequest(new { message = "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc" });
                }
                
                if (request.TicketTypes != null)
                {
                    for (int i = 0; i < request.TicketTypes.Count; i++)
                    {
                        var ticket = request.TicketTypes[i];
                        Console.WriteLine($"Ticket {i}: TypeName={ticket.TypeName}, Price={ticket.Price}, Quantity={ticket.Quantity}");
                    }
                }
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    Console.WriteLine("Error: Token không hợp lệ");
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                Console.WriteLine($"UserId from token: {userId.Value}");

                // Kiểm tra quyền sở hữu event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                {
                    Console.WriteLine($"Error: Không tìm thấy event với ID {eventId}");
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }

                Console.WriteLine($"Found event: {existingEvent.Title}, HostId: {existingEvent.HostId}");

                if (existingEvent.HostId != userId.Value)
                {
                    Console.WriteLine($"Error: User {userId.Value} không có quyền chỉnh sửa event của Host {existingEvent.HostId}");
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");
                }

                // Cập nhật thông tin thời gian
                existingEvent.StartTime = request.StartTime;
                existingEvent.EndTime = request.EndTime;
                existingEvent.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine("Updating event time...");
                await _eventService.UpdateEventAsync(eventId, existingEvent);
                Console.WriteLine("Event time updated successfully");

                // Tạo ticket types
                Console.WriteLine($"Creating {request.TicketTypes?.Count ?? 0} ticket types...");
                
                // Xóa tất cả ticket types cũ trước khi tạo mới
                Console.WriteLine("Clearing existing ticket types...");
                var deleteResult = await _eventService.DeleteTicketTypesForEventAsync(eventId);
                if (deleteResult)
                {
                    Console.WriteLine("Old ticket types removed successfully");
                    // Reload event to get fresh state without old ticket types
                    existingEvent = await _eventService.GetEventByIdAsync(eventId);
                }
                else
                {
                    Console.WriteLine("Failed to remove old ticket types");
                    return BadRequest(new { message = "KhÃ´ng thá»ƒ xÃ³a loáº¡i vÃ© cÅ©" });
                }
                
                // Tạo ticket types mới
                var newTicketTypes = new List<TicketType>();
                
                if (request.TicketTypes == null || request.TicketTypes.Count == 0)
                {
                    Console.WriteLine("No ticket types provided");
                    return BadRequest(new { message = "Vui lòng thêm ít nhất một loại vé" });
                }
                
                foreach (var ticketTypeRequest in request.TicketTypes)
                {
                    Console.WriteLine($"Processing ticket type: {ticketTypeRequest.TypeName}");
                    
                    // Validate required fields
                    if (string.IsNullOrEmpty(ticketTypeRequest.TypeName))
                    {
                        Console.WriteLine($"Error: TypeName is null or empty");
                        return BadRequest(new { message = "Tên loại vé không được để trống" });
                    }
                    
                    // Validate TypeName content - không cho phép ký tự không phù hợp
                    var cleanTypeName = ticketTypeRequest.TypeName.Trim();
                    if (cleanTypeName.Length < 2)
                    {
                        Console.WriteLine($"Error: TypeName too short: {cleanTypeName}");
                        return BadRequest(new { message = "Tên loại vé phải có ít nhất 2 ký tự" });
                    }
                    
                    if (cleanTypeName.Length > 100)
                    {
                        Console.WriteLine($"Error: TypeName too long: {cleanTypeName}");
                        return BadRequest(new { message = "Tên loại vé không được quá 100 ký tự" });
                    }
                    
                    // Kiểm tra ký tự không phù hợp
                    var invalidChars = new[] { '<', '>', '&', '"', '\'', '\\', '/', ';', '=', '(', ')', '[', ']', '{', '}' };
                    if (invalidChars.Any(c => cleanTypeName.Contains(c)))
                    {
                        Console.WriteLine($"Error: TypeName contains invalid characters: {cleanTypeName}");
                        return BadRequest(new { message = "Tên loại vé chứa ký tự không hợp lệ" });
                    }
                    
                    // Kiểm tra nội dung không phù hợp
                    var inappropriateWords = new[] { "cáº·c", "lá»", "Ä‘á»‹t", "Ä‘á»¥", "Ä‘Ã©o", "chÃ³", "lá»“n", "buá»“i", "cá»©t" };
                    var lowerTypeName = cleanTypeName.ToLower();
                    if (inappropriateWords.Any(word => lowerTypeName.Contains(word)))
                    {
                        Console.WriteLine($"Error: TypeName contains inappropriate content: {cleanTypeName}");
                        return BadRequest(new { message = "Tên loại vé chứa nội dung không phù hợp. Vui lòng sử dụng tên phù hợp." });
                    }
                    
                    if (ticketTypeRequest.Price < 0)
                    {
                        Console.WriteLine($"Error: Price is negative: {ticketTypeRequest.Price}");
                        return BadRequest(new { message = "Giá vé không được âm" });
                    }
                    
                    if (ticketTypeRequest.Quantity < 0)
                    {
                        Console.WriteLine($"Error: Quantity is negative: {ticketTypeRequest.Quantity}");
                        return BadRequest(new { message = "Số lượng vé không được âm" });
                    }
                    
                    // Validate MinOrder and MaxOrder
                    if (ticketTypeRequest.MinOrder < 1)
                    {
                        Console.WriteLine($"Error: MinOrder must be at least 1: {ticketTypeRequest.MinOrder}");
                        return BadRequest(new { message = "Đơn hàng tối thiểu phải ít nhất là 1" });
                    }
                    
                    if (ticketTypeRequest.MaxOrder < 1)
                    {
                        Console.WriteLine($"Error: MaxOrder must be at least 1: {ticketTypeRequest.MaxOrder}");
                        return BadRequest(new { message = "Đơn hàng tối đa phải ít nhất là 1" });
                    }
                    
                    if (ticketTypeRequest.MinOrder > ticketTypeRequest.MaxOrder)
                    {
                        Console.WriteLine($"Error: MinOrder ({ticketTypeRequest.MinOrder}) cannot be greater than MaxOrder ({ticketTypeRequest.MaxOrder})");
                        return BadRequest(new { message = "Đơn hàng tối thiểu không thể lớn hơn đơn hàng tối đa" });
                    }
                    
                    // Đảm bảo SaleStart và SaleEnd có giá trị hợp lệ
                    var saleStart = ticketTypeRequest.SaleStart;
                    var saleEnd = ticketTypeRequest.SaleEnd;
                    
                    // Nếu SaleStart hoặc SaleEnd không hợp lệ, sử dụng giá trị mặc định
                    if (saleStart == DateTime.MinValue)
                    {
                        saleStart = DateTime.UtcNow;
                    }
                    
                    if (saleEnd == DateTime.MinValue || saleEnd <= saleStart)
                    {
                        saleEnd = saleStart.AddDays(30); // 30 ngày sau SaleStart
                    }
                    
                    var ticketType = new TicketType
                    {
                        EventId = eventId,
                        TypeName = ticketTypeRequest.TypeName.Trim(),
                        Price = ticketTypeRequest.Price,
                        Quantity = ticketTypeRequest.Quantity,
                        MinOrder = ticketTypeRequest.MinOrder > 0 ? ticketTypeRequest.MinOrder : 1,
                        MaxOrder = ticketTypeRequest.MaxOrder > 0 ? ticketTypeRequest.MaxOrder : 10,
                        SaleStart = saleStart,
                        SaleEnd = saleEnd,
                        Status = "Active"
                    };
                    
                    Console.WriteLine($"TicketType details: TypeName='{ticketType.TypeName}', Price={ticketType.Price}, Quantity={ticketType.Quantity}, MinOrder={ticketType.MinOrder}, MaxOrder={ticketType.MaxOrder}, SaleStart={ticketType.SaleStart}, SaleEnd={ticketType.SaleEnd}, Status='{ticketType.Status}'");
                    
                    Console.WriteLine($"Creating ticket type: {ticketType.TypeName}, Price: {ticketType.Price}, Quantity: {ticketType.Quantity}, MinOrder: {ticketType.MinOrder}, MaxOrder: {ticketType.MaxOrder}");
                    newTicketTypes.Add(ticketType);
                }
                
                // Thêm ticket types mới vào event
                if (existingEvent == null)
                {
                    return NotFound(new { message = "Không tìm thấy sự kiện" });
                }
                
                // Đảm bảo TicketTypes collection được khởi tạo
                if (existingEvent.TicketTypes == null)
                {
                    existingEvent.TicketTypes = new List<TicketType>();
                }
                
                Console.WriteLine($"Adding {newTicketTypes.Count} new ticket types to event...");
                foreach (var ticketType in newTicketTypes)
                {
                    existingEvent.TicketTypes.Add(ticketType);
                }
                
                // Update event with ticket types
                Console.WriteLine("Updating event with new ticket types...");
                try
                {
                    await _eventService.UpdateEventAsync(eventId, existingEvent);
                    Console.WriteLine("Event updated successfully with ticket types");
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine($"Database save error: {dbEx.Message}");
                    Console.WriteLine($"Database save stack trace: {dbEx.StackTrace}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"Database inner exception: {dbEx.InnerException.Message}");
                        Console.WriteLine($"Database inner stack trace: {dbEx.InnerException.StackTrace}");
                    }
                    throw new Exception($"Database save failed: {dbEx.Message}", dbEx);
                }

                return Ok(new EventCreationResponse(
                    eventId,
                    "Bước 2: Thời gian và loại vé đã được cập nhật thành công",
                    true
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== UpdateEventStep2 Error ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện bước 2", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{eventId}/create/step3")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStep3(int eventId, [FromBody] CreateEventStep3Request request)
        {
            try
            {
                Console.WriteLine($"=== UpdateEventStep3 (Virtual Stage) Debug ===");
                Console.WriteLine($"EventId: {eventId}");
                Console.WriteLine($"Request: {JsonSerializer.Serialize(request)}");
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                Console.WriteLine($"UserId: {userId}");

                // Kiểm tra quyền sở hữu event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");

                Console.WriteLine($"Event found: {existingEvent.Title}");

                // Lưu venue layout vào event
                if (request.VenueLayout != null)
                {
                    Console.WriteLine($"Saving venue layout with HasVirtualStage: {request.VenueLayout.HasVirtualStage}");
                    Console.WriteLine($"Number of areas: {request.VenueLayout.Areas?.Count ?? 0}");
                    
                    existingEvent.SetVenueLayout(request.VenueLayout);
                    
                    existingEvent.UpdatedAt = DateTime.UtcNow;
                    await _eventService.UpdateEventAsync(eventId, existingEvent);
                    
                    Console.WriteLine("Venue layout saved successfully");
                }
                else
                {
                    Console.WriteLine("No venue layout provided - skipping");
                }

                return Ok(new EventCreationResponse(
                    eventId,
                    "Bước 3: Sân khấu ảo đã được lưu thành công",
                    true
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateEventStep3: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sự kiện bước 3", error = ex.Message });
            }
        }

        // DUPLICATE CODE REMOVED - Previous duplicate UpdateEventStep4 method (with wrong CreateCompleteEvent body) has been removed
        // The correct UpdateEventStep4 is below


        [HttpPut("{eventId}/move/step4")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStep4(int eventId, [FromBody] CreateEventStep4Request request)
        {
            try
            {
                Console.WriteLine($"=== UpdateEventStep4 Debug ===");
                Console.WriteLine($"EventId: {eventId}");
                Console.WriteLine($"PaymentMethod: {request.PaymentMethod}");
                Console.WriteLine($"BankAccount: {request.BankAccount}");
                Console.WriteLine($"TaxInfo: {request.TaxInfo}");
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                // Kiểm tra quyền sở hữu event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tìm thấy sự kiện" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Bạn không có quyền chỉnh sửa sự kiện này");

                Console.WriteLine($"Event found: {existingEvent.Title}");

                // VALIDATION: Kiểm tra xem tất cả 5 bước đã được hoàn thành chưa
                var validationErrors = new List<string>();

                // Bước 1: Kiểm tra thông tin cơ bản
                if (string.IsNullOrWhiteSpace(existingEvent.Title))
                    validationErrors.Add("Bước 1: Thiếu tên sự kiện (Title)");
                
                if (string.IsNullOrWhiteSpace(existingEvent.Description))
                    validationErrors.Add("Bước 1: Thiếu mô tả sự kiện (Description)");
                
                if (string.IsNullOrWhiteSpace(existingEvent.Category))
                    validationErrors.Add("Bước 1: Thiếu danh mục sự kiện (Category)");
                
                if (string.IsNullOrWhiteSpace(existingEvent.EventMode))
                    validationErrors.Add("Bước 1: Thiếu chế độ sự kiện (EventMode)");

                // Kiểm tra thông tin địa chỉ tùy thuộc vào EventMode
                var eventDetails = existingEvent.GetEventDetails();
                if (existingEvent.EventMode == "Offline")
                {
                    if (string.IsNullOrWhiteSpace(eventDetails?.VenueName))
                        validationErrors.Add("Bước 1: Thiếu tên địa điểm (VenueName) cho sự kiện Offline");
                    
                    if (string.IsNullOrWhiteSpace(eventDetails?.Province))
                        validationErrors.Add("Bước 1: Thiếu tỉnh/thành phố (Province) cho sự kiện Offline");
                    
                    if (string.IsNullOrWhiteSpace(eventDetails?.StreetAddress))
                        validationErrors.Add("Bước 1: Thiếu địa chỉ đường (StreetAddress) cho sự kiện Offline");
                    
                    if (string.IsNullOrWhiteSpace(existingEvent.Location))
                        validationErrors.Add("Bước 1: Thiếu địa chỉ đầy đủ (Location) cho sự kiện Offline");
                }
                else if (existingEvent.EventMode == "Online")
                {
                    if (string.IsNullOrWhiteSpace(existingEvent.Location))
                        validationErrors.Add("Bước 1: Thiếu link sự kiện (Location) cho sự kiện Online");
                }

                // Kiểm tra thông tin tổ chức
                var organizerInfo = existingEvent.GetOrganizerInfo();
                if (string.IsNullOrWhiteSpace(organizerInfo?.OrganizerName))
                    validationErrors.Add("Bước 1: Thiếu tên tổ chức (OrganizerName)");
                
                if (string.IsNullOrWhiteSpace(organizerInfo?.OrganizerInfo))
                    validationErrors.Add("Bước 1: Thiếu thông tin tổ chức (OrganizerInfo)");

                // Bước 2: Kiểm tra thời gian và loại vé
                if (existingEvent.StartTime == default || existingEvent.StartTime == DateTime.MinValue)
                    validationErrors.Add("Bước 2: Thiếu thời gian bắt đầu (StartTime)");
                
                if (existingEvent.EndTime == default || existingEvent.EndTime == DateTime.MinValue)
                    validationErrors.Add("Bước 2: Thiếu thời gian kết thúc (EndTime)");
                
                if (existingEvent.StartTime != default && existingEvent.EndTime != default && existingEvent.StartTime >= existingEvent.EndTime)
                    validationErrors.Add("Bước 2: Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                
                // Kiểm tra ticket types
                if (existingEvent.TicketTypes == null || !existingEvent.TicketTypes.Any())
                    validationErrors.Add("Bước 2: Thiếu loại vé (cần ít nhất một loại vé)");
                else
                {
                    // Kiểm tra từng loại vé có đầy đủ thông tin không
                    int ticketIndex = 0;
                    foreach (var ticketType in existingEvent.TicketTypes)
                    {
                        int i = ticketIndex++;
                        if (string.IsNullOrWhiteSpace(ticketType.TypeName))
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} thiếu tên (TypeName)");
                        
                        if (ticketType.Price < 0)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có giá không hợp lệ (Price)");
                        
                        if (ticketType.Quantity < 0)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có số lượng không hợp lệ (Quantity)");
                        
                        if (ticketType.MinOrder < 1)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có đơn hàng tối thiểu không hợp lệ (MinOrder)");
                        
                        if (ticketType.MaxOrder < 1)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có đơn hàng tối đa không hợp lệ (MaxOrder)");
                        
                        if (ticketType.MinOrder > ticketType.MaxOrder)
                            validationErrors.Add($"Bước 2: Loại vé {i + 1} có MinOrder lớn hơn MaxOrder");
                    }
                }

                // Bước 3: Virtual Stage - Optional, không bắt buộc
                // Không cần validation

                // Bước 5 (Step 4 trong backend): Kiểm tra thông tin thanh toán
                if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                    validationErrors.Add("Bước 5: Thiếu phương thức thanh toán (PaymentMethod)");
                
                if (string.IsNullOrWhiteSpace(request.BankAccount))
                    validationErrors.Add("Bước 5: Thiếu thông tin tài khoản ngân hàng (BankAccount)");

                // Nếu có lỗi validation, không cho phép kích hoạt event
                if (validationErrors.Any())
                {
                    Console.WriteLine("=== Validation Failed ===");
                    foreach (var error in validationErrors)
                    {
                        Console.WriteLine($"- {error}");
                    }
                    
                    return BadRequest(new { 
                        message = "Không thể kích hoạt sự kiện. Vui lòng hoàn thành tất cả các bước bắt buộc.",
                        errors = validationErrors,
                        completed = false
                    });
                }

                // Nếu tất cả validation đều pass, mới được kích hoạt event
                Console.WriteLine("=== All Validations Passed - Activating Event ===");

                // Cập nhật thông tin thanh toán và chuyển status thành Open
                existingEvent.Status = "Open";
                existingEvent.UpdatedAt = DateTime.UtcNow;
                
                // Lưu thông tin thanh toán vào Description hoặc tạo field riêng
                var paymentInfo = $"Payment Method: {request.PaymentMethod}\n" +
                                $"Bank Account: {request.BankAccount}\n" +
                                $"Tax Info: {request.TaxInfo}";
                
                existingEvent.Description = existingEvent.Description + "\n\n" + paymentInfo;
                
                await _eventService.UpdateEventAsync(eventId, existingEvent);

                Console.WriteLine("Step 4 update successful - Event kích hoạt thành công");

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

                var imageUrl = await _fileManagementService.SaveEventImageAsync(file);
                
                return Ok(new { 
                    success = true, 
                    imageUrl = imageUrl,
                    message = "Upload ảnh thành công" 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi upload ảnh", error = ex.Message });
            }
        }

        [HttpPost("cleanup-unused-images")]
        [Authorize]
        public async Task<IActionResult> CleanupUnusedImages()
        {
            try
            {
                var deletedCount = await _fileManagementService.CleanupUnusedImagesAsync();
                
                return Ok(new { 
                    success = true, 
                    deletedCount = deletedCount,
                    message = $"Đã xóa {deletedCount} ảnh không sử dụng" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi dọn dẹp ảnh không sử dụng", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}


