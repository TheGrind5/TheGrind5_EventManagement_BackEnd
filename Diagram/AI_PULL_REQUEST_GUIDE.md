# 🤖 AI PULL REQUEST GUIDE - CONFIGURATION CONFLICTS

## 🎯 MỤC ĐÍCH
Hướng dẫn AI xử lý pull request để tránh xung đột configuration giữa các máy khác nhau.

## ⚠️ VẤN ĐỀ CẦN TRÁNH

### 🔧 Configuration Files Có Thể Gây Xung Đột

#### **Backend Configuration**
```
src/appsettings.json
├── ConnectionStrings.DefaultConnection
├── Jwt.Key
└── Jwt.Issuer/Audience
```

#### **Frontend Configuration**  
```
5GrindThe/TheGrind5_EventManagement_FrontEnd/
├── src/services/api.js (API_BASE_URL)
├── package.json (backend script path)
└── run.bat (frontend path)
```

#### **CORS & Network Configuration**
```
src/Constants/AppConstants.cs
├── CORS_FRONTEND_URL
├── CORS_FRONTEND_URL_ALT  
└── CORS_FRONTEND_URL_HTTPS
```

## 🚫 FILES CẦN BỎ QUA KHI MERGE

### **1. Database Connection Strings**
```json
// ❌ KHÔNG MERGE - Machine-specific
"DefaultConnection": "Server=LAPTOP-NKR40L4O\\SQLEXPRESS;Database=EventDB;User Id=sa;Password=123456;TrustServerCertificate=true;"

// ✅ MERGE - Generic
"DefaultConnection": "Server=localhost;Database=EventDB;Integrated Security=true;TrustServerCertificate=true;"
```

### **2. API Base URLs**
```javascript
// ❌ KHÔNG MERGE - Machine-specific  
const API_BASE_URL = 'http://192.168.1.100:5000/api';

// ✅ MERGE - Generic
const API_BASE_URL = 'http://localhost:5000/api';
```

### **3. CORS Origins**
```csharp
// ❌ KHÔNG MERGE - Machine-specific
public const string CORS_FRONTEND_URL_ALT = "http://192.168.1.100:3001";

// ✅ MERGE - Generic  
public const string CORS_FRONTEND_URL_ALT = "http://localhost:3001";
```

### **4. File Paths**
```batch
# ❌ KHÔNG MERGE - Machine-specific
start "Frontend" cmd /k "cd /d C:\Users\PHOENIX\Desktop\5GrindThe\TheGrind5_EventManagement_FrontEnd && npm start"

# ✅ MERGE - Generic
start "Frontend" cmd /k "cd /d %~dp0\..\5GrindThe\TheGrind5_EventManagement_FrontEnd && npm start"
```

## 🤖 AI MERGE STRATEGY

### **KHI REVIEW PULL REQUEST:**

#### **1. Kiểm Tra Configuration Files**
```bash
# Files cần kiểm tra kỹ
- src/appsettings.json
- src/Constants/AppConstants.cs  
- 5GrindThe/.../src/services/api.js
- 5GrindThe/.../package.json
- run.bat
```

#### **2. Phân Loại Thay Đổi**
```markdown
✅ SAFE TO MERGE:
- Business logic changes
- New features
- Bug fixes
- Code improvements
- Documentation updates

❌ SKIP OR MODIFY:
- Connection strings with specific servers
- Hardcoded IP addresses
- Machine-specific paths
- Local development URLs
```

#### **3. Xử Lý Configuration Conflicts**

##### **Option A: Reject Configuration Changes**
```markdown
❌ REJECT: Configuration changes should be environment-specific
- Revert connection string changes
- Keep generic localhost URLs
- Maintain relative paths
```

##### **Option B: Create Environment-Specific Files**
```markdown
✅ ACCEPT: Create separate config files
- appsettings.Development.json
- appsettings.Production.json  
- .env files for frontend
```

## 📋 CHECKLIST CHO AI

### **TRƯỚC KHI MERGE:**

