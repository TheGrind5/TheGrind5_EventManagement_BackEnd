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

-- Verify tables are empty
PRINT 'Verifying tables are empty...';
SELECT COUNT(*) as UserCount FROM [User];
SELECT COUNT(*) as EventCount FROM Event;
SELECT COUNT(*) as TicketTypeCount FROM TicketType;

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

-- Events của Host 1 (UserId = 1)
INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
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

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
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

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
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
INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
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

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
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

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, Category, Status, CreatedAt, UpdatedAt)
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

-- Verify Events were created successfully
PRINT 'Verifying Events were created...';
SELECT EventId, Title, HostId FROM Event ORDER BY EventId;

-- ========================================
-- INSERT TICKET TYPES (Ticket types cho tất cả events)
-- ========================================

-- Ticket Types cho Event 1: Workshop Lập Trình Web
INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    1, -- EventId của Workshop Lập Trình Web
    N'Vé Thường',
    150000, -- 150k VND
    50, -- 50 vé
    1, -- Min order 1 vé
    5, -- Max order 5 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 6, GETUTCDATE()), -- Bán đến 6 ngày trước event
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    1, -- EventId của Workshop Lập Trình Web
    N'Vé VIP',
    250000, -- 250k VND
    20, -- 20 vé
    1, -- Min order 1 vé
    3, -- Max order 3 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 6, GETUTCDATE()), -- Bán đến 6 ngày trước event
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    1, -- EventId của Workshop Lập Trình Web
    N'Vé Sinh Viên',
    100000, -- 100k VND
    30, -- 30 vé
    1, -- Min order 1 vé
    2, -- Max order 2 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 6, GETUTCDATE()), -- Bán đến 6 ngày trước event
    'Active'
);

-- Ticket Types cho Event 2: Hội Thảo AI & Machine Learning
INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    2, -- EventId của Hội Thảo AI & Machine Learning
    N'Vé Thường',
    200000, -- 200k VND
    100, -- 100 vé
    1, -- Min order 1 vé
    10, -- Max order 10 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 13, GETUTCDATE()), -- Bán đến 13 ngày trước event
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    2, -- EventId của Hội Thảo AI & Machine Learning
    N'Vé Premium',
    350000, -- 350k VND
    50, -- 50 vé
    1, -- Min order 1 vé
    5, -- Max order 5 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 13, GETUTCDATE()), -- Bán đến 13 ngày trước event
    'Active'
);

-- Ticket Types cho Event 3: Sự Kiện Networking Startup
INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    3, -- EventId của Sự Kiện Networking Startup
    N'Vé Thường',
    300000, -- 300k VND
    80, -- 80 vé
    1, -- Min order 1 vé
    8, -- Max order 8 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 20, GETUTCDATE()), -- Bán đến 20 ngày trước event
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    3, -- EventId của Sự Kiện Networking Startup
    N'Vé VIP',
    500000, -- 500k VND
    20, -- 20 vé
    1, -- Min order 1 vé
    4, -- Max order 4 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 20, GETUTCDATE()), -- Bán đến 20 ngày trước event
    'Active'
);

-- Ticket Types cho Event 4: Concert Nhạc Acoustic
INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    4, -- EventId của Concert Nhạc Acoustic
    N'Vé Thường',
    180000, -- 180k VND
    150, -- 150 vé
    1, -- Min order 1 vé
    6, -- Max order 6 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 9, GETUTCDATE()), -- Bán đến 9 ngày trước event
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    4, -- EventId của Concert Nhạc Acoustic
    N'Vé VIP',
    320000, -- 320k VND
    50, -- 50 vé
    1, -- Min order 1 vé
    4, -- Max order 4 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 9, GETUTCDATE()), -- Bán đến 9 ngày trước event
    'Active'
);

