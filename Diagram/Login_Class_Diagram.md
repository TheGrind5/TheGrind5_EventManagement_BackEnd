# Login Flow Class Diagram

```mermaid
classDiagram
    %% DTOs Layer (Top)
    class LoginRequest {
        +string? Email
        +string? Password
    }
    
    class LoginResponse {
        +string AccessToken
        +DateTime ExpiresAt
        +UserReadDto User
    }
    
    class UserReadDto {
        +int UserId
        +string FullName
        +string Email
        +string Phone
        +string Role
        +decimal WalletBalance
    }

    %% Controllers Layer
    class AuthController {
        -IAuthService _authService
        -IUserRepository _userRepository
        +Login(LoginRequest) IActionResult
        -IsValidLoginRequest(LoginRequest) bool
        -CreateLoginResponse(LoginResponse) object
    }

    %% Services Layer
    class IAuthService {
        <<interface>>
        +LoginAsync(email, password) LoginResponse?
    }
    
    class AuthService {
        -IUserRepository _userRepository
        -IJwtService _jwtService
        -IPasswordService _passwordService
        -IUserMapper _userMapper
        +LoginAsync(email, password) LoginResponse?
    }
    
    class IJwtService {
        <<interface>>
        +GenerateToken(User) string
    }
    
    class IPasswordService {
        <<interface>>
        +HashPassword(password) string
        +VerifyPassword(password, hash) bool
    }

    %% Repositories Layer
    class IUserRepository {
        <<interface>>
        +GetUserByEmailAsync(email) User?
        +GetUserByIdAsync(userId) User?
    }
    
    class UserRepository {
        -EventDBContext _context
        +GetUserByEmailAsync(email) User?
        +GetUserByIdAsync(userId) User?
    }

    %% Models & Data Layer
    class User {
        +int UserId
        +string Username
        +string FullName
        +string Email
        +string PasswordHash
        +string Phone
        +string Role
        +decimal WalletBalance
        +DateTime CreatedAt
        +DateTime? UpdatedAt
    }
    
    class EventDBContext {
        +DbSet~User~ Users
    }

    %% Relationships - Vertical Flow (No Crossing)
    LoginRequest --> AuthController : "1. receives"
    AuthController --> IAuthService : "2. calls"
    IAuthService <|-- AuthService : implements
    AuthService --> IUserRepository : "3. queries"
    IUserRepository <|-- UserRepository : implements
    UserRepository --> EventDBContext : "4. uses"
    EventDBContext --> User : "5. manages"
    
    %% Horizontal Dependencies (No Crossing)
    AuthService --> IJwtService : uses
    AuthService --> IPasswordService : uses
    AuthService --> LoginResponse : "6. returns"
    AuthService --> UserReadDto : creates
```

## Luồng xử lý Login

1. **LoginRequest** → **AuthController**: Nhận request từ client
2. **AuthController** → **IAuthService**: Gọi service layer
3. **AuthService** → **IUserRepository**: Query user data
4. **UserRepository** → **EventDBContext**: Truy cập database
5. **EventDBContext** → **User**: Quản lý entity
6. **AuthService** → **LoginResponse**: Trả về kết quả

## Kiến trúc Layers

- **DTOs**: Data Transfer Objects cho input/output
- **Controllers**: Xử lý HTTP requests
- **Services**: Business logic layer
- **Repositories**: Data access layer  
- **Models & Data**: Entity models và database context
