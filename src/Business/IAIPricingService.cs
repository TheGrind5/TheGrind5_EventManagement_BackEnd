using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business;

public interface IAIPricingService
{
    Task<PricingSuggestionResponse> GetPricingSuggestionsAsync(PricingSuggestionRequest request);
}

