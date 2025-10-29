# 🚀 WORKFLOW: Tạo 20+ Events với đầy đủ thông tin để share trên Git

## 📋 Tổng quan

Workflow này giúp bạn:
1. Tạo nhiều events qua UI với đầy đủ thông tin (ảnh, địa điểm, thời gian, vé, visual stage...)
2. Export ra SQL script
3. Commit vào Git
4. Mọi người clone về có thể dùng ngay

---

## 🎯 BƯỚC 1: Tạo Events qua UI

### 1.1. Chạy ứng dụng

```bash
cd src
dotnet run
```

### 1.2. Đăng nhập và tạo events

```
URL: http://localhost:5000
Login: host1@example.com / 123456
```

### 1.3. Tạo từng event với đầy đủ thông tin

Cho mỗi event (tạo 20 events):

**Thông tin cơ bản:**
- ✅ Tiêu đề: "Workshop Python cho người mới bắt đầu"
- ✅ Mô tả chi tiết
- ✅ Category: Technology, Music, Art, Business, Lifestyle...
- ✅ Event Type: Workshop, Concert, Conference, Exhibition...
- ✅ Event Mode: Online / Offline

**Địa điểm:**
- ✅ Location: "Hà Nội, Việt Nam" (hoặc cụ thể hơn)
- ✅ Venue: "Trung tâm Hội nghị Quốc gia"
- ✅ Địa chỉ chi tiết (trong EventDetails JSON)

**Thời gian:**
- ✅ Start Time: 2025-11-15 19:00
- ✅ End Time: 2025-11-15 22:00

**Ảnh:**
- ✅ Upload 2-3 ảnh cho mỗi event
- ✅ Ảnh sẽ được lưu vào: `assets/images/events/` với tên GUID

**Ticket Types:**
- ✅ Tạo nhiều loại vé: VIP, Thường, Sinh viên, Cặp đôi...
- ✅ Giá: 100,000 - 500,000 VND
- ✅ Số lượng: 20-200 vé
- ✅ Min/Max order: 1-10

**Visual Stage (nếu có):**
- ✅ Vẽ layout sân khấu 2D
- ✅ Phân vùng chỗ ngồi
- ✅ Map với ticket types

**Organizer Info:**
- ✅ Logo
- ✅ Tên tổ chức
- ✅ Thông tin

**Terms & Conditions:**
- ✅ Điều khoản chung
- ✅ Điều khoản trẻ em
- ✅ VAT

---

## 🎯 BƯỚC 2: Liệt kê và Đổi tên ảnh

### 2.1. Xem danh sách ảnh đã upload

```powershell
ls assets\images\events\ | Select-Object Name | Sort-Object Name

# Output:
# 5c958cc1-2e0c-48e1-aa69-b572531e8922.jpg
# 8377d73f-6d44-49e3-9812-c7b3d1291c8e.jpg
# ba26a08a-0827-48a0-bdef-cec4641e6415.jpg
# ...
```

### 2.2. Tạo mapping GUID → Tên có nghĩa

Mở file `rename_images_for_export.ps1` và cập nhật mapping:

```powershell
$imageMapping = @{
    # Event 1: Workshop Python
    "5c958cc1-2e0c-48e1-aa69-b572531e8922.jpg" = "python_workshop_1.jpg"
    "8377d73f-6d44-49e3-9812-c7b3d1291c8e.jpg" = "python_workshop_2.jpg"
    
    # Event 2: Rock Concert
    "ba26a08a-0827-48a0-bdef-cec4641e6415.jpg" = "rock_concert_1.jpg"
    "c5c1fd41-06ac-451f-9054-52afd76350b7.jpg" = "rock_concert_2.jpg"
    
    # Event 3: AI Conference
    "cd7ef4aa-8977-4458-8e19-1b6c88d7539f.jpg" = "ai_conference_1.jpg"
    "d78203b3-a549-4975-af3f-06efa8a05758.jpg" = "ai_conference_2.jpg"
    
    # ... Thêm tất cả ảnh của 20 events
}
```

