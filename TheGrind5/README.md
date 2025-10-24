# TheGrind5 Event Management System

Hệ thống quản lý sự kiện TheGrind5 với đầy đủ tính năng đặt vé, thanh toán và quản lý phụ kiện.

## 🚀 Tính năng chính

### Frontend (React)
- **Trang chủ**: Hiển thị danh sách sự kiện
- **Chi tiết sự kiện**: Thông tin đầy đủ về sự kiện
- **Đặt vé**: Chọn loại vé và phụ kiện
- **Thanh toán**: Hỗ trợ ví điện tử và voucher
- **Vé của tôi**: Quản lý vé đã mua
- **Wishlist**: Lưu sự kiện yêu thích

### Backend (ASP.NET Core)
- **API RESTful**: Đầy đủ endpoints cho tất cả tính năng
- **Authentication**: JWT token-based
- **Database**: SQL Server với Entity Framework Core
- **File Upload**: Quản lý hình ảnh sự kiện
- **Email Service**: Gửi OTP và thông báo

## 🛠️ Công nghệ sử dụng

### Frontend
- React 18
- Material-UI (MUI)
- React Router
- Axios
- Context API

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- AutoMapper

## 📁 Cấu trúc project

```
TheGrind5/
├── TheGrind5_EventManagement_BackEnd/     # Backend API
│   ├── src/
│   │   ├── Controllers/                   # API Controllers
│   │   ├── Services/                      # Business Logic
│   │   ├── Models/                       # Data Models
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Repositories/                 # Data Access Layer
│   │   └── Data/                         # Database Context
│   └── SampleData_Insert.sql             # Sample Data
├── TheGrind5_EventManagement_FrontEnd/   # Frontend React
│   ├── src/
│   │   ├── components/                   # React Components
│   │   ├── pages/                        # Page Components
│   │   ├── services/                     # API Services
│   │   └── contexts/                     # React Contexts
│   └── public/                           # Static Files
└── README.md
```

## 🚀 Cách chạy

### Backend
```bash
cd TheGrind5_EventManagement_BackEnd/src
dotnet run
```

### Frontend
```bash
cd TheGrind5_EventManagement_FrontEnd
npm install
npm start
```

## 📝 Tính năng mới

### ✅ Hỗ trợ phụ kiện trong đơn hàng
- **Frontend**: Hiển thị phụ kiện trong trang đặt vé
- **Backend**: Lưu trữ phụ kiện trong database
- **Tính toán**: Tổng cộng = giá vé + giá phụ kiện

### ✅ Cập nhật hiển thị giá tiền
- **MyTicketsPage**: Hiển thị tổng cộng bao gồm phụ kiện
- **PaymentPage**: Chi tiết giá vé và phụ kiện
- **Ticket Detail**: Thông tin đầy đủ về phụ kiện đi kèm

## 🔧 Database Schema

### Bảng mới: OrderProduct
```sql
CREATE TABLE OrderProduct (
    OrderProductId int IDENTITY(1,1) PRIMARY KEY,
    OrderId int NOT NULL,
    ProductId int NOT NULL,
    Quantity int NOT NULL,
    Price decimal(18,2) NOT NULL,
    TotalPrice decimal(18,2) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
    FOREIGN KEY (ProductId) REFERENCES Product(ProductId)
);
```

## 📊 API Endpoints

### Tickets
- `GET /api/Ticket/my-tickets` - Lấy danh sách vé của user
- `GET /api/Ticket/{id}` - Lấy chi tiết vé
- `POST /api/Ticket/checkin` - Check-in vé

### Orders
- `POST /api/Order` - Tạo đơn hàng mới (bao gồm phụ kiện)
- `GET /api/Order/{id}` - Lấy chi tiết đơn hàng

### Products
- `GET /api/Product/event/{eventId}` - Lấy phụ kiện của sự kiện
- `POST /api/Product` - Tạo phụ kiện mới

## 🎯 Kết quả

Sau khi cập nhật, hệ thống sẽ:
- ✅ Hiển thị đúng tổng cộng (vé + phụ kiện)
- ✅ Lưu trữ thông tin phụ kiện trong database
- ✅ Hiển thị phụ kiện trong trang vé của tôi
- ✅ Tính toán chính xác giá tiền

## 👥 Contributors

- **Thien**: Full-stack development
- **AI Assistant**: Code review và optimization

## 📄 License

MIT License - Xem file LICENSE để biết thêm chi tiết.
