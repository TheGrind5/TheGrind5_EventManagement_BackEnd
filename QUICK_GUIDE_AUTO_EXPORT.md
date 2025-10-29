# 🚀 Hướng dẫn Tự động hóa Export Sample Data

## ✨ Workflow CỰC KỲ ĐƠN GIẢN

### Trước đây (5 bước thủ công):
```powershell
# ❌ Phức tạp, mất thời gian
1. Sửa mapping trong rename_images_for_export.ps1
2. .\rename_images_for_export.ps1
3. .\export_sample_data.ps1
4. Sửa mapping trong update_image_paths_in_sql.ps1
5. .\update_image_paths_in_sql.ps1
```

### Bây giờ (1 lệnh duy nhất):
```powershell
# ✅ Tự động 100%
.\auto_export_all.ps1
```

---

## 📖 Hướng dẫn chi tiết

### Bước 1: Tạo Events qua UI

```bash
# 1. Chạy app
cd src
dotnet run

# 2. Đăng nhập
# URL: http://localhost:5000
# Email: host1@example.com
# Password: 123456

# 3. Tạo events với đầy đủ thông tin:
#    ✅ Upload ảnh (2-3 ảnh/event)
#    ✅ Địa điểm, thời gian
#    ✅ Ticket types
#    ✅ Visual Stage
#    ✅ Organizer info
#    ✅ Terms & conditions

# Tạo bao nhiêu events tùy thích (5, 10, 20, 50...)
```

### Bước 2: Chạy script tự động

```powershell
# Chỉ cần 1 lệnh!
.\auto_export_all.ps1
```

**Script sẽ TỰ ĐỘNG:**
1. ✅ Kết nối database
2. ✅ Phân tích events và ảnh
3. ✅ Tự động tạo tên friendly cho ảnh (dựa vào EventId + Title)
4. ✅ Đổi tên tất cả ảnh trong thư mục
5. ✅ Export database ra SQL
6. ✅ Tự động update đường dẫn ảnh trong SQL
7. ✅ Tạo file `ExtendedSampleData_Insert.sql`

**Output:**
```
╔══════════════════════════════════════════════════════════╗
║                    SUCCESS!                              ║
╚══════════════════════════════════════════════════════════╝

  📊 Statistics:
     • Events exported: 20
     • Event images renamed: 45
     • Avatar images renamed: 5
     • SQL file: ExtendedSampleData_Insert.sql

  📁 Files ready for commit:
     • ExtendedSampleData_Insert.sql
     • assets/images/events/ (45 files)
     • assets/images/avatars/ (5 files)

  🚀 Next steps:
     1. Review ExtendedSampleData_Insert.sql
     2. git add ExtendedSampleData_Insert.sql assets/images/
     3. git commit -m 'Add 20 extended sample events'
     4. git push
```

### Bước 3: Review và Commit

```bash
# 1. Kiểm tra file SQL (optional)
notepad ExtendedSampleData_Insert.sql

# 2. Kiểm tra ảnh đã đổi tên
ls assets\images\events\

# Output:
# event_1_workshop_python_1.jpg
# event_1_workshop_python_2.jpg
# event_2_concert_rock_1.jpg
# event_2_concert_rock_2.jpg
# ...

# 3. Add và commit
git add ExtendedSampleData_Insert.sql
git add assets/images/
git commit -m "feat: Add extended sample data with 20 events

- 20 events with full details (images, tickets, stages)
- All images auto-renamed with friendly names
- Ready for team to seed and use"

# 4. Push
git push
```

---

## 👥 Team Member Clone và Sử dụng

### Khi người khác clone về:

```bash
# 1. Clone repo
git clone <repo-url>
cd TheGrind5_EventManagement

# 2. Kiểm tra ảnh
ls assets/images/events/
# ✅ Có 45 files với tên rõ ràng

# 3. Setup database
# Mở SSMS:
CREATE DATABASE EventDB;

# 4. Migration
cd src
dotnet ef database update

# 5. Seed data
# Mở SSMS → Chạy ExtendedSampleData_Insert.sql
# ✅ Database có 20 events với đầy đủ thông tin

# 6. Chạy app
dotnet run

# 7. Truy cập
# http://localhost:5000
# Login: host1@example.com / 123456
# ✅ Thấy 20 events với ảnh, tickets, stages...
```

---

## 🎯 Tính năng của Script Tự động

### 1. Tự động đặt tên ảnh thông minh

**Quy tắc đặt tên:**
- `event_{EventId}_{slug}_{index}.jpg`
- Slug = title không dấu, chữ thường, thay space bằng _

