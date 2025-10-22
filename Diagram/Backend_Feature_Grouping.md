# BACKEND FEATURE GROUPING - GOM CÁC LỚP THÀNH CỤM CHỨC NĂNG

## TỔNG QUAN
Tài liệu này mô tả cách gom các lớp backend thành các cụm chức năng dựa trên danh sách 41 chức năng đã được phân công.

---

## CÁC CỤM CHỨC NĂNG CHÍNH

### 1. AUTHENTICATION & AUTHORIZATION (Xác thực & Phân quyền)
**Người phụ trách:** Thiên, Minh, A.Duy

**Các chức năng:**
- **01. Login** (Thiên)
- **02. Logout** (Thiên) 
- **03. Forgot Password** (Minh)
- **04. Change Password** (A.Duy)
- **24. Register** (Thiên)

**Các lớp liên quan:**
- `AuthController.cs`
- `AuthService.cs`
- `IAuthService.cs`
- `JwtService.cs`
- `IJwtService.cs`
- `PasswordService.cs`
- `IPasswordService.cs`
- `OtpService.cs`
- `IOtpService.cs`
- `EmailService.cs`
- `IEmailService.cs`
- `AuthDTOs.cs`
- `User.cs`
- `OtpCode.cs`

---

### 2. PROFILE MANAGEMENT (Quản lý Hồ sơ)
**Người phụ trách:** A.Duy

**Các chức năng:**
- **04. View Profile** (A.Duy)
- **05. Update Profile** (A.Duy)

**Các lớp liên quan:**
- `ProfileDTOs.cs`
- `UserMapper.cs`
- `IUserMapper.cs`
- `UserRepository.cs`
- `IUserRepository.cs`

---

### 3. WALLET MANAGEMENT (Quản lý Ví)
**Người phụ trách:** Tân

**Các chức năng:**
- **07. View My Wallet** (Tân)
- **08. View Balance** (Tân)

**Các lớp liên quan:**
- `WalletController.cs`
- `WalletService.cs`
- `IWalletService.cs`
- `WalletDTOs.cs`
- `WalletTransaction.cs`

---

### 4. TICKET MANAGEMENT (Quản lý Vé)
**Người phụ trách:** Khanh

**Các chức năng:**
- **10. My Ticket** (Khanh)
- **11. View TicketQR** (Khanh)
- **12. Request Refund** (Khanh)

**Các lớp liên quan:**
- `TicketController.cs`
- `TicketService.cs`
- `ITicketService.cs`
- `TicketDTOs.cs`
- `Ticket.cs`
- `TicketType.cs`

---

### 5. WISHLIST MANAGEMENT (Quản lý Danh sách Yêu thích)
**Người phụ trách:** Khanh

**Các chức năng:**
- **09. Add to Wishlist** (Khanh)

**Các lớp liên quan:**
- `WishlistController.cs`
- `WishlistService.cs`
- `IWishlistService.cs`
- `WishlistDTOs.cs`
- `WishlistMapper.cs`
- `IWishlistMapper.cs`
- `WishlistItem.cs`

---

### 6. EVENT MANAGEMENT (Quản lý Sự kiện)
**Người phụ trách:** Thiên, Tân

**Các chức năng:**
- **22. View Event** (Thiên)
- **23. View Detail** (Thiên)
- **25. Create Events** (Tân)
- **26. Edit Events** (Tân)
- **27. Create Tickets** (Tân)
- **28. View Events List** (Tân)

**Các lớp liên quan:**
- `EventController.cs`
- `EventService.cs`
- `IEventService.cs`
- `EventDTOs.cs`
- `EventMapper.cs`
- `IEventMapper.cs`
- `EventRepository.cs`
- `IEventRepository.cs`
- `Event.cs`

---

### 7. ORDER & PAYMENT (Đặt hàng & Thanh toán)
**Người phụ trách:** Thiên

