# USE CASE DIAGRAM - THEGRIND5 EVENT MANAGEMENT SYSTEM

## 🎭 ACTORS (ROLES)

1. **Guest**: Khách không đăng nhập
2. **Customer**: Khách hàng đã đăng ký và đăng nhập
3. **Host**: Người tổ chức sự kiện (extends Customer)
4. **Admin**: Quản trị viên hệ thống (extends Customer)

---

# 1️⃣ GUEST USE CASES

## Actor: Guest

| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-GUEST-001** | Register Account | Đăng ký tài khoản mới | Email chưa tồn tại | Tài khoản được tạo, role = "Customer" |
| **UC-GUEST-002** | Login | Đăng nhập vào hệ thống | Có tài khoản hợp lệ | Nhận JWT token |
| **UC-GUEST-003** | Browse Events | Duyệt danh sách sự kiện có sẵn | - | Hiển thị danh sách có pagination |
| **UC-GUEST-004** | View Event Details | Xem chi tiết thông tin sự kiện | Event tồn tại | Hiển thị đầy đủ thông tin event |

**Total: 4 use cases**

---

# 2️⃣ CUSTOMER USE CASES

## Actor: Customer (extends Guest)

### Authentication & Profile
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-001** | View My Profile | Xem thông tin cá nhân | Đã đăng nhập | Hiển thị profile đầy đủ |
| **UC-CUST-002** | Update Profile | Cập nhật thông tin cá nhân | Đã đăng nhập | Profile được cập nhật |
| **UC-CUST-003** | Upload Avatar | Tải ảnh đại diện lên | Đã đăng nhập, file < 5MB | Avatar được lưu |

### Event Discovery
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-004** | Search Events | Tìm kiếm sự kiện theo từ khóa | Đã đăng nhập | Trả về kết quả tìm kiếm |
| **UC-CUST-005** | Filter Events | Lọc sự kiện theo category/date | Đã đăng nhập | Trả về danh sách đã lọc |

### Order Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-006** | Create Order | Tạo đơn hàng mua vé | Event tồn tại, còn vé | Order status = "Pending" |
| **UC-CUST-007** | Apply Voucher | Áp dụng mã giảm giá vào order | Có voucher hợp lệ | Discount được tính |
| **UC-CUST-008** | View My Orders | Xem danh sách đơn hàng | Đã đăng nhập | Hiển thị orders với pagination |
| **UC-CUST-009** | View Order Details | Xem chi tiết đơn hàng | Order tồn tại, là owner | Hiển thị đầy đủ thông tin |
| **UC-CUST-010** | Cancel Order | Hủy đơn hàng | Order status = "Pending" | Order status = "Cancelled" |

### Payment
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-011** | Process Payment | Thanh toán đơn hàng qua ví | Order status = "Pending" | Order status = "Paid", tickets được tạo |
| **UC-CUST-012** | Check Sufficient Balance | Kiểm tra số dư đủ thanh toán | Đã đăng nhập | Trả về hasSufficientBalance |

### Ticket Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-013** | View My Tickets | Xem danh sách vé của mình | Đã đăng nhập | Hiển thị tickets với pagination |
| **UC-CUST-014** | View Ticket Details | Xem chi tiết vé | Ticket tồn tại, là owner | Hiển thị đầy đủ thông tin vé |
| **UC-CUST-015** | Check In Ticket | Check-in khi tham gia event | Ticket status = "Assigned" | Ticket status = "Used" |
| **UC-CUST-016** | Refund Ticket | Hoàn tiền vé | Ticket status = "Assigned" | Ticket status = "Refunded", tiền được hoàn |
| **UC-CUST-017** | Validate Ticket | Kiểm tra tính hợp lệ của vé | Ticket tồn tại | Trả về isValid status |

