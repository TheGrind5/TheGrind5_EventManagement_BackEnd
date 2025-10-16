# Class Specifications - Login Flow

## AuthController Class

### Class Description
AuthController là lớp điều khiển chính xử lý các HTTP requests liên quan đến authentication. Lớp này nhận request từ client, validate input, gọi các service tương ứng và trả về response phù hợp.

### Class Methods

| No | Method | Description |
|---|---|---|
| 01 | Login(LoginRequest) | **Input**: LoginRequest object chứa email và password<br>**Output**: IActionResult với status code và response data<br>**Processing**: <br>1. Validate request input (email, password không null/empty)<br>2. Gọi AuthService.LoginAsync() để xử lý authentication<br>3. Nếu login thành công, tạo response với token và user info<br>4. Nếu thất bại, trả về 401 Unauthorized<br>5. Nếu request không hợp lệ, trả về 400 Bad Request |
| 02 | IsValidLoginRequest(LoginRequest) | **Input**: LoginRequest object<br>**Output**: bool - true nếu request hợp lệ<br>**Processing**: <br>1. Kiểm tra Email không null và có format hợp lệ<br>2. Kiểm tra Password không null và không empty<br>3. Trả về true nếu tất cả điều kiện được thỏa mãn |
| 03 | CreateLoginResponse(LoginResponse) | **Input**: LoginResponse object từ AuthService<br>**Output**: Anonymous object chứa response data<br>**Processing**: <br>1. Tạo object response với các field: user, accessToken, expiresAt<br>2. Format data theo chuẩn API response<br>3. Trả về object để serialize thành JSON |

---

## AuthService Class

### Class Description
AuthService là lớp business logic chính cho authentication. Lớp này orchestrate toàn bộ quá trình login, từ việc tìm user, verify password, generate JWT token đến tạo response.

### Class Methods

| No | Method | Description |
|---|---|---|
| 01 | LoginAsync(email, password) | **Input**: string email, string password<br>**Output**: LoginResponse? - null nếu thất bại<br>**Processing**: <br>1. Gọi UserRepository.GetUserByEmailAsync() để lấy user<br>2. Kiểm tra user có tồn tại không<br>3. Gọi PasswordService.VerifyPassword() để verify password<br>4. Nếu password đúng, gọi JwtService.GenerateToken()<br>5. Gọi UserMapper.MapToUserReadDto() để tạo DTO<br>6. Tạo và trả về LoginResponse với token, expiresAt, userDto<br>7. Trả về null nếu email/password không đúng |

---

## UserRepository Class

### Class Description
UserRepository là lớp data access layer, chịu trách nhiệm truy cập database thông qua Entity Framework. Lớp này cung cấp các method để query và manipulate user data.

### Class Methods

| No | Method | Description |
|---|---|---|
| 01 | GetUserByEmailAsync(email) | **Input**: string email<br>**Output**: User? - null nếu không tìm thấy<br>**Processing**: <br>1. Normalize email: ToLowerInvariant().Trim()<br>2. Sử dụng EF LINQ: _context.Users.FirstOrDefaultAsync()<br>3. Query: WHERE LOWER(Email) == normalizedEmail<br>4. Trả về User object hoặc null<br>**SQL Generated**: SELECT TOP(1) [u].[UserID], [u].[Email], [u].[FullName], [u].[PasswordHash], [u].[Phone], [u].[Role], [u].[CreatedAt], [u].[UpdatedAt], [u].[WalletBalance] FROM [User] AS [u] WHERE LOWER([u].[Email]) = @__normalizedEmail_0 |
| 02 | GetUserByIdAsync(userId) | **Input**: int userId<br>**Output**: User? - null nếu không tìm thấy<br>**Processing**: <br>1. Sử dụng EF: _context.Users.FindAsync(userId)<br>2. FindAsync tối ưu cho primary key lookup<br>3. Trả về User object hoặc null |

---

## IJwtService Interface

### Interface Description
IJwtService định nghĩa contract cho việc tạo và quản lý JWT tokens. Interface này được implement bởi JwtService để generate access tokens cho authenticated users.

### Interface Methods

| No | Method | Description |
|---|---|---|
| 01 | GenerateToken(User) | **Input**: User object<br>**Output**: string - JWT token<br>**Processing**: <br>1. Tạo JWT claims từ User properties (UserId, Email, Role)<br>2. Sử dụng secret key để sign token<br>3. Set expiration time (thường 7 ngày)<br>4. Encode và trả về JWT string |

---

## IPasswordService Interface

### Interface Description
IPasswordService định nghĩa contract cho việc hash và verify passwords. Interface này được implement để sử dụng BCrypt hoặc thuật toán hash an toàn khác.

### Interface Methods

| No | Method | Description |
|---|---|---|
| 01 | HashPassword(password) | **Input**: string password (plain text)<br>**Output**: string - hashed password<br>**Processing**: <br>1. Sử dụng BCrypt hoặc thuật toán hash an toàn<br>2. Generate salt tự động<br>3. Hash password với salt<br>4. Trả về hash string để lưu database |
| 02 | VerifyPassword(password, hash) | **Input**: string password (plain text), string hash (from DB)<br>**Output**: bool - true nếu password đúng<br>**Processing**: <br>1. Sử dụng BCrypt.Verify() để so sánh<br>2. Hash input password với salt từ stored hash<br>3. So sánh với stored hash<br>4. Trả về true nếu khớp, false nếu không |

---

## User Model Class

### Class Description
User là entity model đại diện cho bảng User trong database. Class này chứa tất cả properties của user và được map với database thông qua Entity Framework.

### Class Properties

| Property | Type | Description |
|---|---|---|
| UserId | int | Primary key, auto-increment ID của user |
| Username | string | Tên đăng nhập (có thể null trong DB) |
| FullName | string | Họ và tên đầy đủ của user |
| Email | string | Email address (unique, required) |
| PasswordHash | string | Password đã được hash (BCrypt) |
| Phone | string | Số điện thoại (optional) |
| Role | string | Vai trò: Customer, Host, Admin |
| WalletBalance | decimal | Số dư ví (precision 18,2) |
| CreatedAt | DateTime | Thời gian tạo account |
| UpdatedAt | DateTime? | Thời gian cập nhật cuối |

---

## DTOs Classes

### LoginRequest Class
**Description**: Data Transfer Object cho login request từ client

| Property | Type | Description |
|---|---|---|
| Email | string? | Email đăng nhập |
| Password | string? | Mật khẩu đăng nhập |

### LoginResponse Class
**Description**: Data Transfer Object cho login response trả về client

| Property | Type | Description |
|---|---|---|
| AccessToken | string | JWT token để authenticate |
| ExpiresAt | DateTime | Thời gian hết hạn token |
| User | UserReadDto | Thông tin user |

### UserReadDto Class
**Description**: Data Transfer Object cho user information (không chứa sensitive data)

| Property | Type | Description |
|---|---|---|
| UserId | int | ID của user |
| FullName | string | Họ tên |
| Email | string | Email |
| Phone | string | Số điện thoại |
| Role | string | Vai trò |
| WalletBalance | decimal | Số dư ví |
