# Class Diagram - TheGrind5 Event Management Backend

## Mô tả
Class diagram này thể hiện toàn bộ cấu trúc các lớp trong hệ thống quản lý sự kiện TheGrind5 Backend.

## Class Diagram

```mermaid
classDiagram
    class User {
        +int UserId
        +string Username
        +string FullName
        +string Email
        +string PasswordHash
        +string Phone
        +string Role
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +decimal WalletBalance
    }

    class Event {
        +int EventId
        +int HostId
        +string Title
        +string Description
        +DateTime StartTime
        +DateTime EndTime
        +string Location
        +string Category
        +string Status
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class TicketType {
        +int TicketTypeId
        +int EventId
        +string TypeName
        +decimal Price
        +int Quantity
        +int MinOrder
        +int MaxOrder
        +DateTime SaleStart
        +DateTime SaleEnd
        +string Status
    }

    class Ticket {
        +int TicketId
        +int TicketTypeId
        +int OrderItemId
        +string SerialNumber
        +string Status
        +DateTime IssuedAt
        +DateTime UsedAt
        +DateTime RefundedAt
    }

    class Order {
        +int OrderId
        +int CustomerId
        +decimal Amount
        +string Status
        +string PaymentMethod
        +DateTime CreatedAt
        +DateTime UpdatedAt
    }

    class OrderItem {
        +int OrderItemId
        +int OrderId
        +int TicketTypeId
        +int Quantity
        +string SeatNo
        +string Status
    }

    class Payment {
        +int PaymentId
        +int OrderId
        +decimal Amount
        +string Method
        +string Status
        +DateTime PaymentDate
    }

    class WalletTransaction {
        +int TransactionId
        +int UserId
        +decimal Amount
        +string TransactionType
        +string Status
        +string Description
        +string ReferenceId
        +DateTime CreatedAt
        +DateTime CompletedAt
        +decimal BalanceBefore
        +decimal BalanceAfter
    }

    class Voucher {
        +int VoucherId
        +string VoucherCode
        +decimal DiscountPercentage
        +DateTime ValidFrom
        +DateTime ValidTo
        +bool IsActive
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +IsValid() bool
        +CalculateDiscountAmount() decimal
        +CalculateFinalAmount() decimal
    }

    class Wishlist {
        +int Id
        +int UserId
        +int TicketTypeId
        +int Quantity
        +DateTime AddedAt
        +DateTime UpdatedAt
    }

    class OtpCode {
        +int OtpId
        +string Code
        +string Email
        +DateTime CreatedAt
        +DateTime ExpiresAt
        +bool IsUsed
        +DateTime UsedAt
    }

    class AuthController {
        -IAuthService _authService
        -IUserRepository _userRepository
        +Login() IActionResult
        +Register() IActionResult
        +VerifyOtp() IActionResult
        +ResendOtp() IActionResult
        +ForgotPassword() IActionResult
        +ResetPassword() IActionResult
    }

    class EventController {
        -IEventService _eventService
        +GetAllEvents() IActionResult
        +GetEventById() IActionResult
        +CreateEvent() IActionResult
        +UpdateEvent() IActionResult
        +DeleteEvent() IActionResult
        +GetEventsByHost() IActionResult
    }

    class OrderController {
        -IOrderService _orderService
        -IWalletService _walletService
        +CreateOrder() IActionResult
        +GetOrderById() IActionResult
        +GetUserOrders() IActionResult
        +UpdateOrderStatus() IActionResult
        +ProcessPayment() IActionResult
        +ProcessRefund() IActionResult
    }

    class TicketController {
        -ITicketService _ticketService
        +GetTicketsByOrder() IActionResult
        +ValidateTicket() IActionResult
        +UseTicket() IActionResult
    }

    class WalletController {
        -IWalletService _walletService
        +GetBalance() IActionResult
        +GetTransactions() IActionResult
        +Deposit() IActionResult
        +Withdraw() IActionResult
    }

    class VoucherController {
        -IVoucherService _voucherService
        +GetAllVouchers() IActionResult
        +GetVoucherById() IActionResult
        +CreateVoucher() IActionResult
        +UpdateVoucher() IActionResult
        +DeleteVoucher() IActionResult
        +ValidateVoucher() IActionResult
    }

    class WishlistController {
        -IWishlistService _wishlistService
        +GetUserWishlist() IActionResult
        +AddToWishlist() IActionResult
        +RemoveFromWishlist() IActionResult
        +UpdateWishlistItem() IActionResult
    }

    class IAuthService {
        <<interface>>
        +LoginAsync() Task
        +RegisterAsync() Task
    }

    class IEventService {
        <<interface>>
        +GetAllEventsAsync() Task
        +GetEventByIdAsync() Task
        +CreateEventAsync() Task
        +UpdateEventAsync() Task
        +DeleteEventAsync() Task
        +GetEventsByHostAsync() Task
    }

    class IOrderService {
        <<interface>>
        +CreateOrderAsync() Task
        +GetOrderByIdAsync() Task
        +GetUserOrdersAsync() Task
        +UpdateOrderStatusAsync() Task
        +GetExpiredOrdersAsync() Task
        +CleanupExpiredOrdersAsync() Task
    }

    class IWalletService {
        <<interface>>
        +GetWalletBalanceAsync() Task
        +UpdateWalletBalanceAsync() Task
        +CreateTransactionAsync() Task
        +DepositAsync() Task
        +WithdrawAsync() Task
        +ProcessPaymentAsync() Task
        +ProcessRefundAsync() Task
    }

    class ITicketService {
        <<interface>>
        +GetTicketsByOrderAsync() Task
        +ValidateTicketAsync() Task
        +UseTicketAsync() Task
        +GenerateTicketsAsync() Task
        +RefundTicketAsync() Task
    }

    class IWishlistService {
        <<interface>>
        +GetUserWishlistAsync() Task
        +AddToWishlistAsync() Task
        +RemoveFromWishlistAsync() Task
        +UpdateWishlistItemAsync() Task
        +ClearWishlistAsync() Task
    }

    class IEmailService {
        <<interface>>
        +SendEmailAsync() Task
        +SendOtpEmailAsync() Task
        +SendPasswordResetEmailAsync() Task
    }

    class IOtpService {
        <<interface>>
        +GenerateOtpAsync() Task
        +VerifyOtpAsync() Task
        +IsOtpValidAsync() Task
        +CleanupExpiredOtpsAsync() Task
    }

    class IUserRepository {
        <<interface>>
        +GetByIdAsync() Task
        +GetByEmailAsync() Task
        +CreateAsync() Task
        +UpdateAsync() Task
        +DeleteAsync() Task
        +IsEmailExistsAsync() Task
        +GetAllAsync() Task
    }

    class IEventRepository {
        <<interface>>
        +GetByIdAsync() Task
        +GetAllAsync() Task
        +CreateAsync() Task
        +UpdateAsync() Task
        +DeleteAsync() Task
        +GetByHostIdAsync() Task
    }

    class IOrderRepository {
        <<interface>>
        +GetByIdAsync() Task
        +GetByUserIdAsync() Task
        +CreateAsync() Task
        +UpdateAsync() Task
        +DeleteAsync() Task
        +GetExpiredOrdersAsync() Task
    }

    class EventDBContext {
        +DbSet Users
        +DbSet Events
        +DbSet TicketTypes
        +DbSet Tickets
        +DbSet Orders
        +DbSet OrderItems
        +DbSet Payments
        +DbSet WalletTransactions
        +DbSet Vouchers
        +DbSet Wishlists
        +DbSet OtpCodes
        +OnModelCreating()
        +SaveChangesAsync() Task
    }

    User ||--o{ Event : "Hosts"
    User ||--o{ Order : "Places"
    User ||--o{ WalletTransaction : "Has"
    User ||--o{ Wishlist : "Has"
    
    Event ||--o{ TicketType : "Contains"
    
    TicketType ||--o{ OrderItem : "Ordered"
    TicketType ||--o{ Ticket : "Generates"
    TicketType ||--o{ Wishlist : "Wishlisted"
    
    Order ||--o{ OrderItem : "Contains"
    Order ||--o{ Payment : "Has"
    
    OrderItem ||--o{ Ticket : "Generates"
    
    AuthController --> IAuthService
    AuthController --> IUserRepository
    EventController --> IEventService
    OrderController --> IOrderService
    OrderController --> IWalletService
    TicketController --> ITicketService
    WalletController --> IWalletService
    VoucherController --> IVoucherService
    WishlistController --> IWishlistService
    
    IUserRepository --> EventDBContext
    IEventRepository --> EventDBContext
    IOrderRepository --> EventDBContext
```