**💡 Tip**: Để biết ảnh nào thuộc event nào:
- Xem trong database: `SELECT EventId, Title, EventDetails FROM Event`
- Hoặc xem trong UI admin panel
- Sắp xếp theo thời gian tạo file

### 2.3. Chạy script đổi tên

```powershell
.\rename_images_for_export.ps1
```

### 2.4. Kiểm tra

```powershell
ls assets\images\events\

# Output (sau khi đổi tên):
# python_workshop_1.jpg
# python_workshop_2.jpg
# rock_concert_1.jpg
# rock_concert_2.jpg
# ai_conference_1.jpg
# ...
```

---

## 🎯 BƯỚC 3: Export Database

### 3.1. Chạy script export

```powershell
.\export_sample_data.ps1
```

Script sẽ:
- ✅ Kết nối database
- ✅ Export Users, Events, TicketTypes, Vouchers
- ✅ Tạo file `ExtendedSampleData_Insert.sql`

### 3.2. Kiểm tra file SQL

```powershell
notepad ExtendedSampleData_Insert.sql
```

File sẽ có:
```sql
-- INSERT USERS
INSERT INTO [User] (UserId, Username, FullName, ...)
VALUES (1, 'host1', N'Nguyễn Văn Host', ...);

-- INSERT EVENTS
INSERT INTO Event (EventId, Title, EventDetails, ...)
VALUES (1, N'Workshop Python', N'{"images": ["/assets/images/events/5c958cc1-...jpg"]}', ...);
                                                    ↑ 
                                            VẪN CÒN GUID - CẦN SỬA
```

---

## 🎯 BƯỚC 4: Update đường dẫn ảnh trong SQL

### 4.1. Cập nhật mapping trong script

Mở `update_image_paths_in_sql.ps1` và cập nhật (GIỐNG như BƯỚC 2.2):

```powershell
$imageMapping = @{
    "5c958cc1-2e0c-48e1-aa69-b572531e8922.jpg" = "python_workshop_1.jpg"
    "8377d73f-6d44-49e3-9812-c7b3d1291c8e.jpg" = "python_workshop_2.jpg"
    # ... (giống BƯỚC 2.2)
}
```

### 4.2. Chạy script update

```powershell
.\update_image_paths_in_sql.ps1
```

### 4.3. Kiểm tra lại SQL file

```powershell
notepad ExtendedSampleData_Insert.sql
```

Bây giờ sẽ thấy:
```sql
INSERT INTO Event (EventId, Title, EventDetails, ...)
VALUES (1, N'Workshop Python', N'{"images": ["/assets/images/events/python_workshop_1.jpg", "/assets/images/events/python_workshop_2.jpg"]}', ...);
                                                    ↑ 
                                            ĐÃ ĐỔI THÀNH TÊN FRIENDLY ✅
```

---

## 🎯 BƯỚC 5: Commit vào Git

### 5.1. Kiểm tra thay đổi

```bash
git status
```

Sẽ thấy:
```
new file:   ExtendedSampleData_Insert.sql
new file:   assets/images/events/python_workshop_1.jpg
new file:   assets/images/events/python_workshop_2.jpg
new file:   assets/images/events/rock_concert_1.jpg
...
modified:   .gitignore (nếu có)
```

### 5.2. Add tất cả

```bash
git add ExtendedSampleData_Insert.sql
git add assets/images/events/
git add rename_images_for_export.ps1
git add export_sample_data.ps1
git add update_image_paths_in_sql.ps1
git add COMPLETE_WORKFLOW.md
```

### 5.3. Commit

```bash
git commit -m "feat: Add 20 extended sample events with full details

- Added 20 events with images, tickets, visual stages
- Included detailed venue, time, organizer info
- All images renamed with friendly names
- SQL script ready for team to seed data"
```

### 5.4. Push

```bash
git push origin master
```

