# Sơ đồ Kiến trúc Backend - TheGrind5 Event Management

## Tổng quan
Backend được xây dựng bằng ASP.NET Core với kiến trúc Clean Architecture, sử dụng Entity Framework Core cho data access và được tổ chức theo nguyên tắc Separation of Concerns.

## Sơ đồ Kiến trúc Backend (Clean Architecture)

```mermaid
graph TB
    %% Entry Point
    Program[Program.cs<br/>Entry Point & Dependency Injection]
    
    %% Controllers Layer (Presentation)
    subgraph "Controllers Layer"
        AuthController[AuthController<br/>Authentication & Authorization]
        EventController[EventController<br/>Event Management]
    end
    
    %% Core Layer (Business Logic)
    subgraph "Core Layer"
        AuthService[AuthService<br/>Business Logic]
        EventService[EventService<br/>Business Logic]
    end
    
    %% Infrastructure Layer
    subgraph "Infrastructure Layer"
        %% Repositories
        subgraph "Repositories"
            UserRepository[UserRepository<br/>Data Access]
            EventRepository[EventRepository<br/>Data Access]
        end
        
        %% Infrastructure Services
        subgraph "Infrastructure Services"
            JwtService[JwtService<br/>JWT Token Generation]
            PasswordService[PasswordService<br/>Password Hashing]
            UserMapper[UserMapper<br/>Object Mapping]
            EventMapper[EventMapper<br/>Object Mapping]
        end
        
        %% Data Layer
        EventDBContext[EventDBContext<br/>EF Core DbContext]
    end
    
    %% Models & DTOs
    subgraph "Domain Layer"
        User[User Model]
        Event[Event Model]
        Order[Order Model]
        Payment[Payment Model]
        Ticket[Ticket Model]
        TicketType[TicketType Model]
        OrderItem[OrderItem Model]
        AuthDTOs[AuthDTOs<br/>Authentication DTOs]
        EventDTOs[EventDTOs<br/>Event DTOs]
    end
    
    %% Database
    SQLServer[(SQL Server Database)]
    
    %% Connections
    Program --> AuthController
    Program --> EventController
    Program --> EventDBContext
    
    AuthController --> AuthService
    EventController --> EventService
    
    AuthService --> UserRepository
    AuthService --> JwtService
    AuthService --> PasswordService
    AuthService --> UserMapper
    
    EventService --> EventRepository
    EventService --> EventMapper
    
    UserRepository --> EventDBContext
    EventRepository --> EventDBContext
    
    EventDBContext --> User
    EventDBContext --> Event
    EventDBContext --> Order
    EventDBContext --> Payment
    EventDBContext --> Ticket
    EventDBContext --> TicketType
    EventDBContext --> OrderItem
    
    EventDBContext --> SQLServer
    
    AuthController --> AuthDTOs
    EventController --> EventDTOs
    
    %% Styling
    classDef controller fill:#e1f5fe
    classDef core fill:#f3e5f5
    classDef infrastructure fill:#e8f5e8
    classDef domain fill:#fff3e0
    classDef database fill:#ffebee
    
    class AuthController,EventController controller
    class AuthService,EventService core
    class UserRepository,EventRepository,JwtService,PasswordService,UserMapper,EventMapper,EventDBContext infrastructure
    class User,Event,Order,Payment,Ticket,TicketType,OrderItem,AuthDTOs,EventDTOs domain
    class SQLServer database
```

## Luồng Xử lý Chính

### 1. Authentication Flow (Clean Architecture)
```mermaid
sequenceDiagram
    participant Client
    participant AuthController
    participant AuthService
    participant UserRepository
    participant PasswordService
    participant JwtService
    participant UserMapper
    participant EventDBContext
    participant Database
    
    Client->>AuthController: POST /api/Auth/login
    AuthController->>AuthService: LoginAsync(email, password)
    AuthService->>UserRepository: GetUserByEmailAsync(email)
    UserRepository->>EventDBContext: Query Users table
    EventDBContext->>Database: SELECT query
    Database-->>EventDBContext: User data
    EventDBContext-->>UserRepository: User entity
    UserRepository-->>AuthService: User data
    AuthService->>PasswordService: VerifyPassword(password, hash)
    PasswordService-->>AuthService: Verification result
    AuthService->>JwtService: GenerateToken(user)
    JwtService-->>AuthService: JWT token
    AuthService->>UserMapper: MapToUserReadDto(user)
    UserMapper-->>AuthService: User DTO
    AuthService-->>AuthController: LoginResponse with token
    AuthController-->>Client: JSON response with token
```

