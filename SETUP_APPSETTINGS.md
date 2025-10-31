# Hướng dẫn thiết lập appsettings.json

## Cấu trúc file cấu hình

Dự án sử dụng 2 file cấu hình:

1. **`appsettings.json`** - File chính, **CÓ THỂ COMMIT** lên git
   - Chứa: Logging, Connection String, JWT settings
   - Tất cả developer cần file này

2. **`secrets.json`** - File secrets, **KHÔNG COMMIT** lên git
   - Chứa: Google OAuth credentials (Gmail API)
   - Mỗi developer tự tạo file này từ template

## Cách thiết lập cho developer mới

### Bước 1: Copy file template cho appsettings.json
```bash
cd src
copy appsettings.Example.json appsettings.json
```

Hoặc trên Linux/Mac:
```bash
cd src
cp appsettings.Example.json appsettings.json
```

### Bước 2: Copy file template cho secrets.json (OAuth credentials)
```bash
cd src
copy secrets.json.example secrets.json
```

### Bước 3: Điền thông tin vào appsettings.json

Mở file `appsettings.json` và thay thế các placeholder:

#### 1. Connection String
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TheGrind5DB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
}
```

#### 2. JWT Secret Key
```json
"Jwt": {
  "Key": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG_HERE",
  "Issuer": "TheGrind5_EventManagement",
  "Audience": "TheGrind5_EventManagement_Users"
}
```
⚠️ **Lưu ý**: Key phải có ít nhất 32 ký tự.

### Bước 4: Điền thông tin vào secrets.json (OAuth credentials)

Mở file `secrets.json` và điền thông tin Google OAuth:

```json
{
  "Gmail": {
    "ClientId": "YOUR_GOOGLE_OAUTH_CLIENT_ID",
    "ClientSecret": "YOUR_GOOGLE_OAUTH_CLIENT_SECRET",
    "RefreshToken": "YOUR_GOOGLE_OAUTH_REFRESH_TOKEN",
    "ApplicationName": "TheGrind5 Event Management"
  }
}
```

### Bước 5: Lấy Google OAuth Credentials

Nếu bạn cần lấy Google OAuth credentials, hãy liên hệ với team lead hoặc admin để được cấp credentials, hoặc tham khảo file `Diagram/GMAIL_API_SETUP.md` để tự tạo.

## Cách chia sẻ OAuth credentials cho team

**✅ appsettings.json CÓ THỂ COMMIT** - File này không chứa secrets, mọi người đều cần.

**⚠️ secrets.json KHÔNG ĐƯỢC COMMIT** - Chỉ chứa OAuth credentials.

Nếu cần chia sẻ OAuth credentials:
1. Gửi qua Slack/Teams (private message)
2. Hoặc sử dụng password manager của team
3. Hoặc sử dụng Azure Key Vault / AWS Secrets Manager cho production

## File structure

```
src/
├── appsettings.json          # ✅ CÓ THỂ COMMIT (Connection String, JWT settings)
├── appsettings.Example.json   # ✅ Template cho appsettings.json
├── secrets.json              # ⚠️ KHÔNG COMMIT (chỉ chứa OAuth credentials)
├── secrets.json.example      # ✅ Template cho secrets.json
└── ...
```

## Troubleshooting

### Lỗi: "Gmail RefreshToken is not configured"
- Kiểm tra xem bạn đã tạo `appsettings.json` chưa
- Kiểm tra section `Gmail` trong file có đầy đủ các trường không

### Lỗi: "Invalid JWT Key"
- Đảm bảo JWT Key có ít nhất 32 ký tự
- Không có khoảng trắng ở đầu/cuối

