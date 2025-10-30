# 🎯 Admin Dashboard - Hướng Dẫn Sử Dụng

## 📋 Tổng Quan

Admin Dashboard được thiết kế theo phong cách [Volt React Dashboard](https://themewagon.github.io/volt-react-dashboard), cung cấp giao diện quản trị đẹp và hiện đại cho TheGrind5 Event Management System.

---

## 🚀 Tính Năng Đã Hoàn Thành

### ✅ Backend API
- **GET `/api/Auth/users`** - Lấy danh sách tất cả users (chỉ Admin)
  - Yêu cầu: Bearer Token với role Admin
  - Trả về: Danh sách users với đầy đủ thông tin

### ✅ Frontend Components

#### 1. **AdminSidebar** 
- Sidebar navigation đẹp với gradient background
- Menu items với icons Material-UI
- Active state highlighting
- Logout button

#### 2. **UserManagement**
- Hiển thị danh sách tất cả users
- Statistics cards (Tổng users, Admins, Hosts, Customers)
- Search functionality (tên, email, username, role)
- Beautiful table với avatar, role chip, wallet balance
- Responsive design

#### 3. **AdminDashboard**
- Layout container với sidebar và main content
- Routing cho các trang admin
- Protected routes chỉ cho Admin

---

## 🔐 Đăng Nhập Admin

### Tài Khoản Admin:
```
Email:    admin@thegrind5.com
Password: 123456
```

### Các Bước Đăng Nhập:

1. Mở trình duyệt: http://localhost:3000/login
2. Nhập email và password admin
3. Sau khi đăng nhập thành công, tự động redirect về `/admin/users`
4. Giao diện Admin Dashboard hiển thị với sidebar và danh sách users

---

## 📁 Cấu Trúc Files Đã Tạo

```
TheGrind5_EventManagement_FrontEnd/
├── src/
│   ├── components/
│   │   └── admin/
│   │       ├── AdminSidebar.jsx          # Sidebar navigation
│   │       ├── AdminSidebar.css
│   │       ├── UserManagement.jsx        # User management table
│   │       └── UserManagement.css
│   ├── pages/
│   │   ├── AdminDashboard.jsx            # Main admin page
│   │   └── AdminDashboard.css
│   └── services/
│       └── adminAPI.js                    # Admin API service

Backend/
├── src/
│   ├── Controllers/
│   │   └── AuthController.cs             # Added GetAllUsers endpoint
│   └── Repositories/
│       ├── IUserRepository.cs            # Added GetAllUsersAsync
│       └── UserRepository.cs             # Implemented GetAllUsersAsync

Root/
└── CreateAdminAccount.sql                 # SQL script tạo admin
```

---

## 🎨 Giao Diện Admin Dashboard

### Color Scheme (Volt-inspired):
- **Primary**: `#6C5DD3` (Purple)
- **Background**: `#262B40` (Dark Blue)
- **Accent**: Gradient backgrounds
- **Cards**: White with shadow

### Components:

1. **Sidebar**:
   - Fixed position, 260px width
   - Dark gradient background
   - Smooth hover effects
   - Active indicator

2. **Stats Cards**:
   - 4 cards với gradient icons
   - Tổng users, Admins, Hosts, Customers
   - Hover animation

3. **User Table**:
   - Avatar display
   - Role chips với colors
   - Wallet balance formatting
   - Date formatting
   - Search functionality

---

## 🔄 User Flow

```
Admin Login
    ↓
Redirect to /admin/users
    ↓
View User Management Dashboard
    ├── See statistics
    ├── Search users
    ├── View user details
    └── Navigate to other admin pages (Coming soon)
```

---

## 🛠️ Cách Test

### 1. Restart Backend (nếu cần):
```bash
cd src
dotnet run --urls http://localhost:5000
```

### 2. Chạy Frontend (nếu chưa chạy):
```bash
cd TheGrind5_EventManagement_FrontEnd
npm start
```

### 3. Test Admin Login:
1. Mở http://localhost:3000/login
2. Login với `admin@thegrind5.com` / `123456`
3. Kiểm tra redirect về `/admin/users`
4. Xem danh sách users hiển thị đúng

### 4. Test Non-Admin Protection:
1. Logout
2. Login với user thường (customer1@example.com / 123456)
3. Thử truy cập http://localhost:3000/admin/users
4. Phải bị redirect về `/dashboard`

---

## 📊 API Endpoints

### Get All Users (Admin Only)
```http
GET /api/Auth/users
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "userId": 1,
      "username": "admin",
      "fullName": "Quản Trị Viên",
      "email": "admin@thegrind5.com",
      "phone": "0999999999",
      "role": "Admin",
      "walletBalance": 0,
      "avatar": null,
      "dateOfBirth": "1990-01-01",
      "gender": "Nam",
      "createdAt": "2025-10-30T03:07:10",
      "updatedAt": null
    }
    // ... more users
  ],
  "totalCount": 6
}
```

---

## 🎯 Features Roadmap

### ✅ Completed:
- [x] Admin authentication & authorization
- [x] Admin sidebar navigation
- [x] User management dashboard
- [x] User statistics
- [x] User search functionality

### 🚧 Coming Soon:
- [ ] Event Management (Admin view)
- [ ] Order Management
- [ ] Analytics Dashboard
- [ ] System Settings
- [ ] User Edit/Delete functionality
- [ ] Event Approval workflow
- [ ] Revenue reports

---

## 🐛 Troubleshooting

### Vấn đề: Admin không truy cập được dashboard
**Giải pháp**: 
- Kiểm tra role trong database: `SELECT * FROM [User] WHERE Email = 'admin@thegrind5.com'`
- Role phải là `'Admin'` (viết hoa chữ A)

### Vấn đề: 401 Unauthorized khi call API
**Giải pháp**:
- Kiểm tra token trong localStorage
- Logout và login lại
- Kiểm tra backend có running không

### Vấn đề: CSS không load đúng
**Giải pháp**:
- Clear browser cache
- Restart frontend server
- Check console errors

---

## 📝 Notes

- Admin Dashboard sử dụng Material-UI cho components
- Responsive design cho mobile/tablet
- Protected routes với role checking
- Clean code structure và naming conventions
- Ready for expansion với các features mới

---

## 🎉 Kết Luận

Admin Dashboard đã sẵn sàng sử dụng với giao diện đẹp, hiện đại và đầy đủ chức năng quản lý users. Hệ thống đã được tích hợp hoàn chỉnh từ Backend đến Frontend với authentication và authorization đầy đủ.

**Happy Coding! 🚀**

