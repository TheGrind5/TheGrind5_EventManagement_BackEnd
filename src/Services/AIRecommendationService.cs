using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services;

public class AIRecommendationService : IAIRecommendationService
{
    private readonly EventDBContext _context;
    private readonly IHuggingFaceService _huggingFaceService;
    private readonly ILogger<AIRecommendationService> _logger;

    public AIRecommendationService(
        EventDBContext context,
        IHuggingFaceService huggingFaceService,
        ILogger<AIRecommendationService> logger)
    {
        _context = context;
        _huggingFaceService = huggingFaceService;
        _logger = logger;
    }

    public async Task<EventRecommendationResponse> GetEventRecommendationsAsync(int userId)
    {
        try
        {
            // Get user's order history to understand preferences
            var userOrders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.TicketType)
                        .ThenInclude(tt => tt.Event)
                .Where(o => o.CustomerId == userId && o.Status == "Paid")
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            // Get user's wishlist
            var wishlistItems = await _context.Wishlists
                .Include(w => w.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            // Extract preferred categories and event types
            var preferredCategories = userOrders
                .SelectMany(o => o.OrderItems)
                .Select(oi => oi.TicketType.Event.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            // Get upcoming events in preferred categories
            var recommendedEvents = new List<RecommendedEvent>();
            var upcomingEvents = await _context.Events
                .Include(e => e.TicketTypes)
                .Include(e => e.Host)
                .Where(e => e.Status == "Open" && e.StartTime > DateTime.UtcNow)
                .ToListAsync();

            if (!preferredCategories.Any())
            {
                // If no history, recommend popular events
                recommendedEvents = upcomingEvents
                    .OrderByDescending(e => e.TicketTypes.SelectMany(tt => tt.Tickets.Where(t => t.Status == "Assigned")).Count())
                    .Take(5)
                    .Select(e => MapToRecommendedEvent(e, 0.5, "Sự kiện phổ biến"))
                    .ToList();
            }
            else
            {
                // Recommend events in preferred categories
                var categoryEvents = upcomingEvents
                    .Where(e => preferredCategories.Contains(e.Category))
                    .OrderByDescending(e => e.TicketTypes.SelectMany(tt => tt.Tickets.Where(t => t.Status == "Assigned")).Count())
                    .Take(5)
                    .ToList();

                foreach (var evt in categoryEvents)
                {
                    var similarity = CalculateCategorySimilarity(evt.Category, preferredCategories.Where(c => c != null).ToList()!);
                    var reason = $"Phù hợp với sở thích của bạn về {evt.Category}";
                    recommendedEvents.Add(MapToRecommendedEvent(evt, similarity, reason));
                }

                // If not enough recommendations, add more events
                if (recommendedEvents.Count < 5)
                {
                    var additionalEvents = upcomingEvents
                        .Where(e => !recommendedEvents.Any(re => re.EventId == e.EventId))
                        .OrderByDescending(e => e.TicketTypes.SelectMany(tt => tt.Tickets.Where(t => t.Status == "Assigned")).Count())
                        .Take(5 - recommendedEvents.Count)
                        .Select(e => MapToRecommendedEvent(e, 0.3, "Sự kiện đang hot"))
                        .ToList();

                    recommendedEvents.AddRange(additionalEvents);
                }
            }

            // Generate AI reasoning using HuggingFace
            var reasoning = await GenerateReasoningAsync(userId, preferredCategories, recommendedEvents);

            return new EventRecommendationResponse
            {
                Events = recommendedEvents,
                Reasoning = reasoning
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating event recommendations");
            return new EventRecommendationResponse
            {
                Events = new List<RecommendedEvent>(),
                Reasoning = "Không thể tạo gợi ý lúc này. Vui lòng thử lại sau."
            };
        }
    }

    private double CalculateCategorySimilarity(string? category, List<string> preferredCategories)
    {
        if (string.IsNullOrEmpty(category))
            return 0.0;

        var index = preferredCategories.IndexOf(category);
        if (index == -1)
            return 0.0;

        // Higher score for higher preference
        return 1.0 - (index * 0.2);
    }

    private RecommendedEvent MapToRecommendedEvent(Models.Event evt, double similarity, string reason)
    {
        var minPrice = evt.TicketTypes.Any() ? evt.TicketTypes.Min(tt => tt.Price) : 0;
        var imageUrl = string.Empty;

        try
        {
            var eventDetails = evt.GetEventDetails();
            imageUrl = eventDetails.images?.FirstOrDefault() ?? eventDetails.EventImage ?? string.Empty;
        }
        catch { }

        return new RecommendedEvent
        {
            EventId = evt.EventId,
            Title = evt.Title,
            Description = evt.Description,
            StartTime = evt.StartTime,
            EndTime = evt.EndTime,
            Location = evt.Location,
            Category = evt.Category,
            EventMode = evt.EventMode,
            MinPrice = minPrice,
            ImageUrl = imageUrl,
            SimilarityScore = similarity,
            Reason = reason
        };
    }

    private async Task<string> GenerateReasoningAsync(int userId, List<string?> categories, List<RecommendedEvent> events)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            var categoryText = categories.Any() ? string.Join(", ", categories.Where(c => c != null)) : "nhiều thể loại";

            var prompt = $"Dựa trên lịch sử tham dự sự kiện của người dùng về các danh mục: {categoryText}, " +
                        $"hãy giải thích ngắn gọn tại sao hệ thống gợi ý những sự kiện này cho họ. " +
                        $"Trả lời bằng tiếng Việt, không quá 150 từ.";

            return await _huggingFaceService.GenerateTextAsync(prompt, 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI reasoning");
            return "Các sự kiện này được chọn dựa trên sở thích và lịch sử tham dự của bạn.";
        }
    }
}