-- Ticket Types cho Event 5: Triển Lãm Nghệ Thuật Đương Đại
INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    5, -- EventId của Triển Lãm Nghệ Thuật Đương Đại
    N'Vé Thường',
    120000, -- 120k VND
    200, -- 200 vé
    1, -- Min order 1 vé
    10, -- Max order 10 vé
    DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
    DATEADD(day, 17, GETUTCDATE()), -- Bán đến 17 ngày trước event
    'Active'
);

-- Verify Event 6 exists before inserting TicketTypes
IF EXISTS (SELECT 1 FROM Event WHERE EventId = 6)
BEGIN
    PRINT 'Event 6 exists, inserting TicketTypes...';
    
    -- Ticket Types cho Event 6: Workshop Nấu Ăn Healthy
    INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
    VALUES (
        6, -- EventId của Workshop Nấu Ăn Healthy
        N'Vé Thường',
        280000, -- 280k VND
        40, -- 40 vé
        1, -- Min order 1 vé
        4, -- Max order 4 vé
        DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
        DATEADD(day, 24, GETUTCDATE()), -- Bán đến 24 ngày trước event
        'Active'
    );

    INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
    VALUES (
        6, -- EventId của Workshop Nấu Ăn Healthy
        N'Vé Cặp Đôi',
        500000, -- 500k VND (giá cho 2 người)
        20, -- 20 cặp (40 người)
        1, -- Min order 1 cặp
        2, -- Max order 2 cặp
        DATEADD(day, -30, GETUTCDATE()), -- Bán từ 30 ngày trước
        DATEADD(day, 24, GETUTCDATE()), -- Bán đến 24 ngày trước event
        'Active'
    );
END
ELSE
BEGIN
    PRINT 'ERROR: Event 6 does not exist! Cannot insert TicketTypes.';
    SELECT EventId, Title FROM Event ORDER BY EventId;
END

-- ========================================
-- VERIFICATION QUERIES
-- ========================================

-- Kiểm tra Users đã được tạo
SELECT 'Users Created:' as Info, COUNT(*) as Count FROM [User];

SELECT UserId, Username, FullName, Email, Role FROM [User] ORDER BY UserId;


-- Kiểm tra Events đã được tạo
SELECT 'Events Created:' as Info, COUNT(*) as Count FROM Event;
SELECT EventId, HostId, Title, Category, Status FROM Event ORDER BY EventId;

-- Kiểm tra Ticket Types đã được tạo
SELECT 'Ticket Types Created:' as Info, COUNT(*) as Count FROM TicketType;

-- Kiểm tra Ticket Types theo Event
SELECT 
    e.EventId,
    e.Title as EventTitle,
    COUNT(tt.TicketTypeId) as TicketTypeCount
FROM Event e
LEFT JOIN TicketType tt ON e.EventId = tt.EventId
GROUP BY e.EventId, e.Title
ORDER BY e.EventId;

-- Chi tiết Ticket Types
SELECT 
    tt.TicketTypeId,
    tt.EventId,
    e.Title as EventTitle,
    tt.TypeName,
    tt.Price,
    tt.Quantity,
    tt.Status
FROM TicketType tt
JOIN Event e ON tt.EventId = e.EventId
ORDER BY tt.EventId, tt.Price;

-- Kiểm tra Events theo Host
SELECT 
    u.FullName as HostName,
    u.Email as HostEmail,
    COUNT(e.EventId) as EventCount
FROM [User] u
LEFT JOIN Event e ON u.UserId = e.HostId
WHERE u.Role = 'Host'
GROUP BY u.UserId, u.FullName, u.Email;

PRINT '========================================';
PRINT 'Sample data injection completed successfully!';
PRINT '5 Users created (2 Hosts, 3 Customers)';
PRINT '6 Events created (3 per Host)';

PRINT 'Customer 1: 500,000 VND';
PRINT 'Customer 2: 1,250,000.50 VND';
PRINT 'Test Wallet User: 999,999.99 VND';
PRINT 'All users password: 123456';

PRINT '11 Ticket Types created for all events';

PRINT '========================================';
