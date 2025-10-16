# Sequence Diagram - Login và Register

## 1. Login Sequence Diagram

```mermaid
sequenceDiagram
    participant Client as Client/Frontend
    participant AuthController as AuthController
    participant AuthService as AuthService
    participant UserRepository as UserRepository
    participant PasswordService as PasswordService
    participant JwtService as JwtService
    participant Database as Database

    Client->>+AuthController: 1: POST /api/auth/login
    Note over Client,AuthController: { email, password }
    
    AuthController->>AuthController: 2: IsValidLoginRequest()
    alt Request không hợp lệ
        AuthController-->>-Client: 3: 400 Bad Request
    end
    
    AuthController->>+AuthService: 4: LoginAsync(email, password)
    AuthService->>+UserRepository: 5: GetUserByEmailAsync(email)
    UserRepository->>+Database: 6: SELECT * FROM Users WHERE Email = @email
    Database-->>-UserRepository: 7: User data
    UserRepository-->>-AuthService: 8: User object
    
    alt User không tồn tại
        AuthService-->>-AuthController: 9: null
        AuthController-->>-Client: 10: 401 Unauthorized
    end
    
    AuthService->>+PasswordService: 11: VerifyPassword(password, hash)
    PasswordService-->>-AuthService: 12: boolean result
    
    alt Password không đúng
        AuthService-->>-AuthController: 13: null
        AuthController-->>-Client: 14: 401 Unauthorized
    end
    
    AuthService->>+JwtService: 15: GenerateToken(user)
    JwtService-->>-AuthService: 16: JWT Token
    
    AuthService->>AuthService: 17: MapToUserReadDto(user)
    AuthService-->>-AuthController: 18: LoginResponse(token, expiresAt, userDto)
    
    AuthController->>AuthController: 19: CreateLoginResponse()
    AuthController-->>-Client: 20: 200 OK
    Note over Client,AuthController: { user, accessToken, expiresAt }
```

## 2. Register Sequence Diagram

```mermaid
sequenceDiagram
    participant Client as Client/Frontend
    participant AuthController as AuthController
    participant UserRepository as UserRepository
    participant AuthService as AuthService
    participant PasswordService as PasswordService
    participant UserMapper as UserMapper
    participant Database as Database

    Client->>+AuthController: 1: POST /api/auth/register
    Note over Client,AuthController: { username, email, password, fullName, phone }
    
    AuthController->>+UserRepository: 2: IsEmailExistsAsync(email)
    UserRepository->>+Database: 3: SELECT COUNT(*) FROM Users WHERE Email = @email
    Database-->>-UserRepository: 4: count
    UserRepository-->>-AuthController: 5: boolean exists
    
    alt Email đã tồn tại
        AuthController-->>-Client: 6: 400 Bad Request
        Note over Client,AuthController: "Email này đã được sử dụng"
    end
    
    AuthController->>+AuthService: 7: RegisterAsync(request)
    AuthService->>+PasswordService: 8: HashPassword(password)
    PasswordService-->>-AuthService: 9: hashedPassword
    
    AuthService->>+UserMapper: 10: MapFromRegisterRequest(request, hashedPassword)
    UserMapper-->>-AuthService: 11: User object
    
    AuthService->>+UserRepository: 12: CreateUserAsync(user)
    UserRepository->>+Database: 13: INSERT INTO Users VALUES (...)
    Database-->>-UserRepository: 14: User with ID
    UserRepository-->>-AuthService: 15: Created User
    
    AuthService->>+UserMapper: 16: MapToUserReadDto(createdUser)
    UserMapper-->>-AuthService: 17: UserReadDto
    AuthService-->>-AuthController: 18: UserReadDto
    
    AuthController->>AuthController: 19: CreateRegisterResponse()
    AuthController-->>-Client: 20: 200 OK
    Note over Client,AuthController: { message, userId, fullName, email, phone, role, walletBalance }
    
    alt Exception xảy ra
        AuthController-->>-Client: 21: 400 Bad Request
        Note over Client,AuthController: "Có lỗi xảy ra khi đăng ký"
    end
```

## 3. Chi tiết các thành phần tham gia

### Controllers
- **AuthController**: Xử lý HTTP requests cho authentication
- **Endpoints**: `/api/auth/login`, `/api/auth/register`

### Services
- **AuthService**: Business logic cho authentication
- **PasswordService**: Hash và verify password
- **JwtService**: Tạo và quản lý JWT tokens

### Repositories
- **UserRepository**: Data access layer cho User entity
- **Database**: SQL Server database

### Mappers
- **UserMapper**: Chuyển đổi giữa DTOs và Models

## 4. Error Handling

### Login Errors
- **400 Bad Request**: Request không hợp lệ (thiếu email/password)
- **401 Unauthorized**: Email hoặc password không đúng

### Register Errors
- **400 Bad Request**: Email đã tồn tại hoặc có lỗi xảy ra
- **Exception**: Các lỗi khác trong quá trình đăng ký

## 5. Security Features

1. **Password Hashing**: Sử dụng BCrypt để hash password
2. **JWT Tokens**: Sử dụng JWT cho authentication
3. **Email Validation**: Kiểm tra email format và uniqueness
4. **Input Validation**: Validate input trước khi xử lý

