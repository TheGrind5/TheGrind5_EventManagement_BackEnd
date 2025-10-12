# 🏗️ BACKEND ARCHITECTURE - MINDSET: "TÔI CẦN TỚI CÁI GÌ, TÔI LÀM TỚI CÁI ĐÓ ĐI"

## 🎯 MINDSET: "CẦN CÁI NÀO DÙNG CÁI NẤY"

### 📊 **DIAGRAM TỔNG HỢP - KIẾN TRÚC & LUỒNG HOẠT ĐỘNG**

```mermaid
graph TD
    %% ===== FRONTEND LAYER - TOP SECTION =====
    USER["USER INTERFACE<br/>Người dùng cuối<br/>Tương tác với ứng dụng"]
    
    REACT_APP["REACT APP - ENTRY POINT<br/>Tôi cần khởi động ứng dụng frontend<br/>Cần: React Router để điều hướng<br/>Cần: AuthContext để quản lý state<br/>Cần: API service để gọi backend<br/>Làm: Render App component<br/>Làm: Setup routing và context providers<br/>ĐỂ LÀM GÌ: Khởi động React app, setup routing và state management"]
    
    %% ===== FRONTEND SERVICES =====
    AUTH_CONTEXT["AUTH CONTEXT - STATE MANAGEMENT<br/>Tôi cần quản lý authentication state<br/>Cần: React Context API<br/>Cần: localStorage để persist token<br/>Làm: Store user info và token<br/>Làm: Provide login/logout/register functions<br/>Làm: Check authentication status<br/>Làm: Handle loading states<br/>ĐỂ LÀM GÌ: Quản lý trạng thái đăng nhập cho toàn bộ app"]
    
    API_SERVICE["API SERVICE - HTTP CLIENT<br/>Tôi cần giao tiếp với backend API<br/>Cần: Fetch API để gửi HTTP requests<br/>Cần: JWT token để authentication<br/>Làm: Setup base URL và headers<br/>Làm: Handle authentication tokens<br/>Làm: Handle API responses và errors<br/>Làm: authAPI - login, register, getUser<br/>Làm: eventsAPI - CRUD operations<br/>ĐỂ LÀM GÌ: Cung cấp interface để frontend gọi backend API"]
    
    %% ===== FRONTEND COMPONENTS =====
    HEADER["HEADER COMPONENT<br/>Tôi cần hiển thị navigation bar<br/>Cần: AuthContext để check login status<br/>Cần: React Router để navigate<br/>Làm: Hiển thị logo và menu<br/>Làm: Show/hide login/logout buttons<br/>Làm: Navigate giữa các pages<br/>ĐỂ LÀM GÌ: Cung cấp navigation cho user trong toàn bộ app"]
    
    PROTECTED_ROUTE["PROTECTED ROUTE COMPONENT<br/>Tôi cần bảo vệ các routes yêu cầu đăng nhập<br/>Cần: AuthContext để check authentication<br/>Cần: React Router để redirect<br/>Làm: Check user đã login chưa<br/>Làm: Redirect đến login nếu chưa login<br/>Làm: Render protected content nếu đã login<br/>ĐỂ LÀM GÌ: Đảm bảo chỉ user đã đăng nhập mới truy cập được protected pages"]
    
    %% ===== FRONTEND PAGES - AUTHENTICATION FLOW =====
    LOGIN_PAGE["LOGIN PAGE<br/>Tôi cần xử lý đăng nhập user<br/>Cần: AuthContext để quản lý login state<br/>Cần: API service để gọi login endpoint<br/>Làm: Validate form input<br/>Làm: Gọi API login<br/>Làm: Lưu token vào localStorage<br/>ĐỂ LÀM GÌ: Cho phép user đăng nhập vào hệ thống"]
    
    REGISTER_PAGE["REGISTER PAGE<br/>Tôi cần xử lý đăng ký user mới<br/>Cần: AuthContext để quản lý register state<br/>Cần: API service để gọi register endpoint<br/>Làm: Validate form input<br/>Làm: Gọi API register<br/>Làm: Redirect đến login page<br/>ĐỂ LÀM GÌ: Cho phép user tạo tài khoản mới"]
    
    %% ===== FRONTEND PAGES - MAIN APPLICATION FLOW =====
    HOME_PAGE["HOME PAGE<br/>Tôi cần hiển thị trang chủ<br/>Cần: API service để lấy danh sách events<br/>Làm: Fetch events từ backend<br/>Làm: Hiển thị danh sách events<br/>Làm: Navigate đến event details<br/>ĐỂ LÀM GÌ: Cho user xem danh sách sự kiện và chọn sự kiện"]
    
    EVENT_DETAILS_PAGE["EVENT DETAILS PAGE<br/>Tôi cần hiển thị chi tiết sự kiện<br/>Cần: API service để lấy event details<br/>Cần: AuthContext để check user permissions<br/>Làm: Fetch event details từ backend<br/>Làm: Hiển thị thông tin chi tiết<br/>Làm: Handle edit/delete nếu là host<br/>ĐỂ LÀM GÌ: Cho user xem chi tiết sự kiện và quản lý nếu có quyền"]
    
    DASHBOARD_PAGE["DASHBOARD PAGE<br/>Tôi cần hiển thị dashboard user<br/>Cần: AuthContext để lấy user info<br/>Cần: API service để lấy user events<br/>Làm: Fetch user profile<br/>Làm: Fetch user's events<br/>Làm: Hiển thị thống kê cá nhân<br/>ĐỂ LÀM GÌ: Cho user quản lý sự kiện của mình và xem thống kê"]
    
    %% ===== FRONTEND PAGES - ORDER MANAGEMENT FLOW =====
    ORDER_LIST_PAGE["ORDER LIST PAGE<br/>Tôi cần hiển thị danh sách orders của user<br/>Cần: AuthContext để lấy user info<br/>Cần: API service để lấy user orders<br/>Làm: Fetch user orders từ backend<br/>Làm: Hiển thị danh sách orders với status<br/>Làm: Navigate đến order details<br/>ĐỂ LÀM GÌ: Cho user xem lịch sử đặt vé và quản lý orders"]
    
    ORDER_DETAILS_PAGE["ORDER DETAILS PAGE<br/>Tôi cần hiển thị chi tiết 1 order<br/>Cần: API service để lấy order details<br/>Cần: AuthContext để check ownership<br/>Làm: Fetch order details từ backend<br/>Làm: Hiển thị thông tin order, items, payment<br/>Làm: Handle cancel order nếu chưa thanh toán<br/>ĐỂ LÀM GÌ: Cho user xem chi tiết đơn hàng và quản lý"]
    
    CREATE_ORDER_PAGE["CREATE ORDER PAGE<br/>Tôi cần tạo order mới từ event<br/>Cần: API service để tạo order<br/>Cần: AuthContext để lấy user info<br/>Làm: Hiển thị event details và ticket types<br/>Làm: Chọn số lượng vé và seat<br/>Làm: Tính toán tổng tiền<br/>Làm: Gọi API tạo order<br/>ĐỂ LÀM GÌ: Cho user đặt vé sự kiện và tạo đơn hàng"]
    
    %% ===== BACKEND ENTRY POINT =====
    PROG["PROGRAM.CS - ENTRY POINT<br/>Tôi cần khởi động ứng dụng<br/>Cần: Database connection string<br/>Cần: JWT secret key và config<br/>Cần: CORS policy cho frontend<br/>Cần: Đăng ký tất cả services vào DI container<br/>Làm: Configure middleware pipeline<br/>Làm: AddDatabase, AddRepositories, AddInfrastructureServices<br/>Làm: AddApplicationServices, AddCorsPolicy<br/>ĐỂ LÀM GÌ: Khởi động web server, config tất cả dependencies"]
    
    %% ===== BACKEND CONTROLLERS =====
    AUTH_CTRL["AUTH CONTROLLER<br/>Tôi cần xử lý HTTP requests cho authentication<br/>Cần: AuthService để xử lý business logic<br/>Làm: Nhận POST /api/auth/login<br/>Làm: Nhận POST /api/auth/register<br/>Làm: Nhận GET /api/auth/me<br/>Làm: Nhận GET /api/auth/user/{userId}<br/>Làm: Nhận POST /api/auth/seed-admin<br/>Làm: Trả về JSON response cho client<br/>ĐỂ LÀM GÌ: Cho phép user đăng nhập/đăng ký và lấy thông tin cá nhân"]
    
    EVENT_CTRL["EVENT CONTROLLER<br/>Tôi cần xử lý HTTP requests cho events<br/>Cần: EventService để xử lý business logic<br/>Làm: Nhận GET /api/event (lấy danh sách)<br/>Làm: Nhận GET /api/event/{id} (lấy chi tiết)<br/>Làm: Nhận POST /api/event (tạo mới)<br/>Làm: Nhận PUT /api/event/{id} (cập nhật)<br/>Làm: Nhận DELETE /api/event/{id} (xóa)<br/>Làm: Nhận GET /api/event/host/{hostId}<br/>Làm: Nhận POST /api/event/seed<br/>ĐỂ LÀM GÌ: Cho phép user quản lý sự kiện (xem, tạo, sửa, xóa)"]
    
    ORDER_CTRL["ORDER CONTROLLER<br/>Tôi cần xử lý HTTP requests cho orders<br/>Cần: OrderService để xử lý business logic<br/>Làm: Nhận GET /api/order (lấy danh sách orders của user)<br/>Làm: Nhận GET /api/order/{id} (lấy chi tiết order)<br/>Làm: Nhận POST /api/order (tạo order mới)<br/>Làm: Nhận PUT /api/order/{id} (cập nhật order)<br/>Làm: Nhận DELETE /api/order/{id} (hủy order)<br/>Làm: Nhận POST /api/order/{id}/payment (thanh toán)<br/>ĐỂ LÀM GÌ: Cho phép user quản lý đơn hàng và thanh toán"]
    
    %% ===== BACKEND SERVICES =====
    AUTH_SVC["AUTH SERVICE - BUSINESS LOGIC<br/>Tôi cần xử lý logic đăng nhập và đăng ký<br/>Cần: UserRepository để truy cập database<br/>Cần: JwtService để tạo token<br/>Cần: PasswordService để hash password<br/>Cần: UserMapper để chuyển đổi dữ liệu<br/>Làm: Validate email/password<br/>Làm: Hash password với BCrypt<br/>Làm: Tạo JWT token với user info<br/>Làm: Trả về LoginResponse với token<br/>ĐỂ LÀM GÌ: Xác thực user và tạo session token để truy cập hệ thống"]
    
    EVENT_SVC["EVENT SERVICE - BUSINESS LOGIC<br/>Tôi cần xử lý logic quản lý sự kiện<br/>Cần: EventRepository để truy cập database<br/>Cần: EventMapper để chuyển đổi dữ liệu<br/>Làm: Validate event data<br/>Làm: Map DTO thành Entity<br/>Làm: Gọi repository để lưu/xóa/cập nhật<br/>Làm: Map Entity thành DTO để trả về<br/>ĐỂ LÀM GÌ: Quản lý lifecycle của sự kiện từ tạo đến xóa"]
    
    ORDER_SVC["ORDER SERVICE - BUSINESS LOGIC<br/>Tôi cần xử lý logic quản lý đơn hàng<br/>Cần: OrderRepository để truy cập database<br/>Cần: OrderMapper để chuyển đổi dữ liệu<br/>Cần: PaymentService để xử lý thanh toán<br/>Làm: Validate order data và business rules<br/>Làm: Tính toán tổng tiền và kiểm tra inventory<br/>Làm: Tạo order và order items<br/>Làm: Xử lý thanh toán và cập nhật status<br/>ĐỂ LÀM GÌ: Quản lý lifecycle của đơn hàng từ tạo đến hoàn thành"]
    
    %% ===== UTILITY SERVICES =====
    JWT_SVC["JWT SERVICE - TOKEN GENERATION<br/>Tôi cần tạo và verify JWT tokens<br/>Cần: JWT secret key từ configuration<br/>Cần: JWT issuer và audience config<br/>Làm: GenerateToken - tạo token từ user info<br/>Làm: ValidateToken - verify token hợp lệ<br/>Làm: Extract claims từ token<br/>ĐỂ LÀM GÌ: Tạo session token để user không cần đăng nhập lại"]
    
    PASS_SVC["PASSWORD SERVICE - SECURITY<br/>Tôi cần hash và verify passwords<br/>Cần: BCrypt library để hash passwords<br/>Làm: HashPassword - hash password với salt<br/>Làm: VerifyPassword - so sánh password với hash<br/>Làm: Đảm bảo password an toàn<br/>ĐỂ LÀM GÌ: Bảo mật password user, không lưu plain text"]
    
    PAYMENT_SVC["PAYMENT SERVICE - PAYMENT PROCESSING<br/>Tôi cần xử lý thanh toán cho orders<br/>Cần: Payment gateway integration<br/>Cần: OrderRepository để cập nhật order status<br/>Làm: Validate payment data<br/>Làm: Process payment với gateway<br/>Làm: Cập nhật order và payment status<br/>Làm: Handle payment failures và refunds<br/>ĐỂ LÀM GÌ: Xử lý thanh toán an toàn và cập nhật trạng thái đơn hàng"]
    
    %% ===== REPOSITORIES =====
    USER_REPO["USER REPOSITORY - DATA ACCESS<br/>Tôi cần truy cập bảng Users trong database<br/>Cần: EventDBContext để thực hiện SQL queries<br/>Làm: GetUserByEmailAsync - tìm user theo email<br/>Làm: CreateUserAsync - tạo user mới<br/>Làm: GetUserByIdAsync - tìm user theo ID<br/>Làm: IsEmailExistsAsync - kiểm tra email đã tồn tại<br/>ĐỂ LÀM GÌ: Lưu trữ và truy xuất thông tin user từ database"]
    
    EVENT_REPO["EVENT REPOSITORY - DATA ACCESS<br/>Tôi cần truy cập bảng Events trong database<br/>Cần: EventDBContext để thực hiện SQL queries<br/>Làm: GetAllEventsAsync - lấy tất cả events<br/>Làm: GetEventByIdAsync - lấy event theo ID<br/>Làm: CreateEventAsync - tạo event mới<br/>Làm: UpdateEventAsync - cập nhật event<br/>Làm: DeleteEventAsync - xóa event<br/>ĐỂ LÀM GÌ: Lưu trữ và truy xuất thông tin sự kiện từ database"]
    
    ORDER_REPO["ORDER REPOSITORY - DATA ACCESS<br/>Tôi cần truy cập bảng Orders trong database<br/>Cần: EventDBContext để thực hiện SQL queries<br/>Làm: GetOrdersByUserIdAsync - lấy orders của user<br/>Làm: GetOrderByIdAsync - lấy order theo ID<br/>Làm: CreateOrderAsync - tạo order mới<br/>Làm: UpdateOrderAsync - cập nhật order<br/>Làm: DeleteOrderAsync - xóa order<br/>ĐỂ LÀM GÌ: Lưu trữ và truy xuất thông tin đơn hàng từ database"]
    
    %% ===== MAPPERS =====
    USER_MAP["USER MAPPER - DATA TRANSFORMATION<br/>Tôi cần chuyển đổi giữa Entity và DTO<br/>Cần: User entity từ database<br/>Cần: RegisterRequest từ client<br/>Làm: MapToUserReadDto - Entity → DTO<br/>Làm: MapFromRegisterRequest - DTO → Entity<br/>Làm: Ẩn password hash khỏi response<br/>ĐỂ LÀM GÌ: Chuyển đổi dữ liệu giữa database và API, ẩn thông tin nhạy cảm"]
    
    EVENT_MAP["EVENT MAPPER - DATA TRANSFORMATION<br/>Tôi cần chuyển đổi giữa Event Entity và DTO<br/>Cần: Event entity từ database<br/>Cần: CreateEventRequest từ client<br/>Làm: MapToEventDto - Entity → DTO<br/>Làm: MapToEventDetailDto - Entity → Detail DTO<br/>Làm: MapFromCreateEventRequest - DTO → Entity<br/>ĐỂ LÀM GÌ: Chuyển đổi dữ liệu sự kiện giữa database và API"]
    
    ORDER_MAP["ORDER MAPPER - DATA TRANSFORMATION<br/>Tôi cần chuyển đổi giữa Order Entity và DTO<br/>Cần: Order entity từ database<br/>Cần: CreateOrderRequest từ client<br/>Làm: MapToOrderDto - Entity → DTO<br/>Làm: MapToOrderDetailDto - Entity → Detail DTO<br/>Làm: MapFromCreateOrderRequest - DTO → Entity<br/>ĐỂ LÀM GÌ: Chuyển đổi dữ liệu đơn hàng giữa database và API"]
    
    %% ===== DATA LAYER =====
    MODELS["MODELS - ENTITY DEFINITIONS<br/>Tôi cần định nghĩa cấu trúc dữ liệu<br/>Cần: Database tables để map<br/>Làm: User model - UserId, Username, Email, PasswordHash<br/>Làm: Event model - EventId, Title, Description, StartTime<br/>Làm: Order, Ticket, Payment models<br/>Làm: Định nghĩa relationships giữa các entities<br/>ĐỂ LÀM GÌ: Định nghĩa cấu trúc dữ liệu để Entity Framework tạo database"]
    
    DTOS["DTOs - API CONTRACTS<br/>Tôi cần định nghĩa format giao tiếp API<br/>Cần: Client requirements để design<br/>Làm: LoginRequest - email, password<br/>Làm: RegisterRequest - username, email, password<br/>Làm: LoginResponse - user info + token<br/>Làm: EventDto - event info cho client<br/>Làm: CreateEventRequest - data để tạo event<br/>Làm: OrderDto - order info cho client<br/>Làm: CreateOrderRequest - data để tạo order<br/>Làm: PaymentDto - payment info cho client<br/>ĐỂ LÀM GÌ: Định nghĩa contract giữa frontend và backend, đảm bảo type safety"]
    
    %% ===== DATABASE LAYER =====
    DB_CTX["EVENT DB CONTEXT - ORM LAYER<br/>Tôi cần kết nối với database<br/>Cần: SQL Server connection string<br/>Cần: Entity Framework configuration<br/>Làm: Map entities thành database tables<br/>Làm: Thực hiện LINQ queries<br/>Làm: Track changes và save changes<br/>Làm: Handle database transactions<br/>ĐỂ LÀM GÌ: Cung cấp interface để truy cập database một cách type-safe"]
    
    DB["SQL SERVER DATABASE - PHYSICAL STORAGE<br/>Tôi cần lưu trữ dữ liệu thực tế<br/>Cần: SQL Server instance running<br/>Cần: Database schema đã được tạo<br/>Làm: Lưu trữ Users table<br/>Làm: Lưu trữ Events table<br/>Làm: Lưu trữ Orders, Tickets, Payments tables<br/>Làm: Đảm bảo data integrity và performance<br/>ĐỂ LÀM GÌ: Lưu trữ dữ liệu persistent, đảm bảo ACID properties"]
    
    %% ===== CONNECTIONS - FRONTEND FLOW =====
    USER --> REACT_APP
    REACT_APP --> AUTH_CONTEXT
    REACT_APP --> API_SERVICE
    
    %% AUTH CONTEXT CONNECTIONS
    AUTH_CONTEXT --> LOGIN_PAGE
    AUTH_CONTEXT --> REGISTER_PAGE
    AUTH_CONTEXT --> HEADER
    AUTH_CONTEXT --> PROTECTED_ROUTE
    
    %% API SERVICE CONNECTIONS
    API_SERVICE --> LOGIN_PAGE
    API_SERVICE --> REGISTER_PAGE
    API_SERVICE --> HOME_PAGE
    API_SERVICE --> EVENT_DETAILS_PAGE
    API_SERVICE --> DASHBOARD_PAGE
    API_SERVICE --> ORDER_LIST_PAGE
    API_SERVICE --> ORDER_DETAILS_PAGE
    API_SERVICE --> CREATE_ORDER_PAGE
    
    %% ===== CONNECTIONS - FRONTEND TO BACKEND =====
    API_SERVICE --> PROG
    
    %% ===== CONNECTIONS - BACKEND FLOW =====
    PROG --> AUTH_CTRL
    PROG --> EVENT_CTRL
    PROG --> ORDER_CTRL
    
    %% CONTROLLER TO SERVICE CONNECTIONS
    AUTH_CTRL --> AUTH_SVC
    EVENT_CTRL --> EVENT_SVC
    ORDER_CTRL --> ORDER_SVC
    
    %% SERVICE TO REPOSITORY CONNECTIONS
    AUTH_SVC --> USER_REPO
    AUTH_SVC --> JWT_SVC
    AUTH_SVC --> PASS_SVC
    AUTH_SVC --> USER_MAP
    
    EVENT_SVC --> EVENT_REPO
    EVENT_SVC --> EVENT_MAP
    
    ORDER_SVC --> ORDER_REPO
    ORDER_SVC --> ORDER_MAP
    ORDER_SVC --> PAYMENT_SVC
    
    %% REPOSITORY TO DATABASE CONNECTIONS
    USER_REPO --> DB_CTX
    EVENT_REPO --> DB_CTX
    ORDER_REPO --> DB_CTX
    
    %% DATABASE CONNECTIONS
    DB_CTX --> DB
    
    %% DATA LAYER CONNECTIONS
    MODELS --> DB_CTX
    DTOS --> AUTH_CTRL
    DTOS --> EVENT_CTRL
    DTOS --> ORDER_CTRL

    %% ===== STYLING - IMPLEMENTATION STATUS =====
    %% FRONTEND LAYER - IMPLEMENTATION STATUS
    %% ✅ IMPLEMENTED (LIGHT BLUE) - Đã làm xong
    style USER fill:#e3f2fd,stroke:#1976d2,stroke-width:3px
    style REACT_APP fill:#bbdefb,stroke:#1976d2,stroke-width:3px
    style HOME_PAGE fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    style LOGIN_PAGE fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    style REGISTER_PAGE fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    style EVENT_DETAILS_PAGE fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    style DASHBOARD_PAGE fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    style HEADER fill:#90caf9,stroke:#1976d2,stroke-width:2px
    style PROTECTED_ROUTE fill:#90caf9,stroke:#1976d2,stroke-width:2px
    style AUTH_CONTEXT fill:#64b5f6,stroke:#1976d2,stroke-width:2px
    style API_SERVICE fill:#64b5f6,stroke:#1976d2,stroke-width:2px
    
    %% ❌ NOT IMPLEMENTED (WHITE) - Chưa làm
    style ORDER_LIST_PAGE fill:#ffffff,stroke:#666666,stroke-width:2px
    style ORDER_DETAILS_PAGE fill:#ffffff,stroke:#666666,stroke-width:2px
    
    %% 🔄 IN PROGRESS (LIGHT BLUE) - Đang làm
    style CREATE_ORDER_PAGE fill:#bbdefb,stroke:#1976d2,stroke-width:2px
    
    %% BACKEND LAYER - IMPLEMENTATION STATUS
    %% ✅ IMPLEMENTED (GREEN) - Đã làm xong
    style PROG fill:#c8e6c9,stroke:#2e7d32,stroke-width:3px
    style AUTH_CTRL fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style EVENT_CTRL fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style AUTH_SVC fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style EVENT_SVC fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style JWT_SVC fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style PASS_SVC fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style USER_REPO fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style EVENT_REPO fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style USER_MAP fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style EVENT_MAP fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style MODELS fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style DTOS fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style DB_CTX fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style DB fill:#c8e6c9,stroke:#2e7d32,stroke-width:3px
    
    %% ❌ NOT IMPLEMENTED (WHITE) - Chưa làm
    style ORDER_CTRL fill:#ffffff,stroke:#666666,stroke-width:2px
    style ORDER_SVC fill:#ffffff,stroke:#666666,stroke-width:2px
    style PAYMENT_SVC fill:#ffffff,stroke:#666666,stroke-width:2px
    style ORDER_REPO fill:#ffffff,stroke:#666666,stroke-width:2px
    style ORDER_MAP fill:#ffffff,stroke:#666666,stroke-width:2px
```