- [ ] **Kiểm tra connection strings** - Chỉ cho phép localhost/generic
- [ ] **Kiểm tra API URLs** - Chỉ cho phép localhost:5000
- [ ] **Kiểm tra CORS origins** - Chỉ cho phép localhost ports
- [ ] **Kiểm tra file paths** - Chỉ cho phép relative paths
- [ ] **Kiểm tra hardcoded values** - Loại bỏ IP addresses cụ thể

### **SAU KHI MERGE:**

- [ ] **Tạo .gitignore entries** cho machine-specific files
- [ ] **Tạo template files** cho configuration
- [ ] **Cập nhật documentation** về environment setup
- [ ] **Thông báo team** về configuration changes

## 🛡️ PREVENTION STRATEGY

### **1. Git Hooks**
```bash
# Pre-commit hook để kiểm tra
#!/bin/bash
if grep -q "LAPTOP-" src/appsettings.json; then
    echo "❌ Machine-specific configuration detected!"
    exit 1
fi
```

### **2. Template Files**
```json
// appsettings.template.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EventDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

### **3. Environment Variables**
```javascript
// Frontend - sử dụng environment variables
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';
```

## 🎯 AI DECISION MATRIX

| File Type | Change Type | Action |
|-----------|-------------|---------|
| `appsettings.json` | Connection String | ❌ REJECT |
| `appsettings.json` | JWT Settings | ✅ ACCEPT |
| `api.js` | Hardcoded URL | ❌ REJECT |
| `api.js` | Environment Variable | ✅ ACCEPT |
| `AppConstants.cs` | CORS Origins | ❌ REJECT |
| `AppConstants.cs` | Business Constants | ✅ ACCEPT |
| `run.bat` | Absolute Paths | ❌ REJECT |
| `run.bat` | Relative Paths | ✅ ACCEPT |

## 📝 TEMPLATE COMMENTS

### **Khi Reject Configuration Changes:**
```markdown
❌ **Configuration Conflict Detected**

This change contains machine-specific configuration that will cause conflicts on other machines:

- Connection string: `Server=LAPTOP-XXX` 
- API URL: `http://192.168.1.100:5000`
- File path: `C:\Users\PHOENIX\Desktop\...`

**Recommendation:** 
1. Revert to generic configuration (localhost)
2. Use environment variables
3. Create separate config files for different environments

**Safe alternatives:**
- `Server=localhost` instead of specific server names
- `http://localhost:5000` instead of specific IPs  
- Relative paths instead of absolute paths
```

### **Khi Accept Generic Changes:**
```markdown
✅ **Configuration Change Approved**

This change uses generic configuration that works across all machines:

- ✅ Uses localhost instead of specific server names
- ✅ Uses relative paths instead of absolute paths
- ✅ Uses environment variables where appropriate
- ✅ Maintains consistency across development environments
```

## 🚀 IMPLEMENTATION STEPS

### **1. Immediate Actions**
- [ ] Review all pending PRs for configuration conflicts
- [ ] Create configuration templates
- [ ] Update CI/CD to check for machine-specific configs

### **2. Long-term Solutions**
- [ ] Implement environment-based configuration
- [ ] Create setup scripts for new developers
- [ ] Add configuration validation in build process

### **3. Team Communication**
- [ ] Document configuration standards
- [ ] Train team on environment-specific configs
- [ ] Create troubleshooting guide for config conflicts

---

## 🎯 **TÓM TẮT CHO AI**

**KHI XỬ LÝ PULL REQUEST:**

1. **🔍 KIỂM TRA** - Tìm machine-specific configurations
2. **❌ REJECT** - Configuration changes với hardcoded values  
3. **✅ ACCEPT** - Generic configurations (localhost, relative paths)
4. **📝 COMMENT** - Giải thích lý do và đưa ra alternatives
5. **🛡️ PREVENT** - Tạo templates và guidelines

**NGUYÊN TẮC VÀNG:** 
> **"Configuration should be environment-agnostic, not machine-specific"**

**MỤC TIÊU:** 
> **Đảm bảo code chạy được trên mọi máy mà không cần thay đổi configuration! 🎯**
