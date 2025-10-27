-- ========================================
-- TheGrind5 Event Management
-- Sample Data Insert Script (Updated from Database)
-- Generated: 2025-10-27 08:59:52
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
-- INSERT USERS (5 users: 2 hosts, 3 customers + test user)
-- ========================================
-- PASSWORD HASH: All users use password '123456'
-- Hash: $2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u

INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'host1',
    N'Nguyễn Văn Host',
    'host1@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0123456789',
    'Host',
    0.00,
    '2025-10-26 14:30:08.000',
    '2025-10-26 14:30:08.000',
    NULL,
    '1985-03-15',
    N'Nam'
);

INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'host2',
    N'Trần Thị Host',
    'host2@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0987654321',
    'Host',
    0.00,
    '2025-10-26 14:30:08.000',
    '2025-10-26 14:30:08.000',
    NULL,
    '1988-07-22',
    N'Nữ'
);

INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'customer1',
    N'Lê Văn Customer',
    'customer1@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0555123456',
    'Customer',
    500000.00,
    '2025-10-26 14:30:08.000',
    '2025-10-26 14:30:08.000',
    NULL,
    '1992-11-08',
    N'Nam'
);

INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'customer2',
    N'Phạm Thị Test',
    'customer2@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0555987654',
    'Customer',
    1250000.50,
    '2025-10-26 14:30:08.000',
    '2025-10-26 14:30:08.000',
    NULL,
    '1995-04-12',
    N'Nữ'
);

INSERT INTO [User] (Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
VALUES (
    'testwallet',
    N'Test Wallet User',
    'testwallet@example.com',
    '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',
    '0123456789',
    'Customer',
    999999.99,
    '2025-10-26 14:30:08.000',
    '2025-10-26 14:30:08.000',
    NULL,
    '1990-09-25',
    N'Nam'
);


-- Verify Users were created successfully
PRINT 'Users created:';
SELECT UserId, FullName, Email, Role FROM [User] ORDER BY UserId;

-- ========================================
-- INSERT EVENTS (19  (1 rows affected)  events from database)
-- ========================================

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Workshop Lập Trình Web',
    N'Workshop học lập trình web từ cơ bản đến nâng cao với React và Node.js',
    '2025-11-02 14:30:08.000',
    '2025-11-02 18:30:08.000',
    N'Hà Nội, Việt Nam',
    N'Workshop',
    'Offline',
    'Technology',
    'Open',
    N'{"venue": "Trung tâm Hội nghị Quốc gia", "images": ["workshop1.jpg", "workshop2.jpg"], "introduction": "Khóa học lập trình web toàn diện", "specialGuests": ["Nguyễn Văn A - Senior Developer"]}',
    N'{"terms": "Khong hoan tien sau khi dang ky", "childrenTerms": "Trẻ em dưới 16 tuổi cần người giám hộ", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "techworkshop_logo.png", "name": "TechWorkshop Vietnam", "info": "Tổ chức các khóa học công nghệ hàng đầu"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hội Thảo AI & Machine Learning',
    N'Hội thảo về trí tuệ nhân tạo và machine learning trong thời đại 4.0',
    '2025-11-09 14:30:08.000',
    '2025-11-09 20:30:08.000',
    N'TP.HCM, Việt Nam',
    N'Conference',
    'Offline',
    'Technology',
    'Open',
    N'{"venue": "Trung tâm Hội nghị TP.HCM", "images": ["ai1.jpg", "ai2.jpg"], "introduction": "Hội thảo AI hàng đầu Việt Nam", "specialGuests": ["GS. Trần Văn B - AI Expert"]}',
    N'{"terms": "Co the hoan tien truoc 7 ngay", "childrenTerms": "Khong phu hop cho tre em", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "aiconference_logo.png", "name": "AI Vietnam Conference", "info": "Tổ chức hội thảo AI chuyên nghiệp"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Sự Kiện Networking Startup',
    N'Gặp gỡ và kết nối với các startup và nhà đầu tư trong lĩnh vực công nghệ',
    '2025-11-16 14:30:08.000',
    '2025-11-16 17:30:08.000',
    N'Đà Nẵng, Việt Nam',
    N'Networking',
    'Offline',
    'Business',
    'Open',
    N'{"venue": "Khách sạn 5 sao Đà Nẵng", "images": ["networking1.jpg", "networking2.jpg"], "introduction": "Sự kiện kết nối startup lớn nhất miền Trung", "specialGuests": ["CEO Startup A", "Nhà đầu tư B"]}',
    N'{"terms": "Mien phi tham du", "childrenTerms": "Khong phu hop cho tre em", "vatTerms": "Khong ap dung VAT"}',
    N'{"logo": "startupnetwork_logo.png", "name": "Startup Network Vietnam", "info": "Tổ chức sự kiện kết nối startup"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    2,
    N'Concert Nhạc Acoustic',
    N'Đêm nhạc acoustic với các ca sĩ trẻ tài năng',
    '2025-11-05 14:30:08.000',
    '2025-11-05 17:30:08.000',
    N'Hà Nội, Việt Nam',
    N'Concert',
    'Offline',
    'Entertainment',
    'Open',
    N'{"venue": "Nhà hát Lớn Hà Nội", "images": ["concert1.jpg", "concert2.jpg"], "introduction": "Đêm nhạc acoustic đặc sắc", "specialGuests": ["Ca sĩ A", "Ca sĩ B"]}',
    N'{"terms": "Khong hoan tien sau khi mua", "childrenTerms": "Trẻ em dưới 3 tuổi miễn phí", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "acousticconcert_logo.png", "name": "Acoustic Music Vietnam", "info": "Tổ chức các buổi hòa nhạc acoustic"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    2,
    N'Triển Lãm Nghệ Thuật Đương Đại',
    N'Triển lãm các tác phẩm nghệ thuật đương đại của các nghệ sĩ trẻ',
    '2025-11-13 14:30:08.000',
    '2025-11-15 14:30:08.000',
    N'TP.HCM, Việt Nam',
    N'Exhibition',
    'Offline',
    'Art',
    'Open',
    N'{"venue": "Bảo tàng Mỹ thuật TP.HCM", "images": ["art1.jpg", "art2.jpg"], "introduction": "Triển lãm nghệ thuật đương đại lớn nhất năm", "specialGuests": ["Nghệ sĩ A", "Nghệ sĩ B"]}',
    N'{"terms": "Co the hoan tien truoc 1 ngay", "childrenTerms": "Trẻ em dưới 12 tuổi miễn phí", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "artexhibition_logo.png", "name": "Contemporary Art Vietnam", "info": "Tổ chức triển lãm nghệ thuật đương đại"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    2,
    N'Workshop Nấu Ăn Healthy',
    N'Học cách nấu các món ăn healthy và dinh dưỡng cho gia đình',
    '2025-11-20 14:30:08.000',
    '2025-11-20 18:30:08.000',
    N'Hà Nội, Việt Nam',
    N'Workshop',
    'Offline',
    'Lifestyle',
    'Open',
    N'{"venue": "Trung tâm ẩm thực Hà Nội", "images": ["cooking1.jpg", "cooking2.jpg"], "introduction": "Workshop nau an healthy cho moi gia dinh", "specialGuests": ["Đầu bếp A", "Chuyên gia dinh dưỡng B"]}',
    N'{"terms": "Co the hoan tien truoc 3 ngay", "childrenTerms": "Trẻ em từ 8 tuổi trở lên", "vatTerms": "Đã bao gồm VAT 10%"}',
    N'{"logo": "healthyfood_logo.png", "name": "Healthy Food Vietnam", "info": "Tổ chức workshop nau an healthy"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè - Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm

🗓 Ngày: 10 tháng 8 năm 2025

🕖 Giờ: 19h30 – 22h00

📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)

