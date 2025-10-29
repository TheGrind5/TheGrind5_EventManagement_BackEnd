-- Tạo bảng Feedback
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Feedback')
BEGIN
    CREATE TABLE Feedback (
        FeedbackId int IDENTITY(1,1) PRIMARY KEY,
        EventId int NOT NULL,
        UserId int NOT NULL,
        Comment nvarchar(2000) NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt datetime2 NULL,
        ParentFeedbackId int NULL,
        CONSTRAINT FK_Feedback_Event FOREIGN KEY (EventId) REFERENCES [Event](EventId),
        CONSTRAINT FK_Feedback_User FOREIGN KEY (UserId) REFERENCES [User](UserID),
        CONSTRAINT FK_Feedback_Parent FOREIGN KEY (ParentFeedbackId) REFERENCES Feedback(FeedbackId)
    );
    
    CREATE INDEX IX_Feedback_EventId ON Feedback(EventId);
    CREATE INDEX IX_Feedback_UserId ON Feedback(UserId);
    CREATE INDEX IX_Feedback_ParentFeedbackId ON Feedback(ParentFeedbackId);
    
    PRINT 'Table Feedback created successfully';
END
ELSE
BEGIN
    PRINT 'Table Feedback already exists';
END
GO

-- Tạo bảng FeedbackReaction
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FeedbackReaction')
BEGIN
    CREATE TABLE FeedbackReaction (
        ReactionId int IDENTITY(1,1) PRIMARY KEY,
        FeedbackId int NOT NULL,
        UserId int NOT NULL,
        ReactionType nvarchar(50) NOT NULL,
        CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_FeedbackReaction_Feedback FOREIGN KEY (FeedbackId) REFERENCES Feedback(FeedbackId) ON DELETE CASCADE,
        CONSTRAINT FK_FeedbackReaction_User FOREIGN KEY (UserId) REFERENCES [User](UserID),
        CONSTRAINT UQ_FeedbackReaction_Feedback_User UNIQUE (FeedbackId, UserId)
    );
    
    CREATE INDEX IX_FeedbackReaction_FeedbackId_UserId ON FeedbackReaction(FeedbackId, UserId);
    CREATE INDEX IX_FeedbackReaction_UserId ON FeedbackReaction(UserId);
    
    PRINT 'Table FeedbackReaction created successfully';
END
ELSE
BEGIN
    PRINT 'Table FeedbackReaction already exists';
END
GO
