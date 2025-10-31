-- ========================================
-- TheGrind5 Event Management
-- Sample Orders Insert Script
-- ========================================
-- 
-- Script này tạo 4 orders mẫu để test tính năng Quản lý Order
-- Cần chạy SampleData_Insert.sql trước để có Users, Events và TicketTypes
-- ========================================

USE EventDB
GO

-- ========================================
-- VERIFY PREREQUISITES
-- ========================================
-- Kiểm tra xem đã có Users, Events và TicketTypes chưa
IF NOT EXISTS (SELECT 1 FROM [User] WHERE Role = 'Customer')
BEGIN
    PRINT '⚠️ ERROR: Chưa có Customer trong database. Vui lòng chạy SampleData_Insert.sql trước!';
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM Event)
BEGIN
    PRINT '⚠️ ERROR: Chưa có Event trong database. Vui lòng chạy SampleData_Insert.sql trước!';
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM TicketType)
BEGIN
    PRINT '⚠️ ERROR: Chưa có TicketType trong database. Vui lòng chạy SampleData_Insert.sql trước!';
    RETURN;
END

PRINT '✅ Prerequisites check passed. Starting to insert sample orders...';
GO

-- ========================================
-- CLEAR EXISTING ORDERS (Optional - comment out if you want to keep existing orders)
-- ========================================
-- DELETE FROM Payment;
-- DELETE FROM Ticket;
-- DELETE FROM OrderItem;
-- DELETE FROM [Order];
-- DBCC CHECKIDENT ('Payment', RESEED, 0);
-- DBCC CHECKIDENT ('Ticket', RESEED, 0);
-- DBCC CHECKIDENT ('OrderItem', RESEED, 0);
-- DBCC CHECKIDENT ('[Order]', RESEED, 0);
-- PRINT 'Cleared existing orders';
GO

-- ========================================
-- INSERT SAMPLE ORDERS
-- ========================================
-- Lấy EventId và TicketTypeId từ database dựa trên Title và TypeName

-- Kiểm tra và hiển thị Events có sẵn
PRINT '📋 Checking available Events...';
SELECT EventId, Title FROM Event ORDER BY EventId;
PRINT '';

-- Order 1: Customer 1 mua Event 1 - Vé Thường (2 vé)
DECLARE @OrderId1 INT;
DECLARE @CustomerId1 INT;
DECLARE @EventId1 INT;
DECLARE @TicketTypeId1 INT;

SELECT @CustomerId1 = UserId FROM [User] WHERE Email = 'customer1@example.com';
SELECT @EventId1 = EventId FROM Event WHERE Title = N'Workshop Lập Trình Web';
SELECT @TicketTypeId1 = TicketTypeId FROM TicketType WHERE EventId = @EventId1 AND TypeName = N'Vé Thường';

IF @CustomerId1 IS NULL OR @EventId1 IS NULL OR @TicketTypeId1 IS NULL
BEGIN
    PRINT '⚠️ ERROR Order 1: Không tìm thấy dữ liệu cần thiết!';
    PRINT '   CustomerId: ' + ISNULL(CAST(@CustomerId1 AS VARCHAR), 'NULL');
    PRINT '   EventId: ' + ISNULL(CAST(@EventId1 AS VARCHAR), 'NULL');
    PRINT '   TicketTypeId: ' + ISNULL(CAST(@TicketTypeId1 AS VARCHAR), 'NULL');
END
ELSE
BEGIN
    INSERT INTO [Order] (CustomerId, EventId, Amount, Status, PaymentMethod, CreatedAt, UpdatedAt)
    VALUES (
        @CustomerId1,
        @EventId1,
        300000.00,
        'Paid',
        'Wallet',
        DATEADD(day, -5, GETUTCDATE()),
        DATEADD(day, -5, GETUTCDATE())
    );

    SET @OrderId1 = SCOPE_IDENTITY();

    IF @OrderId1 IS NOT NULL
    BEGIN
        INSERT INTO OrderItem (OrderId, TicketTypeId, Quantity, SeatNo, Status)
        VALUES (@OrderId1, @TicketTypeId1, 2, NULL, 'Confirmed');
        
        PRINT '✅ Inserted Order 1: Customer 1 - Workshop Lập Trình Web - Vé Thường (2 vé) - 300,000 VND';
    END
    ELSE
    BEGIN
        PRINT '❌ Failed to insert Order 1';
    END
END
GO

-- Order 2: Customer 2 mua Event 1 - Vé VIP (1 vé)
DECLARE @OrderId2 INT;
DECLARE @CustomerId2 INT;
DECLARE @EventId1_2 INT;
DECLARE @TicketTypeId2 INT;

