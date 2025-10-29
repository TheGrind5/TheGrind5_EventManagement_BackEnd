# Hướng dẫn Commit Ảnh vào Git

## 📸 Chuẩn bị ảnh mẫu

### Bước 1: Tải hoặc chuẩn bị ảnh

Bạn cần chuẩn bị các file ảnh sau:

**Avatars (5 files):**
- `assets/images/avatars/user_1.jpg` - Avatar cho host1@example.com
- `assets/images/avatars/user_2.jpg` - Avatar cho host2@example.com
- `assets/images/avatars/user_3.jpg` - Avatar cho customer1@example.com
- `assets/images/avatars/user_4.jpg` - Avatar cho customer2@example.com
- `assets/images/avatars/user_5.jpg` - Avatar cho testwallet@example.com

**Event Images (12 files):**
- `assets/images/events/workshop1.jpg` và `workshop2.jpg`
- `assets/images/events/ai1.jpg` và `ai2.jpg`
- `assets/images/events/networking1.jpg` và `networking2.jpg`
- `assets/images/events/concert1.jpg` và `concert2.jpg`
- `assets/images/events/art1.jpg` và `art2.jpg`
- `assets/images/events/cooking1.jpg` và `cooking2.jpg`

### Nguồn ảnh miễn phí:

1. **Placeholder Images** (nhanh nhất):
   - Avatar: https://placehold.co/400x400
   - Event: https://placehold.co/1200x630

2. **Stock Photos** (đẹp hơn):
   - https://unsplash.com/
   - https://pexels.com/
   - https://pixabay.com/

### Bước 2: Đặt ảnh vào thư mục

Copy các file ảnh vào đúng thư mục:
```
assets/images/avatars/    ← 5 files avatar
assets/images/events/     ← 12 files event images
```

## 📦 Commit vào Git

### Bước 1: Kiểm tra trạng thái

```bash
git status
```

Bạn sẽ thấy:
```
modified:   .gitignore
modified:   SampleData_Insert.sql
modified:   src/Services/FileManagementService.cs
modified:   src/Controllers/AuthController.cs
modified:   src/Program.cs
modified:   src/Scripts/CleanupUnusedImages.cs
new file:   assets/images/README.md
new file:   assets/images/avatars/user_1.jpg
new file:   assets/images/avatars/user_2.jpg
...
new file:   assets/images/events/workshop1.jpg
...
new file:   SETUP_GUIDE.md
new file:   HOW_TO_COMMIT_IMAGES.md
```

### Bước 2: Add tất cả thay đổi

```bash
git add .
```

Hoặc add từng phần:
```bash
git add .gitignore
git add assets/
git add src/
git add SampleData_Insert.sql
git add SETUP_GUIDE.md
git add HOW_TO_COMMIT_IMAGES.md
```

### Bước 3: Commit

```bash
git commit -m "feat: Chuyển ảnh sang assets/images/ để commit vào git

- Tạo thư mục assets/images/ cho ảnh mẫu
- Cập nhật .gitignore để track assets/images/
- Cập nhật FileManagementService, AuthController, Program.cs
- Cập nhật SampleData_Insert.sql với đường dẫn mới
- Thêm SETUP_GUIDE.md và README cho assets/images/
- Ảnh giờ được commit vào git, clone về là có sẵn"
```

### Bước 4: Push lên GitHub

```bash
git push origin master
```

hoặc

```bash
git push origin main
```

## ✅ Kiểm tra

### Trên GitHub:
1. Vào repository trên GitHub
2. Kiểm tra thư mục `assets/images/`
3. Đảm bảo có đủ 17 files (5 avatars + 12 events)

### Clone về máy khác:
```bash
git clone <repo-url>
cd TheGrind5_EventManagement
ls -la assets/images/avatars/
ls -la assets/images/events/
```

Nếu thấy đầy đủ file → Thành công! ✅

## 🔍 Troubleshooting

### Vấn đề: Ảnh không được add vào git

**Nguyên nhân**: Có thể bị ignore

**Giải pháp**:
```bash
# Force add
git add -f assets/images/
```

### Vấn đề: File quá lớn

**Nguyên nhân**: Ảnh > 5MB

**Giải pháo**: Resize ảnh trước khi commit:
- Avatar: 400x400px
- Event: 1200x630px
- Chất lượng JPG: 80-85%

### Vấn đề: Push bị reject

**Nguyên nhân**: Repository có thay đổi mới

**Giải pháp**:
```bash
git pull --rebase
git push
```

## 📝 Lưu ý

1. **Chỉ commit ảnh mẫu**, không commit ảnh cá nhân
2. **Kiểm tra kích thước** file trước khi commit (< 5MB)
3. **Đặt đúng tên file** như trong hướng dẫn
4. **Định dạng**: JPG, PNG, WEBP (ưu tiên JPG)

## 🎯 Sau khi commit

Khi người khác clone dự án:
1. Clone repo → ảnh đã có sẵn ✅
2. Chạy `SampleData_Insert.sql` → data được seed ✅
3. Chạy backend → ảnh hiển thị ngay ✅

Không cần copy ảnh thủ công!

---

**Đọc thêm**: [SETUP_GUIDE.md](SETUP_GUIDE.md) để biết cách setup toàn bộ dự án.

