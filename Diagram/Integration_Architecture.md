# Sơ đồ Tích hợp Backend-Frontend - TheGrind5 Event Management

## Tổng quan
Hệ thống sử dụng kiến trúc client-server với React frontend và ASP.NET Core backend, giao tiếp qua REST API với JWT authentication.

## Sơ đồ Tích hợp Tổng thể

```mermaid
graph TB
    %% Frontend Layer
    subgraph "Frontend (React)"
        ReactApp[React Application]
        AuthContext[AuthContext]
        Pages[Pages<br/>HomePage, LoginPage, DashboardPage, etc.]
        Components[Components<br/>Header, ProtectedRoute]
        Services[API Services<br/>AuthAPI, EventsAPI]
    end
    
    %% Network Layer
    HTTP[HTTP/HTTPS<br/>REST API Calls]
    CORS[CORS Policy<br/>Cross-Origin Requests]
    
    %% Backend Layer (Clean Architecture)
    subgraph "Backend (ASP.NET Core - Clean Architecture)"
        Controllers[Controllers<br/>AuthController, EventController]
        CoreServices[Core Services<br/>AuthService, EventService]
        Repositories[Repositories<br/>UserRepository, EventRepository]
        InfrastructureServices[Infrastructure Services<br/>JwtService, PasswordService, Mappers]
        DbContext[EventDBContext<br/>EF Core]
    end
    
    %% Database Layer
    SQLServer[(SQL Server Database<br/>EventDB)]
    
    %% External Services
    JWT[JWT Token Service]
    BCrypt[BCrypt Password Hashing]
    
    %% Connections
    ReactApp --> AuthContext
    ReactApp --> Pages
    ReactApp --> Components
    ReactApp --> Services
    
    Services --> HTTP
    HTTP --> CORS
    CORS --> Controllers
    
    Controllers --> CoreServices
    CoreServices --> Repositories
    CoreServices --> InfrastructureServices
    Repositories --> DbContext
    DbContext --> SQLServer
    
    InfrastructureServices --> JWT
    InfrastructureServices --> BCrypt
    
    %% Styling
    classDef frontend fill:#e1f5fe
    classDef backend fill:#f3e5f5
    classDef database fill:#e8f5e8
    classDef network fill:#fff3e0
    classDef external fill:#ffebee
    
    class ReactApp,AuthContext,Pages,Components,Services frontend
    class Controllers,CoreServices,Repositories,InfrastructureServices,DbContext backend
    class SQLServer database
    class HTTP,CORS network
    class JWT,BCrypt external
```

## Luồng Tích hợp Chính

### 1. User Authentication Flow (Clean Architecture)
```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant AuthAPI
    participant AuthController
    participant AuthService
    participant UserRepository
    participant PasswordService
    participant JwtService
    participant UserMapper
    participant Database
    
    User->>Frontend: Enter login credentials
    Frontend->>AuthAPI: login(email, password)
    AuthAPI->>AuthController: POST /api/Auth/login
    AuthController->>AuthService: LoginAsync(email, password)
    AuthService->>UserRepository: GetUserByEmailAsync(email)
    UserRepository->>Database: Query user by email
    Database-->>UserRepository: User data
    UserRepository-->>AuthService: User entity
    AuthService->>PasswordService: VerifyPassword(password, hash)
    PasswordService-->>AuthService: Verification result
    AuthService->>JwtService: GenerateToken(user)
    JwtService-->>AuthService: JWT token
    AuthService->>UserMapper: MapToUserReadDto(user)
    UserMapper-->>AuthService: User DTO
    AuthService-->>AuthController: LoginResponse with token
    AuthController-->>AuthAPI: {user, token, expiresAt}
    AuthAPI-->>Frontend: Authentication response
    Frontend->>Frontend: Store user in AuthContext
    Frontend->>Frontend: Store token in localStorage
    Frontend->>Frontend: Navigate to dashboard
```

### 2. Event Management Flow (Clean Architecture)
```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant EventsAPI
    participant EventController
    participant EventService
    participant EventRepository
    participant EventMapper
    participant Database
    
    User->>Frontend: Visit homepage
    Frontend->>EventsAPI: getAll()
    EventsAPI->>EventController: GET /api/Event
    EventController->>EventService: GetAllEventsAsync()
    EventService->>EventRepository: GetAllEventsAsync()
    EventRepository->>Database: Query events with joins
    Database-->>EventRepository: Event data with relations
    EventRepository-->>EventService: Event list
    EventService->>EventMapper: MapToEventDto(event)
    EventMapper-->>EventService: Event DTO
    EventService-->>EventController: Event DTO list
    EventController-->>EventsAPI: JSON response
    EventsAPI-->>Frontend: Event list
    Frontend->>Frontend: Render event cards
```

### 3. Protected Route Access Flow
```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant AuthContext
    participant Backend
    participant Database
    
    User->>Frontend: Access protected route
    Frontend->>AuthContext: Check authentication
    AuthContext->>AuthContext: Check localStorage
    
    alt User is authenticated
        AuthContext-->>Frontend: User exists
        Frontend->>Frontend: Render protected content
    else User not authenticated
        AuthContext-->>Frontend: No user
        Frontend->>Frontend: Redirect to login
    end
```

## API Integration Details

### Authentication Endpoints
| Method | Endpoint | Frontend Usage | Backend Handler |
|--------|----------|----------------|-----------------|
| POST | `/api/Auth/login` | AuthAPI.login() | AuthController.Login() |
| POST | `/api/Auth/register` | AuthAPI.register() | AuthController.Register() |
| GET | `/api/Auth/me` | AuthAPI.getCurrentUser() | AuthController.GetCurrentUser() |
| GET | `/api/Auth/user/{id}` | AuthAPI.getUser() | AuthController.GetUserById() |

