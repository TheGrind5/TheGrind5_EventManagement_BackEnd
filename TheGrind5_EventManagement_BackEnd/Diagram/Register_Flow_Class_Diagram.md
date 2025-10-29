# Class Diagram - Register Flow

## Mô tả
Class diagram này mô tả luồng đăng ký (register) trong hệ thống TheGrind5 Event Management, tương tự như luồng login đã có.

## Class Diagram

```mermaid
classDiagram
    %% Input DTOs
    class RegisterRequest {
        +string Username
        +string Email
        +string Password
        +string FullName
        +string? Phone
        +string? Avatar
        +DateTime? DateOfBirth
        +string? Gender
    }

    %% Controller Layer
    class AuthController {
        -IAuthService _authService
        -IUserRepository _userRepository
        +Register(RegisterRequest) IActionResult
        -IsEmailExistsAsync(string) bool
        -CreateRegisterResponse(UserReadDto) object
    }

    %% Service Interface
    class IAuthService {
        <<interface>>
        +RegisterAsync(RegisterRequest) UserReadDto
    }

    %% Service Implementation
    class AuthService {
        -IUserRepository _userRepository
        -IPasswordService _passwordService
        -IUserMapper _userMapper
        +RegisterAsync(RegisterRequest) UserReadDto
    }

    %% Password Service Interface
    class IPasswordService {
        <<interface>>
        +HashPassword(string) string
        +VerifyPassword(string, string) bool
    }

    %% Password Service Implementation
    class PasswordService {
        +HashPassword(string) string
        +VerifyPassword(string, string) bool
    }

    %% User Mapper Interface
    class IUserMapper {
        <<interface>>
        +MapFromRegisterRequest(RegisterRequest, string) User
        +MapToUserReadDto(User) UserReadDto
    }

    %% User Mapper Implementation
    class UserMapper {
        +MapFromRegisterRequest(RegisterRequest, string) User
        +MapToUserReadDto(User) UserReadDto
    }

    %% Repository Interface
    class IUserRepository {
        <<interface>>
        +CreateUserAsync(User) User
        +IsEmailExistsAsync(string) bool
    }

    %% Repository Implementation
    class UserRepository {
        -EventDBContext _context
        +CreateUserAsync(User) User
        +IsEmailExistsAsync(string) bool
    }

    %% Database Context
    class EventDBContext {
        +DbSet~User~ Users
    }

    %% Entity Model
    class User {
        +int UserId
        +string Username
        +string FullName
        +string Email
        +string PasswordHash
        +string? Phone
        +string Role
        +decimal WalletBalance
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +string? Avatar
        +DateTime? DateOfBirth
        +string? Gender
    }

    %% Output DTO
    class UserReadDto {
        +int UserId
        +string FullName
        +string Email
        +string Phone
        +string Role
        +decimal WalletBalance
        +string? Avatar
    }

    %% Register Response
    class RegisterResponse {
        +string Message
        +UserReadDto User
    }

    %% Relationships
    AuthController --> IAuthService : "1. calls"
    AuthController --> IUserRepository : "2. checks email exists"
    
    IAuthService <|-- AuthService : implements
    AuthService --> IUserRepository : "3. queries"
    AuthService --> IPasswordService : "4. uses"
    AuthService --> IUserMapper : "5. uses"
    
    IPasswordService <|-- PasswordService : implements
    IUserMapper <|-- UserMapper : implements
    IUserRepository <|-- UserRepository : implements
    
    UserRepository --> EventDBContext : "6. uses"
    EventDBContext --> User : "7. manages"
    
    AuthService --> User : "8. creates"
    AuthService --> UserReadDto : "9. maps to"
    AuthService --> RegisterResponse : "10. returns"
    
    RegisterRequest --> AuthController : "receives"
    UserMapper --> User : "maps from"
    UserMapper --> UserReadDto : "maps to"
```

## Luồng Register Flow

### 1. Nhận Request
- `RegisterRequest` được gửi đến `AuthController.Register()`
- Controller kiểm tra email đã tồn tại chưa qua `IUserRepository.IsEmailExistsAsync()`

### 2. Xử lý Register
- `AuthController` gọi `IAuthService.RegisterAsync()`
- `AuthService` sử dụng `IPasswordService.HashPassword()` để hash mật khẩu
- `AuthService` sử dụng `IUserMapper.MapFromRegisterRequest()` để tạo User entity

### 3. Lưu Database
- `AuthService` gọi `IUserRepository.CreateUserAsync()` để lưu user vào database
- `UserRepository` sử dụng `EventDBContext` để thao tác với database
- `EventDBContext` quản lý `User` entity trong database

### 4. Trả về Response
- `AuthService` sử dụng `IUserMapper.MapToUserReadDto()` để tạo DTO
- Trả về `UserReadDto` cho `AuthController`
- `AuthController` tạo `RegisterResponse` và trả về cho client

## Các thành phần chính

### Controller Layer
- **AuthController**: Entry point cho register request
- Xử lý validation và response formatting

### Service Layer  
- **IAuthService/AuthService**: Business logic cho register
- **IPasswordService/PasswordService**: Xử lý hash password
- **IUserMapper/UserMapper**: Mapping giữa DTOs và Entities

### Repository Layer
- **IUserRepository/UserRepository**: Data access layer
- **EventDBContext**: Database context

### Models & DTOs
- **User**: Entity model
- **RegisterRequest**: Input DTO
- **UserReadDto**: Output DTO
- **RegisterResponse**: Final response

## Đặc điểm của Register Flow

1. **Validation**: Kiểm tra email đã tồn tại
2. **Security**: Hash password trước khi lưu
3. **Mapping**: Sử dụng mapper để chuyển đổi giữa DTOs và Entities
4. **Default Values**: Tự động set role = "Customer", wallet = 0
5. **Error Handling**: Try-catch để xử lý lỗi
6. **Clean Architecture**: Tách biệt rõ ràng các layer

## So sánh với Login Flow

| Aspect | Login Flow | Register Flow |
|--------|------------|---------------|
| Input | LoginRequest | RegisterRequest |
| Validation | Email/Password format | Email exists check |
| Password | Verify existing hash | Hash new password |
| Output | LoginResponse (with token) | UserReadDto (no token) |
| Database | Read operation | Create operation |
| Security | JWT generation | Password hashing |