**Ví dụ:**
```
Event 1: "Workshop Lập Trình Python"
  → event_1_workshop_lap_trinh_python_1.jpg
  → event_1_workshop_lap_trinh_python_2.jpg

Event 2: "Concert Nhạc Rock & Roll"
  → event_2_concert_nhac_rock_roll_1.jpg
  → event_2_concert_nhac_rock_roll_2.jpg

Event 3: "Hội Thảo AI & Machine Learning"
  → event_3_hoi_thao_ai_machine_learning_1.jpg
```

### 2. Xử lý tiếng Việt

Script tự động chuyển đổi:
- `à á ả ã ạ ă ắ ằ ẳ ẵ ặ â ấ ầ ẩ ẫ ậ` → `a`
- `è é ẻ ẽ ẹ ê ế ề ể ễ ệ` → `e`
- `ì í ỉ ĩ ị` → `i`
- `ò ó ỏ õ ọ ô ố ồ ổ ỗ ộ ơ ớ ờ ở ỡ ợ` → `o`
- `ù ú ủ ũ ụ ư ứ ừ ử ữ ự` → `u`
- `ỳ ý ỷ ỹ ỵ` → `y`
- `đ` → `d`

### 3. Xử lý Avatar

Avatar cũng tự động đổi tên:
- `user_{UserId}.jpg`

**Ví dụ:**
```
User 1: abc-123-guid.jpg → user_1.jpg
User 2: def-456-guid.jpg → user_2.jpg
```

### 4. Update paths trong Database Export

Script tự động thay thế trong SQL:
```sql
-- Trước:
N'{"images": ["/assets/images/events/5c958cc1-2e0c-48e1-aa69.jpg"]}'

-- Sau:
N'{"images": ["/assets/images/events/event_1_workshop_python_1.jpg"]}'
```

---

## ⚙️ Tùy chỉnh (Optional)

### Thay đổi connection string:

```powershell
.\auto_export_all.ps1 -ServerName "localhost" -DatabaseName "MyDB"
```

### Thay đổi output file:

```powershell
.\auto_export_all.ps1 -OutputFile "MySampleData.sql"
```

---

## 🔧 Troubleshooting

### Vấn đề 1: "Cannot connect to database"

**Giải pháp:**
```powershell
# Kiểm tra SQL Server đang chạy
# Thử kết nối bằng SSMS trước

# Hoặc thay đổi server name:
.\auto_export_all.ps1 -ServerName "localhost\SQLEXPRESS"
```

### Vấn đề 2: "File already exists"

**Giải pháp:**
Script sẽ skip files đã tồn tại. Nếu muốn rename lại:
```powershell
# Xóa ảnh cũ
Remove-Item assets\images\events\event_*.jpg

# Chạy lại
.\auto_export_all.ps1
```

### Vấn đề 3: "EventDetails JSON parse error"

**Giải pháp:**
Script sẽ báo warning nhưng vẫn tiếp tục. Event đó sẽ không có ảnh trong SQL.

---

## 📊 So sánh

| | Thủ công | Tự động |
|---|---|---|
| **Số bước** | 5 bước | 1 bước |
| **Thời gian** | 15-20 phút | 30 giây |
| **Sửa mapping** | Phải sửa 2 lần | Tự động |
| **Tên ảnh** | Tự đặt | Tự động sinh |
| **Dễ nhầm** | Cao | Không |
| **Xử lý tiếng Việt** | Thủ công | Tự động |

---

## ✅ Checklist

Sau khi chạy `auto_export_all.ps1`:

- [ ] Script chạy thành công (SUCCESS!)
- [ ] File `ExtendedSampleData_Insert.sql` được tạo
- [ ] Ảnh trong `assets/images/events/` có tên `event_X_...jpg`
- [ ] Mở SQL file và kiểm tra paths đã đúng
- [ ] `git status` thấy các file mới
- [ ] Commit và push

---

## 🎉 Tổng kết

**Từ giờ, workflow của bạn:**

1. ✅ Tạo events qua UI (thủ công - cần UI để nhập đầy đủ)
2. ✅ Chạy `.\auto_export_all.ps1` (tự động 100%)
3. ✅ `git add . && git commit && git push` (thủ công - 10 giây)

**Tất cả đã được tự động hóa!** 🚀

Không cần:
- ❌ Sửa mapping thủ công
- ❌ Đặt tên ảnh thủ công
- ❌ Chạy nhiều script
- ❌ Update paths thủ công

**Chỉ cần 1 lệnh là xong!** ⚡

