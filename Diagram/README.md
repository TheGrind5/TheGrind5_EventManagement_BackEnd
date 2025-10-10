# TheGrind5 Event Management - Architecture Documentation

## 📁 Cấu trúc Project (Clean Architecture)

```
src/
├── Controllers/                    # Presentation Layer
│   ├── AuthController.cs          # Authentication endpoints
│   └── EventController.cs         # Event management endpoints
├── Core/                          # Business Logic Layer
│   ├── Services/                  # Business Services
│   │   ├── AuthService.cs         # Authentication business logic
│   │   └── EventService.cs        # Event business logic
│   └── Interfaces/                # Service Interfaces (future)
├── Infrastructure/                # Infrastructure Layer
│   ├── Data/                      # Database
│   │   └── EventDBContext.cs      # EF Core DbContext
│   ├── Repositories/              # Data Access Layer
│   │   ├── IUserRepository.cs     # User repository interface
│   │   ├── UserRepository.cs      # User repository implementation
│   │   ├── IEventRepository.cs    # Event repository interface
│   │   └── EventRepository.cs     # Event repository implementation
│   └── Services/                  # Infrastructure Services
│       ├── Jwt/                   # JWT Services
│       │   ├── IJwtService.cs      # JWT service interface
│       │   └── JwtService.cs      # JWT service implementation
│       ├── Password/              # Password Services
│       │   ├── IPasswordService.cs # Password service interface
│       │   └── PasswordService.cs  # Password service implementation
│       └── Mappers/                # Object Mappers
│           ├── IUserMapper.cs      # User mapper interface
│           ├── UserMapper.cs       # User mapper implementation
│           ├── IEventMapper.cs     # Event mapper interface
│           └── EventMapper.cs      # Event mapper implementation
├── Models/                        # Domain Models
│   ├── User.cs                    # User entity
│   ├── Event.cs                   # Event entity
│   ├── Order.cs                   # Order entity
│   ├── Payment.cs                 # Payment entity
│   ├── Ticket.cs                  # Ticket entity
│   ├── TicketType.cs              # TicketType entity
│   └── OrderItem.cs               # OrderItem entity
├── DTOs/                          # Data Transfer Objects
│   ├── AuthDTOs.cs                # Authentication DTOs
│   └── EventDTOs.cs               # Event DTOs
└── Program.cs                     # Application entry point
```

## 🏗️ Kiến trúc Clean Architecture

### 1. **Presentation Layer (Controllers)**
- **Trách nhiệm**: Chỉ xử lý HTTP requests/responses
- **Không có**: Business logic, database access, mapping logic
- **Có**: Validation, error handling, response formatting

### 2. **Core Layer (Business Logic)**
- **Trách nhiệm**: Business logic chính của ứng dụng
- **Có**: AuthService, EventService
- **Gọi**: Repository và Infrastructure Services

### 3. **Infrastructure Layer**
#### **Repositories (Data Access)**
- **Trách nhiệm**: Truy cập database
- **Có**: UserRepository, EventRepository
- **Gọi**: EventDBContext

#### **Infrastructure Services**
- **JwtService**: JWT token generation/validation
- **PasswordService**: Password hashing/verification
- **Mappers**: Object mapping giữa entities và DTOs

### 4. **Domain Layer**
- **Models**: Domain entities
- **DTOs**: Data Transfer Objects

## 🔄 Luồng xử lý

### Authentication Flow
```
Client → AuthController → AuthService → UserRepository → Database
                                    ↓
                              PasswordService (verify)
                                    ↓
                              JwtService (generate token)
                                    ↓
                              UserMapper (map to DTO)
                                    ↓
                              AuthController → Client
```

### Event Management Flow
```
Client → EventController → EventService → EventRepository → Database
                                    ↓
                              EventMapper (map to DTO)
                                    ↓
                              EventController → Client
```

## 🎯 Lợi ích của Clean Architecture