Hạng VIP: 900.000đ

Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:

Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”

Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):

Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”

Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:

Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.

Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”

“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý

Giám đốc âm nhạc: Long Halo

Đạo diễn sân khấu: Huỳnh Tuấn Kiệt

Ban nhạc: The July Notes Band

Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment

Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN

Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé

🌐 Website: www.hasauhe.vn

📧 Email: concert.hasauhe@gmail.com

☎️ Hotline: 0906 612 123',
    '2025-10-31 10:30:00.000',
    '2025-10-31 17:30:00.000',
    N'',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn Ng\u0169 H\u00E0nh S\u01A1n","Ward":"","EventImage":"/uploads/events/4e351089-f1f4-4489-8b5c-4a055c975567.png","BackgroundImage":"/uploads/events/488220be-8894-47b4-97cf-73f00870be28.jpg","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\n\nH\u1EA1ng VIP: 900.000\u0111\n\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\n\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\n\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\n\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\n\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\n\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\n\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\n\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\n\nBan nh\u1EA1c: The July Notes Band\n\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\n\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\n\uD83C\uDF10 Website: www.hasauhe.vn\n\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\n\nH\u1EA1ng VIP: 900.000\u0111\n\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\n\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\n\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\n\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\n\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\n\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\n\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\n\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\n\nBan nh\u1EA1c: The July Notes Band\n\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\n\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\n\uD83C\uDF10 Website: www.hasauhe.vn\n\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/6012dec1-42f5-41de-9c0f-6bb649e60178.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng L\u00E0m Nh\u1EA1c","OrganizerInfo":"Clb tr\u1EF1c thu\u1ED9c tr\u01B0\u1EDDng \u0111\u1EA1i h\u1ECDc FPT \u0110\u00E0 N\u1EB5ng"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Mơ Thay Em - Đêm Nhạc Thiện Nguyện Vì Trẻ Em Vùng Cao - Tân Lý',
    N'“Mơ Thay Em” là một đêm nhạc thiện nguyện đầy cảm xúc, nơi âm nhạc trở thành cầu nối giữa trái tim nghệ sĩ và những ước mơ nhỏ bé nơi vùng cao xa xôi.
Chương trình hướng tới gây quỹ “Áo Ấm Cho Em”, hỗ trợ mua sách vở, quần áo mùa đông và học cụ cho trẻ em tại các điểm trường vùng núi phía Bắc Việt Nam.

💫 Thời gian & Địa điểm
🗓 Ngày: 15 tháng 12 năm 2025
🕘 Giờ: 19h00 – 22h30
📍 Địa điểm: Nhà hát VOH – Đài Tiếng nói Nhân dân TP.HCM, số 37 Nguyễn Bỉnh Khiêm, Quận 1
🎟 Vé mời: Toàn bộ tiền vé và quyên góp được trích 100% vào quỹ “Áo Ấm Cho Em”
🎤 Nội dung chương trình
Mở màn: Tiết mục hợp ca “Gửi Em Nụ Cười Mùa Đông” – trình diễn bởi dàn hợp xướng thiếu nhi Hòa Bình.

Phần 1 – Những Giấc Mơ Bé Nhỏ:
Trình diễn acoustic với các ca sĩ trẻ như Thịnh Suy, Vũ Cát Tường, Orange.
Chủ đề: Ước mơ, tuổi thơ, và niềm tin.

Phần 2 – Mơ Thay Em:
Sân khấu chính, trình diễn live band cùng các nghệ sĩ khách mời Hà Anh Tuấn, Mỹ Tâm, Đen Vâu.
Gây quỹ trực tiếp bằng hình thức auction (đấu giá vật phẩm nghệ sĩ tặng).

Phần 3 – Trao Yêu Thương:
Công bố số tiền quyên góp được.
Trình chiếu video hành trình trao quà và xây lớp học vùng cao.

❤️ Thông điệp chương trình

“Có những giấc mơ trẻ em không thể nói ra,
nhưng chúng ta có thể hát thay, mơ thay em —
để mùa đông này không còn lạnh trên đôi chân bé nhỏ.”

🧺 Đơn vị tổ chức:

Ban Tổ Chức: Nhóm thiện nguyện Hành Trình Xanh phối hợp cùng Đài VOH và Quỹ Trẻ Em Việt
Bảo trợ truyền thông: HTV, VTV6, Báo Tuổi Trẻ, Zing News
Đối tác tài trợ: Vinamilk, PNJ, Yamaha Music Việt Nam
📞 Liên hệ tham gia / tài trợ
📧 Email: mothayem.charity@gmail.com
☎️ Hotline: 0909 123 456
🌐 Website: www.mothayem.vn

