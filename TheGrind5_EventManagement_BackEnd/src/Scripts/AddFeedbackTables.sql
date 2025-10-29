-- Add Feedback Tables

-- 1. Create Feedback table
CREATE TABLE Feedback (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    UserId INT NOT NULL,
    Comment NVARCHAR(2000) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    ParentFeedbackId INT NULL,
    
    CONSTRAINT FK_Feedback_Event FOREIGN KEY (EventId) REFERENCES Event(EventId) ON DELETE NO ACTION,
    CONSTRAINT FK_Feedback_User FOREIGN KEY (UserId) REFERENCES [User](UserID) ON DELETE NO ACTION,
    CONSTRAINT FK_Feedback_ParentFeedback FOREIGN KEY (ParentFeedbackId) REFERENCES Feedback(FeedbackId) ON DELETE NO ACTION
);

-- 2. Create FeedbackReaction table
CREATE TABLE FeedbackReaction (
    ReactionId INT IDENTITY(1,1) PRIMARY KEY,
    FeedbackId INT NOT NULL,
    UserId INT NOT NULL,
    ReactionType NVARCHAR(50) NOT NULL CHECK (ReactionType IN ('Like', 'Dislike')),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_FeedbackReaction_Feedback FOREIGN KEY (FeedbackId) REFERENCES Feedback(FeedbackId) ON DELETE CASCADE,
    CONSTRAINT FK_FeedbackReaction_User FOREIGN KEY (UserId) REFERENCES [User](UserID) ON DELETE NO ACTION,
    CONSTRAINT UQ_FeedbackReaction_UserFeedback UNIQUE (FeedbackId, UserId)
);

-- Create indexes for better performance
CREATE INDEX IX_Feedback_EventId ON Feedback(EventId);
CREATE INDEX IX_Feedback_UserId ON Feedback(UserId);
CREATE INDEX IX_Feedback_ParentFeedbackId ON Feedback(ParentFeedbackId);
CREATE INDEX IX_FeedbackReaction_FeedbackId ON FeedbackReaction(FeedbackId);
CREATE INDEX IX_FeedbackReaction_UserId ON FeedbackReaction(UserId);