### Wallet Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-018** | View Wallet Balance | Xem số dư ví | Đã đăng nhập | Hiển thị balance |
| **UC-CUST-019** | View Transaction History | Xem lịch sử giao dịch | Đã đăng nhập | Hiển thị transactions với pagination |
| **UC-CUST-020** | View Transaction Details | Xem chi tiết giao dịch | Transaction tồn tại, là owner | Hiển thị đầy đủ thông tin |
| **UC-CUST-021** | Deposit Money | Nạp tiền vào ví | Đã đăng nhập | Số dư tăng, transaction được tạo |
| **UC-CUST-022** | Withdraw Money | Rút tiền từ ví | Đủ số dư | Số dư giảm, transaction được tạo |

### Wishlist Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-023** | View Wishlist | Xem danh sách yêu thích | Đã đăng nhập | Hiển thị items |
| **UC-CUST-024** | Add To Wishlist | Thêm vé vào wishlist | Event tồn tại, chưa có | Item được thêm vào |
| **UC-CUST-025** | Update Wishlist Item | Cập nhật số lượng | Item tồn tại trong wishlist | Số lượng được cập nhật |
| **UC-CUST-026** | Remove From Wishlist | Xóa khỏi wishlist | Item tồn tại | Item bị xóa |
| **UC-CUST-027** | Bulk Remove Wishlist | Xóa nhiều items | Có nhiều items | Các items bị xóa |
| **UC-CUST-028** | Checkout From Wishlist | Mua vé từ wishlist | Wishlist không rỗng | Orders được tạo |

### Voucher
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-029** | Validate Voucher | Kiểm tra mã voucher hợp lệ | Có voucher code | Trả về isValid, discount amount |
| **UC-CUST-030** | View Voucher Info | Xem thông tin voucher | Voucher tồn tại | Hiển thị voucher details |

### Notifications & Communication
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-CUST-031** | View My Notifications | Xem danh sách thông báo | Đã đăng nhập | Hiển thị notifications với pagination |
| **UC-CUST-032** | View Notification Details | Xem chi tiết thông báo | Notification tồn tại | Hiển thị đầy đủ thông tin |
| **UC-CUST-033** | Mark Notification As Read | Đánh dấu thông báo đã đọc | Notification tồn tại | IsRead = true |
| **UC-CUST-034** | Mark All Notifications As Read | Đánh dấu tất cả đã đọc | Đã đăng nhập | Tất cả notifications đã đọc |
| **UC-CUST-035** | Delete Notification | Xóa thông báo | Notification tồn tại | Notification bị xóa |
| **UC-CUST-036** | View Notification Stats | Xem thống kê thông báo | Đã đăng nhập | Hiển thị total và unread count |
| **UC-CUST-037** | Receive Event Reminder | Nhận thông báo nhắc nhở sự kiện | Có vé cho event | Notification được tạo và email được gửi |
| **UC-CUST-038** | Receive Event Update | Nhận thông báo cập nhật sự kiện | Có vé cho event | Notification được tạo |
| **UC-CUST-039** | Receive Event Cancelled | Nhận thông báo hủy sự kiện | Có vé cho event | Notification và hoàn tiền được thực hiện |
| **UC-CUST-040** | Receive Order Confirmation | Nhận xác nhận đơn hàng | Order đã tạo | Notification và email được gửi |
| **UC-CUST-041** | Receive Payment Success | Nhận thông báo thanh toán thành công | Payment thành công | Notification và email được gửi |
| **UC-CUST-042** | Receive Refund Notification | Nhận thông báo hoàn tiền | Refund được xử lý | Notification và email được gửi |

**Total: 42 use cases** (4 từ Guest + 38 Customer-specific)


### Include Relationships (Customer-specific)
- **UC-CUST-007** (Apply Voucher) includes **UC-CUST-006** (Create Order)
- **UC-CUST-010** (Cancel Order) includes **UC-CUST-009** (View Order Details)

