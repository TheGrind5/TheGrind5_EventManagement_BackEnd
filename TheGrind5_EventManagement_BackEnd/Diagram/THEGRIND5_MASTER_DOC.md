# THEGRIND5 EVENT MANAGEMENT - PROJECT BACKBONE

## CORE PATTERN

```
Controller → Service → Repository → Database
     ↓         ↓         ↓
   HTTP    Business   Data Access
```

**Nguyên tắc:** Layer trên chỉ biết interface của layer dưới.

## PATTERNS

### 1. CONTROLLER
**Khi nào:** HTTP request từ client
**Để làm gì:** HTTP ↔ Business logic
**Tại sao:** Client không gọi trực tiếp Service

```csharp
[Route("api/[controller]")]
public class XController : ControllerBase
{
    private readonly IXService _xService;
    
    [HttpGet]
    public async Task<IActionResult> GetX()
    {
        var result = await _xService.GetXAsync();
        return Ok(result);
    }
}
```

### 2. SERVICE
**Khi nào:** Business logic phức tạp
**Để làm gì:** Business rules, orchestrate operations
**Tại sao:** Controller không chứa business logic

```csharp
public class XService : IXService
{
    private readonly IXRepository _xRepository;
    private readonly IXMapper _xMapper;
    
    public async Task<XDto> GetXAsync()
    {
        var entity = await _xRepository.GetXAsync();
        return _xMapper.MapToXDto(entity);
    }
}
```

### 3. REPOSITORY
**Khi nào:** Truy cập database
**Để làm gì:** Tách biệt database logic
**Tại sao:** Service không biết database implementation

```csharp
public class XRepository : IXRepository
{
    private readonly EventDBContext _context;
    
    public async Task<X> GetXAsync(int id)
    {
        return await _context.Xs.FindAsync(id);
    }
}
```

### 4. DTO
**Khi nào:** API contract cho data exchange
**Để làm gì:** Tách biệt API model khỏi database model
**Tại sao:** API không nên expose sensitive data

```csharp
public class UserDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    // Không có PasswordHash
}

public class CreateUserRequest
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string FullName { get; set; }
}
```

### 5. MAPPER
**Khi nào:** Chuyển đổi Entity ↔ DTO
**Để làm gì:** Tách biệt database model khỏi API model
**Tại sao:** Entity có thể chứa sensitive data

```csharp
public class XMapper : IXMapper
{
    public XDto MapToXDto(X entity)
    {
        return new XDto(entity.Property1, entity.Property2);
    }
}
```

### 6. MODEL
**Khi nào:** Define database entities
**Để làm gì:** Represent database tables, relationships
**Tại sao:** Database schema phải match business domain

```csharp
public class User
{
    public int UserId { get; set; }
    [Required][EmailAddress] public string Email { get; set; }
    [Required] public string PasswordHash { get; set; }
    public virtual ICollection<Event> Events { get; set; }
}
```

### 7. DEPENDENCY INJECTION
**Khi nào:** Class cần sử dụng class khác
**Để làm gì:** Tự động tạo và inject dependencies
**Tại sao:** Không cần manually tạo objects

```csharp
builder.Services.AddScoped<IXService, XService>();
builder.Services.AddScoped<IXRepository, XRepository>();
```

### 8. ERROR HANDLING
**Khi nào:** Có thể xảy ra lỗi
**Để làm gì:** Xử lý lỗi graceful
**Tại sao:** Không để lỗi crash application

```csharp
try
{
    var result = await _service.DoSomethingAsync();
    return Ok(result);
}
catch (Exception ex)
{
    return BadRequest(new { message = "Error", error = ex.Message });
}
```

### 9. ASYNC/AWAIT
**Khi nào:** I/O operations (database, API calls)
**Để làm gì:** Không block thread, tăng performance
**Tại sao:** Database operations có thể mất thời gian

```csharp
public async Task<X> GetXAsync(int id)
{
    return await _context.Xs.FindAsync(id);
}
```

### 10. JWT AUTHENTICATION
**Khi nào:** Secure API endpoints
**Để làm gì:** Token-based authentication, role-based access
**Tại sao:** API cần biết user identity và permissions

```csharp
public class JwtService : IJwtService
{
    public string GenerateToken(User user)
    {
        var claims = new[] { new Claim(ClaimTypes.Role, user.Role) };
        // Generate JWT token
    }
}
```

### 11. DATA VALIDATION
**Khi nào:** Validate input data
**Để làm gì:** Ensure data integrity
**Tại sao:** Database không nên chứa invalid data

```csharp
public class User
{
    [Required][EmailAddress] public string Email { get; set; }
    [RegularExpression("^(Customer|Host|Admin)$")] public string Role { get; set; }
}
```

### 12. CONFIGURATION
**Khi nào:** Manage app settings
**Để làm gì:** Database connection, JWT settings
**Tại sao:** App cần biết cách connect database

```csharp
// appsettings.json
{
  "ConnectionStrings": { "DefaultConnection": "Server=..." },
  "Jwt": { "Key": "YourSecretKey" }
}
```

### 13. PASSWORD SECURITY
**Khi nào:** Secure user passwords
**Để làm gì:** Hash passwords, verify login
**Tại sao:** Passwords không được lưu plain text

```csharp
public class PasswordService : IPasswordService
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

### 14. EXTENSION METHODS
**Khi nào:** Organize DI registration
**Để làm gì:** Tách biệt DI logic khỏi Program.cs
**Tại sao:** Program.cs không nên quá dài

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
```

### 15. BACKGROUND SERVICES
**Khi nào:** Chạy task định kỳ
**Để làm gì:** Cleanup, maintenance tasks
**Tại sao:** Không block main thread

```csharp
public class OrderCleanupService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Cleanup logic
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### 16. DATABASE CONTEXT
**Khi nào:** Configure Entity Framework
**Để làm gì:** Define relationships, constraints, precision
**Tại sao:** Database schema phải match business rules

```csharp
public class EventDBContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder b)
    {
        ConfigureUserRelationships(b);
        ConfigureDecimalPrecision(b);
    }
}
```

### 17. CORS POLICY
**Khi nào:** Frontend gọi API từ domain khác
**Để làm gì:** Allow cross-origin requests
**Tại sao:** Browser block cross-origin requests by default

```csharp
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## IMPLEMENTATION STEPS

Khi tạo feature mới:
1. Model → DTOs → Repository → Mapper → Service → Controller → Register DI

## CONCLUSION

Đây là xương sống của toàn bộ project. Mọi feature đều follow pattern này.