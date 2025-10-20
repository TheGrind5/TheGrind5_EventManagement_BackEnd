-- ========================================
-- TheGrind5 Event Management
-- Sample Data Insert Script
-- ========================================

use EventDB

-- Fix database collation for Vietnamese characters
ALTER DATABASE EventDB COLLATE Vietnamese_CI_AI;

-- Fix table collations for Vietnamese text
ALTER TABLE [User] ALTER COLUMN FullName NVARCHAR(255) COLLATE Vietnamese_CI_AI;
ALTER TABLE [User] ALTER COLUMN Email NVARCHAR(255) COLLATE Vietnamese_CI_AI;
ALTER TABLE Event ALTER COLUMN Title NVARCHAR(100) COLLATE Vietnamese_CI_AI;
ALTER TABLE Event ALTER COLUMN Description NVARCHAR(MAX) COLLATE Vietnamese_CI_AI;
ALTER TABLE Event ALTER COLUMN Location NVARCHAR(255) COLLATE Vietnamese_CI_AI;

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
DELETE FROM Voucher;
DELETE FROM Wishlist;
DELETE FROM WalletTransaction;
DELETE FROM OtpCode;
DELETE FROM [User];

-- Reset identity columns
DBCC CHECKIDENT ('Payment', RESEED, 0);
DBCC CHECKIDENT ('Ticket', RESEED, 0);
DBCC CHECKIDENT ('OrderItem', RESEED, 0);
DBCC CHECKIDENT ('[Order]', RESEED, 0);
DBCC CHECKIDENT ('TicketType', RESEED, 0);
DBCC CHECKIDENT ('Event', RESEED, 0);
DBCC CHECKIDENT ('Voucher', RESEED, 0);
DBCC CHECKIDENT ('Wishlist', RESEED, 0);
DBCC CHECKIDENT ('WalletTransaction', RESEED, 0);
DBCC CHECKIDENT ('OtpCode', RESEED, 0);
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
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'host1',
    N'Nguyễn Văn Host',
    'host1@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0123456789',
    'Host',
    0.00,
    GETUTCDATE(),
    GETUTCDATE(),
    NULL, -- Avatar
    '1985-03-15', -- DateOfBirth
    N'Nam' -- Gender
);

-- Host 2
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'host2',
    N'Trần Thị Host',
    'host2@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0987654321',
    'Host',
    0.00,
    GETUTCDATE(),
    GETUTCDATE(),
    NULL, -- Avatar
    '1988-07-22', -- DateOfBirth
    N'Nữ' -- Gender
);

-- Customer 1
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'customer1',
    N'Lê Văn Customer',
    'customer1@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0555123456',
    'Customer',
    500000.00,
    GETUTCDATE(),
    GETUTCDATE(),
    NULL, -- Avatar
    '1992-11-08', -- DateOfBirth
    N'Nam' -- Gender
);

-- Customer 2 (Test user with different balance)
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'customer2',
    N'Phạm Thị Test',
    'customer2@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0555987654',
    'Customer',
    1250000.50,
    GETUTCDATE(),
    GETUTCDATE(),
    NULL, -- Avatar
    '1995-04-12', -- DateOfBirth
    N'Nữ' -- Gender
);

-- Test User với wallet balance cao (để test chức năng wallet)
INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'testwallet',
    N'Test Wallet User',
    'testwallet@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0123456789',
    'Customer',
    999999.99,
    GETUTCDATE(),
    GETUTCDATE(),
    NULL, -- Avatar
    '1990-09-25', -- DateOfBirth
    N'Nam' -- Gender
);

-- ========================================
-- INSERT EVENTS (6 events: 3 của host1, 3 của host2)
-- ========================================

-- Events của Host 1 (UserId = 1)
INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt)
VALUES (
    1, -- HostId của host1
    N'Workshop Lập Trình Web',
    N'Workshop học lập trình web từ cơ bản đến nâng cao với React và Node.js',
    DATEADD(day, 7, GETUTCDATE()),
    DATEADD(day, 7, DATEADD(hour, 4, GETUTCDATE())),
    N'Hà Nội, Việt Nam',
    N'Workshop',
    'Offline',
    'Technology',
    'Open',
    N'{"venue": "Trung tâm Hội nghị Quốc gia", "images": ["workshop1.jpg", "workshop2.jpg"], "introduction": "Khóa học lập trình web toàn diện", "specialGuests": ["Nguyễn Văn A - Senior Developer"]}',
    N'{"terms": "Khong hoan tien sau khi dang ky", "childrenTerms": "Trẻ em dưới 16 tuổi cần người giám hộ", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "techworkshop_logo.png", "name": "TechWorkshop Vietnam", "info": "Tổ chức các khóa học công nghệ hàng đầu"}',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt)
