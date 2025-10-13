# Hướng Dẫn Cấu Hình Dự Án TheGrind5 Event Management

## Yêu Cầu Hệ Thống

### Phần Mềm Cần Thiết
- .NET 8.0 SDK hoặc mới hơn
- Node.js 16.x hoặc mới hơn
- SQL Server (LocalDB, Express, hoặc Developer Edition)
- Git

### Kiểm Tra Phiên Bản
```bash
dotnet --version
node --version
npm --version
```

## Cấu Hình Database

### 1. Cài Đặt SQL Server
- Tải và cài đặt SQL Server Express hoặc Developer Edition
- Hoặc sử dụng SQL Server LocalDB (đã có sẵn với Visual Studio)

### 2. Cấu Hình Connection String
Mở file `src/appsettings.json` và thay đổi connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TEN_MAY_CUA_BAN\\SQLEXPRESS;Database=EventDB;User Id=sa;Password=MAT_KHAU_CUA_BAN;TrustServerCertificate=true;"
  }
}
```

**Các thông tin cần thay đổi:**
- `TEN_MAY_CUA_BAN`: Tên máy tính của bạn
- `SQLEXPRESS`: Tên instance SQL Server (có thể là `MSSQLSERVER` nếu dùng default instance)
- `MAT_KHAU_CUA_BAN`: Mật khẩu SQL Server của bạn
- `EventDB`: Tên database (có thể giữ nguyên hoặc đổi tên)

### 3. Tạo Database
Chạy lệnh sau trong thư mục `src`:
```bash
cd src
dotnet ef database update
```

## Cấu Hình Backend

### 1. Cài Đặt Dependencies
```bash
cd TheGrind5_EventManagement_BackEnd/src
dotnet restore
```

### 2. Cấu Hình JWT
Trong file `src/appsettings.json`, có thể thay đổi JWT settings:
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "TheGrind5_EventManagement",
    "Audience": "TheGrind5_EventManagement_Users"
  }
}
```

### 3. Cấu Hình Port
Trong file `src/Properties/launchSettings.json`, có thể thay đổi port:
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

## Cấu Hình Frontend

### 1. Cài Đặt Dependencies
```bash
cd TheGrind5_EventManagement_FrontEnd
npm install
```

### 2. Cấu Hình API URL
Mở file `src/services/api.js` và kiểm tra:
```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

Đảm bảo port này khớp với backend port đã cấu hình.

## Chạy Dự Án

### Cách 1: Sử dụng run.bat
```bash
# Từ thư mục Backend
./run.bat
```

### Cách 2: Chạy thủ công
**Terminal 1 - Backend:**
```bash
cd TheGrind5_EventManagement_BackEnd/src
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd TheGrind5_EventManagement_FrontEnd
npm start
```

### Cách 3: Sử dụng npm script
```bash
cd TheGrind5_EventManagement_FrontEnd
npm run dev
```

## Kiểm Tra Kết Nối

### Backend
- Mở trình duyệt: `http://localhost:5000/swagger`
- Kiểm tra API endpoints

### Frontend
- Mở trình duyệt: `http://localhost:3000`
- Kiểm tra giao diện

## Xử Lý Lỗi Thường Gặp

### 1. Lỗi Database Connection
```
Cannot open database "EventDB" requested by the login
```
**Giải pháp:**
- Kiểm tra SQL Server đang chạy
- Kiểm tra connection string
- Tạo database thủ công nếu cần

### 2. Lỗi Port đã được sử dụng
```
Address already in use
```
**Giải pháp:**
- Thay đổi port trong `launchSettings.json`
- Hoặc kill process đang sử dụng port

### 3. Lỗi npm install
```
npm ERR! peer dep missing
```
**Giải pháp:**
```bash
npm install --legacy-peer-deps
```

### 4. Lỗi dotnet restore
```
Package restore failed
```
**Giải pháp:**
```bash
dotnet clean
dotnet restore
```

## Cấu Hình Môi Trường Khác

### Development
- Backend: `http://localhost:5000`
- Frontend: `http://localhost:3000`

### Production
- Thay đổi connection string cho production database
- Cấu hình HTTPS
- Thay đổi JWT key cho bảo mật

## Lưu Ý Quan Trọng

1. **Bảo mật**: Không commit file `appsettings.json` có thông tin nhạy cảm
2. **Database**: Backup database trước khi thay đổi
3. **Port**: Đảm bảo port không bị conflict với ứng dụng khác
4. **Firewall**: Mở port cần thiết nếu có firewall

## Cấu Trúc Thư Mục

```
TheGrind5_EventManagement_BackEnd/
├── src/
│   ├── appsettings.json          # Cấu hình database và JWT
│   ├── Properties/
│   │   └── launchSettings.json   # Cấu hình port
│   └── ...
├── run.bat                       # Script chạy cả backend và frontend
└── ...

TheGrind5_EventManagement_FrontEnd/
├── src/
│   └── services/
│       └── api.js               # Cấu hình API URL
├── package.json                 # Dependencies và scripts
└── ...
```

