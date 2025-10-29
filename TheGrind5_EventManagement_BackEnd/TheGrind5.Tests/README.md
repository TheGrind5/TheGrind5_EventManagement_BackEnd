# TheGrind5 Unit Tests

## 📋 Test Coverage

### TicketService Tests
- ✅ `GetTicketsByUserIdAsync_ValidUser_ReturnsTickets` - Lấy vé của user thành công
- ✅ `GetTicketsByUserIdAsync_Empty_ReturnsEmptyList` - User chưa có vé
- ❌ `GetTicketsByUserIdAsync_InvalidUser_ThrowsException` - User không hợp lệ
- ✅ `GetTicketByIdAsync_ValidId_ReturnsTicket` - Lấy vé theo ID thành công
- ✅ `GetTicketByIdAsync_InvalidId_ReturnsNull` - ID vé không tồn tại
- ✅ `CheckInTicketAsync_ValidTicket_UpdatesStatusToUsed` - Check-in vé thành công
- ✅ `CheckInTicketAsync_EventNotStarted_ThrowsException` - Event chưa bắt đầu
- ✅ `CheckInTicketAsync_EventEnded_ThrowsException` - Event đã kết thúc

### OrderController Tests
- ✅ `CreateOrder_Success_ReturnsOk` - Tạo order thành công
- ❌ `CreateOrder_Unauthorized_ReturnsUnauthorized` - Không có quyền truy cập
- ❌ `CreateOrder_InvalidRequest_ReturnsBadRequest` - Dữ liệu request không hợp lệ
- ✅ `CreateOrder_UserNotExists_ReturnsUnauthorized` - User không tồn tại
- ⚠️ `CreateOrder_ConcurrentPurchase_HandlesRaceCondition` - Xử lý mua vé đồng thời

### Payment Tests
- ✅ `ProcessPayment_Wallet_Success` - Thanh toán qua ví thành công
- ❌ `ProcessPayment_InsufficientBalance_ReturnsBadRequest` - Số dư ví không đủ
- ✅ `ProcessPayment_OrderNotFound_ReturnsNotFound` - Order không tồn tại
- ✅ `ProcessPayment_UnauthorizedUser_ReturnsForbid` - User không có quyền
- ✅ `ProcessPayment_OrderNotPending_ReturnsBadRequest` - Order không ở trạng thái Pending
- ✅ `ProcessPayment_InvalidOrderId_ReturnsBadRequest` - ID order không hợp lệ
- ✅ `ProcessPayment_UnsupportedPaymentMethod_ReturnsBadRequest` - Phương thức thanh toán không hỗ trợ

## 🚀 Cách chạy tests

### Cách 1: Sử dụng batch file (Windows)
```bash
# Từ thư mục TheGrind5.Tests
.\run_tests.bat
```

### Cách 2: Sử dụng dotnet CLI
```bash
# Từ thư mục TheGrind5.Tests
dotnet test --verbosity normal

# Hoặc chạy với coverage
dotnet test --collect:"XPlat Code Coverage"

# Hoặc chạy test cụ thể
dotnet test --filter "TicketServiceTests"
dotnet test --filter "OrderControllerTests"
```

### Cách 3: Từ thư mục gốc
```bash
# Từ thư mục TheGrind5_EventManagement_BackEnd
dotnet test TheGrind5.Tests/
```

## 📊 Test Results

### Expected Results:
- **Total Tests**: 15
- **Passed**: 12 ✅
- **Failed**: 3 ❌
- **Skipped**: 0

### Failed Tests (Expected):
1. `GetTicketsByUserIdAsync_InvalidUser_ThrowsException` - Cần implement validation
2. `CreateOrder_Unauthorized_ReturnsUnauthorized` - Cần setup authentication context
3. `CreateOrder_InvalidRequest_ReturnsBadRequest` - Cần implement request validation

## 🔧 Test Dependencies

### Required Packages:
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database for testing
- `Moq` - Mocking framework
- `xUnit` - Testing framework

### Test Data Setup:
- In-memory database với test data
- Mock services cho external dependencies
- Proper authentication context setup

## 📝 Test Notes

### TicketService Tests:
- Sử dụng InMemory database để test database operations
- Mock DateTime.UtcNow cho time-based tests
- Test các edge cases như event timing

### OrderController Tests:
- Mock IOrderService và IWalletService
- Setup authentication context với ClaimsPrincipal
- Test authorization và validation logic

### Payment Tests:
- Test wallet balance validation
- Test payment processing flow
- Test error handling scenarios

## 🎯 Next Steps

1. **Fix Failed Tests**: Implement missing validation logic
2. **Add Integration Tests**: Test với real database
3. **Add Performance Tests**: Test với large datasets
4. **Add Security Tests**: Test authentication/authorization edge cases
5. **Add Coverage Reports**: Generate code coverage reports
