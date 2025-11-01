# 🚀 Quick Start Guide

## Bắt Đầu Nhanh Trong 5 Phút

### Prerequisites
- .NET 8.0 SDK installed
- Visual Studio 2022 hoặc VS Code
- Git repository cloned

### Step 1: Navigate to Test Folder
```bash
cd TheGrind5_EventManagement_BackEnd/TheGrind5_EventManagement.Tests
```

### Step 2: Build Project
```bash
dotnet build
```

### Step 3: Run Tests

#### Option A: Sử dụng run.bat (Recommended)
```bash
run.bat
```

Menu sẽ hiển thị:
```
NEW STRUCTURE:
1. Run ALL tests
2. Run Controllers tests
3. Run Services tests  
4. Run Repositories tests

LEGACY:
5-9. Member tests

UTILITIES:
10. Show Summary
11. Generate Coverage Report
12. Exit
```

#### Option B: Command Line
```bash
# All tests
dotnet test

# Specific category
dotnet test --filter "FullyQualifiedName~Controllers"

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Step 4: View Results
Coverage report sẽ tự động mở trong browser nếu chọn option 11.

## 📚 Documentation Map

### Bắt Đầu Ở Đâu?

#### 1. New Team Member?
→ Đọc: `PROJECT_OVERVIEW.md`
- Hiểu dự án
- Cấu trúc và workflow
- Current status

#### 2. Muốn Viết Tests?
→ Đọc: `AI_TEST_GENERATION_GUIDE.md`
- Quy trình dùng AI
- Prompt templates
- Best practices

#### 3. Muốn Contribute?
→ Đọc: `NEXT_STEPS.md`
- Roadmap chi tiết
- Priorities
- Quick commands

#### 4. Muốn Technical Details?
→ Đọc: `IMPLEMENTATION_SUMMARY.md`
- Architecture decisions
- Patterns
- Code examples

#### 5. Muốn Overview?
→ Đọc: `README.md`
- Summary
- Test stats
- Quick commands

## 🎯 Common Tasks

### Task 1: Chạy Tests
```bash
run.bat → Option 1
```

### Task 2: Xem Coverage
```bash
run.bat → Option 1 (run tests with coverage)
run.bat → Option 11 (generate report)
```

### Task 3: Viết Test Mới
```bash
# 1. Copy sample test
copy UnitTests\Controllers\AuthControllerTests.cs UnitTests\Controllers\[NewController]Tests.cs

# 2. Edit với IDE
# 3. Build & Run
dotnet build
dotnet test --filter "FullyQualifiedName~[NewController]"
```

### Task 4: Debug Failed Test
```bash
# Run với verbosity
dotnet test --filter "TestName" --verbosity detailed

# Hoặc trong IDE:
# F5 để debug test
```

## 🆘 Troubleshooting

### Build Fails
```
Error: File locked by TheGrind5_EventManagement.exe
Solution: Đóng application đang chạy
```

### Tests Not Found
```
Error: No test matches filter
Solution: Check namespace và filter syntax
```

### Coverage Report Empty
```
Solution: Run tests với --collect:"XPlat Code Coverage" trước
```

## 📞 Getting Help

### Questions?
- Check documentation trong folder
- Review sample test files
- Ask team members

### Issues?
- Check IMPLEMENTATION_SUMMARY.md
- Review error messages
- Verify dependencies

### Ideas?
- Document trong NEXT_STEPS.md
- Discuss with team
- Create proposal

---

**Need more?** Check documentation folder 📂

