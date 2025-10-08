# 🚀 TheGrind5 Event Management - Setup Guide

## 📋 Tổng quan

Hướng dẫn này giúp bạn setup project TheGrind5 Event Management trên bất kỳ máy nào một cách dễ dàng và nhất quán.

## 🎯 Giải pháp cho vấn đề hiện tại

### ❌ **Vấn đề cũ:**
- Database connection string cứng chỉ hoạt động trên máy của bạn
- Hard-coded paths trong scripts
- Không có environment configuration
- Khó setup trên máy khác

### ✅ **Giải pháp mới:**
- **Environment-based configuration** - Tự động detect môi trường
- **Docker support** - Database chạy trong container
- **Universal scripts** - Hoạt động trên mọi máy
- **Auto-detection** - Tự động chọn setup phù hợp

## 🛠️ Cách sử dụng

### **Phương pháp 1: Setup tự động (Khuyến nghị)**

```bash
# 1. Chạy setup script
setup.bat

# 2. Chọn option phù hợp:
# - Option 1: Docker setup (Khuyến nghị)
# - Option 2: Local SQL Server
# - Option 3: SQL Server Express
```

### **Phương pháp 2: Development với auto-detection**

```bash
# Chạy development manager
dev-universal.bat

# Script sẽ tự động:
# - Detect Docker có sẵn không
# - Chọn setup phù hợp
# - Start services
```

## 📁 Cấu trúc file mới

```
Backend/
├── 📄 setup.bat                    # Setup script chính
├── 📄 dev-universal.bat           # Development manager
├── 📄 dev.bat                     # Script cũ (giữ lại)
├── 📄 docker-compose.yml          # Full Docker setup
├── 📄 docker-compose.dev.yml      # Dev database only
├── 📄 Dockerfile                  # Backend container
├── 📄 env.example                 # Environment template
├── 📄 appsettings.json            # Default config
├── 📄 appsettings.Development.json
├── 📄 appsettings.Production.json
├── 📄 appsettings.Docker.json    # Docker config
└── 📄 SETUP_GUIDE.md             # Hướng dẫn này
```

## 🔧 Các phương pháp setup

### **1. Docker Setup (Khuyến nghị)**

**Ưu điểm:**
- ✅ Hoạt động trên mọi máy
- ✅ Không cần cài SQL Server
- ✅ Consistent environment
- ✅ Dễ dàng cleanup

**Cách sử dụng:**
```bash
# Chạy setup
setup.bat
# Chọn option 1: Docker setup

# Hoặc chạy trực tiếp
docker-compose -f docker-compose.dev.yml up -d db
dotnet ef database update
dotnet run
```

### **2. Local SQL Server Setup**

**Khi nào dùng:**
- Đã có SQL Server cài sẵn
- Muốn dùng database hiện có
- Không muốn dùng Docker

**Cách sử dụng:**
```bash
setup.bat
# Chọn option 2: Local SQL Server
# Nhập thông tin database của bạn
```

### **3. SQL Server Express Setup**

**Khi nào dùng:**
- Có SQL Server Express
- Muốn setup nhanh
- Development environment

**Cách sử dụng:**
```bash
setup.bat
# Chọn option 3: SQL Server Express
# Nhập instance name (thường là localhost\SQLEXPRESS)
```

## 🚀 Development Workflow

### **Quick Start (Auto-detection)**
```bash
# Chạy development manager
dev-universal.bat

# Script sẽ:
# 1. Auto-detect môi trường
# 2. Start database (Docker hoặc Local)
# 3. Run migrations
# 4. Start backend
# 5. Start frontend
```

### **Manual Development**
```bash
# 1. Start database
docker-compose -f docker-compose.dev.yml up -d db

# 2. Run migrations
dotnet ef database update

# 3. Start backend
dotnet run

# 4. Start frontend (trong terminal khác)
cd ../TheGrind5_EventManagement_FrontEnd
npm start
```

## 🔍 Troubleshooting

### **Database Connection Issues**

**Lỗi thường gặp:**
```
Cannot connect to database
Login failed for user 'sa'
```

**Giải pháp:**
1. **Docker setup:** Đảm bảo Docker đang chạy
2. **Local setup:** Kiểm tra SQL Server service
3. **Credentials:** Verify username/password

### **Port Conflicts**

**Lỗi thường gặp:**
```
Port 5000 is already in use
Port 3000 is already in use
```

**Giải pháp:**
```bash
# Stop all services
dev-universal.bat
# Chọn option 4: Stop All Services

# Hoặc kill manually
taskkill /f /im dotnet.exe
taskkill /f /im node.exe
```

### **Frontend Not Found**

**Lỗi thường gặp:**
```
Frontend directory not found
```

**Giải pháp:**
1. Đảm bảo frontend project ở đúng vị trí:
   ```
   TheGrind5_EventManagement_BackEnd/
   TheGrind5_EventManagement_FrontEnd/
   ```
2. Hoặc update path trong script

## 📊 Environment Configuration

### **Development Environment**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;User Id=sa;Password=YourStrong@Passw0rd;Database=EventDB;TrustServerCertificate=true;"
  }
}
```

### **Docker Environment**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db;User Id=sa;Password=YourStrong@Passw0rd;Database=EventDB;TrustServerCertificate=true;"
  }
}
```

### **Production Environment**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_SERVER};User Id=${DB_USER};Password=${DB_PASSWORD};Database=${DB_NAME};TrustServerCertificate=true;"
  }
}
```

## 🎯 Best Practices

### **1. Team Development**
- Sử dụng Docker setup để đảm bảo consistency
- Commit `env.example` file
- Không commit `appsettings.json` với credentials thật

### **2. Production Deployment**
- Sử dụng environment variables
- Setup proper database credentials
- Enable HTTPS và security features

### **3. Development**
- Sử dụng `dev-universal.bat` cho daily development
- Regular cleanup với stop services
- Monitor logs để debug issues

## 📞 Support

Nếu gặp vấn đề:
1. Chạy `setup.bat` → option 4: Check Prerequisites
2. Kiểm tra logs trong console windows
3. Verify database connection
4. Check port availability

## 🔄 Migration từ setup cũ

Nếu bạn đang dùng setup cũ:

1. **Backup configuration hiện tại:**
   ```bash
   copy appsettings.json appsettings.backup.json
   ```

2. **Chạy setup mới:**
   ```bash
   setup.bat
   ```

3. **Test application:**
   ```bash
   dev-universal.bat
   ```

4. **Verify everything works:**
   - Backend: http://localhost:5000/swagger
   - Frontend: http://localhost:3000
   - Database: Check connection

---

**🎉 Chúc mừng! Bây giờ project của bạn có thể chạy trên mọi máy một cách dễ dàng!**
