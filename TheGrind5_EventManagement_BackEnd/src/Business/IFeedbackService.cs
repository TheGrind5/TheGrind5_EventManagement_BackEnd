using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business;

public interface IFeedbackService
{
    Task<FeedbackListResponse> GetFeedbacksByEventIdAsync(int eventId, int? currentUserId = null);
    Task<FeedbackResponse?> CreateFeedbackAsync(CreateFeedbackRequest request, int userId);
    Task<bool> DeleteFeedbackAsync(int feedbackId, int userId);
    Task<bool> AddReactionAsync(CreateFeedbackReactionRequest request, int userId);
    Task<FeedbackResponse?> CreateReplyAsync(int feedbackId, string comment, int userId);
}
