using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services;

public class AIContentGenerationService : IAIContentGenerationService
{
    private readonly IHuggingFaceService _huggingFaceService;
    private readonly ILogger<AIContentGenerationService> _logger;

    public AIContentGenerationService(
        IHuggingFaceService huggingFaceService,
        ILogger<AIContentGenerationService> logger)
    {
        _huggingFaceService = huggingFaceService;
        _logger = logger;
    }

    public async Task<ContentGenerationResponse> GenerateContentAsync(ContentGenerationRequest request)
    {
        try
        {
            var response = new ContentGenerationResponse();

            // Generate description
            if (!string.IsNullOrEmpty(request.Title))
            {
                response.Description = await GenerateDescriptionAsync(
                    request.Title,
                    request.Category ?? "",
                    request.EventType ?? ""
                );
            }

            // Generate introduction
            if (!string.IsNullOrEmpty(request.Title))
            {
                response.Introduction = await GenerateIntroductionAsync(
                    request.Title,
                    request.Category ?? ""
                );
            }

            // Generate terms and conditions
            response.TermsAndConditions = await GenerateTermsAndConditionsAsync();

            // Generate special experience description
            if (!string.IsNullOrEmpty(request.Category))
            {
                response.SpecialExperience = await GenerateSpecialExperienceAsync(request.Category);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating content");
            return new ContentGenerationResponse
            {
                Description = "Hãy mô tả chi tiết về sự kiện của bạn để thu hút người tham dự.",
                Introduction = "Giới thiệu ngắn gọn về sự kiện và những điểm nổi bật.",
                TermsAndConditions = "Vé đã bán sẽ không được hoàn lại trừ khi sự kiện bị hủy. Người tham dự cần tuân thủ quy định của ban tổ chức.",
                SpecialExperience = "Trải nghiệm đặc biệt đang chờ bạn tại sự kiện này."
            };
        }
    }

    private async Task<string> GenerateDescriptionAsync(string title, string category, string eventType)
    {
        var prompt = $"Viết một mô tả sự kiện khoảng 100-150 từ cho sự kiện có tiêu đề '{title}' " +
                    $"thuộc danh mục {category} và loại {eventType}. " +
                    "Mô tả cần hấp dẫn, thu hút và cung cấp thông tin cần thiết cho người tham dự. " +
                    "Trả lời bằng tiếng Việt.";

        return await _huggingFaceService.GenerateTextAsync(prompt, 200);
    }

    private async Task<string> GenerateIntroductionAsync(string title, string category)
    {
        var prompt = $"Viết phần giới thiệu ngắn gọn (khoảng 50-80 từ) cho sự kiện '{title}' " +
                    $"thuộc danh mục {category}. " +
                    "Phần giới thiệu cần tạo sự tò mò và kích thích người tham dự. " +
                    "Trả lời bằng tiếng Việt.";

        return await _huggingFaceService.GenerateTextAsync(prompt, 120);
    }

    private async Task<string> GenerateTermsAndConditionsAsync()
    {
        var prompt = "Viết điều khoản và điều kiện mẫu cho sự kiện, bao gồm: " +
                    "- Chính sách hoàn tiền\n" +
                    "- Quy định về trẻ em\n" +
                    "- Thuế VAT\n" +
                    "- Quy định hành vi người tham dự\n" +
                    "Trả lời bằng tiếng Việt, ngắn gọn và rõ ràng.";

        return await _huggingFaceService.GenerateTextAsync(prompt, 300);
    }

    private async Task<string> GenerateSpecialExperienceAsync(string category)
    {
        var categoryDescriptions = new Dictionary<string, string>
        {
            { "Business", "kết nối doanh nghiệp, chia sẻ kiến thức kinh doanh" },
            { "Entertainment", "trải nghiệm giải trí tuyệt vời" },
            { "Education", "học tập và phát triển kỹ năng" },
            { "Sports", "thi đấu và hoạt động thể thao" },
            { "Music", "âm nhạc và biểu diễn sống động" },
            { "Food", "ẩm thực và hương vị đặc biệt" },
            { "Technology", "công nghệ và đổi mới" },
            { "Art", "nghệ thuật và sáng tạo" },
            { "Fashion", "thời trang và phong cách" },
            { "Wellness", "sức khỏe và tinh thần" }
        };

        var categoryDescription = categoryDescriptions.ContainsKey(category)
            ? categoryDescriptions[category]
            : "trải nghiệm đáng nhớ";

        var prompt = $"Viết mô tả ngắn gọn (khoảng 50-70 từ) về trải nghiệm đặc biệt cho sự kiện thuộc " +
                    $"danh mục {category}. Tập trung vào {categoryDescription}. " +
                    "Trả lời bằng tiếng Việt, hấp dẫn và tạo cảm hứng.";

        return await _huggingFaceService.GenerateTextAsync(prompt, 100);
    }
}

