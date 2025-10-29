# 🧪 Unit Testing Guide - TheGrind5 Event Management

## 📊 Test Results Summary

**🎉 21/21 Tests Passed (100% Success Rate)**

### 🎯 Completed Test Coverage

#### **TicketService Tests (8 tests)**
- ✅ `GetTicketsByUserIdAsync_ValidUser_ReturnsTickets` - Lấy vé của user thành công
- ✅ `GetTicketsByUserIdAsync_Empty_ReturnsEmptyList` - User chưa có vé
- ✅ `GetTicketsByUserIdAsync_InvalidUser_ThrowsException` - User không hợp lệ (FIXED)
- ✅ `GetTicketByIdAsync_ValidId_ReturnsTicket` - Lấy vé theo ID thành công
- ✅ `GetTicketByIdAsync_InvalidId_ReturnsNull` - ID vé không tồn tại
- ✅ `CheckInTicketAsync_EventNotStarted_ThrowsException` - Event chưa bắt đầu
- ✅ `CheckInTicketAsync_EventEnded_ThrowsException` - Event đã kết thúc
- ✅ `CheckInTicketAsync_ValidTicket_UpdatesStatusToUsed` - Check-in vé thành công (FIXED)

#### **OrderController Tests (12 tests)**
- ✅ `CreateOrder_Success_ReturnsOk` - Tạo order thành công
- ✅ `CreateOrder_Unauthorized_ReturnsUnauthorized` - Không có quyền truy cập
- ✅ `CreateOrder_InvalidRequest_ReturnsBadRequest` - Dữ liệu request không hợp lệ
- ✅ `CreateOrder_ConcurrentPurchase_HandlesRaceCondition` - Xử lý mua vé đồng thời
- ✅ `ProcessPayment_Wallet_Success` - Thanh toán qua ví thành công
- ✅ `ProcessPayment_InsufficientBalance_ReturnsBadRequest` - Số dư ví không đủ
- ✅ `ProcessPayment_OrderNotFound_ReturnsNotFound` - Order không tồn tại
- ✅ `ProcessPayment_OrderNotPending_ReturnsBadRequest` - Order không ở trạng thái pending
- ✅ `ProcessPayment_UnsupportedPaymentMethod_ReturnsBadRequest` - Phương thức thanh toán không hỗ trợ
- ✅ `ProcessPayment_InvalidOrderId_ReturnsBadRequest` - ID order không hợp lệ
- ✅ `ProcessPayment_UnauthorizedUser_ReturnsForbid` - User không có quyền thanh toán
- ✅ `CreateOrder_UserNotExists_ReturnsUnauthorized` - User không tồn tại

## 🛠️ How to Run Tests

### **Method 1: Command Line (Recommended)**
```bash
cd TheGrind5_EventManagement_BackEnd/TheGrind5.Tests
dotnet test
```

**Expected Output:**
```
Passed!  - Failed:     0, Passed:    21, Skipped:     0, Total:    21, Duration: 1 s
```

### **Method 2: Detailed Verbose Output**
```bash
cd TheGrind5_EventManagement_BackEnd/TheGrind5.Tests
dotnet test --verbosity normal
```

**Shows individual test results:**
```
Passed TheGrind5.Tests.Services.TicketServiceTests.GetTicketsByUserIdAsync_ValidUser_ReturnsTickets [377 ms]
Passed TheGrind5.Tests.Controllers.OrderControllerTests.CreateOrder_Success_ReturnsOk [3 ms]
...
```

### **Method 3: Batch File**
```bash
cd TheGrind5_EventManagement_BackEnd/TheGrind5.Tests
.\run_tests.bat
```

### **Method 4: Visual Studio**
1. Mở solution `TheGrind5_EventManagement.sln`
2. Right-click vào test project `TheGrind5.Tests`
3. Chọn "Run Tests"
4. Xem kết quả trong Test Explorer

### **Method 5: Run Specific Tests**
```bash
# Chạy chỉ TicketService tests
dotnet test --filter "TicketServiceTests"

# Chạy chỉ OrderController tests  
dotnet test --filter "OrderControllerTests"

# Chạy test cụ thể
dotnet test --filter "GetTicketsByUserIdAsync_ValidUser_ReturnsTickets"
```

## 📁 Project Structure