## 🎯 **MINDSET CỦA TỪNG LAYER**

### **🚀 Program.cs - "Tôi cần khởi động app"**
- **Cần:** Database connection, JWT config, CORS policy
- **Dùng:** appsettings.json, service interfaces
- **Làm:** Đăng ký tất cả dependencies

### **🎮 Controllers - "Tôi cần xử lý HTTP requests"**
- **Cần:** Services tương ứng
- **Dùng:** AuthService cho login/register, EventService cho events
- **Làm:** Nhận request → Gọi service → Trả response

### **⚙️ Services - "Tôi cần xử lý business logic"**
- **Cần:** Repositories + Utility services
- **Dùng:** UserRepository + JwtService + PasswordService
- **Làm:** Logic nghiệp vụ + Mapping

### **🗄️ Repositories - "Tôi cần truy cập database"**
- **Cần:** Database context
- **Dùng:** EventDBContext (Entity Framework)
- **Làm:** CRUD operations

### **🗃️ Models - "Tôi cần cấu trúc dữ liệu"**
- **Cần:** Database tables
- **Dùng:** Entity Framework
- **Làm:** Định nghĩa entities

## 🔄 **LUỒNG HOẠT ĐỘNG CHÍNH**

### **📝 ĐĂNG NHẬP:**
```
Client → AuthController → AuthService → UserRepository + JwtService + PasswordService → Database
```

### **📝 TẠO SỰ KIỆN:**
```
Client → EventController → EventService → EventRepository + EventMapper → Database
```

### **📝 TẠO ĐƠN HÀNG:**
```
Client → OrderController → OrderService → OrderRepository + OrderMapper + PaymentService → Database
```

### **📝 THANH TOÁN:**
```
Client → OrderController → OrderService → PaymentService → OrderRepository → Database
```

## 🏆 **KẾT LUẬN**

**Kiến trúc này HOÀN HẢO** cho mindset **"Tôi cần tới cái gì, tôi làm tới cái đó đi"**:

- ✅ **Rõ ràng** - Mỗi layer có trách nhiệm riêng
- ✅ **Đơn giản** - Chỉ cần biết layer ngay dưới  
- ✅ **Linh hoạt** - Dễ thay đổi và mở rộng
- ✅ **Team-friendly** - Nhiều người có thể code song song
- ✅ **Testable** - Dễ mock và unit test

**Nguyên tắc vàng:** *"Keep It Simple, Stupid (KISS)"* - Chỉ làm những gì cần thiết! 🎯
