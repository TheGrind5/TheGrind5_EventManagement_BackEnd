# 📚 THEGRIND5 EVENT MANAGEMENT - HƯỚNG DẪN HOÀN CHỈNH

**Tài liệu tổng hợp:** Tất cả hướng dẫn, tính năng, và cải tiến dự án

---

## 📖 MỤC LỤC

1. [Tổng quan Dự án](#tổng-quan-dự-án)
2. [Quản lý Event cho Host](#quản-lý-event-cho-host)
3. [Ràng Buộc Chỉnh Sửa và Xóa Event](#ràng-buộc-chỉnh-sửa-và-xóa-event)
4. [Quản lý Ảnh](#quản-lý-ảnh)
5. [Đề xuất Cải tiến](#đề-xuất-cải-tiến)
6. [Quick Improvement Checklist](#quick-improvement-checklist)
7. [Các Bước Tiếp theo](#các-bước-tiếp-theo)

---

# 1. TỔNG QUAN DỰ ÁN

## 🎯 Giới thiệu

Dự án **TheGrind5 Event Management System** là hệ thống quản lý và bán vé sự kiện hoàn chỉnh với các tính năng:

### ✅ ĐÃ HOÀN THÀNH

#### Backend (ASP.NET Core 8.0)
- ✅ **Authentication & Authorization**: JWT token, đăng ký/đăng nhập, profile management
- ✅ **Event Management**: Tạo, xem, sửa, xóa sự kiện với các ràng buộc bảo mật
- ✅ **Ticket Management**: Bán vé, check-in, hoàn tiền
- ✅ **Order System**: Tạo đơn hàng, thanh toán, tracking
- ✅ **Wallet System**: Nạp tiền, rút tiền, thanh toán qua ví
- ✅ **Voucher System**: Áp dụng mã giảm giá
- ✅ **Wishlist**: Lưu và quản lý vé yêu thích
- ✅ **Virtual Stage 2D**: Thiết kế layout sân khấu ảo
- ✅ **File Upload**: Upload ảnh event, avatar
- ✅ **Pagination**: Đã implement cho EventController, OrderController, TicketController
- ✅ **Global Exception Handler**: Đã có và được đăng ký trong Program.cs
- ✅ **Memory Cache**: Đã đăng ký và có sử dụng trong EventService

#### Frontend (React)
- ✅ **Authentication**: Login, Register, Profile page
- ✅ **Event Browsing**: Home page, Event details, Search & Filter
- ✅ **Create Event**: Multi-step form với Virtual Stage designer
- ✅ **Buy Tickets**: Checkout flow, Payment, Order confirmation
- ✅ **My Tickets**: View, check-in, refund tickets
- ✅ **My Events**: Manage host events with edit constraints
- ✅ **Wallet**: View balance, deposit, withdraw, transaction history
- ✅ **Wishlist**: Add/remove tickets from wishlist
- ✅ **Theme**: Dark/Light mode toggle
- ✅ **Responsive**: Mobile-friendly UI

#### Testing
- ✅ **85+ Test Cases** covering core features
- ✅ **70.8% Code Coverage** (Target: 80%)
- ✅ Comprehensive test suite cho OrderController
- ✅ Tests chia theo từng member trong team

### 📋 DANH SÁCH API ENDPOINTS

#### AuthController
- `POST /api/Auth/register` - Đăng ký
- `POST /api/Auth/login` - Đăng nhập
- `GET /api/Auth/me` - Lấy thông tin user
- `PUT /api/Auth/profile` - Cập nhật profile
- `POST /api/Auth/upload-avatar` - Upload avatar

#### EventController
- `GET /api/Event` - Lấy tất cả events
- `GET /api/Event/{id}` - Lấy event theo ID
- `POST /api/Event` - Tạo event (Authorize)
- `PUT /api/Event/{id}` - Cập nhật event (Authorize)
- `DELETE /api/Event/{id}` - Xóa event (Authorize)
- `GET /api/Event/my-events` - Lấy events của host (Authorize)
- `GET /api/Event/{id}/edit-status` - Kiểm tra có thể edit (Authorize)

#### OrderController
- `POST /api/Order` - Tạo đơn hàng (Authorize)
- `GET /api/Order/my-orders` - Lấy orders của user (Authorize)
- `GET /api/Order/{id}` - Lấy order chi tiết (Authorize)

#### TicketController
- `GET /api/Ticket/my-tickets` - Lấy tickets của user (Authorize)
- `POST /api/Ticket/{id}/check-in` - Check-in vé (Authorize)
- `POST /api/Ticket/{id}/refund` - Hoàn tiền vé (Authorize)

#### WalletController
- `GET /api/Wallet/balance` - Lấy số dư (Authorize)
- `POST /api/Wallet/deposit` - Nạp tiền (Authorize)
- `POST /api/Wallet/withdraw` - Rút tiền (Authorize)
- `GET /api/Wallet/transactions` - Lấy lịch sử giao dịch (Authorize)

#### WishlistController
- `GET /api/Wishlist` - Lấy wishlist (Authorize)
- `POST /api/Wishlist` - Thêm vào wishlist (Authorize)
- `DELETE /api/Wishlist/{id}` - Xóa khỏi wishlist (Authorize)

#### VoucherController
- `GET /api/Voucher/{code}` - Validate voucher code
- `POST /api/Voucher` - Tạo voucher (Authorize - Admin only)

### 🚧 TODO ITEMS CẦN HOÀN THIỆN

#### Priority 1: Core Features
1. **Implement Image Display Locations Modal**
   - File: `TheGrind5_EventManagement_FrontEnd/src/components/event-creation/EventInfoStep.jsx`
   - Action: Tạo modal để hiển thị ảnh đã upload

2. **Implement Inventory Reservation/Release Logic**
   - File: `TheGrind5_EventManagement_FrontEnd/src/services/inventoryService.js`
   - Action: Implement inventory management khi mua vé

3. **Additional Payment Methods**
   - File: `src/Controllers/OrderController.cs` (line 278)
   - Action: Implement Credit Card và Bank Transfer

#### Priority 2: Test Coverage
4. **Improve Test Coverage từ 70.8% lên 80%**
   - Target: OrderController buy ticket flow
   - Action: Add edge cases, error scenarios

#### Priority 3: UI/UX Improvements
5. **Edit Event Page**
   - Action: Implement proper edit event functionality với validation

6. **Event Statistics Dashboard**
   - Action: Revenue, tickets sold, attendance stats

---

# 2. QUẢN LÝ EVENT CHO HOST

## Tổng Quan

Tính năng này cho phép các host (user đã tạo ít nhất 1 event) quản lý các sự kiện của mình, bao gồm xem danh sách, chỉnh sửa và xóa. Tính năng được bảo vệ bởi ràng buộc không cho edit event đã có vé được bán.

## Vấn Đề Bảo Mật Được Giải Quyết

**Vấn đề**: Host có thể thay đổi thông tin sự kiện vào phút chót nhằm trục lợi (ví dụ: thay đổi giá vé, giảm chất lượng sự kiện sau khi đã bán vé).

**Giải pháp**: Chỉ cho phép edit event nếu event đó CHƯA CÓ vé nào được bán.

## Backend Changes

### EventController.cs - Thêm Endpoints

#### GET /api/Event/my-events
- **Mô tả**: Lấy danh sách events của user hiện tại (host)
- **Authorization**: Required
- **Response**: Danh sách events của host

```csharp
[HttpGet("my-events")]
[Authorize]
public async Task<IActionResult> GetMyEvents()
```

#### GET /api/Event/{id}/edit-status
- **Mô tả**: Kiểm tra xem có thể edit event hay không
- **Authorization**: Required
- **Response**:
```json
{
  "eventId": 1,
  "canEdit": true/false,
  "hasTicketsSold": true/false,
  "message": "Có thể chỉnh sửa sự kiện" hoặc "Không thể chỉnh sửa sự kiện đã có vé được bán"
}
```

### EventService.cs - Thêm Logic Kiểm Tra

#### Method: CheckHasTicketsSoldAsync(int eventId)
- **Mô tả**: Kiểm tra xem event có vé nào đã được bán chưa
- **Logic**: 
  1. Lấy danh sách TicketTypes của event
  2. Kiểm tra trong bảng OrderItems xem có ticket types nào có Status != "Cancelled" không
  3. Trả về true nếu có vé đã bán

## Frontend Changes

### MyEventsPage.jsx - Trang Quản Lý Events

#### Tính năng:
- ✅ Hiển thị danh sách events của host dạng grid cards
- ✅ Thông tin hiển thị: Ảnh, tiêu đề, status badge, ngày, địa điểm, category
- ✅ Nút "Xem" - chuyển đến trang chi tiết event
- ✅ Nút "Chỉnh sửa" - kiểm tra edit-status trước khi cho edit
- ✅ Nút "Xóa" - xóa event với confirmation
- ✅ Empty state khi chưa có event
- ✅ Status badges với màu sắc phù hợp

#### Ràng buộc Edit:
```javascript
const handleEditEvent = async (eventId) => {
  // Kiểm tra edit status trước
  const statusResponse = await eventsAPI.getEditStatus(eventId);
  
  if (statusResponse.data.canEdit) {
    // Cho phép edit
  } else {
    // Block và thông báo
    alert(statusResponse.data.message);
  }
};
```

### Header.jsx - Thêm Nút "My Events"

- Chỉ hiển thị nút "My Events" khi user đã có ít nhất 1 event
- Gọi API `eventsAPI.getMyEvents()` khi user đăng nhập
- Hiển thị trong cả desktop navigation và mobile dropdown menu

## Luồng Hoạt Động

1. **User trở thành Host**: User tạo event → Hệ thống tự động coi user là host
2. **Hiển thị nút "My Events"**: User đăng nhập → Header kiểm tra events → Hiển thị nút nếu có events
3. **Xem danh sách events**: Click "My Events" → MyEventsPage → Gọi API GET /api/Event/my-events
4. **Chỉnh sửa event**: Click "Chỉnh sửa" → Kiểm tra edit-status → Cho phép hoặc block
5. **Xóa event**: Click "Xóa" → Confirmation dialog → Gọi API DELETE

## Bảo Mật

### Ràng buộc Edit
- ✅ Event chưa có vé được bán → Cho phép edit
- ❌ Event đã có vé được bán → Không cho phép edit

### Ownership Check
- ✅ Chỉ host mới có thể xem/edit events của mình
- ✅ Backend kiểm tra `eventData.HostId != userId.Value`

---

# 3. RÀNG BUỘC CHỈNH SỬA VÀ XÓA EVENT

## Tổng Quan

Đã thêm các ràng buộc bảo mật cho việc chỉnh sửa và xóa event để bảo vệ khách hàng.

## 1. Ràng Buộc Xóa Event

### Backend: DELETE /api/Event/{id}

**Quy tắc:**
- ✅ Chỉ host mới có thể xóa event của mình
- ✅ Không cho xóa nếu event đã có vé được mua thành công (Order.Status = "Paid")
- ✅ Nếu có vé Paid → Trả về thông báo yêu cầu liên hệ hỗ trợ

**Code:**
```csharp
[HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> DeleteEvent(int id)
{
    // Kiểm tra quyền
    if (eventData.HostId != userId.Value)
        return Forbid("Bạn không có quyền xóa sự kiện này");

    // Kiểm tra vé Paid
    var hasTicketsSold = await _eventService.CheckHasPaidTicketsAsync(id);
    if (hasTicketsSold)
    {
        return BadRequest(new { 
            message = "Không thể xóa sự kiện đã có vé được mua thành công. Hãy liên hệ hỗ trợ nếu muốn hủy sự kiện." 
        });
    }
    
    // Cho phép xóa
}
```

## 2. Ràng Buộc Chỉnh Sửa Thời Gian và Địa Điểm

### Backend: GET /api/Event/{id}/edit-status

**Quy tắc:**
- ✅ Không cho phép chỉnh sửa event đã có vé được bán
- ✅ Chỉ cho phép chỉnh sửa thời gian/địa điểm **trước 48 giờ** tính từ thời gian bắt đầu sự kiện
- ✅ Sau 48 giờ → Block và thông báo số giờ còn lại

**Response:**
```json
{
  "eventId": 1,
  "canEdit": true/false,
  "canEditTimeLocation": true/false,
  "hasTicketsSold": true/false,
  "hoursUntilStart": 72.5,
  "message": "..."
}
```

**Code:**
```csharp
// Kiểm tra xem có vé đã bán chưa
var hasTicketsSold = await _eventService.CheckHasTicketsSoldAsync(id);

// Kiểm tra thời gian
var hoursUntilStart = (eventData.StartTime - DateTime.UtcNow).TotalHours;
var canEditTimeLocation = hoursUntilStart >= 48;

// Message động
message = !canEditTimeLocation
    ? $"Chỉ có thể chỉnh sửa thời gian và địa điểm trước 48 giờ. Còn {Math.Round(hoursUntilStart, 1)} giờ đến sự kiện."
    : "Có thể chỉnh sửa sự kiện"
```

## 3. Methods Mới Trong Backend

### CheckHasPaidTicketsAsync()
Kiểm tra event có vé đã được mua thành công (Order.Status = "Paid") không.

```csharp
public async Task<bool> CheckHasPaidTicketsAsync(int eventId)
{
    var ticketTypeIds = eventData.TicketTypes.Select(tt => tt.TicketTypeId).ToList();
    
    var hasPaidTickets = await _context.OrderItems
        .Include(oi => oi.Order)
        .Where(oi => ticketTypeIds.Contains(oi.TicketTypeId) 
            && oi.Order.Status == "Paid"
            && oi.Status != "Cancelled")
        .AnyAsync();
    
    return hasPaidTickets;
}
```

**Safety:** Trả về `true` nếu có lỗi để không cho xóa.

### CheckHasTicketsSoldAsync()
Kiểm tra event có vé nào được bán chưa (bao gồm cả Pending và Paid).

```csharp
public async Task<bool> CheckHasTicketsSoldAsync(int eventId)
{
    var hasTicketsSold = await _context.OrderItems
        .Where(oi => ticketTypeIds.Contains(oi.TicketTypeId) 
            && oi.Status != "Cancelled")
        .AnyAsync();
    
    return hasTicketsSold;
}
```

## 4. Phân Biệt 2 Methods

| Method | Mục đích | Sử dụng cho |
|--------|----------|-------------|
| `CheckHasTicketsSoldAsync` | Kiểm tra có vé bất kỳ (Pending, Paid) | Chặn chỉnh sửa event |
| `CheckHasPaidTicketsAsync` | Chỉ kiểm tra vé đã thanh toán (Paid) | Chặn xóa event |

**Lý do:** 
- Chỉnh sửa → Chặn khi có bất kỳ vé nào đã bán
- Xóa → Chỉ chặn khi có vé đã thanh toán (Paid)

## 5. Luồng Hoạt Động

### Khi Click "Xóa Event":
```
User click "Xóa" → Confirm dialog
→ Backend kiểm tra:
  1. Quyền ownership
  2. Có vé Paid không?
  → Nếu có → Block + thông báo
  → Nếu không → Cho phép xóa
```

### Khi Click "Chỉnh Sửa Event":
```
User click "Chỉnh sửa" 
→ Backend kiểm tra edit-status:
  1. Có vé bán chưa?
  2. Còn cách start time bao nhiêu giờ?
  → Nếu < 48h → Block + thông báo số giờ còn lại
  → Nếu >= 48h → Cho phép edit
```

## API Response Examples

### Delete với vé Paid:
```json
HTTP 400 Bad Request
{
  "message": "Không thể xóa sự kiện đã có vé được mua thành công. Hãy liên hệ hỗ trợ nếu muốn hủy sự kiện."
}
```

### Edit trước 48h:
```json
HTTP 200 OK
{
  "eventId": 1,
  "canEdit": true,
  "canEditTimeLocation": false,
  "hasTicketsSold": false,
  "hoursUntilStart": 35.2,
  "message": "Chỉ có thể chỉnh sửa thời gian và địa điểm trước 48 giờ. Còn 35.2 giờ đến sự kiện."
}
```

### Edit sau 48h:
```json
HTTP 200 OK
{
  "eventId":104,
  "canEdit": true,
  "canEditTimeLocation": true,
  "hasTicketsSold": false,
  "hoursUntilStart": 120.5,
  "message": "Có thể chỉnh sửa sự kiện"
}
```

## Testing Checklist

- [ ] Test xóa event không có vé
- [ ] Test xóa event có vé Pending (chưa thanh toán)
- [ ] Test xóa event có vé Paid (đã thanh toán) → Should block
- [ ] Test chỉnh sửa event > 48h → Should allow
- [ ] Test chỉnh sửa event < 48h → Should block
- [ ] Test chỉnh sửa event đã có vé bán → Should block

## Notes

- Ràng buộc 48 giờ giúp khách hàng có đủ thời gian chuẩn bị
- Xóa chỉ chặn vé Paid để tránh spam order Pending
- Thông báo rõ ràng giúp host hiểu lý do bị chặn

---

# 4. QUẢN LÝ ẢNH

## 4.1. Hướng dẫn Commit Ảnh vào Git

### Chuẩn bị ảnh mẫu

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

### Commit vào Git

```bash
# 1. Kiểm tra trạng thái
git status

# 2. Add tất cả thay đổi
git add .

# 3. Commit
git commit -m "feat: Chuyển ảnh sang assets/images/ để commit vào git

- Tạo thư mục assets/images/ cho ảnh mẫu
- Cập nhật .gitignore để track assets/images/
- Cập nhật FileManagementService, AuthController, Program.cs
- Cập nhật SampleData_Insert.sql với đường dẫn mới
- Ảnh giờ được commit vào git, clone về là có sẵn"

# 4. Push lên GitHub
git push origin main
```

### Troubleshooting

**Vấn đề: Ảnh không được add vào git**
```bash
# Force add
git add -f assets/images/
```

**Vấn đề: File quá lớn**
- Avatar: 400x400px
- Event: 1200x630px
- Chất lượng JPG: 80-85%

## 4.2. Khắc phục: File ảnh không tìm thấy

### Vấn đề

Script export tìm thấy ảnh trong database nhưng không tìm thấy file thực tế trên disk.

**Lý do có thể:**
1. File đã bị xóa
2. File ở thư mục khác
3. Database có GUID nhưng file thực tế tên khác

### Giải pháp

#### Cách 1: Chấp nhận dùng GUID trong SQL (KHUYẾN NGHỊ)

**Nếu bạn không có file ảnh gốc:**
- SQL vẫn export được với đường dẫn GUID
- Khi chạy SQL, database sẽ có data
- Ảnh sẽ không hiển thị nếu file không tồn tại (nhưng data vẫn đầy đủ)
- Bạn có thể upload ảnh mới sau và cập nhật database

**Ưu điểm:** Không cần làm gì, SQL vẫn dùng được

#### Cách 2: Upload lại ảnh vào assets/images/events/

**Nếu bạn có ảnh (từ nguồn khác hoặc backup):**

```powershell
# Copy ảnh vào thư mục với đúng tên GUID
Copy-Item "path\to\your\image.jpg" "assets\images\events\b33c34c7-d13b-4531-a7b7-a21add3fd730.jpg"
Copy-Item "path\to\your\image2.png" "assets\images\events\d031d269-2b9e-4981-9087-72332ce748f9.png"
```

Sau đó chạy lại script:
```
.\auto_export_all.bat
```

#### Cách 3: Tạo ảnh placeholder (tạm thời)

**Nếu muốn có ảnh để test:**

```powershell
# Chạy script này để tạo ảnh placeholder với đúng tên GUID
$guids = @(
    "b33c34c7-d13b-4531-a7b7-a21add3fd730.jpg",
    "d031d269-2b9e-4981-9087-72332ce748f9.png",
    # ... thêm các GUID khác
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
```

### Kiểm tra files hiện có

```powershell
# Xem files trong wwwroot/uploads/events/
Get-ChildItem "src\wwwroot\uploads\events\" | Select-Object Name

# Xem files trong assets/images/events/
Get-ChildItem "assets\images\events\" -ErrorAction SilentlyContinue | Select-Object Name
```

### Kết luận

**Khuyến nghị:**
- Nếu không có ảnh gốc → Chấp nhận dùng GUID trong SQL (Cách 1)
- Nếu cần ảnh để test → Dùng placeholder (Cách 3)
- Nếu có ảnh gốc → Copy vào assets với đúng tên GUID (Cách 2)

**Lưu ý:** SQL vẫn export được và hoạt động bình thường, chỉ là ảnh không hiển thị nếu file không tồn tại.

## 4.3. Hướng dẫn Tự động hóa Export Sample Data

### ✨ Workflow CỰC KỲ ĐƠN GIẢN

**Trước đây (5 bước thủ công):**
```powershell
# ❌ Phức tạp, mất thời gian
1. Sửa mapping trong rename_images_for_export.ps1
2. .\rename_images_for_export.ps1
3. .\export_sample_data.ps1
4. Sửa mapping trong update_image_paths_in_sql.ps1
5. .\update_image_paths_in_sql.ps1
```

**Bây giờ (1 lệnh duy nhất):**
```powershell
# ✅ Tự động 100%
.\auto_export_all.ps1
```

### Hướng dẫn chi tiết

#### Bước 1: Tạo Events qua UI

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
```

#### Bước 2: Chạy script tự động

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

#### Bước 3: Review và Commit

```bash
# 1. Kiểm tra file SQL (optional)
notepad ExtendedSampleData_Insert.sql

# 2. Kiểm tra ảnh đã đổi tên
ls assets\images\events\

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

### Tính năng của Script Tự động

#### 1. Tự động đặt tên ảnh thông minh

**Quy tắc đặt tên:**
- `event_{EventId}_{slug}_{index}.jpg`
- Slug = title không dấu, chữ thường, thay space bằng _

**Ví dụ:**
```
Event 1: "Workshop Lập Trình Python"
  → event_1_workshop_lap_trinh_python_1.jpg
  → event_1_workshop_lap_trinh_python_2.jpg
```

#### 2. Xử lý tiếng Việt

Script tự động chuyển đổi:
- `à á ả ã ạ ă ắ ằ ẳ ẵ ặ â ấ ầ ẩ ẫ ậ` → `a`
- `è é ẻ ẽ ẹ ê ế ề ể ễ ệ` → `e`
- `ì í ỉ ĩ ị` → `i`
- `ò ó ỏ õ ọ ô ố ồ ổ ỗ ộ ơ ớ ờ ở ỡ ợ` → `o`
- `ù ú ủ ũ ụ ư ứ ừ ử ữ ự` → `u`
- `ỳ ý ỷ ỹ ỵ` → `y`
- `đ` → `d`

#### 3. Xử lý Avatar

Avatar cũng tự động đổi tên:
- `user_{UserId}.jpg`

### Troubleshooting

**Vấn đề 1: "Cannot connect to database"**
```powershell
# Kiểm tra SQL Server đang chạy
# Thử kết nối bằng SSMS trước

# Hoặc thay đổi server name:
.\auto_export_all.ps1 -ServerName "localhost\SQLEXPRESS"
```

**Vấn đề 2: "File already exists"**
```powershell
# Xóa ảnh cũ
Remove-Item assets\images\events\event_*.jpg

# Chạy lại
.\auto_export_all.ps1
```

**Vấn đề 3: "EventDetails JSON parse error"**

Script sẽ báo warning nhưng vẫn tiếp tục. Event đó sẽ không có ảnh trong SQL.

### So sánh

| | Thủ công | Tự động |
|---|---|---|
| **Số bước** | 5 bước | 1 bước |
| **Thời gian** | 15-20 phút | 30 giây |
| **Sửa mapping** | Phải sửa 2 lần | Tự động |
| **Tên ảnh** | Tự đặt | Tự động sinh |
| **Dễ nhầm** | Cao | Không |
| **Xử lý tiếng Việt** | Thủ công | Tự động |

---

# 5. ĐỀ XUẤT CẢI TIẾN

## 📊 TỔNG QUAN HIỆN TRẠNG

### ✅ Điểm mạnh - Đã hoàn thành
- **Kiến trúc rõ ràng:** Sử dụng Repository Pattern, Service Layer, DTOs
- **Bảo mật:** JWT Authentication, Authorization checks
- **Testing:** 70.8% code coverage với 85+ test cases
- **Documentation:** Swagger UI, README files
- ✅ **Pagination:** Đã implement cho EventController, OrderController, TicketController
- ✅ **Global Exception Handler:** Đã có và được đăng ký trong Program.cs
- ✅ **Memory Cache:** Đã đăng ký và có sử dụng trong EventService

### ⚠️ Điểm cần cải thiện - Cần làm tiếp
- ⚠️ **Caching chưa đầy đủ:** Chỉ có trong EventService, cần mở rộng cho các service khác
- ❌ **Input Validation:** Chưa có FluentValidation hoặc validation middleware
- ❌ **Rate Limiting:** Chưa có mechanism bảo vệ API
- ❌ **Refresh Token:** Chưa có mechanism refresh JWT token
- ❌ **Inventory Reservation:** TODO comments trong `inventoryService.js` chưa được implement
- ❌ **Payment Gateway:** Chỉ hỗ trợ Wallet, chưa có Credit Card/Bank Transfer
- ⚠️ **Logging:** Đang dùng `Console.WriteLine` nhiều, nên chuyển sang Serilog
- ❌ **Performance:** Thiếu database indexes, query optimization

## 🎯 CẢI TIẾN THEO ĐỘ ƯU TIÊN

### 🔴 PRIORITY 1: CRITICAL - Cần làm ngay

#### 1.1. **Thêm Pagination cho tất cả API endpoints trả về danh sách**

**Vấn đề:** Hiện tại các API như `GET /api/Event`, `GET /api/Order/my-orders` load toàn bộ data, có thể gây vấn đề performance khi data lớn.

**Giải pháp:**
```csharp
// Tạo DTO cho Pagination
public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

// Extension method cho IQueryable
public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int pageSize)
{
    return source.Skip((page - 1) * pageSize).Take(pageSize);
}
```

**Files cần sửa:**
- `src/Controllers/EventController.cs` - Thêm pagination cho GetAllEvents
- `src/Controllers/OrderController.cs` - Thêm pagination cho GetMyOrders
- `src/Controllers/TicketController.cs` - Thêm pagination cho GetMyTickets
- `src/Controllers/WishlistController.cs` - Thêm pagination

#### 1.2. **Implement Inventory Reservation/Release Logic**

**Vấn đề:** `inventoryService.js` có TODO comments, chưa có logic reserve/release tickets khi user đang checkout.

**Backend:**
```csharp
// Thêm model Reservation
public class TicketReservation
{
    public int ReservationId { get; set; }
    public int TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    
    public TicketType TicketType { get; set; }
    public User User { get; set; }
}

// Service method
public async Task<string> ReserveTicketsAsync(int ticketTypeId, int quantity, int userId)
{
    // Check availability
    // Lock tickets in database
    // Create reservation record
    // Set expiration (15 minutes)
    // Return reservation ID
}
```

**Frontend:**
```javascript
// inventoryService.js - Implement reserve/release
static async reserveTickets(ticketTypeId, quantity) {
  const response = await fetch(`/api/Inventory/reserve`, {
    method: 'POST',
    body: JSON.stringify({ ticketTypeId, quantity }),
    headers: { 'Authorization': `Bearer ${token}` }
  });
  // Return reservation ID
}
```

#### 1.3. **Standardize Error Handling với Global Exception Handler**

**Vấn đề:** Error handling không thống nhất giữa các controllers.

**Giải pháp:**
```csharp
// Middleware/GlobalExceptionHandler.cs
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var response = new ErrorResponse
        {
            Success = false,
            Message = exception.Message,
            StatusCode = httpContext.Response.StatusCode
        };

        switch (exception)
        {
            case ArgumentException argEx:
                response.StatusCode = 400;
                response.Message = argEx.Message;
                break;
            case UnauthorizedAccessException:
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                break;
            case NotFoundException:
                response.StatusCode = 404;
                break;
            default:
                response.StatusCode = 500;
                response.Message = "Internal server error";
                break;
        }

        httpContext.Response.StatusCode = response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}

// Register trong Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
app.UseExceptionHandler();
```

#### 1.4. **Thêm Input Validation Middleware**

**Giải pháp:**
```csharp
// Sử dụng FluentValidation (recommended)
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDTO>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.EventId).GreaterThan(0);
        RuleFor(x => x.Quantity).InclusiveBetween(1, 100);
    }
}
```

### 🟡 PRIORITY 2: IMPORTANT - Nên làm sớm

#### 2.1. **Implement Caching với Redis (hoặc Memory Cache)**

**Implementation:**
```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Service
public class EventService
{
    private readonly IMemoryCache _cache;
    
    public async Task<EventDTO> GetEventByIdAsync(int eventId)
    {
        var cacheKey = $"event_{eventId}";
        
        if (_cache.TryGetValue(cacheKey, out EventDTO cachedEvent))
        {
            return cachedEvent;
        }
        
        var eventData = await _repository.GetByIdAsync(eventId);
        var dto = _mapper.MapToDto(eventData);
        
        _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(10));
        return dto;
    }
}
```

#### 2.2. **Thêm Rate Limiting**

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthPolicy", opt =>
    {
        opt.PermitLimit = 5; // 5 requests
        opt.Window = TimeSpan.FromMinutes(1);
    });
    
    options.AddFixedWindowLimiter("ApiPolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

// Apply trong controllers
[EnableRateLimiting("ApiPolicy")]
[ApiController]
public class OrderController : ControllerBase
{
    // ...
}
```

#### 2.3. **Implement Refresh Token Mechanism**

```csharp
// Thêm RefreshToken vào User model hoặc tạo bảng riêng
public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}

// AuthController
[HttpPost("refresh-token")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
{
    // Validate refresh token
    // Generate new JWT
    // Return new tokens
}
```

#### 2.4. **Thêm Payment Gateway Integration (Stripe/VNPay)**

```csharp
// Payment Strategy Pattern
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}

public class WalletPaymentService : IPaymentService { }
public class StripePaymentService : IPaymentService { }
public class VNPayPaymentService : IPaymentService { }
```

### 🟢 PRIORITY 3: NICE TO HAVE - Làm khi có thời gian

- **Email Notifications** - Order confirmation, Payment success, Event reminders
- **QR Code Generation** cho Tickets
- **Real-time Notifications** với SignalR
- **Enhanced Logging** với Serilog
- **Health Checks** cho monitoring

## 📊 PERFORMANCE OPTIMIZATIONS

### Database Indexing
```sql
-- Thêm indexes cho frequently queried columns
CREATE INDEX IX_Events_StartDate ON Events(StartDate);
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Tickets_UserId ON Tickets(UserId);
CREATE INDEX IX_TicketTypes_EventId ON TicketTypes(EventId);
```

### Query Optimization
```csharp
// Include related data trong 1 query
var events = await _context.Events
    .Include(e => e.TicketTypes)
    .Include(e => e.Organizer)
    .ToListAsync();
```

---

# 6. QUICK IMPROVEMENT CHECKLIST

## 🔴 PRIORITY 1 - Critical (Làm ngay)

### Backend
- [ ] **Pagination cho API endpoints**
  - Thêm `PagedRequest` và `PagedResponse<T>` DTOs
  - Update: `EventController.GetAllEvents()`
  - Update: `OrderController.GetMyOrders()`
  - Update: `TicketController.GetMyTickets()`
  - Update: `WishlistController.GetWishlist()`
  
- [ ] **Inventory Reservation System**
  - Tạo model `TicketReservation`
  - Implement `ReserveTicketsAsync()` trong `OrderService`
  - Implement `ReleaseReservationAsync()`
  - Add API endpoint: `POST /api/Inventory/reserve`
  - Frontend: Implement `reserveTickets()` và `releaseReservation()` trong `inventoryService.js`

- [ ] **Global Exception Handler**
  - Tạo `GlobalExceptionHandler.cs`
  - Register trong `Program.cs`: `builder.Services.AddExceptionHandler<GlobalExceptionHandler>()`
  - Standardize error response format

- [ ] **Input Validation**
  - Install FluentValidation: `dotnet add package FluentValidation.AspNetCore`
  - Create validators cho các DTOs
  - Register: `builder.Services.AddFluentValidation(...)`

## 🟡 PRIORITY 2 - Important (Làm sớm)

### Backend
- [ ] **Caching**
  - Install: `dotnet add package Microsoft.Extensions.Caching.Memory`
  - Hoặc Redis: `dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis`
  - Cache events (10 phút)
  - Cache ticket types (5 phút)
  - Cache user profiles (15 phút)

- [ ] **Rate Limiting**
  - Install: `dotnet add package Microsoft.AspNetCore.RateLimiting`
  - Create policies: Auth (5 req/min), API (100 req/min)
  - Apply `[EnableRateLimiting]` attributes

- [ ] **Refresh Token**
  - Tạo model `RefreshToken`
  - Add migration
  - Implement `RefreshTokenAsync()` trong `AuthService`
  - Add endpoint: `POST /api/Auth/refresh-token`

- [ ] **Payment Gateway**
  - Tạo `IPaymentService` interface
  - Implement: `WalletPaymentService`, `StripePaymentService`, `VNPayPaymentService`
  - Update `OrderController.ProcessPayment()`

- [ ] **Event Statistics API**
  - Endpoint: `GET /api/Event/{id}/statistics`
  - Return: revenue, tickets sold, check-in rate, etc.

## 🟢 PRIORITY 3 - Nice to Have

- [ ] **Email Notifications**
- [ ] **QR Code Generation**
- [ ] **SignalR Real-time**
- [ ] **Serilog Logging**
- [ ] **Health Checks**

## 🎨 Frontend Improvements

- [ ] **State Management** - Create `GlobalStateContext`
- [ ] **Error Boundaries** - Enhance existing `ErrorBoundary.jsx`
- [ ] **Loading States** - Consistent loading spinners
- [ ] **Image Optimization** - Lazy loading images
- [ ] **Unit Tests** - Target: 60%+ coverage

## 🔒 Security

- [ ] **Input Sanitization** - Install `HtmlSanitizer`
- [ ] **XSS Prevention** - Use `DOMPurify` in frontend
- [ ] **CORS Configuration** - Restrict in production

## 📊 Performance

- [ ] **Database Indexes** - Add indexes for frequently queried columns
- [ ] **Query Optimization** - Use `.Include()`, `.AsNoTracking()`
- [ ] **Frontend Optimization** - Code splitting, lazy load routes

---

# 7. CÁC BƯỚC TIẾP THEO

## 🎯 Roadmap

### Tuần 1: Polish Core Features
- [ ] Complete Image Display Locations Modal
- [ ] Implement Inventory Reservation Logic
- [ ] Add error handling improvements
- [ ] Review và fix any console warnings/errors

### Tuần 2: Payment & Testing
- [ ] Implement Credit Card payment gateway (mock hoặc integrate Stripe/VNPay)
- [ ] Implement Bank Transfer payment
- [ ] Improve test coverage to 80%
- [ ] Add integration tests

### Tuần 3: UI/UX & Documentation
- [ ] Create EditEventPage component
- [ ] Add event statistics dashboard
- [ ] Update README với deployment guide
- [ ] Create API documentation (Swagger đã có)

### Tuần 4: Deployment & Demo
- [ ] Deploy backend to cloud (Azure/AWS/Heroku)
- [ ] Deploy frontend to Vercel/Netlify
- [ ] Create demo video
- [ ] Prepare presentation

## 🐛 COMMON ISSUES & SOLUTIONS

### Issue 1: CORS Error
**Solution:** Kiểm tra CORS configuration trong `Program.cs`

### Issue 2: JWT Token Expired
**Solution:** Implement refresh token mechanism

### Issue 3: Upload File Size Limit
**Solution:** Configure trong `Program.cs` hoặc `appsettings.json`

### Issue 4: Database Connection Error
**Solution:** Kiểm tra connection string trong `appsettings.json`

## 💡 GỢI Ý CẢI TIẾN

### Features có thể thêm:
1. **Real-time notifications** cho ticket sales, event updates
2. **Email/SMS notifications** cho order confirmations
3. **QR Code generation** cho tickets
4. **Event analytics dashboard** cho Host
5. **Social sharing** - share events lên Facebook/Instagram
6. **Review & Rating** system cho events
7. **Admin panel** để quản lý users, events
8. **Multi-language support** (i18n)

### Technical Improvements:
1. **Performance:**
   - Add caching (Redis) cho frequently accessed data
   - Optimize database queries
   - Add pagination everywhere

2. **Security:**
   - Add rate limiting
   - Implement CORS properly
   - Add input sanitization

3. **Monitoring:**
   - Add logging (Serilog)
   - Add health checks
   - Add application insights

## 📚 TÀI LIỆU THAM KHẢO

### Backend:
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://jwt.io/)

### Frontend:
- [React Documentation](https://react.dev/)
- [Material-UI](https://mui.com/)
- [React Router](https://reactrouter.com/)

### Testing:
- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)

## 📞 LIÊN LẠC & HỖ TRỢ

### Team Members:
- Thiên - OrderController & OrderService testing
- Minh - OrderController & TicketService testing  
- Khanh - OrderService testing
- Tân - Controller, Repository & Wallet testing
- A Duy - OrderService & TicketService testing

### Useful Commands:

```bash
# Backend
cd src
dotnet run

# Frontend
npm start

# Tests
dotnet test
dotnet test --collect:"XPlat Code Coverage"

# Database
dotnet ef migrations add <name>
dotnet ef database update
```

## 🎉 KẾT LUẬN

Dự án của bạn đã có **foundation rất tốt** với các tính năng core đã hoàn thành. Hiện tại bạn đang ở giai đoạn **polish and enhancement** - đây là giai đoạn quan trọng để làm cho project trở nên **production-ready**.

**Hãy bắt đầu với:**
1. ✅ Xem lại danh sách TODO items
2. ✅ Chọn 1-2 tasks để implement
3. ✅ Test thoroughly
4. ✅ Commit và push code
5. ✅ Repeat!

**Remember:** Mỗi commit nhỏ đều đáng giá. Đừng bị overwhelm bởi todo list lớn - cứ làm từng bước một! 💪

Good luck! 🚀

