# Class Diagram - Payment Flow

## Mô tả tổng quan
Sơ đồ này minh họa kiến trúc của luồng thanh toán (Payment Flow) - luồng xử lý các giao dịch thanh toán trong hệ thống quản lý sự kiện. Luồng bao gồm việc xử lý thanh toán, quản lý ví điện tử, xử lý hoàn tiền và tracking các giao dịch.

### Các thành phần chính:
- **Controller Layer**: PaymentController xử lý HTTP requests
- **Service Layer**: PaymentService, WalletService, RefundService chứa business logic
- **Repository Layer**: PaymentRepository, WalletRepository, UserRepository xử lý data access
- **Mapper Layer**: PaymentMapper chuyển đổi giữa entities và DTOs
- **Entity Models**: Payment, WalletTransaction, Refund, Order, User, Wallet
- **DTOs**: Request/Response DTOs cho API communication

### Validation Rules chính:
- **Payment Amount Validation**: Amount phải > 0 và hợp lệ
- **Payment Method Validation**: Payment method phải được hỗ trợ
- **Order Status Validation**: Order phải có status = "Pending" để thanh toán
- **Wallet Balance Validation**: Kiểm tra số dư ví đủ để thanh toán
- **Payment Status Validation**: Chỉ cho phép các status hợp lệ
- **Authorization Validation**: Chỉ owner hoặc admin mới có quyền truy cập
- **Refund Validation**: Chỉ cho phép refund payments đã thành công

## Class Diagram