VALUES (
    1, -- HostId của host1
    N'Hội Thảo AI & Machine Learning',
    N'Hội thảo về trí tuệ nhân tạo và machine learning trong thời đại 4.0',
    DATEADD(day, 14, GETUTCDATE()),
    DATEADD(day, 14, DATEADD(hour, 6, GETUTCDATE())),
    N'TP.HCM, Việt Nam',
    N'Conference',
    'Offline',
    'Technology',
    'Open',
    N'{"venue": "Trung tâm Hội nghị TP.HCM", "images": ["ai1.jpg", "ai2.jpg"], "introduction": "Hội thảo AI hàng đầu Việt Nam", "specialGuests": ["GS. Trần Văn B - AI Expert"]}',
    N'{"terms": "Co the hoan tien truoc 7 ngay", "childrenTerms": "Khong phu hop cho tre em", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "aiconference_logo.png", "name": "AI Vietnam Conference", "info": "Tổ chức hội thảo AI chuyên nghiệp"}',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt)
VALUES (
    1, -- HostId của host1
    N'Sự Kiện Networking Startup',
    N'Gặp gỡ và kết nối với các startup và nhà đầu tư trong lĩnh vực công nghệ',
    DATEADD(day, 21, GETUTCDATE()),
    DATEADD(day, 21, DATEADD(hour, 3, GETUTCDATE())),
    N'Đà Nẵng, Việt Nam',
    N'Networking',
    'Offline',
    'Business',
    'Open',
    N'{"venue": "Khách sạn 5 sao Đà Nẵng", "images": ["networking1.jpg", "networking2.jpg"], "introduction": "Sự kiện kết nối startup lớn nhất miền Trung", "specialGuests": ["CEO Startup A", "Nhà đầu tư B"]}',
    N'{"terms": "Mien phi tham du", "childrenTerms": "Khong phu hop cho tre em", "vatTerms": "Khong ap dung VAT"}',
    N'{"logo": "startupnetwork_logo.png", "name": "Startup Network Vietnam", "info": "Tổ chức sự kiện kết nối startup"}',
    GETUTCDATE(),
    GETUTCDATE()
);

-- Events của Host 2 (UserId = 2)
INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt)
VALUES (
    2, -- HostId của host2
    N'Concert Nhạc Acoustic',
    N'Đêm nhạc acoustic với các ca sĩ trẻ tài năng',
    DATEADD(day, 10, GETUTCDATE()),
    DATEADD(day, 10, DATEADD(hour, 3, GETUTCDATE())),
    N'Hà Nội, Việt Nam',
    N'Concert',
    'Offline',
    'Entertainment',
    'Open',
    N'{"venue": "Nhà hát Lớn Hà Nội", "images": ["concert1.jpg", "concert2.jpg"], "introduction": "Đêm nhạc acoustic đặc sắc", "specialGuests": ["Ca sĩ A", "Ca sĩ B"]}',
    N'{"terms": "Khong hoan tien sau khi mua", "childrenTerms": "Trẻ em dưới 3 tuổi miễn phí", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "acousticconcert_logo.png", "name": "Acoustic Music Vietnam", "info": "Tổ chức các buổi hòa nhạc acoustic"}',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt)
