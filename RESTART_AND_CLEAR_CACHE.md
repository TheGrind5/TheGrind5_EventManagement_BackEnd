# RESTART BACKEND VÀ CLEAR CACHE - QUAN TRỌNG!

## ✅ Đã kiểm tra:
- ✅ Database: Tất cả 13 events có images đúng paths
- ✅ Files: 76 images tồn tại trong assets/images/events/
- ✅ API: Trả về đúng eventImage cho tất cả events
- ✅ Static Files: Tất cả images accessible (200 OK)

## 🔧 VẤN ĐỀ: Browser Cache hoặc Backend chưa restart

### BƯỚC 1: RESTART BACKEND (BẮT BUỘC!)

```bash
# 1. Dừng backend hiện tại
# Trong terminal đang chạy backend, nhấn: Ctrl+C

# 2. Chạy lại backend
cd src
dotnet run

# 3. Đợi backend khởi động xong (sẽ thấy "Now listening on: http://localhost:5000")
```

### BƯỚC 2: CLEAR BROWSER CACHE (BẮT BUỘC!)

#### Cách 1: Hard Refresh (NHANH)
- **Chrome/Edge**: Nhấn `Ctrl + Shift + R` hoặc `Ctrl + F5`
- **Firefox**: Nhấn `Ctrl + Shift + R`

#### Cách 2: Xóa Cache hoàn toàn (CHẮC CHẮN)
1. Nhấn `Ctrl + Shift + Delete`
2. Chọn:
   - ✅ Cached images and files
   - ✅ Cookies and other site data (optional)
3. Time range: "All time"
4. Click "Clear data"

#### Cách 3: Incognito/Private Window (TEST)
- **Chrome**: `Ctrl + Shift + N`
- **Firefox**: `Ctrl + Shift + P`
- Mở `http://localhost:3001` trong cửa sổ mới

### BƯỚC 3: KIỂM TRA CONSOLE

1. Mở Developer Tools: `F12`
2. Tab **Network**
3. Filter: `img` hoặc `png` hoặc `jpg`
4. Refresh trang (F5)
5. Xem các requests đến `/assets/images/events/`:
   - ✅ Status 200 = OK
   - ❌ Status 404 = File không tồn tại hoặc path sai
   - ❌ Status 304 = Browser đang dùng cache (clear cache lại)

### BƯỚC 4: TEST TRỰC TIẾP URL

Mở từng URL này trong browser:
```
http://localhost:5000/assets/images/events/b33c34c7-d13b-4531-a7b7-a21add3fd730.jpg
http://localhost:5000/assets/images/events/f09b13c3-f68a-45f4-96aa-8e448cdd3328.png
http://localhost:5000/assets/images/events/8ba8951c-e857-4564-a27f-ece4ef9a6f06.png
```

- ✅ Nếu thấy ảnh → Static files OK
- ❌ Nếu 404 → Backend chưa restart hoặc config sai

---

## 🎯 SAU KHI LÀM XONG:

Tất cả ảnh sẽ hiển thị!

Nếu vẫn không thấy:
1. Kiểm tra backend console có log `[Static Files] Configured...` không
2. Kiểm tra Network tab xem có requests đến images không
3. Kiểm tra Console tab xem có lỗi JavaScript không