### Extend Relationships (Customer-specific)
- **UC-CUST-002** (Update Profile) extends **UC-CUST-001** (View My Profile)
- **UC-CUST-025** (Update Wishlist Item) extends **UC-CUST-023** (View Wishlist)
- **UC-CUST-026** (Remove From Wishlist) extends **UC-CUST-023**
- **UC-CUST-027** (Bulk Remove Wishlist) extends **UC-CUST-023**
- **UC-CUST-028** (Checkout From Wishlist) extends **UC-CUST-023**
- **UC-CUST-016** (Refund Ticket) extends **UC-CUST-014** (View Ticket Details)
- **UC-CUST-015** (Check In Ticket) extends **UC-CUST-014**
- **UC-CUST-017** (Validate Ticket) extends **UC-CUST-014**
---

# 3️⃣ HOST USE CASES

## Actor: Host (extends Customer)

### Event Creation
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-HOST-001** | Create Event | Tạo sự kiện mới | Đã đăng nhập, role = Host | Event được tạo, status = "Draft" |
| **UC-HOST-002** | Add Event Information | Thêm thông tin cơ bản | Event đã được tạo (Draft) | Thông tin được lưu |
| **UC-HOST-003** | Configure Tickets | Cấu hình loại vé và giá | Event đã có thông tin cơ bản | Ticket types được tạo |
| **UC-HOST-004** | Design Virtual Stage | Thiết kế sân khấu ảo 2D | Event đã có thông tin | Venue layout được lưu |
| **UC-HOST-005** | Set Payment Info | Cấu hình thông tin thanh toán | Event đã có tickets | Payment info được lưu |
| **UC-HOST-006** | Publish Event | Kích hoạt sự kiện | Tất cả 5 bước đã hoàn thành | Status "Draft" → "Open" |
| **UC-HOST-007** | Create Complete Event | Tạo sự kiện hoàn chỉnh trong 1 lần | Tất cả thông tin đã có | Event status = "Open" |

### Event Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-HOST-008** | View My Events | Xem danh sách sự kiện của mình | Đã đăng nhập, role = Host | Hiển thị events của host |
| **UC-HOST-009** | Check Edit Permission | Kiểm tra quyền chỉnh sửa event | Event tồn tại, là owner | Trả về canEdit, hasTicketsSold |
| **UC-HOST-010** | Edit Event | Chỉnh sửa sự kiện | Event chưa có vé bán, là owner | Thông tin được cập nhật |
| **UC-HOST-011** | Delete Event | Xóa sự kiện | Event chưa có vé Paid | Event bị xóa |
| **UC-HOST-012** | Check Creation Status | Kiểm tra tiến độ tạo event | Event tồn tại | Trả về completion status |

### Event Media
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-HOST-013** | Upload Event Image | Tải ảnh cho sự kiện | Event tồn tại, là owner | Ảnh được lưu |
| **UC-HOST-014** | Cleanup Unused Images | Dọn dẹp ảnh không dùng | Event tồn tại | Ảnh không dùng được xóa |

**Total: 14 use cases (Host-specific)** + 42 use cases from Customer = **56 use cases**

### Include Relationships (Host-specific)
- **UC-HOST-001** includes **UC-CUST-001** (Phải có profile để tạo event)
- **UC-HOST-002** includes **UC-HOST-001** (Phải tạo event trước)
- **UC-HOST-003** includes **UC-HOST-002** (Phải có info trước)
- **UC-HOST-004** includes **UC-HOST-002** (Phải có info trước) - **OPTIONAL**
- **UC-HOST-005** includes **UC-HOST-003** (Phải có tickets trước)
- **UC-HOST-006** includes **UC-HOST-002**, **UC-HOST-003**, **UC-HOST-005** (Phải hoàn thành)
- **UC-HOST-007** includes **UC-HOST-001** (Tạo complete event)
- **UC-HOST-010** includes **UC-HOST-009** (Phải kiểm tra quyền trước)
- **UC-HOST-011** includes **UC-HOST-009** (Phải kiểm tra quyền trước)
- **UC-HOST-013** includes **UC-HOST-008** (Phải có event để upload)

