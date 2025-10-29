# Sample Data - User Passwords

## Thông tin Users được tạo trong Database

### Host Users (2 users)

#### Host 1
- **Username**: `host1`
- **Full Name**: Nguyễn Văn Host
- **Email**: host1@example.com
- **Password**: `host123`
- **Phone**: 0123456789
- **Role**: Host

#### Host 2
- **Username**: `host2`
- **Full Name**: Trần Thị Host
- **Email**: host2@example.com
- **Password**: `host456`
- **Phone**: 0987654321
- **Role**: Host

### Customer User (1 user)

#### Customer 1
- **Username**: `customer1`
- **Full Name**: Lê Văn Customer
- **Email**: customer1@example.com
- **Password**: `customer789`
- **Phone**: 0555123456
- **Role**: Customer

## Events được tạo

### Events của Host 1 (3 events)
1. **Workshop Lập Trình Web** - Technology (Hà Nội)
2. **Hội Thảo AI & Machine Learning** - Technology (TP.HCM)
3. **Sự Kiện Networking Startup** - Business (Đà Nẵng)

### Events của Host 2 (3 events)
1. **Concert Nhạc Acoustic** - Entertainment (Hà Nội)
2. **Triển Lãm Nghệ Thuật Đương Đại** - Art (TP.HCM)
3. **Workshop Nấu Ăn Healthy** - Lifestyle (Hà Nội)

## Cách sử dụng

1. **Chạy SQL Script**: Mở file `SampleData_Insert.sql` và chạy trong SQL Server Management Studio hoặc Azure Data Studio
2. **Kết nối Database**: Đảm bảo kết nối đến database `TheGrind5_EventManagement`
3. **Kiểm tra kết quả**: Script sẽ hiển thị thông tin xác nhận sau khi chạy thành công

## Lưu ý
- Tất cả passwords đã được hash bằng BCrypt trong database
- Passwords gốc được ghi ở đây để test login
- Database sẽ được xóa dữ liệu cũ trước khi insert dữ liệu mới
- Script bao gồm các query verification để kiểm tra dữ liệu đã được tạo