Payment Method: bank_transfer
Bank Account: [{"bankName":"MB Bank","accountNumber":"04358345653","accountHolder":"Khanh Ngu da","isDefault":true}]
Tax Info: ',
    '2025-11-06 23:30:00.000',
    '2025-11-07 23:35:00.000',
    N'123 Đường Lê Duẩn, Phường Hải Châu II, Quận Hải Châu, Đà Nẵng',
    N'Public',
    'Offline',
    'Music',
    'Open',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 \u0110\u01B0\u1EDDng L\u00EA Du\u1EA9n","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng H\u1EA3i Ch\u00E2u II","EventImage":"/uploads/events/7d306195-172d-4789-9aaa-e993961caa5a.jpg","BackgroundImage":"/uploads/events/c5c1fd41-06ac-451f-9054-52afd76350b7.jpeg","EventIntroduction":"\u201CM\u01A1 Thay Em\u201D l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c thi\u1EC7n nguy\u1EC7n \u0111\u1EA7y c\u1EA3m x\u00FAc, n\u01A1i \u00E2m nh\u1EA1c tr\u1EDF th\u00E0nh c\u1EA7u n\u1ED1i gi\u1EEFa tr\u00E1i tim ngh\u1EC7 s\u0129 v\u00E0 nh\u1EEFng \u01B0\u1EDBc m\u01A1 nh\u1ECF b\u00E9 n\u01A1i v\u00F9ng cao xa x\u00F4i.\nCh\u01B0\u01A1ng tr\u00ECnh h\u01B0\u1EDBng t\u1EDBi g\u00E2y qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D, h\u1ED7 tr\u1EE3 mua s\u00E1ch v\u1EDF, qu\u1EA7n \u00E1o m\u00F9a \u0111\u00F4ng v\u00E0 h\u1ECDc c\u1EE5 cho tr\u1EBB em t\u1EA1i c\u00E1c \u0111i\u1EC3m tr\u01B0\u1EDDng v\u00F9ng n\u00FAi ph\u00EDa B\u1EAFc Vi\u1EC7t Nam.\n\n\uD83D\uDCAB Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 15 th\u00E1ng 12 n\u0103m 2025\n\uD83D\uDD58 Gi\u1EDD: 19h00 \u2013 22h30\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t VOH \u2013 \u0110\u00E0i Ti\u1EBFng n\u00F3i Nh\u00E2n d\u00E2n TP.HCM, s\u1ED1 37 Nguy\u1EC5n B\u1EC9nh Khi\u00EAm, Qu\u1EADn 1\n\uD83C\uDF9F V\u00E9 m\u1EDDi: To\u00E0n b\u1ED9 ti\u1EC1n v\u00E9 v\u00E0 quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c tr\u00EDch 100% v\u00E0o qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D\n\uD83C\uDFA4 N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\nM\u1EDF m\u00E0n: Ti\u1EBFt m\u1EE5c h\u1EE3p ca \u201CG\u1EEDi Em N\u1EE5 C\u01B0\u1EDDi M\u00F9a \u0110\u00F4ng\u201D \u2013 tr\u00ECnh di\u1EC5n b\u1EDFi d\u00E0n h\u1EE3p x\u01B0\u1EDBng thi\u1EBFu nhi H\u00F2a B\u00ECnh.\n\nPh\u1EA7n 1 \u2013 Nh\u1EEFng Gi\u1EA5c M\u01A1 B\u00E9 Nh\u1ECF:\nTr\u00ECnh di\u1EC5n acoustic v\u1EDBi c\u00E1c ca s\u0129 tr\u1EBB nh\u01B0 Th\u1ECBnh Suy, V\u0169 C\u00E1t T\u01B0\u1EDDng, Orange.\nCh\u1EE7 \u0111\u1EC1: \u01AF\u1EDBc m\u01A1, tu\u1ED5i th\u01A1, v\u00E0 ni\u1EC1m tin.\n\nPh\u1EA7n 2 \u2013 M\u01A1 Thay Em:\nS\u00E2n kh\u1EA5u ch\u00EDnh, tr\u00ECnh di\u1EC5n live band c\u00F9ng c\u00E1c ngh\u1EC7 s\u0129 kh\u00E1ch m\u1EDDi H\u00E0 Anh Tu\u1EA5n, M\u1EF9 T\u00E2m, \u0110en V\u00E2u.\nG\u00E2y qu\u1EF9 tr\u1EF1c ti\u1EBFp b\u1EB1ng h\u00ECnh th\u1EE9c auction (\u0111\u1EA5u gi\u00E1 v\u1EADt ph\u1EA9m ngh\u1EC7 s\u0129 t\u1EB7ng).\n\nPh\u1EA7n 3 \u2013 Trao Y\u00EAu Th\u01B0\u01A1ng:\nC\u00F4ng b\u1ED1 s\u1ED1 ti\u1EC1n quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c.\nTr\u00ECnh chi\u1EBFu video h\u00E0nh tr\u00ECnh trao qu\u00E0 v\u00E0 x\u00E2y l\u1EDBp h\u1ECDc v\u00F9ng cao.\n\n\u2764\uFE0F Th\u00F4ng \u0111i\u1EC7p ch\u01B0\u01A1ng tr\u00ECnh\n\n\u201CC\u00F3 nh\u1EEFng gi\u1EA5c m\u01A1 tr\u1EBB em kh\u00F4ng th\u1EC3 n\u00F3i ra,\nnh\u01B0ng ch\u00FAng ta c\u00F3 th\u1EC3 h\u00E1t thay, m\u01A1 thay em \u2014\n\u0111\u1EC3 m\u00F9a \u0111\u00F4ng n\u00E0y kh\u00F4ng c\u00F2n l\u1EA1nh tr\u00EAn \u0111\u00F4i ch\u00E2n b\u00E9 nh\u1ECF.\u201D\n\n\uD83E\uDDFA \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c:\n\nBan T\u1ED5 Ch\u1EE9c: Nh\u00F3m thi\u1EC7n nguy\u1EC7n H\u00E0nh Tr\u00ECnh Xanh ph\u1ED1i h\u1EE3p c\u00F9ng \u0110\u00E0i VOH v\u00E0 Qu\u1EF9 Tr\u1EBB Em Vi\u1EC7t\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: HTV, VTV6, B\u00E1o Tu\u1ED5i Tr\u1EBB, Zing News\n\u0110\u1ED1i t\u00E1c t\u00E0i tr\u1EE3: Vinamilk, PNJ, Yamaha Music Vi\u1EC7t Nam\n\uD83D\uDCDE Li\u00EAn h\u1EC7 tham gia / t\u00E0i tr\u1EE3\n\uD83D\uDCE7 Email: mothayem.charity@gmail.com\n\u260E\uFE0F Hotline: 0909 123 456\n\uD83C\uDF10 Website: www.mothayem.vn","EventDetails":"\u201CM\u01A1 Thay Em\u201D l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c thi\u1EC7n nguy\u1EC7n \u0111\u1EA7y c\u1EA3m x\u00FAc, n\u01A1i \u00E2m nh\u1EA1c tr\u1EDF th\u00E0nh c\u1EA7u n\u1ED1i gi\u1EEFa tr\u00E1i tim ngh\u1EC7 s\u0129 v\u00E0 nh\u1EEFng \u01B0\u1EDBc m\u01A1 nh\u1ECF b\u00E9 n\u01A1i v\u00F9ng cao xa x\u00F4i.\nCh\u01B0\u01A1ng tr\u00ECnh h\u01B0\u1EDBng t\u1EDBi g\u00E2y qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D, h\u1ED7 tr\u1EE3 mua s\u00E1ch v\u1EDF, qu\u1EA7n \u00E1o m\u00F9a \u0111\u00F4ng v\u00E0 h\u1ECDc c\u1EE5 cho tr\u1EBB em t\u1EA1i c\u00E1c \u0111i\u1EC3m tr\u01B0\u1EDDng v\u00F9ng n\u00FAi ph\u00EDa B\u1EAFc Vi\u1EC7t Nam.\n\n\uD83D\uDCAB Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 15 th\u00E1ng 12 n\u0103m 2025\n\uD83D\uDD58 Gi\u1EDD: 19h00 \u2013 22h30\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t VOH \u2013 \u0110\u00E0i Ti\u1EBFng n\u00F3i Nh\u00E2n d\u00E2n TP.HCM, s\u1ED1 37 Nguy\u1EC5n B\u1EC9nh Khi\u00EAm, Qu\u1EADn 1\n\uD83C\uDF9F V\u00E9 m\u1EDDi: To\u00E0n b\u1ED9 ti\u1EC1n v\u00E9 v\u00E0 quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c tr\u00EDch 100% v\u00E0o qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D\n\uD83C\uDFA4 N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\nM\u1EDF m\u00E0n: Ti\u1EBFt m\u1EE5c h\u1EE3p ca \u201CG\u1EEDi Em N\u1EE5 C\u01B0\u1EDDi M\u00F9a \u0110\u00F4ng\u201D \u2013 tr\u00ECnh di\u1EC5n b\u1EDFi d\u00E0n h\u1EE3p x\u01B0\u1EDBng thi\u1EBFu nhi H\u00F2a B\u00ECnh.\n\nPh\u1EA7n 1 \u2013 Nh\u1EEFng Gi\u1EA5c M\u01A1 B\u00E9 Nh\u1ECF:\nTr\u00ECnh di\u1EC5n acoustic v\u1EDBi c\u00E1c ca s\u0129 tr\u1EBB nh\u01B0 Th\u1ECBnh Suy, V\u0169 C\u00E1t T\u01B0\u1EDDng, Orange.\nCh\u1EE7 \u0111\u1EC1: \u01AF\u1EDBc m\u01A1, tu\u1ED5i th\u01A1, v\u00E0 ni\u1EC1m tin.\n\nPh\u1EA7n 2 \u2013 M\u01A1 Thay Em:\nS\u00E2n kh\u1EA5u ch\u00EDnh, tr\u00ECnh di\u1EC5n live band c\u00F9ng c\u00E1c ngh\u1EC7 s\u0129 kh\u00E1ch m\u1EDDi H\u00E0 Anh Tu\u1EA5n, M\u1EF9 T\u00E2m, \u0110en V\u00E2u.\nG\u00E2y qu\u1EF9 tr\u1EF1c ti\u1EBFp b\u1EB1ng h\u00ECnh th\u1EE9c auction (\u0111\u1EA5u gi\u00E1 v\u1EADt ph\u1EA9m ngh\u1EC7 s\u0129 t\u1EB7ng).\n\nPh\u1EA7n 3 \u2013 Trao Y\u00EAu Th\u01B0\u01A1ng:\nC\u00F4ng b\u1ED1 s\u1ED1 ti\u1EC1n quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c.\nTr\u00ECnh chi\u1EBFu video h\u00E0nh tr\u00ECnh trao qu\u00E0 v\u00E0 x\u00E2y l\u1EDBp h\u1ECDc v\u00F9ng cao.\n\n\u2764\uFE0F Th\u00F4ng \u0111i\u1EC7p ch\u01B0\u01A1ng tr\u00ECnh\n\n\u201CC\u00F3 nh\u1EEFng gi\u1EA5c m\u01A1 tr\u1EBB em kh\u00F4ng th\u1EC3 n\u00F3i ra,\nnh\u01B0ng ch\u00FAng ta c\u00F3 th\u1EC3 h\u00E1t thay, m\u01A1 thay em \u2014\n\u0111\u1EC3 m\u00F9a \u0111\u00F4ng n\u00E0y kh\u00F4ng c\u00F2n l\u1EA1nh tr\u00EAn \u0111\u00F4i ch\u00E2n b\u00E9 nh\u1ECF.\u201D\n\n\uD83E\uDDFA \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c:\n\nBan T\u1ED5 Ch\u1EE9c: Nh\u00F3m thi\u1EC7n nguy\u1EC7n H\u00E0nh Tr\u00ECnh Xanh ph\u1ED1i h\u1EE3p c\u00F9ng \u0110\u00E0i VOH v\u00E0 Qu\u1EF9 Tr\u1EBB Em Vi\u1EC7t\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: HTV, VTV6, B\u00E1o Tu\u1ED5i Tr\u1EBB, Zing News\n\u0110\u1ED1i t\u00E1c t\u00E0i tr\u1EE3: Vinamilk, PNJ, Yamaha Music Vi\u1EC7t Nam\n\uD83D\uDCDE Li\u00EAn h\u1EC7 tham gia / t\u00E0i tr\u1EE3\n\uD83D\uDCE7 Email: mothayem.charity@gmail.com\n\u260E\uFE0F Hotline: 0909 123 456\n\uD83C\uDF10 Website: www.mothayem.vn","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/2773df59-6399-41d1-9179-a75d779607ec.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"Club tr\u1EF1c thu\u1ED9c tr\u01B0\u1EDDng \u0111\u1EA1i h\u1ECDc FPT"}',
    GETUTCDATE(),
    GETUTCDATE(),
    N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761492494537","Name":"S\u00E2n Kh\u1EA5u","Shape":"rectangle","Coordinates":[{"X":201,"Y":145.1125030517578},{"X":849,"Y":145.1125030517578},{"X":849,"Y":259.1125030517578},{"X":201,"Y":259.1125030517578}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761492513829","Name":"V\u00E9 VIP","Shape":"rectangle","Coordinates":[{"X":179,"Y":305.1124954223633},{"X":888,"Y":305.1124954223633},{"X":888,"Y":360.1124954223633},{"X":179,"Y":360.1124954223633}],"Color":"#764ba2","TicketTypeId":1,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761492527322","Name":"V\u00E9 Th\u01B0\u1EDDng","Shape":"rectangle","Coordinates":[{"X":232,"Y":399.1125030517578},{"X":826,"Y":399.1125030517578},{"X":826,"Y":642.1125030517578},{"X":232,"Y":642.1125030517578}],"Color":"#f093fb","TicketTypeId":0,"IsStanding":false,"Capacity":null,"Label":""}]}'
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Draft',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    NULL
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Hạ Sau Hè – Tân Lý Live Concert',
    N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