```mermaid
classDiagram
    %% Request DTOs
    class ProcessPaymentRequestDTO {
        +int OrderId
        +string PaymentMethod
        +decimal Amount
        +string? TransactionId
        +string? Description
    }

    class RefundRequestDTO {
        +int PaymentId
        +decimal RefundAmount
        +string Reason
        +string? Description
    }

    class WalletDepositRequestDTO {
        +decimal Amount
        +string PaymentMethod
        +string? TransactionId
        +string? Description
    }

    class WalletWithdrawRequestDTO {
        +decimal Amount
        +string? Description
    }

    %% Response DTOs
    class PaymentResponseDTO {
        +int PaymentId
        +int OrderId
        +decimal Amount
        +string PaymentMethod
        +string Status
        +DateTime CreatedAt
        +string TransactionId
        +string Message
    }

    class RefundResponseDTO {
        +int RefundId
        +int PaymentId
        +decimal RefundAmount
        +string Status
        +DateTime CreatedAt
        +string Reason
        +string Message
    }

    class WalletTransactionDTO {
        +int TransactionId
        +string TransactionType
        +decimal Amount
        +string Status
        +string? Description
        +string? ReferenceId
        +DateTime CreatedAt
        +decimal BalanceBefore
        +decimal BalanceAfter
    }

    class WalletBalanceDTO {
        +decimal CurrentBalance
        +decimal AvailableBalance
        +decimal PendingBalance
        +DateTime LastUpdated
    }

    %% Controller Layer
    class PaymentController {
        -IPaymentService _paymentService
        -IWalletService _walletService
        -IRefundService _refundService
        +ProcessPayment(ProcessPaymentRequestDTO): IActionResult
        +GetPaymentById(int): IActionResult
        +GetPaymentsByOrderId(int): IActionResult
        +GetUserPayments(int): IActionResult
        +GetMyPayments(): IActionResult
        +ProcessRefund(RefundRequestDTO): IActionResult
        +GetRefundById(int): IActionResult
        +GetRefundsByPaymentId(int): IActionResult
        +GetWalletBalance(): IActionResult
        +GetWalletTransactions(int, int, int): IActionResult
        +DepositToWallet(WalletDepositRequestDTO): IActionResult
        +WithdrawFromWallet(WalletWithdrawRequestDTO): IActionResult
        -GetUserIdFromToken(): int?
        -IsValidPaymentRequest(ProcessPaymentRequestDTO): bool
    }

    %% Service Layer Interfaces
    class IPaymentService {
        <<interface>>
        +ProcessPaymentAsync(ProcessPaymentRequestDTO, int): PaymentResponseDTO
        +GetPaymentByIdAsync(int): PaymentDTO?
        +GetPaymentsByOrderIdAsync(int): List~PaymentDTO~
        +GetUserPaymentsAsync(int): List~PaymentDTO~
        +GetPaymentModelByIdAsync(int): Payment?
        +UpdatePaymentStatusAsync(int, string): bool
        +ValidatePaymentRequestAsync(ProcessPaymentRequestDTO): bool
        +MapToPaymentDto(Payment): PaymentDTO
        +MapFromProcessPaymentRequest(ProcessPaymentRequestDTO, int): Payment
    }

    class IWalletService {
        <<interface>>
        +GetWalletBalanceAsync(int): decimal
        +GetWalletBalanceDtoAsync(int): WalletBalanceDTO
        +UpdateWalletBalanceAsync(int, decimal): bool
        +CreateTransactionAsync(WalletTransaction): WalletTransaction
        +GetTransactionByIdAsync(int): WalletTransaction?
        +GetUserTransactionsAsync(int, int, int): List~WalletTransaction~
        +DepositAsync(int, decimal, string?, string?): WalletTransaction
        +WithdrawAsync(int, decimal, string?, string?): WalletTransaction
        +ProcessPaymentAsync(int, decimal, int, string?): WalletTransaction
        +ProcessRefundAsync(int, decimal, int, string?): WalletTransaction
        +HasSufficientBalanceAsync(int, decimal): bool
        +ValidateTransactionAsync(WalletTransaction): bool
        +GetWalletTransactionsAsync(int, int, int): List~WalletTransactionDTO~
    }

    class IRefundService {
        <<interface>>
        +ProcessRefundAsync(RefundRequestDTO, int): RefundResponseDTO
        +GetRefundByIdAsync(int): RefundDTO?
        +GetRefundsByPaymentIdAsync(int): List~RefundDTO~
        +GetUserRefundsAsync(int): List~RefundDTO~
        +GetRefundModelByIdAsync(int): Refund?
        +UpdateRefundStatusAsync(int, string): bool
        +ValidateRefundRequestAsync(RefundRequestDTO): bool
        +MapToRefundDto(Refund): RefundDTO
        +MapFromRefundRequest(RefundRequestDTO, int): Refund
    }

    %% Service Layer Implementations
    class PaymentService {
        -IPaymentRepository _paymentRepository
        -IPaymentMapper _paymentMapper
        -IWalletService _walletService
        -IOrderService _orderService
        -EventDBContext _context
        +ProcessPaymentAsync(ProcessPaymentRequestDTO, int): PaymentResponseDTO
        +GetPaymentByIdAsync(int): PaymentDTO?
        +GetPaymentsByOrderIdAsync(int): List~PaymentDTO~
        +GetUserPaymentsAsync(int): List~PaymentDTO~
        +GetPaymentModelByIdAsync(int): Payment?
        +UpdatePaymentStatusAsync(int, string): bool
        +ValidatePaymentRequestAsync(ProcessPaymentRequestDTO): bool
        +MapToPaymentDto(Payment): PaymentDTO
        +MapFromProcessPaymentRequest(ProcessPaymentRequestDTO, int): Payment
        -ValidateOrderForPaymentAsync(int): bool
        -ProcessWalletPaymentAsync(int, decimal, int): WalletTransaction
    }

    class WalletService {
        -EventDBContext _context
        -IUserRepository _userRepository
        -IWalletRepository _walletRepository
        +GetWalletBalanceAsync(int): decimal
        +GetWalletBalanceDtoAsync(int): WalletBalanceDTO
        +UpdateWalletBalanceAsync(int, decimal): bool
        +CreateTransactionAsync(WalletTransaction): WalletTransaction
        +GetTransactionByIdAsync(int): WalletTransaction?
        +GetUserTransactionsAsync(int, int, int): List~WalletTransaction~
        +DepositAsync(int, decimal, string?, string?): WalletTransaction
        +WithdrawAsync(int, decimal, string?, string?): WalletTransaction
        +ProcessPaymentAsync(int, decimal, int, string?): WalletTransaction
        +ProcessRefundAsync(int, decimal, int, string?): WalletTransaction
        +HasSufficientBalanceAsync(int, decimal): bool
        +ValidateTransactionAsync(WalletTransaction): bool
        +GetWalletTransactionsAsync(int, int, int): List~WalletTransactionDTO~
        -CalculateNewBalanceAsync(int, decimal, string): decimal
        -CreateWalletTransactionAsync(int, string, decimal, string?, string?): WalletTransaction
    }

    class RefundService {
        -IRefundRepository _refundRepository
        -IRefundMapper _refundMapper
        -IPaymentService _paymentService
        -IWalletService _walletService
        -EventDBContext _context
        +ProcessRefundAsync(RefundRequestDTO, int): RefundResponseDTO
        +GetRefundByIdAsync(int): RefundDTO?
        +GetRefundsByPaymentIdAsync(int): List~RefundDTO~
        +GetUserRefundsAsync(int): List~RefundDTO~
        +GetRefundModelByIdAsync(int): Refund?
        +UpdateRefundStatusAsync(int, string): bool
        +ValidateRefundRequestAsync(RefundRequestDTO): bool
        +MapToRefundDto(Refund): RefundDTO
        +MapFromRefundRequest(RefundRequestDTO, int): Refund
        -ValidatePaymentForRefundAsync(int): bool
        -ProcessWalletRefundAsync(int, decimal, int): WalletTransaction
    }

    %% Repository Layer Interfaces
    class IPaymentRepository {
        <<interface>>
        +CreatePaymentAsync(Payment): Payment
        +GetPaymentByIdAsync(int): Payment?
        +GetPaymentsByOrderIdAsync(int): List~Payment~
        +GetUserPaymentsAsync(int): List~Payment~
        +UpdatePaymentStatusAsync(int, string): bool
        +UpdatePaymentAsync(int, Payment): Payment?
        +DeletePaymentAsync(int): bool
    }

    class IWalletRepository {
        <<interface>>
        +GetWalletByUserIdAsync(int): Wallet?
        +CreateWalletAsync(Wallet): Wallet
        +UpdateWalletBalanceAsync(int, decimal): bool
        +GetWalletTransactionsAsync(int, int, int): List~WalletTransaction~
        +CreateTransactionAsync(WalletTransaction): WalletTransaction
        +GetTransactionByIdAsync(int): WalletTransaction?
    }

    class IRefundRepository {
        <<interface>>
        +CreateRefundAsync(Refund): Refund
        +GetRefundByIdAsync(int): Refund?
        +GetRefundsByPaymentIdAsync(int): List~Refund~
        +GetUserRefundsAsync(int): List~Refund~
        +UpdateRefundStatusAsync(int, string): bool
        +UpdateRefundAsync(int, Refund): Refund?
        +DeleteRefundAsync(int): bool
    }

    class IUserRepository {
        <<interface>>
        +GetUserByIdAsync(int): User?
        +UpdateUserAsync(User): bool
    }

    %% Repository Layer Implementations
    class PaymentRepository {
        -EventDBContext _context
        +CreatePaymentAsync(Payment): Payment
        +GetPaymentByIdAsync(int): Payment?
        +GetPaymentsByOrderIdAsync(int): List~Payment~
        +GetUserPaymentsAsync(int): List~Payment~
        +UpdatePaymentStatusAsync(int, string): bool
        +UpdatePaymentAsync(int, Payment): Payment?
        +DeletePaymentAsync(int): bool
    }

    class WalletRepository {
        -EventDBContext _context
        +GetWalletByUserIdAsync(int): Wallet?
        +CreateWalletAsync(Wallet): Wallet
        +UpdateWalletBalanceAsync(int, decimal): bool
        +GetWalletTransactionsAsync(int, int, int): List~WalletTransaction~
        +CreateTransactionAsync(WalletTransaction): WalletTransaction
        +GetTransactionByIdAsync(int): WalletTransaction?
    }

    class RefundRepository {
        -EventDBContext _context
        +CreateRefundAsync(Refund): Refund
        +GetRefundByIdAsync(int): Refund?
        +GetRefundsByPaymentIdAsync(int): List~Refund~
        +GetUserRefundsAsync(int): List~Refund~
        +UpdateRefundStatusAsync(int, string): bool
        +UpdateRefundAsync(int, Refund): Refund?
        +DeleteRefundAsync(int): bool
    }

    class UserRepository {
        -EventDBContext _context
        +GetUserByIdAsync(int): User?
        +UpdateUserAsync(User): bool
    }

    %% Mapper Layer
    class IPaymentMapper {
        <<interface>>
        +MapToPaymentDto(Payment): PaymentDTO
        +MapToPaymentDetailDto(Payment): PaymentDetailDto
        +MapToProcessPaymentResponse(Payment): PaymentResponseDTO
        +MapFromProcessPaymentRequest(ProcessPaymentRequestDTO, int): Payment
    }

    class PaymentMapper {
        +MapToPaymentDto(Payment): PaymentDTO
        +MapToPaymentDetailDto(Payment): PaymentDetailDto
        +MapToProcessPaymentResponse(Payment): PaymentResponseDTO
        +MapFromProcessPaymentRequest(ProcessPaymentRequestDTO, int): Payment
    }

    class IRefundMapper {
        <<interface>>
        +MapToRefundDto(Refund): RefundDTO
        +MapToRefundDetailDto(Refund): RefundDetailDto
        +MapToRefundResponse(Refund): RefundResponseDTO
        +MapFromRefundRequest(RefundRequestDTO, int): Refund
    }

    class RefundMapper {
        +MapToRefundDto(Refund): RefundDTO
        +MapToRefundDetailDto(Refund): RefundDetailDto
        +MapToRefundResponse(Refund): RefundResponseDTO
        +MapFromRefundRequest(RefundRequestDTO, int): Refund
    }

    %% Data Context
    class EventDBContext {
        +DbSet~Payment~ Payments
        +DbSet~Refund~ Refunds
        +DbSet~Wallet~ Wallets
        +DbSet~WalletTransaction~ WalletTransactions
        +DbSet~Order~ Orders
        +DbSet~User~ Users
    }

    %% Entity Models
    class Payment {
        +int PaymentId
        +int OrderId
        +int UserId
        +decimal Amount
        +string PaymentMethod
        +string Status
        +string? TransactionId
        +string? Description
        +DateTime CreatedAt
        +DateTime? CompletedAt
        +DateTime? FailedAt
        +Order Order
        +User User
        +List~Refund~ Refunds
    }

    class Refund {
        +int RefundId
        +int PaymentId
        +int UserId
        +decimal RefundAmount
        +string Status
        +string Reason
        +string? Description
        +DateTime CreatedAt
        +DateTime? ProcessedAt
        +DateTime? FailedAt
        +Payment Payment
        +User User
    }

    class Wallet {
        +int WalletId
        +int UserId
        +decimal Balance
        +decimal AvailableBalance
        +decimal PendingBalance
        +DateTime CreatedAt
        +DateTime LastUpdated
        +User User
        +List~WalletTransaction~ Transactions
    }

    class WalletTransaction {
        +int TransactionId
        +int UserId
        +int? WalletId
        +string TransactionType
        +decimal Amount
        +string Status
        +string? Description
        +string? ReferenceId
        +DateTime CreatedAt
        +DateTime? CompletedAt
        +decimal BalanceBefore
        +decimal BalanceAfter
        +User User
        +Wallet? Wallet
    }

    class Order {
        +int OrderId
        +int CustomerId
        +decimal Amount
        +string Status
        +string PaymentMethod
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +User Customer
        +List~Payment~ Payments
    }

    class User {
        +int UserId
        +string Username
        +string FullName
        +string Email
        +string PasswordHash
        +string Phone
        +string Role
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +string? Avatar
        +DateTime? DateOfBirth
        +string? Gender
        +Wallet? Wallet
        +List~Payment~ Payments
        +List~Refund~ Refunds
        +List~WalletTransaction~ WalletTransactions
    }

    %% Response DTOs
    class PaymentDTO {
        +int PaymentId
        +int OrderId
        +decimal Amount
        +string PaymentMethod
        +string Status
        +DateTime CreatedAt
        +string TransactionId
    }

    class PaymentDetailDto {
        +int PaymentId
        +int OrderId
        +int UserId
        +decimal Amount
        +string PaymentMethod
        +string Status
        +string? TransactionId
        +string? Description
        +DateTime CreatedAt
        +DateTime? CompletedAt
        +DateTime? FailedAt
        +List~RefundDTO~ Refunds
    }

    class RefundDTO {
        +int RefundId
        +int PaymentId
        +decimal RefundAmount
        +string Status
        +string Reason
        +DateTime CreatedAt
        +string? Description
    }

    class RefundDetailDto {
        +int RefundId
        +int PaymentId
        +int UserId
        +decimal RefundAmount
        +string Status
        +string Reason
        +string? Description
        +DateTime CreatedAt
        +DateTime? ProcessedAt
        +DateTime? FailedAt
    }

    %% Relationships
    PaymentController --> IPaymentService : "1. calls"
    PaymentController --> IWalletService : "2. calls"
    PaymentController --> IRefundService : "3. calls"
    
    PaymentService ..|> IPaymentService : "implements"
    PaymentService --> IPaymentRepository : "4. queries"
    PaymentService --> IPaymentMapper : "5. uses"
    PaymentService --> IWalletService : "6. uses"
    PaymentService --> IOrderService : "7. uses"
    
    WalletService ..|> IWalletService : "implements"
    WalletService --> IUserRepository : "8. queries"
    WalletService --> IWalletRepository : "9. queries"
    
    RefundService ..|> IRefundService : "implements"
    RefundService --> IRefundRepository : "10. queries"
    RefundService --> IRefundMapper : "11. uses"
    RefundService --> IPaymentService : "12. uses"
    RefundService --> IWalletService : "13. uses"
    
    PaymentRepository ..|> IPaymentRepository : "implements"
    PaymentRepository --> EventDBContext : "14. uses"
    
    WalletRepository ..|> IWalletRepository : "implements"
    WalletRepository --> EventDBContext : "15. uses"
    
    RefundRepository ..|> IRefundRepository : "implements"
    RefundRepository --> EventDBContext : "16. uses"
    
    UserRepository ..|> IUserRepository : "implements"
    UserRepository --> EventDBContext : "17. uses"
    
    PaymentMapper ..|> IPaymentMapper : "implements"
    RefundMapper ..|> IRefundMapper : "implements"
    
    EventDBContext --> Payment : "18. manages"
    EventDBContext --> Refund : "19. manages"
    EventDBContext --> Wallet : "20. manages"
    EventDBContext --> WalletTransaction : "21. manages"
    EventDBContext --> Order : "22. manages"
    EventDBContext --> User : "23. manages"
    
    Payment --> Order : "24. belongs to"
    Payment --> User : "25. belongs to"
    Payment --> Refund : "26. has many"
    
    Refund --> Payment : "27. belongs to"
    Refund --> User : "28. belongs to"
    
    Wallet --> User : "29. belongs to"
    Wallet --> WalletTransaction : "30. has many"
    
    WalletTransaction --> User : "31. belongs to"
    WalletTransaction --> Wallet : "32. belongs to (optional)"
    
    User --> Wallet : "33. has one (optional)"
    User --> Payment : "34. has many"
    User --> Refund : "35. has many"
    User --> WalletTransaction : "36. has many"
    
    PaymentMapper --> PaymentDTO : "37. maps to"
    PaymentMapper --> PaymentResponseDTO : "38. maps to"
    PaymentMapper --> PaymentDetailDto : "39. maps to"
    
    RefundMapper --> RefundDTO : "40. maps to"
    RefundMapper --> RefundResponseDTO : "41. maps to"
    RefundMapper --> RefundDetailDto : "42. maps to"
    
    ProcessPaymentRequestDTO --> PaymentController : "43. receives"
    PaymentResponseDTO --> PaymentController : "44. returns"
    RefundRequestDTO --> PaymentController : "45. receives"
    RefundResponseDTO --> PaymentController : "46. returns"
    WalletDepositRequestDTO --> PaymentController : "47. receives"
    WalletWithdrawRequestDTO --> PaymentController : "48. receives"
    WalletBalanceDTO --> PaymentController : "49. returns"
    WalletTransactionDTO --> PaymentController : "50. returns"
```