### ✅ **Separation of Concerns**
- Mỗi layer có trách nhiệm rõ ràng
- Dễ maintain và debug
- Dễ test từng component riêng biệt

### ✅ **Dependency Inversion**
- Core layer không phụ thuộc vào Infrastructure
- Dễ thay đổi implementation
- Dễ mock cho testing

### ✅ **Scalability**
- Dễ thêm tính năng mới
- Dễ thay đổi database
- Dễ thêm external services

### ✅ **Team Collaboration**
- Developer mới dễ hiểu
- Code review dễ dàng
- Không conflict khi làm việc nhóm

## 📊 So sánh với cấu trúc cũ

| **Cấu trúc cũ** | **Cấu trúc mới (Clean Architecture)** |
|----------------|----------------------------------------|
| ❌ Tất cả ở 1 folder | ✅ Tách biệt theo layer |
| ❌ Controller "ôm" quá nhiều | ✅ Controller chỉ xử lý HTTP |
| ❌ Service truy cập DB trực tiếp | ✅ Service gọi Repository |
| ❌ JWT logic ở AuthService | ✅ Tách riêng JwtService |
| ❌ Password logic ở AuthService | ✅ Tách riêng PasswordService |
| ❌ Mapping logic ở Controller | ✅ Tách riêng Mapper Services |
| ❌ Khó test và maintain | ✅ Dễ test và maintain |
| ❌ Khó scale | ✅ Dễ scale |

## 🚀 Cách thêm tính năng mới

### Thêm Email Service
1. Tạo `Infrastructure/Services/Email/`
2. Tạo `IEmailService.cs` và `EmailService.cs`
3. Cập nhật `Program.cs` dependency injection
4. Gọi từ `AuthService` khi cần

### Thêm Payment Feature
1. Tạo `Infrastructure/Services/Payment/`
2. Tạo `Core/Services/PaymentService.cs`
3. Tạo `Controllers/PaymentController.cs`
4. Cập nhật DTOs và Models

## 🧪 Testing Strategy

### Unit Tests
- **Controllers**: Mock Services
- **Services**: Mock Repositories và Infrastructure Services
- **Repositories**: Mock DbContext
- **Infrastructure Services**: Test riêng biệt

### Integration Tests
- **API Tests**: Test toàn bộ flow
- **Database Tests**: Test với real database
- **Authentication Tests**: Test JWT flow

## 📈 Performance Considerations

### Database
- **Connection Pooling**: SQL Server connection pooling
- **Query Optimization**: EF Core optimizations
- **Indexing**: Database indexes cho performance

### Caching
- **Future**: Redis cache cho frequently accessed data
- **Future**: Response caching cho static data

### Monitoring
- **Logging**: Structured logging
- **Metrics**: Performance metrics
- **Health Checks**: Application health monitoring

## 🔒 Security Best Practices

### Authentication
- **JWT Tokens**: Secure token generation
- **Password Hashing**: BCrypt with salt
- **Token Expiration**: Configurable expiration

### Authorization
- **Role-based Access**: User roles
- **Route Protection**: Protected endpoints
- **Input Validation**: Model validation

### Data Protection
- **SQL Injection**: EF Core protection
- **XSS Prevention**: Input sanitization
- **CORS**: Cross-origin configuration

## 📚 Documentation

- **Backend_Architecture.md**: Chi tiết kiến trúc backend
- **Frontend_Architecture.md**: Chi tiết kiến trúc frontend  
- **Integration_Architecture.md**: Tích hợp backend-frontend
- **README.md**: Tổng quan cấu trúc project

## 🎉 Kết luận

Cấu trúc Clean Architecture này giúp:
- **Code dễ đọc, dễ hiểu**
- **Dễ maintain và debug**
- **Dễ test và scale**
- **Tuân thủ SOLID principles**
- **Professional development practices**

**Đây là level Senior Developer với Enterprise patterns!** 🚀