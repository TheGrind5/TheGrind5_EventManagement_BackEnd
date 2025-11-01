-- TheGrind5 Extended Sample Data
-- Auto-generated: 2025-10-29 21:37:51
-- Events: 13 | Images: 10

USE EventDB;
GO

-- Clear existing data
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
DELETE FROM AISuggestion;
DELETE FROM [User];
DELETE FROM Campus;

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
DBCC CHECKIDENT ('Campus', RESEED, 0);

-- Insert Users
SET IDENTITY_INSERT [User] ON;

IF NOT EXISTS (SELECT 1 FROM [User] WHERE UserId = 1)
BEGIN
    INSERT INTO [User] (UserId, Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
    VALUES (1, 'host1', N'Lý Thanh Tân', 'host1@example.com', '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u', '0123456789', 'Host', 0.00, GETUTCDATE(), GETUTCDATE(), '/assets/images/avatars/user_1.jpg', '1985-04-15', N'Nam');
END

IF NOT EXISTS (SELECT 1 FROM [User] WHERE UserId = 2)
BEGIN
    INSERT INTO [User] (UserId, Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
    VALUES (2, 'host2', N'Khanh Thông Minh', 'host2@example.com', '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u', '0987654321', 'Host', 0.00, GETUTCDATE(), GETUTCDATE(), '/assets/images/avatars/user_2.jpg', '1988-07-22', N'Nữ');
END

IF NOT EXISTS (SELECT 1 FROM [User] WHERE UserId = 3)
BEGIN
    INSERT INTO [User] (UserId, Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
    VALUES (3, 'customer1', N'Lê Văn Customer', 'customer1@example.com', '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u', '0555123456', 'Customer', 500000.00, GETUTCDATE(), GETUTCDATE(), NULL, '1992-11-08', N'Nam');
END

IF NOT EXISTS (SELECT 1 FROM [User] WHERE UserId = 4)
BEGIN
    INSERT INTO [User] (UserId, Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
    VALUES (4, 'customer2', N'Phạm Thị Test', 'customer2@example.com', '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u', '0555987654', 'Customer', 1250000.50, GETUTCDATE(), GETUTCDATE(), NULL, '1995-04-12', N'Nữ');
END

IF NOT EXISTS (SELECT 1 FROM [User] WHERE UserId = 5)
BEGIN
    INSERT INTO [User] (UserId, Username, FullName, Email, PasswordHash, Phone, Role, WalletBalance, CreatedAt, UpdatedAt, Avatar, DateOfBirth, Gender)
    VALUES (5, 'testwallet', N'Test Wallet User', 'testwallet@example.com', '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u', '0123456789', 'Customer', 999999.99, GETUTCDATE(), GETUTCDATE(), NULL, '1990-09-25', N'Nam');
END

SET IDENTITY_INSERT [User] OFF;

-- ========================================
-- Insert Campus Data: 5 FPT Campuses
-- ========================================
INSERT INTO Campus (CampusName, Province, IsActive, CreatedAt)
VALUES
    (N'Hà Nội', N'Hà Nội', 1, SYSDATETIME()),
    (N'Đà Nẵng', N'Đà Nẵng', 1, SYSDATETIME()),
    (N'Quy Nhơn', N'Quy Nhơn', 1, SYSDATETIME()),
    (N'TP. Hồ Chí Minh', N'TP. Hồ Chí Minh', 1, SYSDATETIME()),
    (N'Cần Thơ', N'Cần Thơ', 1, SYSDATETIME());

PRINT 'Đã thêm 5 campus vào bảng Campus';
GO

-- ========================================
-- CREATE ADMIN ACCOUNT
-- ========================================
-- 
-- PASSWORD: 123456 (same hash as other users)
-- Hash được tạo bằng bcrypt: $2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u
-- 
-- LOGIN CREDENTIALS:
-- Email: admin@thegrind5.com
-- Password: 123456
-- Role: Admin
-- ========================================

-- Kiểm tra xem admin đã tồn tại chưa và tạo nếu chưa có
IF NOT EXISTS (SELECT 1 FROM [User] WHERE Email = 'admin@thegrind5.com')
BEGIN
    -- Tạo UserId cho admin (sau user cuối cùng là 5, nên admin sẽ là 6)
    DECLARE @AdminId INT = (SELECT ISNULL(MAX(UserId), 0) FROM [User]) + 1;
    
    INSERT INTO [User] (
        Username, 
        FullName, 
        Email, 
        PasswordHash, 
        Phone, 
        Role, 
        WalletBalance, 
        CreatedAt, 
        UpdatedAt, 
        Avatar, 
        DateOfBirth, 
        Gender
    )
    VALUES (
        'admin',                                                                    -- Username
        N'Quản Trị Viên',                                                          -- FullName
        'admin@thegrind5.com',                                                      -- Email
        '$2a$11$DeIW.c5wburPqu.9eeGZFucgHpogn/DHtnvEkJdbd8uGH/6BBIb5u',           -- PasswordHash (123456)
        '0999999999',                                                               -- Phone
        'Admin',                                                                    -- Role
        0.00,                                                                       -- WalletBalance
        GETUTCDATE(),                                                               -- CreatedAt
        GETUTCDATE(),                                                               -- UpdatedAt
        NULL,                                                                       -- Avatar
        '1990-01-01',                                                               -- DateOfBirth
        N'Nam'                                                                      -- Gender
    );
    
    PRINT '✅ Admin account created successfully!';
    PRINT '';
    PRINT '========================================';
    PRINT 'ADMIN ACCOUNT INFORMATION';
    PRINT '========================================';
    PRINT 'Email: admin@thegrind5.com';
    PRINT 'Password: 123456';
    PRINT 'Role: Admin';
    PRINT '========================================';
END
ELSE
BEGIN
    PRINT '⚠️  Admin account already exists!';
    PRINT 'Email: admin@thegrind5.com';
END

GO

-- Insert Events
SET IDENTITY_INSERT Event ON;

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (4, 2, N'BẢN HÒA CA – ĐÊM NHẠC THIỆN NGUYỆN VÌ TRẺ EM VÙNG NÚI', N'🌈 “Khi âm nhạc cất lên – yêu thương lan tỏa”
💌 1. Giới thiệu chương trình

“Bản Hòa Ca” là chương trình âm nhạc thiện nguyện do nhóm nghệ sĩ trẻ cùng quỹ cộng đồng “Hành Trình Xanh” phối hợp tổ chức, nhằm gây quỹ hỗ trợ trẻ em vùng núi khó khăn tại các tỉnh Tây Bắc và Tây Nguyên.
Sự kiện là nơi hội tụ của âm nhạc – sẻ chia – và lòng nhân ái, mang đến một đêm diễn kết nối trái tim của người thành thị với những ước mơ nhỏ bé nơi núi rừng xa xôi.

🗓 2. Thời gian & Địa điểm

📅 Ngày: 21 tháng 12 năm 2025
🕖 Giờ: 19h00 – 22h30
📍 Địa điểm: Nhà hát Lớn Hà Nội
🎟 Giá vé: 200.000 – 1.000.000 VNĐ (toàn bộ lợi nhuận chuyển vào quỹ thiện nguyện “Nốt Nhạc Cho Em”)

🎵 3. Mục đích & Ý nghĩa

Gây quỹ xây dựng thư viện mini và lớp học nghệ thuật cho trẻ em vùng cao.
Trao 500 phần quà Tết (áo ấm, giày dép, sách vở) cho trẻ nhỏ tại Lai Châu và Kon Tum.
Tạo cơ hội để nghệ sĩ, doanh nghiệp và cộng đồng cùng lan tỏa thông điệp thiện nguyện qua âm nhạc.

🎤 4. Nội dung chương trình
Phần 1 – Giai Điệu Của Núi
Mở màn bằng tiết mục múa dân tộc “Mây Trên Đỉnh Rừng”.
Biểu diễn acoustic “Tiếng Suối Gọi”, “Đi Giữa Đại Ngàn” – ca sĩ Tân Lý & Nguyên Hà.

Phần 2 – Trái Tim Cho Em

Dàn đồng ca thiếu nhi thể hiện “Nụ Cười Nơi Cao Nguyên”.
Trình chiếu phóng sự “Hành trình đến bản Séo Lủng – ngôi trường trên mây”.
Đấu giá thiện nguyện các vật phẩm nghệ sĩ quyên tặng (đàn, tranh, áo lưu niệm).

Phần 3 – Hòa Ca Ánh Sáng

Ca sĩ khách mời: Hà Anh Tuấn, Phan Mạnh Quỳnh, Mỹ Anh, Tân Lý.
Ca khúc chủ đề “Bản Hòa Ca” – tác phẩm gốc sáng tác riêng cho chương trình.
Kết thúc với nghi thức thắp sáng “Cây Nến Hy Vọng” – mỗi ngọn nến tượng trưng cho một ước mơ của trẻ em vùng núi.

💫 5. Thông điệp chương trình

“Giữa đại ngàn, mỗi tiếng hát là một tia sáng.
Khi chúng ta cùng ngân lên một bản hòa ca – mọi trái tim đều gần nhau hơn.”

🧡 6. Đơn vị tổ chức & bảo trợ

Đơn vị tổ chức: Quỹ Hành Trình Xanh, CLB Rhythm – Xưởng Làm Nhạc, Công ty Tân Lý Entertainment
Bảo trợ truyền thông: Báo Tuổi Trẻ, VTV', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), N'123 LÊ Duẩn, Phường Hòa Thuận Đông, Quận Hải Châu, Đà Nẵng', N'Concert', 'Offline', 'Music', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00CA Du\u1EA9n","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng H\u00F2a Thu\u1EADn \u0110\u00F4ng","EventImage":"/assets/images/events/thanh_am_viet.jpg","BackgroundImage":"/assets/images/events/thanh_am_viet.jpg","EventIntroduction":null,"EventDetails":null,"specialGuestsList":null,"SpecialExperience":null}', N'{"terms": "Khong hoan tien sau khi mua", "childrenTerms": "Trẻ em dưới 3 tuổi miễn phí", "vatTerms": "Đã bao gồm VAT 10%"}', N'{"OrganizerLogo":"/assets/images/events/d78203b3-a549-4975-af3f-06efa8a05758.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng L\u00E0m Nh\u1EA1c","OrganizerInfo":"none."}', N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761673746474","Name":"Khu v\u1EF1c 1","Shape":"rectangle","Coordinates":[{"X":198,"Y":104.11249542236328},{"X":810,"Y":104.11249542236328},{"X":810,"Y":327.1124954223633},{"X":198,"Y":327.1124954223633}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761673747474","Name":"Khu v\u1EF1c 2","Shape":"rectangle","Coordinates":[{"X":176,"Y":377.1124954223633},{"X":310,"Y":377.1124954223633},{"X":310,"Y":468.1124954223633},{"X":176,"Y":468.1124954223633}],"Color":"#764ba2","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761673748282","Name":"Khu v\u1EF1c 3","Shape":"rectangle","Coordinates":[{"X":610,"Y":390.1124954223633},{"X":790,"Y":390.1124954223633},{"X":790,"Y":449.1124954223633},{"X":610,"Y":449.1124954223633}],"Color":"#f093fb","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761673748904","Name":"Khu v\u1EF1c 4","Shape":"rectangle","Coordinates":[{"X":525,"Y":509.1124954223633},{"X":658,"Y":509.1124954223633},{"X":658,"Y":567.1124954223633},{"X":525,"Y":567.1124954223633}],"Color":"#4facfe","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761673749486","Name":"Khu v\u1EF1c 5","Shape":"rectangle","Coordinates":[{"X":858,"Y":573.1124954223633},{"X":950,"Y":573.1124954223633},{"X":950,"Y":636.1124954223633},{"X":858,"Y":636.1124954223633}],"Color":"#00f2fe","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761673750176","Name":"Khu v\u1EF1c 6","Shape":"rectangle","Coordinates":[{"X":403,"Y":540.1124954223633},{"X":478,"Y":540.1124954223633},{"X":478,"Y":593.1124954223633},{"X":403,"Y":593.1124954223633}],"Color":"#43e97b","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""}]}', GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (9, 1, N'Mơ Thay Em - Đêm Nhạc Thiện Nguyện Vì Trẻ Em Vùng Cao - Tân Lý', N'“Mơ Thay Em” là một đêm nhạc thiện nguyện đầy cảm xúc, nơi âm nhạc trở thành cầu nối giữa trái tim nghệ sĩ và những ước mơ nhỏ bé nơi vùng cao xa xôi.
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

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), N'123 Đường Lê Duẩn, Phường Hải Châu II, Quận Hải Châu, Đà Nẵng', N'Public', 'Offline', 'Music', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 \u0110\u01B0\u1EDDng L\u00EA Du\u1EA9n","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng H\u1EA3i Ch\u00E2u II","EventImage":"/assets/images/events/mo_thay_em.jpg","BackgroundImage":"/assets/images/events/mo_thay_em.jpg","EventIntroduction":"\u201CM\u01A1 Thay Em\u201D l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c thi\u1EC7n nguy\u1EC7n \u0111\u1EA7y c\u1EA3m x\u00FAc, n\u01A1i \u00E2m nh\u1EA1c tr\u1EDF th\u00E0nh c\u1EA7u n\u1ED1i gi\u1EEFa tr\u00E1i tim ngh\u1EC7 s\u0129 v\u00E0 nh\u1EEFng \u01B0\u1EDBc m\u01A1 nh\u1ECF b\u00E9 n\u01A1i v\u00F9ng cao xa x\u00F4i.\nCh\u01B0\u01A1ng tr\u00ECnh h\u01B0\u1EDBng t\u1EDBi g\u00E2y qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D, h\u1ED7 tr\u1EE3 mua s\u00E1ch v\u1EDF, qu\u1EA7n \u00E1o m\u00F9a \u0111\u00F4ng v\u00E0 h\u1ECDc c\u1EE5 cho tr\u1EBB em t\u1EA1i c\u00E1c \u0111i\u1EC3m tr\u01B0\u1EDDng v\u00F9ng n\u00FAi ph\u00EDa B\u1EAFc Vi\u1EC7t Nam.\n\n\uD83D\uDCAB Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 15 th\u00E1ng 12 n\u0103m 2025\n\uD83D\uDD58 Gi\u1EDD: 19h00 \u2013 22h30\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t VOH \u2013 \u0110\u00E0i Ti\u1EBFng n\u00F3i Nh\u00E2n d\u00E2n TP.HCM, s\u1ED1 37 Nguy\u1EC5n B\u1EC9nh Khi\u00EAm, Qu\u1EADn 1\n\uD83C\uDF9F V\u00E9 m\u1EDDi: To\u00E0n b\u1ED9 ti\u1EC1n v\u00E9 v\u00E0 quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c tr\u00EDch 100% v\u00E0o qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D\n\uD83C\uDFA4 N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\nM\u1EDF m\u00E0n: Ti\u1EBFt m\u1EE5c h\u1EE3p ca \u201CG\u1EEDi Em N\u1EE5 C\u01B0\u1EDDi M\u00F9a \u0110\u00F4ng\u201D \u2013 tr\u00ECnh di\u1EC5n b\u1EDFi d\u00E0n h\u1EE3p x\u01B0\u1EDBng thi\u1EBFu nhi H\u00F2a B\u00ECnh.\n\nPh\u1EA7n 1 \u2013 Nh\u1EEFng Gi\u1EA5c M\u01A1 B\u00E9 Nh\u1ECF:\nTr\u00ECnh di\u1EC5n acoustic v\u1EDBi c\u00E1c ca s\u0129 tr\u1EBB nh\u01B0 Th\u1ECBnh Suy, V\u0169 C\u00E1t T\u01B0\u1EDDng, Orange.\nCh\u1EE7 \u0111\u1EC1: \u01AF\u1EDBc m\u01A1, tu\u1ED5i th\u01A1, v\u00E0 ni\u1EC1m tin.\n\nPh\u1EA7n 2 \u2013 M\u01A1 Thay Em:\nS\u00E2n kh\u1EA5u ch\u00EDnh, tr\u00ECnh di\u1EC5n live band c\u00F9ng c\u00E1c ngh\u1EC7 s\u0129 kh\u00E1ch m\u1EDDi H\u00E0 Anh Tu\u1EA5n, M\u1EF9 T\u00E2m, \u0110en V\u00E2u.\nG\u00E2y qu\u1EF9 tr\u1EF1c ti\u1EBFp b\u1EB1ng h\u00ECnh th\u1EE9c auction (\u0111\u1EA5u gi\u00E1 v\u1EADt ph\u1EA9m ngh\u1EC7 s\u0129 t\u1EB7ng).\n\nPh\u1EA7n 3 \u2013 Trao Y\u00EAu Th\u01B0\u01A1ng:\nC\u00F4ng b\u1ED1 s\u1ED1 ti\u1EC1n quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c.\nTr\u00ECnh chi\u1EBFu video h\u00E0nh tr\u00ECnh trao qu\u00E0 v\u00E0 x\u00E2y l\u1EDBp h\u1ECDc v\u00F9ng cao.\n\n\u2764\uFE0F Th\u00F4ng \u0111i\u1EC7p ch\u01B0\u01A1ng tr\u00ECnh\n\n\u201CC\u00F3 nh\u1EEFng gi\u1EA5c m\u01A1 tr\u1EBB em kh\u00F4ng th\u1EC3 n\u00F3i ra,\nnh\u01B0ng ch\u00FAng ta c\u00F3 th\u1EC3 h\u00E1t thay, m\u01A1 thay em \u2014\n\u0111\u1EC3 m\u00F9a \u0111\u00F4ng n\u00E0y kh\u00F4ng c\u00F2n l\u1EA1nh tr\u00EAn \u0111\u00F4i ch\u00E2n b\u00E9 nh\u1ECF.\u201D\n\n\uD83E\uDDFA \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c:\n\nBan T\u1ED5 Ch\u1EE9c: Nh\u00F3m thi\u1EC7n nguy\u1EC7n H\u00E0nh Tr\u00ECnh Xanh ph\u1ED1i h\u1EE3p c\u00F9ng \u0110\u00E0i VOH v\u00E0 Qu\u1EF9 Tr\u1EBB Em Vi\u1EC7t\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: HTV, VTV6, B\u00E1o Tu\u1ED5i Tr\u1EBB, Zing News\n\u0110\u1ED1i t\u00E1c t\u00E0i tr\u1EE3: Vinamilk, PNJ, Yamaha Music Vi\u1EC7t Nam\n\uD83D\uDCDE Li\u00EAn h\u1EC7 tham gia / t\u00E0i tr\u1EE3\n\uD83D\uDCE7 Email: mothayem.charity@gmail.com\n\u260E\uFE0F Hotline: 0909 123 456\n\uD83C\uDF10 Website: www.mothayem.vn","EventDetails":"\u201CM\u01A1 Thay Em\u201D l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c thi\u1EC7n nguy\u1EC7n \u0111\u1EA7y c\u1EA3m x\u00FAc, n\u01A1i \u00E2m nh\u1EA1c tr\u1EDF th\u00E0nh c\u1EA7u n\u1ED1i gi\u1EEFa tr\u00E1i tim ngh\u1EC7 s\u0129 v\u00E0 nh\u1EEFng \u01B0\u1EDBc m\u01A1 nh\u1ECF b\u00E9 n\u01A1i v\u00F9ng cao xa x\u00F4i.\nCh\u01B0\u01A1ng tr\u00ECnh h\u01B0\u1EDBng t\u1EDBi g\u00E2y qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D, h\u1ED7 tr\u1EE3 mua s\u00E1ch v\u1EDF, qu\u1EA7n \u00E1o m\u00F9a \u0111\u00F4ng v\u00E0 h\u1ECDc c\u1EE5 cho tr\u1EBB em t\u1EA1i c\u00E1c \u0111i\u1EC3m tr\u01B0\u1EDDng v\u00F9ng n\u00FAi ph\u00EDa B\u1EAFc Vi\u1EC7t Nam.\n\n\uD83D\uDCAB Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 15 th\u00E1ng 12 n\u0103m 2025\n\uD83D\uDD58 Gi\u1EDD: 19h00 \u2013 22h30\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t VOH \u2013 \u0110\u00E0i Ti\u1EBFng n\u00F3i Nh\u00E2n d\u00E2n TP.HCM, s\u1ED1 37 Nguy\u1EC5n B\u1EC9nh Khi\u00EAm, Qu\u1EADn 1\n\uD83C\uDF9F V\u00E9 m\u1EDDi: To\u00E0n b\u1ED9 ti\u1EC1n v\u00E9 v\u00E0 quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c tr\u00EDch 100% v\u00E0o qu\u1EF9 \u201C\u00C1o \u1EA4m Cho Em\u201D\n\uD83C\uDFA4 N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\nM\u1EDF m\u00E0n: Ti\u1EBFt m\u1EE5c h\u1EE3p ca \u201CG\u1EEDi Em N\u1EE5 C\u01B0\u1EDDi M\u00F9a \u0110\u00F4ng\u201D \u2013 tr\u00ECnh di\u1EC5n b\u1EDFi d\u00E0n h\u1EE3p x\u01B0\u1EDBng thi\u1EBFu nhi H\u00F2a B\u00ECnh.\n\nPh\u1EA7n 1 \u2013 Nh\u1EEFng Gi\u1EA5c M\u01A1 B\u00E9 Nh\u1ECF:\nTr\u00ECnh di\u1EC5n acoustic v\u1EDBi c\u00E1c ca s\u0129 tr\u1EBB nh\u01B0 Th\u1ECBnh Suy, V\u0169 C\u00E1t T\u01B0\u1EDDng, Orange.\nCh\u1EE7 \u0111\u1EC1: \u01AF\u1EDBc m\u01A1, tu\u1ED5i th\u01A1, v\u00E0 ni\u1EC1m tin.\n\nPh\u1EA7n 2 \u2013 M\u01A1 Thay Em:\nS\u00E2n kh\u1EA5u ch\u00EDnh, tr\u00ECnh di\u1EC5n live band c\u00F9ng c\u00E1c ngh\u1EC7 s\u0129 kh\u00E1ch m\u1EDDi H\u00E0 Anh Tu\u1EA5n, M\u1EF9 T\u00E2m, \u0110en V\u00E2u.\nG\u00E2y qu\u1EF9 tr\u1EF1c ti\u1EBFp b\u1EB1ng h\u00ECnh th\u1EE9c auction (\u0111\u1EA5u gi\u00E1 v\u1EADt ph\u1EA9m ngh\u1EC7 s\u0129 t\u1EB7ng).\n\nPh\u1EA7n 3 \u2013 Trao Y\u00EAu Th\u01B0\u01A1ng:\nC\u00F4ng b\u1ED1 s\u1ED1 ti\u1EC1n quy\u00EAn g\u00F3p \u0111\u01B0\u1EE3c.\nTr\u00ECnh chi\u1EBFu video h\u00E0nh tr\u00ECnh trao qu\u00E0 v\u00E0 x\u00E2y l\u1EDBp h\u1ECDc v\u00F9ng cao.\n\n\u2764\uFE0F Th\u00F4ng \u0111i\u1EC7p ch\u01B0\u01A1ng tr\u00ECnh\n\n\u201CC\u00F3 nh\u1EEFng gi\u1EA5c m\u01A1 tr\u1EBB em kh\u00F4ng th\u1EC3 n\u00F3i ra,\nnh\u01B0ng ch\u00FAng ta c\u00F3 th\u1EC3 h\u00E1t thay, m\u01A1 thay em \u2014\n\u0111\u1EC3 m\u00F9a \u0111\u00F4ng n\u00E0y kh\u00F4ng c\u00F2n l\u1EA1nh tr\u00EAn \u0111\u00F4i ch\u00E2n b\u00E9 nh\u1ECF.\u201D\n\n\uD83E\uDDFA \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c:\n\nBan T\u1ED5 Ch\u1EE9c: Nh\u00F3m thi\u1EC7n nguy\u1EC7n H\u00E0nh Tr\u00ECnh Xanh ph\u1ED1i h\u1EE3p c\u00F9ng \u0110\u00E0i VOH v\u00E0 Qu\u1EF9 Tr\u1EBB Em Vi\u1EC7t\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: HTV, VTV6, B\u00E1o Tu\u1ED5i Tr\u1EBB, Zing News\n\u0110\u1ED1i t\u00E1c t\u00E0i tr\u1EE3: Vinamilk, PNJ, Yamaha Music Vi\u1EC7t Nam\n\uD83D\uDCDE Li\u00EAn h\u1EC7 tham gia / t\u00E0i tr\u1EE3\n\uD83D\uDCE7 Email: mothayem.charity@gmail.com\n\u260E\uFE0F Hotline: 0909 123 456\n\uD83C\uDF10 Website: www.mothayem.vn","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/2773df59-6399-41d1-9179-a75d779607ec.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"Club tr\u1EF1c thu\u1ED9c tr\u01B0\u1EDDng \u0111\u1EA1i h\u1ECDc FPT"}', N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761492494537","Name":"S\u00E2n Kh\u1EA5u","Shape":"rectangle","Coordinates":[{"X":201,"Y":145.1125030517578},{"X":849,"Y":145.1125030517578},{"X":849,"Y":259.1125030517578},{"X":201,"Y":259.1125030517578}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761492513829","Name":"V\u00E9 VIP","Shape":"rectangle","Coordinates":[{"X":179,"Y":305.1124954223633},{"X":888,"Y":305.1124954223633},{"X":888,"Y":360.1124954223633},{"X":179,"Y":360.1124954223633}],"Color":"#764ba2","TicketTypeId":1,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761492527322","Name":"V\u00E9 Th\u01B0\u1EDDng","Shape":"rectangle","Coordinates":[{"X":232,"Y":399.1125030517578},{"X":826,"Y":399.1125030517578},{"X":826,"Y":642.1125030517578},{"X":232,"Y":642.1125030517578}],"Color":"#f093fb","TicketTypeId":0,"IsStanding":false,"Capacity":null,"Label":""}]}', GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (18, 1, N'Hạ Sau Hè – Tân Lý Live Concert', N'“Hạ Sau Hè” là live concert đầu tiên trong sự nghiệp của ca sĩ Tân Lý, đánh dấu chặng đường trưởng thành của anh sau nhiều năm hoạt động trong làng nhạc Indie Việt.
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
☎️ Hotline: 0906 612 123', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh', N'Public', 'Offline', 'Music', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"TP. H\u1ED3 Ch\u00ED Minh","District":"Qu\u1EADn 1","Ward":"Ph\u01B0\u1EDDng B\u1EBFn Ngh\u00E9","EventImage":"/assets/images/events/ha_sau_he.png","BackgroundImage":"/assets/images/events/ha_sau_he.png","EventIntroduction":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","EventDetails":"\u201CH\u1EA1 Sau H\u00E8\u201D l\u00E0 live concert \u0111\u1EA7u ti\u00EAn trong s\u1EF1 nghi\u1EC7p c\u1EE7a ca s\u0129 T\u00E2n L\u00FD, \u0111\u00E1nh d\u1EA5u ch\u1EB7ng \u0111\u01B0\u1EDDng tr\u01B0\u1EDFng th\u00E0nh c\u1EE7a anh sau nhi\u1EC1u n\u0103m ho\u1EA1t \u0111\u1ED9ng trong l\u00E0ng nh\u1EA1c Indie Vi\u1EC7t.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB nh\u1EEFng k\u00FD \u1EE9c thanh xu\u00E2n, m\u1ED1i t\u00ECnh c\u0169 v\u00E0 nh\u1EEFng ng\u00E0y h\u00E8 ch\u01B0a k\u1ECBp n\u00F3i l\u1EDDi t\u1EA1m bi\u1EC7t, \u201CH\u1EA1 Sau H\u00E8\u201D h\u1EE9a h\u1EB9n s\u1EBD l\u00E0 h\u00E0nh tr\u00ECnh \u00E2m nh\u1EA1c chan ch\u1EE9a c\u1EA3m x\u00FAc \u2013 n\u01A1i ng\u01B0\u1EDDi nghe \u0111\u01B0\u1EE3c s\u1ED1ng l\u1EA1i trong t\u1EEBng nh\u1ECBp giai \u0111i\u1EC7u c\u1EE7a tu\u1ED5i tr\u1EBB.\n\n\uD83C\uDF05 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\uD83D\uDDD3 Ng\u00E0y: 10 th\u00E1ng 8 n\u0103m 2025\n\uD83D\uDD56 Gi\u1EDD: 19h30 \u2013 22h00\n\uD83D\uDCCD \u0110\u1ECBa \u0111i\u1EC3m: Nh\u00E0 h\u00E1t H\u00F2a B\u00ECnh, Qu\u1EADn 10, TP.HCM\n\n\uD83C\uDF9F Gi\u00E1 v\u00E9:\n\nH\u1EA1ng VVIP: 1.500.000\u0111 (G\u1EB7p g\u1EE1, k\u00FD t\u1EB7ng, ch\u1EE5p \u1EA3nh c\u00F9ng ca s\u0129)\nH\u1EA1ng VIP: 900.000\u0111\nH\u1EA1ng Th\u01B0\u1EDDng: 500.000\u0111\n\n\uD83C\uDF3B N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\nPh\u1EA7n m\u1EDF m\u00E0n \u2013 Thanh \u00E2m m\u00F9a n\u1EAFng:\nCa kh\u00FAc: \u201CNg\u00E0y Em \u0110i Qua\u201D, \u201CH\u1EA1 G\u1ECDi T\u00EAn\u201D, \u201CN\u1EAFng Tr\u00EAn M\u00E1i Tr\u01B0\u1EDDng\u201D\nPhong c\u00E1ch: Pop \u2013 Folk pha ch\u00FAt Retro, t\u00E1i hi\u1EC7n kh\u00F4ng kh\u00ED h\u1ECDc tr\u00F2.\n\nPh\u1EA7n 2 \u2013 H\u1EA1 Sau H\u00E8 (Main Stage):\nC\u00E1c b\u1EA3n hit g\u1EAFn li\u1EC1n v\u1EDBi t\u00EAn tu\u1ED5i T\u00E2n L\u00FD: \u201C\u0110o\u1EA1n K\u1EBFt M\u00F9a Xanh\u201D, \u201CNg\u01B0\u1EDDi \u1EDE L\u1EA1i Sau H\u00E8\u201D, \u201CC\u00E1nh Di\u1EC1u N\u0103m \u1EA4y\u201D\nS\u00E2n kh\u1EA5u \u00E1nh s\u00E1ng LED m\u00F4 ph\u1ECFng c\u01A1n m\u01B0a m\u00F9a h\u1EA1 \u2013 \u0111i\u1EC3m nh\u1EA5n c\u1EA3m x\u00FAc c\u1EE7a \u0111\u00EAm nh\u1EA1c.\n\nPh\u1EA7n 3 \u2013 H\u1EB9n G\u1EB7p L\u1EA1i, Th\u00E1ng T\u00E1m:\nT\u00E2n L\u00FD song ca c\u00F9ng kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t L\u00EA C\u00E1t Tr\u1ECDng L\u00FD v\u00E0 Th\u00E1i \u0110inh.\nK\u1EBFt th\u00FAc b\u1EB1ng ca kh\u00FAc m\u1EDBi \u201CNh\u1EEFng Ng\u00E0y N\u1EAFng C\u00F2n L\u1EA1i\u201D \u2013 ra m\u1EAFt \u0111\u1ED9c quy\u1EC1n trong concert.\n\n\uD83D\uDC8C Th\u00F4ng \u0111i\u1EC7p \u00E2m nh\u1EA1c\n\n\u201CM\u00F9a h\u1EA1 kh\u00F4ng bao gi\u1EDD th\u1EADt s\u1EF1 qua \u0111i,\nch\u1EC9 l\u00E0 ta kh\u00F4ng c\u00F2n \u0111\u1EE9ng d\u01B0\u1EDBi n\u1EAFng n\u0103m \u1EA5y n\u1EEFa.\u201D\n\u201CH\u1EA1 Sau H\u00E8\u201D kh\u00F4ng ch\u1EC9 l\u00E0 m\u1ED9t \u0111\u00EAm nh\u1EA1c \u2013 m\u00E0 l\u00E0 l\u1EDDi tri \u00E2n c\u1EE7a T\u00E2n L\u00FD d\u00E0nh cho tu\u1ED5i tr\u1EBB, cho nh\u1EEFng ng\u01B0\u1EDDi \u0111\u00E3 y\u00EAu, \u0111\u00E3 \u0111i qua, v\u00E0 v\u1EABn m\u1EC9m c\u01B0\u1EDDi khi nh\u00ECn l\u1EA1i.\n\n\uD83C\uDFA7 \u0110\u1ED9i ng\u0169 s\u1EA3n xu\u1EA5t\n\nCa s\u0129 ch\u00EDnh: T\u00E2n L\u00FD\nGi\u00E1m \u0111\u1ED1c \u00E2m nh\u1EA1c: Long Halo\n\u0110\u1EA1o di\u1EC5n s\u00E2n kh\u1EA5u: Hu\u1EF3nh Tu\u1EA5n Ki\u1EC7t\nBan nh\u1EA1c: The July Notes Band\nKh\u00E1ch m\u1EDDi: L\u00EA C\u00E1t Tr\u1ECDng L\u00FD, Th\u00E1i \u0110inh, Nguy\u00EAn H\u00E0\n\n\uD83D\uDCAB \u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c \u0026 t\u00E0i tr\u1EE3\n\nT\u1ED5 ch\u1EE9c: L\u00FD Sound Entertainment\nB\u1EA3o tr\u1EE3 truy\u1EC1n th\u00F4ng: Billboard Vi\u1EC7t Nam, Yan News, TikTok Music VN\n\u0110\u1ED3ng h\u00E0nh: Shopee Music, Yamaha, Highlands Coffee\n\n\uD83D\uDCF1 Li\u00EAn h\u1EC7 \u0026 \u0111\u1EB7t v\u00E9\n\uD83C\uDF10 Website: www.hasauhe.vn\n\uD83D\uDCE7 Email: concert.hasauhe@gmail.com\n\u260E\uFE0F Hotline: 0906 612 123","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/8d50228a-e278-40a6-ae12-fea80d5cc9ab.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng l\u00E0m nh\u1EA1c","OrganizerInfo":"clb fpt"}', N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761493198019","Name":"Stage","Shape":"rectangle","Coordinates":[{"X":278,"Y":69.11250305175781},{"X":746,"Y":69.11250305175781},{"X":746,"Y":325.1125030517578},{"X":278,"Y":325.1125030517578}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761493215543","Name":"H\u1EA1ng VVIP","Shape":"rectangle","Coordinates":[{"X":346,"Y":349.1124954223633},{"X":693,"Y":349.1124954223633},{"X":693,"Y":413.1124954223633},{"X":346,"Y":413.1124954223633}],"Color":"#764ba2","TicketTypeId":1,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761494556787","Name":"H\u1EA1ng Vip","Shape":"rectangle","Coordinates":[{"X":402.4000015258789,"Y":465.4125061035156},{"X":580.4000015258789,"Y":465.4125061035156},{"X":580.4000015258789,"Y":515.4125061035156},{"X":402.4000015258789,"Y":515.4125061035156}],"Color":"#f093fb","TicketTypeId":2,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761494583688","Name":"H\u1EA1ng Th\u01B0\u1EDDng","Shape":"rectangle","Coordinates":[{"X":225,"Y":507.1125030517578},{"X":786,"Y":507.1125030517578},{"X":786,"Y":678.1125030517578},{"X":225,"Y":678.1125030517578}],"Color":"#fee140","TicketTypeId":3,"IsStanding":false,"Capacity":null,"Label":""}]}', GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (22, 3, N'GÁNH CẢ TƯƠNG LAI', N'🧠 Chủ đề chính

“Fix bug không chỉ bằng tay, mà bằng cả tương lai.”
Gánh Cả Tương Lai là sự kiện học thuật đặc biệt dành cho các lập trình viên trẻ, sinh viên công nghệ và coder đang trong giai đoạn thực tập, nhằm nâng cao kỹ năng xử lý lỗi, tối ưu hệ thống và làm việc nhóm trong môi trường thực tế.

💡 Nội dung nổi bật

Workshop 1: Debugging Mastery – Phương pháp tư duy khi gặp bug phức tạp (có demo thực tế trên IDE Visual Studio & IntelliJ).

Workshop 2: AI-Powered Debugging – Ứng dụng trí tuệ nhân tạo trong phát hiện và sửa lỗi.
Panel Discussion: “Khi bug trở thành bài học” – chia sẻ từ các senior developer FPT Software.
Mini Contest: “Fix It Fast” – cuộc thi lập trình sửa lỗi trong 30 phút với đề bài thật từ doanh nghiệp.

🧩 Mục tiêu sự kiện

Tăng cường kỹ năng nhận diện và phân tích lỗi.
Giúp coder hiểu sâu hơn về quy trình debug chuyên nghiệp.
Tạo cơ hội networking giữa sinh viên và kỹ sư phần mềm tại các công ty đối tác.

🏆 Giải thưởng

🥇 Giải “Bug Hunter Vàng” – 3.000.000 VNĐ + Giấy chứng nhận.
🥈 Giải “Tốc độ ánh sáng” – 1.000.000 VNĐ + quà từ nhà tài trợ.
🎁 Mọi người tham dự đều nhận Sticker “Keep Calm & Fix Bug” độc quyền.

🎤 Khách mời đặc biệt

Anh Nguyễn Minh Trí – Senior Engineer, FPT Software.
Chị Vũ Thảo Linh – Software QA Lead, VNG Corporation.
MC: Coder hài hước Tân Lý – người dẫn dắt không khí trẻ trung, dí dỏm.

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), NULL, N'Public', 'Online', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"","StreetAddress":"","Province":"","District":"","Ward":"","EventImage":"/assets/images/events/er_training.jpg","BackgroundImage":"/assets/images/events/er_training.jpg","EventIntroduction":"\uD83E\uDDE0 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\n\u201CFix bug kh\u00F4ng ch\u1EC9 b\u1EB1ng tay, m\u00E0 b\u1EB1ng c\u1EA3 t\u01B0\u01A1ng lai.\u201D\nG\u00E1nh C\u1EA3 T\u01B0\u01A1ng Lai l\u00E0 s\u1EF1 ki\u1EC7n h\u1ECDc thu\u1EADt \u0111\u1EB7c bi\u1EC7t d\u00E0nh cho c\u00E1c l\u1EADp tr\u00ECnh vi\u00EAn tr\u1EBB, sinh vi\u00EAn c\u00F4ng ngh\u1EC7 v\u00E0 coder \u0111ang trong giai \u0111o\u1EA1n th\u1EF1c t\u1EADp, nh\u1EB1m n\u00E2ng cao k\u1EF9 n\u0103ng x\u1EED l\u00FD l\u1ED7i, t\u1ED1i \u01B0u h\u1EC7 th\u1ED1ng v\u00E0 l\u00E0m vi\u1EC7c nh\u00F3m trong m\u00F4i tr\u01B0\u1EDDng th\u1EF1c t\u1EBF.\n\n\uD83D\uDCA1 N\u1ED9i dung n\u1ED5i b\u1EADt\n\nWorkshop 1: Debugging Mastery \u2013 Ph\u01B0\u01A1ng ph\u00E1p t\u01B0 duy khi g\u1EB7p bug ph\u1EE9c t\u1EA1p (c\u00F3 demo th\u1EF1c t\u1EBF tr\u00EAn IDE Visual Studio \u0026 IntelliJ).\n\nWorkshop 2: AI-Powered Debugging \u2013 \u1EE8ng d\u1EE5ng tr\u00ED tu\u1EC7 nh\u00E2n t\u1EA1o trong ph\u00E1t hi\u1EC7n v\u00E0 s\u1EEDa l\u1ED7i.\nPanel Discussion: \u201CKhi bug tr\u1EDF th\u00E0nh b\u00E0i h\u1ECDc\u201D \u2013 chia s\u1EBB t\u1EEB c\u00E1c senior developer FPT Software.\nMini Contest: \u201CFix It Fast\u201D \u2013 cu\u1ED9c thi l\u1EADp tr\u00ECnh s\u1EEDa l\u1ED7i trong 30 ph\u00FAt v\u1EDBi \u0111\u1EC1 b\u00E0i th\u1EADt t\u1EEB doanh nghi\u1EC7p.\n\n\uD83E\uDDE9 M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nT\u0103ng c\u01B0\u1EDDng k\u1EF9 n\u0103ng nh\u1EADn di\u1EC7n v\u00E0 ph\u00E2n t\u00EDch l\u1ED7i.\nGi\u00FAp coder hi\u1EC3u s\u00E2u h\u01A1n v\u1EC1 quy tr\u00ECnh debug chuy\u00EAn nghi\u1EC7p.\nT\u1EA1o c\u01A1 h\u1ED9i networking gi\u1EEFa sinh vi\u00EAn v\u00E0 k\u1EF9 s\u01B0 ph\u1EA7n m\u1EC1m t\u1EA1i c\u00E1c c\u00F4ng ty \u0111\u1ED1i t\u00E1c.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng\n\n\uD83E\uDD47 Gi\u1EA3i \u201CBug Hunter V\u00E0ng\u201D \u2013 3.000.000 VN\u0110 \u002B Gi\u1EA5y ch\u1EE9ng nh\u1EADn.\n\uD83E\uDD48 Gi\u1EA3i \u201CT\u1ED1c \u0111\u1ED9 \u00E1nh s\u00E1ng\u201D \u2013 1.000.000 VN\u0110 \u002B qu\u00E0 t\u1EEB nh\u00E0 t\u00E0i tr\u1EE3.\n\uD83C\uDF81 M\u1ECDi ng\u01B0\u1EDDi tham d\u1EF1 \u0111\u1EC1u nh\u1EADn Sticker \u201CKeep Calm \u0026 Fix Bug\u201D \u0111\u1ED9c quy\u1EC1n.\n\n\uD83C\uDFA4 Kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t\n\nAnh Nguy\u1EC5n Minh Tr\u00ED \u2013 Senior Engineer, FPT Software.\nCh\u1ECB V\u0169 Th\u1EA3o Linh \u2013 Software QA Lead, VNG Corporation.\nMC: Coder h\u00E0i h\u01B0\u1EDBc T\u00E2n L\u00FD \u2013 ng\u01B0\u1EDDi d\u1EABn d\u1EAFt kh\u00F4ng kh\u00ED tr\u1EBB trung, d\u00ED d\u1ECFm.","EventDetails":"\uD83E\uDDE0 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\n\u201CFix bug kh\u00F4ng ch\u1EC9 b\u1EB1ng tay, m\u00E0 b\u1EB1ng c\u1EA3 t\u01B0\u01A1ng lai.\u201D\nG\u00E1nh C\u1EA3 T\u01B0\u01A1ng Lai l\u00E0 s\u1EF1 ki\u1EC7n h\u1ECDc thu\u1EADt \u0111\u1EB7c bi\u1EC7t d\u00E0nh cho c\u00E1c l\u1EADp tr\u00ECnh vi\u00EAn tr\u1EBB, sinh vi\u00EAn c\u00F4ng ngh\u1EC7 v\u00E0 coder \u0111ang trong giai \u0111o\u1EA1n th\u1EF1c t\u1EADp, nh\u1EB1m n\u00E2ng cao k\u1EF9 n\u0103ng x\u1EED l\u00FD l\u1ED7i, t\u1ED1i \u01B0u h\u1EC7 th\u1ED1ng v\u00E0 l\u00E0m vi\u1EC7c nh\u00F3m trong m\u00F4i tr\u01B0\u1EDDng th\u1EF1c t\u1EBF.\n\n\uD83D\uDCA1 N\u1ED9i dung n\u1ED5i b\u1EADt\n\nWorkshop 1: Debugging Mastery \u2013 Ph\u01B0\u01A1ng ph\u00E1p t\u01B0 duy khi g\u1EB7p bug ph\u1EE9c t\u1EA1p (c\u00F3 demo th\u1EF1c t\u1EBF tr\u00EAn IDE Visual Studio \u0026 IntelliJ).\n\nWorkshop 2: AI-Powered Debugging \u2013 \u1EE8ng d\u1EE5ng tr\u00ED tu\u1EC7 nh\u00E2n t\u1EA1o trong ph\u00E1t hi\u1EC7n v\u00E0 s\u1EEDa l\u1ED7i.\nPanel Discussion: \u201CKhi bug tr\u1EDF th\u00E0nh b\u00E0i h\u1ECDc\u201D \u2013 chia s\u1EBB t\u1EEB c\u00E1c senior developer FPT Software.\nMini Contest: \u201CFix It Fast\u201D \u2013 cu\u1ED9c thi l\u1EADp tr\u00ECnh s\u1EEDa l\u1ED7i trong 30 ph\u00FAt v\u1EDBi \u0111\u1EC1 b\u00E0i th\u1EADt t\u1EEB doanh nghi\u1EC7p.\n\n\uD83E\uDDE9 M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nT\u0103ng c\u01B0\u1EDDng k\u1EF9 n\u0103ng nh\u1EADn di\u1EC7n v\u00E0 ph\u00E2n t\u00EDch l\u1ED7i.\nGi\u00FAp coder hi\u1EC3u s\u00E2u h\u01A1n v\u1EC1 quy tr\u00ECnh debug chuy\u00EAn nghi\u1EC7p.\nT\u1EA1o c\u01A1 h\u1ED9i networking gi\u1EEFa sinh vi\u00EAn v\u00E0 k\u1EF9 s\u01B0 ph\u1EA7n m\u1EC1m t\u1EA1i c\u00E1c c\u00F4ng ty \u0111\u1ED1i t\u00E1c.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng\n\n\uD83E\uDD47 Gi\u1EA3i \u201CBug Hunter V\u00E0ng\u201D \u2013 3.000.000 VN\u0110 \u002B Gi\u1EA5y ch\u1EE9ng nh\u1EADn.\n\uD83E\uDD48 Gi\u1EA3i \u201CT\u1ED1c \u0111\u1ED9 \u00E1nh s\u00E1ng\u201D \u2013 1.000.000 VN\u0110 \u002B qu\u00E0 t\u1EEB nh\u00E0 t\u00E0i tr\u1EE3.\n\uD83C\uDF81 M\u1ECDi ng\u01B0\u1EDDi tham d\u1EF1 \u0111\u1EC1u nh\u1EADn Sticker \u201CKeep Calm \u0026 Fix Bug\u201D \u0111\u1ED9c quy\u1EC1n.\n\n\uD83C\uDFA4 Kh\u00E1ch m\u1EDDi \u0111\u1EB7c bi\u1EC7t\n\nAnh Nguy\u1EC5n Minh Tr\u00ED \u2013 Senior Engineer, FPT Software.\nCh\u1ECB V\u0169 Th\u1EA3o Linh \u2013 Software QA Lead, VNG Corporation.\nMC: Coder h\u00E0i h\u01B0\u1EDBc T\u00E2n L\u00FD \u2013 ng\u01B0\u1EDDi d\u1EABn d\u1EAFt kh\u00F4ng kh\u00ED tr\u1EBB trung, d\u00ED d\u1ECFm.","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/0f37ad58-fd8f-416a-97c5-edfb4fc2112a.jpg","OrganizerName":"Rhythm - X\u01B0\u1EDFng L\u00E0m Nh\u1EA1c","OrganizerInfo":"ni"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (23, 1, N'FPT La’Fest 2025', N'🌏 Chủ đề chính:

“Ngôn ngữ mở lối – Văn hoá dẫn đường.”

FPT La’Fest 2025 là cuộc thi ngôn ngữ và văn hoá liên trường dành cho học sinh, sinh viên thuộc hệ thống giáo dục FPT, nhằm tôn vinh khả năng ngôn ngữ, bản sắc dân tộc và tinh thần hội nhập toàn cầu.

💡 Nội dung nổi bật

Vòng loại: Thi kỹ năng ngôn ngữ (Anh, Nhật, Trung, Hàn).
Vòng bán kết: Thể hiện năng lực sáng tạo qua video hoặc bài thuyết trình về chủ đề văn hoá.
Vòng chung kết: Biểu diễn nghệ thuật, tranh biện, và showcase văn hoá dân gian kết hợp yếu tố hiện đại.

🎯 Mục tiêu sự kiện

Gắn kết cộng đồng học sinh, sinh viên FPT trên toàn quốc.
Khuyến khích tinh thần học tập ngoại ngữ và tìm hiểu văn hoá quốc tế.
Tạo sân chơi học thuật sáng tạo, lan toả giá trị văn hoá Việt Nam.

🏆 Giải thưởng

🥇 Quán quân toàn quốc: 10.000.000 VNĐ + Học bổng ngoại ngữ.
🥈 Giải sáng tạo: 5.000.000 VNĐ + Kỷ niệm chương FPT La’Fest.
🥉 Giải văn hoá ấn tượng: 3.000.000 VNĐ + quà tặng từ nhà tài trợ.

🎤 Ngôn ngữ cuộc thi

Tiếng Anh 🇬🇧
Tiếng Nhật 🇯🇵
Tiếng Trung 🇨🇳
Tiếng Hàn 🇰🇷

📲 Đăng ký & Thông tin chi tiết

👉 Quét QR Code trên poster hoặc truy cập trang web nội bộ FPT Edu.

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), NULL, N'Public', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"FPT Education c\u01A1 s\u1EDF \u0110\u00E0 N\u1EB5ng","StreetAddress":"123 \u0110\u01B0\u1EDDng T\u00F4n Th\u1EA5t Thuy\u1EBFt","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng B\u00ECnh Hi\u00EAn","EventImage":"/assets/images/events/la_fest.png","BackgroundImage":"/assets/images/events/la_fest.png","EventIntroduction":"\uD83C\uDF0F Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh:\n\n\u201CNg\u00F4n ng\u1EEF m\u1EDF l\u1ED1i \u2013 V\u0103n ho\u00E1 d\u1EABn \u0111\u01B0\u1EDDng.\u201D\n\nFPT La\u2019Fest 2025 l\u00E0 cu\u1ED9c thi ng\u00F4n ng\u1EEF v\u00E0 v\u0103n ho\u00E1 li\u00EAn tr\u01B0\u1EDDng d\u00E0nh cho h\u1ECDc sinh, sinh vi\u00EAn thu\u1ED9c h\u1EC7 th\u1ED1ng gi\u00E1o d\u1EE5c FPT, nh\u1EB1m t\u00F4n vinh kh\u1EA3 n\u0103ng ng\u00F4n ng\u1EEF, b\u1EA3n s\u1EAFc d\u00E2n t\u1ED9c v\u00E0 tinh th\u1EA7n h\u1ED9i nh\u1EADp to\u00E0n c\u1EA7u.\n\n\uD83D\uDCA1 N\u1ED9i dung n\u1ED5i b\u1EADt\n\nV\u00F2ng lo\u1EA1i: Thi k\u1EF9 n\u0103ng ng\u00F4n ng\u1EEF (Anh, Nh\u1EADt, Trung, H\u00E0n).\nV\u00F2ng b\u00E1n k\u1EBFt: Th\u1EC3 hi\u1EC7n n\u0103ng l\u1EF1c s\u00E1ng t\u1EA1o qua video ho\u1EB7c b\u00E0i thuy\u1EBFt tr\u00ECnh v\u1EC1 ch\u1EE7 \u0111\u1EC1 v\u0103n ho\u00E1.\nV\u00F2ng chung k\u1EBFt: Bi\u1EC3u di\u1EC5n ngh\u1EC7 thu\u1EADt, tranh bi\u1EC7n, v\u00E0 showcase v\u0103n ho\u00E1 d\u00E2n gian k\u1EBFt h\u1EE3p y\u1EBFu t\u1ED1 hi\u1EC7n \u0111\u1EA1i.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nG\u1EAFn k\u1EBFt c\u1ED9ng \u0111\u1ED3ng h\u1ECDc sinh, sinh vi\u00EAn FPT tr\u00EAn to\u00E0n qu\u1ED1c.\nKhuy\u1EBFn kh\u00EDch tinh th\u1EA7n h\u1ECDc t\u1EADp ngo\u1EA1i ng\u1EEF v\u00E0 t\u00ECm hi\u1EC3u v\u0103n ho\u00E1 qu\u1ED1c t\u1EBF.\nT\u1EA1o s\u00E2n ch\u01A1i h\u1ECDc thu\u1EADt s\u00E1ng t\u1EA1o, lan to\u1EA3 gi\u00E1 tr\u1ECB v\u0103n ho\u00E1 Vi\u1EC7t Nam.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng\n\n\uD83E\uDD47 Qu\u00E1n qu\u00E2n to\u00E0n qu\u1ED1c: 10.000.000 VN\u0110 \u002B H\u1ECDc b\u1ED5ng ngo\u1EA1i ng\u1EEF.\n\uD83E\uDD48 Gi\u1EA3i s\u00E1ng t\u1EA1o: 5.000.000 VN\u0110 \u002B K\u1EF7 ni\u1EC7m ch\u01B0\u01A1ng FPT La\u2019Fest.\n\uD83E\uDD49 Gi\u1EA3i v\u0103n ho\u00E1 \u1EA5n t\u01B0\u1EE3ng: 3.000.000 VN\u0110 \u002B qu\u00E0 t\u1EB7ng t\u1EEB nh\u00E0 t\u00E0i tr\u1EE3.\n\n\uD83C\uDFA4 Ng\u00F4n ng\u1EEF cu\u1ED9c thi\n\nTi\u1EBFng Anh \uD83C\uDDEC\uD83C\uDDE7\nTi\u1EBFng Nh\u1EADt \uD83C\uDDEF\uD83C\uDDF5\nTi\u1EBFng Trung \uD83C\uDDE8\uD83C\uDDF3\nTi\u1EBFng H\u00E0n \uD83C\uDDF0\uD83C\uDDF7\n\n\uD83D\uDCF2 \u0110\u0103ng k\u00FD \u0026 Th\u00F4ng tin chi ti\u1EBFt\n\n\uD83D\uDC49 Qu\u00E9t QR Code tr\u00EAn poster ho\u1EB7c truy c\u1EADp trang web n\u1ED9i b\u1ED9 FPT Edu.","EventDetails":"\uD83C\uDF0F Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh:\n\n\u201CNg\u00F4n ng\u1EEF m\u1EDF l\u1ED1i \u2013 V\u0103n ho\u00E1 d\u1EABn \u0111\u01B0\u1EDDng.\u201D\n\nFPT La\u2019Fest 2025 l\u00E0 cu\u1ED9c thi ng\u00F4n ng\u1EEF v\u00E0 v\u0103n ho\u00E1 li\u00EAn tr\u01B0\u1EDDng d\u00E0nh cho h\u1ECDc sinh, sinh vi\u00EAn thu\u1ED9c h\u1EC7 th\u1ED1ng gi\u00E1o d\u1EE5c FPT, nh\u1EB1m t\u00F4n vinh kh\u1EA3 n\u0103ng ng\u00F4n ng\u1EEF, b\u1EA3n s\u1EAFc d\u00E2n t\u1ED9c v\u00E0 tinh th\u1EA7n h\u1ED9i nh\u1EADp to\u00E0n c\u1EA7u.\n\n\uD83D\uDCA1 N\u1ED9i dung n\u1ED5i b\u1EADt\n\nV\u00F2ng lo\u1EA1i: Thi k\u1EF9 n\u0103ng ng\u00F4n ng\u1EEF (Anh, Nh\u1EADt, Trung, H\u00E0n).\nV\u00F2ng b\u00E1n k\u1EBFt: Th\u1EC3 hi\u1EC7n n\u0103ng l\u1EF1c s\u00E1ng t\u1EA1o qua video ho\u1EB7c b\u00E0i thuy\u1EBFt tr\u00ECnh v\u1EC1 ch\u1EE7 \u0111\u1EC1 v\u0103n ho\u00E1.\nV\u00F2ng chung k\u1EBFt: Bi\u1EC3u di\u1EC5n ngh\u1EC7 thu\u1EADt, tranh bi\u1EC7n, v\u00E0 showcase v\u0103n ho\u00E1 d\u00E2n gian k\u1EBFt h\u1EE3p y\u1EBFu t\u1ED1 hi\u1EC7n \u0111\u1EA1i.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nG\u1EAFn k\u1EBFt c\u1ED9ng \u0111\u1ED3ng h\u1ECDc sinh, sinh vi\u00EAn FPT tr\u00EAn to\u00E0n qu\u1ED1c.\nKhuy\u1EBFn kh\u00EDch tinh th\u1EA7n h\u1ECDc t\u1EADp ngo\u1EA1i ng\u1EEF v\u00E0 t\u00ECm hi\u1EC3u v\u0103n ho\u00E1 qu\u1ED1c t\u1EBF.\nT\u1EA1o s\u00E2n ch\u01A1i h\u1ECDc thu\u1EADt s\u00E1ng t\u1EA1o, lan to\u1EA3 gi\u00E1 tr\u1ECB v\u0103n ho\u00E1 Vi\u1EC7t Nam.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng\n\n\uD83E\uDD47 Qu\u00E1n qu\u00E2n to\u00E0n qu\u1ED1c: 10.000.000 VN\u0110 \u002B H\u1ECDc b\u1ED5ng ngo\u1EA1i ng\u1EEF.\n\uD83E\uDD48 Gi\u1EA3i s\u00E1ng t\u1EA1o: 5.000.000 VN\u0110 \u002B K\u1EF7 ni\u1EC7m ch\u01B0\u01A1ng FPT La\u2019Fest.\n\uD83E\uDD49 Gi\u1EA3i v\u0103n ho\u00E1 \u1EA5n t\u01B0\u1EE3ng: 3.000.000 VN\u0110 \u002B qu\u00E0 t\u1EB7ng t\u1EEB nh\u00E0 t\u00E0i tr\u1EE3.\n\n\uD83C\uDFA4 Ng\u00F4n ng\u1EEF cu\u1ED9c thi\n\nTi\u1EBFng Anh \uD83C\uDDEC\uD83C\uDDE7\nTi\u1EBFng Nh\u1EADt \uD83C\uDDEF\uD83C\uDDF5\nTi\u1EBFng Trung \uD83C\uDDE8\uD83C\uDDF3\nTi\u1EBFng H\u00E0n \uD83C\uDDF0\uD83C\uDDF7\n\n\uD83D\uDCF2 \u0110\u0103ng k\u00FD \u0026 Th\u00F4ng tin chi ti\u1EBFt\n\n\uD83D\uDC49 Qu\u00E9t QR Code tr\u00EAn poster ho\u1EB7c truy c\u1EADp trang web n\u1ED9i b\u1ED9 FPT Edu.","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/6bdc3205-51c7-48d9-b557-7a3c41d85670.png","OrganizerName":"FPT Education","OrganizerInfo":"none"}', N'{"HasVirtualStage":true,"CanvasWidth":1000,"CanvasHeight":700,"Areas":[{"Id":"area_1761745337391","Name":"Khu v\u1EF1c 1","Shape":"rectangle","Coordinates":[{"X":174,"Y":70.3125},{"X":859,"Y":70.3125},{"X":859,"Y":322.3125},{"X":174,"Y":322.3125}],"Color":"#667eea","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761745339001","Name":"Khu v\u1EF1c 2","Shape":"rectangle","Coordinates":[{"X":155,"Y":404.3125},{"X":302,"Y":404.3125},{"X":302,"Y":532.3125},{"X":155,"Y":532.3125}],"Color":"#764ba2","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761745340095","Name":"Khu v\u1EF1c 3","Shape":"rectangle","Coordinates":[{"X":468,"Y":419.3125},{"X":570,"Y":419.3125},{"X":570,"Y":524.3125},{"X":468,"Y":524.3125}],"Color":"#f093fb","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""},{"Id":"area_1761745340970","Name":"Khu v\u1EF1c 4","Shape":"rectangle","Coordinates":[{"X":726,"Y":401.3125},{"X":877,"Y":401.3125},{"X":877,"Y":544.3125},{"X":726,"Y":544.3125}],"Color":"#4facfe","TicketTypeId":null,"IsStanding":false,"Capacity":null,"Label":""}]}', GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (24, 1, N'FPT KHĂN VAS 2025', N'🧣 Sự kiện: FPT KHĂN VAS 2025

Đơn vị tổ chức: Khối Giáo dục FPT (FPT University, FPT Polytechnic, FPT High School, FPT Academy International, BTEC, Greenwich Vietnam, Swinburne Vietnam...)

Thời gian:

📅 Hạn chót đăng ký & nộp bài: 10/11/2025
🧵 Công bố & trưng bày tác phẩm: 20/11/2025
Địa điểm: Toàn hệ thống FPT Education trên toàn quốc

💐 Chủ đề chính

“Khăn gửi tri ân – Vải dệt yêu thương.”
FPT Khăn Vải 2025 là cuộc thi thiết kế khăn vải sáng tạo chào mừng 43 năm Ngày Nhà giáo Việt Nam 20/11, nhằm tôn vinh thầy cô giáo, khuyến khích tinh thần nghệ thuật và lòng biết ơn trong cộng đồng sinh viên FPT.

🎨 Nội dung nổi bật

Thiết kế sáng tạo khăn vải mang biểu tượng FPT và thông điệp tri ân.
Workshop mini: Hướng dẫn sử dụng phần mềm thiết kế (Adobe Illustrator, Canva, Figma).
Triển lãm trực tuyến: Trưng bày các mẫu khăn được bình chọn cao nhất trên nền tảng Threads City.

🎯 Mục tiêu sự kiện

Góp phần lan tỏa tinh thần tri ân Ngày Nhà giáo Việt Nam.
Khơi gợi sự sáng tạo, cảm xúc nghệ thuật trong sinh viên.
Tạo cơ hội để ý tưởng sinh viên được sản xuất thực tế và gửi tặng thầy cô.

🏆 Giải thưởng & Vinh danh

🥇 430 bài dự thi sớm nhất: được sản xuất thật và gửi tặng đến thầy cô trong ngày 20/11.
🥈 20 giải yêu thích nhất: bình chọn bởi cư dân Threads City – nhận quà lưu niệm đặc biệt.
🧣 Tác phẩm nổi bật được giới thiệu trong triển lãm “Khăn Vải FPT” toàn quốc.

🧵 Hình thức tham gia

Sinh viên đăng ký & nộp thiết kế qua QR code trên poster.
File nộp: PNG/JPG hoặc bản thiết kế gốc (AI/PSD).
Kích thước chuẩn: 60x60 cm – chủ đề tự do, khuyến khích yếu tố truyền thống & FPT branding.

💬 Thông điệp
“Một chiếc khăn, một lời tri ân.
Từ nét vẽ của sinh viên – đến trái tim người thầy.”

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), NULL, N'Public', 'Offline', 'Art', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i H\u1ECDc FPT H\u00E0 N\u1ED9i","StreetAddress":"123 \u0110\u01B0\u1EDDng L\u00EA Du\u1EA9n","Province":"H\u00E0 N\u1ED9i","District":"Qu\u1EADn Ba \u0110\u00ECnh","Ward":"Ph\u01B0\u1EDDng Li\u1EC5u Giai","EventImage":"/assets/images/events/khan_vas.png","BackgroundImage":"/assets/images/events/khan_vas.png","EventIntroduction":"\uD83E\uDDE3 S\u1EF1 ki\u1EC7n: FPT KH\u0102N VAS 2025\n\n\u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c: Kh\u1ED1i Gi\u00E1o d\u1EE5c FPT (FPT University, FPT Polytechnic, FPT High School, FPT Academy International, BTEC, Greenwich Vietnam, Swinburne Vietnam...)\n\nTh\u1EDDi gian:\n\n\uD83D\uDCC5 H\u1EA1n ch\u00F3t \u0111\u0103ng k\u00FD \u0026 n\u1ED9p b\u00E0i: 10/11/2025\n\uD83E\uDDF5 C\u00F4ng b\u1ED1 \u0026 tr\u01B0ng b\u00E0y t\u00E1c ph\u1EA9m: 20/11/2025\n\u0110\u1ECBa \u0111i\u1EC3m: To\u00E0n h\u1EC7 th\u1ED1ng FPT Education tr\u00EAn to\u00E0n qu\u1ED1c\n\n\uD83D\uDC90 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\n\u201CKh\u0103n g\u1EEDi tri \u00E2n \u2013 V\u1EA3i d\u1EC7t y\u00EAu th\u01B0\u01A1ng.\u201D\nFPT Kh\u0103n V\u1EA3i 2025 l\u00E0 cu\u1ED9c thi thi\u1EBFt k\u1EBF kh\u0103n v\u1EA3i s\u00E1ng t\u1EA1o ch\u00E0o m\u1EEBng 43 n\u0103m Ng\u00E0y Nh\u00E0 gi\u00E1o Vi\u1EC7t Nam 20/11, nh\u1EB1m t\u00F4n vinh th\u1EA7y c\u00F4 gi\u00E1o, khuy\u1EBFn kh\u00EDch tinh th\u1EA7n ngh\u1EC7 thu\u1EADt v\u00E0 l\u00F2ng bi\u1EBFt \u01A1n trong c\u1ED9ng \u0111\u1ED3ng sinh vi\u00EAn FPT.\n\n\uD83C\uDFA8 N\u1ED9i dung n\u1ED5i b\u1EADt\n\nThi\u1EBFt k\u1EBF s\u00E1ng t\u1EA1o kh\u0103n v\u1EA3i mang bi\u1EC3u t\u01B0\u1EE3ng FPT v\u00E0 th\u00F4ng \u0111i\u1EC7p tri \u00E2n.\nWorkshop mini: H\u01B0\u1EDBng d\u1EABn s\u1EED d\u1EE5ng ph\u1EA7n m\u1EC1m thi\u1EBFt k\u1EBF (Adobe Illustrator, Canva, Figma).\nTri\u1EC3n l\u00E3m tr\u1EF1c tuy\u1EBFn: Tr\u01B0ng b\u00E0y c\u00E1c m\u1EABu kh\u0103n \u0111\u01B0\u1EE3c b\u00ECnh ch\u1ECDn cao nh\u1EA5t tr\u00EAn n\u1EC1n t\u1EA3ng Threads City.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nG\u00F3p ph\u1EA7n lan t\u1ECFa tinh th\u1EA7n tri \u00E2n Ng\u00E0y Nh\u00E0 gi\u00E1o Vi\u1EC7t Nam.\nKh\u01A1i g\u1EE3i s\u1EF1 s\u00E1ng t\u1EA1o, c\u1EA3m x\u00FAc ngh\u1EC7 thu\u1EADt trong sinh vi\u00EAn.\nT\u1EA1o c\u01A1 h\u1ED9i \u0111\u1EC3 \u00FD t\u01B0\u1EDFng sinh vi\u00EAn \u0111\u01B0\u1EE3c s\u1EA3n xu\u1EA5t th\u1EF1c t\u1EBF v\u00E0 g\u1EEDi t\u1EB7ng th\u1EA7y c\u00F4.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng \u0026 Vinh danh\n\n\uD83E\uDD47 430 b\u00E0i d\u1EF1 thi s\u1EDBm nh\u1EA5t: \u0111\u01B0\u1EE3c s\u1EA3n xu\u1EA5t th\u1EADt v\u00E0 g\u1EEDi t\u1EB7ng \u0111\u1EBFn th\u1EA7y c\u00F4 trong ng\u00E0y 20/11.\n\uD83E\uDD48 20 gi\u1EA3i y\u00EAu th\u00EDch nh\u1EA5t: b\u00ECnh ch\u1ECDn b\u1EDFi c\u01B0 d\u00E2n Threads City \u2013 nh\u1EADn qu\u00E0 l\u01B0u ni\u1EC7m \u0111\u1EB7c bi\u1EC7t.\n\uD83E\uDDE3 T\u00E1c ph\u1EA9m n\u1ED5i b\u1EADt \u0111\u01B0\u1EE3c gi\u1EDBi thi\u1EC7u trong tri\u1EC3n l\u00E3m \u201CKh\u0103n V\u1EA3i FPT\u201D to\u00E0n qu\u1ED1c.\n\n\uD83E\uDDF5 H\u00ECnh th\u1EE9c tham gia\n\nSinh vi\u00EAn \u0111\u0103ng k\u00FD \u0026 n\u1ED9p thi\u1EBFt k\u1EBF qua QR code tr\u00EAn poster.\nFile n\u1ED9p: PNG/JPG ho\u1EB7c b\u1EA3n thi\u1EBFt k\u1EBF g\u1ED1c (AI/PSD).\nK\u00EDch th\u01B0\u1EDBc chu\u1EA9n: 60x60 cm \u2013 ch\u1EE7 \u0111\u1EC1 t\u1EF1 do, khuy\u1EBFn kh\u00EDch y\u1EBFu t\u1ED1 truy\u1EC1n th\u1ED1ng \u0026 FPT branding.\n\n\uD83D\uDCAC Th\u00F4ng \u0111i\u1EC7p\n\u201CM\u1ED9t chi\u1EBFc kh\u0103n, m\u1ED9t l\u1EDDi tri \u00E2n.\nT\u1EEB n\u00E9t v\u1EBD c\u1EE7a sinh vi\u00EAn \u2013 \u0111\u1EBFn tr\u00E1i tim ng\u01B0\u1EDDi th\u1EA7y.\u201D","EventDetails":"\uD83E\uDDE3 S\u1EF1 ki\u1EC7n: FPT KH\u0102N VAS 2025\n\n\u0110\u01A1n v\u1ECB t\u1ED5 ch\u1EE9c: Kh\u1ED1i Gi\u00E1o d\u1EE5c FPT (FPT University, FPT Polytechnic, FPT High School, FPT Academy International, BTEC, Greenwich Vietnam, Swinburne Vietnam...)\n\nTh\u1EDDi gian:\n\n\uD83D\uDCC5 H\u1EA1n ch\u00F3t \u0111\u0103ng k\u00FD \u0026 n\u1ED9p b\u00E0i: 10/11/2025\n\uD83E\uDDF5 C\u00F4ng b\u1ED1 \u0026 tr\u01B0ng b\u00E0y t\u00E1c ph\u1EA9m: 20/11/2025\n\u0110\u1ECBa \u0111i\u1EC3m: To\u00E0n h\u1EC7 th\u1ED1ng FPT Education tr\u00EAn to\u00E0n qu\u1ED1c\n\n\uD83D\uDC90 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\n\u201CKh\u0103n g\u1EEDi tri \u00E2n \u2013 V\u1EA3i d\u1EC7t y\u00EAu th\u01B0\u01A1ng.\u201D\nFPT Kh\u0103n V\u1EA3i 2025 l\u00E0 cu\u1ED9c thi thi\u1EBFt k\u1EBF kh\u0103n v\u1EA3i s\u00E1ng t\u1EA1o ch\u00E0o m\u1EEBng 43 n\u0103m Ng\u00E0y Nh\u00E0 gi\u00E1o Vi\u1EC7t Nam 20/11, nh\u1EB1m t\u00F4n vinh th\u1EA7y c\u00F4 gi\u00E1o, khuy\u1EBFn kh\u00EDch tinh th\u1EA7n ngh\u1EC7 thu\u1EADt v\u00E0 l\u00F2ng bi\u1EBFt \u01A1n trong c\u1ED9ng \u0111\u1ED3ng sinh vi\u00EAn FPT.\n\n\uD83C\uDFA8 N\u1ED9i dung n\u1ED5i b\u1EADt\n\nThi\u1EBFt k\u1EBF s\u00E1ng t\u1EA1o kh\u0103n v\u1EA3i mang bi\u1EC3u t\u01B0\u1EE3ng FPT v\u00E0 th\u00F4ng \u0111i\u1EC7p tri \u00E2n.\nWorkshop mini: H\u01B0\u1EDBng d\u1EABn s\u1EED d\u1EE5ng ph\u1EA7n m\u1EC1m thi\u1EBFt k\u1EBF (Adobe Illustrator, Canva, Figma).\nTri\u1EC3n l\u00E3m tr\u1EF1c tuy\u1EBFn: Tr\u01B0ng b\u00E0y c\u00E1c m\u1EABu kh\u0103n \u0111\u01B0\u1EE3c b\u00ECnh ch\u1ECDn cao nh\u1EA5t tr\u00EAn n\u1EC1n t\u1EA3ng Threads City.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nG\u00F3p ph\u1EA7n lan t\u1ECFa tinh th\u1EA7n tri \u00E2n Ng\u00E0y Nh\u00E0 gi\u00E1o Vi\u1EC7t Nam.\nKh\u01A1i g\u1EE3i s\u1EF1 s\u00E1ng t\u1EA1o, c\u1EA3m x\u00FAc ngh\u1EC7 thu\u1EADt trong sinh vi\u00EAn.\nT\u1EA1o c\u01A1 h\u1ED9i \u0111\u1EC3 \u00FD t\u01B0\u1EDFng sinh vi\u00EAn \u0111\u01B0\u1EE3c s\u1EA3n xu\u1EA5t th\u1EF1c t\u1EBF v\u00E0 g\u1EEDi t\u1EB7ng th\u1EA7y c\u00F4.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng \u0026 Vinh danh\n\n\uD83E\uDD47 430 b\u00E0i d\u1EF1 thi s\u1EDBm nh\u1EA5t: \u0111\u01B0\u1EE3c s\u1EA3n xu\u1EA5t th\u1EADt v\u00E0 g\u1EEDi t\u1EB7ng \u0111\u1EBFn th\u1EA7y c\u00F4 trong ng\u00E0y 20/11.\n\uD83E\uDD48 20 gi\u1EA3i y\u00EAu th\u00EDch nh\u1EA5t: b\u00ECnh ch\u1ECDn b\u1EDFi c\u01B0 d\u00E2n Threads City \u2013 nh\u1EADn qu\u00E0 l\u01B0u ni\u1EC7m \u0111\u1EB7c bi\u1EC7t.\n\uD83E\uDDE3 T\u00E1c ph\u1EA9m n\u1ED5i b\u1EADt \u0111\u01B0\u1EE3c gi\u1EDBi thi\u1EC7u trong tri\u1EC3n l\u00E3m \u201CKh\u0103n V\u1EA3i FPT\u201D to\u00E0n qu\u1ED1c.\n\n\uD83E\uDDF5 H\u00ECnh th\u1EE9c tham gia\n\nSinh vi\u00EAn \u0111\u0103ng k\u00FD \u0026 n\u1ED9p thi\u1EBFt k\u1EBF qua QR code tr\u00EAn poster.\nFile n\u1ED9p: PNG/JPG ho\u1EB7c b\u1EA3n thi\u1EBFt k\u1EBF g\u1ED1c (AI/PSD).\nK\u00EDch th\u01B0\u1EDBc chu\u1EA9n: 60x60 cm \u2013 ch\u1EE7 \u0111\u1EC1 t\u1EF1 do, khuy\u1EBFn kh\u00EDch y\u1EBFu t\u1ED1 truy\u1EC1n th\u1ED1ng \u0026 FPT branding.\n\n\uD83D\uDCAC Th\u00F4ng \u0111i\u1EC7p\n\u201CM\u1ED9t chi\u1EBFc kh\u0103n, m\u1ED9t l\u1EDDi tri \u00E2n.\nT\u1EEB n\u00E9t v\u1EBD c\u1EE7a sinh vi\u00EAn \u2013 \u0111\u1EBFn tr\u00E1i tim ng\u01B0\u1EDDi th\u1EA7y.\u201D","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/f472718a-f761-4067-b4d0-08760d2106ae.png","OrganizerName":"FPT Education","OrganizerInfo":"none"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (25, 1, N'F-CODE TALENT 2025', N'🚀 Chủ đề chính

“Code the Future – Giải mã tài năng, bứt phá giới hạn.”

F-Code Talent 2025 là cuộc thi học thuật hàng đầu dành cho sinh viên ngành Software Engineering (SE) của Đại học FPT, nhằm tìm kiếm những gương mặt lập trình viên trẻ xuất sắc, có khả năng tư duy thuật toán, teamwork và phát triển sản phẩm sáng tạo.

💡 Nội dung & Cấu trúc cuộc thi

Vòng 1 – Code Challenge:

Thi lập trình thuật toán trên nền tảng trực tuyến (LeetCode format).
Thời gian: 90 phút – Cá nhân thi độc lập.

Vòng 2 – Debug Battle:

Đội 2-3 người cùng giải quyết lỗi trong project thật.
Đánh giá tốc độ phân tích, teamwork và cách trình bày giải pháp.

Vòng 3 – Product Pitching:

Mỗi đội trình bày ý tưởng sản phẩm phần mềm trước hội đồng.
Tiêu chí: Tính sáng tạo, khả năng triển khai, tiềm năng ứng dụng thực tế.

🎯 Mục tiêu sự kiện

Phát hiện và bồi dưỡng tài năng lập trình trẻ của FPTU.
Tạo môi trường thực hành mô phỏng doanh nghiệp cho sinh viên SE.
Kết nối sinh viên với các chuyên gia từ FPT Software, Axon Active, và các đối tác công nghệ.

🏆 Giải thưởng hấp dẫn

🥇 F-Code Champion 2025: 5.000.000 VNĐ + Cúp lưu niệm + Cơ hội Internship tại FPT Software.
🥈 Best Debugger Award: 3.000.000 VNĐ + Giấy chứng nhận.
🥉 Innovation Prize: 2.000.000 VNĐ + Quà công nghệ (mechanical keyboard, headset, v.v).
🎖️ Tất cả thí sinh lọt Top 10 được cấp chứng nhận “SE Talent 2025” từ CLB F-Code.

👨‍💻 Ban giám khảo & khách mời

Thầy Trần Đức Anh – Trưởng Bộ môn Kỹ thuật phần mềm, FPT University.
Anh Nguyễn Quốc Toàn – Software Architect, FPT Software.
Chị Lê Bảo Trâm – Scrum Master, Axon Active Vietnam.

📲 Cách tham gia

Đăng ký tại fcode.fpt.edu.vn/talent2025 hoặc quét QR code trên poster.
Hình thức thi: kết hợp trực tiếp & trực tuyến.
Đối tượng: Sinh viên ngành Software Engineering từ năm 1–4.


', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), NULL, N'Public', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0110\u1EA1i h\u1ECDc FPT H\u00E0 N\u1ED9i","StreetAddress":"123 \u0110\u01B0\u1EDDng L\u00EA Du\u1EA9n","Province":"H\u00E0 N\u1ED9i","District":"Qu\u1EADn Ba \u0110\u00ECnh","Ward":"Ph\u01B0\u1EDDng Li\u1EC5u Giai","EventImage":"/assets/images/events/f_talent_code.png","BackgroundImage":"/assets/images/events/f_talent_code.png","EventIntroduction":"\uD83D\uDE80 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\n\u201CCode the Future \u2013 Gi\u1EA3i m\u00E3 t\u00E0i n\u0103ng, b\u1EE9t ph\u00E1 gi\u1EDBi h\u1EA1n.\u201D\n\nF-Code Talent 2025 l\u00E0 cu\u1ED9c thi h\u1ECDc thu\u1EADt h\u00E0ng \u0111\u1EA7u d\u00E0nh cho sinh vi\u00EAn ng\u00E0nh Software Engineering (SE) c\u1EE7a \u0110\u1EA1i h\u1ECDc FPT, nh\u1EB1m t\u00ECm ki\u1EBFm nh\u1EEFng g\u01B0\u01A1ng m\u1EB7t l\u1EADp tr\u00ECnh vi\u00EAn tr\u1EBB xu\u1EA5t s\u1EAFc, c\u00F3 kh\u1EA3 n\u0103ng t\u01B0 duy thu\u1EADt to\u00E1n, teamwork v\u00E0 ph\u00E1t tri\u1EC3n s\u1EA3n ph\u1EA9m s\u00E1ng t\u1EA1o.\n\n\uD83D\uDCA1 N\u1ED9i dung \u0026 C\u1EA5u tr\u00FAc cu\u1ED9c thi\n\nV\u00F2ng 1 \u2013 Code Challenge:\n\nThi l\u1EADp tr\u00ECnh thu\u1EADt to\u00E1n tr\u00EAn n\u1EC1n t\u1EA3ng tr\u1EF1c tuy\u1EBFn (LeetCode format).\nTh\u1EDDi gian: 90 ph\u00FAt \u2013 C\u00E1 nh\u00E2n thi \u0111\u1ED9c l\u1EADp.\n\nV\u00F2ng 2 \u2013 Debug Battle:\n\n\u0110\u1ED9i 2-3 ng\u01B0\u1EDDi c\u00F9ng gi\u1EA3i quy\u1EBFt l\u1ED7i trong project th\u1EADt.\n\u0110\u00E1nh gi\u00E1 t\u1ED1c \u0111\u1ED9 ph\u00E2n t\u00EDch, teamwork v\u00E0 c\u00E1ch tr\u00ECnh b\u00E0y gi\u1EA3i ph\u00E1p.\n\nV\u00F2ng 3 \u2013 Product Pitching:\n\nM\u1ED7i \u0111\u1ED9i tr\u00ECnh b\u00E0y \u00FD t\u01B0\u1EDFng s\u1EA3n ph\u1EA9m ph\u1EA7n m\u1EC1m tr\u01B0\u1EDBc h\u1ED9i \u0111\u1ED3ng.\nTi\u00EAu ch\u00ED: T\u00EDnh s\u00E1ng t\u1EA1o, kh\u1EA3 n\u0103ng tri\u1EC3n khai, ti\u1EC1m n\u0103ng \u1EE9ng d\u1EE5ng th\u1EF1c t\u1EBF.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nPh\u00E1t hi\u1EC7n v\u00E0 b\u1ED3i d\u01B0\u1EE1ng t\u00E0i n\u0103ng l\u1EADp tr\u00ECnh tr\u1EBB c\u1EE7a FPTU.\nT\u1EA1o m\u00F4i tr\u01B0\u1EDDng th\u1EF1c h\u00E0nh m\u00F4 ph\u1ECFng doanh nghi\u1EC7p cho sinh vi\u00EAn SE.\nK\u1EBFt n\u1ED1i sinh vi\u00EAn v\u1EDBi c\u00E1c chuy\u00EAn gia t\u1EEB FPT Software, Axon Active, v\u00E0 c\u00E1c \u0111\u1ED1i t\u00E1c c\u00F4ng ngh\u1EC7.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng h\u1EA5p d\u1EABn\n\n\uD83E\uDD47 F-Code Champion 2025: 5.000.000 VN\u0110 \u002B C\u00FAp l\u01B0u ni\u1EC7m \u002B C\u01A1 h\u1ED9i Internship t\u1EA1i FPT Software.\n\uD83E\uDD48 Best Debugger Award: 3.000.000 VN\u0110 \u002B Gi\u1EA5y ch\u1EE9ng nh\u1EADn.\n\uD83E\uDD49 Innovation Prize: 2.000.000 VN\u0110 \u002B Qu\u00E0 c\u00F4ng ngh\u1EC7 (mechanical keyboard, headset, v.v).\n\uD83C\uDF96\uFE0F T\u1EA5t c\u1EA3 th\u00ED sinh l\u1ECDt Top 10 \u0111\u01B0\u1EE3c c\u1EA5p ch\u1EE9ng nh\u1EADn \u201CSE Talent 2025\u201D t\u1EEB CLB F-Code.\n\n\uD83D\uDC68\u200D\uD83D\uDCBB Ban gi\u00E1m kh\u1EA3o \u0026 kh\u00E1ch m\u1EDDi\n\nTh\u1EA7y Tr\u1EA7n \u0110\u1EE9c Anh \u2013 Tr\u01B0\u1EDFng B\u1ED9 m\u00F4n K\u1EF9 thu\u1EADt ph\u1EA7n m\u1EC1m, FPT University.\nAnh Nguy\u1EC5n Qu\u1ED1c To\u00E0n \u2013 Software Architect, FPT Software.\nCh\u1ECB L\u00EA B\u1EA3o Tr\u00E2m \u2013 Scrum Master, Axon Active Vietnam.\n\n\uD83D\uDCF2 C\u00E1ch tham gia\n\n\u0110\u0103ng k\u00FD t\u1EA1i fcode.fpt.edu.vn/talent2025 ho\u1EB7c qu\u00E9t QR code tr\u00EAn poster.\nH\u00ECnh th\u1EE9c thi: k\u1EBFt h\u1EE3p tr\u1EF1c ti\u1EBFp \u0026 tr\u1EF1c tuy\u1EBFn.\n\u0110\u1ED1i t\u01B0\u1EE3ng: Sinh vi\u00EAn ng\u00E0nh Software Engineering t\u1EEB n\u0103m 1\u20134.\n","EventDetails":"\uD83D\uDE80 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\n\u201CCode the Future \u2013 Gi\u1EA3i m\u00E3 t\u00E0i n\u0103ng, b\u1EE9t ph\u00E1 gi\u1EDBi h\u1EA1n.\u201D\n\nF-Code Talent 2025 l\u00E0 cu\u1ED9c thi h\u1ECDc thu\u1EADt h\u00E0ng \u0111\u1EA7u d\u00E0nh cho sinh vi\u00EAn ng\u00E0nh Software Engineering (SE) c\u1EE7a \u0110\u1EA1i h\u1ECDc FPT, nh\u1EB1m t\u00ECm ki\u1EBFm nh\u1EEFng g\u01B0\u01A1ng m\u1EB7t l\u1EADp tr\u00ECnh vi\u00EAn tr\u1EBB xu\u1EA5t s\u1EAFc, c\u00F3 kh\u1EA3 n\u0103ng t\u01B0 duy thu\u1EADt to\u00E1n, teamwork v\u00E0 ph\u00E1t tri\u1EC3n s\u1EA3n ph\u1EA9m s\u00E1ng t\u1EA1o.\n\n\uD83D\uDCA1 N\u1ED9i dung \u0026 C\u1EA5u tr\u00FAc cu\u1ED9c thi\n\nV\u00F2ng 1 \u2013 Code Challenge:\n\nThi l\u1EADp tr\u00ECnh thu\u1EADt to\u00E1n tr\u00EAn n\u1EC1n t\u1EA3ng tr\u1EF1c tuy\u1EBFn (LeetCode format).\nTh\u1EDDi gian: 90 ph\u00FAt \u2013 C\u00E1 nh\u00E2n thi \u0111\u1ED9c l\u1EADp.\n\nV\u00F2ng 2 \u2013 Debug Battle:\n\n\u0110\u1ED9i 2-3 ng\u01B0\u1EDDi c\u00F9ng gi\u1EA3i quy\u1EBFt l\u1ED7i trong project th\u1EADt.\n\u0110\u00E1nh gi\u00E1 t\u1ED1c \u0111\u1ED9 ph\u00E2n t\u00EDch, teamwork v\u00E0 c\u00E1ch tr\u00ECnh b\u00E0y gi\u1EA3i ph\u00E1p.\n\nV\u00F2ng 3 \u2013 Product Pitching:\n\nM\u1ED7i \u0111\u1ED9i tr\u00ECnh b\u00E0y \u00FD t\u01B0\u1EDFng s\u1EA3n ph\u1EA9m ph\u1EA7n m\u1EC1m tr\u01B0\u1EDBc h\u1ED9i \u0111\u1ED3ng.\nTi\u00EAu ch\u00ED: T\u00EDnh s\u00E1ng t\u1EA1o, kh\u1EA3 n\u0103ng tri\u1EC3n khai, ti\u1EC1m n\u0103ng \u1EE9ng d\u1EE5ng th\u1EF1c t\u1EBF.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nPh\u00E1t hi\u1EC7n v\u00E0 b\u1ED3i d\u01B0\u1EE1ng t\u00E0i n\u0103ng l\u1EADp tr\u00ECnh tr\u1EBB c\u1EE7a FPTU.\nT\u1EA1o m\u00F4i tr\u01B0\u1EDDng th\u1EF1c h\u00E0nh m\u00F4 ph\u1ECFng doanh nghi\u1EC7p cho sinh vi\u00EAn SE.\nK\u1EBFt n\u1ED1i sinh vi\u00EAn v\u1EDBi c\u00E1c chuy\u00EAn gia t\u1EEB FPT Software, Axon Active, v\u00E0 c\u00E1c \u0111\u1ED1i t\u00E1c c\u00F4ng ngh\u1EC7.\n\n\uD83C\uDFC6 Gi\u1EA3i th\u01B0\u1EDFng h\u1EA5p d\u1EABn\n\n\uD83E\uDD47 F-Code Champion 2025: 5.000.000 VN\u0110 \u002B C\u00FAp l\u01B0u ni\u1EC7m \u002B C\u01A1 h\u1ED9i Internship t\u1EA1i FPT Software.\n\uD83E\uDD48 Best Debugger Award: 3.000.000 VN\u0110 \u002B Gi\u1EA5y ch\u1EE9ng nh\u1EADn.\n\uD83E\uDD49 Innovation Prize: 2.000.000 VN\u0110 \u002B Qu\u00E0 c\u00F4ng ngh\u1EC7 (mechanical keyboard, headset, v.v).\n\uD83C\uDF96\uFE0F T\u1EA5t c\u1EA3 th\u00ED sinh l\u1ECDt Top 10 \u0111\u01B0\u1EE3c c\u1EA5p ch\u1EE9ng nh\u1EADn \u201CSE Talent 2025\u201D t\u1EEB CLB F-Code.\n\n\uD83D\uDC68\u200D\uD83D\uDCBB Ban gi\u00E1m kh\u1EA3o \u0026 kh\u00E1ch m\u1EDDi\n\nTh\u1EA7y Tr\u1EA7n \u0110\u1EE9c Anh \u2013 Tr\u01B0\u1EDFng B\u1ED9 m\u00F4n K\u1EF9 thu\u1EADt ph\u1EA7n m\u1EC1m, FPT University.\nAnh Nguy\u1EC5n Qu\u1ED1c To\u00E0n \u2013 Software Architect, FPT Software.\nCh\u1ECB L\u00EA B\u1EA3o Tr\u00E2m \u2013 Scrum Master, Axon Active Vietnam.\n\n\uD83D\uDCF2 C\u00E1ch tham gia\n\n\u0110\u0103ng k\u00FD t\u1EA1i fcode.fpt.edu.vn/talent2025 ho\u1EB7c qu\u00E9t QR code tr\u00EAn poster.\nH\u00ECnh th\u1EE9c thi: k\u1EBFt h\u1EE3p tr\u1EF1c ti\u1EBFp \u0026 tr\u1EF1c tuy\u1EBFn.\n\u0110\u1ED1i t\u01B0\u1EE3ng: Sinh vi\u00EAn ng\u00E0nh Software Engineering t\u1EEB n\u0103m 1\u20134.\n","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/792a87e4-8654-4126-9502-d561a45492eb.png","OrganizerName":"FPT Education","OrganizerInfo":"none."}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (26, 1, N'ARCANA AWAKENING – FPTU Debate Championship 2025', N'🧠 Chủ đề chính
“Arcana Awakening – Đánh thức tiềm năng, mở khóa trí tuệ.”
Arcana Awakening là chương trình tuyển thành viên Ban Tổ Chức cho cuộc thi tranh biện lớn nhất FPTU – Debate Championship 2026.
Lấy cảm hứng từ bộ bài Tarot, sự kiện mang thông điệp đánh thức những lá bài tiềm ẩn trong mỗi người, đại diện cho sức sáng tạo, khả năng phản biện và kỹ năng tổ chức ấn tượng.

💡 Nội dung tuyển chọn

Vòng đơn: Ứng viên gửi hồ sơ giới thiệu bản thân, kỹ năng và vị trí mong muốn (Logistics, Content, Media, Design, HR…).
Vòng phỏng vấn: Trao đổi trực tiếp với trưởng ban và mentor – đánh giá năng lực teamwork, leadership, và khả năng xử lý tình huống.
Kết quả: Công bố chính thức qua email & fanpage FPTU Debate Championship.

🎯 Mục tiêu sự kiện

Tìm kiếm những nhân tố trẻ đầy nhiệt huyết tham gia đội ngũ tổ chức FPTU Debate Championship 2026.
Tạo môi trường thực hành thực tế trong lĩnh vực event management, media, public speaking và tranh biện học thuật.
Bồi dưỡng kỹ năng mềm: giao tiếp, sáng tạo, quản lý dự án, làm việc nhóm.

🏆 Quyền lợi thành viên BTC

Nhận chứng nhận chính thức từ FPTU Debate Championship 2026.
Tham gia chuỗi workshop nội bộ về leadership & event planning.
Kết nối cùng các diễn giả và huấn luyện viên tranh biện chuyên nghiệp.
Cơ hội trở thành core team của những mùa Debate Championship tiếp theo.

📲 Cách tham gia

Truy cập website đăng ký BTC: [FPTU Debate Championship 2026] (qua QR code trên poster).
Điền form ứng tuyển & chờ phản hồi lịch phỏng vấn.

📧 Liên hệ Ban Tổ Chức

Head of Organization: Nguyễn Văn Hiệp – 0906 165 937
Human Resources: Vũ Kỳ Anh – 0947 031 104
Media & Design: Phạm Thị Anh Thư – 0981 703 557
Email: fptudebatechampionship@gmail.com
Fanpage: FPTU Debate Championship

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), NULL, N'Public', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Tr\u01B0\u1EDDng \u0111\u1EA1i h\u1ECDc FPT H\u00E0 N\u1ED9i","StreetAddress":"123 \u0110\u01B0\u1EDDng L\u00EA Du\u1EA9n","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng H\u1EA3i Ch\u00E2u II","EventImage":"/assets/images/events/arcana_awakening.png","BackgroundImage":"/assets/images/events/arcana_awakening.png","EventIntroduction":"\uD83E\uDDE0 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\u201CArcana Awakening \u2013 \u0110\u00E1nh th\u1EE9c ti\u1EC1m n\u0103ng, m\u1EDF kh\u00F3a tr\u00ED tu\u1EC7.\u201D\nArcana Awakening l\u00E0 ch\u01B0\u01A1ng tr\u00ECnh tuy\u1EC3n th\u00E0nh vi\u00EAn Ban T\u1ED5 Ch\u1EE9c cho cu\u1ED9c thi tranh bi\u1EC7n l\u1EDBn nh\u1EA5t FPTU \u2013 Debate Championship 2026.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB b\u1ED9 b\u00E0i Tarot, s\u1EF1 ki\u1EC7n mang th\u00F4ng \u0111i\u1EC7p \u0111\u00E1nh th\u1EE9c nh\u1EEFng l\u00E1 b\u00E0i ti\u1EC1m \u1EA9n trong m\u1ED7i ng\u01B0\u1EDDi, \u0111\u1EA1i di\u1EC7n cho s\u1EE9c s\u00E1ng t\u1EA1o, kh\u1EA3 n\u0103ng ph\u1EA3n bi\u1EC7n v\u00E0 k\u1EF9 n\u0103ng t\u1ED5 ch\u1EE9c \u1EA5n t\u01B0\u1EE3ng.\n\n\uD83D\uDCA1 N\u1ED9i dung tuy\u1EC3n ch\u1ECDn\n\nV\u00F2ng \u0111\u01A1n: \u1EE8ng vi\u00EAn g\u1EEDi h\u1ED3 s\u01A1 gi\u1EDBi thi\u1EC7u b\u1EA3n th\u00E2n, k\u1EF9 n\u0103ng v\u00E0 v\u1ECB tr\u00ED mong mu\u1ED1n (Logistics, Content, Media, Design, HR\u2026).\nV\u00F2ng ph\u1ECFng v\u1EA5n: Trao \u0111\u1ED5i tr\u1EF1c ti\u1EBFp v\u1EDBi tr\u01B0\u1EDFng ban v\u00E0 mentor \u2013 \u0111\u00E1nh gi\u00E1 n\u0103ng l\u1EF1c teamwork, leadership, v\u00E0 kh\u1EA3 n\u0103ng x\u1EED l\u00FD t\u00ECnh hu\u1ED1ng.\nK\u1EBFt qu\u1EA3: C\u00F4ng b\u1ED1 ch\u00EDnh th\u1EE9c qua email \u0026 fanpage FPTU Debate Championship.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nT\u00ECm ki\u1EBFm nh\u1EEFng nh\u00E2n t\u1ED1 tr\u1EBB \u0111\u1EA7y nhi\u1EC7t huy\u1EBFt tham gia \u0111\u1ED9i ng\u0169 t\u1ED5 ch\u1EE9c FPTU Debate Championship 2026.\nT\u1EA1o m\u00F4i tr\u01B0\u1EDDng th\u1EF1c h\u00E0nh th\u1EF1c t\u1EBF trong l\u0129nh v\u1EF1c event management, media, public speaking v\u00E0 tranh bi\u1EC7n h\u1ECDc thu\u1EADt.\nB\u1ED3i d\u01B0\u1EE1ng k\u1EF9 n\u0103ng m\u1EC1m: giao ti\u1EBFp, s\u00E1ng t\u1EA1o, qu\u1EA3n l\u00FD d\u1EF1 \u00E1n, l\u00E0m vi\u1EC7c nh\u00F3m.\n\n\uD83C\uDFC6 Quy\u1EC1n l\u1EE3i th\u00E0nh vi\u00EAn BTC\n\nNh\u1EADn ch\u1EE9ng nh\u1EADn ch\u00EDnh th\u1EE9c t\u1EEB FPTU Debate Championship 2026.\nTham gia chu\u1ED7i workshop n\u1ED9i b\u1ED9 v\u1EC1 leadership \u0026 event planning.\nK\u1EBFt n\u1ED1i c\u00F9ng c\u00E1c di\u1EC5n gi\u1EA3 v\u00E0 hu\u1EA5n luy\u1EC7n vi\u00EAn tranh bi\u1EC7n chuy\u00EAn nghi\u1EC7p.\nC\u01A1 h\u1ED9i tr\u1EDF th\u00E0nh core team c\u1EE7a nh\u1EEFng m\u00F9a Debate Championship ti\u1EBFp theo.\n\n\uD83D\uDCF2 C\u00E1ch tham gia\n\nTruy c\u1EADp website \u0111\u0103ng k\u00FD BTC: [FPTU Debate Championship 2026] (qua QR code tr\u00EAn poster).\n\u0110i\u1EC1n form \u1EE9ng tuy\u1EC3n \u0026 ch\u1EDD ph\u1EA3n h\u1ED3i l\u1ECBch ph\u1ECFng v\u1EA5n.\n\n\uD83D\uDCE7 Li\u00EAn h\u1EC7 Ban T\u1ED5 Ch\u1EE9c\n\nHead of Organization: Nguy\u1EC5n V\u0103n Hi\u1EC7p \u2013 0906 165 937\nHuman Resources: V\u0169 K\u1EF3 Anh \u2013 0947 031 104\nMedia \u0026 Design: Ph\u1EA1m Th\u1ECB Anh Th\u01B0 \u2013 0981 703 557\nEmail: fptudebatechampionship@gmail.com\nFanpage: FPTU Debate Championship","EventDetails":"\uD83E\uDDE0 Ch\u1EE7 \u0111\u1EC1 ch\u00EDnh\n\u201CArcana Awakening \u2013 \u0110\u00E1nh th\u1EE9c ti\u1EC1m n\u0103ng, m\u1EDF kh\u00F3a tr\u00ED tu\u1EC7.\u201D\nArcana Awakening l\u00E0 ch\u01B0\u01A1ng tr\u00ECnh tuy\u1EC3n th\u00E0nh vi\u00EAn Ban T\u1ED5 Ch\u1EE9c cho cu\u1ED9c thi tranh bi\u1EC7n l\u1EDBn nh\u1EA5t FPTU \u2013 Debate Championship 2026.\nL\u1EA5y c\u1EA3m h\u1EE9ng t\u1EEB b\u1ED9 b\u00E0i Tarot, s\u1EF1 ki\u1EC7n mang th\u00F4ng \u0111i\u1EC7p \u0111\u00E1nh th\u1EE9c nh\u1EEFng l\u00E1 b\u00E0i ti\u1EC1m \u1EA9n trong m\u1ED7i ng\u01B0\u1EDDi, \u0111\u1EA1i di\u1EC7n cho s\u1EE9c s\u00E1ng t\u1EA1o, kh\u1EA3 n\u0103ng ph\u1EA3n bi\u1EC7n v\u00E0 k\u1EF9 n\u0103ng t\u1ED5 ch\u1EE9c \u1EA5n t\u01B0\u1EE3ng.\n\n\uD83D\uDCA1 N\u1ED9i dung tuy\u1EC3n ch\u1ECDn\n\nV\u00F2ng \u0111\u01A1n: \u1EE8ng vi\u00EAn g\u1EEDi h\u1ED3 s\u01A1 gi\u1EDBi thi\u1EC7u b\u1EA3n th\u00E2n, k\u1EF9 n\u0103ng v\u00E0 v\u1ECB tr\u00ED mong mu\u1ED1n (Logistics, Content, Media, Design, HR\u2026).\nV\u00F2ng ph\u1ECFng v\u1EA5n: Trao \u0111\u1ED5i tr\u1EF1c ti\u1EBFp v\u1EDBi tr\u01B0\u1EDFng ban v\u00E0 mentor \u2013 \u0111\u00E1nh gi\u00E1 n\u0103ng l\u1EF1c teamwork, leadership, v\u00E0 kh\u1EA3 n\u0103ng x\u1EED l\u00FD t\u00ECnh hu\u1ED1ng.\nK\u1EBFt qu\u1EA3: C\u00F4ng b\u1ED1 ch\u00EDnh th\u1EE9c qua email \u0026 fanpage FPTU Debate Championship.\n\n\uD83C\uDFAF M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nT\u00ECm ki\u1EBFm nh\u1EEFng nh\u00E2n t\u1ED1 tr\u1EBB \u0111\u1EA7y nhi\u1EC7t huy\u1EBFt tham gia \u0111\u1ED9i ng\u0169 t\u1ED5 ch\u1EE9c FPTU Debate Championship 2026.\nT\u1EA1o m\u00F4i tr\u01B0\u1EDDng th\u1EF1c h\u00E0nh th\u1EF1c t\u1EBF trong l\u0129nh v\u1EF1c event management, media, public speaking v\u00E0 tranh bi\u1EC7n h\u1ECDc thu\u1EADt.\nB\u1ED3i d\u01B0\u1EE1ng k\u1EF9 n\u0103ng m\u1EC1m: giao ti\u1EBFp, s\u00E1ng t\u1EA1o, qu\u1EA3n l\u00FD d\u1EF1 \u00E1n, l\u00E0m vi\u1EC7c nh\u00F3m.\n\n\uD83C\uDFC6 Quy\u1EC1n l\u1EE3i th\u00E0nh vi\u00EAn BTC\n\nNh\u1EADn ch\u1EE9ng nh\u1EADn ch\u00EDnh th\u1EE9c t\u1EEB FPTU Debate Championship 2026.\nTham gia chu\u1ED7i workshop n\u1ED9i b\u1ED9 v\u1EC1 leadership \u0026 event planning.\nK\u1EBFt n\u1ED1i c\u00F9ng c\u00E1c di\u1EC5n gi\u1EA3 v\u00E0 hu\u1EA5n luy\u1EC7n vi\u00EAn tranh bi\u1EC7n chuy\u00EAn nghi\u1EC7p.\nC\u01A1 h\u1ED9i tr\u1EDF th\u00E0nh core team c\u1EE7a nh\u1EEFng m\u00F9a Debate Championship ti\u1EBFp theo.\n\n\uD83D\uDCF2 C\u00E1ch tham gia\n\nTruy c\u1EADp website \u0111\u0103ng k\u00FD BTC: [FPTU Debate Championship 2026] (qua QR code tr\u00EAn poster).\n\u0110i\u1EC1n form \u1EE9ng tuy\u1EC3n \u0026 ch\u1EDD ph\u1EA3n h\u1ED3i l\u1ECBch ph\u1ECFng v\u1EA5n.\n\n\uD83D\uDCE7 Li\u00EAn h\u1EC7 Ban T\u1ED5 Ch\u1EE9c\n\nHead of Organization: Nguy\u1EC5n V\u0103n Hi\u1EC7p \u2013 0906 165 937\nHuman Resources: V\u0169 K\u1EF3 Anh \u2013 0947 031 104\nMedia \u0026 Design: Ph\u1EA1m Th\u1ECB Anh Th\u01B0 \u2013 0981 703 557\nEmail: fptudebatechampionship@gmail.com\nFanpage: FPTU Debate Championship","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/44375f59-4993-4d36-b123-afd1f1b8bc5f.png","OrganizerName":"FPTU","OrganizerInfo":"none"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (27, 1, N'FIRST TECH CHALLENGE VIETNAM 2025 – KICKOFF', N'🚀 Giới thiệu sự kiện

FIRST Tech Challenge Vietnam 2025–2026 – KICKOFF là sự kiện khởi động chính thức của mùa giải Robotics lớn nhất dành cho học sinh, sinh viên yêu công nghệ tại Việt Nam.
Sự kiện mang chủ đề “FIRST AGE”, hứa hẹn mở ra một kỷ nguyên công nghệ mới, nơi sáng tạo và kỹ thuật kết hợp để giải quyết những thách thức toàn cầu.

📅 Thời gian & Địa điểm

Thời gian: 10h00 – 16h00, ngày 15/11/2025
Địa điểm: FPT Complex, FPT City, Đà Nẵng
Livestream: Trực tiếp trên fanpage FIRST Tech Challenge Vietnam

🧩 Nội dung chương trình

🔹 Giới thiệu chủ đề & nhiệm vụ thi đấu mùa giải 2025–2026
🔹 Trình diễn robot mẫu và workshop kỹ thuật
🔹 Kết nối đội thi mới & huấn luyện viên từ các trường trên toàn quốc
🔹 Phát động chương trình huấn luyện “Build The Future” – hướng dẫn học sinh chế tạo robot đầu tiên.
🔹 Giao lưu cùng đội vô địch mùa giải 2024–2025

🧠 Đối tượng tham gia

Học sinh THPT, sinh viên đại học yêu thích khoa học, công nghệ, kỹ thuật và robot
Giáo viên, mentor, và các câu lạc bộ STEM tại Việt Nam
Các doanh nghiệp quan tâm đến công nghệ giáo dục và đổi mới sáng tạo

🏆 Mục tiêu sự kiện

Truyền cảm hứng STEM – Robotics đến thế hệ trẻ Việt Nam
Giúp học sinh – sinh viên tiếp cận công nghệ tự động hóa, lập trình và cơ khí thực tế
Tạo môi trường kết nối – học hỏi – thi đấu chuyên nghiệp theo tiêu chuẩn quốc tế

🔧 Cơ hội cho đội thi

Đăng ký tham gia FIRST Tech Challenge Vietnam 2025–2026
Nhận robot kit, hướng dẫn kỹ thuật và mentor hỗ trợ
Tham gia vòng loại khu vực tại Hà Nội, Đà Nẵng, TP.HCM
Cơ hội đại diện Việt Nam thi đấu tại FIRST Tech Challenge World Championship 2026 🌎

📲 Cách đăng ký
👉 Đăng ký đội thi tại: www.firsttechvietnam.vn/kickoff
📧 Liên hệ BTC: info@firsttechvietnam.vn
📞 Hotline: 0909 123 456
💬 Thông điệp sự kiện
“Build. Code. Compete. Inspire — Hành trình khởi đầu kỷ nguyên FIRST AGE, nơi robot và con người cùng viết nên tương lai.”

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), NULL, N'Public', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"FPT Complex, FPT City","StreetAddress":"123 L\u00EA Du\u1EA9n","Province":"\u0110\u00E0 N\u1EB5ng","District":"Qu\u1EADn H\u1EA3i Ch\u00E2u","Ward":"Ph\u01B0\u1EDDng H\u00F2a Thu\u1EADn \u0110\u00F4ng","EventImage":"/assets/images/events/kick_off.jpg","BackgroundImage":"/assets/images/events/kick_off.jpg","EventIntroduction":"\uD83D\uDE80 Gi\u1EDBi thi\u1EC7u s\u1EF1 ki\u1EC7n\n\nFIRST Tech Challenge Vietnam 2025\u20132026 \u2013 KICKOFF l\u00E0 s\u1EF1 ki\u1EC7n kh\u1EDFi \u0111\u1ED9ng ch\u00EDnh th\u1EE9c c\u1EE7a m\u00F9a gi\u1EA3i Robotics l\u1EDBn nh\u1EA5t d\u00E0nh cho h\u1ECDc sinh, sinh vi\u00EAn y\u00EAu c\u00F4ng ngh\u1EC7 t\u1EA1i Vi\u1EC7t Nam.\nS\u1EF1 ki\u1EC7n mang ch\u1EE7 \u0111\u1EC1 \u201CFIRST AGE\u201D, h\u1EE9a h\u1EB9n m\u1EDF ra m\u1ED9t k\u1EF7 nguy\u00EAn c\u00F4ng ngh\u1EC7 m\u1EDBi, n\u01A1i s\u00E1ng t\u1EA1o v\u00E0 k\u1EF9 thu\u1EADt k\u1EBFt h\u1EE3p \u0111\u1EC3 gi\u1EA3i quy\u1EBFt nh\u1EEFng th\u00E1ch th\u1EE9c to\u00E0n c\u1EA7u.\n\n\uD83D\uDCC5 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\nTh\u1EDDi gian: 10h00 \u2013 16h00, ng\u00E0y 15/11/2025\n\u0110\u1ECBa \u0111i\u1EC3m: FPT Complex, FPT City, \u0110\u00E0 N\u1EB5ng\nLivestream: Tr\u1EF1c ti\u1EBFp tr\u00EAn fanpage FIRST Tech Challenge Vietnam\n\n\uD83E\uDDE9 N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\n\uD83D\uDD39 Gi\u1EDBi thi\u1EC7u ch\u1EE7 \u0111\u1EC1 \u0026 nhi\u1EC7m v\u1EE5 thi \u0111\u1EA5u m\u00F9a gi\u1EA3i 2025\u20132026\n\uD83D\uDD39 Tr\u00ECnh di\u1EC5n robot m\u1EABu v\u00E0 workshop k\u1EF9 thu\u1EADt\n\uD83D\uDD39 K\u1EBFt n\u1ED1i \u0111\u1ED9i thi m\u1EDBi \u0026 hu\u1EA5n luy\u1EC7n vi\u00EAn t\u1EEB c\u00E1c tr\u01B0\u1EDDng tr\u00EAn to\u00E0n qu\u1ED1c\n\uD83D\uDD39 Ph\u00E1t \u0111\u1ED9ng ch\u01B0\u01A1ng tr\u00ECnh hu\u1EA5n luy\u1EC7n \u201CBuild The Future\u201D \u2013 h\u01B0\u1EDBng d\u1EABn h\u1ECDc sinh ch\u1EBF t\u1EA1o robot \u0111\u1EA7u ti\u00EAn.\n\uD83D\uDD39 Giao l\u01B0u c\u00F9ng \u0111\u1ED9i v\u00F4 \u0111\u1ECBch m\u00F9a gi\u1EA3i 2024\u20132025\n\n\uD83E\uDDE0 \u0110\u1ED1i t\u01B0\u1EE3ng tham gia\n\nH\u1ECDc sinh THPT, sinh vi\u00EAn \u0111\u1EA1i h\u1ECDc y\u00EAu th\u00EDch khoa h\u1ECDc, c\u00F4ng ngh\u1EC7, k\u1EF9 thu\u1EADt v\u00E0 robot\nGi\u00E1o vi\u00EAn, mentor, v\u00E0 c\u00E1c c\u00E2u l\u1EA1c b\u1ED9 STEM t\u1EA1i Vi\u1EC7t Nam\nC\u00E1c doanh nghi\u1EC7p quan t\u00E2m \u0111\u1EBFn c\u00F4ng ngh\u1EC7 gi\u00E1o d\u1EE5c v\u00E0 \u0111\u1ED5i m\u1EDBi s\u00E1ng t\u1EA1o\n\n\uD83C\uDFC6 M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nTruy\u1EC1n c\u1EA3m h\u1EE9ng STEM \u2013 Robotics \u0111\u1EBFn th\u1EBF h\u1EC7 tr\u1EBB Vi\u1EC7t Nam\nGi\u00FAp h\u1ECDc sinh \u2013 sinh vi\u00EAn ti\u1EBFp c\u1EADn c\u00F4ng ngh\u1EC7 t\u1EF1 \u0111\u1ED9ng h\u00F3a, l\u1EADp tr\u00ECnh v\u00E0 c\u01A1 kh\u00ED th\u1EF1c t\u1EBF\nT\u1EA1o m\u00F4i tr\u01B0\u1EDDng k\u1EBFt n\u1ED1i \u2013 h\u1ECDc h\u1ECFi \u2013 thi \u0111\u1EA5u chuy\u00EAn nghi\u1EC7p theo ti\u00EAu chu\u1EA9n qu\u1ED1c t\u1EBF\n\n\uD83D\uDD27 C\u01A1 h\u1ED9i cho \u0111\u1ED9i thi\n\n\u0110\u0103ng k\u00FD tham gia FIRST Tech Challenge Vietnam 2025\u20132026\nNh\u1EADn robot kit, h\u01B0\u1EDBng d\u1EABn k\u1EF9 thu\u1EADt v\u00E0 mentor h\u1ED7 tr\u1EE3\nTham gia v\u00F2ng lo\u1EA1i khu v\u1EF1c t\u1EA1i H\u00E0 N\u1ED9i, \u0110\u00E0 N\u1EB5ng, TP.HCM\nC\u01A1 h\u1ED9i \u0111\u1EA1i di\u1EC7n Vi\u1EC7t Nam thi \u0111\u1EA5u t\u1EA1i FIRST Tech Challenge World Championship 2026 \uD83C\uDF0E\n\n\uD83D\uDCF2 C\u00E1ch \u0111\u0103ng k\u00FD\n\uD83D\uDC49 \u0110\u0103ng k\u00FD \u0111\u1ED9i thi t\u1EA1i: www.firsttechvietnam.vn/kickoff\n\uD83D\uDCE7 Li\u00EAn h\u1EC7 BTC: info@firsttechvietnam.vn\n\uD83D\uDCDE Hotline: 0909 123 456\n\uD83D\uDCAC Th\u00F4ng \u0111i\u1EC7p s\u1EF1 ki\u1EC7n\n\u201CBuild. Code. Compete. Inspire \u2014 H\u00E0nh tr\u00ECnh kh\u1EDFi \u0111\u1EA7u k\u1EF7 nguy\u00EAn FIRST AGE, n\u01A1i robot v\u00E0 con ng\u01B0\u1EDDi c\u00F9ng vi\u1EBFt n\u00EAn t\u01B0\u01A1ng lai.\u201D","EventDetails":"\uD83D\uDE80 Gi\u1EDBi thi\u1EC7u s\u1EF1 ki\u1EC7n\n\nFIRST Tech Challenge Vietnam 2025\u20132026 \u2013 KICKOFF l\u00E0 s\u1EF1 ki\u1EC7n kh\u1EDFi \u0111\u1ED9ng ch\u00EDnh th\u1EE9c c\u1EE7a m\u00F9a gi\u1EA3i Robotics l\u1EDBn nh\u1EA5t d\u00E0nh cho h\u1ECDc sinh, sinh vi\u00EAn y\u00EAu c\u00F4ng ngh\u1EC7 t\u1EA1i Vi\u1EC7t Nam.\nS\u1EF1 ki\u1EC7n mang ch\u1EE7 \u0111\u1EC1 \u201CFIRST AGE\u201D, h\u1EE9a h\u1EB9n m\u1EDF ra m\u1ED9t k\u1EF7 nguy\u00EAn c\u00F4ng ngh\u1EC7 m\u1EDBi, n\u01A1i s\u00E1ng t\u1EA1o v\u00E0 k\u1EF9 thu\u1EADt k\u1EBFt h\u1EE3p \u0111\u1EC3 gi\u1EA3i quy\u1EBFt nh\u1EEFng th\u00E1ch th\u1EE9c to\u00E0n c\u1EA7u.\n\n\uD83D\uDCC5 Th\u1EDDi gian \u0026 \u0110\u1ECBa \u0111i\u1EC3m\n\nTh\u1EDDi gian: 10h00 \u2013 16h00, ng\u00E0y 15/11/2025\n\u0110\u1ECBa \u0111i\u1EC3m: FPT Complex, FPT City, \u0110\u00E0 N\u1EB5ng\nLivestream: Tr\u1EF1c ti\u1EBFp tr\u00EAn fanpage FIRST Tech Challenge Vietnam\n\n\uD83E\uDDE9 N\u1ED9i dung ch\u01B0\u01A1ng tr\u00ECnh\n\n\uD83D\uDD39 Gi\u1EDBi thi\u1EC7u ch\u1EE7 \u0111\u1EC1 \u0026 nhi\u1EC7m v\u1EE5 thi \u0111\u1EA5u m\u00F9a gi\u1EA3i 2025\u20132026\n\uD83D\uDD39 Tr\u00ECnh di\u1EC5n robot m\u1EABu v\u00E0 workshop k\u1EF9 thu\u1EADt\n\uD83D\uDD39 K\u1EBFt n\u1ED1i \u0111\u1ED9i thi m\u1EDBi \u0026 hu\u1EA5n luy\u1EC7n vi\u00EAn t\u1EEB c\u00E1c tr\u01B0\u1EDDng tr\u00EAn to\u00E0n qu\u1ED1c\n\uD83D\uDD39 Ph\u00E1t \u0111\u1ED9ng ch\u01B0\u01A1ng tr\u00ECnh hu\u1EA5n luy\u1EC7n \u201CBuild The Future\u201D \u2013 h\u01B0\u1EDBng d\u1EABn h\u1ECDc sinh ch\u1EBF t\u1EA1o robot \u0111\u1EA7u ti\u00EAn.\n\uD83D\uDD39 Giao l\u01B0u c\u00F9ng \u0111\u1ED9i v\u00F4 \u0111\u1ECBch m\u00F9a gi\u1EA3i 2024\u20132025\n\n\uD83E\uDDE0 \u0110\u1ED1i t\u01B0\u1EE3ng tham gia\n\nH\u1ECDc sinh THPT, sinh vi\u00EAn \u0111\u1EA1i h\u1ECDc y\u00EAu th\u00EDch khoa h\u1ECDc, c\u00F4ng ngh\u1EC7, k\u1EF9 thu\u1EADt v\u00E0 robot\nGi\u00E1o vi\u00EAn, mentor, v\u00E0 c\u00E1c c\u00E2u l\u1EA1c b\u1ED9 STEM t\u1EA1i Vi\u1EC7t Nam\nC\u00E1c doanh nghi\u1EC7p quan t\u00E2m \u0111\u1EBFn c\u00F4ng ngh\u1EC7 gi\u00E1o d\u1EE5c v\u00E0 \u0111\u1ED5i m\u1EDBi s\u00E1ng t\u1EA1o\n\n\uD83C\uDFC6 M\u1EE5c ti\u00EAu s\u1EF1 ki\u1EC7n\n\nTruy\u1EC1n c\u1EA3m h\u1EE9ng STEM \u2013 Robotics \u0111\u1EBFn th\u1EBF h\u1EC7 tr\u1EBB Vi\u1EC7t Nam\nGi\u00FAp h\u1ECDc sinh \u2013 sinh vi\u00EAn ti\u1EBFp c\u1EADn c\u00F4ng ngh\u1EC7 t\u1EF1 \u0111\u1ED9ng h\u00F3a, l\u1EADp tr\u00ECnh v\u00E0 c\u01A1 kh\u00ED th\u1EF1c t\u1EBF\nT\u1EA1o m\u00F4i tr\u01B0\u1EDDng k\u1EBFt n\u1ED1i \u2013 h\u1ECDc h\u1ECFi \u2013 thi \u0111\u1EA5u chuy\u00EAn nghi\u1EC7p theo ti\u00EAu chu\u1EA9n qu\u1ED1c t\u1EBF\n\n\uD83D\uDD27 C\u01A1 h\u1ED9i cho \u0111\u1ED9i thi\n\n\u0110\u0103ng k\u00FD tham gia FIRST Tech Challenge Vietnam 2025\u20132026\nNh\u1EADn robot kit, h\u01B0\u1EDBng d\u1EABn k\u1EF9 thu\u1EADt v\u00E0 mentor h\u1ED7 tr\u1EE3\nTham gia v\u00F2ng lo\u1EA1i khu v\u1EF1c t\u1EA1i H\u00E0 N\u1ED9i, \u0110\u00E0 N\u1EB5ng, TP.HCM\nC\u01A1 h\u1ED9i \u0111\u1EA1i di\u1EC7n Vi\u1EC7t Nam thi \u0111\u1EA5u t\u1EA1i FIRST Tech Challenge World Championship 2026 \uD83C\uDF0E\n\n\uD83D\uDCF2 C\u00E1ch \u0111\u0103ng k\u00FD\n\uD83D\uDC49 \u0110\u0103ng k\u00FD \u0111\u1ED9i thi t\u1EA1i: www.firsttechvietnam.vn/kickoff\n\uD83D\uDCE7 Li\u00EAn h\u1EC7 BTC: info@firsttechvietnam.vn\n\uD83D\uDCDE Hotline: 0909 123 456\n\uD83D\uDCAC Th\u00F4ng \u0111i\u1EC7p s\u1EF1 ki\u1EC7n\n\u201CBuild. Code. Compete. Inspire \u2014 H\u00E0nh tr\u00ECnh kh\u1EDFi \u0111\u1EA7u k\u1EF7 nguy\u00EAn FIRST AGE, n\u01A1i robot v\u00E0 con ng\u01B0\u1EDDi c\u00F9ng vi\u1EBFt n\u00EAn t\u01B0\u01A1ng lai.\u201D","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/fd5ef5c2-84b7-4aad-9d51-4b1e064ad835.png","OrganizerName":"FPTU","OrganizerInfo":"none."}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (28, 1, N'VƯỢT LŨ – CUỘC THI THỂ THAO ĐA MÔN 2025', N'🏃‍♂️ "Vượt qua giới hạn – Chinh phục thử thách"

Vượt Lũ là cuộc thi thể thao đa môn quy mô lớn dành cho học sinh, sinh viên các trường đại học tại Đà Nẵng, nhằm tôn vinh tinh thần thể thao, sự kiên trì và khả năng vượt qua thử thách của thế hệ trẻ.

💪 Nội dung thi đấu

🏊‍♀️ Bơi lội: 50m tự do, 100m bơi ếch
🏃‍♂️ Chạy bộ: 3km, 5km đường trường
🚴‍♀️ Đạp xe: 10km địa hình
🧗‍♂️ Leo núi: Leo tường nhân tạo
🏋️‍♂️ CrossFit: Thử thách sức bền toàn diện

🎯 Mục tiêu

Khuyến khích lối sống lành mạnh và rèn luyện thể chất
Xây dựng tinh thần đồng đội và tinh thần fair-play
Phát hiện tài năng thể thao trẻ tại khu vực miền Trung

🏆 Giải thưởng

🥇 Nhất toàn năng: 5.000.000 VNĐ + Huy chương vàng
🥈 Nhì toàn năng: 3.000.000 VNĐ + Huy chương bạc
🥉 Ba toàn năng: 2.000.000 VNĐ + Huy chương đồng
🎁 Giải phong cách: 1.000.000 VNĐ cho đội có tinh thần thể thao tốt nhất

📅 Thời gian & Địa điểm
📅 Ngày: 20 tháng 11 năm 2025
🕘 Giờ: 6h00 – 18h00
📍 Địa điểm: Trung tâm Thể thao Đà Nẵng, Quận Hải Châu

💬 Thông điệp
"Vượt Lũ không chỉ là vượt qua dòng nước, mà là vượt qua chính bản thân mình."

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), N'Trung tâm Thể thao Đà Nẵng, Quận Hải Châu, Đà Nẵng', N'Public', 'Offline', 'Sports', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Trung tâm Thể thao Đà Nẵng","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/vuot_lu.png","BackgroundImage":"/assets/images/events/vuot_lu.png","EventIntroduction":"Vượt Lũ là cuộc thi thể thao đa môn quy mô lớn dành cho học sinh, sinh viên các trường đại học tại Đà Nẵng, nhằm tôn vinh tinh thần thể thao, sự kiên trì và khả năng vượt qua thử thách của thế hệ trẻ.","EventDetails":"🏃‍♂️ \"Vượt qua giới hạn – Chinh phục thử thách\"\n\nVượt Lũ là cuộc thi thể thao đa môn quy mô lớn dành cho học sinh, sinh viên các trường đại học tại Đà Nẵng, nhằm tôn vinh tinh thần thể thao, sự kiên trì và khả năng vượt qua thử thách của thế hệ trẻ.\n\n💪 Nội dung thi đấu\n\n🏊‍♀️ Bơi lội: 50m tự do, 100m bơi ếch\n🏃‍♂️ Chạy bộ: 3km, 5km đường trường\n🚴‍♀️ Đạp xe: 10km địa hình\n🧗‍♂️ Leo núi: Leo tường nhân tạo\n🏋️‍♂️ CrossFit: Thử thách sức bền toàn diện\n\n🎯 Mục tiêu\n\nKhuyến khích lối sống lành mạnh và rèn luyện thể chất\nXây dựng tinh thần đồng đội và tinh thần fair-play\nPhát hiện tài năng thể thao trẻ tại khu vực miền Trung\n\n🏆 Giải thưởng\n\n🥇 Nhất toàn năng: 5.000.000 VNĐ + Huy chương vàng\n🥈 Nhì toàn năng: 3.000.000 VNĐ + Huy chương bạc\n🥉 Ba toàn năng: 2.000.000 VNĐ + Huy chương đồng\n🎁 Giải phong cách: 1.000.000 VNĐ cho đội có tinh thần thể thao tốt nhất\n\n📅 Thời gian & Địa điểm\n📅 Ngày: 20 tháng 11 năm 2025\n🕘 Giờ: 6h00 – 18h00\n📍 Địa điểm: Trung tâm Thể thao Đà Nẵng, Quận Hải Châu\n\n💬 Thông điệp\n\"Vượt Lũ không chỉ là vượt qua dòng nước, mà là vượt qua chính bản thân mình.\"","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/vuot_lu.png","OrganizerName":"FPT Sports Club","OrganizerInfo":"Câu lạc bộ thể thao FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (29, 1, N'FPT HACKATHON 2025 – CODE FOR IMPACT', N'💻 "Code không chỉ là code, mà là giải pháp cho tương lai"

FPT Hackathon 2025 là cuộc thi lập trình marathon 48 giờ lớn nhất dành cho sinh viên công nghệ thông tin tại miền Trung, nơi các đội thi sẽ xây dựng các sản phẩm công nghệ giải quyết các vấn đề thực tế trong cộng đồng.

🎯 Chủ đề năm 2025

"Tech for Social Good" – Công nghệ vì cộng đồng
Các lĩnh vực: Giáo dục, Y tế, Môi trường, Giao thông, An sinh xã hội

🛠️ Nội dung cuộc thi

⏰ Thời gian: 48 giờ liên tục (từ 8h00 ngày thứ 6 đến 8h00 ngày Chủ nhật)
👥 Đội thi: 3-4 thành viên
💡 Hình thức: Tự do lựa chọn công nghệ, framework, ngôn ngữ lập trình
🎤 Pitching: 5 phút trình bày + 2 phút Q&A

🏆 Giải thưởng

🥇 Giải Nhất: 10.000.000 VNĐ + Cơ hội thực tập tại FPT Software
🥈 Giải Nhì: 5.000.000 VNĐ + Voucher công nghệ 2.000.000đ
🥉 Giải Ba: 3.000.000 VNĐ + Khóa học online miễn phí
🌟 Giải Sáng tạo: 2.000.000 VNĐ cho giải pháp độc đáo nhất
👥 Giải Teamwork: 1.000.000 VNĐ cho đội có tinh thần hợp tác tốt nhất

🎁 Quyền lợi tham gia

🍕 Ăn uống miễn phí trong 48 giờ
☕ Nước uống và cà phê không giới hạn
🏨 Chỗ nghỉ tại chỗ (nếu cần)
🎁 Swag bag với quà tặng từ nhà tài trợ
📜 Chứng nhận tham gia cho tất cả đội thi

👨‍💻 Ban giám khảo

Anh Lê Minh Đức – CTO, FPT Software Đà Nẵng
Chị Nguyễn Thị Hoa – Product Manager, VNG Corporation
Anh Trần Văn Nam – Senior Architect, Google Vietnam

📅 Thời gian & Địa điểm
📅 Ngày: 14-16 tháng 11 năm 2025
📍 Địa điểm: FPT University Đà Nẵng
💬 Liên hệ: hackathon@fpt.edu.vn

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 9, GETUTCDATE()), N'FPT University Đà Nẵng, Quận Hải Châu, Đà Nẵng', N'Public', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"FPT University Đà Nẵng","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/hackathon.png","BackgroundImage":"/assets/images/events/hackathon.png","EventIntroduction":"FPT Hackathon 2025 là cuộc thi lập trình marathon 48 giờ lớn nhất dành cho sinh viên công nghệ thông tin tại miền Trung, nơi các đội thi sẽ xây dựng các sản phẩm công nghệ giải quyết các vấn đề thực tế trong cộng đồng.","EventDetails":"💻 \"Code không chỉ là code, mà là giải pháp cho tương lai\"\n\nFPT Hackathon 2025 là cuộc thi lập trình marathon 48 giờ lớn nhất dành cho sinh viên công nghệ thông tin tại miền Trung, nơi các đội thi sẽ xây dựng các sản phẩm công nghệ giải quyết các vấn đề thực tế trong cộng đồng.\n\n🎯 Chủ đề năm 2025\n\n\"Tech for Social Good\" – Công nghệ vì cộng đồng\nCác lĩnh vực: Giáo dục, Y tế, Môi trường, Giao thông, An sinh xã hội\n\n🛠️ Nội dung cuộc thi\n\n⏰ Thời gian: 48 giờ liên tục (từ 8h00 ngày thứ 6 đến 8h00 ngày Chủ nhật)\n👥 Đội thi: 3-4 thành viên\n💡 Hình thức: Tự do lựa chọn công nghệ, framework, ngôn ngữ lập trình\n🎤 Pitching: 5 phút trình bày + 2 phút Q&A\n\n🏆 Giải thưởng\n\n🥇 Giải Nhất: 10.000.000 VNĐ + Cơ hội thực tập tại FPT Software\n🥈 Giải Nhì: 5.000.000 VNĐ + Voucher công nghệ 2.000.000đ\n🥉 Giải Ba: 3.000.000 VNĐ + Khóa học online miễn phí\n🌟 Giải Sáng tạo: 2.000.000 VNĐ cho giải pháp độc đáo nhất\n👥 Giải Teamwork: 1.000.000 VNĐ cho đội có tinh thần hợp tác tốt nhất\n\n🎁 Quyền lợi tham gia\n\n🍕 Ăn uống miễn phí trong 48 giờ\n☕ Nước uống và cà phê không giới hạn\n🏨 Chỗ nghỉ tại chỗ (nếu cần)\n🎁 Swag bag với quà tặng từ nhà tài trợ\n📜 Chứng nhận tham gia cho tất cả đội thi\n\n👨‍💻 Ban giám khảo\n\nAnh Lê Minh Đức – CTO, FPT Software Đà Nẵng\nChị Nguyễn Thị Hoa – Product Manager, VNG Corporation\nAnh Trần Văn Nam – Senior Architect, Google Vietnam\n\n📅 Thời gian & Địa điểm\n📅 Ngày: 14-16 tháng 11 năm 2025\n📍 Địa điểm: FPT University Đà Nẵng\n💬 Liên hệ: hackathon@fpt.edu.vn","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/hackathon.png","OrganizerName":"FPT University","OrganizerInfo":"Trường Đại học FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (30, 2, N'ĐĂNG HÌNH HẠNH PHÚC – PHOTO CONTEST 2025', N'📸 "Mỗi khoảnh khắc đều đáng ghi lại"

Đăng Hình Hạnh Phúc là cuộc thi nhiếp ảnh thường niên do CLB Nhiếp ảnh FPT tổ chức, khuyến khích các bạn trẻ ghi lại những khoảnh khắc hạnh phúc, đẹp đẽ trong cuộc sống hàng ngày.

📷 Hạng mục thi

🏙️ Phong cảnh: Thiên nhiên, kiến trúc, thành phố
👥 Chân dung: Con người và cảm xúc
🍔 Ẩm thực: Món ăn và văn hóa ẩm thực
📱 Cuộc sống: Khoảnh khắc đời thường
🎨 Sáng tạo: Ảnh nghệ thuật và conceptual

🎯 Yêu cầu

📷 Ảnh độ phân giải tối thiểu: 1920x1080
🎨 Được phép chỉnh sửa nhẹ (không được composite hoặc thay đổi nội dung)
📝 Kèm caption mô tả ý nghĩa bức ảnh (tối đa 200 từ)
📅 Thời gian chụp: Từ tháng 1/2025 đến tháng 11/2025

🏆 Giải thưởng

🥇 Giải Nhất mỗi hạng mục: 2.000.000 VNĐ + Máy ảnh Fujifilm
🥈 Giải Nhì mỗi hạng mục: 1.000.000 VNĐ + Lens 50mm
🥉 Giải Ba mỗi hạng mục: 500.000 VNĐ + Tripod cao cấp
🌟 Giải Ảnh được yêu thích nhất: 1.000.000 VNĐ (bình chọn online)
🎖️ Top 10 mỗi hạng mục: Chứng nhận + Quà lưu niệm

📅 Thời gian
📤 Nộp ảnh: 01/10/2025 – 30/11/2025
🏆 Công bố kết quả: 15/12/2025
🎨 Triển lãm: 20-25/12/2025 tại Gallery FPT

💬 Thông điệp
"Hạnh phúc không phải là điều ta tìm kiếm, mà là điều ta nhận ra khi ta biết dừng lại và ngắm nhìn."

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 60, GETUTCDATE()), NULL, N'Public', 'Online', 'Art', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Gallery FPT","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/dang_hinh_hanh_phuc.png","BackgroundImage":"/assets/images/events/dang_hinh_hanh_phuc.png","EventIntroduction":"Đăng Hình Hạnh Phúc là cuộc thi nhiếp ảnh thường niên do CLB Nhiếp ảnh FPT tổ chức, khuyến khích các bạn trẻ ghi lại những khoảnh khắc hạnh phúc, đẹp đẽ trong cuộc sống hàng ngày.","EventDetails":"📸 \"Mỗi khoảnh khắc đều đáng ghi lại\"\n\nĐăng Hình Hạnh Phúc là cuộc thi nhiếp ảnh thường niên do CLB Nhiếp ảnh FPT tổ chức, khuyến khích các bạn trẻ ghi lại những khoảnh khắc hạnh phúc, đẹp đẽ trong cuộc sống hàng ngày.\n\n📷 Hạng mục thi\n\n🏙️ Phong cảnh: Thiên nhiên, kiến trúc, thành phố\n👥 Chân dung: Con người và cảm xúc\n🍔 Ẩm thực: Món ăn và văn hóa ẩm thực\n📱 Cuộc sống: Khoảnh khắc đời thường\n🎨 Sáng tạo: Ảnh nghệ thuật và conceptual\n\n🎯 Yêu cầu\n\n📷 Ảnh độ phân giải tối thiểu: 1920x1080\n🎨 Được phép chỉnh sửa nhẹ (không được composite hoặc thay đổi nội dung)\n📝 Kèm caption mô tả ý nghĩa bức ảnh (tối đa 200 từ)\n📅 Thời gian chụp: Từ tháng 1/2025 đến tháng 11/2025\n\n🏆 Giải thưởng\n\n🥇 Giải Nhất mỗi hạng mục: 2.000.000 VNĐ + Máy ảnh Fujifilm\n🥈 Giải Nhì mỗi hạng mục: 1.000.000 VNĐ + Lens 50mm\n🥉 Giải Ba mỗi hạng mục: 500.000 VNĐ + Tripod cao cấp\n🌟 Giải Ảnh được yêu thích nhất: 1.000.000 VNĐ (bình chọn online)\n🎖️ Top 10 mỗi hạng mục: Chứng nhận + Quà lưu niệm\n\n📅 Thời gian\n📤 Nộp ảnh: 01/10/2025 – 30/11/2025\n🏆 Công bố kết quả: 15/12/2025\n🎨 Triển lãm: 20-25/12/2025 tại Gallery FPT\n\n💬 Thông điệp\n\"Hạnh phúc không phải là điều ta tìm kiếm, mà là điều ta nhận ra khi ta biết dừng lại và ngắm nhìn.\"","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/dang_hinh_hanh_phuc.png","OrganizerName":"CLB Nhiếp Ảnh FPT","OrganizerInfo":"Câu lạc bộ nhiếp ảnh trực thuộc FPT University"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (31, 1, N'HALLOW VERSE – ĐÊM THƠ HALLOWEEN 2025', N'🎃 "Thơ và ma – Khi văn chương gặp bóng tối"

Hallow Verse là đêm thơ đặc biệt kết hợp giữa nghệ thuật thơ ca và không khí Halloween, nơi các nhà thơ trẻ sẽ trình diễn những tác phẩm thơ mang chủ đề bí ẩn, ma quái và siêu thực.

📜 Nội dung chương trình

🎭 Phần 1 – Thơ Ánh Trăng:
Các bài thơ về bóng tối, ánh trăng và những điều bí ẩn
Trình diễn: Nhà thơ trẻ Tân Lý, Nguyễn Phong Việt

👻 Phần 2 – Câu Chuyện Ma:
Thơ kể chuyện ma và hồn ma
Nghệ sĩ: Các nhà thơ underground và các tác giả trẻ

🕯️ Phần 3 – Hóa Thân:
Thơ performance art kết hợp với ánh sáng và hiệu ứng đặc biệt
Đạo diễn: Nguyễn Duy Hưng

🎨 Đặc biệt

🎭 Hóa trang Halloween tự do (khuyến khích)
🕯️ Không gian được trang trí theo phong cách Gothic
📸 Khu vực chụp ảnh với backdrop Halloween đặc biệt
🍷 Nước uống và snacks miễn phí

🎫 Giá vé

🎟️ Vé thường: 100.000đ
🎟️ Vé VIP (bao gồm gói quà Halloween): 300.000đ

📅 Thời gian & Địa điểm
📅 Ngày: 31 tháng 10 năm 2025
🕘 Giờ: 19h00 – 22h00
📍 Địa điểm: Không gian văn hóa Đông Kinh, Quận 1, TP.HCM

💬 Thông điệp
"Trong bóng tối, ta tìm thấy ánh sáng của lời thơ."

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 7, DATEADD(hour, 3, GETUTCDATE())), N'Không gian văn hóa Đông Kinh, Quận 1, TP.HCM', N'Public', 'Offline', 'Art', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Không gian văn hóa Đông Kinh","StreetAddress":"123 Lê Duẩn","Province":"TP. Hồ Chí Minh","District":"Quận 1","Ward":"Phường Bến Nghé","EventImage":"/assets/images/events/hallow_verse.jpg","BackgroundImage":"/assets/images/events/hallow_verse.jpg","EventIntroduction":"Hallow Verse là đêm thơ đặc biệt kết hợp giữa nghệ thuật thơ ca và không khí Halloween, nơi các nhà thơ trẻ sẽ trình diễn những tác phẩm thơ mang chủ đề bí ẩn, ma quái và siêu thực.","EventDetails":"🎃 \"Thơ và ma – Khi văn chương gặp bóng tối\"\n\nHallow Verse là đêm thơ đặc biệt kết hợp giữa nghệ thuật thơ ca và không khí Halloween, nơi các nhà thơ trẻ sẽ trình diễn những tác phẩm thơ mang chủ đề bí ẩn, ma quái và siêu thực.\n\n📜 Nội dung chương trình\n\n🎭 Phần 1 – Thơ Ánh Trăng:\nCác bài thơ về bóng tối, ánh trăng và những điều bí ẩn\nTrình diễn: Nhà thơ trẻ Tân Lý, Nguyễn Phong Việt\n\n👻 Phần 2 – Câu Chuyện Ma:\nThơ kể chuyện ma và hồn ma\nNghệ sĩ: Các nhà thơ underground và các tác giả trẻ\n\n🕯️ Phần 3 – Hóa Thân:\nThơ performance art kết hợp với ánh sáng và hiệu ứng đặc biệt\nĐạo diễn: Nguyễn Duy Hưng\n\n🎨 Đặc biệt\n\n🎭 Hóa trang Halloween tự do (khuyến khích)\n🕯️ Không gian được trang trí theo phong cách Gothic\n📸 Khu vực chụp ảnh với backdrop Halloween đặc biệt\n🍷 Nước uống và snacks miễn phí\n\n🎫 Giá vé\n\n🎟️ Vé thường: 100.000đ\n🎟️ Vé VIP (bao gồm gói quà Halloween): 300.000đ\n\n📅 Thời gian & Địa điểm\n📅 Ngày: 31 tháng 10 năm 2025\n🕘 Giờ: 19h00 – 22h00\n📍 Địa điểm: Không gian văn hóa Đông Kinh, Quận 1, TP.HCM\n\n💬 Thông điệp\n\"Trong bóng tối, ta tìm thấy ánh sáng của lời thơ.\"","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/hallow_verse.jpg","OrganizerName":"CLB Văn Học FPT","OrganizerInfo":"Câu lạc bộ văn học trực thuộc FPT University"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (32, 1, N'CREATIVE IDENTITY CHALLENGE 2025', N'🎨 "Khám phá bản sắc – Định hình phong cách"

Creative Identity Challenge là cuộc thi thiết kế và sáng tạo nội dung dành cho sinh viên các ngành Thiết kế, Marketing và Truyền thông, nhằm tìm kiếm những ý tưởng sáng tạo độc đáo về bản sắc thương hiệu và phong cách cá nhân.

🎯 Chủ đề năm 2025

"Define Your Identity" – Định nghĩa bản sắc của bạn
Thể hiện qua: Logo design, Brand identity, Social media content, Packaging design

🛠️ Hạng mục thi

📐 Brand Identity Design: Hệ thống nhận diện thương hiệu hoàn chỉnh
📱 Digital Content: Social media posts, banners, infographics
📦 Packaging Design: Thiết kế bao bì sản phẩm
🎨 Illustration: Minh họa và artwork
📹 Video Content: Video quảng cáo ngắn (30-60 giây)

🏆 Giải thưởng

🥇 Giải Nhất toàn giải: 8.000.000 VNĐ + Máy tính bảng Wacom
🥈 Giải Nhì toàn giải: 5.000.000 VNĐ + Khóa học thiết kế 3 tháng
🥉 Giải Ba toàn giải: 3.000.000 VNĐ + Bộ dụng cụ thiết kế
🌟 Giải Sáng tạo xuất sắc: 2.000.000 VNĐ (mỗi hạng mục)
🎖️ Top 20: Chứng nhận + Portfolio review từ chuyên gia

🎁 Quyền lợi

📚 Workshop miễn phí về Branding & Design Thinking
🎨 Portfolio review 1-1 với các chuyên gia trong ngành
🌐 Triển lãm online các tác phẩm xuất sắc
💼 Cơ hội thực tập tại các agency hàng đầu

👨‍🎨 Ban giám khảo

Chị Lê Thị Mai – Creative Director, Dentsu Vietnam
Anh Phạm Văn Hùng – Senior Designer, Ogilvy Vietnam
Chị Nguyễn Thị Lan – Founder, Design Studio XYZ

📅 Thời gian
📤 Nộp bài: 01/11/2025 – 30/11/2025
🏆 Chung kết & Pitching: 10/12/2025
🎨 Triển lãm: 15-20/12/2025

💬 Thông điệp
"Bản sắc không phải là điều ta mặc, mà là điều ta thể hiện."

', DATEADD(day, 7, GETUTCDATE()), DATEADD(day, 60, GETUTCDATE()), NULL, N'Public', 'Online', 'Art', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Gallery FPT","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/creative_identity_challenge.jpg","BackgroundImage":"/assets/images/events/creative_identity_challenge.jpg","EventIntroduction":"Creative Identity Challenge là cuộc thi thiết kế và sáng tạo nội dung dành cho sinh viên các ngành Thiết kế, Marketing và Truyền thông, nhằm tìm kiếm những ý tưởng sáng tạo độc đáo về bản sắc thương hiệu và phong cách cá nhân.","EventDetails":"🎨 \"Khám phá bản sắc – Định hình phong cách\"\n\nCreative Identity Challenge là cuộc thi thiết kế và sáng tạo nội dung dành cho sinh viên các ngành Thiết kế, Marketing và Truyền thông, nhằm tìm kiếm những ý tưởng sáng tạo độc đáo về bản sắc thương hiệu và phong cách cá nhân.\n\n🎯 Chủ đề năm 2025\n\n\"Define Your Identity\" – Định nghĩa bản sắc của bạn\nThể hiện qua: Logo design, Brand identity, Social media content, Packaging design\n\n🛠️ Hạng mục thi\n\n📐 Brand Identity Design: Hệ thống nhận diện thương hiệu hoàn chỉnh\n📱 Digital Content: Social media posts, banners, infographics\n📦 Packaging Design: Thiết kế bao bì sản phẩm\n🎨 Illustration: Minh họa và artwork\n📹 Video Content: Video quảng cáo ngắn (30-60 giây)\n\n🏆 Giải thưởng\n\n🥇 Giải Nhất toàn giải: 8.000.000 VNĐ + Máy tính bảng Wacom\n🥈 Giải Nhì toàn giải: 5.000.000 VNĐ + Khóa học thiết kế 3 tháng\n🥉 Giải Ba toàn giải: 3.000.000 VNĐ + Bộ dụng cụ thiết kế\n🌟 Giải Sáng tạo xuất sắc: 2.000.000 VNĐ (mỗi hạng mục)\n🎖️ Top 20: Chứng nhận + Portfolio review từ chuyên gia\n\n🎁 Quyền lợi\n\n📚 Workshop miễn phí về Branding & Design Thinking\n🎨 Portfolio review 1-1 với các chuyên gia trong ngành\n🌐 Triển lãm online các tác phẩm xuất sắc\n💼 Cơ hội thực tập tại các agency hàng đầu\n\n👨‍🎨 Ban giám khảo\n\nChị Lê Thị Mai – Creative Director, Dentsu Vietnam\nAnh Phạm Văn Hùng – Senior Designer, Ogilvy Vietnam\nChị Nguyễn Thị Lan – Founder, Design Studio XYZ\n\n📅 Thời gian\n📤 Nộp bài: 01/11/2025 – 30/11/2025\n🏆 Chung kết & Pitching: 10/12/2025\n🎨 Triển lãm: 15-20/12/2025\n\n💬 Thông điệp\n\"Bản sắc không phải là điều ta mặc, mà là điều ta thể hiện.\"","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/creative_identity_challenge.jpg","OrganizerName":"FPT Design Club","OrganizerInfo":"Câu lạc bộ thiết kế FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (33, 1, N'GREEN UNIVERSITY – NGÀY HỘI MÔI TRƯỜNG 2025', N'🌱 "Xanh hóa tương lai – Bảo vệ môi trường hôm nay"
Green University là ngày hội môi trường quy mô lớn dành cho sinh viên các trường đại học, nhằm nâng cao nhận thức về bảo vệ môi trường, khuyến khích lối sống xanh và lan tỏa thông điệp bền vững.

🌿 Nội dung chương trình
Workshop "Sống Xanh": Hướng dẫn giảm rác thải nhựa, tái chế và tiết kiệm năng lượng.
Triển lãm dự án xanh: Trưng bày các dự án bảo vệ môi trường từ sinh viên.
Chợ đồ cũ: Trao đổi và tái sử dụng đồ vật, giảm thiểu lãng phí.
Cuộc thi "Campus Xanh": Thiết kế không gian xanh cho khuôn viên trường.
Hoạt động trồng cây: Mỗi người tham gia trồng một cây xanh.

🎯 Mục tiêu
Nâng cao nhận thức về biến đổi khí hậu và bảo vệ môi trường.
Khuyến khích hành động thực tế: giảm rác thải, tiết kiệm năng lượng.
Xây dựng cộng đồng sinh viên quan tâm đến môi trường.
Tạo mạng lưới kết nối các dự án xanh tại các trường đại học.

🏆 Hoạt động nổi bật
Đổi rác lấy quà: Đem theo chai nhựa, giấy cũ để đổi lấy cây xanh, túi vải.
Flash mob "Xanh hóa Campus": Hoạt động tập thể tạo hiệu ứng lan tỏa.
Ký cam kết "Sống Xanh": Cam kết thực hiện các hành động bảo vệ môi trường.

💚 Đối tác
GreenHub Vietnam, WWF Vietnam, 350.org Vietnam

', DATEADD(day, 10, GETUTCDATE()), DATEADD(day, 10, DATEADD(hour, 8, GETUTCDATE())), N'Khuôn viên Trường Đại học FPT, Đà Nẵng', N'Festival', 'Offline', 'Environment', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Khuôn viên Trường Đại học FPT","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/green_uni.jpg","BackgroundImage":"/assets/images/events/green_uni.jpg","EventIntroduction":"Green University là ngày hội môi trường quy mô lớn dành cho sinh viên các trường đại học, nhằm nâng cao nhận thức về bảo vệ môi trường, khuyến khích lối sống xanh và lan tỏa thông điệp bền vững.","EventDetails":"🌱 \"Xanh hóa tương lai – Bảo vệ môi trường hôm nay\"\n\nGreen University là ngày hội môi trường quy mô lớn dành cho sinh viên các trường đại học, nhằm nâng cao nhận thức về bảo vệ môi trường, khuyến khích lối sống xanh và lan tỏa thông điệp bền vững.\n\n🌿 Nội dung chương trình\n\nWorkshop \"Sống Xanh\": Hướng dẫn giảm rác thải nhựa, tái chế và tiết kiệm năng lượng.\nTriển lãm dự án xanh: Trưng bày các dự án bảo vệ môi trường từ sinh viên.\nChợ đồ cũ: Trao đổi và tái sử dụng đồ vật, giảm thiểu lãng phí.\nCuộc thi \"Campus Xanh\": Thiết kế không gian xanh cho khuôn viên trường.\nHoạt động trồng cây: Mỗi người tham gia trồng một cây xanh.\n\n🎯 Mục tiêu\n\nNâng cao nhận thức về biến đổi khí hậu và bảo vệ môi trường.\nKhuyến khích hành động thực tế: giảm rác thải, tiết kiệm năng lượng.\nXây dựng cộng đồng sinh viên quan tâm đến môi trường.\nTạo mạng lưới kết nối các dự án xanh tại các trường đại học.\n\n🏆 Hoạt động nổi bật\n\nĐổi rác lấy quà: Đem theo chai nhựa, giấy cũ để đổi lấy cây xanh, túi vải.\nFlash mob \"Xanh hóa Campus\": Hoạt động tập thể tạo hiệu ứng lan tỏa.\nKý cam kết \"Sống Xanh\": Cam kết thực hiện các hành động bảo vệ môi trường.\n\n💚 Đối tác\n\nGreenHub Vietnam, WWF Vietnam, 350.org Vietnam","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/green_uni.jpg","OrganizerName":"FPT Green Club","OrganizerInfo":"Câu lạc bộ môi trường FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (34, 1, N'2K7 SAY HI – CHÀO MỪNG SINH VIÊN KHÓA MỚI 2025', N'👋 "Xin chào 2K7 – Chào mừng đến với FPT!"
2K7 Say Hi là sự kiện chào đón chính thức dành cho tân sinh viên khóa 2027 (2K7) tại Đại học FPT, tạo không khí thân thiện, gắn kết và giúp các bạn làm quen với môi trường học tập mới.

🎉 Nội dung chương trình
Lễ khai mạc: Chào đón tân sinh viên với không khí sôi động, nhạc sống.
Workshop "Học tập hiệu quả": Chia sẻ kinh nghiệm học tập từ sinh viên khóa trên.
Team Building: Hoạt động kết nối, làm quen giữa các tân sinh viên.
Giới thiệu CLB: Các câu lạc bộ giới thiệu hoạt động và tuyển thành viên.
Gala Dinner: Bữa tiệc chào mừng với các tiết mục văn nghệ đặc sắc.

🎯 Mục tiêu
Giúp tân sinh viên làm quen với môi trường đại học.
Kết nối tân sinh viên với nhau và với cộng đồng FPT.
Giới thiệu các hoạt động ngoại khóa, câu lạc bộ.
Tạo cảm giác thân thuộc và hào hứng cho năm học đầu tiên.

🎁 Quà tặng
Áo đồng phục 2K7 Say Hi.
Bộ dụng cụ học tập FPT.
Sách hướng dẫn sinh viên năm nhất.
Gói ưu đãi từ đối tác.

👥 Ban tổ chức
Đội ngũ sinh viên năm 2-3-4 của FPT University.
Sự hỗ trợ từ Phòng Công tác Sinh viên.

', DATEADD(day, 14, GETUTCDATE()), DATEADD(day, 14, DATEADD(hour, 6, GETUTCDATE())), N'Sân vận động FPT University, Đà Nẵng', N'Orientation', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Sân vận động FPT University","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/2k7_say_hi.png","BackgroundImage":"/assets/images/events/2k7_say_hi.png","EventIntroduction":"2K7 Say Hi là sự kiện chào đón chính thức dành cho tân sinh viên khóa 2027 (2K7) tại Đại học FPT, tạo không khí thân thiện, gắn kết và giúp các bạn làm quen với môi trường học tập mới.","EventDetails":"👋 \"Xin chào 2K7 – Chào mừng đến với FPT!\"\n\n2K7 Say Hi là sự kiện chào đón chính thức dành cho tân sinh viên khóa 2027 (2K7) tại Đại học FPT, tạo không khí thân thiện, gắn kết và giúp các bạn làm quen với môi trường học tập mới.\n\n🎉 Nội dung chương trình\n\nLễ khai mạc: Chào đón tân sinh viên với không khí sôi động, nhạc sống.\nWorkshop \"Học tập hiệu quả\": Chia sẻ kinh nghiệm học tập từ sinh viên khóa trên.\nTeam Building: Hoạt động kết nối, làm quen giữa các tân sinh viên.\nGiới thiệu CLB: Các câu lạc bộ giới thiệu hoạt động và tuyển thành viên.\nGala Dinner: Bữa tiệc chào mừng với các tiết mục văn nghệ đặc sắc.\n\n🎯 Mục tiêu\n\nGiúp tân sinh viên làm quen với môi trường đại học.\nKết nối tân sinh viên với nhau và với cộng đồng FPT.\nGiới thiệu các hoạt động ngoại khóa, câu lạc bộ.\nTạo cảm giác thân thuộc và hào hứng cho năm học đầu tiên.\n\n🎁 Quà tặng\n\nÁo đồng phục 2K7 Say Hi.\nBộ dụng cụ học tập FPT.\nSách hướng dẫn sinh viên năm nhất.\nGói ưu đãi từ đối tác.\n\n👥 Ban tổ chức\n\nĐội ngũ sinh viên năm 2-3-4 của FPT University.\nSự hỗ trợ từ Phòng Công tác Sinh viên.","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/2k7_say_hi.png","OrganizerName":"FPT Student Union","OrganizerInfo":"Hội Sinh viên FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (35, 1, N'TALKSHOW – HÀNH TRÌNH KHỞI NGHIỆP', N'💼 "Từ ý tưởng đến thành công – Hành trình khởi nghiệp của các doanh nhân trẻ"
Talkshow Hành Trình Khởi Nghiệp là chương trình chia sẻ kinh nghiệm và truyền cảm hứng từ các doanh nhân trẻ thành công, giúp các bạn trẻ có định hướng rõ ràng về con đường khởi nghiệp của mình.

🎤 Diễn giả
Anh Nguyễn Văn A – Founder & CEO, Startup X (Thành lập 2020, vốn hóa 10 triệu USD).
Chị Lê Thị B – Co-founder, TechY (Ra mắt sản phẩm được 50,000+ người dùng).
Anh Trần Văn C – Serial Entrepreneur, nhà đầu tư thiên thần.

💡 Nội dung chương trình
Phần 1: "Từ ý tưởng đến MVP": Các bước đầu tiên khi khởi nghiệp.
Phần 2: "Tìm kiếm vốn đầu tư": Cách tiếp cận nhà đầu tư và pitch ý tưởng.
Phần 3: "Xây dựng đội ngũ": Tìm kiếm và giữ chân nhân tài.
Q&A: Trả lời câu hỏi trực tiếp từ khán giả.
Networking: Kết nối với các doanh nhân và nhà đầu tư.

🎯 Đối tượng
Sinh viên có ý tưởng khởi nghiệp.
Người trẻ đang tìm kiếm cơ hội kinh doanh.
Các founder startup ở giai đoạn đầu.

🏆 Quyền lợi
Tài liệu "Startup Guide" từ các diễn giả.
Cơ hội 1-1 mentoring với các doanh nhân.
Kết nối với cộng đồng startup trẻ.

', DATEADD(day, 12, GETUTCDATE()), DATEADD(day, 12, DATEADD(hour, 4, GETUTCDATE())), N'Hội trường lớn FPT University, Đà Nẵng', N'Talkshow', 'Offline', 'Education', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Hội trường lớn FPT University","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/talkshow.jpg","BackgroundImage":"/assets/images/events/talkshow.jpg","EventIntroduction":"Talkshow Hành Trình Khởi Nghiệp là chương trình chia sẻ kinh nghiệm và truyền cảm hứng từ các doanh nhân trẻ thành công, giúp các bạn trẻ có định hướng rõ ràng về con đường khởi nghiệp của mình.","EventDetails":"💼 \"Từ ý tưởng đến thành công – Hành trình khởi nghiệp của các doanh nhân trẻ\"\n\nTalkshow Hành Trình Khởi Nghiệp là chương trình chia sẻ kinh nghiệm và truyền cảm hứng từ các doanh nhân trẻ thành công, giúp các bạn trẻ có định hướng rõ ràng về con đường khởi nghiệp của mình.\n\n🎤 Diễn giả\n\nAnh Nguyễn Văn A – Founder & CEO, Startup X (Thành lập 2020, vốn hóa 10 triệu USD).\nChị Lê Thị B – Co-founder, TechY (Ra mắt sản phẩm được 50,000+ người dùng).\nAnh Trần Văn C – Serial Entrepreneur, nhà đầu tư thiên thần.\n\n💡 Nội dung chương trình\n\nPhần 1: \"Từ ý tưởng đến MVP\": Các bước đầu tiên khi khởi nghiệp.\nPhần 2: \"Tìm kiếm vốn đầu tư\": Cách tiếp cận nhà đầu tư và pitch ý tưởng.\nPhần 3: \"Xây dựng đội ngũ\": Tìm kiếm và giữ chân nhân tài.\nQ&A: Trả lời câu hỏi trực tiếp từ khán giả.\nNetworking: Kết nối với các doanh nhân và nhà đầu tư.\n\n🎯 Đối tượng\n\nSinh viên có ý tưởng khởi nghiệp.\nNgười trẻ đang tìm kiếm cơ hội kinh doanh.\nCác founder startup ở giai đoạn đầu.\n\n🏆 Quyền lợi\n\nTài liệu \"Startup Guide\" từ các diễn giả.\nCơ hội 1-1 mentoring với các doanh nhân.\nKết nối với cộng đồng startup trẻ.","specialGuestsList":"Nguyễn Văn A, Lê Thị B, Trần Văn C","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/talkshow.jpg","OrganizerName":"FPT Entrepreneurship Club","OrganizerInfo":"Câu lạc bộ khởi nghiệp FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

INSERT INTO Event (EventId, HostId, Title, Description, StartTime, EndTime, Location, EventType, EventMode, Category, Status, EventDetails, TermsAndConditions, OrganizerInfo, VenueLayout, CreatedAt, UpdatedAt)
VALUES (36, 1, N'CỐC VIỆT – LỄ HỘI VĂN HÓA VIỆT NAM 2025', N'🇻🇳 "Gìn giữ bản sắc – Phát huy truyền thống"
Cốc Việt là lễ hội văn hóa quy mô lớn nhằm tôn vinh văn hóa truyền thống Việt Nam, từ ẩm thực, trang phục, nghệ thuật đến các trò chơi dân gian, tạo không gian giao lưu văn hóa đặc sắc cho sinh viên.

🍜 Nội dung chương trình
Gian hàng ẩm thực: Phở, bún chả, chè, bánh mì và các món ăn truyền thống.
Trình diễn áo dài: Fashion show áo dài truyền thống và hiện đại.
Biểu diễn nghệ thuật: Múa lân, múa rồng, ca trù, chèo.
Trò chơi dân gian: Kéo co, nhảy bao bố, đập niêu, đi cà kheo.
Workshop văn hóa: Làm bánh tráng, in tranh Đông Hồ, thư pháp.

🎯 Mục tiêu
Gìn giữ và phát huy giá trị văn hóa truyền thống Việt Nam.
Tạo sân chơi văn hóa cho sinh viên tìm hiểu về di sản dân tộc.
Kết nối sinh viên Việt Nam và quốc tế qua văn hóa.
Quảng bá văn hóa Việt Nam đến cộng đồng.

🎨 Điểm nhấn
Khu vực "Làng Việt": Tái hiện không gian làng quê Bắc Bộ.
Triển lãm "Việt Nam qua các thời kỳ": Lịch sử văn hóa qua hiện vật.
Khu vực selfie với backdrop phong cảnh Việt Nam.

🏆 Giải thưởng
Giải "Gian hàng được yêu thích nhất": 2.000.000 VNĐ.
Giải "Trò chơi xuất sắc nhất": 1.000.000 VNĐ.
Nhiều phần quà lưu niệm văn hóa.

', DATEADD(day, 18, GETUTCDATE()), DATEADD(day, 18, DATEADD(hour, 10, GETUTCDATE())), N'Khuôn viên FPT University, Đà Nẵng', N'Festival', 'Offline', 'Culture', 'Open', N'{"venue":null,"images":null,"introduction":null,"specialGuests":null,"VenueName":"Khuôn viên FPT University","StreetAddress":"123 Lê Duẩn","Province":"Đà Nẵng","District":"Quận Hải Châu","Ward":"Phường Hòa Thuận Đông","EventImage":"/assets/images/events/coc_viet.jpg","BackgroundImage":"/assets/images/events/coc_viet.jpg","EventIntroduction":"Cốc Việt là lễ hội văn hóa quy mô lớn nhằm tôn vinh văn hóa truyền thống Việt Nam, từ ẩm thực, trang phục, nghệ thuật đến các trò chơi dân gian, tạo không gian giao lưu văn hóa đặc sắc cho sinh viên.","EventDetails":"🇻🇳 \"Gìn giữ bản sắc – Phát huy truyền thống\"\n\nCốc Việt là lễ hội văn hóa quy mô lớn nhằm tôn vinh văn hóa truyền thống Việt Nam, từ ẩm thực, trang phục, nghệ thuật đến các trò chơi dân gian, tạo không gian giao lưu văn hóa đặc sắc cho sinh viên.\n\n🍜 Nội dung chương trình\n\nGian hàng ẩm thực: Phở, bún chả, chè, bánh mì và các món ăn truyền thống.\nTrình diễn áo dài: Fashion show áo dài truyền thống và hiện đại.\nBiểu diễn nghệ thuật: Múa lân, múa rồng, ca trù, chèo.\nTrò chơi dân gian: Kéo co, nhảy bao bố, đập niêu, đi cà kheo.\nWorkshop văn hóa: Làm bánh tráng, in tranh Đông Hồ, thư pháp.\n\n🎯 Mục tiêu\n\nGìn giữ và phát huy giá trị văn hóa truyền thống Việt Nam.\nTạo sân chơi văn hóa cho sinh viên tìm hiểu về di sản dân tộc.\nKết nối sinh viên Việt Nam và quốc tế qua văn hóa.\nQuảng bá văn hóa Việt Nam đến cộng đồng.\n\n🎨 Điểm nhấn\n\nKhu vực \"Làng Việt\": Tái hiện không gian làng quê Bắc Bộ.\nTriển lãm \"Việt Nam qua các thời kỳ\": Lịch sử văn hóa qua hiện vật.\nKhu vực selfie với backdrop phong cảnh Việt Nam.\n\n🏆 Giải thưởng\n\nGiải \"Gian hàng được yêu thích nhất\": 2.000.000 VNĐ.\nGiải \"Trò chơi xuất sắc nhất\": 1.000.000 VNĐ.\nNhiều phần quà lưu niệm văn hóa.","specialGuestsList":"","SpecialExperience":""}', N'{"TermsAndConditions":"","ChildrenTerms":"","VATTerms":""}', N'{"OrganizerLogo":"/assets/images/events/coc_viet.jpg","OrganizerName":"FPT Culture Club","OrganizerInfo":"Câu lạc bộ văn hóa FPT"}', NULL, GETUTCDATE(), GETUTCDATE());

SET IDENTITY_INSERT Event OFF;

-- Insert Ticket Types
SET IDENTITY_INSERT TicketType ON;

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (57, 4, N'Vé Thường', 180000.00, 150, 1, 6, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (58, 4, N'Vé VIP', 320000.00, 50, 1, 4, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (19, 9, N'Vé Thường', 100000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (20, 9, N'Vé VIP', 500000.00, 50, 1, 5, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (45, 18, N'Hạng VVIP', 1500000.00, 10, 1, 3, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (46, 18, N'Hạng VIP', 900000.00, 30, 1, 6, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (47, 18, N'Hạng Thường', 500000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (59, 22, N'Vé Thường', 100000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (60, 23, N'Vé Thường', 100000.00, 50, 1, 2, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (61, 23, N'Vé VIP', 500000.00, 10, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (62, 24, N'Khăn Thường', 10000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (63, 24, N'Khăn Kaki', 15000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (64, 25, N'Ngành SE', 10000.00, 100, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (65, 25, N'Ngành AI', 10000.00, 100, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (66, 25, N'Ngành ATTT', 10000.00, 100, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (67, 26, N'Vé Thường', 1000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (68, 26, N'Vé VIP', 100000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (69, 27, N'Thi Thường', 100000.00, 100, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (70, 27, N'Thi Thường + Cheat', 10000000.00, 10, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (71, 28, N'Vé Tham Gia', 50000.00, 200, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (72, 29, N'Vé Đội Thi', 100000.00, 50, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 8, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (73, 30, N'Gói Tham Gia', 50000.00, 300, 1, 5, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 59, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (74, 31, N'Vé Thường', 100000.00, 100, 1, 5, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (75, 31, N'Vé VIP', 300000.00, 30, 1, 3, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 6, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (76, 32, N'Gói Tham Gia', 100000.00, 200, 1, 5, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 59, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (77, 33, N'Vé Tham Gia', 50000.00, 300, 1, 10, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 9, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (78, 34, N'Vé Tân Sinh Viên', 0.00, 500, 1, 1, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 13, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (79, 35, N'Vé Thường', 150000.00, 200, 1, 5, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 11, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (80, 35, N'Vé VIP', 300000.00, 50, 1, 3, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 11, GETUTCDATE()), 'Active');

INSERT INTO TicketType (TicketTypeId, EventId, TypeName, Price, Quantity, MinOrder, MaxOrder, SaleStart, SaleEnd, Status)
VALUES (81, 36, N'Vé Tham Gia', 100000.00, 400, 1, 8, DATEADD(day, -30, GETUTCDATE()), DATEADD(day, 17, GETUTCDATE()), 'Active');

SET IDENTITY_INSERT TicketType OFF;

-- Verification
SELECT 'Users:' as Info, COUNT(*) as Count FROM [User];
SELECT 'Events:' as Info, COUNT(*) as Count FROM Event;
SELECT 'Ticket Types:' as Info, COUNT(*) as Count FROM TicketType;
SELECT 'Campus:' as Info, COUNT(*) as Count FROM Campus;
PRINT 'Sample data loaded successfully!';

GO

-- Hiển thị thông tin Admin account
SELECT 
    UserId,
    Username,
    FullName,
    Email,
    Role,
    WalletBalance,
    CreatedAt
FROM [User]
WHERE Email = 'admin@thegrind5.com';

GO