### Extend Relationships (Host-specific)
- **UC-HOST-004** extends **UC-HOST-006** (Virtual Stage là optional)
- **UC-HOST-010** extends **UC-HOST-008** (Edit là optional action)
- **UC-HOST-011** extends **UC-HOST-008** (Delete là optional action)
- **UC-HOST-014** extends **UC-HOST-008** (Cleanup là optional action)



---

# 4️⃣ ADMIN USE CASES

## Actor: Admin (extends Customer, extends Host)

### User Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-ADMIN-001** | View All Users | Xem danh sách tất cả users | Đã đăng nhập, role = Admin | Hiển thị users với filter |
| **UC-ADMIN-002** | View User Details | Xem chi tiết user | User tồn tại | Hiển thị đầy đủ thông tin |
| **UC-ADMIN-003** | Search Users | Tìm kiếm users | - | Trả về kết quả tìm kiếm |
| **UC-ADMIN-004** | Filter Users By Role | Lọc users theo role | - | Trả về users theo role |
| **UC-ADMIN-005** | View All Hosts | Xem danh sách hosts | Đã đăng nhập, role = Admin | Hiển thị hosts với filter |
| **UC-ADMIN-006** | View All Customers | Xem danh sách customers | Đã đăng nhập, role = Admin | Hiển thị customers với filter |

### Ban Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-ADMIN-007** | Ban User | Cấm tài khoản user | User tồn tại, không phải Admin | User bị cấm, không thể login |
| **UC-ADMIN-008** | Unban User | Bỏ cấm tài khoản | User bị cấm | User có thể login lại |

### Statistics
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-ADMIN-009** | View Statistics | Xem thống kê tổng quan hệ thống | Đã đăng nhập, role = Admin | Hiển thị statistics dashboard |

### Voucher Management
| ID | Use Case Name | Description | Precondition | Postcondition |
|---|---|---|---|---|
| **UC-ADMIN-010** | Create Voucher | Tạo mã giảm giá mới | Đã đăng nhập, role = Admin | Voucher được tạo |
| **UC-ADMIN-011** | View All Vouchers | Xem tất cả vouchers | Đã đăng nhập, role = Admin | Hiển thị danh sách vouchers |

**Total: 11 use cases (Admin-specific)** + 56 use cases from Host = **67 use cases**

### Include Relationships (Admin-specific)
- **UC-ADMIN-001** includes **UC-CUST-001** (Phải có profile)
- **UC-ADMIN-002** includes **UC-ADMIN-001** (Phải duyệt danh sách trước)
- **UC-ADMIN-003** extends **UC-ADMIN-001** (Search là form nâng cao)
- **UC-ADMIN-004** extends **UC-ADMIN-001** (Filter là form nâng cao)
- **UC-ADMIN-005** includes **UC-ADMIN-001** (Là subset của View All Users)
- **UC-ADMIN-006** includes **UC-ADMIN-001** (Là subset của View All Users)
- **UC-ADMIN-007** includes **UC-ADMIN-002** (Phải xem user trước)
- **UC-ADMIN-008** includes **UC-ADMIN-002** (Phải xem user trước)
- **UC-ADMIN-009** includes **UC-CUST-001** (Phải có profile)

### Extend Relationships (Admin-specific)
- **UC-ADMIN-003** extends **UC-ADMIN-001** (Search là optional)
- **UC-ADMIN-004** extends **UC-ADMIN-001** (Filter là optional)
- **UC-ADMIN-007** extends **UC-ADMIN-002** (Ban là optional action)
- **UC-ADMIN-008** extends **UC-ADMIN-002** (Unban là optional action)

---

# 🔗 ACTOR GENERALIZATION RELATIONSHIPS

```
Customer extends Guest
Host extends Customer
Admin extends Customer
```

**Diagram:**
```
Guest
 ↑
Customer
 ↑
Host ───┐
       ├──→ Admin
       └──→ (implicit inherit from Customer)
```

