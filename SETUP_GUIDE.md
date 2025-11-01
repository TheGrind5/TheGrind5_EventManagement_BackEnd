# Hướng Dẫn Cấu Hình Project

## 1. Cấu hình Database Connection String

File `src/appsettings.json` đã được cấu hình mặc định cho **LocalDB**. Nếu bạn dùng SQL Server khác, hãy sửa connection string:

### Các tùy chọn phổ biến:

#### LocalDB (Mặc định - phù hợp nếu có Visual Studio):
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

#### SQL Server Express:
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=EventDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
```
Hoặc nếu có tên instance khác:
```json
"DefaultConnection": "Server=YOUR_COMPUTER_NAME\\SQLEXPRESS;Database=EventDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

#### SQL Server với Username/Password:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=EventDB;User Id=your_username;Password=your_password;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

## 2. Khởi tạo Database

### Bước 1: Chạy script tạo database
```bash
# Mở SQL Server Management Studio (SSMS) hoặc Azure Data Studio
# Kết nối đến SQL Server của bạn
# Mở và chạy file: TheGrind5_Query.sql
```

### Bước 2: Chạy script insert dữ liệu mẫu (tùy chọn)
```bash
# Sau khi tạo database thành công
# Chạy file: ExtendedSampleData_Insert.sql
```

## 3. Cấu hình JWT (Đã có sẵn, có thể giữ nguyên)

JWT key trong `appsettings.json` đã được cấu hình. Bạn có thể giữ nguyên hoặc thay đổi cho bảo mật hơn.

## 4. Cấu hình VNPay (Tùy chọn - chỉ cần nếu test thanh toán)

Nếu muốn test tính năng thanh toán VNPay, bạn cần:
1. Đăng ký tài khoản VNPay Sandbox tại: https://sandbox.vnpayment.vn/
2. Cập nhật các thông tin sau trong `appsettings.json`:
   - `TmnCode`: Mã terminal của bạn
   - `HashSecret`: Mã hash secret của bạn

## 5. Cấu hình HuggingFace AI (Tùy chọn - chỉ cần nếu dùng AI features)

Nếu muốn dùng tính năng AI suggestion:
1. Đăng ký tài khoản tại: https://huggingface.co/
2. Lấy API key từ: https://huggingface.co/settings/tokens
3. Cập nhật `HuggingFace.ApiKey` trong `appsettings.json`

## 6. Chạy Tests

### Sử dụng run.bat (Windows):
```bash
cd TheGrind5_EventManagement.Tests
run.bat
```

### Hoặc chạy trực tiếp:
```bash
cd TheGrind5_EventManagement.Tests
dotnet test
```

## 7. Chạy Application

```bash
cd src
dotnet run
```

Hoặc chạy từ Visual Studio/VS Code.

## Lưu ý:

- Nếu gặp lỗi connection string, kiểm tra:
  1. SQL Server đã được cài đặt và đang chạy
  2. SQL Server Browser service đang chạy (nếu dùng named instance)
  3. Tên server/instance đúng với máy của bạn
  4. Database EventDB đã được tạo

- Để kiểm tra tên SQL Server instance trên máy của bạn:
  ```bash
  # Mở SQL Server Configuration Manager
  # Hoặc chạy lệnh trong SQL Server Management Studio:
  SELECT @@SERVERNAME;
  ```

## Troubleshooting

### Lỗi: "Cannot open database 'EventDB'"
→ Chạy lại file `TheGrind5_Query.sql` để tạo database

### Lỗi: "Login failed for user"
→ Kiểm tra connection string và đảm bảo SQL Server đang chạy

### Lỗi: "A network-related or instance-specific error"
→ Kiểm tra SQL Server Browser service đang chạy
→ Thử đổi connection string sang `(localdb)\mssqllocaldb` hoặc `.` hoặc `localhost`

