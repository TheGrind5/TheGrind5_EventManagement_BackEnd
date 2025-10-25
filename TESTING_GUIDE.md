# 🚀 Unit Testing Setup - Buy Ticket Flow

## 📋 Tổng quan
Project đã được setup đầy đủ cho Unit Testing với cả Backend (.NET) và Frontend (React), tập trung vào **luồng Buy Ticket**.

## 🎯 Kết quả hiện tại
- ✅ **Backend**: 3 tests passed (xUnit + Moq + FluentAssertions)
- ✅ **Frontend**: 5 tests passed (Jest + React Testing Library)
- ✅ **Coverage**: Reports được tạo cho cả hai
- ✅ **Scripts**: Tự động hóa chạy tests

## 🏗️ Cấu trúc Test

### Backend (.NET)
```
TheGrind5_EventManagement_BackEnd/
├── TheGrind5_EventManagement.Tests/
│   ├── Services/
│   │   ├── OrderServiceTests.cs      # Test tạo order
│   │   └── TicketServiceTests.cs     # Test quản lý vé
│   ├── Controllers/
│   │   └── EventControllerTests.cs  # Test API endpoints
│   └── TestResults/                  # Coverage reports
└── run-tests.bat                     # Script chạy tests
```

### Frontend (React)
```
TheGrind5_EventManagement_FrontEnd/
├── src/
│   └── __tests__/
│       └── buyTicket/
│           └── simple.test.js        # Test cho Buy Ticket flow
├── coverage/                         # Coverage reports
└── run-frontend-tests.bat            # Script chạy tests
```

## 🚀 Cách chạy Tests

### 1. Chạy cả Backend và Frontend cùng lúc
```bash
# Từ thư mục Backend
cd TheGrind5_EventManagement_BackEnd
.\run-tests.bat
```

### 2. Chạy riêng từng phần

#### Backend (.NET)
```bash
cd TheGrind5_EventManagement_BackEnd/TheGrind5_EventManagement.Tests
dotnet test                                    # Chạy tests
dotnet test --collect:"XPlat Code Coverage"   # Với coverage
```

#### Frontend (React)
```bash
cd TheGrind5_EventManagement_FrontEnd
npm test                    # Chạy tests (watch mode)
npm run test:once          # Chạy tests một lần
npm run test:coverage      # Với coverage
npm run test:watch         # Watch mode
```

## 📊 Coverage Reports

### Backend Coverage
- **Location**: `TheGrind5_EventManagement.Tests/TestResults/[guid]/coverage.cobertura.xml`
- **Format**: Cobertura XML
- **View**: Mở bằng IDE hoặc coverage viewer

### Frontend Coverage
- **Location**: `coverage/lcov-report/index.html`
- **Format**: HTML report
- **View**: Mở file `index.html` trong browser

## 🎯 Test Cases hiện tại

### Backend Tests
- ✅ **OrderServiceTests**: Placeholder tests cho tạo order
- ✅ **TicketServiceTests**: Placeholder tests cho quản lý vé  
- ✅ **EventControllerTests**: Placeholder tests cho API endpoints

### Frontend Tests
- ✅ **Basic validation**: Test cấu trúc dữ liệu
- ✅ **Price calculation**: Test tính toán giá
- ✅ **Ticket type validation**: Test validation loại vé
- ✅ **Order data structure**: Test cấu trúc order
- ✅ **Price formatting**: Test format giá tiền

## 🔧 Scripts có sẵn

### Backend Scripts
- `run-tests.bat` - Chạy cả Backend và Frontend
- `run-tests.ps1` - PowerShell version

### Frontend Scripts  
- `run-frontend-tests.bat` - Chỉ chạy Frontend
- `npm run test:coverage` - Chạy với coverage
- `npm run test:once` - Chạy một lần
- `npm run test:watch` - Watch mode

## 📈 Bước tiếp theo

Theo hướng dẫn cuộc thi Unit Testing với AI Prompt:

1. **✅ Giai đoạn 1**: Phân tích & Chọn Feature (15') - HOÀN THÀNH
2. **🔄 Giai đoạn 2**: Thiết kế Test Cases (20') - ĐANG LÀM
3. **⏳ Giai đoạn 3**: Sinh Test Code (75') - CHƯA LÀM
4. **⏳ Giai đoạn 4**: Chạy & Debug Tests (40') - CHƯA LÀM
5. **⏳ Giai đoạn 5**: Tối ưu & Mocking (15') - CHƯA LÀM
6. **⏳ Giai đoạn 6**: Documentation & Demo (15') - CHƯA LÀM

## 💡 Tips

- **Chạy tests thường xuyên** để đảm bảo code không bị break
- **Xem coverage reports** để biết phần nào cần test thêm
- **Sử dụng scripts** để tiết kiệm thời gian
- **Tập trung vào Buy Ticket flow** như yêu cầu cuộc thi

---
**Status**: ✅ Setup hoàn tất - Sẵn sàng phát triển tests chi tiết!
