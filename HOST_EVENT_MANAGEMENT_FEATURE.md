# Tính Năng Quản Lý Event Cho Host

## Tổng Quan

Tính năng này cho phép các host (user đã tạo ít nhất 1 event) quản lý các sự kiện của mình, bao gồm xem danh sách, chỉnh sửa và xóa. Trace được bảo vệ bởi ràng buộc không cho edit event đã có vé được bán.

## Vấn Đề Bảo Mật Được Giải Quyết

**Vấn đề**: Host có thể thay đổi thông tin sự kiện vào phút chót nhằm trục lợi (ví dụ: thay đổi giá vé, giảm chất lượng sự kiện sau khi đã bán vé).

**Giải pháp**: Chỉ cho phép edit event nếu event đó CHƯA CÓ vé nào được bán.

## Backend Changes

### 1. EventController.cs - Thêm Endpoints

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

### 2. EventService.cs - Thêm Logic Kiểm Tra

#### Method: CheckHasTicketsSoldAsync(int eventId)
- **Mô tả**: Kiểm tra xem event có vé nào đã được bán chưa
- **Logic**: 
  1. Lấy danh sách TicketTypes của event
  2. Kiểm tra trong bảng OrderItems xem có ticket types nào có Status != "Cancelled" không
  3. Trả về true nếu có vé đã bán

```csharp
public async Task<bool> CheckHasTicketsSoldAsync(int eventId)
{
    // method
}
```

### 3. IEventService.cs - Thêm Interface Method

```csharp
Task<bool> CheckHasTicketsSoldAsync(int eventId);
```

### 4. Dependency Injection

EventService đã được Burned với EventDBContext để có thể query database:

```csharp
public EventService(
    IEventRepository eventRepository, 
    IEventMapper eventMapper, 
    IFileManagementService fileManagementService, 
    EventDBContext context)
```

## Frontend Changes

### 1. MyEventsPage.jsx - Trang Quản Lý Events

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

### 2. Header.jsx - Thêm Nút "My Events"

#### Logic hiển thị:
- Chỉ hiển thị nút "My Events" khi user đã có ít nhất 1 event
- Gọi API `eventsAPI.getMyEvents()` khi user đăng nhập
- Hiển thị trong cả desktop navigation và mobile dropdown menu

```javascript
const [hasEvents, setHasEvents] = useState(false);
const [checkingEvents, setCheckingEvents] = useState(true);

useEffect(() => {
  if (user) {
    checkHasEvents();
  }
}, [user]);

// Chỉ hiển thị khi !checkingEvents && hasEvents
```

### 3. App.js - Thêm Route

```javascript
<Route 
  path="/my-events" 
  element={
    <ProtectedRoute>
      <MyEventsPage />
    </ProtectedRoute>
  } 
/>
```

### 4. apiClient.js - Thêm API Method

```javascript
getEditStatus: async (eventId) => {
  return api.get(`/Event/${eventId}/edit-status`);
}
```

## Luồng Hoạt Động

### 1. User trở thành Host
```
User tạo event → Hệ thống tự động coi user là host
```

### 2. Hiển thị nút "My Events"
```
User đăng nhập → Header kiểm tra events của user
→ Nếu có events → Hiển thị nút "My Events"
```

### 3. Xem danh sách events
```
Click "My Events" → MyEventsPage
→ Gọi API GET /api/Event/my-events
→ Hiển thị grid cards
```

### 4. Xem event chi tiết
```
Click nút "Xem" → Chuyển đến /event/{eventId}
```

### 5. Chỉnh sửa event
```
Click nút "Chỉnh sửa" → Kiểm tra edit-status
→ Nếu canEdit = true → Cho phép edit
→ Nếu canEdit = false → Block và thông báo
```

### 6. Xóa event
```
Click nút "Xóa" → Confirmation dialog
→ User xác nhận → Gọi API DELETE
→ Refresh danh sách
```

## Bảo Mật

### Ràng buộc Edit
- ✅ Event chưa có vé được bán → Cho phép edit
- ❌ Event đã có vé được bán → Không cho phép edit
- Hiển thị message rõ ràng về lý do

### Ownership Check
- ✅ Chỉ host mới có thể xem/edit events của mình
- ✅ Backend kiểm tra `eventData.HostId != userId.Value`

## API Endpoints Summary

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | /api/Event/my-events | ✅ | Lấy events của user hiện tại |
| GET | /api/Event/{id}/edit-status | ✅ | Kiểm tra có thể edit hay không |
| DELETE | /api/Event/{id} | ✅ | Xóa event |

## UI/UX Features

### Status Badges
- **Đang mở** (Open) - Green
- **Nháp** (Draft) - Gray  
- **Đã đóng** (Closed) - Red
- **Đã hủy** (Cancelled) - Red

### Empty State
- Hiển thị khi user chưa có event nào
- Nút "Tạo Sự Kiện" để đi đến trang tạo event

### Card Design
- Ảnh sự kiện
- Tiêu đề
- Status badge
- Ngày giờ
- Địa điểm
- Category
- Action buttons

## Testing Checklist

- [ ] Test GET /api/Event/my-events với user có events
- [ ] Test GET /api/Event/my-events với user không có events
- [ ] Test GET /api/Event/{id}/edit-status với event chưa có vé
- [ ] Test GET /api/Event/{id}/edit-status với event đã có vé
- [ ] Test nút "My Events" hiển thị đúng khi user có events
- [ ] Test nút "Chỉnh sửa" block khi event đã có vé
- [ ] Test nút "Xóa" với confirmation

## Future Enhancements

### TODO:
1. **Tạo trang EditEventPage riêng** hoặc sử dụng lại CreateEventPage với mode edit
2. **Thêm pagination** cho danh sách events
3. **Thêm filter/sort** events theo status, ngày, etc.
4. **Thêm statistics** cho mỗi event (số vé đã bán, doanh thu, etc.)
5. **Thêm export** danh sách events ra Excel/PDF

## Files Modified

### Backend
- `src/Controllers/EventController.cs`
- `src/Services/EventService.cs`
- `src/Business/IEventService.cs`

### Frontend
- `src/pages/MyEventsPage.jsx` (NEW)
- `src/components/layout/Header.jsx`
- `src/App.js`
- `src/services/apiClient.js`

## Notes

- Tính năng này giúp host quản lý events dễ dàng hơn
- Ràng buộc edit bảo vệ khách hàng khỏi việc host thay đổi thông tin trục lợi
- User tự động trở thành host sau khi tạo event đầu tiên
- Nút "My Events" chỉ hiển thị cho những user đã có events