SELECT @CustomerId2 = UserId FROM [User] WHERE Email = 'customer2@example.com';
SELECT @EventId1_2 = EventId FROM Event WHERE Title = N'Workshop Lập Trình Web';
SELECT @TicketTypeId2 = TicketTypeId FROM TicketType WHERE EventId = @EventId1_2 AND TypeName = N'Vé VIP';

IF @CustomerId2 IS NULL OR @EventId1_2 IS NULL OR @TicketTypeId2 IS NULL
BEGIN
    PRINT '⚠️ ERROR Order 2: Không tìm thấy dữ liệu cần thiết!';
    PRINT '   CustomerId: ' + ISNULL(CAST(@CustomerId2 AS VARCHAR), 'NULL');
    PRINT '   EventId: ' + ISNULL(CAST(@EventId1_2 AS VARCHAR), 'NULL');
    PRINT '   TicketTypeId: ' + ISNULL(CAST(@TicketTypeId2 AS VARCHAR), 'NULL');
END
ELSE
BEGIN
    INSERT INTO [Order] (CustomerId, EventId, Amount, Status, PaymentMethod, CreatedAt, UpdatedAt)
    VALUES (
        @CustomerId2,
        @EventId1_2,
        250000.00,
        'Paid',
        'Wallet',
        DATEADD(day, -3, GETUTCDATE()),
        DATEADD(day, -3, GETUTCDATE())
    );

    SET @OrderId2 = SCOPE_IDENTITY();

    IF @OrderId2 IS NOT NULL
    BEGIN
        INSERT INTO OrderItem (OrderId, TicketTypeId, Quantity, SeatNo, Status)
        VALUES (@OrderId2, @TicketTypeId2, 1, 'A-12', 'Confirmed');
        
        PRINT '✅ Inserted Order 2: Customer 2 - Workshop Lập Trình Web - Vé VIP (1 vé) - 250,000 VND';
    END
    ELSE
    BEGIN
        PRINT '❌ Failed to insert Order 2';
    END
END
GO

-- Order 3: Customer 1 mua Event 2 - Vé Premium (1 vé)
DECLARE @OrderId3 INT;
DECLARE @CustomerId1_3 INT;
DECLARE @EventId2_3 INT;
DECLARE @TicketTypeId3 INT;

SELECT @CustomerId1_3 = UserId FROM [User] WHERE Email = 'customer1@example.com';
SELECT @EventId2_3 = EventId FROM Event WHERE Title = N'Hội Thảo AI & Machine Learning';
SELECT @TicketTypeId3 = TicketTypeId FROM TicketType WHERE EventId = @EventId2_3 AND TypeName = N'Vé Premium';

IF @CustomerId1_3 IS NULL OR @EventId2_3 IS NULL OR @TicketTypeId3 IS NULL
BEGIN
    PRINT '⚠️ ERROR Order 3: Không tìm thấy dữ liệu cần thiết!';
    PRINT '   CustomerId: ' + ISNULL(CAST(@CustomerId1_3 AS VARCHAR), 'NULL');
    PRINT '   EventId: ' + ISNULL(CAST(@EventId2_3 AS VARCHAR), 'NULL');
    PRINT '   TicketTypeId: ' + ISNULL(CAST(@TicketTypeId3 AS VARCHAR), 'NULL');
END
ELSE
BEGIN
    INSERT INTO [Order] (CustomerId, EventId, Amount, Status, PaymentMethod, CreatedAt, UpdatedAt)
    VALUES (
        @CustomerId1_3,
        @EventId2_3,
        350000.00,
        'Paid',
        'Wallet',
        DATEADD(day, -2, GETUTCDATE()),
        DATEADD(day, -2, GETUTCDATE())
    );

    SET @OrderId3 = SCOPE_IDENTITY();

    IF @OrderId3 IS NOT NULL
    BEGIN
        INSERT INTO OrderItem (OrderId, TicketTypeId, Quantity, SeatNo, Status)
        VALUES (@OrderId3, @TicketTypeId3, 1, NULL, 'Confirmed');
        
        PRINT '✅ Inserted Order 3: Customer 1 - Hội Thảo AI & Machine Learning - Vé Premium (1 vé) - 350,000 VND';
    END
    ELSE
    BEGIN
        PRINT '❌ Failed to insert Order 3';
    END
END
GO

-- Order 4: Customer 2 mua Event 3 - Vé Thường (2 vé)
DECLARE @OrderId4 INT;
DECLARE @CustomerId2_4 INT;
DECLARE @EventId3_4 INT;
DECLARE @TicketTypeId4 INT;

