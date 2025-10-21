# QUY TẮC LẬP TRÌNH - THEGRIND5 EVENT MANAGEMENT

## 📋 MỤC LỤC
1. [Quy tắc đặt tên (Naming Conventions)](#quy-tắc-đặt-tên)
2. [Quy tắc API và DTOs](#quy-tắc-api-và-dtos)
3. [Quy tắc Database Schema](#quy-tắc-database-schema)
4. [Quy tắc Error Handling](#quy-tắc-error-handling)
5. [Quy tắc Code Structure](#quy-tắc-code-structure)
6. [Quy tắc Frontend-Backend Integration](#quy-tắc-frontend-backend-integration)

---

## 🏷️ QUY TẮC ĐẶT TÊN

### **Backend (C#/.NET)**
```csharp
// ✅ ĐÚNG - PascalCase cho public members
public class UserService { }
public string FullName { get; set; }
public async Task<IActionResult> CreateOrder() { }

// ✅ ĐÚNG - camelCase cho private fields
private readonly IOrderService _orderService;
private string _connectionString;

// ✅ ĐÚNG - camelCase cho parameters
public async Task<bool> ValidateUserExistsAsync(int userId) { }
```

### **Frontend (JavaScript/React)**
```javascript
// ✅ ĐÚNG - camelCase cho tất cả
const [userData, setUserData] = useState(null);
const handleCreateOrder = async () => { };
const orderData = { eventId: 1, quantity: 2 };
```

### **Database (SQL Server)**
```sql
-- ✅ ĐÚNG - PascalCase cho table/column names
CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE
);

-- ✅ ĐÚNG - PascalCase cho constraints
CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerId) REFERENCES [User](UserId)
```

---

## 🔄 QUY TẮC API VÀ DTOs

### **API Response Format**
```csharp
// ✅ ĐÚNG - camelCase cho JSON responses
return Ok(new {
    user = new {
        userId = user.UserId,
        fullName = user.FullName,
        email = user.Email,
        phone = user.Phone,
        role = user.Role,
        walletBalance = user.WalletBalance
    },
    accessToken = result.AccessToken,
    expiresAt = result.ExpiresAt
});
```

### **DTOs Structure**
```csharp
// ✅ ĐÚNG - camelCase cho DTOs
public record ProfileDetailDto(
    int userId,
    string username,
    string fullName,
    string email,
    string phone,
    string role,
    DateTime createdAt,
    DateTime? updatedAt
);

// ✅ ĐÚNG - camelCase cho request DTOs
public record UpdateProfileRequest(
    string? fullName = null,
    string? phone = null
);
```

### **Frontend API Calls**
```javascript
// ✅ ĐÚNG - camelCase cho request data
const orderData = {
    eventId: parseInt(id),
    ticketTypeId: parseInt(selectedTicketType),
    quantity: quantity,
    seatNo: null
};

// ✅ ĐÚNG - camelCase cho response handling
const response = await ordersAPI.create(orderData);
console.log('Order created:', response.order.orderId);
```

---

## 🗄️ QUY TẮC DATABASE SCHEMA

### **Primary Keys**
```sql
-- ✅ ĐÚNG - IDENTITY(1,1) để đảm bảo ID bắt đầu từ 1
CREATE TABLE [User](
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    -- Không được phép ID = 0
);
```

### **Data Types Consistency**
```sql
-- ✅ ĐÚNG - Standardized lengths
Username NVARCHAR(100) NOT NULL UNIQUE,
FullName NVARCHAR(255) NOT NULL,
Email NVARCHAR(255) NOT NULL UNIQUE,
Phone NVARCHAR(20),
PasswordHash NVARCHAR(500) NOT NULL,
Role VARCHAR(16) NOT NULL,
WalletBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
UpdatedAt DATETIME2(0)
```

### **Foreign Key Constraints**
```sql
-- ✅ ĐÚNG - ON DELETE CASCADE cho most relationships
CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerId) REFERENCES [User](UserId) ON DELETE CASCADE

-- ✅ ĐÚNG - ON DELETE SET NULL cho optional relationships
CONSTRAINT FK_Ticket_OrderItem FOREIGN KEY (OrderItemId) REFERENCES OrderItem(OrderItemId) ON DELETE SET NULL
```

### **Collation Settings**
```sql
-- ✅ ĐÚNG - Vietnamese_CI_AS cho text fields
ALTER TABLE [User] ALTER COLUMN FullName NVARCHAR(255) COLLATE Vietnamese_CI_AS;
ALTER TABLE [User] ALTER COLUMN Email NVARCHAR(255) COLLATE Vietnamese_CI_AS;
```

---

## ⚠️ QUY TẮC ERROR HANDLING

### **Backend Error Handling**
```csharp
// ✅ ĐÚNG - Comprehensive error handling
[HttpPost]
[Authorize]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
{
    try
    {
        var userId = GetUserIdFromToken();
        Console.WriteLine($"Extracted userId from token: {userId}");
        
        if (userId == null)
            return Unauthorized(new { message = "Token không hợp lệ" });

        // Validate user exists in database
        var userExists = await _orderService.ValidateUserExistsAsync(userId.Value);
        if (!userExists)
            return Unauthorized(new { message = "Người dùng không tồn tại trong hệ thống" });

        // Business logic here...
        
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating order: {ex}");
        return BadRequest(new { message = "Có lỗi xảy ra khi tạo order", error = ex.Message });
    }
}
```

### **Frontend Error Handling**
```javascript
// ✅ ĐÚNG - Comprehensive error handling
const handleCreateOrder = async (e) => {
    try {
        setCreatingOrder(true);
        setError(null);
        
        const orderData = {
            eventId: parseInt(id),
            ticketTypeId: parseInt(selectedTicketType),
            quantity: quantity,
            seatNo: null
        };
        
        const response = await ordersAPI.create(orderData);
        setOrderSuccess(true);
        
    } catch (error) {
        console.error('Error creating order:', error);
        
        let errorMessage = 'Có lỗi xảy ra khi tạo đơn hàng';
        if (error.message) {
            errorMessage = error.message;
        } else if (error.response?.data?.message) {
            errorMessage = error.response.data.message;
        }
        
        setError(errorMessage);
    } finally {
        setCreatingOrder(false);
    }
};
```

---

## 🏗️ QUY TẮC CODE STRUCTURE

### **Service Layer Pattern**
```csharp
// ✅ ĐÚNG - Service interface
public interface IOrderService
{
    Task<CreateOrderResponseDTO> CreateOrderAsync(CreateOrderRequestDTO request, int customerId);
    Task<bool> ValidateUserExistsAsync(int userId);
}

// ✅ ĐÚNG - Service implementation
public class OrderService : IOrderService
{
    public async Task<bool> ValidateUserExistsAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error validating user: {ex.Message}", ex);
        }
    }
}
```

### **Repository Pattern**
```csharp
// ✅ ĐÚNG - Repository with proper data loading
public async Task<Order> CreateOrderAsync(Order order)
{
    try
    {
        await _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        // Load related data explicitly
        await _context.Entry(order)
            .Reference(o => o.Customer)
            .LoadAsync();
            
        await _context.Entry(order)
            .Collection(o => o.OrderItems)
            .LoadAsync();
            
        return order;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error creating order: {ex.Message}", ex);
    }
}
```

---

## 🔗 QUY TẮC FRONTEND-BACKEND INTEGRATION

### **Data Flow Consistency**
```javascript
// ✅ ĐÚNG - Frontend sends camelCase
const orderData = {
    eventId: parseInt(id),
    ticketTypeId: parseInt(selectedTicketType),
    quantity: quantity
};

// ✅ ĐÚNG - Backend receives and processes
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
{
    // request.EventId, request.TicketTypeId, request.Quantity
}

// ✅ ĐÚNG - Backend responds with camelCase
return Ok(new {
    order = new {
        orderId = createdOrder.OrderId,
        amount = createdOrder.Amount,
        status = createdOrder.Status
    }
});
```

### **Authentication Flow**
```javascript
// ✅ ĐÚNG - Frontend handles auth consistently
const { login, user } = useAuth();

const handleLogin = async (email, password) => {
    const result = await login(email, password);
    if (result.success) {
        // user object contains: userId, fullName, email, phone, role, walletBalance
        navigate('/dashboard');
    }
};
```

---

## 📝 QUY TẮC VALIDATION

### **Backend Validation**
```csharp
// ✅ ĐÚNG - Model validation
public partial class User 
{
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string FullName { get; set; }
    
    [Required]
    public string Email { get; set; }
}
```

### **Frontend Validation**
```javascript
// ✅ ĐÚNG - Form validation
const validateForm = (formData) => {
    const errors = {};
    
    if (!formData.eventId || isNaN(parseInt(formData.eventId))) {
        errors.eventId = 'ID sự kiện không hợp lệ';
    }
    
    if (!formData.quantity || formData.quantity <= 0) {
        errors.quantity = 'Số lượng vé phải lớn hơn 0';
    }
    
    return { isValid: Object.keys(errors).length === 0, errors };
};
```

---

## 🚀 QUY TẮC DEPLOYMENT

### **Database Migration**
```sql
-- ✅ ĐÚNG - Reset identity after schema changes
DBCC CHECKIDENT ('[User]', RESEED, 0);
DBCC CHECKIDENT ('Event', RESEED, 0);
DBCC CHECKIDENT ('[Order]', RESEED, 0);
```

### **Build Process**
```bash
# ✅ ĐÚNG - Build sequence
cd src
dotnet build
dotnet run
```

---

## ⚡ QUY TẮC PERFORMANCE

### **Database Queries**
```csharp
// ✅ ĐÚNG - Explicit loading to prevent N+1 queries
await _context.Entry(createdOrder)
    .Collection(o => o.OrderItems)
    .LoadAsync();
    
if (createdOrder.OrderItems.Any())
{
    var orderItem = createdOrder.OrderItems.First();
    await _context.Entry(orderItem)
        .Reference(oi => oi.TicketType)
        .LoadAsync();
}
```

### **Frontend Optimization**
```javascript
// ✅ ĐÚNG - Proper state management
const [loading, setLoading] = useState(false);
const [error, setError] = useState(null);

// ✅ ĐÚNG - Cleanup on unmount
useEffect(() => {
    return () => {
        // Cleanup logic
    };
}, []);
```

---

## 🔍 QUY TẮC DEBUGGING

### **Logging Standards**
```csharp
// ✅ ĐÚNG - Comprehensive logging
Console.WriteLine($"Extracted userId from token: {userId}");
Console.WriteLine($"User {userId.Value} exists in database: {userExists}");
Console.WriteLine($"Error creating order: {ex}");
```

### **Error Messages**
```csharp
// ✅ ĐÚNG - User-friendly error messages
return BadRequest(new { message = "Có lỗi xảy ra khi tạo order", error = ex.Message });
return Unauthorized(new { message = "Token không hợp lệ" });
return NotFound(new { message = "Không tìm thấy user" });
```

---

## 📋 CHECKLIST TRƯỚC KHI COMMIT

### **Backend Checklist**
- [ ] Tất cả API responses sử dụng camelCase
- [ ] Error handling đầy đủ với try-catch
- [ ] Logging được thêm vào các method quan trọng
- [ ] Validation được thêm vào models
- [ ] Database schema có IDENTITY(1,1) và constraints đúng
- [ ] Build thành công không có lỗi

### **Frontend Checklist**
- [ ] Tất cả API calls sử dụng camelCase
- [ ] Error handling đầy đủ với try-catch
- [ ] Form validation được implement
- [ ] State management đúng cách
- [ ] Cleanup logic được thêm vào useEffect

### **Integration Checklist**
- [ ] Frontend và Backend sử dụng cùng naming convention
- [ ] API responses match frontend expectations
- [ ] Database schema matches backend models
- [ ] Error messages consistent across layers
- [ ] Authentication flow works end-to-end

---

## 🎯 TÓM TẮT QUAN TRỌNG

1. **Naming**: Backend PascalCase, Frontend camelCase, Database PascalCase
2. **API**: Luôn sử dụng camelCase cho JSON responses
3. **Database**: IDENTITY(1,1), standardized lengths, proper constraints
4. **Error Handling**: Comprehensive try-catch, user-friendly messages
5. **Integration**: Consistent data flow between frontend and backend
6. **Validation**: Both client-side and server-side validation
7. **Logging**: Debug information for troubleshooting
8. **Performance**: Explicit loading, proper state management

**Lưu ý**: Tuân thủ nghiêm ngặt các quy tắc này để đảm bảo tính nhất quán và dễ bảo trì của hệ thống.
