using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AISuggestionController : ControllerBase
{
    private readonly IAIRecommendationService _recommendationService;
    private readonly IAIChatbotService _chatbotService;
    private readonly IAIPricingService _pricingService;
    private readonly IAIContentGenerationService _contentGenerationService;
    private readonly IAISuggestionRepository _suggestionRepository;

    public AISuggestionController(
        IAIRecommendationService recommendationService,
        IAIChatbotService chatbotService,
        IAIPricingService pricingService,
        IAIContentGenerationService contentGenerationService,
        IAISuggestionRepository suggestionRepository)
    {
        _recommendationService = recommendationService;
        _chatbotService = chatbotService;
        _pricingService = pricingService;
        _contentGenerationService = contentGenerationService;
        _suggestionRepository = suggestionRepository;
    }

    /// <summary>
    /// Get personalized event recommendations for a customer
    /// </summary>
    [HttpPost("recommend-events")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetEventRecommendations()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Người dùng chưa đăng nhập" });

            var response = await _recommendationService.GetEventRecommendationsAsync(userId.Value);

            // Log suggestion to history
            await LogSuggestionAsync(userId.Value, "EventRecommendation", null, response);

            return Ok(ApiResponse<EventRecommendationResponse>.SuccessResponse(response, "Gợi ý sự kiện thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<EventRecommendationResponse>.ErrorResponse(
                $"Lỗi khi tạo gợi ý sự kiện: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get AI chatbot response
    /// </summary>
    [HttpPost("chatbot")]
    public async Task<IActionResult> AskChatbot([FromBody] ChatbotRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(ApiResponse<ChatbotResponse>.ErrorResponse("Câu hỏi không được để trống"));

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Người dùng chưa đăng nhập" });

            var response = await _chatbotService.GetChatbotResponseAsync(request.Question, request.EventId, userId);

            // Log suggestion to history
            var requestData = JsonSerializer.Serialize(new { request.Question, request.EventId });
            var responseData = JsonSerializer.Serialize(response);
            await LogSuggestionAsync(userId.Value, "ChatbotQA", requestData, responseData);

            return Ok(ApiResponse<ChatbotResponse>.SuccessResponse(response, "Chatbot đã trả lời"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ChatbotResponse>.ErrorResponse(
                $"Lỗi khi xử lý câu hỏi: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get pricing suggestions for event creation
    /// </summary>
    [HttpPost("suggest-pricing")]
    [Authorize(Roles = "Host")]
    public async Task<IActionResult> GetSuggestedPricing([FromBody] PricingSuggestionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Người dùng chưa đăng nhập" });

            var response = await _pricingService.GetPricingSuggestionsAsync(request);

            // Log suggestion to history
            var requestData = JsonSerializer.Serialize(request);
            var responseData = JsonSerializer.Serialize(response);
            await LogSuggestionAsync(userId.Value, "PricingSuggestion", requestData, responseData);

            return Ok(ApiResponse<PricingSuggestionResponse>.SuccessResponse(response, "Gợi ý giá vé thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PricingSuggestionResponse>.ErrorResponse(
                $"Lỗi khi tạo gợi ý giá vé: {ex.Message}"));
        }
    }

    /// <summary>
    /// Generate content for event description
    /// </summary>
    [HttpPost("generate-content")]
    [Authorize(Roles = "Host")]
    public async Task<IActionResult> GenerateContent([FromBody] ContentGenerationRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Người dùng chưa đăng nhập" });

            var response = await _contentGenerationService.GenerateContentAsync(request);

            // Log suggestion to history
            var requestData = JsonSerializer.Serialize(request);
            var responseData = JsonSerializer.Serialize(response);
            await LogSuggestionAsync(userId.Value, "ContentGeneration", requestData, responseData);

            return Ok(ApiResponse<ContentGenerationResponse>.SuccessResponse(response, "Tạo nội dung thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ContentGenerationResponse>.ErrorResponse(
                $"Lỗi khi tạo nội dung: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get user's AI suggestion history
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetSuggestionHistory()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Người dùng chưa đăng nhập" });

            var history = await _suggestionRepository.GetUserSuggestionHistoryAsync(userId.Value);

            var historyResponse = history.Select(h => new AISuggestionHistoryResponse
            {
                SuggestionId = h.SuggestionId,
                SuggestionType = h.SuggestionType,
                RequestData = h.RequestData,
                CreatedAt = h.CreatedAt
            }).ToList();

            return Ok(ApiResponse<List<AISuggestionHistoryResponse>>.SuccessResponse(
                historyResponse, "Lấy lịch sử thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<AISuggestionHistoryResponse>>.ErrorResponse(
                $"Lỗi khi lấy lịch sử: {ex.Message}"));
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        // Fallback: try to get from "sub" claim
        userIdClaim = User.FindFirst("sub")?.Value;
        if (int.TryParse(userIdClaim, out userId))
        {
            return userId;
        }

        // Fallback: try to get from "UserId" claim
        userIdClaim = User.FindFirst("UserId")?.Value;
        if (int.TryParse(userIdClaim, out userId))
        {
            return userId;
        }

        return null;
    }

    private async Task LogSuggestionAsync(int userId, string suggestionType, string? requestData, object responseData)
    {
        try
        {
            var responseJson = responseData is string ? (string)responseData : JsonSerializer.Serialize(responseData);
            
            var aiSuggestion = new AISuggestion
            {
                UserId = userId,
                SuggestionType = suggestionType,
                RequestData = requestData,
                ResponseData = responseJson,
                CreatedAt = DateTime.UtcNow
            };

            await _suggestionRepository.CreateAISuggestionAsync(aiSuggestion);
        }
        catch
        {
            // Silently fail logging - don't break the main response
        }
    }
}

