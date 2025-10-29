using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly EventDBContext _context;

    public FeedbackRepository(EventDBContext context)
    {
        _context = context;
    }

    public async Task<List<Feedback>> GetFeedbacksByEventIdAsync(int eventId, int? currentUserId = null)
    {
        var query = _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Reactions)
                .ThenInclude(r => r.User)
            .Include(f => f.Replies)
                .ThenInclude(r => r.User)
            .Include(f => f.Replies)
                .ThenInclude(r => r.Reactions)
            .Where(f => f.EventId == eventId && f.ParentFeedbackId == null)
            .OrderByDescending(f => f.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<Feedback?> GetFeedbackByIdAsync(int feedbackId)
    {
        return await _context.Feedbacks
            .Include(f => f.User)
            .Include(f => f.Reactions)
                .ThenInclude(r => r.User)
            .Include(f => f.Replies)
                .ThenInclude(r => r.User)
            .Include(f => f.Replies)
                .ThenInclude(r => r.Reactions)
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
    }

    public async Task<Feedback?> CreateFeedbackAsync(Feedback feedback)
    {
        try
        {
            Console.WriteLine($"=== CreateFeedbackAsync Start ===");
            Console.WriteLine($"EventId: {feedback.EventId}");
            Console.WriteLine($"UserId: {feedback.UserId}");
            Console.WriteLine($"Comment: {feedback.Comment}");
            Console.WriteLine($"ParentFeedbackId: {feedback.ParentFeedbackId}");
            Console.WriteLine($"CreatedAt before: {feedback.CreatedAt}");
            
            feedback.CreatedAt = DateTime.UtcNow;
            Console.WriteLine($"CreatedAt after: {feedback.CreatedAt}");
            
            _context.Feedbacks.Add(feedback);
            Console.WriteLine("Added to context, calling SaveChanges...");
            
            await _context.SaveChangesAsync();
            Console.WriteLine($"SaveChanges successful! FeedbackId: {feedback.FeedbackId}");
            
            return feedback;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in CreateFeedbackAsync: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public async Task<bool> DeleteFeedbackAsync(int feedbackId)
    {
        var feedback = await _context.Feedbacks.FindAsync(feedbackId);
        if (feedback == null) return false;

        _context.Feedbacks.Remove(feedback);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<FeedbackReaction?> GetReactionAsync(int feedbackId, int userId)
    {
        return await _context.FeedbackReactions
            .FirstOrDefaultAsync(r => r.FeedbackId == feedbackId && r.UserId == userId);
    }

    public async Task<FeedbackReaction?> CreateReactionAsync(FeedbackReaction reaction)
    {
        reaction.CreatedAt = DateTime.UtcNow;
        _context.FeedbackReactions.Add(reaction);
        await _context.SaveChangesAsync();
        return reaction;
    }

    public async Task<bool> UpdateReactionAsync(FeedbackReaction reaction)
    {
        var existing = await _context.FeedbackReactions.FindAsync(reaction.ReactionId);
        if (existing == null) return false;

        existing.ReactionType = reaction.ReactionType;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReactionAsync(int reactionId)
    {
        var reaction = await _context.FeedbackReactions.FindAsync(reactionId);
        if (reaction == null) return false;

        _context.FeedbackReactions.Remove(reaction);
        await _context.SaveChangesAsync();
        return true;
    }
}