## Luồng hoạt động chính (Payment Process)

### 1. Xử lý thanh toán (ProcessPaymentAsync)
1. **PaymentController** nhận `ProcessPaymentRequestDTO`
2. **PaymentController** gọi `IPaymentService.ProcessPaymentAsync()`
3. **PaymentService** thực hiện business validation:
   - Kiểm tra Order status = "Pending"
   - Kiểm tra Payment amount hợp lệ
   - Kiểm tra Payment method được hỗ trợ
4. **PaymentService** gọi `IWalletService.HasSufficientBalanceAsync()` để kiểm tra số dư
5. Nếu đủ số dư, **PaymentService** gọi `IWalletService.ProcessPaymentAsync()`
6. **WalletService** tạo `WalletTransaction` và cập nhật balance
7. **PaymentService** tạo `Payment` entity với status "Completed"
8. **PaymentService** gọi `IOrderService.UpdateOrderStatusAsync()` với status "Paid"
9. **PaymentMapper** map Payment entity thành `PaymentResponseDTO`
10. Trả về response cho client

### 2. Xử lý hoàn tiền (ProcessRefundAsync)
1. **PaymentController** nhận `RefundRequestDTO`
2. **PaymentController** gọi `IRefundService.ProcessRefundAsync()`
3. **RefundService** thực hiện business validation:
   - Kiểm tra Payment status = "Completed"
   - Kiểm tra Refund amount <= Payment amount
   - Kiểm tra Payment chưa được refund hoàn toàn
