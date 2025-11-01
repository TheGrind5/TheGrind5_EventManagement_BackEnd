# 🤖 AI Test Generation Guide

## 📖 Tổng Quan

Tài liệu này mô tả quy trình sử dụng AI để tạo comprehensive test suite cho TheGrind5 Event Management System. Quy trình này đã được áp dụng thành công để tạo test infrastructure và các sample test files.

## 🎯 Mục Tiêu

Tạo test suite hoàn chỉnh để đạt **80% code coverage** cho toàn bộ dự án Event Management, bao gồm:
- **14 Controllers**
- **21 Services**
- **6 Repositories**
- **4 Mappers**
- **3 Helpers**

## 🔄 Quy Trình Sử D dụng AI

### Phase 1: Lập Kế Hoạch (Planning)

#### Bước 1: Hiểu Dự Án
**Yêu cầu AI:**
```
"Tôi cần lập kế hoạch để tạo test suite cho dự án TheGrind5 Event Management.
Hãy giúp tôi phân tích cấu trúc dự án và xác định phạm vi test cần thiết."
```

**AI đã phân tích:**
- Tổng số components cần test
- Phân loại theo Controllers, Services, Repositories
- Ước tính số lượng test cases cần thiết
- Phân bổ độ ưu tiên

#### Bước 2: Xác Định Yêu Cầu
**Yêu cầu với AI:**
```
"Tôi muốn:
1. Test toàn bộ dự án
2. Cả Unit Tests và Integration Tests
3. Test đầy đủ với happy path, error cases, edge cases
4. Tái tổ chức lại theo module thay vì theo member"
```

**AI đã đề xuất:**
- Cấu trúc thư mục mới theo module
- Loại tests cần thiết
- Test patterns và best practices

### Phase 2: Tạo Infrastructure (Foundation)

#### Bước 1: Tạo TestDataBuilder
**Yêu cầu AI:**
```
"Hãy tạo TestDataBuilder class để dễ dàng tạo test data.
Builder cần support tất cả models trong dự án."
```

**AI đã thực hiện:**
1. Phân tích tất cả models trong `src/Models/`
2. Tạo builder methods cho mỗi model
3. Tạo helper methods cho common scenarios
4. Implement auto-incrementing IDs

**Kết quả:** `TestDataBuilder.cs` (531 lines)

#### Bước 2: Tạo DatabaseFixture
**Yêu cầu AI:**
```
"Tạo DatabaseFixture để setup InMemory database cho integration tests.
Fixture cần có methods để seed data và clear data."
```

**AI đã thực hiện:**
1. Setup InMemory database
2. Implement ClearAllDataAsync()
3. Implement SeedTestDataAsync()
4. Dispose pattern

**Kết quả:** `DatabaseFixture.cs` (182 lines)

#### Bước 3: Tạo TestHelpers
**Yêu cầu AI:**
```
"Tạo các helper classes cho tests:
- TestHelper: General utilities
- MockHelper: Mock object factories"
```

**AI đã thực hiện:**
1. ClaimsPrincipal creation
2. Property verification
3. Random data generation
4. Mock object factories

**Kết quả:**
- `TestHelper.cs` (164 lines)
- `MockHelper.cs` (189 lines)

### Phase 3: Tạo Sample Tests (Examples)

#### Bước 1: AuthController Tests
**Yêu cầu AI:**
```
"Tạo comprehensive unit tests cho AuthController.
Test phải cover login, register, profile operations, wallet operations.
Sử dụng TestDataBuilder và TestHelper đã tạo."
```

**AI đã thực hiện:**
1. Setup mock dependencies
2. Create 13 test cases covering:
   - Login: valid, invalid, banned user
   - Register: valid, duplicate email, weak password
   - GetCurrentUser
   - UpdateProfile
   - WalletBalance
3. Apply AAA pattern
4. Use FluentAssertions

**Kết quả:** `AuthControllerTests.cs` (343 lines, 13 tests)

#### Bước 2: AuthService Tests
**Yêu cầu AI:**
```
"Tạo unit tests cho AuthService.
Focus vào business logic và error handling."
```

**AI đã thực hiện:**
1. Mock tất cả dependencies
2. Create 8 test cases
3. Test error scenarios
4. Verify mock calls

**Kết quả:** `AuthServiceTests.cs` (226 lines, 8 tests)

#### Bước 3: UserRepository Tests
**Yêu cầu AI:**
```
"Tạo integration tests cho UserRepository với InMemory database.
Test CRUD operations và complex queries."
```

**AI đã thực hiện:**
1. Setup InMemory database
2. Create 13 test cases
3. Test transactions
4. Verify database state

**Kết quả:** `UserRepositoryTests.cs` (277 lines, 13 tests)

## 📝 Prompts Examples