## Hỗ Trợ

Nếu gặp vấn đề, kiểm tra:
1. Logs trong console
2. Network tab trong browser DevTools
3. SQL Server Management Studio để kiểm tra database
4. Task Manager để kiểm tra process đang chạy

---

## AI CONTEXT - CHỈ DÀNH CHO AI CURSOR

### PROJECT CONTEXT
This is a full-stack event management system with ASP.NET Core backend and React frontend. When working on this project:

1. **ALWAYS check the current configuration first** - Read appsettings.json, launchSettings.json, and api.js before making changes
2. **Database operations** - Use Entity Framework migrations, never manual SQL
3. **Authentication flow** - JWT tokens are required for protected endpoints
4. **API consistency** - Follow existing patterns in Controllers and Services
5. **Frontend integration** - API calls go through services/api.js, not direct fetch

### CRITICAL FILES TO READ FIRST
- `src/appsettings.json` - Database connection and JWT settings
- `src/Program.cs` - Service registration and middleware configuration  
- `src/Controllers/AuthController.cs` - Authentication endpoints
- `src/Controllers/EventController.cs` - Event management endpoints
- `src/services/api.js` - Frontend API integration
- `run.bat` - Project startup script

### DEVELOPMENT WORKFLOW
1. **Before coding**: Read existing code patterns in Controllers/Services
2. **Database changes**: Create EF migrations, never modify database directly
3. **API changes**: Update both Controller and corresponding Frontend service
4. **Testing**: Use Swagger UI at localhost:5000/swagger
5. **Frontend changes**: Ensure API calls match backend endpoints

### COMMON TASKS FOR AI
- **Add new API endpoint**: Create in Controller, add to Frontend service, test with Swagger
- **Database schema changes**: Create migration, update models, test with seed data
- **Authentication issues**: Check JWT configuration, token expiration, CORS settings
- **Frontend-Backend connection**: Verify API_BASE_URL in api.js matches backend port
- **Dependency issues**: Use exact versions specified in .csproj and package.json

### ERROR RESOLUTION PRIORITY
1. Check connection string in appsettings.json
2. Verify port configuration in launchSettings.json  
3. Ensure API_BASE_URL matches backend port
4. Check JWT token validity and expiration
5. Verify CORS configuration for localhost:3000
6. Run `dotnet ef database update` if database issues
7. Use `npm install --legacy-peer-deps` for frontend dependency issues

### CODE PATTERNS TO FOLLOW
- **Backend**: Controller -> Service -> Repository -> DbContext
- **Frontend**: Component -> API Service -> Backend
- **Authentication**: JWT token in Authorization header
- **Error handling**: Use ApiResponseHelper for consistent responses
- **Mapping**: Use Mapper classes for Entity-DTO conversion

### QUICK COMMANDS FOR AI
```bash
# Check if backend is running
netstat -ano | findstr :5000

# Check if frontend is running  
netstat -ano | findstr :3000

# Reset database
dotnet ef database drop
dotnet ef database update

# Test API endpoints
curl http://localhost:5000/api/Event

# Check logs
dotnet run --verbosity detailed
```

### AI ASSISTANCE GUIDELINES
- **Always read existing code first** before suggesting changes
- **Maintain consistency** with existing patterns and naming
- **Test changes** using Swagger UI and browser DevTools
- **Consider both frontend and backend** when making changes
- **Use existing helper classes** like ApiResponseHelper and Mappers
- **Follow the established architecture** - don't bypass layers

### USER WORKING STYLE & RULES
**Phong cách làm việc của user:**
- **Luôn trả lời bằng tiếng Việt**
- **Commit và merge code cực kỳ cẩn thận** - không được làm bừa
- **Integration (tính nhất quán) và correctness (sự chính xác) là ưu tiên số 1**
- **Trước khi thêm bất cứ thứ gì, phải kiểm tra tương thích hoàn toàn với code hiện tại**
- **Không được phép làm bừa, chế bậy**
- **Code phải chặt chẽ, đặt tính đúng cao hơn tính nhanh**
- **Phong cách kiên trì và kỷ luật - chỉ đi từng bước nhỏ nhàm chán nhưng đều đặn**
- **Không tạo tất cả file 1 lượt, chỉ generate từng chút một**
- **Không nhảy cóc, làm từng bước một cách có hệ thống**

**Nguyên tắc làm việc:**
1. **Kiểm tra tương thích trước** - Mọi thay đổi phải tương thích 100% với code hiện tại
2. **Tính chính xác trên hết** - Code đúng quan trọng hơn code nhanh
3. **Từng bước nhỏ** - Không làm tất cả cùng lúc, chia nhỏ task
4. **Kiểm tra kỹ lưỡng** - Mỗi bước phải được verify trước khi tiếp tục
5. **Không vội vàng** - Chậm mà chắc, đúng mà không sai

