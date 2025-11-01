using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Business;

public interface IAISuggestionRepository
{
    Task<AISuggestion> CreateAISuggestionAsync(AISuggestion aiSuggestion);
    
    Task<List<AISuggestion>> GetUserSuggestionHistoryAsync(int userId);
    
    Task<List<AISuggestion>> GetUserSuggestionHistoryByTypeAsync(int userId, string suggestionType);
}

