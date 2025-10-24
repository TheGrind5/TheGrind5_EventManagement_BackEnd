# Unit Testing Setup Guide

## Tổng quan
Project này đã được setup đầy đủ cho Unit Testing với cả Backend (.NET) và Frontend (React).

## Backend Testing (.NET)

### Tech Stack
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Coverage**: Coverlet

### Cấu trúc
```
TheGrind5_EventManagement_BackEnd/
├── src/                           # Main project
├── TheGrind5_EventManagement.Tests/  # Test project
│   ├── BasicTests.cs             # Sample tests
│   └── TheGrind5_EventManagement.Tests.csproj
└── TheGrind5_EventManagement.sln
```

### Commands
```bash
# Chạy tests
dotnet test

# Chạy tests với coverage
dotnet test --collect:"XPlat Code Coverage"

# Chạy tests trong watch mode
dotnet test --watch

# Build test project
dotnet build TheGrind5_EventManagement.Tests
```

### Coverage Report
Coverage report được tạo tại: `TestResults/[guid]/coverage.cobertura.xml`

## Frontend Testing (React)

### Tech Stack
- **Framework**: Jest (built-in với react-scripts)
- **Testing Library**: React Testing Library
- **Assertions**: Jest DOM matchers
- **Coverage**: Jest built-in

### Cấu trúc
```
TheGrind5_EventManagement_FrontEnd/
├── src/
│   ├── components/
│   ├── utils/
│   │   └── __tests__/
│   │       └── math.test.js      # Sample tests
│   └── setupTests.js            # Jest setup
├── jest.config.js               # Jest configuration
└── package.json
```

### Commands
```bash
# Chạy tests
npm test

# Chạy tests với coverage
npm run test:coverage

# Chạy tests trong watch mode
npm run test:watch

# Chạy tests một lần (không watch)
npm test -- --watchAll=false
```

### Coverage Report
Coverage report được tạo tại: `coverage/lcov-report/index.html`

## Sample Tests

### Backend (.NET)
```csharp
[Fact]
public void SimpleMathTest_ShouldPass()
{
    // Arrange
    int a = 2;
    int b = 3;

    // Act
    int result = a + b;

    // Assert
    result.Should().Be(5);
}
```

### Frontend (React)
```javascript
test('add function should add two numbers correctly', () => {
  expect(add(2, 3)).toBe(5);
  expect(add(-1, 1)).toBe(0);
  expect(add(0, 0)).toBe(0);
});
```

## IDE Extensions (Recommended)

### Visual Studio Code
- Jest (Orta.vscode-jest)
- Test Explorer UI
- Coverage Gutters
- C# Dev Kit

### Visual Studio / Rider
- Built-in test runner
- Coverage reports
- Test Explorer

## Troubleshooting

### Backend Issues
- **Build errors**: Kiểm tra main project có lỗi không
- **Missing references**: Đảm bảo test project reference đúng main project
- **Coverage không hiển thị**: Chạy với flag `--collect:"XPlat Code Coverage"`

### Frontend Issues
- **Module not found**: Kiểm tra import paths
- **Jest không chạy**: Xóa node_modules và reinstall
- **Coverage không hiển thị**: Chạy với flag `--coverage`

## Next Steps

1. **Chọn Core Feature** để test (theo hướng dẫn cuộc thi)
2. **Phân tích functions** cần test
3. **Tạo test cases** với AI prompts
4. **Implement tests** từng bước một
5. **Debug và optimize** coverage

## AI Prompts Ready

Bạn đã sẵn sàng sử dụng các AI prompts từ cuộc thi:
- Prompt 1: Phân tích code và identify functions
- Prompt 2: Generate test cases
- Prompt 3: Generate test code
- Prompt 4: Debug failing tests
- Prompt 5: Generate mocks

---

**Status**: ✅ Setup hoàn tất - Sẵn sàng bắt đầu Unit Testing Challenge!
