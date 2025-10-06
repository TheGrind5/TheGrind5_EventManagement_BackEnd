# Hướng dẫn tích hợp Frontend và Backend

## Tổng quan
Project này đã được tích hợp hoàn chỉnh giữa Frontend (React) và Backend (.NET Core API).

## Cấu trúc Backend

### Controllers
- **AuthController**: Xử lý đăng nhập/đăng ký
  - `POST /api/auth/login` - Đăng nhập
  - `POST /api/auth/register` - Đăng ký
  - `GET /api/auth/me` - Lấy thông tin user hiện tại

- **EventController**: Quản lý sự kiện
  - `GET /api/event` - Lấy danh sách sự kiện
  - `GET /api/event/{id}` - Lấy chi tiết sự kiện
  - `POST /api/event` - Tạo sự kiện mới
  - `PUT /api/event/{id}` - Cập nhật sự kiện
  - `DELETE /api/event/{id}` - Xóa sự kiện

- **UserController**: Quản lý người dùng
  - `GET /api/user` - Lấy danh sách user

### Services
- **AuthService**: Xử lý authentication logic
- **SeedService**: Tạo dữ liệu mẫu (admin user)
- **EventSeedService**: Tạo dữ liệu mẫu (events và ticket types)

### Models
- **User**: Thông tin người dùng (đã thêm Username field)
- **Event**: Thông tin sự kiện
- **TicketType**: Loại vé
- **Order, OrderItem, Payment, Ticket**: Quản lý đơn hàng và thanh toán

## Cấu trúc Frontend

### API Layer
- **auth.js**: API calls cho authentication
- **events.js**: API calls cho events

### Components
- **AuthContext**: Quản lý trạng thái authentication
- **LoginForm/RegisterForm**: Form đăng nhập/đăng ký
- **ProtectedRoute**: Bảo vệ routes cần authentication

### Pages
- **GuestLandingPage**: Trang chủ cho khách
- **CustomerDashboard**: Dashboard cho user đã đăng nhập
- **EventDetailsPage**: Chi tiết sự kiện

## Cách chạy

### Backend
```bash
cd TheGrind5_EventManagement_BackEnd
dotnet run
```
Backend sẽ chạy trên `http://localhost:5000`

### Frontend
```bash
cd TheGrind5_EventManagement_FrontEnd
npm install
npm start
```
Frontend sẽ chạy trên `http://localhost:5173`

## Dữ liệu mẫu

### Admin User
- Email: `admin@test.com`
- Password: `admin123`
- Username: `admin`

### Sample Events
- WATERSOME 2025 (Music)
- EXSH Concert (Music)  
- TRANG TRANG A (Art)

## API Endpoints

### Authentication
```
POST /api/auth/login
Body: { "email": "admin@test.com", "password": "admin123" }
Response: { "accessToken": "...", "expiresAt": "...", "user": {...} }

POST /api/auth/register
Body: { "username": "test", "email": "test@test.com", "password": "123456", "fullName": "Test User" }
Response: { "userId": 1, "fullName": "Test User", "email": "test@test.com", "phone": "", "role": "Customer" }
```

### Events
```
GET /api/event
Response: [{ "eventId": 1, "title": "WATERSOME 2025", ... }]

GET /api/event/1
Response: { "eventId": 1, "title": "WATERSOME 2025", "ticketTypes": [...] }
```

## Tính năng đã tích hợp

✅ **Authentication System**
- Đăng nhập/đăng ký
- JWT token (simple implementation)
- Protected routes

✅ **Event Management**
- Hiển thị danh sách sự kiện
- Chi tiết sự kiện
- API integration

✅ **Responsive UI**
- Modern design
- Loading states
- Error handling

✅ **Database Integration**
- Entity Framework Core
- In-Memory Database
- Seed data

## Lưu ý

1. **Database**: Hiện tại sử dụng In-Memory Database, dữ liệu sẽ mất khi restart
2. **Authentication**: Sử dụng simple token, chưa implement JWT đầy đủ
3. **CORS**: Đã cấu hình cho frontend localhost:5173
4. **Error Handling**: Có fallback data khi API fails

## Phát triển tiếp

1. **JWT Authentication**: Implement JWT token đầy đủ
2. **Database**: Chuyển sang SQL Server/PostgreSQL
3. **File Upload**: Upload hình ảnh cho events
4. **Payment**: Tích hợp thanh toán
5. **Email**: Gửi email xác nhận
6. **Real-time**: SignalR cho notifications
