-- ========================================
-- TheGrind5 Event Management
-- Sample Data Insert Script
-- ========================================

use EventDB

-- ========================================
-- CLEAR EXISTING DATA (in correct order due to foreign keys)
-- ========================================

-- Delete in reverse order of dependencies
DELETE FROM Payment;
DELETE FROM Ticket;
DELETE FROM OrderItem;
DELETE FROM [Order];
DELETE FROM TicketType;
DELETE FROM Event;
DELETE FROM [User];

-- Reset identity columns
DBCC CHECKIDENT ('Payment', RESEED, 0);
DBCC CHECKIDENT ('Ticket', RESEED, 0);
DBCC CHECKIDENT ('OrderItem', RESEED, 0);
DBCC CHECKIDENT ('[Order]', RESEED, 0);
DBCC CHECKIDENT ('TicketType', RESEED, 0);
DBCC CHECKIDENT ('Event', RESEED, 0);
DBCC CHECKIDENT ('[User]', RESEED, 0);

-- ========================================
-- INSERT USERS (5 users: 2 hosts, 3 customers)
-- ========================================
-- 
-- PASSWORD HASH: Tất cả users sử dụng password "123456"
-- Hash được tạo bằng bcrypt: $2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u
-- 
-- LOGIN CREDENTIALS:
-- Email: host1@example.com / Password: 123456
-- Email: host2@example.com / Password: 123456  
-- Email: customer1@example.com / Password: 123456 (500,000 VND)
-- Email: customer2@example.com / Password: 123456 (1,250,000.50 VND)
-- Email: testwallet@example.com / Password: 123456 (999,999.99 VND)
-- ========================================

-- Host 1
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt)
VALUES (
    'host1',
    N'Nguyễn Văn Host',
    'host1@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0123456789',
    'Host',
    0.00,
    GETUTCDATE(),
    GETUTCDATE()
);

-- Host 2
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt)
VALUES (
    'host2',
    N'Trần Thị Host',
    'host2@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0987654321',
    'Host',
    0.00,
    GETUTCDATE(),
    GETUTCDATE()
);

-- Customer 1
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt)
VALUES (
    'customer1',
    N'Lê Văn Customer',
    'customer1@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0555123456',
    'Customer',
    500000.00,
    GETUTCDATE(),
    GETUTCDATE()
);

-- Customer 2 (Test user with different balance)
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt)
VALUES (
    'customer2',
    N'Phạm Thị Test',
    'customer2@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0555987654',
    'Customer',
    1250000.50,
    GETUTCDATE(),
    GETUTCDATE()
);

-- Test User với wallet balance cao (để test chức năng wallet)
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt)
VALUES (
    'testwallet',
    N'Test Wallet User',
    'testwallet@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0123456789',
    'Customer',
    999999.99,
    GETUTCDATE(),
    GETUTCDATE()
);

-- ========================================
-- INSERT EVENTS (6 events: 3 của host1, 3 của host2)
-- ========================================

-- Events của Host 1 (UserID = 1)
INSERT INTO Event (HostID, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
VALUES (
    1, -- HostId của host1
    N'Workshop Lập Trình Web',
    N'Workshop học lập trình web từ cơ bản đến nâng cao với React và Node.js',
    DATEADD(day, 7, GETUTCDATE()),
    DATEADD(day, 7, DATEADD(hour, 4, GETUTCDATE())),
    N'Hà Nội, Việt Nam',
    'Technology',
    'Open',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostID, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
VALUES (
    1, -- HostId của host1
    N'Hội Thảo AI & Machine Learning',
    N'Hội thảo về trí tuệ nhân tạo và machine learning trong thời đại 4.0',
    DATEADD(day, 14, GETUTCDATE()),
    DATEADD(day, 14, DATEADD(hour, 6, GETUTCDATE())),
    N'TP.HCM, Việt Nam',
    'Technology',
    'Open',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostID, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
VALUES (
    1, -- HostId của host1
    N'Sự Kiện Networking Startup',
    N'Gặp gỡ và kết nối với các startup và nhà đầu tư trong lĩnh vực công nghệ',
    DATEADD(day, 21, GETUTCDATE()),
    DATEADD(day, 21, DATEADD(hour, 3, GETUTCDATE())),
    N'Đà Nẵng, Việt Nam',
    'Business',
    'Open',
    GETUTCDATE(),
    GETUTCDATE()
);

-- Events của Host 2 (UserId = 2)
INSERT INTO Event (HostID, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
VALUES (
    2, -- HostId của host2
    N'Concert Nhạc Acoustic',
    N'Đêm nhạc acoustic với các ca sĩ trẻ tài năng',
    DATEADD(day, 10, GETUTCDATE()),
    DATEADD(day, 10, DATEADD(hour, 3, GETUTCDATE())),
    N'Hà Nội, Việt Nam',
    'Entertainment',
    'Open',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostID, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
VALUES (
    2, -- HostId của host2
    N'Triển Lãm Nghệ Thuật Đương Đại',
    N'Triển lãm các tác phẩm nghệ thuật đương đại của các nghệ sĩ trẻ',
    DATEADD(day, 18, GETUTCDATE()),
    DATEADD(day, 20, GETUTCDATE()),
    N'TP.HCM, Việt Nam',
    'Art',
    'Open',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostID, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
VALUES (
    2, -- HostId của host2
    N'Workshop Nấu Ăn Healthy',
    N'Học cách nấu các món ăn healthy và dinh dưỡng cho gia đình',
    DATEADD(day, 25, GETUTCDATE()),
    DATEADD(day, 25, DATEADD(hour, 4, GETUTCDATE())),
    N'Hà Nội, Việt Nam',
    'Lifestyle',
    'Open',
    GETUTCDATE(),
    GETUTCDATE()
);

-- ========================================
-- VERIFICATION QUERIES
-- ========================================

-- Kiểm tra Users đã được tạo
SELECT 'Users Created:' as Info, COUNT(*) as Count FROM [User];
SELECT UserID, Username, FullName, Email, Role, WalletBalance FROM [User] ORDER BY UserID;

-- Kiểm tra Events đã được tạo
SELECT 'Events Created:' as Info, COUNT(*) as Count FROM Event;
SELECT EventID, HostID, Title, Category, Status FROM Event ORDER BY EventID;

-- Kiểm tra Events theo Host
SELECT 
    u.FullName as HostName,
    u.Email as HostEmail,
    COUNT(e.EventID) as EventCount
FROM [User] u
LEFT JOIN Event e ON u.UserID = e.HostID
WHERE u.Role = 'Host'
GROUP BY u.UserID, u.FullName, u.Email;

PRINT '========================================';
PRINT 'Sample data injection completed successfully!';
PRINT '5 Users created (2 Hosts, 3 Customers)';
PRINT '6 Events created (3 per Host)';
PRINT 'Customer 1: 500,000 VND';
PRINT 'Customer 2: 1,250,000.50 VND';
PRINT 'Test Wallet User: 999,999.99 VND';
PRINT 'All users password: 123456';
PRINT '========================================';