4. **RefundService** gọi `IWalletService.ProcessRefundAsync()`
5. **WalletService** tạo `WalletTransaction` với type "Refund" và cập nhật balance
6. **RefundService** tạo `Refund` entity với status "Processed"
7. **RefundMapper** map Refund entity thành `RefundResponseDTO`
8. Trả về response cho client

### 3. Quản lý ví điện tử (Wallet Management)
- **PaymentController.GetWalletBalance()**: Lấy số dư ví hiện tại
- **PaymentController.GetWalletTransactions()**: Lấy lịch sử giao dịch ví
- **PaymentController.DepositToWallet()**: Nạp tiền vào ví
- **PaymentController.WithdrawFromWallet()**: Rút tiền từ ví

### 4. Theo dõi giao dịch (Transaction Tracking)
- **WalletService** tạo `WalletTransaction` cho mỗi giao dịch với:
  - TransactionType: "Deposit", "Withdraw", "Payment", "Refund"
  - BalanceBefore/BalanceAfter để tracking
  - ReferenceId để liên kết với Order/Payment
- **PaymentService** tạo `Payment` entity với status tracking
- **RefundService** tạo `Refund` entity với reason và status

### 5. Error Handling & Validation
- **PaymentService**: Comprehensive validation với ArgumentException cho business rules
- **Transaction Rollback**: Tất cả operations đều có try-catch với transaction rollback
- **Data Annotations**: Input validation với Data Annotations trong DTOs và Models
- **Authorization Checks**: Kiểm tra quyền truy cập ở controller level
- **Concurrent Access**: Database transactions đảm bảo data consistency

