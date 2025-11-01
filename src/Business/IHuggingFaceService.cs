namespace TheGrind5_EventManagement.Business;

public interface IHuggingFaceService
{
    Task<string> GenerateTextAsync(string prompt, int maxLength = 500);
    
    Task<List<float>> GetEmbeddingsAsync(string text);
    
    Task<string> GenerateWithContextAsync(string prompt, string context, int maxLength = 500);
}