**Explanation:**
- **Guest**: Chỉ có thể đăng ký, đăng nhập, xem events
- **Customer**: Kế thừa tất cả từ Guest + có thể mua vé, quản lý wallet, wishlist
- **Host**: Kế thừa tất cả từ Customer + có thể tạo và quản lý events
- **Admin**: Kế thừa tất cả từ Customer + có thể quản trị users, ban/unban, xem statistics

---

# 📊 SUMMARY STATISTICS

| Role | Direct Use Cases | Inherited | Total |
|---|---|---|---|
| **Guest** | 4 | 0 | **4** |
| **Customer** | 38 | 4 | **42** |
| **Host** | 14 | 42 | **56** |
| **Admin** | 11 | 56 | **67** |

---

# 🎯 BUSINESS RULES

1. **Event Edit Constraint**: Host chỉ edit được event khi chưa có vé bán (UC-HOST-009, UC-HOST-010)
2. **Time/Location Edit**: Chỉ edit được time/location trước 48h tính từ StartTime (UC-HOST-009)
3. **Event Delete**: Không xóa được event đã có vé Paid (UC-HOST-011)
4. **Order Cancel**: Chỉ hủy được order status = "Pending" (UC-CUST-010)
5. **Ticket Check-in**: Chỉ check-in được khi status = "Assigned" (UC-CUST-015)
6. **Ticket Refund**: Chỉ refund được khi status = "Assigned" (UC-CUST-016)
7. **Wallet Balance**: Số dư không được âm (UC-CUST-018, UC-CUST-022)
8. **Ban Constraint**: Không ban được Admin (UC-ADMIN-007)
9. **Notification Types**: Có 7 loại thông báo: EventReminder, EventUpdate, PaymentSuccess, Refund, OrderConfirmation, EventCancelled, NewMessage (UC-CUST-031-042)
10. **Notification Privacy**: User chỉ xem được notification của chính mình (UC-CUST-031-035)

---

# 📝 NAMING CONVENTIONS

- **Use Case Names**: "Verb + Noun" format (e.g., "Create Order", "View Profile")
- **ID Format**: UC-{ROLE}-{NUMBER} (e.g., UC-CUST-001, UC-HOST-005)
- **Role Prefixes**:
  - GUEST: Guest (anonymous user)
  - CUST: Customer
  - HOST: Host
  - ADMIN: Admin

---

# 📈 USE CASE FLOW SUMMARY

## Typical Customer Journey
```
Guest → Register (UC-GUEST-001) → Login (UC-GUEST-002)
  ↓
Customer → Browse Events (UC-GUEST-003) → View Details (UC-GUEST-004)
  ↓
  → Create Order (UC-CUST-006) → Apply Voucher (UC-CUST-007) [Optional]
  ↓
  → Receive Order Confirmation (UC-CUST-040) → Process Payment (UC-CUST-011)
  ↓
  → Receive Payment Success (UC-CUST-041) → View Tickets (UC-CUST-013)
  ↓
  → Receive Event Reminder (UC-CUST-037) → Check In (UC-CUST-015) 
  OR Receive Refund Notification (UC-CUST-042) [Optional]
```

## Typical Host Journey
```
Customer → Become Host (when create first event)
  ↓
Host → Create Event (UC-HOST-001) → Add Info (UC-HOST-002)
  ↓
  → Configure Tickets (UC-HOST-003) → Design Stage (UC-HOST-004) [Optional]
  ↓
  → Set Payment (UC-HOST-005) → Publish (UC-HOST-006)
  ↓
  → View My Events (UC-HOST-008) → Edit/Delete (UC-HOST-010, UC-HOST-011)
```

## Typical Admin Journey
```
Admin → View All Users (UC-ADMIN-001) → View Details (UC-ADMIN-002)
  ↓
  → Ban/Unban User (UC-ADMIN-007, UC-ADMIN-008)
  ↓
  → View Statistics (UC-ADMIN-009)
  ↓
  → Create Voucher (UC-ADMIN-010) → View All Vouchers (UC-ADMIN-011)
```