### Prompt Template 1: Tạo New Test File
```
"Hãy tạo unit tests cho [ComponentName] theo pattern của AuthControllerTests.cs.

Requirements:
- Sử dụng TestDataBuilder để tạo test data
- Sử dụng MockHelper cho mock setup
- Cover các scenarios:
  * Happy path: [list scenarios]
  * Error cases: [list scenarios]
  * Edge cases: [list scenarios]
- Follow naming convention: [MethodName]_[Scenario]_[ExpectedResult]
- Use FluentAssertions cho assertions
- Target: [number] test cases
"

Example:
"Hãy tạo unit tests cho EventController theo pattern của AuthControllerTests.cs.
Cover: GetAllEvents, GetEventById, CreateEvent, UpdateEvent, DeleteEvent, SearchEvents.
Target: 20 test cases."
```

### Prompt Template 2: Extend Existing Tests
```
"Extend [ExistingTestFile] với thêm test cases cho [scenarios].
Đảm bảo maintain code style và patterns hiện tại.
"

Example:
"Extend OrderControllerTests với thêm test cases cho voucher application và refund processing."
```

### Prompt Template 3: Fix Issues
```
"Test [TestName] đang fail với error: [error message].
Hãy giúp tôi debug và fix."
"

Example:
"Test UserRepository_CreateUser_ThrowsExceptionOnDuplicate đang fail.
Error: Expected exception was not thrown."
```

## 🏗️ Architecture Patterns

### Pattern 1: Controller Tests
```csharp
public class [Controller]Tests
{
    private readonly Mock<IService1> _mockService1;
    private readonly Mock<IService2> _mockService2;
    private readonly Mock<ILogger<Controller>> _mockLogger;
    private readonly [Controller] _controller;

    public [Controller]Tests()
    {
        _mockService1 = new Mock<IService1>();
        _mockService2 = new Mock<IService2>();
        _mockLogger = new Mock<ILogger<Controller>>();
        _controller = new [Controller](_mockService1.Object, _mockService2.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task MethodName_Scenario_ExpectedResult()
    {
        // Arrange
        var data = TestDataBuilder.Create[Model]();
        _mockService1.Setup(x => x.MethodAsync())
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Method(data);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockService1.Verify(x => x.MethodAsync(), Times.Once);
    }
}
```

### Pattern 2: Service Tests
```csharp
public class [Service]Tests
{
    private readonly Mock<IRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly [Service] _service;

    // Tests focus on business logic
}
```

### Pattern 3: Repository Tests
```csharp
public class [Repository]Tests : IDisposable
{
    private readonly EventDBContext _context;
    private readonly [Repository] _repository;

    public [Repository]Tests()
    {
        var options = new DbContextOptionsBuilder<EventDBContext>()
            .UseInMemoryDatabase(databaseName: $"TestDB_{Guid.NewGuid()}")
            .Options;
        _context = new EventDBContext(options);
        _repository = new [Repository](_context);
    }

    // Tests with actual database operations
}
```

## ✅ Best Practices Applied

### 1. Naming Conventions
- Test methods: `[MethodName]_[Scenario]_[ExpectedResult]`
- Test classes: `[ComponentName]Tests`
- Arrange-Act-Assert pattern

### 2. Test Isolation
- Mỗi test độc lập
- Fresh database instance cho repository tests
- Mock mọi external dependencies

### 3. Coverage Distribution
- **Happy Path**: ~60% tests
- **Error Cases**: ~30% tests
- **Edge Cases**: ~10% tests

### 4. Code Quality
- FluentAssertions cho readable assertions
- Descriptive test names
- Comments trong complex tests
- Follow SOLID principles

## 🎓 Lessons Learned

### 1. Start with Infrastructure
**Lesson**: Setup infrastructure đầy đủ trước khi viết tests
**Benefit**: Tests nhất quán, dễ maintain

### 2. Create Samples First
**Lesson**: Tạo 1-2 sample test files hoàn chỉnh trước
**Benefit**: Establish patterns cho team

### 3. Iterative Approach
**Lesson**: Không tạo hết tất cả tests một lúc
**Benefit**: Có thể refine patterns dần

### 4. AI as Assistant, Not Replacement
**Lesson**: AI tạo code, developers review và refine
**Benefit**: Balance tốc độ và quality

## 📊 Metrics & Results

### Time Saved
- Manual: ~40-60 hours cho infrastructure
- With AI: ~4-6 hours
- **Savings: 85-90%**

### Code Quality
- **Linter errors**: 0
- **Code coverage**: Target 80%+
- **Test count**: 34+ và đang tăng

### Maintainability
- **Consistent patterns**: 100%
- **Documentation**: Comprehensive
- **Reusability**: High

## 🚀 Next Steps

### Immediate Actions
1. Review generated tests
2. Run coverage analysis
3. Fill gaps identified

### Short-term Goals
- Extend tests cho remaining controllers
- Add service tests cho critical components
- Implement integration tests

### Long-term Goals
- Achieve 80%+ coverage
- Performance benchmarks
- CI/CD integration

## 📞 Support

### Khi Cần Help với AI
1. Provide context đầy đủ
2. Reference existing patterns
3. Specify requirements clearly
4. Iterate based on results

### Khi Generated Code Cần Fix
1. Describe issue clearly
2. Share error messages
3. Show relevant code context
4. AI will suggest fixes

---

**Version**: 1.0  
**Last Updated**: 01/11/2025  
**Status**: Active Development 🚀

