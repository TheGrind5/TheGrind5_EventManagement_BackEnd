using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories;

public interface IFeedbackRepository
{
    Task<List<Feedback>> GetFeedbacksByEventIdAsync(int eventId, int? currentUserId = null);
    Task<Feedback?> GetFeedbackByIdAsync(int feedbackId);
    Task<Feedback?> CreateFeedbackAsync(Feedback feedback);
    Task<bool> DeleteFeedbackAsync(int feedbackId);
    Task<FeedbackReaction?> GetReactionAsync(int feedbackId, int userId);
    Task<FeedbackReaction?> CreateReactionAsync(FeedbackReaction reaction);
    Task<bool> UpdateReactionAsync(FeedbackReaction reaction);
    Task<bool> DeleteReactionAsync(int reactionId);
}
