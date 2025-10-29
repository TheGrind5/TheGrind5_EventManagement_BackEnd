using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly EventDBContext _context;

    public FeedbackService(
        IFeedbackRepository feedbackRepository,
        IEventRepository eventRepository,
        IUserRepository userRepository,
        EventDBContext context)
    {
        _feedbackRepository = feedbackRepository;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<FeedbackListResponse> GetFeedbacksByEventIdAsync(int eventId, int? currentUserId = null)
    {
        // Validate event exists
        var eventExists = await _eventRepository.GetEventByIdAsync(eventId);
        if (eventExists == null)
            throw new ArgumentException("Event not found");

        var feedbacks = await _feedbackRepository.GetFeedbacksByEventIdAsync(eventId, currentUserId);
        
        var feedbackResponses = feedbacks.Select(f => MapToFeedbackResponse(f, currentUserId)).ToList();

        return new FeedbackListResponse(
            Feedbacks: feedbackResponses,
            TotalCount: feedbackResponses.Count
        );
    }

    public async Task<FeedbackResponse?> CreateFeedbackAsync(CreateFeedbackRequest request, int userId)
    {
        // Validate event exists
        var eventExists = await _eventRepository.GetEventByIdAsync(request.EventId);
        if (eventExists == null)
            throw new ArgumentException("Event not found");

        // Validate user exists
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        // If this is a reply, validate parent feedback exists
        if (request.ParentFeedbackId.HasValue)
        {
            var parentFeedback = await _feedbackRepository.GetFeedbackByIdAsync(request.ParentFeedbackId.Value);
            if (parentFeedback == null)
                throw new ArgumentException("Parent feedback not found");
            
            // Only allow replies to feedbacks, not replies to replies
            if (parentFeedback.ParentFeedbackId.HasValue)
                throw new ArgumentException("Cannot reply to a reply");
        }

        var feedback = new Feedback
        {
            EventId = request.EventId,
            UserId = userId,
            Comment = request.Comment,
            ParentFeedbackId = request.ParentFeedbackId,
            CreatedAt = DateTime.UtcNow
        };

        var createdFeedback = await _feedbackRepository.CreateFeedbackAsync(feedback);
        
        if (createdFeedback == null)
            return null;

        // Reload with all relationships
        var fullFeedback = await _feedbackRepository.GetFeedbackByIdAsync(createdFeedback.FeedbackId);
        return fullFeedback != null ? MapToFeedbackResponse(fullFeedback, userId) : null;
    }

    public async Task<bool> DeleteFeedbackAsync(int feedbackId, int userId)
    {
        var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
        if (feedback == null)
            throw new ArgumentException("Feedback not found");

        // Check if user owns the feedback
        if (feedback.UserId != userId)
            throw new UnauthorizedAccessException("You don't have permission to delete this feedback");

        return await _feedbackRepository.DeleteFeedbackAsync(feedbackId);
    }

    public async Task<bool> AddReactionAsync(CreateFeedbackReactionRequest request, int userId)
    {
        var feedback = await _feedbackRepository.GetFeedbackByIdAsync(request.FeedbackId);
        if (feedback == null)
            throw new ArgumentException("Feedback not found");

        if (request.ReactionType != "Like" && request.ReactionType != "Dislike")
            throw new ArgumentException("ReactionType must be 'Like' or 'Dislike'");

        // Check if user already reacted
        var existingReaction = await _feedbackRepository.GetReactionAsync(request.FeedbackId, userId);

        if (existingReaction != null)
        {
            // If same reaction, remove it (toggle off)
            if (existingReaction.ReactionType == request.ReactionType)
            {
                return await _feedbackRepository.DeleteReactionAsync(existingReaction.ReactionId);
            }
            else
            {
                // Update to new reaction type
                existingReaction.ReactionType = request.ReactionType;
                return await _feedbackRepository.UpdateReactionAsync(existingReaction);
            }
        }
        else
        {
            // Create new reaction
            var reaction = new FeedbackReaction
            {
                FeedbackId = request.FeedbackId,
                UserId = userId,
                ReactionType = request.ReactionType,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _feedbackRepository.CreateReactionAsync(reaction);
            return created != null;
        }
    }

    public async Task<FeedbackResponse?> CreateReplyAsync(int feedbackId, string comment, int userId)
    {
        var parentFeedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
        if (parentFeedback == null)
            throw new ArgumentException("Feedback not found");

        // Cannot reply to a reply
        if (parentFeedback.ParentFeedbackId.HasValue)
            throw new ArgumentException("Cannot reply to a reply");

        var request = new CreateFeedbackRequest(
            EventId: parentFeedback.EventId,
            Comment: comment,
            ParentFeedbackId: feedbackId
        );

        return await CreateFeedbackAsync(request, userId);
    }

    private FeedbackResponse MapToFeedbackResponse(Feedback feedback, int? currentUserId)
    {
        var replies = feedback.Replies
            .OrderBy(r => r.CreatedAt)
            .Select(r => MapToFeedbackResponse(r, currentUserId))
            .ToList();

        var reactions = feedback.Reactions;
        var likeCount = reactions.Count(r => r.ReactionType == "Like");
        var dislikeCount = reactions.Count(r => r.ReactionType == "Dislike");

        string? userReaction = null;
        if (currentUserId.HasValue)
        {
            var userReactionObj = reactions.FirstOrDefault(r => r.UserId == currentUserId.Value);
            userReaction = userReactionObj?.ReactionType;
        }

        return new FeedbackResponse(
            FeedbackId: feedback.FeedbackId,
            EventId: feedback.EventId,
            UserId: feedback.UserId,
            UserName: feedback.User?.FullName ?? "Unknown",
            UserAvatar: feedback.User?.Avatar ?? "",
            Comment: feedback.Comment,
            CreatedAt: feedback.CreatedAt,
            UpdatedAt: feedback.UpdatedAt,
            ParentFeedbackId: feedback.ParentFeedbackId,
            Replies: replies,
            Stats: new FeedbackStats(
                LikeCount: likeCount,
                DislikeCount: dislikeCount,
                UserReaction: userReaction
            )
        );
    }
}