## Mô tả các thành phần chính

### 1. Models (Entity Classes)
- **User**: Người dùng hệ thống (Customer, Host, Admin)
- **Event**: Sự kiện được tổ chức
- **TicketType**: Loại vé cho sự kiện
- **Ticket**: Vé cụ thể được tạo ra
- **Order**: Đơn hàng mua vé
- **OrderItem**: Chi tiết từng loại vé trong đơn hàng
- **Payment**: Thanh toán cho đơn hàng
- **WalletTransaction**: Giao dịch ví điện tử
- **Voucher**: Mã giảm giá
- **Wishlist**: Danh sách yêu thích
- **OtpCode**: Mã OTP xác thực

### 2. Controllers (API Endpoints)
- **AuthController**: Xác thực, đăng ký, đăng nhập
- **EventController**: Quản lý sự kiện
- **OrderController**: Quản lý đơn hàng
- **TicketController**: Quản lý vé
- **WalletController**: Quản lý ví điện tử
- **VoucherController**: Quản lý mã giảm giá
- **WishlistController**: Quản lý danh sách yêu thích

### 3. Services (Business Logic)
- **IAuthService**: Logic xác thực
- **IEventService**: Logic quản lý sự kiện
- **IOrderService**: Logic quản lý đơn hàng
- **IWalletService**: Logic quản lý ví
- **ITicketService**: Logic quản lý vé
- **IWishlistService**: Logic danh sách yêu thích
- **IEmailService**: Gửi email
- **IOtpService**: Xử lý OTP

### 4. Repositories (Data Access)
- **IUserRepository**: Truy cập dữ liệu User
- **IEventRepository**: Truy cập dữ liệu Event
- **IOrderRepository**: Truy cập dữ liệu Order

### 5. Data Context
- **EventDBContext**: Entity Framework DbContext

## Kiến trúc tổng thể
Hệ thống sử dụng kiến trúc **Clean Architecture** với các lớp:
- **Controllers**: API Layer
- **Services**: Business Logic Layer  
- **Repositories**: Data Access Layer
- **Models**: Entity Layer
- **DTOs**: Data Transfer Objects

## Mối quan hệ chính
1. **User** có thể tạo nhiều **Event** (Host)
2. **User** có thể đặt nhiều **Order** (Customer)
3. **Event** có nhiều **TicketType**
4. **TicketType** tạo ra nhiều **Ticket**
5. **Order** chứa nhiều **OrderItem**
6. **OrderItem** tạo ra nhiều **Ticket**
7. **Order** có thể có nhiều **Payment**
8. **User** có nhiều **WalletTransaction**
9. **User** có nhiều **Wishlist** items
