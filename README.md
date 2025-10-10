# TheGrind5 Event Management System

## 📁 Cấu trúc Project

```
TheGrind5_EventManagement_BackEnd/
├── 📁 Scripts/                    # Development & Setup Scripts
│   └── thegrind5-manager.bat     # All-in-one manager (Setup + Development)
├── 📁 Docker/                     # Docker Configuration
│   ├── docker-compose.yml         # Full production setup
│   ├── docker-compose.dev.yml     # Development database
│   └── Dockerfile                 # Backend container
├── 📁 Config/                     # Configuration Files
│   ├── appsettings.Production.json
│   ├── appsettings.Docker.json
│   └── env.example                # Environment template
├── 📁 Documentation/              # Documentation
│   └── SETUP_GUIDE.md            # Detailed setup guide
├── 📁 src/                        # Source Code
│   ├── 📁 Controllers/            # API Controllers
│   ├── 📁 Services/               # Business Logic
│   ├── 📁 Models/                 # Data Models
│   ├── 📁 Data/                    # Database Context
│   ├── 📁 DTOs/                   # Data Transfer Objects
│   ├── 📁 Respositories/          # Repository Pattern
│   ├── 📁 Migrations/             # Database Migrations
│   ├── 📁 Properties/             # Project Properties
│   ├── Program.cs                 # Application Entry Point
│   └── TheGrind5_EventManagement.csproj
└── 📄 [Other project files...]
```

## 🚀 Quick Start

### **Cách sử dụng (SQL Server Express Optimized)**
```bash
# Chạy universal manager
Scripts\thegrind5-manager.bat

# MAIN MENU (6 options đơn giản):
# 1. Quick Start (Auto-detect + SQL Express) - Khuyến nghị
# 2. Setup with SQL Server Express - Khuyến nghị
# 3. Start Development
# 4. Stop All Services
# 5. Check Status
# 6. Exit

# SUB-MENUS (chi tiết khi cần):
# - Setup Menu: SQL Express (Recommended), Docker, Local SQL, Prerequisites
# - Development Menu: Auto-detect (SQL Express preferred), SQL Express, Docker, Local Database
```

## 🛠️ Development Workflow

### **Daily Development**
```bash
# 1. Start development environment
Scripts\thegrind5-manager.bat

# 2. Chọn option 1: Quick Start (Auto-detect + SQL Express)
# Script sẽ tự động detect SQL Server Express và start services
```

### **Manual Development**
```bash
# 1. Start database (Docker)
Docker\docker-compose -f docker-compose.dev.yml up -d db

# 2. Run migrations
cd src
dotnet ef database update

# 3. Start backend
dotnet run

# 4. Start frontend (in another terminal)
cd ../TheGrind5_EventManagement_FrontEnd
npm start
```

## 📊 Environment Configuration

### **Development (Default)**
- Database: Local SQL Server hoặc Docker
- Port: 5000 (Backend), 3000 (Frontend)
- Environment: Development

### **Docker Environment**
- Database: Docker container
- Port: 5000 (Backend), 3000 (Frontend)
- Environment: Docker

### **Production Environment**
- Database: Production SQL Server
- Port: 5000 (Backend), 3000 (Frontend)
- Environment: Production

## 🔧 Troubleshooting

### **Common Issues**

1. **Scripts không chạy được:**
   ```bash
   # Đảm bảo bạn đang ở root directory
   cd TheGrind5_EventManagement_BackEnd
   Scripts\setup.bat
   ```

2. **Database connection failed:**
   ```bash
   # Chạy setup script để configure database
   Scripts\setup.bat
   # Chọn option phù hợp với setup của bạn
   ```

3. **Port conflicts:**
   ```bash
   # Stop all services
   Scripts\dev-universal.bat
   # Chọn option 4: Stop All Services
   ```

## 📚 Documentation

- **Detailed Setup Guide:** `Documentation/SETUP_GUIDE.md`
- **API Documentation:** http://localhost:5000/swagger (khi chạy backend)
- **Frontend:** http://localhost:3000 (khi chạy frontend)

## 🎯 Best Practices

### **Team Development**
1. Sử dụng Docker setup để đảm bảo consistency
2. Commit `Config/env.example` file
3. Không commit `appsettings.json` với credentials thật

### **Production Deployment**
1. Sử dụng environment variables
2. Setup proper database credentials
3. Enable HTTPS và security features

## 🚀 Simple All-in-One Script

### Single Script for Everything:
```bash
# Main script with all features
Scripts\thegrind5.bat
```

### Features:
- **Setup Environment** (First time only)
- **Start Development** (Backend + Frontend)
- **Stop All Services** (Clean shutdown)
- **Check Status** (System health check)

### Quick Usage:
```bash
# Run the script
Scripts\thegrind5.bat

# Then choose:
# 1. Setup Environment (first time)
# 2. Start Development (daily use)
# 3. Stop All Services (when done)
# 4. Check Status (troubleshooting)
```

## 📞 Support

Nếu gặp vấn đề:
1. Chạy `Scripts\thegrind5.bat` → option 4 (Check Status)
2. Kiểm tra logs trong console windows
3. Verify database connection
4. Check port availability

---

**🎉 Chúc mừng! Project của bạn đã được tổ chức tốt và sẵn sàng cho team development!**
