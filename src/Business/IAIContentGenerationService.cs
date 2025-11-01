using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business;

public interface IAIContentGenerationService
{
    Task<ContentGenerationResponse> GenerateContentAsync(ContentGenerationRequest request);
}