Lấy cảm hứng từ những ký ức thanh xuân, mối tình cũ và những ngày hè chưa kịp nói lời tạm biệt, “Hạ Sau Hè” hứa hẹn sẽ là hành trình âm nhạc chan chứa cảm xúc – nơi người nghe được sống lại trong từng nhịp giai điệu của tuổi trẻ.

🌅 Thời gian & Địa điểm
🗓 Ngày: 10 tháng 8 năm 2025
🕖 Giờ: 19h30 – 22h00
📍 Địa điểm: Nhà hát Hòa Bình, Quận 10, TP.HCM

🎟 Giá vé:

Hạng VVIP: 1.500.000đ (Gặp gỡ, ký tặng, chụp ảnh cùng ca sĩ)
Hạng VIP: 900.000đ
Hạng Thường: 500.000đ

🌻 Nội dung chương trình

Phần mở màn – Thanh âm mùa nắng:
Ca khúc: “Ngày Em Đi Qua”, “Hạ Gọi Tên”, “Nắng Trên Mái Trường”
Phong cách: Pop – Folk pha chút Retro, tái hiện không khí học trò.

Phần 2 – Hạ Sau Hè (Main Stage):
Các bản hit gắn liền với tên tuổi Tân Lý: “Đoạn Kết Mùa Xanh”, “Người Ở Lại Sau Hè”, “Cánh Diều Năm Ấy”
Sân khấu ánh sáng LED mô phỏng cơn mưa mùa hạ – điểm nhấn cảm xúc của đêm nhạc.

Phần 3 – Hẹn Gặp Lại, Tháng Tám:
Tân Lý song ca cùng khách mời đặc biệt Lê Cát Trọng Lý và Thái Đinh.
Kết thúc bằng ca khúc mới “Những Ngày Nắng Còn Lại” – ra mắt độc quyền trong concert.

💌 Thông điệp âm nhạc

“Mùa hạ không bao giờ thật sự qua đi,
chỉ là ta không còn đứng dưới nắng năm ấy nữa.”
“Hạ Sau Hè” không chỉ là một đêm nhạc – mà là lời tri ân của Tân Lý dành cho tuổi trẻ, cho những người đã yêu, đã đi qua, và vẫn mỉm cười khi nhìn lại.

🎧 Đội ngũ sản xuất

Ca sĩ chính: Tân Lý
Giám đốc âm nhạc: Long Halo
Đạo diễn sân khấu: Huỳnh Tuấn Kiệt
Ban nhạc: The July Notes Band
Khách mời: Lê Cát Trọng Lý, Thái Đinh, Nguyên Hà

💫 Đơn vị tổ chức & tài trợ

Tổ chức: Lý Sound Entertainment
Bảo trợ truyền thông: Billboard Việt Nam, Yan News, TikTok Music VN
Đồng hành: Shopee Music, Yamaha, Highlands Coffee

📱 Liên hệ & đặt vé
🌐 Website: www.hasauhe.vn
📧 Email: concert.hasauhe@gmail.com
☎️ Hotline: 0906 612 123

