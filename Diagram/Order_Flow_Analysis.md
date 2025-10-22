# PHÂN TÍCH LUỒNG ORDER - THEGRIND5 EVENT MANAGEMENT

## TỔNG QUAN LUỒNG ORDER

Luồng order trong hệ thống TheGrind5 Event Management chạy từ **Frontend (React)** đến **Backend (ASP.NET Core)** qua các lớp kiến trúc khác nhau.

## 1. FRONTEND LAYER (React)

### 1.1 CreateOrderPage.jsx
- **Vai trò**: Component chính để tạo order
- **Chức năng**:
  - Hiển thị form tạo order
  - Validate dữ liệu đầu vào
  - Gọi API tạo order
  - Xử lý response và redirect

### 1.2 API Service Layer
- **File**: `src/services/api.js`
- **Chức năng**: 
  - `ordersAPI.create(orderData)` - Gọi API tạo order
  - Xử lý authentication headers
  - Error handling

## 2. BACKEND LAYER (ASP.NET Core)

### 2.1 Controller Layer
**File**: `Controllers/OrderController.cs`
- **Endpoint**: `POST /api/Order`
- **Chức năng**:
  - Nhận request từ frontend
  - Validate authentication
  - Gọi business service
  - Trả về response

### 2.2 Business Service Layer
**File**: `Services/OrderService.cs`
- **Interface**: `Business/IOrderService.cs`
- **Chức năng**:
  - Business logic validation
  - Kiểm tra inventory
  - Tính toán giá
  - Gọi repository

### 2.3 Repository Layer
**File**: `Repositories/OrderRepository.cs`
- **Interface**: `Repositories/IOrderRepository.cs`
- **Chức năng**:
  - Database operations
  - CRUD operations
  - Entity Framework operations

### 2.4 Mapper Layer
**File**: `Mappers/OrderMapper.cs`
- **Chức năng**:
  - Convert giữa DTOs và Models
  - Map request/response objects

## 3. CHI TIẾT LUỒNG XỬ LÝ

### 3.1 Frontend → Backend Flow

```
1. User nhập thông tin order trong CreateOrderPage.jsx
   ↓
2. handleCreateOrder() validate dữ liệu
   ↓
3. ordersAPI.create(orderData) gọi API
   ↓
4. HTTP POST request đến /api/Order
   ↓
5. OrderController.CreateOrder() nhận request
   ↓
6. Validate authentication & user
   ↓
7. OrderService.CreateOrderAsync() xử lý business logic
   ↓
8. OrderRepository.CreateOrderAsync() lưu database
   ↓
9. OrderMapper.MapToCreateOrderResponse() convert response
   ↓
10. Trả về JSON response cho frontend
```

### 3.2 Business Logic Validation

**OrderService.CreateOrderAsync()** thực hiện các validation:

1. **Basic Validation**:
   - Quantity > 0
   - TicketTypeId tồn tại
   - Event tồn tại

2. **Business Rules**:
   - Event status = "Open"
   - TicketType status = "Active"
   - Kiểm tra thời gian bán vé (SaleStart/SaleEnd)
   - Kiểm tra MinOrder/MaxOrder
   - Kiểm tra số lượng vé còn lại

3. **Price Calculation**:
   - Tính unitPrice từ TicketType.Price
   - Tính totalAmount = unitPrice * quantity

4. **Database Transaction**:
   - Begin transaction
   - Tạo Order entity
   - Tạo OrderItem entity
   - Commit transaction

### 3.3 Database Operations

**OrderRepository.CreateOrderAsync()**:
1. Set order properties (CustomerId, Amount, Status="Pending")
2. Add Order to context
3. Save changes
4. Load related data (Customer, OrderItems, TicketType, Event)

## 4. CÁC LỚP KIẾN TRÚC

### 4.1 Presentation Layer
- **CreateOrderPage.jsx**: UI component
- **api.js**: API service functions

### 4.2 Application Layer
- **OrderController.cs**: API endpoints
- **OrderService.cs**: Business logic

### 4.3 Domain Layer
- **Models/Order.cs**: Domain entity
- **DTOs/**: Data transfer objects
- **Business/IOrderService.cs**: Business interface

### 4.4 Infrastructure Layer
- **OrderRepository.cs**: Data access
- **OrderMapper.cs**: Object mapping
- **EventDBContext.cs**: Database context

## 5. DATA FLOW

### 5.1 Request Flow
```
Frontend Form Data
    ↓
CreateOrderRequestDTO
    ↓
OrderController.CreateOrder()
    ↓
OrderService.CreateOrderAsync()
    ↓
OrderMapper.MapFromCreateOrderRequest()
    ↓
OrderRepository.CreateOrderAsync()
    ↓
Database (Orders table)
```

### 5.2 Response Flow
```
Database (Orders table)
    ↓
OrderRepository.CreateOrderAsync()
    ↓
OrderMapper.MapToCreateOrderResponse()
    ↓
OrderService.CreateOrderAsync()
    ↓
OrderController.CreateOrder()
    ↓
JSON Response
    ↓
Frontend (CreateOrderPage.jsx)
```

## 6. ERROR HANDLING

### 6.1 Frontend Error Handling
- Try-catch trong handleCreateOrder()
- Display error messages
- Redirect to login nếu unauthorized

### 6.2 Backend Error Handling
- Controller: Basic validation
- Service: Business rule validation
- Repository: Database error handling
- Transaction rollback nếu có lỗi

## 7. SECURITY & VALIDATION

### 7.1 Authentication
- JWT token validation
- User authorization check
- User existence validation

### 7.2 Data Validation
- Input sanitization
- Business rule enforcement
- Database constraints

## 8. PERFORMANCE CONSIDERATIONS

### 8.1 Database Optimization
- Entity Framework Include() để load related data
- Transaction scope optimization
- Async/await pattern

### 8.2 Frontend Optimization
- Loading states
- Error boundaries
- Form validation

## KẾT LUẬN

Luồng order trong hệ thống TheGrind5 được thiết kế theo kiến trúc **Clean Architecture** với các lớp rõ ràng:

1. **Frontend**: React components + API services
2. **Backend**: Controller → Service → Repository → Database
3. **Mapping**: DTOs ↔ Models conversion
4. **Validation**: Multi-layer validation (UI → Business → Database)

Luồng này đảm bảo tính nhất quán, bảo mật và hiệu suất cao cho việc xử lý order trong hệ thống event management.
