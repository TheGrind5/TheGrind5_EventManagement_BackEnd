# 📋 Thien Test Cases Log - TheGrind5 Event Management

## 📊 Tổng quan
- **Tác giả:** Thiên
- **Ngày tạo:** 25/10/2025
- **Tổng số test cases:** 21
- **Mục tiêu:** Tăng coverage cho OrderController và OrderService trong luồng Buy Ticket
- **Trạng thái:** ✅ Tất cả test cases đều PASS

---

## 🎯 Mục tiêu Coverage
- **Target:** Đạt 80% code coverage cho OrderController
- **Focus:** Luồng Buy Ticket (OrderController + OrderService)
- **Approach:** Unit testing với Mock objects và Integration testing với InMemory database

---

## 📁 File Structure
```
Thien/
├── OrderControllerCoverageTests.cs (14 test cases)
└── OrderServiceCoreTests.cs (7 test cases)
```

---

## 🧪 OrderControllerCoverageTests.cs

### 📋 Test Cases Overview
**Tổng số:** 14 test cases  
**Mục đích:** Test các API endpoints của OrderController  
**Phương pháp:** Unit testing với Mock services  

### 🔧 Setup & Configuration
```csharp
// Mock services được sử dụng
- Mock<IOrderService> _mockOrderService
- Mock<IWalletService> _mockWalletService  
- Mock<ILogger<OrderController>> _mockLogger

// Controller context setup
- Claims: UserId = 1, Email = test@example.com
- Authentication: TestAuthType
```

### 📝 Chi tiết Test Cases

#### 1. CreateOrder Tests (5 test cases)

**TC-001: CreateOrder_WithValidRequest_ShouldReturnOk**
- **Mục đích:** Test tạo order thành công với request hợp lệ
- **Input:** EventId=1, TicketTypeId=1, Quantity=2
- **Expected:** OkObjectResult với OrderId=1, TotalAmount=200.00m
- **Mock:** ValidateUserExistsAsync(true), CreateOrderAsync(response)

**TC-002: CreateOrder_WithInvalidRequest_ShouldReturnBadRequest**
- **Mục đích:** Test validation với EventId=0 (invalid)
- **Input:** EventId=0, TicketTypeId=1, Quantity=2
- **Expected:** BadRequestObjectResult
- **Mock:** ValidateUserExistsAsync(true)

**TC-003: CreateOrder_WithNullRequest_ShouldReturnBadRequest**
- **Mục đích:** Test với request null
- **Input:** null request
- **Expected:** BadRequestObjectResult
- **Mock:** ValidateUserExistsAsync(true)

**TC-004: CreateOrder_WithInvalidTicketTypeId_ShouldReturnBadRequest**
- **Mục đích:** Test với TicketTypeId không hợp lệ
- **Input:** TicketTypeId=999 (non-existent)
- **Expected:** BadRequestObjectResult
- **Mock:** ValidateUserExistsAsync(true)

**TC-005: CreateOrder_WithInvalidQuantity_ShouldReturnBadRequest**
- **Mục đích:** Test với Quantity <= 0
- **Input:** Quantity=0
- **Expected:** BadRequestObjectResult
- **Mock:** ValidateUserExistsAsync(true)

#### 2. GetOrderById Tests (1 test case)

**TC-006: GetOrderById_WithValidId_ShouldReturnOk**
- **Mục đích:** Test lấy order theo ID thành công
- **Input:** OrderId=1
- **Expected:** OkObjectResult với order details
- **Mock:** GetOrderByIdAsync(order)

#### 3. GetUserOrders Tests (1 test case)

**TC-007: GetUserOrders_WithValidUserId_ShouldReturnOk**
- **Mục đích:** Test lấy danh sách orders của user
- **Input:** UserId=1
- **Expected:** OkObjectResult với list orders
- **Mock:** GetUserOrdersAsync(orders)

#### 4. UpdateOrderStatus Tests (1 test case)

