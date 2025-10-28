# Ràng Buộc Chỉnh Sửa và Xóa Event

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
    ? $"Chỉ có thể chỉnh sửa thời gian và địa điểm trước 48 giờ. Còn {Math.Round(hoursUntilStart, 1)} giờ đến sự kiện کشور."
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
Kiểm tra event có vé nào được bán chưa receiving(bao gồm cả Pending và Paid).

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

## 6. API Response Examples

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

## Files Modified

### Backend
- `src/Controllers/EventController.cs`
  - Updated `DeleteEvent()` - Thêm ràng buộc vé Paid
  - Updated `GetEventEditStatus()` - Thêm ràng buộc 48h
  
- `src/Services/EventService.cs`
  - Added `CheckHasPaidTicketsAsync()` method
  
- `src/Business/IEventService.cs`
  - Added `CheckHasPaidTicketsAsync()` interface method

### Frontend
- `src/pages/MyEventsPage.jsx`
  - Nút "Xóa" đã có sẵn
  - Nút "Chỉnh sửa" đã có sẵn

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