## Business Rules quan trọng

1. **Transaction Safety**: Tất cả operations đều sử dụng database transaction
2. **Payment Validation**: Chỉ có thể thanh toán orders đang ở status "Pending"
3. **Refund Validation**: Chỉ có thể refund payments đã thành công
4. **Wallet Balance**: Kiểm tra số dư đủ trước khi xử lý thanh toán
5. **Status Flow**: Payment status flow: Pending → Completed → Refunded (hoặc Failed)
6. **Wallet Integration**: Tất cả giao dịch đều được ghi lại trong `WalletTransaction`
7. **Authorization**: Chỉ owner hoặc admin mới có thể xem/cập nhật payments
8. **Refund Limits**: Refund amount không được vượt quá payment amount
9. **Transaction Tracking**: Mỗi giao dịch ví đều được ghi lại với BalanceBefore/BalanceAfter
10. **Error Handling**: Tất cả operations đều có try-catch với transaction rollback
11. **Data Validation**: Input validation với Data Annotations và custom business rules
12. **Concurrent Access**: Database transactions đảm bảo data consistency trong môi trường concurrent

## Các điểm phức tạp trong luồng

1. **Multi-step validation**: Nhiều business rules phải được validate
2. **Transaction management**: Cần đảm bảo data consistency
3. **Wallet integration**: Integration với wallet system
4. **Refund processing**: Xử lý hoàn tiền với validation phức tạp
5. **Balance tracking**: Real-time tracking wallet balance
6. **Error handling**: Comprehensive error handling với transaction rollback
7. **Concurrent access**: Xử lý concurrent requests để tránh race conditions
8. **Authorization**: Kiểm tra quyền truy cập ở nhiều levels
9. **Data mapping**: Complex mapping giữa entities và DTOs
10. **Transaction history**: Tracking đầy đủ lịch sử giao dịch
