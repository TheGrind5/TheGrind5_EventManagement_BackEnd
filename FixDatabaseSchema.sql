-- ============================================
-- Comprehensive Database Schema Fix Script
-- ============================================
-- This script fixes all schema issues when database was created from TheGrind5_Query.sql
-- It runs 3 operations in sequence:
-- 1. Fix migration history to avoid conflicts
-- 2. Add missing columns to Order table
-- 3. Create Notification table if missing
-- ============================================
-- Run this in SQL Server Management Studio
-- ============================================

USE EventDB;
GO

PRINT '============================================';
PRINT 'Starting Database Schema Fix Script';
PRINT '============================================';
GO

-- ============================================
-- PART 1: Fix Migration History
-- ============================================
PRINT '';
PRINT 'Step 1: Fixing migration history...';
GO

-- Check if __EFMigrationsHistory table exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('__EFMigrationsHistory') AND type = 'U')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32) NOT NULL,
        PRIMARY KEY (MigrationId)
    );
    PRINT '  -> Created __EFMigrationsHistory table';
END
ELSE
BEGIN
    PRINT '  -> __EFMigrationsHistory table already exists';
END
GO

-- List of all migrations in the project (insert only if not exists)
DECLARE @migrations TABLE (
    MigrationId NVARCHAR(150),
    ProductVersion NVARCHAR(32)
);

INSERT INTO @migrations VALUES
    ('20251023034023_InitialCreate', '8.0.0'),
    ('20251030024709_FixDecimalPrecision', '8.0.0'),
    ('20251030054055_AddCampusModel', '8.0.0'),
    ('20251030090332_AddUserBanFeature', '8.0.0'),
    ('20251030194530_SyncAfterFixes', '8.0.0'),
    ('20251031090131_SyncModelAfterMerge', '8.0.0'),
    ('20251031101258_AddNotificationTable', '8.0.0'),
    ('20251031112955_AddEventIdToOrderAndEventQuestionSupport', '8.0.0'),
    ('20251031171934_FixOtpCodeAndVoucherPrecision', '8.0.0'),
    ('20251031232328_AddAISuggestionTable', '8.0.0'),
    ('20251031232942_AddVNPayFieldsToPayment', '8.0.0');

-- Insert migrations only if they don't exist
DECLARE @inserted INT;
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
SELECT m.MigrationId, m.ProductVersion
FROM @migrations m
WHERE NOT EXISTS (
    SELECT 1 FROM __EFMigrationsHistory h WHERE h.MigrationId = m.MigrationId
);

SET @inserted = @@ROWCOUNT;
IF @inserted > 0
    PRINT '  -> Inserted ' + CAST(@inserted AS VARCHAR) + ' migration records';
ELSE
    PRINT '  -> All migrations already in history';

GO

-- ============================================
-- PART 2: Fix Order Table
-- ============================================
PRINT '';
PRINT 'Step 2: Adding missing columns to Order table...';
GO

-- Check if EventId column exists and add if not
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Order]') AND name = 'EventId')
BEGIN
    ALTER TABLE [Order]
    ADD EventId INT NOT NULL DEFAULT 0;
    
    PRINT '  -> Added EventId column to Order table';
END
ELSE
BEGIN
    PRINT '  -> EventId column already exists in Order table';
END
GO

-- Check if OrderAnswers column exists and add if not
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Order]') AND name = 'OrderAnswers')
BEGIN
    ALTER TABLE [Order]
    ADD OrderAnswers NVARCHAR(MAX) NULL;
    
    PRINT '  -> Added OrderAnswers column to Order table';
END
ELSE
BEGIN
    PRINT '  -> OrderAnswers column already exists in Order table';
END
GO

-- Add foreign key constraint if EventId column was added
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Order_Event_EventId')
BEGIN
    ALTER TABLE [Order]
    ADD CONSTRAINT FK_Order_Event_EventId 
    FOREIGN KEY (EventId) REFERENCES [Event](EventId) ON DELETE NO ACTION;
    
    PRINT '  -> Added foreign key constraint FK_Order_Event_EventId';
END
ELSE
BEGIN
    PRINT '  -> Foreign key constraint FK_Order_Event_EventId already exists';
END
GO

-- Add index if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Order_EventId' AND object_id = OBJECT_ID('[Order]'))
BEGIN
    CREATE INDEX IX_Order_EventId ON [Order](EventId);
    
    PRINT '  -> Created index IX_Order_EventId';
END
ELSE
BEGIN
    PRINT '  -> Index IX_Order_EventId already exists';
END
GO

-- Update existing Orders with EventId if it's 0
DECLARE @updated INT;
UPDATE o
SET EventId = (
    SELECT TOP 1 e.EventId
    FROM OrderItem oi
    INNER JOIN TicketType tt ON oi.TicketTypeId = tt.TicketTypeId
    INNER JOIN Event e ON tt.EventId = e.EventId
    WHERE oi.OrderId = o.OrderId
)
FROM [Order] o
WHERE o.EventId = 0 AND EXISTS (
    SELECT 1 FROM OrderItem oi 
    INNER JOIN TicketType tt ON oi.TicketTypeId = tt.TicketTypeId
    WHERE oi.OrderId = o.OrderId
);

SET @updated = @@ROWCOUNT;
IF @updated > 0
    PRINT '  -> Updated ' + CAST(@updated AS VARCHAR) + ' existing Orders with EventId from OrderItems';
ELSE
    PRINT '  -> No Orders needed updating';

GO

-- ============================================
-- PART 3: Create Notification Table
-- ============================================
PRINT '';
PRINT 'Step 3: Creating Notification table...';
GO

-- Create Notification table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[Notification]') AND type = 'U')
BEGIN
    CREATE TABLE [Notification](
        NotificationId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Content NVARCHAR(MAX) NULL,
        Type NVARCHAR(MAX) NOT NULL,
        IsRead BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        ReadAt DATETIME2(0) NULL,
        RelatedEventId INT NULL,
        RelatedOrderId INT NULL,
        RelatedTicketId INT NULL,
        CONSTRAINT FK_Notification_User_UserId 
            FOREIGN KEY (UserId) REFERENCES [User](UserID) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Notification_UserId ON [Notification](UserId);
    
    PRINT '  -> Created Notification table';
END
ELSE
BEGIN
    PRINT '  -> Notification table already exists';
END
GO

-- ============================================
-- Summary
-- ============================================
PRINT '';
PRINT '============================================';
PRINT 'Database Schema Fix Completed Successfully!';
PRINT '============================================';
PRINT '';
GO

-- Display current migration history
PRINT 'Current migration history:';
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;
GO

-- Display schema changes summary
PRINT '';
PRINT 'Schema verification:';
SELECT 
    'Order.EventId' AS ColumnName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Order]') AND name = 'EventId') 
         THEN 'EXISTS' 
         ELSE 'MISSING' 
    END AS Status
UNION ALL
SELECT 
    'Order.OrderAnswers' AS ColumnName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[Order]') AND name = 'OrderAnswers') 
         THEN 'EXISTS' 
         ELSE 'MISSING' 
    END AS Status
UNION ALL
SELECT 
    'Notification table' AS ColumnName,
    CASE WHEN EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('[Notification]') AND type = 'U') 
         THEN 'EXISTS' 
         ELSE 'MISSING' 
    END AS Status;
GO

PRINT '';
PRINT 'Script completed successfully!';
PRINT 'Please restart your backend application now.';
GO