**Các chức năng:**
- **17. Buy Ticket** (Thiên)
- **18. Buy Product** (Thiên)
- **19. Payment** (Thiên)
- **15. Apply Voucher** (Thiên)

**Các lớp liên quan:**
- `OrderController.cs`
- `OrderService.cs`
- `IOrderService.cs`
- `OrderDTOs.cs`
- `OrderMapper.cs`
- `IOrderMapper.cs`
- `OrderRepository.cs`
- `IOrderRepository.cs`
- `Order.cs`
- `OrderItem.cs`
- `Payment.cs`
- `OrderCleanupService.cs`

---

### 8. VOUCHER MANAGEMENT (Quản lý Mã giảm giá)
**Người phụ trách:** Tân

**Các chức năng:**
- **29. Create Voucher** (Tân)
- **30. List Voucher** (Tân)

**Các lớp liên quan:**
- `VoucherController.cs`
- `VoucherService.cs`
- `VoucherDTOs.cs`
- `Voucher.cs`

---

### 9. PRODUCT MANAGEMENT (Quản lý Sản phẩm)
**Người phụ trách:** Tân

**Các chức năng:**
- **31. Create Product** (Tân)

**Các lớp liên quan:**
- (Cần tạo thêm các lớp cho Product management)

---

### 10. ADMIN MANAGEMENT (Quản lý Admin)
**Người phụ trách:** Tân

**Các chức năng:**
- **32. View Customers List** (Tân)
- **33. View Feedbacks** (Tân)
- **34. Send Notification** (Tân)
- **36. View Statistic** (Tân)
- **37. View Dashboard** (Tân)
- **38. Access and Work with Dashboard** (Tân)
- **39. View Account List** (Tân)
- **40. Account Moderation** (Tân)
- **41. Ban Accounts** (Tân)

**Các lớp liên quan:**
- (Cần tạo thêm các lớp cho Admin management)

---

### 11. ADDITIONAL FEATURES (Tính năng Bổ sung)
**Người phụ trách:** Tân

**Các chức năng:**
- **13. AI Suggestion** (Tân)
- **14. Views Notification** (Tân)
- **16. Report Event** (Tân)
- **20. Feedback** (Tân)
- **21. Searching** (Tân)
- **35. Edit Background** (Tân)

**Các lớp liên quan:**
- (Cần tạo thêm các lớp cho các tính năng bổ sung)

---

## CẤU TRÚC THƯ MỤC ĐỀ XUẤT

```
src/
├── Features/
│   ├── 01_Authentication/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 02_ProfileManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 03_WalletManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 04_TicketManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 05_WishlistManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 06_EventManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 07_OrderPayment/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 08_VoucherManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 09_ProductManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   ├── 10_AdminManagement/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── DTOs/
│   │   └── Models/
│   └── 11_AdditionalFeatures/
│       ├── Controllers/
│       ├── Services/
│       ├── DTOs/
│       └── Models/
├── Shared/
│   ├── Data/
│   ├── Helpers/
│   ├── Constants/
│   └── Extensions/
└── Program.cs
```

---

## LỢI ÍCH CỦA VIỆC GOM CỤM

1. **Tổ chức rõ ràng:** Mỗi cụm chức năng có trách nhiệm riêng biệt
2. **Dễ bảo trì:** Tìm kiếm và sửa lỗi nhanh chóng
3. **Phân công rõ ràng:** Mỗi người phụ trách một cụm cụ thể
4. **Mở rộng dễ dàng:** Thêm tính năng mới vào cụm tương ứng
5. **Tái sử dụng:** Các lớp trong cùng cụm có thể tái sử dụng lẫn nhau

---

## GHI CHÚ

- Các lớp hiện tại sẽ được di chuyển vào các cụm tương ứng
- Các lớp mới cần tạo sẽ được thêm vào cụm phù hợp
- Mỗi cụm sẽ có cấu trúc thư mục riêng với Controllers, Services, DTOs, Models
- Các lớp dùng chung sẽ được đặt trong thư mục Shared