SELECT @CustomerId2_4 = UserId FROM [User] WHERE Email = 'customer2@example.com';
SELECT @EventId3_4 = EventId FROM Event WHERE Title = N'Sự Kiện Networking Startup';
SELECT @TicketTypeId4 = TicketTypeId FROM TicketType WHERE EventId = @EventId3_4 AND TypeName = N'Vé Thường';

IF @CustomerId2_4 IS NULL OR @EventId3_4 IS NULL OR @TicketTypeId4 IS NULL
BEGIN
    PRINT '⚠️ ERROR Order 4: Không tìm thấy dữ liệu cần thiết!';
    PRINT '   CustomerId: ' + ISNULL(CAST(@CustomerId2_4 AS VARCHAR), 'NULL');
    PRINT '   EventId: ' + ISNULL(CAST(@EventId3_4 AS VARCHAR), 'NULL');
    PRINT '   TicketTypeId: ' + ISNULL(CAST(@TicketTypeId4 AS VARCHAR), 'NULL');
END
ELSE
BEGIN
    INSERT INTO [Order] (CustomerId, EventId, Amount, Status, PaymentMethod, CreatedAt, UpdatedAt)
    VALUES (
        @CustomerId2_4,
        @EventId3_4,
        600000.00,
        'Paid',
        'Wallet',
        DATEADD(day, -1, GETUTCDATE()),
        DATEADD(day, -1, GETUTCDATE())
    );

    SET @OrderId4 = SCOPE_IDENTITY();

    IF @OrderId4 IS NOT NULL
    BEGIN
        INSERT INTO OrderItem (OrderId, TicketTypeId, Quantity, SeatNo, Status)
        VALUES (@OrderId4, @TicketTypeId4, 2, NULL, 'Confirmed');
        
        PRINT '✅ Inserted Order 4: Customer 2 - Sự Kiện Networking Startup - Vé Thường (2 vé) - 600,000 VND';
    END
    ELSE
    BEGIN
        PRINT '❌ Failed to insert Order 4';
    END
END
GO

-- ========================================
-- VERIFICATION QUERIES
-- ========================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICATION - Sample Orders Created';
PRINT '========================================';
PRINT '';

-- Tổng số orders
SELECT 'Total Orders:' as Info, COUNT(*) as Count FROM [Order];
PRINT '';

-- Chi tiết orders với thông tin khách hàng và sự kiện (bao gồm số lượng vé)
SELECT 
    o.OrderId as 'ID Vé',
    u.FullName as 'Tên người mua',
    u.Email as 'Email',
    e.Title as 'Sự kiện',
    STUFF((
        SELECT ', ' + tt2.TypeName + ' (' + CAST(oi2.Quantity AS VARCHAR) + ' vé)'
        FROM OrderItem oi2
        INNER JOIN TicketType tt2 ON oi2.TicketTypeId = tt2.TicketTypeId
        WHERE oi2.OrderId = o.OrderId
        FOR XML PATH('')
    ), 1, 2, '') as 'Vé đã mua',
    (SELECT SUM(oi3.Quantity) FROM OrderItem oi3 WHERE oi3.OrderId = o.OrderId) as 'Số lượng vé',
    FORMAT(o.Amount, 'N0') + ' VND' as 'Số tiền',
    o.Status as 'Trạng thái',
    FORMAT(o.CreatedAt, 'dd/MM/yyyy HH:mm') as 'Thời gian mua'
FROM [Order] o
INNER JOIN [User] u ON o.CustomerId = u.UserId
INNER JOIN Event e ON o.EventId = e.EventId
GROUP BY o.OrderId, u.FullName, u.Email, e.Title, o.Amount, o.Status, o.CreatedAt
ORDER BY o.OrderId;

PRINT '';
PRINT '========================================';
PRINT '✅ Sample Orders Insertion Complete!';
PRINT '========================================';
PRINT '';
PRINT '📋 Tóm tắt 4 orders đã tạo:';
PRINT '   1. Order #1: Lê Văn Customer - Workshop Lập Trình Web - Vé Thường (2 vé) - 300,000 VND';
PRINT '   2. Order #2: Phạm Thị Test - Workshop Lập Trình Web - Vé VIP (1 vé) - 250,000 VND';
PRINT '   3. Order #3: Lê Văn Customer - Hội Thảo AI & Machine Learning - Vé Premium (1 vé) - 350,000 VND';
PRINT '   4. Order #4: Phạm Thị Test - Sự Kiện Networking Startup - Vé Thường (2 vé) - 600,000 VND';
PRINT '';
PRINT '🌐 Bây giờ bạn có thể truy cập trang Quản lý Order để xem dữ liệu!';