---

## 🎯 BƯỚC 6: Team Clone và Sử dụng

### 6.1. Team member clone repo

```bash
git clone <repo-url>
cd TheGrind5_EventManagement
```

### 6.2. Kiểm tra ảnh

```bash
ls assets/images/events/

# Output:
# python_workshop_1.jpg ✅
# python_workshop_2.jpg ✅
# rock_concert_1.jpg ✅
# ... (tất cả ảnh đã có)
```

### 6.3. Setup database

```bash
# 1. Tạo database
# Mở SSMS → CREATE DATABASE EventDB

# 2. Migration
cd src
dotnet ef database update

# 3. Seed extended data
# Mở SSMS → Chạy ExtendedSampleData_Insert.sql
```

### 6.4. Chạy app

```bash
dotnet run
```

### 6.5. Kiểm tra

```
http://localhost:5000

→ Login: host1@example.com / 123456
→ Vào My Events
→ ✅ Thấy 20 events với đầy đủ:
  - Ảnh hiển thị
  - Thông tin chi tiết
  - Ticket types
  - Visual stage diagram
  - Organizer info
  - Terms & conditions
```

---

## ⚡ QUICK REFERENCE

### Workflow tóm tắt:

```bash
# 1. Tạo events qua UI
dotnet run
# → Tạo 20 events với đầy đủ thông tin

# 2. Đổi tên ảnh
# → Sửa rename_images_for_export.ps1
.\rename_images_for_export.ps1

# 3. Export database
.\export_sample_data.ps1

# 4. Update image paths
# → Sửa update_image_paths_in_sql.ps1 (dùng mapping giống bước 2)
.\update_image_paths_in_sql.ps1

# 5. Commit
git add .
git commit -m "Add 20 extended sample events"
git push
```

### Team member workflow:

```bash
# 1. Clone
git clone <repo>

# 2. Setup DB
CREATE DATABASE EventDB
dotnet ef database update

# 3. Seed
# Chạy ExtendedSampleData_Insert.sql trong SSMS

# 4. Run
dotnet run

# ✅ DONE!
```

---

## 🔧 Troubleshooting

### Vấn đề 1: Ảnh không map đúng

**Triệu chứng**: SQL có `python_workshop_1.jpg` nhưng file thật tên khác

**Giải pháp**:
1. Kiểm tra lại mapping trong script
2. Đảm bảo GUID đúng
3. Chạy lại `rename_images_for_export.ps1`
4. Chạy lại `update_image_paths_in_sql.ps1`

### Vấn đề 2: Export thiếu data

**Triệu chứng**: SQL không có đủ events

**Giải pháp**:
- Kiểm tra connection string trong `export_sample_data.ps1`
- Đảm bảo database có data
- Xem log lỗi khi chạy script

### Vấn đề 3: Visual Stage không export

**Triệu chứng**: VenueLayout = NULL

**Giải pháp**:
- Kiểm tra column `VenueLayout` trong database
- Đảm bảo đã vẽ stage trong UI
- Script đã có export VenueLayout ✅

---

## 📝 Lưu ý quan trọng

### ✅ NÊN:
- Đặt tên ảnh có nghĩa, dễ quản lý
- Tạo nhiều loại ticket types đa dạng
- Thêm thông tin chi tiết cho events
- Test lại SQL script trước khi push

### ❌ KHÔNG NÊN:
- Commit ảnh quá lớn (> 5MB)
- Dùng tên ảnh có ký tự đặc biệt
- Quên update mapping trong script
- Push mà chưa test

---

## 🎉 Kết luận

Sau khi hoàn thành workflow này:
- ✅ Bạn có 20+ events với đầy đủ thông tin
- ✅ Ảnh có tên rõ ràng, dễ quản lý
- ✅ SQL script ready để share
- ✅ Team clone về chạy ngay
- ✅ Không cần copy ảnh thủ công

**→ Mọi người đều có data giống nhau để phát triển!** 🚀