Payment Method: bank_transfer
Bank Account: [{"bankName":"MB Bank","accountNumber":"04358345653","accountHolder":"Khanh Ngu da","isDefault":true}]
Tax Info: ',
    '2025-11-12 09:30:00.000',
    '2025-11-12 23:35:00.000',
    N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh',
    N'Public',
    'Offline',
    'Music',
    'Open',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/uploads/events/feb6c575-f0ed-49b6-8ba4-f747e3db6216.png","BackgroundImage":"/uploads/events/1f518ef5-75fd-4d39-b721-f703509a96b9.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}',
    GETUTCDATE(),
    GETUTCDATE(),
    N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761493198019","Name":"Stage","Shape":"rectangle","Coordinates":[{"X":278,"Y":69.11250305175781},{"X":746,"Y":69.11250305175781},{"X":746,"Y":325.1125030517578},{"X":278,"Y":325.1125030517578}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761493215543","Name":"H\u1EA1ng VVIP","Shape":"rectangle","Coordinates":[{"X":346,"Y":349.1124954223633},{"X":693,"Y":349.1124954223633},{"X":693,"Y":413.1124954223633},{"X":346,"Y":413.1124954223633}],"Color":"#764ba2","TicketTypeId":1,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761494556787","Name":"H\u1EA1ng Vip","Shape":"rectangle","Coordinates":[{"X":402.4000015258789,"Y":465.4125061035156},{"X":580.4000015258789,"Y":465.4125061035156},{"X":580.4000015258789,"Y":515.4125061035156},{"X":402.4000015258789,"Y":515.4125061035156}],"Color":"#f093fb","TicketTypeId":2,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761494583688","Name":"H\u1EA1ng Th\u01B0\u1EDDng","Shape":"rectangle","Coordinates":[{"X":225,"Y":507.1125030517578},{"X":786,"Y":507.1125030517578},{"X":786,"Y":678.1125030517578},{"X":225,"Y":678.1125030517578}],"Color":"#fee140","TicketTypeId":3,"IsStanding":false,"Capacity":null,"Label":""}]}'
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'Rhythm: Xưởng Làm Nhạc – Casting II',
    N'Sau thành công vang dội của mùa đầu tiên, CLB Rhythm – Xưởng Làm Nhạc chính thức trở lại với Casting II: Tìm kiếm thế hệ nghệ sĩ – producer mới năm 2025.
Đây là nơi những người trẻ yêu âm nhạc có thể:

🔹 Trải nghiệm môi trường sáng tạo chuyên nghiệp
🔹 Học hỏi từ các mentor hàng đầu trong lĩnh vực sản xuất âm nhạc
🔹 Kết nối cộng đồng nghệ sĩ indie và underground Việt

🗓 Thời gian & Địa điểm
📅 Ngày Casting: 16 & 17 tháng 11 năm 2025
🕙 Giờ: 9h00 – 17h00
📍 Địa điểm: Phòng Studio A1, Tầng 5 – Trường Đại học Văn Lang, Cơ sở Bình Thạnh
🧾 Hình thức đăng ký: Online form qua website chính thức hoặc trực tiếp tại bàn đăng ký của CLB
🎶 Đối tượng tham gia
Sinh viên đam mê âm nhạc, đặc biệt trong các lĩnh vực:

🎧 Producer (làm beat, phối nhạc)
🎤 Vocalist / Rapper / Singer-Songwriter
🎹 Composer / Arranger
🎸 Musician (guitar, piano, violin, drum...)
🎬 Media & Visual Art (quay dựng, thiết kế poster, MV, social content)
🎵 Các vòng tuyển chọn

Vòng 1 – Gửi Portfolio / Demo:
Ứng viên gửi 1–2 sản phẩm âm nhạc tự thực hiện (hoặc cover phối mới).
→ 50 bạn được chọn vào vòng 2.

Vòng 2 – Trải nghiệm Studio:

Ứng viên sẽ được chia nhóm tạo beat hoặc biểu diễn trực tiếp.
Đánh giá dựa trên: Creativity – Teamwork – Stage Feel – Originality

Vòng 3 – Phỏng vấn cá nhân:

Trò chuyện cùng Ban Chủ Nhiệm và Mentor về định hướng âm nhạc.
20 thành viên chính thức được chọn tham gia nhiệm kỳ 2025–2026.

🧑‍🏫 Ban giám khảo & mentor

Tân Lý – Ca sĩ / Singer-Songwriter (khách mời đặc biệt)
Rin Beat – Producer (chuyên dòng Lo-fi / R&B)
Hà My – Vocal Coach, Trưởng nhóm kỹ thuật thanh nhạc Rhythm
Đình Phong – Chủ nhiệm CLB Rhythm

🎁 Quyền lợi thành viên được chọn

Được đào tạo kỹ năng sáng tác – hòa âm – biểu diễn – thu âm miễn phí.
Tham gia sản xuất mini album nội bộ “Rhythm 2025: Youth Frequency”.
Có cơ hội trình diễn tại các sự kiện âm nhạc trong và ngoài trường.
Nhận chứng nhận tham gia & hỗ trợ portfolio nghệ sĩ cá nhân.

