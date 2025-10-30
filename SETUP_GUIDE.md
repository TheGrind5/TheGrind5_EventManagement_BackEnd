# TheGrind5 Event Management - Setup Guide

## 📋 Hướng dẫn Clone và Setup Dự án

Hướng dẫn chi tiết để clone dự án và có sẵn dữ liệu mẫu (bao gồm cả hình ảnh).

---

## 🚀 Các bước Setup

### Bước 1: Clone Repository

```bash
git clone https://github.com/your-repo/TheGrind5_EventManagement.git
cd TheGrind5_EventManagement
```

### Bước 2: Kiểm tra Ảnh Mẫu

Sau khi clone, kiểm tra thư mục `assets/images/` đã có các file ảnh mẫu:

```
assets/
└── images/
    ├── avatars/
    │   ├── user_1.jpg
    │   ├── user_2.jpg
    │   ├── user_3.jpg
    │   ├── user_4.jpg
    │   └── user_5.jpg
    └── events/
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

**Lưu ý**: Nếu thư mục trống, hãy đọc [assets/images/README.md](assets/images/README.md) để biết cách chuẩn bị ảnh.

### Bước 3: Cấu hình Database

1. Mở SQL Server Management Studio (SSMS)
2. Kết nối tới SQL Server instance của bạn
3. Tạo database mới:

```sql
CREATE DATABASE EventDB;
GO
```

### Bước 4: Chạy Migration

Có 2 cách để tạo database schema:

#### Cách 1: Dùng Entity Framework Migration (Khuyến nghị)

```bash
cd src
dotnet ef database update
```

#### Cách 2: Chạy SQL Script thủ công

Mở file `Diagram/TheGrind5_Optimized_Database.sql` và chạy trong SSMS.

### Bước 5: Seed Dữ liệu Mẫu

Mở file `SampleData_Insert.sql` và chạy trong SSMS.

**Dữ liệu được seed:**
- ✅ 5 Users (2 Hosts, 3 Customers) với avatars
- ✅ 6 Events (mỗi event có 2 ảnh)
- ✅ 11 Ticket Types
- ✅ 5 Vouchers

**Thông tin đăng nhập:**

| Email | Password | Role | Wallet Balance |
|-------|----------|------|----------------|
| host1@example.com | 123456 | Host | 0 VND |
| host2@example.com | 123456 | Host | 0 VND |
| customer1@example.com | 123456 | Customer | 500,000 VND |
| customer2@example.com | 123456 | Customer | 1,250,000.50 VND |
| testwallet@example.com | 123456 | Customer | 999,999.99 VND |

### Bước 6: Cấu hình Backend

1. Copy file cấu hình mẫu:

```bash
cd src
cp appsettings.Example.json appsettings.json
```

2. Cập nhật connection string trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EventDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

3. Cấu hình JWT (nếu cần):

```json
{
  "Jwt": {
    "Key": "YourSecretKeyHere-MustBeAtLeast32Characters",
    "Issuer": "TheGrind5EventManagement",
    "Audience": "TheGrind5Users"
  }
}
```

### Bước 7: Chạy Backend

```bash
cd src
dotnet run
```

Backend sẽ chạy tại: `http://localhost:5000`

### Bước 8: Kiểm tra Ảnh

Mở trình duyệt và truy cập:
- Avatar: `http://localhost:5000/assets/images/avatars/user_1.jpg`
- Event: `http://localhost:5000/assets/images/events/workshop1.jpg`

Nếu ảnh hiển thị → Setup thành công! ✅

### Bước 9: Setup Frontend (Optional)

```bash
cd ../5GrindThe/TheGrind5_EventManagement_FrontEnd
npm install
npm start
```

Frontend sẽ chạy tại: `http://localhost:3000`

---

## 📂 Cấu trúc Thư mục Ảnh

### Ảnh trong Repo (tracked bởi Git)

```
assets/images/
├── avatars/      → Avatar của users mẫu (commit vào git)
└── events/       → Ảnh events mẫu (commit vào git)
```

**Đặc điểm:**
- ✅ Được commit vào Git
- ✅ Clone về là có sẵn
- ✅ Dùng cho seed data
- ✅ Path: `/assets/images/...`

### Ảnh Upload mới (bị ignore)

