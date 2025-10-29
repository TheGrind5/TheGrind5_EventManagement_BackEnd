# Khắc phục: File ảnh không tìm thấy

## Vấn đề

Script export tìm thấy ảnh trong database nhưng không tìm thấy file thực tế trên disk.

**Lý do có thể:**
1. File đã bị xóa
2. File ở thư mục khác
3. Database có GUID nhưng file thực tế tên khác

## Giải pháp

### Cách 1: Chấp nhận dùng GUID trong SQL (KHUYẾN NGHỊ)

**Nếu bạn không có file ảnh gốc:**
- SQL vẫn export được với đường dẫn GUID
- Khi chạy SQL, database sẽ có data
- Ảnh sẽ không hiển thị nếu file không tồn tại (nhưng data vẫn đầy đủ)
- Bạn có thể upload ảnh mới sau và cập nhật database

**Ưu điểm:** Không cần làm gì, SQL vẫn dùng được

---

### Cách 2: Upload lại ảnh vào assets/images/events/

**Nếu bạn có ảnh (từ nguồn khác hoặc backup):**

```powershell
# Copy ảnh vào thư mục với đúng tên GUID
# Ví dụ:
Copy-Item "path\to\your\image.jpg" "assets\images\events\b33c34c7-d13b-4531-a7b7-a21add3fd730.jpg"
Copy-Item "path\to\your\image2.png" "assets\images\events\d031d269-2b9e-4981-9087-72332ce748f9.png"
# ... (lặp lại cho tất cả 10 files)
```

Sau đó chạy lại script:
```
.\auto_export_all.bat
```

---

### Cách 3: Tạo ảnh placeholder (tạm thời)

**Nếu muốn có ảnh để test:**

```powershell
# Chạy script này để tạo ảnh placeholder với đúng tên GUID
$guids = @(
    "b33c34c7-d13b-4531-a7b7-a21add3fd730.jpg",
    "d031d269-2b9e-4981-9087-72332ce748f9.png",
    "8ba8951c-e857-4564-a27f-ece4ef9a6f06.png",
    "fd85325e-d50e-40ee-8d45-f9d6f2606ecf.png",
    "d81cf3f5-0052-4338-acbf-eaaab9ddc883.png",
    "7ca0e109-966b-48c1-99eb-c5e92faab8ff.png",
    "f09b13c3-f68a-45f4-96aa-8e448cdd3328.png",
    "5fd99852-3985-4cb2-bb55-455f077e9dab.png",
    "c5d33b8c-efc2-4234-8c9e-2f200ee0008c.png",
    "360872c9-e969-4b9c-b569-f40e166cf9d5.jpg"
)

if (-not (Test-Path "assets\images\events\")) {
    New-Item -ItemType Directory -Path "assets\images\events\" -Force | Out-Null
}

foreach ($guid in $guids) {
    $ext = [System.IO.Path]::GetExtension($guid)
    $url = if ($ext -eq ".jpg" -or $ext -eq ".jpeg") {
        "https://placehold.co/1200x630/667eea/white?text=Event+Image"
    } else {
        "https://placehold.co/1200x630/764ba2/white?text=Event+Image"
    }
    $output = "assets\images\events\$guid"
    Write-Host "Downloading placeholder: $guid"
    Invoke-WebRequest -Uri $url -OutFile $output
}

Write-Host "Done! Placeholder images created."
```

Sau đó chạy lại:
```
.\auto_export_all.bat
```

---

## Kiểm tra files hiện có

```powershell
# Xem files trong wwwroot/uploads/events/
Get-ChildItem "src\wwwroot\uploads\events\" | Select-Object Name

# Xem files trong assets/images/events/
Get-ChildItem "assets\images\events\" -ErrorAction SilentlyContinue | Select-Object Name
```

---

## Kết luận

**Khuyến nghị:**
- Nếu không có ảnh gốc → Chấp nhận dùng GUID trong SQL (Cách 1)
- Nếu cần ảnh để test → Dùng placeholder (Cách 3)
- Nếu có ảnh gốc → Copy vào assets với đúng tên GUID (Cách 2)

**Lưu ý:** SQL vẫn export được và hoạt động bình thường, chỉ là ảnh không hiển thị nếu file không tồn tại.