Payment Method: bank_transfer
Bank Account: [{"bankName":"MB Bank","accountNumber":"04358345653","accountHolder":"Khanh Ngu da","isDefault":true}]
Tax Info: ',
    '2025-11-17 17:00:00.000',
    '2025-11-18 22:35:00.000',
    N'',
    N'Public',
    'Offline',
    'Education',
    'Open',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Thi\u1EC7n Tr\u1ECB","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng Ph\u01B0\u1EDBc Ninh","EventImage":"/uploads/events/8a55141c-aea4-4a0e-9b4b-8367cbc0625b.jpg","BackgroundImage":"/uploads/events/1ce4f25c-0742-45e7-b1f6-d8a95dca15f2.jpg","EventIntroduction":"Sau th\u00E0nh c\u00F4ng vang d\u1ED9i c\u1EE7a m\u00F9a \u0111\u1EA7u ti\u00EAn, CLB Rhythm \u2013 X\u01B0\u1EDFng L\u00E0m Nh\u1EA1c ch\u00EDnh th\u1EE9c tr\u1EDF l\u1EA1i v\u1EDBi Casting II: T\u00ECm ki\u1EBFm th\u1EBF h\u1EC7 ngh\u1EC7 s\u0129 \u2013 producer m\u1EDBi n\u0103m 2025.\n\u0110\u00E2y l\u00E0 n\u01A1i nh\u1EEFng ng\u01B0\u1EDDi tr\u1EBB y\u00EAu \u00E2m nh\u1EA1c c\u00F3 th\u1EC3:\n\n\uD83D\uDD39 Tr\u1EA3i nghi\u1EC7m m\u00F4i tr\u01B0\u1EDDng s\u00E1ng t\u1EA1o chuy\u00EAn nghi\u1EC7p\n\uD83D\uDD39 H\u1ECDc h\u1ECFi t\u1EEB c\u00E1c mentor h\u00E0ng \u0111\u1EA7u trong l\u0129nh v\u1EF1c s\u1EA3n xu\u1EA5t \u00E2m nh\u1EA1c\n\uD83D\uDD39 K\u1EBFt n\u1ED1i c\u1ED9ng \u0111\u1ED3ng ngh\u1EC7 s\u0129 indie v\u00E0 underground Vi\u1EC7t\n\n\uD83D\uDDD3 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDCC5 Ng\u00E0y Casting: 16 \u0026 17 th\u00E1ng 11 n\u0103m 2025\n\uD83D\uDD59 Gi\u1EDD: 9h00 \u2013 17h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Ph\u00F2ng Studio A1, T\u1EA7ng 5 \u2013 Tr\u01B0\u1EDDng \u0110\u1EA1i h\u1ECDc V\u0103n Lang, C\u01A1 s\u1EDF B\u00ECnh Th\u1EA1nh\n\uD83E\uDDFE H\u00ECnh th\u1EE9c \u0111\u0103ng k\u00FD: Online form qua website ch\u00EDnh th\u1EE9c ho\u1EB7c tr\u1EF1c ti\u1EBFp t\u1EA1i b\u00E0n \u0111\u0103ng k\u00FD c\u1EE7a CLB\n\uD83C\uDFB6 \u0110\u1ED1i t\u01B0\u1EE3ng tham gia\nSinh vi\u00EAn \u0111am m\u00EA \u00E2m nh\u1EA1c, \u0111\u1EB7c bi\u1EC7t trong c\u00E1c l\u0129nh v\u1EF1c:\n\n\uD83C\uDFA7 Producer (l\u00E0m beat, ph\u1ED1i nh\u1EA1c)\n\uD83C\uDFA4 Vocalist / Rapper / Singer-Songwriter\n\uD83C\uDFB9 Composer / Arranger\n\uD83C\uDFB8 Musician (guitar, piano, violin, drum...)\n\uD83C\uDFAC Media \u0026 Visual Art (quay d\u1EF1ng, thi\u1EBFt k\u1EBF poster, MV, social content)\n\uD83C\uDFB5 C\u00E1c v\u00F2ng tuy\u1EC3n ch\u1ECDn\n\nV\u00F2ng 1 \u2013 G\u1EEDi Portfolio / Demo:\n\u1EE8ng vi\u00EAn g\u1EEDi 1\u20132 s\u1EA3n ph\u1EA9m \u00E2m nh\u1EA1c t\u1EF1 th\u1EF1c hi\u1EC7n (ho\u1EB7c cover ph\u1ED1i m\u1EDBi).\n\u2192 50 b\u1EA1n \u0111\u01B0\u1EE3c ch\u1ECDn v\u00E0o v\u00F2ng 2.\n\nV\u00F2ng 2 \u2013 Tr\u1EA3i nghi\u1EC7m Studio:\n\n\u1EE8ng vi\u00EAn s\u1EBD \u0111\u01B0\u1EE3c chia nh\u00F3m t\u1EA1o beat ho\u1EB7c bi\u1EC3u di\u1EC5n tr\u1EF1c ti\u1EBFp.\n\u0110\u00E1nh gi\u00E1 d\u1EF1a tr\u00EAn: Creativity \u2013 Teamwork \u2013 Stage Feel \u2013 Originality\n\nV\u00F2ng 3 \u2013 Ph\u1ECFng v\u1EA5n c\u00E1 nh\u00E2n:\n\nTr\u00F2 chuy\u1EC7n c\u00F9ng Ban Ch\u1EE7 Nhi\u1EC7m v\u00E0 Mentor v\u1EC1 \u0111\u1ECBnh h\u01B0\u1EDBng \u00E2m nh\u1EA1c.\n20 th\u00E0nh vi\u00EAn ch\u00EDnh th\u1EE9c \u0111\u01B0\u1EE3c ch\u1ECDn tham gia nhi\u1EC7m k\u1EF3 2025\u20132026.\n\n\uD83E\uDDD1\u200D\uD83C\uDFEB Ban gi\u00E1m kh\u1EA3o \u0026 mentor\n\nT\u00E2n L\u00FD \u2013 Ca s\u0129 / Singer-Songwriter (kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t)\nRin Beat \u2013 Producer (chuy\u00EAn d\u00F2ng Lo-fi / R\u0026B)\nH\u00E0 My \u2013 Vocal Coach, Tr\u01B0\u1EDFng nh\u00F3m k\u1EF9 thu\u1EADt thanh nh\u1EA1c Rhythm\n\u0110\u00ECnh Phong \u2013 Ch\u1EE7 nhi\u1EC7m CLB Rhythm\n\n\uD83C\uDF81 Quy\u1EC1n l\u1EE3i th\u00E0nh vi\u00EAn \u0111\u01B0\u1EE3c ch\u1ECDn\n\n\u0110\u01B0\u1EE3c \u0111\u00E0o t\u1EA1o k\u1EF9 n\u0103ng s\u00E1ng t\u00E1c \u2013 h\u00F2a \u00E2m \u2013 bi\u1EC3u di\u1EC5n \u2013 thu \u00E2m mi\u1EC5n ph\u00ED.\nTham gia s\u1EA3n xu\u1EA5t mini album n\u1ED9i b\u1ED9 \u201CRhythm 2025: Youth Frequency\u201D.\nC\u00F3 c\u01A1 h\u1ED9i tr\u00ECnh di\u1EC5n t\u1EA1i c\u00E1c s\u1EF1 ki\u1EC7n \u00E2m nh\u1EA1c trong v\u00E0 ngo\u00E0i tr\u01B0\u1EDDng.\nNh\u1EADn ch\u1EE9ng nh\u1EADn tham gia \u0026 h\u1ED7 tr\u1EE3 portfolio ngh\u1EC7 s\u0129 c\u00E1 nh\u00E2n.","EventDetails":"Sau th\u00E0nh c\u00F4ng vang d\u1ED9i c\u1EE7a m\u00F9a \u0111\u1EA7u ti\u00EAn, CLB Rhythm \u2013 X\u01B0\u1EDFng L\u00E0m Nh\u1EA1c ch\u00EDnh th\u1EE9c tr\u1EDF l\u1EA1i v\u1EDBi Casting II: T\u00ECm ki\u1EBFm th\u1EBF h\u1EC7 ngh\u1EC7 s\u0129 \u2013 producer m\u1EDBi n\u0103m 2025.\n\u0110\u00E2y l\u00E0 n\u01A1i nh\u1EEFng ng\u01B0\u1EDDi tr\u1EBB y\u00EAu \u00E2m nh\u1EA1c c\u00F3 th\u1EC3:\n\n\uD83D\uDD39 Tr\u1EA3i nghi\u1EC7m m\u00F4i tr\u01B0\u1EDDng s\u00E1ng t\u1EA1o chuy\u00EAn nghi\u1EC7p\n\uD83D\uDD39 H\u1ECDc h\u1ECFi t\u1EEB c\u00E1c mentor h\u00E0ng \u0111\u1EA7u trong l\u0129nh v\u1EF1c s\u1EA3n xu\u1EA5t \u00E2m nh\u1EA1c\n\uD83D\uDD39 K\u1EBFt n\u1ED1i c\u1ED9ng \u0111\u1ED3ng ngh\u1EC7 s\u0129 indie v\u00E0 underground Vi\u1EC7t\n\n\uD83D\uDDD3 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDCC5 Ng\u00E0y Casting: 16 \u0026 17 th\u00E1ng 11 n\u0103m 2025\n\uD83D\uDD59 Gi\u1EDD: 9h00 \u2013 17h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Ph\u00F2ng Studio A1, T\u1EA7ng 5 \u2013 Tr\u01B0\u1EDDng \u0110\u1EA1i h\u1ECDc V\u0103n Lang, C\u01A1 s\u1EDF B\u00ECnh Th\u1EA1nh\n\uD83E\uDDFE H\u00ECnh th\u1EE9c \u0111\u0103ng k\u00FD: Online form qua website ch\u00EDnh th\u1EE9c ho\u1EB7c tr\u1EF1c ti\u1EBFp t\u1EA1i b\u00E0n \u0111\u0103ng k\u00FD c\u1EE7a CLB\n\uD83C\uDFB6 \u0110\u1ED1i t\u01B0\u1EE3ng tham gia\nSinh vi\u00EAn \u0111am m\u00EA \u00E2m nh\u1EA1c, \u0111\u1EB7c bi\u1EC7t trong c\u00E1c l\u0129nh v\u1EF1c:\n\n\uD83C\uDFA7 Producer (l\u00E0m beat, ph\u1ED1i nh\u1EA1c)\n\uD83C\uDFA4 Vocalist / Rapper / Singer-Songwriter\n\uD83C\uDFB9 Composer / Arranger\n\uD83C\uDFB8 Musician (guitar, piano, violin, drum...)\n\uD83C\uDFAC Media \u0026 Visual Art (quay d\u1EF1ng, thi\u1EBFt k\u1EBF poster, MV, social content)\n\uD83C\uDFB5 C\u00E1c v\u00F2ng tuy\u1EC3n ch\u1ECDn\n\nV\u00F2ng 1 \u2013 G\u1EEDi Portfolio / Demo:\n\u1EE8ng vi\u00EAn g\u1EEDi 1\u20132 s\u1EA3n ph\u1EA9m \u00E2m nh\u1EA1c t\u1EF1 th\u1EF1c hi\u1EC7n (ho\u1EB7c cover ph\u1ED1i m\u1EDBi).\n\u2192 50 b\u1EA1n \u0111\u01B0\u1EE3c ch\u1ECDn v\u00E0o v\u00F2ng 2.\n\nV\u00F2ng 2 \u2013 Tr\u1EA3i nghi\u1EC7m Studio:\n\n\u1EE8ng vi\u00EAn s\u1EBD \u0111\u01B0\u1EE3c chia nh\u00F3m t\u1EA1o beat ho\u1EB7c bi\u1EC3u di\u1EC5n tr\u1EF1c ti\u1EBFp.\n\u0110\u00E1nh gi\u00E1 d\u1EF1a tr\u00EAn: Creativity \u2013 Teamwork \u2013 Stage Feel \u2013 Originality\n\nV\u00F2ng 3 \u2013 Ph\u1ECFng v\u1EA5n c\u00E1 nh\u00E2n:\n\nTr\u00F2 chuy\u1EC7n c\u00F9ng Ban Ch\u1EE7 Nhi\u1EC7m v\u00E0 Mentor v\u1EC1 \u0111\u1ECBnh h\u01B0\u1EDBng \u00E2m nh\u1EA1c.\n20 th\u00E0nh vi\u00EAn ch\u00EDnh th\u1EE9c \u0111\u01B0\u1EE3c ch\u1ECDn tham gia nhi\u1EC7m k\u1EF3 2025\u20132026.\n\n\uD83E\uDDD1\u200D\uD83C\uDFEB Ban gi\u00E1m kh\u1EA3o \u0026 mentor\n\nT\u00E2n L\u00FD \u2013 Ca s\u0129 / Singer-Songwriter (kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t)\nRin Beat \u2013 Producer (chuy\u00EAn d\u00F2ng Lo-fi / R\u0026B)\nH\u00E0 My \u2013 Vocal Coach, Tr\u01B0\u1EDFng nh\u00F3m k\u1EF9 thu\u1EADt thanh nh\u1EA1c Rhythm\n\u0110\u00ECnh Phong \u2013 Ch\u1EE7 nhi\u1EC7m CLB Rhythm\n\n\uD83C\uDF81 Quy\u1EC1n l\u1EE3i th\u00E0nh vi\u00EAn \u0111\u01B0\u1EE3c ch\u1ECDn\n\n\u0110\u01B0\u1EE3c \u0111\u00E0o t\u1EA1o k\u1EF9 n\u0103ng s\u00E1ng t\u00E1c \u2013 h\u00F2a \u00E2m \u2013 bi\u1EC3u di\u1EC5n \u2013 thu \u00E2m mi\u1EC5n ph\u00ED.\nTham gia s\u1EA3n xu\u1EA5t mini album n\u1ED9i b\u1ED9 \u201CRhythm 2025: Youth Frequency\u201D.\nC\u00F3 c\u01A1 h\u1ED9i tr\u00ECnh di\u1EC5n t\u1EA1i c\u00E1c s\u1EF1 ki\u1EC7n \u00E2m nh\u1EA1c trong v\u00E0 ngo\u00E0i tr\u01B0\u1EDDng.\nNh\u1EADn ch\u1EE9ng nh\u1EADn tham gia \u0026 h\u1ED7 tr\u1EE3 portfolio ngh\u1EC7 s\u0129 c\u00E1 nh\u00E2n.","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/5c16f17e-02c8-4061-948c-c5defd1c8c61.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng L\u00E0m Nh\u1EA1c","OrganizerInfo":"clb tr\u1EF1c thu\u1ED9c tr\u01B0\u1EDDng \u0111\u1EA1i h\u1ECDc fpt \u0111\u00E0 n\u1EB5ng"}',
    GETUTCDATE(),
    GETUTCDATE(),
    N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761497340150","Name":"Khu v\u1EF1c 1","Shape":"rectangle","Coordinates":[{"X":216,"Y":135.3125},{"X":790,"Y":135.3125},{"X":790,"Y":319.3125},{"X":216,"Y":319.3125}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761497353125","Name":"Khu v\u1EF1c 2","Shape":"rectangle","Coordinates":[{"X":168,"Y":394.1124954223633},{"X":481,"Y":394.1124954223633},{"X":481,"Y":607.1124954223633},{"X":168,"Y":607.1124954223633}],"Color":"#764ba2","TicketTypeId":1,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761497356703","Name":"Khu v\u1EF1c 3","Shape":"rectangle","Coordinates":[{"X":551,"Y":393.1125030517578},{"X":888,"Y":393.1125030517578},{"X":888,"Y":600.1125030517578},{"X":551,"Y":600.1125030517578}],"Color":"#f093fb","TicketTypeId":2,"IsStanding":false,"Capacity":null,"Label":""}]}'
);