### Event Endpoints
| Method | Endpoint | Frontend Usage | Backend Handler |
|--------|----------|----------------|-----------------|
| GET | `/api/Event` | EventsAPI.getAll() | EventController.GetAllEvents() |
| GET | `/api/Event/{id}` | EventsAPI.getById() | EventController.GetEventById() |
| POST | `/api/Event` | EventsAPI.create() | EventController.CreateEvent() |
| PUT | `/api/Event/{id}` | EventsAPI.update() | EventController.UpdateEvent() |
| DELETE | `/api/Event/{id}` | EventsAPI.delete() | EventController.DeleteEvent() |
| GET | `/api/Event/host/{id}` | EventsAPI.getByHost() | EventController.GetEventsByHost() |

## Data Flow Architecture

### 1. Request Flow
```mermaid
graph LR
    User[User Action] --> Frontend[React Component]
    Frontend --> API[API Service]
    API --> HTTP[HTTP Request]
    HTTP --> Backend[ASP.NET Controller]
    Backend --> Service[Business Service]
    Service --> Repository[Repository]
    Repository --> Database[(Database)]
```

### 2. Response Flow
```mermaid
graph LR
    Database[(Database)] --> Repository[Repository]
    Repository --> Service[Business Service]
    Service --> Backend[ASP.NET Controller]
    Backend --> HTTP[HTTP Response]
    HTTP --> API[API Service]
    API --> Frontend[React Component]
    Frontend --> User[User Interface]
```

## Security Integration

### JWT Token Flow
```mermaid
sequenceDiagram
    participant Frontend
    participant Backend
    participant Database
    
    Frontend->>Backend: Login request
    Backend->>Database: Validate credentials
    Database-->>Backend: User data
    Backend->>Backend: Generate JWT token
    Backend-->>Frontend: Token + user data
    Frontend->>Frontend: Store token in localStorage
    
    Note over Frontend: Subsequent requests include token
    Frontend->>Backend: API request with Authorization header
    Backend->>Backend: Validate JWT token
    Backend-->>Frontend: Protected resource
```

### CORS Configuration
- **Allowed Origins**: `http://localhost:3000`, `http://localhost:5173`, `https://localhost:5173`
- **Allowed Methods**: GET, POST, PUT, DELETE
- **Allowed Headers**: Any header
- **Credentials**: Not configured (stateless)

## Error Handling Integration

### Frontend Error Handling
```javascript
// API Service Error Handling
try {
  const response = await fetch(url, options);
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || 'Request failed');
  }
  return response.json();
} catch (error) {
  console.error('API Error:', error);
  throw error;
}
```

### Backend Error Handling
```csharp
// Controller Error Handling
try {
    var result = await _service.ProcessRequest();
    return Ok(result);
} catch (Exception ex) {
    return BadRequest(new { message = "Error occurred", error = ex.Message });
}
```

## State Management Integration

### Frontend State
- **AuthContext**: Global authentication state
- **Component State**: Local component state
- **LocalStorage**: Persistent user data

### Backend State
- **Database**: Persistent data storage
- **Session**: Stateless (JWT-based)
- **Cache**: No caching implemented

## Performance Considerations

### Frontend Optimizations
- **Code Splitting**: Route-based splitting
- **Lazy Loading**: Component lazy loading
- **State Management**: Efficient re-renders

### Backend Optimizations
- **Database Queries**: EF Core optimizations
- **Connection Pooling**: SQL Server connection pooling
- **Response Compression**: Not configured

### Network Optimizations
- **HTTP/2**: Not explicitly configured
- **Compression**: Not configured
- **Caching**: No caching headers

## Deployment Architecture

### Development Environment
```mermaid
graph LR
    Frontend[React Dev Server<br/>localhost:3000] --> Backend[ASP.NET Core<br/>localhost:5000]
    Backend --> Database[SQL Server<br/>Local Instance]
```

### Production Environment
```mermaid
graph LR
    Frontend[React Build<br/>Static Files] --> Backend[ASP.NET Core<br/>Production Server]
    Backend --> Database[SQL Server<br/>Production Database]
```

## Monitoring và Logging

### Frontend Logging
- **Console Logs**: Development debugging
- **Error Boundaries**: React error handling
- **Network Monitoring**: Browser dev tools

### Backend Logging
- **Console Logs**: Development debugging
- **Exception Handling**: Try-catch blocks
- **Database Logging**: EF Core logging

## Testing Strategy

### Frontend Testing
- **Unit Tests**: Component testing
- **Integration Tests**: API integration
- **E2E Tests**: User workflow testing

### Backend Testing
- **Unit Tests**: Service and repository testing
- **Integration Tests**: API endpoint testing
- **Database Tests**: Data access testing

## Scalability Considerations

### Frontend Scalability
- **Code Splitting**: Reduce bundle size
- **Lazy Loading**: Load components on demand
- **State Management**: Efficient state updates

### Backend Scalability
- **Database Indexing**: Query optimization
- **Connection Pooling**: Database connection management
- **Caching**: Future implementation
- **Load Balancing**: Future implementation

## Security Best Practices

### Frontend Security
- **Input Validation**: Form validation
- **XSS Prevention**: React's built-in protection
- **Token Storage**: Secure localStorage usage

### Backend Security
- **Authentication**: JWT token validation
- **Authorization**: Role-based access control
- **Input Validation**: Model validation
- **SQL Injection**: EF Core protection
- **Password Security**: BCrypt hashing
