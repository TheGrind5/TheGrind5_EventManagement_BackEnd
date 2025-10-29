# Sample Images for TheGrind5 Event Management

## Mục đích
Thư mục này chứa **ảnh mẫu** để seed database khi clone dự án về.

## Cấu trúc thư mục

```
assets/images/
├── avatars/          # Avatar của users mẫu
│   ├── user_1.jpg   # Avatar cho host1@example.com
│   ├── user_2.jpg   # Avatar cho host2@example.com
│   ├── user_3.jpg   # Avatar cho customer1@example.com
│   ├── user_4.jpg   # Avatar cho customer2@example.com
│   └── user_5.jpg   # Avatar cho testwallet@example.com
│
└── events/          # Ảnh sự kiện mẫu
    ├── workshop1.jpg
    ├── workshop2.jpg
    ├── ai1.jpg
    ├── ai2.jpg
    ├── networking1.jpg
    ├── networking2.jpg
    ├── concert1.jpg
    ├── concert2.jpg
    ├── art1.jpg
    ├── art2.jpg
    ├── cooking1.jpg
    └── cooking2.jpg
```

## Hướng dẫn

### 1. Chuẩn bị ảnh mẫu

Đặt các file ảnh vào thư mục tương ứng:
- `avatars/`: Ảnh đại diện cho 5 users mẫu (user_1.jpg đến user_5.jpg)
- `events/`: Ảnh cho 6 events mẫu (mỗi event có 2 ảnh)

**Lưu ý**: Đặt tên file **chính xác** như trong danh sách trên để khớp với `SampleData_Insert.sql`

### 2. Chạy seed data

Sau khi clone dự án về:

```sql
-- Chạy file SQL để tạo database và seed data
USE EventDB;
GO
EXEC sp_executesql @sql = N'<nội dung SampleData_Insert.sql>';
```

Hoặc mở SQL Server Management Studio và chạy file `SampleData_Insert.sql`

### 3. Kiểm tra

Sau khi seed:
- Database có 5 users, 6 events, ticket types, vouchers
- Ảnh hiển thị đầy đủ khi truy cập:
  - `http://localhost:5000/assets/images/avatars/user_1.jpg`
  - `http://localhost:5000/assets/images/events/workshop1.jpg`

## Định dạng ảnh

- **Định dạng hỗ trợ**: JPG, JPEG, PNG, GIF, WEBP
- **Kích thước khuyến nghị**:
  - Avatar: 200x200px hoặc 400x400px (vuông)
  - Event images: 1200x630px hoặc 1920x1080px (phù hợp cho banner)
- **Dung lượng**: Tối đa 5MB/file

## Lưu ý quan trọng

⚠️ **Thư mục này được commit vào Git** (không bị ignore)
- Chỉ đặt ảnh mẫu để seed database
- Không đặt ảnh cá nhân/nhạy cảm
- Ảnh upload từ users sẽ được lưu vào `src/wwwroot/uploads/` (bị ignore)

## Placeholder images

Nếu chưa có ảnh thật, có thể dùng placeholder:
- https://placehold.co/400x400 (cho avatars)
- https://placehold.co/1200x630 (cho events)

Hoặc dùng free stock images:
- https://unsplash.com/
- https://pexels.com/

