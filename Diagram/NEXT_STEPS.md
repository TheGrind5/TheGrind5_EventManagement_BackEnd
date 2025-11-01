# 📋 Next Steps để Hoàn Thành Test Suite

## ✅ Đã Hoàn Thành

1. **Test Infrastructure Setup** ✅
   - Tạo cấu trúc thư mục mới (UnitTests/, IntegrationTests/, Fixtures/, Helpers/)
   - Tạo TestDataBuilder với tất cả model builders
   - Tạo DatabaseFixture cho integration tests
   - Tạo TestHelper và MockHelper utilities
   - Cập nhật README.md với cấu trúc mới

2. **Sample Test Files** ✅
   - AuthControllerTests.cs (13 test cases) làm mẫu
   - Demonstrates best practices và patterns

## 🔨 Cần Thực Hiện

### Phase 1: Controller Tests (Priority: High)

Tạo Unit Tests cho 13 Controllers còn lại, theo pattern của AuthControllerTests.cs:

1. **EventController** (25 tests)
   - GetAllEvents với pagination
   - GetEventById
   - SearchEvents với filters
   - CreateEvent, UpdateEvent, DeleteEvent
   - UploadEventImage

2. **OrderController** (25 tests - extend hiện tại)
   - CreateOrder, GetOrder, CancelOrder
   - Payment processing
   - Error cases

3. **WalletController** (18 tests)
   - GetBalance, TopUp, Withdraw
   - Transaction history
   - Error cases

4. **TicketController** (15 tests)
   - GetTickets
   - CheckInTicket
   - TransferTicket

5. **PaymentController** (18 tests)
   - CreatePayment
   - VNPay callback handling
   - Payment status check

6. **VoucherController** (15 tests)
   - GetVouchers, ValidateVoucher
   - ApplyVoucher

7. **WishlistController** (12 tests)
   - Add/Remove wishlist items
   - GetWishlist

8. **NotificationController** (12 tests)
   - GetNotifications, MarkAsRead
   - DeleteNotification

9. **AdminController** (15 tests)
   - BanUser, UnbanUser
   - GetStatistics
   - Admin-only access tests

10. **CampusController** (10 tests)
    - CRUD operations

11. **EventQuestionController** (12 tests)
    - Create/Answer questions

12. **AISuggestionController** (10 tests)
    - GetSuggestions, RateSuggestion

13. **ExportController** (8 tests)
    - Export data functionality

### Phase 2: Service Tests (Priority: High)

Tạo Unit Tests cho 21 Services với mock dependencies:

1. **AuthService** (18 tests)
2. **EventService** (20 tests)
3. **OrderService** (22 tests - extend)
4. **TicketService** (18 tests - extend)
5. **WalletService** (15 tests - extend)
6. **PaymentService** (16 tests)
7. **EmailService** (12 tests)
8. **NotificationService** (12 tests)
9. **VoucherService** (14 tests)
10. **WishlistService** (10 tests)
11. **JwtService** (10 tests)
12. **PasswordService** (8 tests)
13. **FileManagementService** (12 tests)
14. **HuggingFaceService** (10 tests)
15. **OtpService** (12 tests)
16. **AI Services** (32 tests tổng)
17. **OrderCleanupService** (8 tests)
18. **SampleDataExportService** (8 tests)
19. **AdminService** (tests)
20. **EventQuestionService** (tests)
21. **VNPayService** (tests)

### Phase 3: Repository Tests (Priority: Medium)

Tạo Unit Tests cho 6 Repositories với InMemory database:

1. **EventRepository** (12 tests)
2. **OrderRepository** (12 tests - extend)
3. **UserRepository** (12 tests)
4. **PaymentRepository** (10 tests)
5. **EventQuestionRepository** (10 tests)
6. **AISuggestionRepository** (10 tests)

### Phase 4: Mapper & Helper Tests (Priority: Low)

1. **EventMapper** (8 tests)
2. **OrderMapper** (8 tests)
3. **UserMapper** (8 tests)
4. **WishlistMapper** (8 tests)
5. **ApiResponseHelper** (6 tests)
6. **ImagePathConverter** (6 tests)
7. **VNPayHelper** (6 tests)

### Phase 5: Integration Tests (Priority: High)

1. **Controller Integration Tests** (25 tests)
   - Full request/response cycle
   - Authentication flows
   - File upload operations

2. **Service Integration Tests** (15 tests)
   - Multi-service workflows
   - Database transactions

3. **End-to-End Tests** (10 tests)
   - Complete user journeys
   - Buy ticket full flow
   - Payment to notification delivery

### Phase 6: Coverage Analysis (Priority: Critical)

1. Chạy coverage analysis
2. Identify gaps
3. Tạo thêm tests để đạt 80%+ coverage
4. Generate coverage reports

## 🎯 Test Patterns & Best Practices

### Unit Test Template

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using TheGrind5_EventManagement.Controllers;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Tests.Fixtures;
using TheGrind5_EventManagement.Tests.Helpers;

namespace TheGrind5_EventManagement.Tests.UnitTests.Controllers;

public class [ControllerName]Tests
{
    private readonly Mock<IService> _mockService;
    private readonly Mock<ILogger<ControllerName>> _mockLogger;
    private readonly [ControllerName] _controller;

    public [ControllerName]Tests()
    {
        _mockService = new Mock<IService>();
        _mockLogger = new Mock<ILogger<ControllerName>>();
        _controller = new [ControllerName](_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task MethodName_Scenario_ExpectedResult()
    {
        // Arrange
        var data = TestDataBuilder.Create[Model]();
        _mockService.Setup(x => x.MethodAsync())
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Method(data);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockService.Verify(x => x.MethodAsync(), Times.Once);
    }
}
```

### Test Categories

Mỗi test class nên có:
1. **Happy Path Tests** (60%)
2. **Error Cases** (30%)
3. **Edge Cases** (10%)

### Naming Convention

- `[MethodName]_[Scenario]_[ExpectedResult]`
- Ví dụ: `Login_InvalidCredentials_ReturnsUnauthorized`

## 📊 Coverage Goals

| Module | Current | Target | Priority |
|--------|---------|--------|----------|
| Controllers | ~20% | 85% | High |
| Services | ~30% | 90% | Critical |
| Repositories | ~25% | 85% | High |
| Mappers | 0% | 95% | Low |
| Helpers | 0% | 90% | Low |
| **Overall** | **~70%** | **80%+** | **Critical** |

## 🚀 Quick Start Commands

```bash
# Chạy tất cả tests
dotnet test

# Chạy với coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Chạy tests theo namespace
dotnet test --filter "FullyQualifiedName~UnitTests.Controllers"
dotnet test --filter "FullyQualifiedName~UnitTests.Services"
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Chạy một test cụ thể
dotnet test --filter "FullyQualifiedName~AuthControllerTests.Login_ValidCredentials_ReturnsOkWithToken"
```

## 📝 Notes

- Legacy tests trong Thien/, Minh/, etc. được giữ lại cho backward compatibility
- New tests được thêm vào cấu trúc module-based mới
- Tất cả test files sử dụng TestDataBuilder và helpers
- Follow naming conventions và test patterns đã established

---

**Status:** In Progress  
**Last Updated:** 01/11/2025

