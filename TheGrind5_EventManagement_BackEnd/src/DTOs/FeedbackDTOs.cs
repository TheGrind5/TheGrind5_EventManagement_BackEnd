using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.DTOs;

public record CreateFeedbackRequest(
    int EventId,
    string Comment,
    int? ParentFeedbackId = null
);

public record CreateFeedbackReactionRequest(
    int FeedbackId,
    string ReactionType // Like or Dislike
);

public record FeedbackResponse(
    int FeedbackId,
    int EventId,
    int UserId,
    string UserName,
    string UserAvatar,
    string Comment,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int? ParentFeedbackId,
    List<FeedbackResponse> Replies,
    FeedbackStats Stats
);

public record FeedbackStats(
    int LikeCount,
    int DislikeCount,
    string? UserReaction // Like, Dislike, or null
);

public record FeedbackListResponse(
    List<FeedbackResponse> Feedbacks,
    int TotalCount
);