**TC-008: UpdateOrderStatus_WithValidData_ShouldReturnOk**
- **Mục đích:** Test cập nhật trạng thái order
- **Input:** OrderId=1, Status="Confirmed"
- **Expected:** OkObjectResult
- **Mock:** UpdateOrderStatusAsync(true)

#### 5. CancelOrder Tests (5 test cases)

**TC-009: CancelOrder_WithValidOrder_ShouldReturnOk**
- **Mục đích:** Test hủy order thành công
- **Input:** OrderId=1 (Pending status)
- **Expected:** OkObjectResult
- **Mock:** GetOrderByIdAsync(order), CancelOrderAsync(true)

**TC-010: CancelOrder_WithNonPendingOrder_ShouldReturnBadRequest**
- **Mục đích:** Test hủy order không ở trạng thái Pending
- **Input:** OrderId=1 (Confirmed status)
- **Expected:** BadRequestObjectResult
- **Mock:** GetOrderByIdAsync(confirmedOrder)

**TC-011: CancelOrder_WithInvalidOrderId_ShouldReturnBadRequest**
- **Mục đích:** Test với OrderId không hợp lệ
- **Input:** OrderId=0
- **Expected:** BadRequestObjectResult
- **Mock:** GetOrderByIdAsync(null)

**TC-012: CancelOrder_WithNonExistentOrder_ShouldReturnNotFound**
- **Mục đích:** Test với order không tồn tại
- **Input:** OrderId=999
- **Expected:** NotFoundObjectResult
- **Mock:** GetOrderByIdAsync(null)

**TC-013: CancelOrder_WithUnauthorizedUser_ShouldReturnForbid**
- **Mục đích:** Test hủy order của user khác
- **Input:** OrderId=1 (belongs to user 2)
- **Expected:** ForbidResult
- **Mock:** GetOrderByIdAsync(orderWithDifferentUser)

#### 6. CleanupExpiredOrders Tests (1 test case)

**TC-014: CleanupExpiredOrders_ShouldReturnOk**
- **Mục đích:** Test cleanup orders hết hạn
- **Input:** No parameters
- **Expected:** OkObjectResult với count
- **Mock:** CleanupExpiredOrdersAsync(count)

---

## 🧪 OrderServiceCoreTests.cs

### 📋 Test Cases Overview
**Tổng số:** 7 test cases  
**Mục đích:** Test business logic của OrderService  
**Phương pháp:** Integration testing với InMemory database  

### 🔧 Setup & Configuration
```csharp
// Database setup
- InMemory database với Guid.NewGuid().ToString()
- Ignore TransactionIgnoredWarning

// Dependencies
- OrderRepository(_context)
- OrderMapper()
- TicketService(_context)
- OrderService(repository, mapper, ticketService, context)

// Test data seeding
- 2 Users (UserId: 1, 2)
- 2 Events (EventId: 1, 2) 
- 2 TicketTypes (VIP: 100$, Standard: 50$)
```

### 📝 Chi tiết Test Cases

#### 1. Order Creation Tests (4 test cases)

**TC-015: ValidRequest_CreatesOrder**
- **Mục đích:** Test tạo order với request hợp lệ
- **Scenario:** Customer mua 2 vé VIP
- **Input:** TicketTypeId=1, Quantity=2, SeatNo="A1, A2"
- **Expected:** Order với Amount=200m, Status="Pending"
- **Test:** MapFromCreateOrderRequest, validate Amount calculation

**TC-016: OutOfStock_BlocksPurchase**
- **Mục đích:** Test block purchase khi hết vé
- **Scenario:** TicketType có Quantity=0
- **Input:** TicketTypeId với Quantity=0
- **Expected:** Exception hoặc validation failure
- **Test:** CheckTicketAvailability returns 0

**TC-017: InvalidTicketType_NotFound**
- **Mục đích:** Test với TicketType không tồn tại
- **Scenario:** TicketTypeId=999
- **Input:** Non-existent TicketTypeId
- **Expected:** Null hoặc exception
- **Test:** Database query returns null

