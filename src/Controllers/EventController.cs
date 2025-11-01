using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
        private readonly EventDBContext _context;

        public EventController(IEventService eventService, IFileManagementService fileManagementService, EventDBContext context)
        {
            _eventService = eventService;
            _fileManagementService = fileManagementService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                Console.WriteLine($"Getting events - Page: {page}, PageSize: {pageSize}");
                
                var pagedRequest = new PagedRequest
                {
                    Page = page,
                    PageSize = pageSize
                };
                
                var pagedEvents = await _eventService.GetAllEventsAsync(pagedRequest);
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
                return BadRequest(new { message = ex.Message });
            }
        }
   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID sá»± kiá»‡n không há»£p lá»‡" });

                var eventData = await _eventService.GetEventByIdAsync(id);
                if (eventData == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                return Ok(_eventService.MapToEventDetailDto(eventData));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi láº¥y thông tin sá»± kiá»‡n", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                var eventData = _eventService.MapFromCreateEventRequest(request, userId.Value);
                var createdEvent = await _eventService.CreateEventAsync(eventData);

                return Ok(new { message = "Táº¡o sá»± kiá»‡n thÃ nh cÃ´ng", eventId = createdEvent?.EventId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi táº¡o sá»± kiá»‡n", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                // Kiá»ƒm tra quyá»n sá»Ÿ há»¯u event
                var existingEvent = await _eventService.GetEventByIdAsync(id);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Báº¡n không cÃ³ quyá»n chá»‰nh sá»­a sá»± kiá»‡n nÃ y");

                Console.WriteLine($"Found event: {existingEvent.Title}, HostId: {existingEvent.HostId}");

                // Tìm CampusId từ campus name nếu có
                if (!string.IsNullOrWhiteSpace(request.Campus))
                {
                    var campus = await _context.Campuses
                        .FirstOrDefaultAsync(c => c.Name == request.Campus || c.Code == request.Campus);
                    if (campus != null)
                    {
                        existingEvent.CampusId = campus.CampusId;
                    }
                }

                // Cáº­p nháº­t thông tin cÆ¡ báº£n cá»§a event
                existingEvent.Title = request.Title ?? existingEvent.Title;
                existingEvent.Description = request.Description ?? existingEvent.Description;
                existingEvent.Category = request.Category ?? existingEvent.Category;
                existingEvent.EventMode = request.EventMode ?? existingEvent.EventMode;
                existingEvent.Location = request.Location ?? existingEvent.Location;
                existingEvent.UpdatedAt = DateTime.UtcNow;

                // Cáº­p nháº­t EventDetails - MERGE vá»›i data cÅ© Ä‘á»ƒ không máº¥t thông tin
                var existingDetails = existingEvent.GetEventDetails();
                Console.WriteLine($"Existing EventImage: {existingDetails?.EventImage ?? "NULL"}");
                Console.WriteLine($"Existing BackgroundImage: {existingDetails?.BackgroundImage ?? "NULL"}");
                
                // Helper method Ä‘á»ƒ normalize Ä‘Æ°á»ng dáº«n áº£nh: /uploads/events/ -> /assets/images/events/
                string NormalizeImagePath(string? imagePath)
                {
                    if (string.IsNullOrEmpty(imagePath)) return string.Empty;
                    // Chuyá»ƒn Ä‘á»•i tá»« /uploads/events/ thÃ nh /assets/images/events/
                    return imagePath.Replace("/uploads/events/", "/assets/images/events/")
                                   .Replace("/uploads/avatars/", "/assets/images/avatars/");
                }
                
                // Xá»­ lÃ½ images array - Æ°u tiÃªn EventImage vÃ BackgroundImage tá»« request
                var imagesList = new List<string>();
                
                // QUAN TRá»ŒNG: Xá»­ lÃ½ EventImage - náº¿u cÃ³ giÃ¡ trá»‹ má»›i (dÃ¹ lÃ empty string), dÃ¹ng giÃ¡ trá»‹ má»›i
                // Náº¿u request.EventImage lÃ null hoáº·c empty, thÃ¬ giá»¯ giÃ¡ trá»‹ cÅ©
                var normalizedEventImage = string.Empty;
                if (request.EventImage != null) // CÃ³ field trong request (dÃ¹ cÃ³ thá»ƒ lÃ empty)
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
                    // Náº¿u request không cÃ³ EventImage (null), giá»¯ EventImage cÅ© (Ä‘Ã£ normalize)
                    var existingEventImage = NormalizeImagePath(existingDetails?.EventImage);
                    if (!string.IsNullOrEmpty(existingEventImage))
                    {
                        normalizedEventImage = existingEventImage;
                        imagesList.Add(existingEventImage);
                    }
                    else if (existingDetails?.images != null && existingDetails.images.Length > 0)
                    {
                        // Náº¿u cÃ³ images array, normalize vÃ láº¥y pháº§n tá»­ Ä‘áº§u tiÃªn
                        normalizedEventImage = NormalizeImagePath(existingDetails.images[0]);
                        if (!string.IsNullOrEmpty(normalizedEventImage))
                        {
                            imagesList.Add(normalizedEventImage);
                        }
                    }
                }
                
                // QUAN TRá»ŒNG: Xá»­ lÃ½ BackgroundImage - náº¿u cÃ³ giÃ¡ trá»‹ má»›i (dÃ¹ lÃ empty string), dÃ¹ng giÃ¡ trá»‹ má»›i
                var normalizedBackgroundImage = string.Empty;
                if (request.BackgroundImage != null) // CÃ³ field trong request (dÃ¹ cÃ³ thá»ƒ lÃ empty)
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
                    // Náº¿u request không cÃ³ BackgroundImage (null), giá»¯ BackgroundImage cÅ© (Ä‘Ã£ normalize)
                    var existingBgImage = NormalizeImagePath(existingDetails?.BackgroundImage);
                    if (!string.IsNullOrEmpty(existingBgImage))
                    {
                        normalizedBackgroundImage = existingBgImage;
                        imagesList.Add(existingBgImage);
                    }
                    else if (existingDetails?.images != null && existingDetails.images.Length > 1)
                    {
                        // Náº¿u cÃ³ images array, normalize vÃ láº¥y pháº§n tá»­ thá»© 2
                        normalizedBackgroundImage = NormalizeImagePath(existingDetails.images[1]);
                        if (!string.IsNullOrEmpty(normalizedBackgroundImage))
                        {
                            imagesList.Add(normalizedBackgroundImage);
                        }
                    }
                }
                
                // Äáº£m báº£o EventImage vÃ BackgroundImage Ä‘Æ°á»£c sync vá»›i images array (Ä‘Ã£ normalize)
                // finalEventImage vÃ finalBackgroundImage cÃ³ thá»ƒ lÃ empty string náº¿u user xÃ³a áº£nh
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
                        // Náº¿u không cÃ³ eventImage, thÃªm empty string Ä‘á»ƒ giá»¯ vá»‹ trÃ­
                        if (imagesList.Count == 0) imagesList.Add("");
                        imagesList.Add(finalBackgroundImage);
                    }
                }
                
                // Normalize cÃ¡c áº£nh cÃ²n láº¡i trong images array (náº¿u cÃ³)
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
                    // Giá»¯ nguyÃªn data cÅ© náº¿u request không cÃ³
                    VenueName = request.VenueName ?? existingDetails?.VenueName,
                    Province = request.Province ?? existingDetails?.Province,
                    District = request.District ?? existingDetails?.District,
                    Ward = request.Ward ?? existingDetails?.Ward,
                    StreetAddress = request.StreetAddress ?? existingDetails?.StreetAddress,
                    // QUAN TRá»ŒNG: LuÃ´n cáº­p nháº­t EventImage vÃ BackgroundImage (Ä‘Ã£ normalize vá» /assets/images/events/)
                    EventImage = finalEventImage,
                    BackgroundImage = finalBackgroundImage,
                    // Cáº­p nháº­t images array - Ä‘áº£m báº£o sync vá»›i EventImage vÃ BackgroundImage (Ä‘Ã£ normalize)
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
                
                // Cáº­p nháº­t OrganizerInfo riÃªng
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

                // Cáº­p nháº­t event vÃ sá»± kiá»‡n váº¡o database
                var updatedEvent = await _eventService.UpdateEventAsync(id, existingEvent);
                if (updatedEvent == null)
                {
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });
                }

                // Debug: Kiá»ƒm tra data sau khi update
                var verifyDetails = updatedEvent.GetEventDetails();
                Console.WriteLine($"=== After Update Verification ===");
                Console.WriteLine($"EventId: {updatedEvent.EventId}");
                Console.WriteLine($"EventImage in DB: {verifyDetails?.EventImage}");
                Console.WriteLine($"BackgroundImage in DB: {verifyDetails?.BackgroundImage}");
                Console.WriteLine($"UpdatedAt: {updatedEvent.UpdatedAt}");
                
                // Tráº£ vá» event Ä‘Ã£ update Ä‘á»ƒ frontend cÃ³ thá»ƒ reload ngay
                var responseDto = _eventService.MapToEventDetailDto(updatedEvent);
                Console.WriteLine("Event updated successfully");

                return Ok(new { 
                    message = "Cáº­p nháº­t sá»± kiá»‡n thÃ nh cÃ´ng",
                    eventData = responseDto // Tráº£ vá» event data Ä‘á»ƒ frontend cÃ³ thá»ƒ update state ngay
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi cáº­p nháº­t sá»± kiá»‡n", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                // Kiá»ƒm tra quyá»n sá»Ÿ há»¯u event
                var eventData = await _eventService.GetEventByIdAsync(id);
                if (eventData == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                if (eventData.HostId != userId.Value)
                    return Forbid("Báº¡n không cÃ³ quyá»n xÃ³a sá»± kiá»‡n nÃ y");

                // Kiá»ƒm tra xem event cÃ³ vÃ© Ä‘Ã£ Ä‘Æ°á»£c bÃ¡n thÃ nh cÃ´ng chÆ°a (Status = "Paid")
                var hasTicketsSold = await _eventService.CheckHasPaidTicketsAsync(id);
                if (hasTicketsSold)
                {
                    return BadRequest(new { 
                        message = "Không thá»ƒ xÃ³a sá»± kiá»‡n Ä‘Ã£ cÃ³ vÃ© Ä‘Æ°á»£c mua thÃ nh cÃ´ng. HÃ£y liÃªn há»‡ há»— trá»£ náº¿u muá»‘n há»§y sá»± kiá»‡n." 
                    });
                }

                var result = await _eventService.DeleteEventAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });
                }

                return Ok(new { message = "XÃ³a sá»± kiá»‡n thÃ nh cÃ´ng" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi xÃ³a sá»± kiá»‡n", error = ex.Message });
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
                return BadRequest(new { message = "Có lá»—i xcháº£y ra khi láº¥y danh sáº¡ch sá»± kiá»‡n cá»§a host", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                var events = await _eventService.GetEventsByHostAsync(userId.Value);
                var eventDtos = events.Select(e => _eventService.MapToEventDto(e)).ToList();
                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi láº¥y danh sáº¡ch sá»± kiá»‡n cá»§a tÃ´i", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                var eventData = await _eventService.GetEventByIdAsync(id);
                if (eventData == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                if (eventData.HostId != userId.Value)
                    return Forbid("Báº¡n không cÃ³ quyá»n xem sá»± kiá»‡n nÃ y");

                // Kiá»ƒm tra xem event cÃ³ ticket Ä‘Ã£ Ä‘Æ°á»£c bÃ¡n chÆ°a
                var hasTicketsSold = await _eventService.CheckHasTicketsSoldAsync(id);

                // Kiá»ƒm tra xem cÃ²n cÃ¡ch thá»i gian báº¯t Ä‘áº§u bao nhiÃªu giá»
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
                        ? "Không thá»ƒ chá»‰nh sá»­a sá»± kiá»‡n Ä‘Ã£ cÃ³ vÃ© Ä‘Æ°á»£c bÃ¡n" 
                        : !canEditTimeLocation
                            ? $"Chá»‰ cÃ³ thá»ƒ chá»‰nh sá»­a thá»i gian vÃ Ä‘á»‹a Ä‘iá»ƒm trÆ°á»›c 48 giá». CÃ²n {Math.Round(hoursUntilStart, 1)} giá» Ä‘áº¿n sá»± kiá»‡n."
                            : "Có thá»ƒ chá»‰nh sá»­a sá»± kiá»‡n"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi kiá»ƒm tra tráº¡ng thÃ¡i chá»‰nh sá»­a", error = ex.Message });
            }
        }

        // ========================================
        // NEW EVENT CREATION ENDPOINTS (Ticket Box Style)
        // ========================================

        // IMPORTANT: create/complete pháº£i Ä‘áº·t trÆ°á»›c create/step1 Ä‘á»ƒ trÃ¡nh route conflict
        // (Route cá»¥ thá»ƒ hÆ¡n pháº£i Ä‘áº·t trÆ°á»›c route generic hÆ¡n)

        [HttpPost("create/complete")]
        [Authorize]
        public async Task<IActionResult> CreateCompleteEvent([FromBody] CreateCompleteEventRequest request)
        {
            try
            {
                Console.WriteLine($"=== CreateCompleteEvent Debug ===");
                
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                // VALIDATION: Kiá»ƒm tra táº¥t cáº£ 5 bÆ°á»›c trÆ°á»›c khi táº¡o event
                var validationErrors = new List<string>();

                // BÆ°á»›c 1: Kiá»ƒm tra thông tin cÆ¡ báº£n
                if (string.IsNullOrWhiteSpace(request.Title))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tÃªn sá»± kiá»‡n (Title)");
                
                if (string.IsNullOrWhiteSpace(request.Description))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u mÃ´ táº£ sá»± kiá»‡n (Description)");
                
                if (string.IsNullOrWhiteSpace(request.Category))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u danh má»¥c sá»± kiá»‡n (Category)");
                
                if (string.IsNullOrWhiteSpace(request.EventMode))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u cháº¿ Ä‘á»™ sá»± kiá»‡n (EventMode)");

                // Kiá»ƒm tra thông tin Ä‘á»‹a chá»‰
                if (request.EventMode == "Offline")
                {
                    if (string.IsNullOrWhiteSpace(request.VenueName))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tÃªn Ä‘á»‹a Ä‘iá»ƒm (VenueName) cho sá»± kiá»‡n Offline");
                    
                    if (string.IsNullOrWhiteSpace(request.Province))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tá»‰nh/thÃ nh phá»‘ (Province) cho sá»± kiá»‡n Offline");
                    
                    if (string.IsNullOrWhiteSpace(request.StreetAddress))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u Ä‘á»‹a chá»‰ Ä‘Æ°á»ng (StreetAddress) cho sá»± kiá»‡n Offline");
                }
                else if (request.EventMode == "Online")
                {
                    if (string.IsNullOrWhiteSpace(request.Location))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u link sá»± kiá»‡n (Location) cho sá»± kiá»‡n Online");
                }

                // Kiá»ƒm tra thông tin tá»• chá»©c
                if (string.IsNullOrWhiteSpace(request.OrganizerName))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tÃªn tá»• chá»©c (OrganizerName)");
                
                if (string.IsNullOrWhiteSpace(request.OrganizerInfo))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u thông tin tá»• chá»©c (OrganizerInfo)");

                // BÆ°á»›c 2: Kiá»ƒm tra thá»i gian vÃ loáº¡i vÃ
                if (request.StartTime == default || request.StartTime == DateTime.MinValue)
                    validationErrors.Add("BÆ°á»›c 2: Thiáº¿u thá»i gian báº¯t Ä‘áº§u (StartTime)");
                
                if (request.EndTime == default || request.EndTime == DateTime.MinValue)
                    validationErrors.Add("BÆ°á»›c 2: Thiáº¿u thá»i gian káº¿t thÃºc (EndTime)");
                
                if (request.StartTime != default && request.EndTime != default && request.StartTime >= request.EndTime)
                    validationErrors.Add("BÆ°á»›c 2: Thá»i gian báº¯t Ä‘áº§u pháº£i nhá» hÆ¡n thá»i gian káº¿t thÃºc");
                
                // Kiá»ƒm tra ticket types
                if (request.TicketTypes == null || !request.TicketTypes.Any())
                    validationErrors.Add("BÆ°á»›c 2: Thiáº¿u loáº¡i vÃ (cáº§n Ã­t nháº¥t má»™t loáº¡i vÃ©)");
                else
                {
                    int ticketIndex = 0;
                    foreach (var ticketType in request.TicketTypes)
                    {
                        int i = ticketIndex++;
                        if (string.IsNullOrWhiteSpace(ticketType.TypeName))
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} thiáº¿u tÃªn (TypeName)");
                        
                        if (ticketType.Price < 0)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ giÃ¡ khÃ´ng há»£p lá»‡ (Price)");
                        
                        if (ticketType.Quantity < 0)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ sá»‘ lÆ°á»£ng khÃ´ng há»£p lá»‡ (Quantity)");
                        
                        if (ticketType.MinOrder < 1)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ Ä‘Æ¡n hÃ nh tá»‘i thiá»ƒu khÃ´ng há»£p lá»‡ (MinOrder)");
                        
                        if (ticketType.MaxOrder < 1)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ Ä‘Æ¡n hÃ nh tá»‘i Ä‘a khÃ´ng há»£p lá»‡ (MaxOrder)");
                        
                        if (ticketType.MinOrder > ticketType.MaxOrder)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ MinOrder lá»›n hÆ¡n MaxOrder");
                    }
                }

                // BÆ°á»›c 5: Kiá»ƒm tra thông tin thanh toÃ¡n
                if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                    validationErrors.Add("BÆ°á»›c 5: Thiáº¿u phÆ°Æ¡ng thá»©c thanh toÃ¡n (PaymentMethod)");
                
                if (string.IsNullOrWhiteSpace(request.BankAccount))
                    validationErrors.Add("BÆ°á»›c 5: Thiáº¿u thông tin tÃ i khoáº£n ngÃ¢n hÃ nh (BankAccount)");

                // Náº¿u cÃ³ lá»—i validation, khÃ´ng cho phÃ©p táº¡o event
                if (validationErrors.Any())
                {
                    Console.WriteLine("=== Validation Failed ===");
                    foreach (var error in validationErrors)
                    {
                        Console.WriteLine($"- {error}");
                    }
                    
                    return BadRequest(new { 
                        message = "Không thá»ƒ táº¡o sá»± kiá»‡n. Vui lÃ²ng hoÃ n thÃ nh táº¥t cáº£ cÃ¡c bÆ°á»›c báº¯t buá»™c.",
                        errors = validationErrors,
                        completed = false
                    });
                }

                // Táº¥t cáº£ validation Ä‘á»u pass - Táº¡o event hoÃ n chá»‰nh
                Console.WriteLine("=== All Validations Passed - Creating Complete Event ===");
                
                // Táº¡o location string
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

                // Tìm CampusId từ campus name nếu có
                int? campusId = null;
                if (!string.IsNullOrWhiteSpace(request.Campus))
                {
                    var campus = await _context.Campuses
                        .FirstOrDefaultAsync(c => c.Name == request.Campus || c.Code == request.Campus);
                    if (campus != null)
                    {
                        campusId = campus.CampusId;
                    }
                }

                // Táº¡o event vá»›i táº¥t cáº£ thông tin
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
                    Status = "Open", // Trá»±c tiáº¿p set Open vÃ¬ Ä'Ã£ hoÃ n thÃ nh Ä'á»§ 5 bÆ°á»›c
                    CampusId = campusId, // Set CampusId nếu tìm thấy
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

                // Set VenueLayout náº¿u cÃ³
                if (request.VenueLayout != null)
                {
                    eventData.SetVenueLayout(request.VenueLayout);
                }

                // Táº¡o event
                var createdEvent = await _eventService.CreateEventAsync(eventData);

                // Táº¡o ticket types
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
                            EventId = createdEvent?.EventId ?? 0,
                            TypeName = ticketRequest.TypeName?.Trim() ?? string.Empty,
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
                    if (createdEvent?.TicketTypes == null)
                        createdEvent!.TicketTypes = new List<TicketType>();
                    
                    foreach (var ticketType in ticketTypesToAdd)
                    {
                        createdEvent.TicketTypes!.Add(ticketType);
                    }

                    // Update event để lưu ticket types
                    await _eventService.UpdateEventAsync(createdEvent.EventId, createdEvent);
                }

                // AUTO-FIX VenueLayout: Đồng bộ TicketTypeId và LinkedTicket sau khi tạo TicketTypes
                if (request.VenueLayout != null)
                {
                    try
                    {
                        var currentTicketTypes = createdEvent?.TicketTypes?.ToList() ?? new List<TicketType>();
                        if (currentTicketTypes.Any())
                        {
                            var layoutToSave = request.VenueLayout;
                            if (layoutToSave.Areas != null && layoutToSave.Areas.Count > 0)
                            {
                                foreach (var area in layoutToSave.Areas)
                                {
                                    if (area == null) continue;

                                    // Nếu thiếu TicketTypeId mà có LinkedTicket.TypeName -> map theo tên
                                    if ((!area.TicketTypeId.HasValue || area.TicketTypeId.Value == 0) && area.LinkedTicket != null)
                                    {
                                        var byName = currentTicketTypes.FirstOrDefault(t => string.Equals(t.TypeName?.Trim(), area.LinkedTicket.TypeName?.Trim(), StringComparison.OrdinalIgnoreCase));
                                        if (byName != null)
                                        {
                                            area.TicketTypeId = byName.TicketTypeId;
                                        }
                                    }

                                    // Nếu có TicketTypeId nhưng thiếu LinkedTicket -> bổ sung snapshot
                                    if (area.TicketTypeId.HasValue && area.LinkedTicket == null)
                                    {
                                        var matched = currentTicketTypes.FirstOrDefault(t => t.TicketTypeId == area.TicketTypeId.Value);
                                        if (matched != null)
                                        {
                                            area.LinkedTicket = new LinkedTicketSnapshot
                                            {
                                                TicketTypeId = matched.TicketTypeId,
                                                TypeName = matched.TypeName,
                                                Price = matched.Price,
                                                Quantity = matched.Quantity,
                                                MinOrder = matched.MinOrder ?? 1,
                                                MaxOrder = matched.MaxOrder ?? 10,
                                                SaleStart = matched.SaleStart,
                                                SaleEnd = matched.SaleEnd,
                                                Status = matched.Status ?? "Active"
                                            };
                                        }
                                    }
                                }

                                // Lưu lại VenueLayout đã đồng bộ
                                createdEvent!.SetVenueLayout(layoutToSave);
                                await _eventService.UpdateEventAsync(createdEvent.EventId, createdEvent);
                            }
                        }
                    }
                    catch (Exception syncEx)
                    {
                        Console.WriteLine($"[CreateCompleteEvent] VenueLayout sync warning: {syncEx.Message}");
                    }
                }

                // Thêm thông tin thanh toán vào Description
                var paymentInfo = $"Payment Method: {request.PaymentMethod}\n" +
                                $"Bank Account: {request.BankAccount}\n" +
                                $"Tax Info: {request.TaxInfo}";
                
                createdEvent!.Description = (createdEvent.Description ?? string.Empty) + "\n\n" + paymentInfo;
                await _eventService.UpdateEventAsync(createdEvent.EventId, createdEvent);

                Console.WriteLine($"Complete event created successfully with ID: {createdEvent.EventId}");

                return Ok(new EventCreationResponse(
                    createdEvent.EventId,
                    "Sá»± kiá»‡n Ä‘Ã£ Ä‘Æ°á»£c táº¡o thÃ nh cÃ´ng!",
                    true
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating complete event: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi táº¡o sá»± kiá»‡n hoÃ n chá»‰nh", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                // Tìm CampusId từ campus name nếu có
                int? campusId = null;
                if (!string.IsNullOrWhiteSpace(request.Campus))
                {
                    var campus = await _context.Campuses
                        .FirstOrDefaultAsync(c => c.Name == request.Campus || c.Code == request.Campus);
                    if (campus != null)
                    {
                        campusId = campus.CampusId;
                    }
                }

                // Táº¡o event vá»›i thông tin bÆ°á»›c 1
                var eventData = new Event
                {
                    HostId = userId.Value,
                    Title = request.Title,
                    Description = request.Description,
                    EventMode = request.EventMode,
                    EventType = request.EventType,
                    Category = request.Category,
                    CampusId = campusId, // Set CampusId nếu tìm thấy
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };

                var createdEvent = await _eventService.CreateEventAsync(eventData);
                
                if (createdEvent == null)
                {
                    return BadRequest("Failed to create event");
                }
                
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
                createdEvent!.SetEventDetails(eventDetails);
                
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
                    "BÆ°á»›c 1: ThÃ´ng tin sá»± kiá»‡n Ä‘Ã£ Ä‘Æ°á»£c lÆ°u thÃ nh cÃ´ng",
                    true
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi táº¡o sá»± kiá»‡n bÆ°á»›c 1", error = ex.Message });
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
                    return BadRequest(new { message = "Thá»i gian báº¯t Ä‘áº§u không há»£p lá»‡" });
                }
                
                if (request.EndTime == DateTime.MinValue)
                {
                    Console.WriteLine("Error: EndTime is DateTime.MinValue");
                    return BadRequest(new { message = "Thá»i gian káº¿t thÃºc không há»£p lá»‡" });
                }
                
                if (request.StartTime >= request.EndTime)
                {
                    Console.WriteLine($"Error: StartTime ({request.StartTime}) >= EndTime ({request.EndTime})");
                    return BadRequest(new { message = "Thá»i gian báº¯t Ä‘áº§u pháº£i nhá» hÆ¡n thá»i gian káº¿t thÃºc" });
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
                    Console.WriteLine("Error: Token không há»£p lá»‡");
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });
                }

                Console.WriteLine($"UserId from token: {userId.Value}");

                // Kiá»ƒm tra quyá»n sá»Ÿ há»¯u event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                {
                    Console.WriteLine($"Error: Không tÃ¬m tháº¥y event vá»›i ID {eventId}");
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });
                }

                Console.WriteLine($"Found event: {existingEvent.Title}, HostId: {existingEvent.HostId}");

                if (existingEvent.HostId != userId.Value)
                {
                    Console.WriteLine($"Error: User {userId.Value} không cÃ³ quyá»n chá»‰nh sá»­a event cá»§a Host {existingEvent.HostId}");
                    return Forbid("Báº¡n không cÃ³ quyá»n chá»‰nh sá»­a sá»± kiá»‡n nÃ y");
                }

                // Cáº­p nháº­t thông tin thá»i gian
                existingEvent.StartTime = request.StartTime;
                existingEvent.EndTime = request.EndTime;
                existingEvent.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine("Updating event time...");
                await _eventService.UpdateEventAsync(eventId, existingEvent);
                Console.WriteLine("Event time updated successfully");

                // Táº¡o ticket types
                Console.WriteLine($"Creating {request.TicketTypes?.Count ?? 0} ticket types...");
                
                // XÃ³a táº¥t cáº£ ticket types cÅ© trÆ°á»›c khi táº¡o má»›i
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
                    return BadRequest(new { message = "Không thá»ƒ xÃ³a loáº¡i vÃ© cÅ©" });
                }
                
                // Táº¡o ticket types má»›i
                var newTicketTypes = new List<TicketType>();
                
                if (request.TicketTypes == null || request.TicketTypes.Count == 0)
                {
                    Console.WriteLine("No ticket types provided");
                    return BadRequest(new { message = "Vui lÃ²ng thÃªm Ã­t nháº¥t má»™t loáº¡i vÃ©" });
                }
                
                foreach (var ticketTypeRequest in request.TicketTypes)
                {
                    Console.WriteLine($"Processing ticket type: {ticketTypeRequest.TypeName}");
                    
                    // Validate required fields
                    if (string.IsNullOrEmpty(ticketTypeRequest.TypeName))
                    {
                        Console.WriteLine($"Error: TypeName is null or empty");
                        return BadRequest(new { message = "TÃªn loáº¡i vÃ© không Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng" });
                    }
                    
                    // Validate TypeName content - không cho phÃ©p kÃ½ tá»± không phÃ¹ há»£p
                    var cleanTypeName = ticketTypeRequest.TypeName.Trim();
                    if (cleanTypeName.Length < 2)
                    {
                        Console.WriteLine($"Error: TypeName too short: {cleanTypeName}");
                        return BadRequest(new { message = "TÃªn loáº¡i vÃ© pháº£i cÃ³ Ã­t nháº¥t 2 kÃ½ tá»±" });
                    }
                    
                    if (cleanTypeName.Length > 100)
                    {
                        Console.WriteLine($"Error: TypeName too long: {cleanTypeName}");
                        return BadRequest(new { message = "TÃªn loáº¡i vÃ© không Ä‘Æ°á»£c quÃ¡ 100 kÃ½ tá»±" });
                    }
                    
                    // Kiá»ƒm tra kÃ½ tá»± không phÃ¹ há»£p
                    var invalidChars = new[] { '<', '>', '&', '"', '\'', '\\', '/', ';', '=', '(', ')', '[', ']', '{', '}' };
                    if (invalidChars.Any(c => cleanTypeName.Contains(c)))
                    {
                        Console.WriteLine($"Error: TypeName contains invalid characters: {cleanTypeName}");
                        return BadRequest(new { message = "TÃªn loáº¡i vÃ© chá»©a kÃ½ tá»± không há»£p lá»‡" });
                    }
                    
                    // Kiá»ƒm tra ná»™i dung không phÃ¹ há»£p
                    var inappropriateWords = new[] { "cáº·c", "lá»", "Ä‘á»‹t", "Ä‘á»¥", "Ä‘Ã©o", "chÃ³", "lá»“n", "buá»“i", "cá»©t" };
                    var lowerTypeName = cleanTypeName.ToLower();
                    if (inappropriateWords.Any(word => lowerTypeName.Contains(word)))
                    {
                        Console.WriteLine($"Error: TypeName contains inappropriate content: {cleanTypeName}");
                        return BadRequest(new { message = "TÃªn loáº¡i vÃ© chá»©a ná»™i dung không phÃ¹ há»£p. Vui lÃ²ng sá»­ dá»¥ng tÃªn phÃ¹ há»£p." });
                    }
                    
                    if (ticketTypeRequest.Price < 0)
                    {
                        Console.WriteLine($"Error: Price is negative: {ticketTypeRequest.Price}");
                        return BadRequest(new { message = "GiÃ¡ vÃ© không Ä‘Æ°á»£c Ã¢m" });
                    }
                    
                    if (ticketTypeRequest.Quantity < 0)
                    {
                        Console.WriteLine($"Error: Quantity is negative: {ticketTypeRequest.Quantity}");
                        return BadRequest(new { message = "Sá»‘ lÆ°á»£ng vÃ© không Ä‘Æ°á»£c Ã¢m" });
                    }
                    
                    // Validate MinOrder and MaxOrder
                    if (ticketTypeRequest.MinOrder < 1)
                    {
                        Console.WriteLine($"Error: MinOrder must be at least 1: {ticketTypeRequest.MinOrder}");
                        return BadRequest(new { message = "ÄÆ¡n hÃ nh tá»‘i thiá»ƒu pháº£i Ã­t nháº¥t lÃ 1" });
                    }
                    
                    if (ticketTypeRequest.MaxOrder < 1)
                    {
                        Console.WriteLine($"Error: MaxOrder must be at least 1: {ticketTypeRequest.MaxOrder}");
                        return BadRequest(new { message = "ÄÆ¡n hÃ nh tá»‘i Ä‘a pháº£i Ã­t nháº¥t lÃ 1" });
                    }
                    
                    if (ticketTypeRequest.MinOrder > ticketTypeRequest.MaxOrder)
                    {
                        Console.WriteLine($"Error: MinOrder ({ticketTypeRequest.MinOrder}) cannot be greater than MaxOrder ({ticketTypeRequest.MaxOrder})");
                        return BadRequest(new { message = "ÄÆ¡n hÃ nh tá»‘i thiá»ƒu không thá»ƒ lá»›n hÆ¡n Ä‘Æ¡n hÃ nh tá»‘i Ä‘a" });
                    }
                    
                    // Äáº£m báº£o SaleStart vÃ SaleEnd cÃ³ giÃ¡ trá»‹ há»£p lá»‡
                    var saleStart = ticketTypeRequest.SaleStart;
                    var saleEnd = ticketTypeRequest.SaleEnd;
                    
                    // Náº¿u SaleStart hoáº·c SaleEnd không há»£p lá»‡, sá»­ dá»¥ng giÃ¡ trá»‹ máº·c Ä‘á»‹nh
                    if (saleStart == DateTime.MinValue)
                    {
                        saleStart = DateTime.UtcNow;
                    }
                    
                    if (saleEnd == DateTime.MinValue || saleEnd <= saleStart)
                    {
                        saleEnd = saleStart.AddDays(30); // 30 ngÃ y sau SaleStart
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
                
                // Thêm ticket types má»›i vào event
                if (existingEvent == null)
                {
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });
                }
                
                // Äáº£m báº£o TicketTypes collection Ä‘Æ°á»£c khá»Ÿi táº¡o
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
                    "BÆ°á»›c 2: Thá»i gian vÃ loáº¡i vÃ£ Ä‘Ã£ Ä‘Æ°á»£c cáº­p nháº­t thÃ nh cÃ´ng",
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
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi cáº­p nháº­t sá»± kiá»‡n bÆ°á»›c 2", error = ex.Message, stackTrace = ex.StackTrace });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                Console.WriteLine($"UserId: {userId}");

                // Kiá»ƒm tra quyá»n sá»Ÿ há»¯u event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Báº¡n không cÃ³ quyá»n chá»‰nh sá»­a sá»± kiá»‡n nÃ y");

                Console.WriteLine($"Event found: {existingEvent.Title}");

                // Lưu venue layout vào event
                if (request.VenueLayout != null)
                {
                    Console.WriteLine($"Saving venue layout with HasVirtualStage: {request.VenueLayout.HasVirtualStage}");
                    Console.WriteLine($"Number of areas: {request.VenueLayout.Areas?.Count ?? 0}");
                    
                    // AUTO-FIX: Nếu area có TicketTypeId nhưng LinkedTicket null, tự động bổ sung snapshot
                    if (request.VenueLayout.Areas != null && request.VenueLayout.Areas.Count > 0)
                    {
                        var eventTicketTypes = existingEvent.TicketTypes?.ToList() ?? new List<TicketType>();
                        foreach (var area in request.VenueLayout.Areas)
                        {
                            if (area != null && area.TicketTypeId.HasValue && area.LinkedTicket == null)
                            {
                                var matched = eventTicketTypes.FirstOrDefault(t => t.TicketTypeId == area.TicketTypeId.Value);
                                if (matched != null)
                                {
                                            area.LinkedTicket = new LinkedTicketSnapshot
                                    {
                                        TicketTypeId = matched.TicketTypeId,
                                        TypeName = matched.TypeName,
                                        Price = matched.Price,
                                                Quantity = matched.Quantity,
                                                MinOrder = matched.MinOrder ?? 1,
                                                MaxOrder = matched.MaxOrder ?? 10,
                                        SaleStart = matched.SaleStart,
                                        SaleEnd = matched.SaleEnd,
                                        Status = matched.Status ?? "Active"
                                    };
                                    Console.WriteLine($"Auto-filled LinkedTicket for area {area.Id} with TicketTypeId {area.TicketTypeId}");
                                }
                                else
                                {
                                    Console.WriteLine($"Warning: TicketTypeId {area.TicketTypeId} not found for event {eventId}. LinkedTicket remains null.");
                                }
                            }
                        }
                    }
                    
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
                    "BÆ°á»›c 3: SÃ¢n kháº¥u áº£o Ä‘Ã£ Ä‘Æ°á»£c lÆ°u thÃ nh cÃ´ng",
                    true
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateEventStep3: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi cáº­p nháº­t sá»± kiá»‡n bÆ°á»›c 3", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                // Kiá»ƒm tra quyá»n sá»Ÿ há»¯u event
                var existingEvent = await _eventService.GetEventByIdAsync(eventId);
                if (existingEvent == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                if (existingEvent.HostId != userId.Value)
                    return Forbid("Báº¡n không cÃ³ quyá»n chá»‰nh sá»­a sá»± kiá»‡n nÃ y");

                Console.WriteLine($"Event found: {existingEvent.Title}");

                // VALIDATION: Kiá»ƒm tra xem táº¥t cáº£ 5 bÆ°á»›c Ä‘Ã£ Ä‘Æ°á»£c hoÃ n thÃ nh chÆ°a
                var validationErrors = new List<string>();

                // BÆ°á»›c 1: Kiá»ƒm tra thông tin cÆ¡ báº£n
                if (string.IsNullOrWhiteSpace(existingEvent.Title))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tÃªn sá»± kiá»‡n (Title)");
                
                if (string.IsNullOrWhiteSpace(existingEvent.Description))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u mÃ´ táº£ sá»± kiá»‡n (Description)");
                
                if (string.IsNullOrWhiteSpace(existingEvent.Category))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u danh má»¥c sá»± kiá»‡n (Category)");
                
                if (string.IsNullOrWhiteSpace(existingEvent.EventMode))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u cháº¿ Ä‘á»™ sá»± kiá»‡n (EventMode)");

                // Kiá»ƒm tra thông tin Ä‘á»‹a chá»‰ tÃ¹y thuá»™c vÃ loáº¡i vÃ
                var eventDetails = existingEvent.GetEventDetails();
                if (existingEvent.EventMode == "Offline")
                {
                    if (string.IsNullOrWhiteSpace(eventDetails?.VenueName))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tÃªn Ä‘á»‹a Ä‘iá»ƒm (VenueName) cho sá»± kiá»‡n Offline");
                    
                    if (string.IsNullOrWhiteSpace(eventDetails?.Province))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tá»‰nh/thÃ nh phá»‘ (Province) cho sá»± kiá»‡n Offline");
                    
                    if (string.IsNullOrWhiteSpace(eventDetails?.StreetAddress))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u Ä‘á»‹a chá»‰ Ä‘Æ°á»ng (StreetAddress) cho sá»± kiá»‡n Offline");
                    
                    if (string.IsNullOrWhiteSpace(existingEvent.Location))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u Ä‘á»‹a chá»‰ Ä‘áº§y Ä‘á»§ (Location) cho sá»± kiá»‡n Offline");
                }
                else if (existingEvent.EventMode == "Online")
                {
                    if (string.IsNullOrWhiteSpace(existingEvent.Location))
                        validationErrors.Add("BÆ°á»›c 1: Thiáº¿u link sá»± kiá»‡n (Location) cho sá»± kiá»‡n Online");
                }

                // Kiá»ƒm tra thông tin tá»• chá»©c
                var organizerInfo = existingEvent.GetOrganizerInfo();
                if (string.IsNullOrWhiteSpace(organizerInfo?.OrganizerName))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u tÃªn tá»• chá»©c (OrganizerName)");
                
                if (string.IsNullOrWhiteSpace(organizerInfo?.OrganizerInfo))
                    validationErrors.Add("BÆ°á»›c 1: Thiáº¿u thông tin tá»• chá»©c (OrganizerInfo)");

                // BÆ°á»›c 2: Kiá»ƒm tra thá»i gian vÃ loáº¡i vÃ
                if (existingEvent.StartTime == default || existingEvent.StartTime == DateTime.MinValue)
                    validationErrors.Add("BÆ°á»›c 2: Thiáº¿u thá»i gian báº¯t Ä‘áº§u (StartTime)");
                
                if (existingEvent.EndTime == default || existingEvent.EndTime == DateTime.MinValue)
                    validationErrors.Add("BÆ°á»›c 2: Thiáº¿u thá»i gian káº¿t thÃºc (EndTime)");
                
                if (existingEvent.StartTime != default && existingEvent.EndTime != default && existingEvent.StartTime >= existingEvent.EndTime)
                    validationErrors.Add("BÆ°á»›c 2: Thá»i gian báº¯t Ä‘áº§u pháº£i nhá» hÆ¡n thá»i gian káº¿t thÃºc");
                
                // Kiá»ƒm tra ticket types
                if (existingEvent.TicketTypes == null || !existingEvent.TicketTypes.Any())
                    validationErrors.Add("BÆ°á»›c 2: Thiáº¿u loáº¡i vÃ (cáº§n Ã­t nháº¥t má»™t loáº¡i vÃ©)");
                else
                {
                    // Kiá»ƒm tra tá»«ng loáº¡i vÃ cÃ³ Ä‘áº§y Ä‘á»§ thông tin không
                    int ticketIndex = 0;
                    foreach (var ticketType in existingEvent.TicketTypes)
                    {
                        int i = ticketIndex++;
                        if (string.IsNullOrWhiteSpace(ticketType.TypeName))
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} thiáº¿u tÃªn (TypeName)");
                        
                        if (ticketType.Price < 0)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ giÃ¡ không há»£p lá»‡ (Price)");
                        
                        if (ticketType.Quantity < 0)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ sá»‘ lÆ°á»£ng không há»£p lá»‡ (Quantity)");
                        
                        if (ticketType.MinOrder < 1)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ Ä‘Æ¡n hÃ nh tá»‘i thiá»ƒu không há»£p lá»‡ (MinOrder)");
                        
                        if (ticketType.MaxOrder < 1)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ Ä‘Æ¡n hÃ nh tá»‘i Ä‘a không há»£p lá»‡ (MaxOrder)");
                        
                        if (ticketType.MinOrder > ticketType.MaxOrder)
                            validationErrors.Add($"BÆ°á»›c 2: Loáº¡i vÃ {i + 1} cÃ³ MinOrder lá»›n hÆ¡n MaxOrder");
                    }
                }

                // BÆ°á»›c 3: Virtual Stage - Optional, không báº¯t buá»™c
                // Không cáº§n validation

                // BÆ°á»›c 5 (Step 4 trong backend): Kiá»ƒm tra thông tin thanh toÃ¡n
                if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                    validationErrors.Add("BÆ°á»›c 5: Thiáº¿u phÆ°Æ¡ng thá»©c thanh toÃ¡n (PaymentMethod)");
                
                if (string.IsNullOrWhiteSpace(request.BankAccount))
                    validationErrors.Add("BÆ°á»›c 5: Thiáº¿u thông tin tÃ i khoáº£n ngÃ¢n hÃ nh (BankAccount)");

                // Náº¿u cÃ³ lá»—i validation, khÃ´ng cho phÃ©p kÃ­ch hoáº¡t event
                if (validationErrors.Any())
                {
                    Console.WriteLine("=== Validation Failed ===");
                    foreach (var error in validationErrors)
                    {
                        Console.WriteLine($"- {error}");
                    }
                    
                    return BadRequest(new { 
                        message = "Không thá»ƒ kÃ­ch hoáº¡t sá»± kiá»‡n. Vui lÃ²ng hoÃ n thÃ nh táº¥t cáº£ cÃ¡c bÆ°á»›c báº¯t buá»™c.",
                        errors = validationErrors,
                        completed = false
                    });
                }

                // Náº¿u táº¥t cáº£ validation Ä‘á»u pass, má»›i Ä‘Æ°á»£c kÃ­ch hoáº¡t event
                Console.WriteLine("=== All Validations Passed - Activating Event ===");

                // Cáº­p nháº­t thông tin thanh toÃ¡n vÃ chuyá»ƒn status thÃ nh Open
                existingEvent.Status = "Open";
                existingEvent.UpdatedAt = DateTime.UtcNow;
                
                // LÆ°u thông tin thanh toÃ¡n vÃ táº¡o field riÃªng
                var paymentInfo = $"Payment Method: {request.PaymentMethod}\n" +
                                $"Bank Account: {request.BankAccount}\n" +
                                $"Tax Info: {request.TaxInfo}";
                
                existingEvent.Description = existingEvent.Description + "\n\n" + paymentInfo;
                
                await _eventService.UpdateEventAsync(eventId, existingEvent);

                Console.WriteLine("Step 4 update successful - Event activated");

                return Ok(new EventCreationResponse(
                    eventId,
                    "BÆ°á»›c 4: Thông tin thanh toÃ¡n Ä‘Ã£ Ä‘Æ°á»£c lÆ°u vÃ sá»± kiá»‡n Ä‘Ã£ Ä‘Æ°á»£c kÃ­ch hoáº¡t thÃ nh cÃ´ng!",
                    true
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi cáº­p nháº­t sá»± kiá»‡n bÆ°á»›c 4", error = ex.Message });
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
                    return Unauthorized(new { message = "Token không há»£p lá»‡" });

                var eventData = await _eventService.GetEventByIdAsync(eventId);
                if (eventData == null)
                    return NotFound(new { message = "Không tÃ¬m tháº¥y sá»± kiá»‡n" });

                if (eventData.HostId != userId.Value)
                    return Forbid("Báº¡n không cÃ³ quyá»n xem sá»± kiá»‡n nÃ y");

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
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi láº¥y tráº¡ng thÃ¡i táº¡o sá»± kiá»‡n", error = ex.Message });
            }
        }

        [HttpPost("upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Không cÃ³ file Ä‘Æ°á»£c chá»n" });

                var imageUrl = await _fileManagementService.SaveEventImageAsync(file);
                
                return Ok(new { 
                    success = true, 
                    imageUrl = imageUrl,
                    message = "Upload áº£nh thÃ nh cÃ´ng" 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi upload áº£nh", error = ex.Message });
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
                    message = $"ÄÃ£ xÃ³a {deletedCount} áº£nh khÃ´ng sá»­ dá»¥ng" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lá»—i xáº£y ra khi dá»n dáº¹p áº£nh", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}

