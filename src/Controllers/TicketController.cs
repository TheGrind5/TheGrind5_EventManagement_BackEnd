using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        //Hàm dựng để dùng ticket service
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var pagedRequest = new PagedRequest
                {
                    Page = page,
                    PageSize = pageSize
                };

                var pagedTickets = await _ticketService.GetTicketsByUserIdAsync(userId.Value, pagedRequest);
                var ticketDtos = pagedTickets.Data.Select(MapToTicketDto).ToList();

                var response = new
                {
                    data = ticketDtos,
                    totalCount = pagedTickets.TotalCount,
                    page = pagedTickets.Page,
                    pageSize = pagedTickets.PageSize,
                    totalPages = pagedTickets.TotalPages,
                    hasPreviousPage = pagedTickets.HasPreviousPage,
                    hasNextPage = pagedTickets.HasNextPage,
                    // Summary counts across all tickets (not just current page)
                    summary = new
                    {
                        availableCount = ticketDtos.Count(t => t.Status == "Assigned"),
                        usedCount = ticketDtos.Count(t => t.Status == "Used"),
                        refundedCount = ticketDtos.Count(t => t.Status == "Refunded")
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách vé", error = ex.Message });
            }
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                    return NotFound(new { message = "Không tìm thấy vé" });

                // Check if ticket belongs to the user
                if (ticket.OrderItem?.Order?.CustomerId != userId.Value)
                    return Forbid("Bạn không có quyền xem vé này");

                return Ok(MapToTicketDto(ticket));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin vé", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetTicketsByEvent(int eventId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByEventIdAsync(eventId);
                var ticketDtos = tickets.Select(MapToTicketDto).ToList();

                return Ok(ticketDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách vé của sự kiện", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}/types")]
        public async Task<IActionResult> GetTicketTypesByEvent(int eventId)
        {
            try
            {
                Console.WriteLine($"🔍 DEBUG: GetTicketTypesByEvent called with eventId: {eventId}");
                
                var ticketTypes = await _ticketService.GetTicketTypesByEventIdAsync(eventId);
                Console.WriteLine($"🔍 DEBUG: Found {ticketTypes.Count()} ticket types for event {eventId}");
                
                var ticketTypeDtos = new List<object>();
                foreach (var ticketType in ticketTypes)
                {
                    Console.WriteLine($"🔍 DEBUG: Processing ticket type: ID={ticketType.TicketTypeId}, Name={ticketType.TypeName}, Status={ticketType.Status}");
                    var availableQty = await CalculateAvailableQuantity(ticketType.TicketTypeId);
                    ticketTypeDtos.Add(new
                    {
                        ticketTypeId = ticketType.TicketTypeId,
                        eventId = ticketType.EventId,
                        typeName = ticketType.TypeName,
                        price = ticketType.Price,
                        quantity = ticketType.Quantity,
                        availableQuantity = availableQty,
                        minOrder = ticketType.MinOrder,
                        maxOrder = ticketType.MaxOrder,
                        saleStart = ticketType.SaleStart,
                        saleEnd = ticketType.SaleEnd,
                        status = ticketType.Status
                    });
                }

                Console.WriteLine($"🔍 DEBUG: Returning {ticketTypeDtos.Count} ticket type DTOs");
                return Ok(ticketTypeDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔍 DEBUG: Error in GetTicketTypesByEvent: {ex.Message}");
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách loại vé của sự kiện", error = ex.Message });
            }
        }

        [HttpGet("type/{ticketTypeId}/availability")]
        public async Task<IActionResult> GetTicketTypeAvailability(int ticketTypeId)
        {
            try
            {
                var ticketType = await _ticketService.GetTicketTypeByIdAsync(ticketTypeId);
                if (ticketType == null)
                    return NotFound(new { message = "Không tìm thấy loại vé" });

                var availableQuantity = await CalculateAvailableQuantity(ticketTypeId);
                
                return Ok(new {
                    ticketTypeId = ticketTypeId,
                    totalQuantity = ticketType.Quantity,
                    availableQuantity = availableQuantity,
                    soldQuantity = ticketType.Quantity - availableQuantity,
                    isAvailable = availableQuantity > 0,
                    minOrder = ticketType.MinOrder,
                    maxOrder = ticketType.MaxOrder,
                    saleStart = ticketType.SaleStart,
                    saleEnd = ticketType.SaleEnd,
                    isOnSale = DateTime.Now >= ticketType.SaleStart && DateTime.Now <= ticketType.SaleEnd
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi kiểm tra số lượng vé", error = ex.Message });
            }
        }

        [HttpPut("{ticketId}/check-in")]
        public async Task<IActionResult> CheckInTicket(int ticketId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                    return NotFound(new { message = "Không tìm thấy vé" });

                // Check if ticket belongs to the user
                if (ticket.OrderItem?.Order?.CustomerId != userId.Value)
                    return Forbid("Bạn không có quyền check-in vé này");

                var updatedTicket = await _ticketService.CheckInTicketAsync(ticketId);
                
                return Ok(new { 
                    message = "Check-in thành công", 
                    ticket = MapToTicketDto(updatedTicket) 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi check-in vé", error = ex.Message });
            }
        }

        [HttpPut("{ticketId}/refund")]
        public async Task<IActionResult> RefundTicket(int ticketId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { message = "Token không hợp lệ" });

                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                    return NotFound(new { message = "Không tìm thấy vé" });

                // Check if ticket belongs to the user
                if (ticket.OrderItem?.Order?.CustomerId != userId.Value)
                    return Forbid("Bạn không có quyền hoàn tiền vé này");

                var refundedTicket = await _ticketService.RefundTicketAsync(ticketId);
                
                return Ok(new { 
                    message = "Hoàn tiền vé thành công", 
                    ticket = MapToTicketDto(refundedTicket) 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi hoàn tiền vé", error = ex.Message });
            }
        }

        [HttpGet("{ticketId}/validate")]
        public async Task<IActionResult> ValidateTicket(int ticketId)
        {
            try
            {
                var isValid = await _ticketService.IsTicketValidAsync(ticketId);
                
                return Ok(new { 
                    ticketId = ticketId,
                    isValid = isValid,
                    message = isValid ? "Vé hợp lệ" : "Vé không hợp lệ hoặc đã được sử dụng"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi kiểm tra vé", error = ex.Message });
            }
        }

        private TicketDTO MapToTicketDto(Models.Ticket ticket)
        {
            return new TicketDTO
            {
                TicketId = ticket.TicketId,
                SerialNumber = ticket.SerialNumber,
                Status = ticket.Status,
                IssuedAt = ticket.IssuedAt,
                UsedAt = ticket.UsedAt,
                RefundedAt = ticket.RefundedAt,
                TicketType = new TicketTypeInfoDTO
                {
                    TicketTypeId = ticket.TicketType.TicketTypeId,
                    TypeName = ticket.TicketType.TypeName,
                    Price = ticket.TicketType.Price
                },
                Event = new EventInfoDTO
                {
                    EventId = ticket.TicketType.Event.EventId,
                    Title = ticket.TicketType.Event.Title,
                    Description = ticket.TicketType.Event.Description,
                    StartTime = ticket.TicketType.Event.StartTime,
                    EndTime = ticket.TicketType.Event.EndTime,
                    Location = ticket.TicketType.Event.Location,
                    Category = ticket.TicketType.Event.Category
                },
                Order = ticket.OrderItem?.Order != null ? new OrderInfoDTO
                {
                    OrderId = ticket.OrderItem.Order.OrderId,
                    Amount = ticket.OrderItem.Order.Amount,
                    Status = ticket.OrderItem.Order.Status,
                    CreatedAt = ticket.OrderItem.Order.CreatedAt
                } : new OrderInfoDTO
                {
                    OrderId = 0,
                    Amount = 0,
                    Status = "Pending",
                    CreatedAt = DateTime.MinValue
                }
            };
        }

        private async Task<int> CalculateAvailableQuantity(int ticketTypeId)
        {
            try
            {
                // Get total quantity for this ticket type
                var ticketType = await _ticketService.GetTicketTypeByIdAsync(ticketTypeId);
                if (ticketType == null) return 0;

                // Get count of sold tickets for this ticket type
                var soldTickets = await _ticketService.GetSoldTicketsCountAsync(ticketTypeId);
                
                // Calculate available quantity
                return Math.Max(0, ticketType.Quantity - soldTickets);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