VALUES (
    2, -- HostId của host2
    N'Triển Lãm Nghệ Thuật Đương Đại',
    N'Triển lãm các tác phẩm nghệ thuật đương đại của các nghệ sĩ trẻ',
    DATEADD(day, 18, GETUTCDATE()),
    DATEADD(day, 20, GETUTCDATE()),
    N'TP.HCM, Việt Nam',
    N'Exhibition',
    'Offline',
    'Art',
    'Open',
    N'{"venue": "Bảo tàng Mỹ thuật TP.HCM", "images": ["art1.jpg", "art2.jpg"], "introduction": "Triển lãm nghệ thuật đương đại lớn nhất năm", "specialGuests": ["Nghệ sĩ A", "Nghệ sĩ B"]}',
    N'{"terms": "Co the hoan tien truoc 1 ngay", "childrenTerms": "Trẻ em dưới 12 tuổi miễn phí", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "artexhibition_logo.png", "name": "Contemporary Art Vietnam", "info": "Tổ chức triển lãm nghệ thuật đương đại"}',
    GETUTCDATE(),
    GETUTCDATE()
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt)
VALUES (
    2, -- HostId của host2
    N'Workshop Nấu Ăn Healthy',
    N'Học cách nấu các món ăn healthy và dinh dưỡng cho gia đình',
    DATEADD(day, 25, GETUTCDATE()),
    DATEADD(day, 25, DATEADD(hour, 4, GETUTCDATE())),
    N'Hà Nội, Việt Nam',
    N'Workshop',
    'Offline',
    'Lifestyle',
    'Open',
    N'{"venue": "Trung tâm ẩm thực Hà Nội", "images": ["cooking1.jpg", "cooking2.jpg"], "introduction": "Workshop nau an healthy cho moi gia dinh", "specialGuests": ["Đầu bếp A", "Chuyên gia dinh dưỡng B"]}',
    N'{"terms": "Co the hoan tien truoc 3 ngay", "childrenTerms": "Trẻ em từ 8 tuổi trở lên", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "healthyfood_logo.png", "name": "Healthy Food Vietnam", "info": "Tổ chức workshop nau an healthy"}',
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

-- ========================================
-- VERIFICATION QUERIES
-- ========================================

-- Kiểm tra Users đã được tạo
SELECT 'Users Created:' as Info, COUNT(*) as Count FROM [User];

SELECT UserId, FullName, Email, Role FROM [User] ORDER BY UserId;


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

-- ========================================
-- INSERT VOUCHERS
-- ========================================

-- Voucher giảm 10%
INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'WELCOME10',
    10, -- Giảm 10%
    GETUTCDATE(), -- Có hiệu lực từ hôm nay
    DATEADD(month, 3, GETUTCDATE()), -- Hết hạn sau 3 tháng
    1, -- Active
    GETUTCDATE()
);

-- Voucher giảm 20%
INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'SAVE20',
    20, -- Giảm 20%
    GETUTCDATE(), -- Có hiệu lực từ hôm nay
    DATEADD(month, 2, GETUTCDATE()), -- Hết hạn sau 2 tháng
    1, -- Active
    GETUTCDATE()
);

-- Voucher giảm 15% (đã hết hạn để test)
INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'EXPIRED15',
    15, -- Giảm 15%
    DATEADD(month, -2, GETUTCDATE()), -- Có hiệu lực từ 2 tháng trước
    DATEADD(month, -1, GETUTCDATE()), -- Hết hạn 1 tháng trước
    1, -- Active (nhưng đã hết hạn)
    GETUTCDATE()
);

-- Voucher giảm 25%
INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'SUMMER25',
    25, -- Giảm 25%
    GETUTCDATE(), -- Có hiệu lực từ hôm nay
    DATEADD(month, 1, GETUTCDATE()), -- Hết hạn sau 1 tháng
    1, -- Active
    GETUTCDATE()
);

-- Voucher giảm 30%
INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'VIP30',
    30, -- Giảm 30%
    GETUTCDATE(), -- Có hiệu lực từ hôm nay
    DATEADD(month, 6, GETUTCDATE()), -- Hết hạn sau 6 tháng
    1, -- Active
    GETUTCDATE()
);

-- Kiểm tra Vouchers đã được tạo
SELECT 'Vouchers Created:' as Info, COUNT(*) as Count FROM Voucher;
SELECT VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive FROM Voucher ORDER BY VoucherCode;

PRINT '========================================';
PRINT 'Sample data injection completed successfully!';
PRINT '5 Users created (2 Hosts, 3 Customers)';
PRINT '6 Events created (3 per Host)';
PRINT '5 Vouchers created';

PRINT 'Customer 1: 500,000 VND';
PRINT 'Customer 2: 1,250,000.50 VND';
PRINT 'Test Wallet User: 999,999.99 VND';
PRINT 'All users password: 123456';

PRINT '11 Ticket Types created for all events';
PRINT '5 Vouchers created (WELCOME10, SAVE20, EXPIRED15, SUMMER25, VIP30)';

PRINT '========================================';