**TC-018: EventNotOpen_BlocksPurchase**
- **Mục đích:** Test block purchase khi event chưa mở
- **Scenario:** Event status != "Open"
- **Input:** Event với Status="Closed"
- **Expected:** Validation failure
- **Test:** Event status validation

#### 2. Business Logic Tests (3 test cases)

**TC-019: CalculateTotalPrice_Accurate**
- **Mục đích:** Test tính toán tổng tiền chính xác
- **Scenario:** 2 vé VIP @ 100$ mỗi vé
- **Input:** Quantity=2, Price=100
- **Expected:** Total = 200
- **Test:** Price * Quantity calculation

**TC-020: CheckTicketAvailability_ReturnsCount**
- **Mục đích:** Test kiểm tra số lượng vé còn lại
- **Scenario:** TicketType có Quantity=10, đã bán 3
- **Input:** TicketTypeId=1
- **Expected:** Available = 7
- **Test:** Quantity - Sold count

**TC-021: ValidateEventIsActive_ReturnsTrue**
- **Mục đích:** Test validation event đang active
- **Scenario:** Event với Status="Open", trong thời gian sale
- **Input:** EventId=1
- **Expected:** True
- **Test:** Status="Open" && DateTime.Now trong SaleStart-SaleEnd

---

## 📊 Coverage Analysis

### 🎯 Coverage Targets
- **OrderController:** Target 80% line coverage
- **OrderService:** Target 80% line coverage
- **Focus Areas:** Buy Ticket flow endpoints

### 📈 Test Coverage Breakdown
```
OrderController Methods:
✅ CreateOrder - 5 test cases
✅ GetOrderById - 1 test case  
✅ GetUserOrders - 1 test case
✅ UpdateOrderStatus - 1 test case
✅ CancelOrder - 5 test cases
✅ CleanupExpiredOrders - 1 test case

OrderService Methods:
✅ CreateOrderAsync - 4 test cases
✅ CalculateTotalPrice - 1 test case
✅ CheckTicketAvailability - 1 test case
✅ ValidateEventIsActive - 1 test case
```

### 🔍 Coverage Gaps Identified
1. **Error handling paths** - Exception scenarios
2. **Edge cases** - Boundary value testing
3. **Concurrent scenarios** - Race conditions
4. **Integration scenarios** - Cross-service interactions

---

## 🚀 Test Execution

### 📋 Prerequisites
```bash
# Dependencies
- .NET 8.0
- xUnit
- Moq
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory
```

### 🏃‍♂️ Running Tests
```bash
# Run all Thien tests
dotnet test --filter "Thien"

# Run specific test class
dotnet test --filter "OrderControllerCoverageTests"
dotnet test --filter "OrderServiceCoreTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### 📊 Expected Results
- **Total Tests:** 21
- **Passed:** 21 ✅
- **Failed:** 0 ❌
- **Skipped:** 0 ⏭️
- **Execution Time:** ~2-3 seconds

---

## 📝 Notes & Observations

### ✅ Strengths
1. **Comprehensive coverage** của OrderController endpoints
2. **Real business scenarios** testing với InMemory database
3. **Proper mocking** của dependencies
4. **Clear test naming** convention
5. **Good separation** giữa Unit và Integration tests

### ⚠️ Areas for Improvement
1. **Exception handling** tests cần thêm
2. **Edge cases** testing có thể mở rộng
3. **Performance testing** cho concurrent scenarios
4. **Integration testing** với real database

### 🔧 Technical Details
- **Mock Strategy:** Sử dụng Moq cho Unit tests
- **Database Strategy:** InMemory cho Integration tests
- **Assertion Strategy:** xUnit + FluentAssertions
- **Test Data:** Seeded data trong constructor

---

## 📅 Maintenance

### 🔄 Regular Updates
- Review test cases khi có thay đổi business logic
- Update mock data khi có thay đổi model
- Monitor coverage metrics định kỳ

### 🐛 Bug Tracking
- Log failed tests với detailed error messages
- Track performance regression
- Monitor test execution time

---

**📧 Contact:** Thiên  
**📅 Last Updated:** 25/10/2025  
**🔄 Version:** 1.0
