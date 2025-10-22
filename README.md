# TheGrind5 Event Management System

## 📁 Cấu trúc Project

```
TheGrind5_EventManagement_BackEnd/
├── 📁 Diagram/                    # Architecture & Documentation
│   ├── Backend_Architecture_Mermaid.md
│   ├── Business_Flow_User_Order_Process.md
│   ├── Class_Diagram_Backend.md
│   ├── CreateOrder.md
│   ├── SampleData_Passwords.md
│   └── SETUP_GUIDE.md
├── 📁 src/                        # Source Code
│   ├── 📁 Controllers/            # API Controllers
│   ├── 📁 Services/               # Business Logic
│   ├── 📁 Models/                 # Data Models
│   ├── 📁 Data/                   # Database Context
│   ├── 📁 DTOs/                   # Data Transfer Objects
│   ├── 📁 Repositories/           # Repository Pattern
│   ├── 📁 Migrations/             # Database Migrations
│   ├── 📁 Business/               # Business Interfaces
│   ├── 📁 Constants/              # Application Constants
│   ├── 📁 Extensions/             # Service Extensions
│   ├── 📁 Helpers/                # Helper Classes
│   ├── 📁 Mappers/                # Object Mappers
│   ├── 📁 Properties/             # Project Properties
│   ├── Program.cs                 # Application Entry Point
│   ├── appsettings.json           # Configuration
│   └── TheGrind5_EventManagement.csproj
├── 📄 run.bat                     # Development startup script
├── 📄 README.md                   # This file
├── 📄 SampleData_Insert.sql       # Sample data for database
└── 📄 TheGrind5_Query.sql         # Database queries
```

## 🚀 Quick Start

### Chạy ứng dụng
```bash
# Chạy cả Backend và Frontend
.\run.bat

# Hoặc chạy riêng lẻ:
# Backend
cd src
dotnet run

# Frontend (từ thư mục gốc)
cd ..\TheGrind5_EventManagement_FrontEnd
npm start
```


