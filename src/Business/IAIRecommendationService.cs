using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business;

public interface IAIRecommendationService
{
    Task<EventRecommendationResponse> GetEventRecommendationsAsync(int userId);
}

