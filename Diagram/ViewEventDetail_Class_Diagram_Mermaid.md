# View Event Detail - Class Diagram (Mermaid)

```mermaid
classDiagram
    %% Controller Layer
    class EventController {
        -IEventService _eventService
        +GetEventById(int id) IActionResult
    }

    %% Service Layer
    class IEventService {
        <<interface>>
        +GetEventByIdAsync(int eventId) Event?
        +MapToEventDetailDto(Event) object
    }

    class EventService {
        -IEventRepository _eventRepository
        -IEventMapper _eventMapper
        +GetEventByIdAsync(int eventId) Event?
        +MapToEventDetailDto(Event) object
    }

    %% Repository Layer
    class IEventRepository {
        <<interface>>
        +GetEventByIdAsync(int eventId) Event?
    }

    class EventRepository {
        -EventDBContext _context
        +GetEventByIdAsync(int eventId) Event?
    }

    %% Data Access Layer
    class EventDBContext {
        +DbSet~Event~ Events
        +DbSet~User~ Users
        +DbSet~TicketType~ TicketTypes
        +DbSet~Order~ Orders
        +DbSet~OrderItem~ OrderItems
        +DbSet~Payment~ Payments
        +DbSet~Ticket~ Tickets
        +DbSet~OtpCode~ OtpCodes
        +DbSet~WalletTransaction~ WalletTransactions
        +DbSet~Wishlist~ Wishlists
        +DbSet~Voucher~ Vouchers
    }

    %% Domain Models
    class Event {
        +int EventId
        +int HostId
        +string Title
        +string Description
        +DateTime StartTime
        +DateTime EndTime
        +string Location
        +string Category
        +string Status
        +DateTime CreatedAt
        +DateTime? UpdatedAt
    }

    class User {
        +int UserId
        +string Username
        +string FullName
        +string Email
        +string PasswordHash
        +string? Phone
        +string Role
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +string? Avatar
        +DateTime? DateOfBirth
        +string? Gender
        +decimal WalletBalance
    }

    class TicketType {
        +int TicketTypeId
        +int EventId
        +string TypeName
        +decimal Price
        +int Quantity
        +int? MinOrder
        +int? MaxOrder
        +DateTime SaleStart
        +DateTime SaleEnd
        +string Status
    }

    %% Mapper Layer
    class IEventMapper {
        <<interface>>
        +MapToEventDetailDto(Event) object
    }

    class EventMapper {
        +MapToEventDetailDto(Event) object
    }

    %% DTOs
    class EventDetailDto {
        +int EventId
        +string Title
        +string Description
        +DateTime StartTime
        +DateTime EndTime
        +string Location
        +string Category
        +string Status
        +DateTime CreatedAt
        +string HostName
        +string HostEmail
        +List~TicketTypeDto~ TicketTypes
    }

    class TicketTypeDto {
        +int TicketTypeId
        +string TypeName
        +decimal Price
        +int Quantity
        +int? MinOrder
        +int? MaxOrder
        +DateTime SaleStart
        +DateTime SaleEnd
        +string Status
    }

    %% Relationships
    EventController --> IEventService : "1. calls"
    EventService ..|> IEventService : implements
    EventService --> IEventRepository : "2. queries"
    EventService --> IEventMapper : "3. uses"
    EventRepository ..|> IEventRepository : implements
    EventRepository --> EventDBContext : "4. uses"
    EventDBContext --> Event : "5. manages"
    EventDBContext --> User : "6. manages"
    EventDBContext --> TicketType : "7. manages"
    Event --> User : "8. belongs to (Host)"
    Event --> TicketType : "9. has many"
    EventMapper ..|> IEventMapper : implements
    EventMapper --> EventDetailDto : "10. maps to"
    EventDetailDto --> TicketTypeDto : "11. contains"
    EventService --> EventDetailDto : "12. returns"
```

## Mô tả luồng View Event Detail

### 🏗️ Kiến trúc phân lớp:

1. **Controller Layer**: `EventController` - Điểm vào xử lý HTTP requests
2. **Service Layer**: `IEventService`/`EventService` - Chứa business logic
3. **Repository Layer**: `IEventRepository`/`EventRepository` - Tầng truy cập dữ liệu
4. **Data Access Layer**: `EventDBContext` - Quản lý kết nối database
5. **Domain Models**: `Event`, `User`, `TicketType` - Các thực thể domain
6. **Mapper Layer**: `IEventMapper`/`EventMapper` - Chuyển đổi objects
7. **DTOs**: `EventDetailDto`, `TicketTypeDto` - Data transfer objects

### 🔄 Luồng tương tác:

1. **Client** → `EventController.GetEventById(id)`
2. **Controller** → `IEventService.GetEventByIdAsync(id)`
3. **Service** → `IEventRepository.GetEventByIdAsync(id)`
4. **Repository** → `EventDBContext` (với Include để lấy Host và TicketTypes)
5. **Database** → Trả về Event entity với thông tin liên quan
6. **Mapper** → Chuyển đổi Event thành `EventDetailDto`
7. **Response** → Trả về cho Client

### 📋 Pattern áp dụng:

- **Dependency Injection**: Tất cả dependencies được inject
- **Repository Pattern**: Tách biệt logic truy cập dữ liệu
- **Service Layer Pattern**: Chứa business logic
- **DTO Pattern**: Tách biệt domain model khỏi API response
- **Mapper Pattern**: Chuyển đổi giữa domain model và DTO
- **Layered Architecture**: Phân tách rõ ràng các lớp chức năng
