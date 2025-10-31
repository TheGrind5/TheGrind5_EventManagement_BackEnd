# Hướng dẫn khởi động lại Backend để sử dụng tính năng Quản lý Order

## Vấn đề:
Backend đang chạy với code cũ, chưa có endpoint `/api/admin/orders`. Cần restart backend để load endpoint mới.

## Các bước:

### 1. Dừng Backend hiện tại:
- Tìm process `TheGrind5_EventManagement` trong Task Manager và dừng nó
- Hoặc nhấn `Ctrl+C` trong terminal đang chạy backend

### 2. Build lại Backend:
```bash
cd src
dotnet build
```

### 3. Chạy lại Backend:
```bash
dotnet run
```

### 4. Kiểm tra Backend đã chạy:
- Mở browser: http://localhost:5000/swagger
- Tìm endpoint `GET /api/admin/orders`
- Nếu thấy endpoint này, backend đã load code mới

### 5. Test API trực tiếp:
- Mở browser hoặc Postman
- GET: `http://localhost:5000/api/admin/orders`
- Header: `Authorization: Bearer <token_admin>`
- Nếu trả về data, API đã hoạt động

### 6. Refresh Frontend:
- F5 hoặc Ctrl+R để refresh trang
- Mở F12 → Console để xem log

## Lưu ý:
- Backend phải chạy trên port 5000 (hoặc port được cấu hình trong appsettings.json)
- Frontend phải được cấu hình đúng API_URL trong environment.js
- Token Admin phải hợp lệ và chưa hết hạn

