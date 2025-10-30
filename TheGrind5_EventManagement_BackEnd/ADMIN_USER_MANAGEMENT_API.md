# 🔐 Admin User Management API Documentation

## Mô tả
API cho phép Admin quản lý danh sách người dùng (Users, Hosts, Customers) trong hệ thống.

## Authentication
**Tất cả endpoints đều yêu cầu JWT token với role "Admin"**

```
Authorization: Bearer <your-jwt-token>
```

---

## 📋 API Endpoints

### 1. Lấy danh sách tất cả users
**GET** `/api/admin/users`

#### Query Parameters:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| role | string | No | null | Filter theo role: "Host", "Customer", "Admin" |
| searchTerm | string | No | null | Tìm kiếm theo username, email, fullname |
| pageNumber | int | No | 1 | Số trang (bắt đầu từ 1) |
| pageSize | int | No | 10 | Số items mỗi trang (max 100) |
| sortBy | string | No | "CreatedAt" | Sắp xếp theo: CreatedAt, FullName, Email, WalletBalance, Role |
| sortOrder | string | No | "desc" | Thứ tự: "asc" hoặc "desc" |

#### Example Request:
```bash
GET /api/admin/users?role=Host&searchTerm=nguyen&pageNumber=1&pageSize=10&sortBy=CreatedAt&sortOrder=desc
```