### 2. Event Management Flow (Clean Architecture)
```mermaid
sequenceDiagram
    participant Client
    participant EventController
    participant EventService
    participant EventRepository
    participant EventMapper
    participant EventDBContext
    participant Database
    
    Client->>EventController: GET /api/Event
    EventController->>EventService: GetAllEventsAsync()
    EventService->>EventRepository: GetAllEventsAsync()
    EventRepository->>EventDBContext: Include Host & TicketTypes
    EventDBContext->>Database: Complex query with joins
    Database-->>EventDBContext: Event data with relations
    EventDBContext-->>EventRepository: List of Events
    EventRepository-->>EventService: Event list
    EventService->>EventMapper: MapToEventDto(event)
    EventMapper-->>EventService: Event DTO
    EventService-->>EventController: Event DTO list
    EventController-->>Client: JSON response
```

## Các Layer và Trách nhiệm (Clean Architecture)

### 1. Presentation Layer (Controllers)
- **AuthController**: Chỉ xử lý HTTP requests/responses, không có business logic
- **EventController**: Chỉ xử lý HTTP requests/responses, không có business logic

### 2. Core Layer (Business Logic)
- **AuthService**: Business logic cho authentication, gọi các Infrastructure services
- **EventService**: Business logic cho event management, gọi Repository và Mapper

### 3. Infrastructure Layer
#### **Repositories (Data Access)**
- **UserRepository**: Data access cho User entity
- **EventRepository**: Data access cho Event entity

#### **Infrastructure Services**
- **JwtService**: JWT token generation và validation
- **PasswordService**: Password hashing và verification
- **UserMapper**: Object mapping cho User entities
- **EventMapper**: Object mapping cho Event entities

#### **Data Layer**
- **EventDBContext**: EF Core DbContext cho tất cả entities

### 4. Domain Layer (Models & DTOs)
- **User, Event, Order, Payment, Ticket, TicketType, OrderItem**: Domain entities
- **AuthDTOs, EventDTOs**: Data Transfer Objects

## Cấu hình và Dependencies

### Program.cs Configuration (Clean Architecture)
- **Repository Layer**: IUserRepository, IEventRepository
- **Infrastructure Services**: IJwtService, IPasswordService, IUserMapper, IEventMapper
- **Core Services**: AuthService, EventService
- **CORS configuration** cho frontend
- **Swagger/OpenAPI documentation**
- **Database connection string**
- **JWT authentication setup**

### Database Relationships
- User → Events (One-to-Many)
- Event → TicketTypes (One-to-Many)
- Order → OrderItems (One-to-Many)
- TicketType → Tickets (One-to-Many)
- Order → Payment (One-to-One)

## Security Features (Clean Architecture)
- **JWT Token authentication** (JwtService)
- **BCrypt password hashing** (PasswordService)
- **CORS policy configuration**
- **Input validation**
- **Authorization attributes**
- **Separation of concerns** - Security logic tách biệt trong Infrastructure layer

## API Endpoints

### Authentication
- `POST /api/Auth/login` - Đăng nhập
- `POST /api/Auth/register` - Đăng ký
- `GET /api/Auth/me` - Lấy thông tin user hiện tại
- `GET /api/Auth/user/{userId}` - Lấy thông tin user theo ID

### Events
- `GET /api/Event` - Lấy tất cả events
- `GET /api/Event/{id}` - Lấy event theo ID
- `POST /api/Event` - Tạo event mới (cần authentication)
- `PUT /api/Event/{id}` - Cập nhật event (cần authentication)
- `DELETE /api/Event/{id}` - Xóa event (cần authentication)
- `GET /api/Event/host/{hostId}` - Lấy events theo host
