using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    // GET api/feedback/event/{eventId}
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetFeedbacksByEventId(int eventId)
    {
        try
        {
            // Get userId if authenticated, otherwise pass null
            int? userId = null;
            try
            {
                userId = GetUserIdFromToken();
            }
            catch
            {
                // User is not authenticated, continue without userId
            }
            
            var result = await _feedbackService.GetFeedbacksByEventIdAsync(eventId, userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách feedback", error = ex.Message });
        }
    }

    // POST api/feedback
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
    {
        try
        {
            Console.WriteLine("============================================");
            Console.WriteLine("=== CreateFeedback Controller Started ===");
            Console.WriteLine($"Request EventId: {request.EventId}");
            Console.WriteLine($"Request Comment: {request.Comment}");
            Console.WriteLine($"Request ParentFeedbackId: {request.ParentFeedbackId}");
            
            var userId = GetUserIdFromToken();
            Console.WriteLine($"UserId from token: {userId}");
            
            if (userId == null)
            {
                Console.WriteLine("ERROR: UserId is null!");
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            Console.WriteLine("Calling CreateFeedbackAsync...");
            var result = await _feedbackService.CreateFeedbackAsync(request, userId.Value);
            Console.WriteLine("CreateFeedbackAsync completed successfully");
            Console.WriteLine("============================================");
            
            return Ok(new { message = "Tạo feedback thành công", data = result });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"========= ARGUMENT EXCEPTION =========");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"========= EXCEPTION =========");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException Message: {ex.InnerException.Message}");
                Console.WriteLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
            }
            return BadRequest(new { message = "Có lỗi xảy ra khi tạo feedback", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // DELETE api/feedback/{feedbackId}
    [HttpDelete("{feedbackId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFeedback(int feedbackId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _feedbackService.DeleteFeedbackAsync(feedbackId, userId.Value);
            return Ok(new { message = "Xóa feedback thành công", success = result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi xóa feedback", error = ex.Message });
        }
    }

    // POST api/feedback/reaction
    [HttpPost("reaction")]
    [Authorize]
    public async Task<IActionResult> AddReaction([FromBody] CreateFeedbackReactionRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _feedbackService.AddReactionAsync(request, userId.Value);
            return Ok(new { message = "Thêm reaction thành công", success = result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi thêm reaction", error = ex.Message });
        }
    }

    // POST api/feedback/{feedbackId}/reply
    [HttpPost("{feedbackId}/reply")]
    [Authorize]
    public async Task<IActionResult> CreateReply(int feedbackId, [FromBody] CreateReplyRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _feedbackService.CreateReplyAsync(feedbackId, request.Comment, userId.Value);
            return Ok(new { message = "Trả lời thành công", data = result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi trả lời", error = ex.Message });
        }
    }

    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? int.Parse(userIdClaim) : null;
    }
}

// Helper DTO for reply request
public record CreateReplyRequest(string Comment);