#### Example Response:
```json
{
  "success": true,
  "message": "Lấy danh sách người dùng thành công",
  "data": {
    "users": [
      {
        "userId": 2,
        "username": "host1",
        "fullName": "Nguyễn Văn Host",
        "email": "host1@example.com",
        "phone": "0123456789",
        "role": "Host",
        "walletBalance": 0.00,
        "createdAt": "2025-10-29T03:00:00Z",
        "updatedAt": "2025-10-29T03:00:00Z",
        "avatar": null,
        "dateOfBirth": "1985-03-15T00:00:00Z",
        "gender": "Nam"
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

---

### 2. Lấy thông tin chi tiết một user
**GET** `/api/admin/users/{userId}`

#### Path Parameters:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| userId | int | Yes | ID của user |

#### Example Request:
```bash
GET /api/admin/users/2
```

#### Example Response:
```json
{
  "success": true,
  "message": "Lấy thông tin người dùng thành công",
  "data": {
    "userId": 2,
    "username": "host1",
    "fullName": "Nguyễn Văn Host",
    "email": "host1@example.com",
    "phone": "0123456789",
    "role": "Host",
    "walletBalance": 0.00,
    "createdAt": "2025-10-29T03:00:00Z",
    "updatedAt": "2025-10-29T03:00:00Z",
    "avatar": null,
    "dateOfBirth": "1985-03-15T00:00:00Z",
    "gender": "Nam"
  }
}
```

---

### 3. Lấy thống kê tổng quan
**GET** `/api/admin/statistics`

#### Example Request:
```bash
GET /api/admin/statistics
```

#### Example Response:
```json
{
  "success": true,
  "message": "Lấy thống kê thành công",
  "data": {
    "totalUsers": 6,
    "totalHosts": 2,
    "totalCustomers": 3,
    "totalAdmins": 1,
    "newUsersThisMonth": 6,
    "totalWalletBalance": 2749999.99
  }
}
```

---

### 4. Lấy danh sách Hosts
**GET** `/api/admin/hosts`

#### Query Parameters:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| searchTerm | string | No | null | Tìm kiếm theo username, email, fullname |
| pageNumber | int | No | 1 | Số trang |
| pageSize | int | No | 10 | Số items mỗi trang |

#### Example Request:
```bash
GET /api/admin/hosts?searchTerm=nguyen&pageNumber=1&pageSize=10
```

#### Example Response:
```json
{
  "success": true,
  "message": "Lấy danh sách Host thành công",
  "data": {
    "users": [
      {
        "userId": 2,
        "username": "host1",
        "fullName": "Nguyễn Văn Host",
        "email": "host1@example.com",
        "phone": "0123456789",
        "role": "Host",
        "walletBalance": 0.00,
        "createdAt": "2025-10-29T03:00:00Z",
        "updatedAt": "2025-10-29T03:00:00Z",
        "avatar": null,
        "dateOfBirth": "1985-03-15T00:00:00Z",
        "gender": "Nam"
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

---

### 5. Lấy danh sách Customers
**GET** `/api/admin/customers`

#### Query Parameters:
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| searchTerm | string | No | null | Tìm kiếm theo username, email, fullname |
| pageNumber | int | No | 1 | Số trang |
| pageSize | int | No | 10 | Số items mỗi trang |

#### Example Request:
```bash
GET /api/admin/customers?pageNumber=1&pageSize=10
```

#### Example Response:
```json
{
  "success": true,
  "message": "Lấy danh sách Customer thành công",
  "data": {
    "users": [
      {
        "userId": 4,
        "username": "customer1",
        "fullName": "Lê Văn Customer",
        "email": "customer1@example.com",
        "phone": "0555123456",
        "role": "Customer",
        "walletBalance": 500000.00,
        "createdAt": "2025-10-29T03:00:00Z",
        "updatedAt": "2025-10-29T03:00:00Z",
        "avatar": null,
        "dateOfBirth": "1992-11-08T00:00:00Z",
        "gender": "Nam"
      }
    ],
    "totalCount": 3,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

---

## 🔒 Error Responses

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 403 Forbidden (Không phải Admin)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Không tìm thấy người dùng"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Có lỗi xảy ra khi lấy danh sách người dùng",
  "error": "Error details..."
}
```

---

## 📝 Use Cases

### 1. Admin xem tổng quan dashboard
```
GET /api/admin/statistics
```
→ Hiển thị tổng số users, hosts, customers, admins, user mới tháng này, tổng wallet balance

### 2. Admin xem tất cả users
```
GET /api/admin/users?pageNumber=1&pageSize=20
```
→ Hiển thị tất cả users với pagination

### 3. Admin tìm kiếm user theo tên
```
GET /api/admin/users?searchTerm=nguyen&pageNumber=1&pageSize=10
```
→ Tìm kiếm users có "nguyen" trong username, email hoặc fullname

### 4. Admin xem danh sách Hosts
```
GET /api/admin/hosts?pageNumber=1&pageSize=10
```
→ Chỉ hiển thị users có role "Host"

### 5. Admin xem danh sách Customers
```
GET /api/admin/customers?pageNumber=1&pageSize=10
```
→ Chỉ hiển thị users có role "Customer"

### 6. Admin xem chi tiết một user
```
GET /api/admin/users/5
```
→ Hiển thị đầy đủ thông tin của user có ID = 5

---

## 🎯 Features

✅ **Pagination** - Hỗ trợ phân trang với pageNumber và pageSize
✅ **Filter by Role** - Lọc theo role (Host, Customer, Admin)
✅ **Search** - Tìm kiếm theo username, email, fullname
✅ **Sort** - Sắp xếp theo nhiều trường (CreatedAt, FullName, Email, WalletBalance, Role)
✅ **Statistics** - Thống kê tổng quan về users
✅ **Authorization** - Chỉ Admin mới có quyền truy cập
✅ **Logging** - Ghi log mọi hành động của Admin

---

## 🧪 Test với Admin account

**Login với Admin:**
```bash
POST /api/auth/login
{
  "email": "admin@thegrind5.com",
  "password": "123456"
}
```

**Sau đó dùng token để gọi Admin APIs:**
```bash
GET /api/admin/users
Authorization: Bearer <token-từ-login>
```

---

## 📚 Database Schema

```sql
User Table:
- UserId (PK)
- Username
- FullName
- Email
- Phone
- Role (Customer, Host, Admin)
- WalletBalance
- CreatedAt
- UpdatedAt
- Avatar
- DateOfBirth
- Gender
```

---

## ✨ Tóm tắt

Hệ thống Admin User Management cung cấp đầy đủ các chức năng:
- Xem danh sách tất cả users với filter, search, pagination
- Xem chi tiết từng user
- Xem thống kê tổng quan
- Xem danh sách riêng cho Hosts và Customers
- Bảo mật với JWT và Role-based Authorization

Tất cả APIs đều CHẠY ĐƯỢC, ĐƠNGIẢN, và tuân thủ KISS principle! 🎉

