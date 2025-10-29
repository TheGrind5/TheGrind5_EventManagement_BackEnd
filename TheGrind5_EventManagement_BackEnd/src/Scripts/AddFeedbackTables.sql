-- Add Feedback Tables (Safe version - check before create)

-- 1. Create Feedback table (only if not exists)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Feedback')
BEGIN
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
    PRINT 'Table Feedback created successfully';
END
ELSE
BEGIN
    PRINT 'Table Feedback already exists - skipped';
END

-- 2. Create FeedbackReaction table (only if not exists)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FeedbackReaction')
BEGIN
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
    PRINT 'Table FeedbackReaction created successfully';
END
ELSE
BEGIN
    PRINT 'Table FeedbackReaction already exists - skipped';
END

-- Create indexes for better performance (only if not exists)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Feedback_EventId' AND object_id = OBJECT_ID('Feedback'))
    CREATE INDEX IX_Feedback_EventId ON Feedback(EventId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Feedback_UserId' AND object_id = OBJECT_ID('Feedback'))
    CREATE INDEX IX_Feedback_UserId ON Feedback(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Feedback_ParentFeedbackId' AND object_id = OBJECT_ID('Feedback'))
    CREATE INDEX IX_Feedback_ParentFeedbackId ON Feedback(ParentFeedbackId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FeedbackReaction_FeedbackId' AND object_id = OBJECT_ID('FeedbackReaction'))
    CREATE INDEX IX_FeedbackReaction_FeedbackId ON FeedbackReaction(FeedbackId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FeedbackReaction_UserId' AND object_id = OBJECT_ID('FeedbackReaction'))
    CREATE INDEX IX_FeedbackReaction_UserId ON FeedbackReaction(UserId);

PRINT 'All indexes checked/created successfully';
