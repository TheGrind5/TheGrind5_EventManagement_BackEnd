# Hướng dẫn thiết lập appsettings.json

## Tại sao appsettings.json bị ignore?

File `appsettings.json` chứa các thông tin nhạy cảm (secrets) như:
- Google OAuth credentials (Client ID, Client Secret, Refresh Token)
- JWT Secret Key
- Connection String với password

Do đó, file này được ignore bởi git để bảo mật.

## Cách thiết lập cho developer mới

### Bước 1: Copy file template
```bash
cd src
copy appsettings.Example.json appsettings.json
```

Hoặc trên Linux/Mac:
```bash
cd src
cp appsettings.Example.json appsettings.json
```

### Bước 2: Điền thông tin vào appsettings.json

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

#### 3. Gmail OAuth Credentials
```json
"Gmail": {
  "ClientId": "YOUR_GOOGLE_OAUTH_CLIENT_ID",
  "ClientSecret": "YOUR_GOOGLE_OAUTH_CLIENT_SECRET",
  "RefreshToken": "YOUR_GOOGLE_OAUTH_REFRESH_TOKEN",
  "ApplicationName": "TheGrind5 Event Management"
}
```

### Bước 3: Lấy Google OAuth Credentials

Nếu bạn cần lấy Google OAuth credentials, hãy liên hệ với team lead hoặc admin để được cấp credentials, hoặc tham khảo file `Diagram/GMAIL_API_SETUP.md` để tự tạo.

## Cách chia sẻ credentials cho team

**⚠️ KHÔNG BAO GIỜ commit appsettings.json lên git!**

Nếu cần chia sẻ credentials:
1. Gửi qua Slack/Teams (private message)
2. Hoặc sử dụng password manager của team
3. Hoặc sử dụng Azure Key Vault / AWS Secrets Manager cho production

## File structure

```
src/
├── appsettings.json          # ⚠️ Local file, không commit (chứa secrets thật)
├── appsettings.Example.json   # ✅ Template file, có thể commit (chứa placeholder)
└── ...
```

## Troubleshooting

### Lỗi: "Gmail RefreshToken is not configured"
- Kiểm tra xem bạn đã tạo `appsettings.json` chưa
- Kiểm tra section `Gmail` trong file có đầy đủ các trường không

### Lỗi: "Invalid JWT Key"
- Đảm bảo JWT Key có ít nhất 32 ký tự
- Không có khoảng trắng ở đầu/cuối

