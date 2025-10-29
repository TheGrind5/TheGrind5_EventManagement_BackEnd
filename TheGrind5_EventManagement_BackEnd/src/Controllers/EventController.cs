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
                if (existingEvent.TicketTypes != null && existingEvent.TicketTypes.Any())
                {
                    Console.WriteLine($"Found {existingEvent.TicketTypes.Count} existing ticket types to remove");
                    existingEvent.TicketTypes.Clear();
                    
                    // Lưu thay đổi để xóa ticket types cũ
                    Console.WriteLine("Saving changes to remove old ticket types...");
                    await _eventService.UpdateEventAsync(eventId, existingEvent);
                    Console.WriteLine("Old ticket types removed successfully");
                }
                else
                {
                    Console.WriteLine("No existing ticket types found");
                }
                
                // Tạo ticket types mới
                var newTicketTypes = new List<TicketType>();
                
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
                    var inappropriateWords = new[] { "cặc", "lỏ", "địt", "đụ", "đéo", "chó", "lồn", "buồi", "cứt" };
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
                Console.WriteLine($"=== UpdateEventStep3 Debug ===");
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

                // Xử lý dữ liệu từ request
                Console.WriteLine($"EventSettings: {request.EventSettings}");
                Console.WriteLine($"AllowRefund: {request.AllowRefund}");
                Console.WriteLine($"RefundDaysBefore: {request.RefundDaysBefore}");
                Console.WriteLine($"RequireApproval: {request.RequireApproval}");

                // Cập nhật cài đặt sự kiện
                existingEvent.UpdatedAt = DateTime.UtcNow;
                
                // Lưu thông tin cài đặt vào Description hoặc tạo field riêng
                // Hiện tại lưu vào Description để đơn giản
                var settingsInfo = $"Event Settings: {request.EventSettings}\n" +
                                 $"Allow Refund: {request.AllowRefund}\n" +
                                 $"Refund Days Before: {request.RefundDaysBefore}\n" +
                                 $"Require Approval: {request.RequireApproval}";
                
                existingEvent.Description = existingEvent.Description + "\n\n" + settingsInfo;
                
                await _eventService.UpdateEventAsync(eventId, existingEvent);

                Console.WriteLine("Step 3 update successful");

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

                // Cập nhật thông tin thanh toán và chuyển status thành Open
                existingEvent.Status = "Open";
                existingEvent.UpdatedAt = DateTime.UtcNow;
                
                // Lưu thông tin thanh toán vào Description hoặc tạo field riêng
                var paymentInfo = $"Payment Method: {request.PaymentMethod}\n" +
                                $"Bank Account: {request.BankAccount}\n" +
                                $"Tax Info: {request.TaxInfo}";
                
                existingEvent.Description = existingEvent.Description + "\n\n" + paymentInfo;
                
                await _eventService.UpdateEventAsync(eventId, existingEvent);

                Console.WriteLine("Step 4 update successful - Event activated");

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
                return BadRequest(new { message = "Có lỗi xảy ra khi dọn dẹp ảnh", error = ex.Message });
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