INSERT INTO Event (HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, CreatedAt, UpdatedAt, VenueLayout)
VALUES (
    1,
    N'khanh loz',
    N'fy6guhi9ojigrjojio9fji8ji8fjkirjkjrrjojrjijijikmjik

Payment Method: bank_transfer
Bank Account: [{"bankName":"MB Bank","accountNumber":"04358345653","accountHolder":"Khanh Ngu da","isDefault":true}]
Tax Info: ',
    '2025-10-27 17:00:00.000',
    '2025-10-29 17:00:00.000',
    N'',
    N'Public',
    'Offline',
    'Art',
    'Open',
    N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"hgyhujhi8h8ih","StreetAddress":"huhi8hi8","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng H\u1EA3i Ch\u00E2u II","EventImage":"/uploads/events/577d0814-9ae0-4c0d-a437-bb6aecd9c392.jpg","BackgroundImage":"/uploads/events/564e358f-6bbe-4390-9b74-44cd5d5d9a91.jpg","EventIntroduction":"fy6guhi9ojigrjojio9fji8ji8fjkirjkjrrjojrjijijikmjik","EventDetails":"fy6guhi9ojigrjojio9fji8ji8fjkirjkjrrjojrjijijikmjik","specialGuestsList":"","SpecialExperience":""}',
    N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}',
    N'{"OrganizerLogo":"/uploads/events/3e59bb3b-065f-47f6-87b4-f0a1fda484f4.jpg","OrganizerName":"llos","OrganizerInfo":"jkdfgtvyhbv"}',
    GETUTCDATE(),
    GETUTCDATE(),
    N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761528851280","Name":"saan khau","Shape":"rectangle","Coordinates":[{"X":258,"Y":165.3125},{"X":736,"Y":165.3125},{"X":736,"Y":238.3125},{"X":258,"Y":238.3125}],"Color":"#fee140","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761528872430","Name":"Khu v\u1EF1c 2","Shape":"rectangle","Coordinates":[{"X":188,"Y":286.11249923706055},{"X":481,"Y":286.11249923706055},{"X":481,"Y":564.1124992370605},{"X":188,"Y":564.1124992370605}],"Color":"#764ba2","TicketTypeId":1,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761528878269","Name":"Khu v\u1EF1c 3","Shape":"rectangle","Coordinates":[{"X":566,"Y":296.1124954223633},{"X":855,"Y":296.1124954223633},{"X":855,"Y":556.1124954223633},{"X":566,"Y":556.1124954223633}],"Color":"#f093fb","TicketTypeId":2,"IsStanding":false,"Capacity":null,"Label":""}]}'
);

