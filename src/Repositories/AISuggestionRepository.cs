using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories;

public class AISuggestionRepository : IAISuggestionRepository
{
    private readonly EventDBContext _context;

    public AISuggestionRepository(EventDBContext context)
    {
        _context = context;
    }

    public async Task<AISuggestion> CreateAISuggestionAsync(AISuggestion aiSuggestion)
    {
        _context.AISuggestions.Add(aiSuggestion);
        await _context.SaveChangesAsync();
        return aiSuggestion;
    }

    public async Task<List<AISuggestion>> GetUserSuggestionHistoryAsync(int userId)
    {
        return await _context.AISuggestions
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AISuggestion>> GetUserSuggestionHistoryByTypeAsync(int userId, string suggestionType)
    {
        return await _context.AISuggestions
            .Where(a => a.UserId == userId && a.SuggestionType == suggestionType)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}

