# 🎫 CREATE ORDER FLOW

## 🔄 **LUỒNG CHÍNH**

```mermaid
sequenceDiagram
    participant User as 👤 USER
    participant Frontend as 🖥️ FRONTEND
    participant Controller as 🎮 ORDER CONTROLLER
    participant Service as ⚙️ ORDER SERVICE
    participant Repository as 🗄️ ORDER REPOSITORY
    participant Database as 💾 DATABASE

    User->>Frontend: 1. Chọn event, ticket type, nhập quantity
    Frontend->>Controller: 2. POST /api/order + JWT token
    
    Controller->>Controller: 3. Validate JWT + request data
    Controller->>Service: 4. CreateOrderAsync(request, userId)
    
    Service->>Database: 5. Get TicketType info
    Service->>Service: 6. Calculate total amount
    Service->>Repository: 7. CreateOrderAsync(order)
    
    Repository->>Database: 8. Save Order + OrderItems
    Repository-->>Service: 9. Created Order entity
    
    Service-->>Controller: 10. CreateOrderResponseDTO
    Controller-->>Frontend: 11. 200 OK + order data
    Frontend-->>User: 12. Hiển thị "Order created successfully"
```

## 🏗️ **KIẾN TRÚC**

```mermaid
graph TD
    FRONTEND["🖥️ FRONTEND<br/>CreateOrderPage.jsx"]
    CONTROLLER["🎮 ORDER CONTROLLER<br/>POST /api/order<br/>Validate JWT + data"]
    SERVICE["⚙️ ORDER SERVICE<br/>Business logic<br/>Calculate amount"]
    REPOSITORY["🗄️ ORDER REPOSITORY<br/>Save to database"]
    DATABASE["💾 DATABASE<br/>Orders, OrderItems, TicketTypes"]
    
    FRONTEND --> CONTROLLER
    CONTROLLER --> SERVICE
    SERVICE --> REPOSITORY
    REPOSITORY --> DATABASE
    
    style FRONTEND fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    style CONTROLLER fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style SERVICE fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style REPOSITORY fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style DATABASE fill:#c8e6c9,stroke:#2e7d32,stroke-width:3px
```

## 📋 **CÁC BƯỚC CHÍNH**

### **1. Frontend**
- User chọn event và ticket type
- Nhập quantity, seat number
- Gọi API với JWT token

### **2. Controller**
- Validate JWT token
- Extract userId từ token
- Validate request data
- Gọi OrderService

### **3. Service**
- Validate quantity > 0
- Get TicketType từ database
- Calculate: totalAmount = price × quantity
- Create Order entity
- Gọi OrderRepository

### **4. Repository**
- Set timestamps và status = "Pending"
- Save Order và OrderItems
- Load related data
- Return Order entity

### **5. Response**
- Map Order thành CreateOrderResponseDTO
- Return success response với order data

## 🛡️ **ERROR HANDLING**

- **401**: JWT token không hợp lệ
- **400**: Request data không hợp lệ
- **400**: TicketType không tồn tại
- **400**: Event không tồn tại

## 🎯 **MINDSET**

**"Tôi cần tới cái gì, tôi làm tới cái đó đi":**
- Controller: Xử lý HTTP request
- Service: Xử lý business logic
- Repository: Lưu dữ liệu
- Mapper: Chuyển đổi dữ liệu
