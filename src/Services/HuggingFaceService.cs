using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheGrind5_EventManagement.Business;

namespace TheGrind5_EventManagement.Services;

public class HuggingFaceService : IHuggingFaceService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HuggingFaceService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _textGenerationModel;
    private readonly string _embeddingModel;

    public HuggingFaceService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<HuggingFaceService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _apiKey = _configuration["HuggingFace:ApiKey"] ?? string.Empty;
        _baseUrl = _configuration["HuggingFace:BaseUrl"] ?? "https://api-inference.huggingface.co/models/";
        _textGenerationModel = _configuration["HuggingFace:TextGenerationModel"] ?? "mistralai/Mistral-7B-Instruct-v0.2";
        _embeddingModel = _configuration["HuggingFace:EmbeddingModel"] ?? "sentence-transformers/all-MiniLM-L6-v2";

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<string> GenerateTextAsync(string prompt, int maxLength = 500)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("HuggingFace API Key not configured, returning fallback response");
                return GetFallbackResponse(prompt);
            }

            var modelUrl = $"{_baseUrl}{_textGenerationModel}";
            var requestBody = new
            {
                inputs = prompt,
                parameters = new
                {
                    max_new_tokens = maxLength,
                    temperature = 0.7,
                    top_p = 0.9,
                    do_sample = true
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(modelUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);

            // Extract generated text from response
            if (jsonDoc.RootElement.TryGetProperty("generated_text", out var generatedText))
            {
                return generatedText.GetString() ?? string.Empty;
            }

            // Alternative response format
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array && jsonDoc.RootElement.GetArrayLength() > 0)
            {
                var firstItem = jsonDoc.RootElement[0];
                if (firstItem.TryGetProperty("generated_text", out var altGeneratedText))
                {
                    return altGeneratedText.GetString() ?? string.Empty;
                }
            }

            _logger.LogWarning("Could not extract generated text from HuggingFace response");
            return GetFallbackResponse(prompt);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling HuggingFace API");
            return GetFallbackResponse(prompt);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout calling HuggingFace API");
            return GetFallbackResponse(prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling HuggingFace API");
            return GetFallbackResponse(prompt);
        }
    }

    public async Task<List<float>> GetEmbeddingsAsync(string text)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("HuggingFace API Key not configured, returning empty embeddings");
                return new List<float>(384); // Default embedding dimension
            }

            var modelUrl = $"{_baseUrl}{_embeddingModel}";
            var requestBody = new { inputs = text };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(modelUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);

            // Parse embedding array
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array && jsonDoc.RootElement.GetArrayLength() > 0)
            {
                var embeddingArray = jsonDoc.RootElement[0];
                var embeddings = new List<float>();

                foreach (var item in embeddingArray.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Number)
                    {
                        embeddings.Add((float)item.GetDouble());
                    }
                }

                return embeddings;
            }

            _logger.LogWarning("Could not extract embeddings from HuggingFace response");
            return new List<float>(384);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting embeddings from HuggingFace API");
            return new List<float>(384);
        }
    }

    public async Task<string> GenerateWithContextAsync(string prompt, string context, int maxLength = 500)
    {
        var enhancedPrompt = $"{context}\n\nQuestion: {prompt}\nAnswer:";
        return await GenerateTextAsync(enhancedPrompt, maxLength);
    }

    private string GetFallbackResponse(string prompt)
    {
        // Fallback responses for when API is unavailable
        var lowerPrompt = prompt.ToLower();

        if (lowerPrompt.Contains("price") || lowerPrompt.Contains("cost"))
        {
            return "Tôi khuyến nghị bạn tham khảo giá vé từ các sự kiện tương tự trong cùng danh mục. Giá có thể dao động từ 100,000 VNĐ đến 5,000,000 VNĐ tùy thuộc vào quy mô và địa điểm sự kiện.";
        }

        if (lowerPrompt.Contains("event") || lowerPrompt.Contains("sự kiện"))
        {
            return "Chúng tôi có nhiều sự kiện thú vị đang diễn ra. Vui lòng xem trang chủ để khám phá các sự kiện phù hợp với bạn.";
        }

        if (lowerPrompt.Contains("refund") || lowerPrompt.Contains("hoàn tiền"))
        {
            return "Bạn có thể yêu cầu hoàn tiền cho đơn hàng chưa sử dụng trong vòng 7 ngày trước sự kiện. Tiền sẽ được hoàn lại vào ví điện tử của bạn trong vòng 3-5 ngày làm việc.";
        }

        return "Xin lỗi, tôi không thể trả lời câu hỏi này ngay bây giờ. Vui lòng liên hệ bộ phận hỗ trợ để được giúp đỡ chi tiết hơn.";
    }
}