```
src/wwwroot/uploads/
├── avatars/      → Avatar mới upload (không commit)
└── events/       → Ảnh events mới (không commit)
```

**Đặc điểm:**
- ❌ Bị ignore trong `.gitignore`
- ❌ Không commit vào Git
- ✅ Dùng cho ảnh upload mới từ users
- ✅ Path: `/uploads/...`

---

## 🔧 Troubleshooting

### Vấn đề 1: Ảnh không hiển thị

**Nguyên nhân**: Chưa có ảnh mẫu trong `assets/images/`

**Giải pháp**: 
1. Đọc `assets/images/README.md`
2. Tải ảnh mẫu từ placeholder hoặc stock images
3. Đặt vào đúng thư mục với đúng tên file

### Vấn đề 2: Database connection failed

**Nguyên nhân**: Connection string không đúng

**Giải pháp**:
1. Kiểm tra SQL Server đang chạy
2. Cập nhật connection string trong `appsettings.json`
3. Thử kết nối bằng SSMS trước

### Vấn đề 3: Migration lỗi

**Nguyên nhân**: Database đã tồn tại hoặc conflict

**Giải pháp**:
```bash
# Xóa database cũ
DROP DATABASE EventDB;
GO

# Tạo lại
CREATE DATABASE EventDB;
GO

# Chạy lại migration
dotnet ef database update
```

### Vấn đề 4: Seed data lỗi

**Nguyên nhân**: Foreign key constraint hoặc data đã tồn tại

**Giải pháp**: File `SampleData_Insert.sql` đã có phần CLEAR EXISTING DATA ở đầu. Chạy toàn bộ file.

---

## 📝 Lưu ý quan trọng

### 1. Về Ảnh

- **Ảnh mẫu** (assets/images/): commit vào git
- **Ảnh upload mới**: KHÔNG commit (bị ignore)
- Định dạng hỗ trợ: JPG, JPEG, PNG, GIF, WEBP
- Kích thước tối đa: 5MB/file

### 2. Về Database

- Luôn backup database trước khi seed lại
- Password mặc định: `123456` (đã hash bằng bcrypt)
- Wallet balance có thể thay đổi trong SQL script

### 3. Về Security

- Đừng commit `appsettings.json` (đã có trong .gitignore)
- Đổi JWT secret key trong production
- Đổi password users sau khi seed

---

## 🎯 Kiểm tra Setup thành công

Sau khi setup xong, kiểm tra:

- [ ] Backend chạy tại `http://localhost:5000`
- [ ] Swagger UI hiển thị tại `http://localhost:5000/swagger`
- [ ] Database có 5 users, 6 events
- [ ] Ảnh avatar hiển thị: `http://localhost:5000/assets/images/avatars/user_1.jpg`
- [ ] Ảnh event hiển thị: `http://localhost:5000/assets/images/events/workshop1.jpg`
- [ ] Login thành công với `host1@example.com` / `123456`
- [ ] Frontend kết nối được backend (nếu có)

---

## 📚 Tài liệu liên quan

- [README.md](README.md) - Tổng quan dự án
- [assets/images/README.md](assets/images/README.md) - Hướng dẫn chuẩn bị ảnh mẫu
- [SampleData_Insert.sql](SampleData_Insert.sql) - Script seed data
- [Diagram/](Diagram/) - Database schema và ERD

---

## 🤝 Đóng góp

Nếu gặp vấn đề trong quá trình setup, vui lòng:
1. Kiểm tra Troubleshooting ở trên
2. Tạo Issue trên GitHub
3. Liên hệ team để được hỗ trợ

---

## ✅ Tóm tắt Quick Start

```bash
# 1. Clone repo
git clone <repo-url>
cd TheGrind5_EventManagement

# 2. Kiểm tra ảnh có trong assets/images/

# 3. Tạo database
# Mở SSMS → CREATE DATABASE EventDB

# 4. Migration
cd src
dotnet ef database update

# 5. Seed data
# Mở SSMS → chạy SampleData_Insert.sql

# 6. Config
cp appsettings.Example.json appsettings.json
# Cập nhật connection string

# 7. Run
dotnet run

# 8. Test
# http://localhost:5000/assets/images/avatars/user_1.jpg
```

🎉 **Chúc bạn setup thành công!**