-- Verify Events were created successfully
PRINT 'Events created:';
SELECT EventId, HostId, Title, Category, Status FROM Event ORDER BY EventId;

-- ========================================
-- INSERT TICKET TYPES
-- ========================================

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    1,
    N'Vé Thường',
    150000.00,
    50,
    1,
    5,
    '2025-09-26 14:30:08.000',
    '2025-11-01 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    1,
    N'Vé VIP',
    250000.00,
    20,
    1,
    3,
    '2025-09-26 14:30:08.000',
    '2025-11-01 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    1,
    N'Vé Sinh Viên',
    100000.00,
    30,
    1,
    2,
    '2025-09-26 14:30:08.000',
    '2025-11-01 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    2,
    N'Vé Thường',
    200000.00,
    100,
    1,
    10,
    '2025-09-26 14:30:08.000',
    '2025-11-08 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    2,
    N'Vé Premium',
    350000.00,
    50,
    1,
    5,
    '2025-09-26 14:30:08.000',
    '2025-11-08 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    3,
    N'Vé Thường',
    300000.00,
    80,
    1,
    8,
    '2025-09-26 14:30:08.000',
    '2025-11-15 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    3,
    N'Vé VIP',
    500000.00,
    20,
    1,
    4,
    '2025-09-26 14:30:08.000',
    '2025-11-15 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    4,
    N'Vé Thường',
    180000.00,
    150,
    1,
    6,
    '2025-09-26 14:30:08.000',
    '2025-11-04 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    4,
    N'Vé VIP',
    320000.00,
    50,
    1,
    4,
    '2025-09-26 14:30:08.000',
    '2025-11-04 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    5,
    N'Vé Thường',
    120000.00,
    200,
    1,
    10,
    '2025-09-26 14:30:08.000',
    '2025-11-12 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    6,
    N'Vé Thường',
    280000.00,
    40,
    1,
    4,
    '2025-09-26 14:30:08.000',
    '2025-11-19 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    6,
    N'Vé Cặp Đôi',
    500000.00,
    20,
    1,
    2,
    '2025-09-26 14:30:08.000',
    '2025-11-19 14:30:08.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    7,
    N'Hạng Thường:',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 14:58:22.000',
    '2025-11-25 14:58:22.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    7,
    N'Hạng VIP:',
    900000.00,
    50,
    1,
    10,
    '2025-10-26 14:58:49.000',
    '2025-11-25 14:58:49.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    7,
    N'Hạng VVIP',
    1500000.00,
    20,
    1,
    5,
    '2025-10-26 14:59:13.000',
    '2025-11-25 14:59:13.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    9,
    N'Vé Thường',
    100000.00,
    100,
    1,
    10,
    '2025-10-26 15:27:23.000',
    '2025-11-25 15:27:23.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    9,
    N'Vé VIP',
    500000.00,
    50,
    1,
    5,
    '2025-10-26 15:27:41.000',
    '2025-11-25 15:27:41.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    10,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    10,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    10,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    11,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    11,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    11,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    12,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    12,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    12,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    13,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    13,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    13,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    14,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    14,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    14,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    15,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    15,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    15,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    16,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    16,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    16,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    17,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    17,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    17,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    18,
    N'Hạng VVIP',
    1500000.00,
    10,
    1,
    3,
    '2025-10-26 15:37:32.000',
    '2025-11-25 15:37:32.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    18,
    N'Hạng VIP',
    900000.00,
    30,
    1,
    6,
    '2025-10-26 15:38:07.000',
    '2025-11-25 15:38:07.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    18,
    N'Hạng Thường',
    500000.00,
    100,
    1,
    10,
    '2025-10-26 15:38:42.000',
    '2025-11-25 15:38:42.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    19,
    N'Vé Thường',
    100000.00,
    100,
    1,
    10,
    '2025-10-26 16:48:10.000',
    '2025-11-25 16:48:10.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    19,
    N'Vé VIP',
    200000.00,
    100,
    1,
    10,
    '2025-10-26 16:48:31.000',
    '2025-11-25 16:48:31.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    20,
    N've thuong',
    100000.00,
    100,
    1,
    10,
    '2025-10-27 01:31:41.000',
    '2025-11-26 01:31:41.000',
    'Active'
);

INSERT INTO TicketType (EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (
    20,
    N've vip',
    1000000.00,
    100,
    1,
    10,
    '2025-10-27 01:33:40.000',
    '2025-11-26 01:33:40.000',
    'Active'
);

-- ========================================
-- INSERT VOUCHERS
-- ========================================

INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'WELCOME10',
    10.00,
    '2025-10-26 14:30:08.000',
    '2026-01-26 14:30:08.000',
    1,
    '2025-10-26 14:30:08.000'
);

INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'SAVE20',
    20.00,
    '2025-10-26 14:30:08.000',
    '2025-12-26 14:30:08.000',
    1,
    '2025-10-26 14:30:08.000'
);

INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'EXPIRED15',
    15.00,
    '2025-08-26 14:30:08.000',
    '2025-09-26 14:30:08.000',
    1,
    '2025-10-26 14:30:08.000'
);

INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'SUMMER25',
    25.00,
    '2025-10-26 14:30:08.000',
    '2025-11-26 14:30:08.000',
    1,
    '2025-10-26 14:30:08.000'
);

INSERT INTO Voucher (VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, CreatedAt)
VALUES (
    'VIP30',
    30.00,
    '2025-10-26 14:30:08.000',
    '2026-04-26 14:30:08.000',
    1,
    '2025-10-26 14:30:08.000'
);

-- ========================================
-- FINAL VERIFICATION
-- ========================================

-- Check Ticket Types by Event
SELECT 'Ticket Types Count:' as Info, COUNT(*) as Count FROM TicketType;
SELECT 'Vouchers Count:' as Info, COUNT(*) as Count FROM Voucher;

PRINT '========================================';
PRINT 'Sample data injection completed successfully!';
PRINT 'Total: 5 Users (2 Hosts, 3 Customers)';
PRINT 'Total:  Events';
PRINT 'Total: 48 Ticket Types';
PRINT 'Total: 5 Vouchers';
PRINT '========================================';