```
TheGrind5.Tests/
├── Controllers/
│   └── OrderControllerTests.cs          # 13 tests cho OrderController
├── Services/
│   └── TicketServiceTests.cs            # 8 tests cho TicketService
├── TestHelpers/
│   └── TestDTOs.cs                      # Helper DTOs cho testing
├── run_tests.bat                        # Script chạy tests
├── README.md                            # Hướng dẫn chi tiết
└── Unit_Testing_Guide.md               # File này
```

## 🔧 Test Dependencies

- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database cho testing
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing

## 🎯 Test Categories

### **Unit Tests**
- Test individual methods in isolation
- Mock external dependencies
- Fast execution
- High coverage

### **Integration Tests**
- Test component interactions
- Use real database (InMemory)
- Test complete workflows
- Slower execution

## ✅ Issues Fixed Successfully

### **1. GetTicketsByUserIdAsync_InvalidUser_ThrowsException** ✅ FIXED
**Issue**: Test expects `ArgumentException` but no exception is thrown
**Fix Applied**: Added validation in `TicketService.GetTicketsByUserIdAsync()`:
```csharp
if (userId <= 0)
    throw new ArgumentException("User ID must be greater than 0", nameof(userId));
```

### **2. CheckInTicketAsync_ValidTicket_UpdatesStatusToUsed** ✅ FIXED
**Issue**: Event has already ended, causing `InvalidOperationException`
**Fix Applied**: Updated test data to use future event dates:
```csharp
StartTime = DateTime.UtcNow.AddMinutes(-30), // Event started 30 minutes ago
EndTime = DateTime.UtcNow.AddHours(10),      // Event ends in 10 hours
```

## 📈 Test Coverage Analysis

### **High Coverage Areas**
- ✅ Order creation workflow
- ✅ Payment processing
- ✅ Error handling
- ✅ Authorization checks

### **Areas Needing More Tests**
- ⚠️ Edge cases for ticket validation
- ⚠️ Complex business logic scenarios
- ⚠️ Performance testing
- ⚠️ Concurrent access scenarios

## 🎓 Best Practices Applied

### **1. Test Naming Convention**
```csharp
[MethodName]_[Scenario]_[ExpectedResult]
// Example: CreateOrder_Success_ReturnsOk
```

### **2. AAA Pattern**
```csharp
// Arrange - Setup test data
// Act - Execute method
// Assert - Verify results
```

### **3. Mocking Strategy**
- Mock external dependencies
- Use InMemory database for data access
- Isolate units under test

### **4. Test Data Management**
- Use builder pattern for test data
- Clean up after each test
- Use unique identifiers

## 🚀 Next Steps

1. ✅ **Fix failing tests** - All 2 failing test cases have been resolved
2. **Add more edge cases** - Cover more business scenarios
3. **Performance testing** - Add load and stress tests
4. **Integration testing** - Test complete user workflows
5. **Code coverage** - Aim for 90%+ coverage
6. **Add more service tests** - Test other services like EventService, AuthService
7. **Add controller tests** - Test other controllers like EventController, AuthController

## 🔧 Troubleshooting

### **Common Issues & Solutions**

#### **1. Tests Not Running**
```bash
# Ensure you're in the correct directory
cd TheGrind5_EventManagement_BackEnd/TheGrind5.Tests

# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

#### **2. Build Errors**
```bash
# Restore packages
dotnet restore

# Check for missing dependencies
dotnet list package
```

#### **3. Test Failures**
- Check console output for detailed error messages
- Verify test data setup
- Ensure all dependencies are properly mocked
- Check database context configuration

#### **4. Performance Issues**
- Use `--logger "console;verbosity=minimal"` for faster output
- Run specific test categories instead of all tests
- Consider using `--parallel` flag for parallel execution

## 📊 Test Metrics

### **Current Status**
- **Total Tests**: 21
- **Passed**: 21 (100%)
- **Failed**: 0 (0%)
- **Skipped**: 0 (0%)
- **Average Duration**: ~1 second
- **Coverage**: TicketService + OrderController

### **Test Categories**
- **Unit Tests**: 21 tests
- **Integration Tests**: 0 tests (planned)
- **Performance Tests**: 0 tests (planned)

## 📚 Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Entity Framework Testing](https://docs.microsoft.com/en-us/ef/core/testing/)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)

---

**🎉 Congratulations!** You now have a solid foundation for unit testing in TheGrind5 Event Management System. The test suite covers the most critical functionality and provides a good starting point for ensuring code quality and reliability.

**Last Updated**: October 25, 2025  
**Test Status**: ✅ All 21 tests passing  
**Next Review**: Add more service and controller tests
