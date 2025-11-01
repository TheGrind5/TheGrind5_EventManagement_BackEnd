using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services;

public class AIPricingService : IAIPricingService
{
    private readonly EventDBContext _context;
    private readonly IHuggingFaceService _huggingFaceService;
    private readonly ILogger<AIPricingService> _logger;

    public AIPricingService(
        EventDBContext context,
        IHuggingFaceService huggingFaceService,
        ILogger<AIPricingService> logger)
    {
        _context = context;
        _huggingFaceService = huggingFaceService;
        _logger = logger;
    }

    public async Task<PricingSuggestionResponse> GetPricingSuggestionsAsync(PricingSuggestionRequest request)
    {
        try
        {
            // Get similar events to analyze pricing
            var query = _context.Events
                .Include(e => e.TicketTypes)
                .Where(e => e.Status == "Open" && e.TicketTypes.Any());

            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(e => e.Category == request.Category);
            }

            var similarEvents = await query.ToListAsync();

            if (!similarEvents.Any())
            {
                return new PricingSuggestionResponse
                {
                    SuggestedPrices = GetDefaultPriceSuggestions(),
                    Analysis = "Không có dữ liệu sự kiện tương tự để phân tích. Chúng tôi đưa ra gợi ý giá mặc định.",
                    Recommendation = "Vui lòng tham khảo giá của các sự kiện tương tự trên thị trường."
                };
            }

            // Analyze pricing from similar events
            var priceRanges = AnalyzePriceRanges(similarEvents);

            // Generate AI analysis
            var analysis = await GeneratePricingAnalysis(similarEvents, request);

            return new PricingSuggestionResponse
            {
                SuggestedPrices = priceRanges,
                Analysis = analysis,
                Recommendation = GenerateRecommendation(priceRanges, similarEvents.Count)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating pricing suggestions");
            return new PricingSuggestionResponse
            {
                SuggestedPrices = GetDefaultPriceSuggestions(),
                Analysis = "Không thể phân tích giá tại thời điểm này.",
                Recommendation = "Vui lòng tham khảo giá của các sự kiện tương tự."
            };
        }
    }

    private List<PriceRange> AnalyzePriceRanges(List<Models.Event> events)
    {
        var prices = events
            .SelectMany(e => e.TicketTypes)
            .Where(tt => tt.Price > 0)
            .Select(tt => tt.Price)
            .ToList();

        if (!prices.Any())
        {
            return GetDefaultPriceSuggestions();
        }

        var pricesOrdered = prices.OrderBy(p => p).ToList();
        var minPrice = pricesOrdered[0];
        var maxPrice = pricesOrdered[pricesOrdered.Count - 1];
        var medianPrice = pricesOrdered[pricesOrdered.Count / 2];
        var avgPrice = prices.Average();

        var priceRanges = new List<PriceRange>();

        // Early Bird price (20% off from median)
        var earlyBirdPrice = medianPrice * 0.8m;
        priceRanges.Add(new PriceRange
        {
            TicketType = "Early Bird",
            MinPrice = earlyBirdPrice * 0.9m,
            MaxPrice = earlyBirdPrice * 1.1m,
            RecommendedPrice = earlyBirdPrice,
            Reasoning = "Giá ưu đãi cho người đăng ký sớm, thường rẻ hơn 20% so với giá chính thức."
        });

        // Regular price (median)
        priceRanges.Add(new PriceRange
        {
            TicketType = "Regular",
            MinPrice = medianPrice * 0.9m,
            MaxPrice = medianPrice * 1.1m,
            RecommendedPrice = medianPrice,
            Reasoning = "Giá vé tiêu chuẩn dựa trên phân tích giá của các sự kiện tương tự."
        });

        // VIP price (higher than average)
        var vipPrice = avgPrice * 1.5m;
        if (vipPrice < maxPrice)
        {
            vipPrice = maxPrice;
        }
        priceRanges.Add(new PriceRange
        {
            TicketType = "VIP",
            MinPrice = vipPrice * 0.9m,
            MaxPrice = vipPrice * 1.2m,
            RecommendedPrice = vipPrice,
            Reasoning = "Giá vé VIP với trải nghiệm đặc biệt, thường cao hơn 50% so với giá trung bình."
        });

        return priceRanges;
    }

    private List<PriceRange> GetDefaultPriceSuggestions()
    {
        return new List<PriceRange>
        {
            new PriceRange
            {
                TicketType = "Standard",
                MinPrice = 200000,
                MaxPrice = 1000000,
                RecommendedPrice = 500000,
                Reasoning = "Giá đề xuất mặc định cho sự kiện."
            }
        };
    }

    private async Task<string> GeneratePricingAnalysis(List<Models.Event> events, PricingSuggestionRequest request)
    {
        try
        {
            var totalEvents = events.Count;
            var avgPrice = events.SelectMany(e => e.TicketTypes).Average(tt => tt.Price);
            var categoryText = !string.IsNullOrEmpty(request.Category) ? request.Category : "các danh mục";
            var locationText = !string.IsNullOrEmpty(request.Location) ? $"tại {request.Location}" : "";

            var context = $"Dựa trên phân tích {totalEvents} sự kiện tương tự {categoryText} {locationText}, " +
                         $"giá vé trung bình là {avgPrice:N0} VNĐ.";

            var prompt = $"{context} Hãy phân tích ngắn gọn về thị trường giá vé và đưa ra nhận xét. " +
                        "Trả lời bằng tiếng Việt, không quá 100 từ.";

            return await _huggingFaceService.GenerateTextAsync(prompt, 150);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating pricing analysis");
            return "Phân tích dựa trên giá vé của các sự kiện tương tự trong hệ thống.";
        }
    }

    private string GenerateRecommendation(List<PriceRange> priceRanges, int eventCount)
    {
        if (priceRanges.Any())
        {
            var regular = priceRanges.FirstOrDefault(p => p.TicketType == "Regular");
            if (regular != null)
            {
                return $"Chúng tôi khuyến nghị bắt đầu với gói 'Regular' ở mức giá {regular.RecommendedPrice:N0} VNĐ, " +
                       $"dựa trên phân tích {eventCount} sự kiện tương tự.";
            }
        }

        return "Vui lòng cân nhắc các yếu tố như chi phí tổ chức, địa điểm và giá trị sự kiện khi đặt giá.";
    }
}

