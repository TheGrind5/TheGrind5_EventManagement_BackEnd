-- Migration script for WishlistItem table
-- Run this script after creating the migration with Entity Framework

-- Create WishlistItem table
CREATE TABLE [dbo].[WishlistItem] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int NOT NULL,
    [TicketTypeId] int NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_WishlistItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WishlistItem_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([UserID]) ON DELETE CASCADE,
    CONSTRAINT [FK_WishlistItem_TicketType_TicketTypeId] FOREIGN KEY ([TicketTypeId]) REFERENCES [TicketType] ([TicketTypeId]) ON DELETE RESTRICT,
    CONSTRAINT [CK_WishlistItem_Quantity] CHECK ([Quantity] >= 1),
    CONSTRAINT [UQ_WishlistItem_User_TicketType] UNIQUE ([UserId], [TicketTypeId])
);

-- Create indexes for better performance
CREATE INDEX [IX_WishlistItem_UserId] ON [dbo].[WishlistItem] ([UserId]);
CREATE INDEX [IX_WishlistItem_TicketTypeId] ON [dbo].[WishlistItem] ([TicketTypeId]);
CREATE INDEX [IX_WishlistItem_CreatedAt] ON [dbo].[WishlistItem] ([CreatedAt]);

-- Insert sample data (optional)
-- You can uncomment these lines if you want to add sample wishlist items for testing

/*
-- Sample wishlist items for testing
INSERT INTO [dbo].[WishlistItem] ([UserId], [TicketTypeId], [Quantity], [CreatedAt])
VALUES 
    (1, 1, 2, GETUTCDATE()),
    (1, 2, 1, GETUTCDATE()),
    (2, 1, 1, GETUTCDATE());
*/
