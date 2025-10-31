# Hướng dẫn sửa lỗi: Invalid object name 'Notification'

## Vấn đề
Bảng `Notification` chưa tồn tại trong database, dẫn đến lỗi khi NotificationService cố gắng query dữ liệu.

## Giải pháp

### Cách 1: Chạy script SQL (Nhanh nhất)
1. Mở SQL Server Management Studio hoặc Azure Data Studio
2. Kết nối đến database của project
3. Chạy file `CREATE_NOTIFICATION_TABLE.sql` trong thư mục backend
4. Hoặc chạy script sau:

```sql
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notification')
BEGIN
    CREATE TABLE [Notification] (
        [NotificationId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Content] NVARCHAR(MAX) NULL,
        [Type] NVARCHAR(MAX) NOT NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL,
        [ReadAt] DATETIME2 NULL,
        [RelatedEventId] INT NULL,
        [RelatedOrderId] INT NULL,
        [RelatedTicketId] INT NULL,
        
        CONSTRAINT [PK_Notification] PRIMARY KEY ([NotificationId]),
        CONSTRAINT [FK_Notification_User_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [User] ([UserID]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Notification_UserId] ON [Notification] ([UserId]);
END
```

### Cách 2: Chạy migration bằng EF Core
1. Mở terminal trong thư mục `src`
2. Chạy lệnh:
```bash
dotnet ef database update
```

Hoặc nếu chỉ muốn apply migration cụ thể:
```bash
dotnet ef database update AddNotificationTable
```

## Đã sửa
✅ Thêm exception handling trong NotificationService để không crash khi bảng chưa tồn tại
✅ Thêm notification service vào OrderService để gửi thông báo khi tạo order và payment thành công
✅ Tạo script SQL để tạo bảng

## Sau khi tạo bảng
Sau khi chạy script SQL hoặc migration, restart backend và các thông báo sẽ hoạt động bình thường:
- Thông báo khi tạo order thành công
- Thông báo khi payment thành công
- Thông báo khi refund

