# 📧 Hướng Dẫn Thiết Lập Gmail API cho TheGrind5 Event Management

## 🎯 Tổng Quan

Hệ thống sử dụng **1 Gmail trung tâm** (`nguyenluonghoangthien5605@gmail.com`) để gửi email đến **người dùng thực tế** từ database.

**Flow:**
- **Từ**: `nguyenluonghoangthien5605@gmail.com` (config trong appsettings.json)
- **Đến**: Email người dùng lấy từ database (User.Email)

**⚠️ LƯU Ý QUAN TRỌNG:**
- **Test users** trong OAuth Consent Screen chỉ để cấp quyền đăng nhập Gmail API
- **KHÔNG ảnh hưởng** đến việc gửi email cho người dùng thực tế
- Bạn vẫn gửi được email cho BẤT KỲ địa chỉ email nào trong database
- Test users chỉ là cách Google kiểm soát ai có thể tạo Refresh Token

## 🔧 Bước 1: Tạo Project trên Google Cloud Console

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Đặt tên project: **TheGrind5 Event Management**

## 🔌 Bước 2: Kích Hoạt Gmail API

1. Vào **API & Services** → **Library**
2. Tìm kiếm **"Gmail API"**
3. Nhấn **Enable** để kích hoạt

## 🔐 Bước 3: Cấu Hình OAuth Consent Screen

1. Vào **API & Services** → **OAuth consent screen**
2. Chọn **External** (cho testing) hoặc **Internal** (cho production nội bộ)
3. Điền thông tin:
   - **App name**: TheGrind5 Event Management
   - **User support email**: nguyenluonghoangthien5605@gmail.com
   - **Developer contact**: nguyenluonghoangthien5605@gmail.com
4. Nhấn **Save and Continue**
5. **Scopes** → Nhấn **Add or Remove Scopes**
6. Chọn scope: **`https://www.googleapis.com/auth/gmail.send`**
7. Nhấn **Add to Table** → **Update** → **Save and Continue**
8. **Test users** (nếu chọn External):
   - ⚠️ **BẮT BUỘC:** Phải thêm test users nếu chọn External
   - Nhấn **Add Users**
   - Thêm email: `nguyenluonghoangthien5605@gmail.com`
   - Nếu có email khác cần test, thêm luôn vào đây
9. Nhấn **Save and Continue** → **Back to Dashboard**

**⚠️ QUAN TRỌNG:** 
- Email được thêm vào Test users mới có thể dùng app trong chế độ Testing
- Nếu quên thêm email vào Test users → sẽ gặp lỗi "Error 403: access_denied"

## 🔑 Bước 4: Tạo OAuth Credentials

1. Vào **API & Services** → **Credentials**
2. Nhấn **+ Create Credentials** → **OAuth client ID**
3. Nếu chưa có OAuth consent screen, làm bước 3 trước
4. Chọn **Application type**: **Web application**
5. Đặt tên: **TheGrind5 Event Management Client**
6. Thêm **Authorized redirect URIs**: 
   ```
   https://developers.google.com/oauthplayground
   ```
7. Nhấn **Create**
8. Sau khi tạo, một popup hiển thị **Client ID** (copy và lưu)
9. **⚠️ QUAN TRỌNG:** Popup chỉ hiển thị Client ID, không hiển thị Client Secret
10. Nhấn **OK** để đóng popup
11. Trong danh sách **OAuth 2.0 Client IDs**, tìm client vừa tạo và click vào tên client
12. Trong trang chi tiết, bạn sẽ thấy **Client secret** - copy và lưu cẩn thận!

## 🎫 Bước 5: Lấy Refresh Token

1. Truy cập [OAuth 2.0 Playground](https://developers.google.com/oauthplayground/)
2. Nhấn **⚙️ Settings** (góc trên bên phải)
3. Tick **✅ Use your own OAuth credentials**
4. Dán **Client ID** và **Client Secret** của bạn
5. Nhấn **Close**

6. Bên trái, tìm và chọn:
   - Expand **Gmail API v1**
   - Tick **`https://www.googleapis.com/auth/gmail.send`**
7. Nhấn **Authorize APIs**
8. Đăng nhập với tài khoản: **nguyenluonghoangthien5605@gmail.com**
9. Nhấn **Allow** để cấp quyền
10. Nhấn **Exchange authorization code for tokens**
11. Copy **Refresh token** và lưu cẩn thận

## ⚙️ Bước 6: Cấu Hình appsettings.json

Mở file `src/appsettings.json` và cập nhật phần `Gmail`:

```json
{
  "Gmail": {
    "ApplicationName": "TheGrind5 Event Management",
    "ClientId": "YOUR_CLIENT_ID_HERE",
    "ClientSecret": "YOUR_CLIENT_SECRET_HERE",
    "RefreshToken": "YOUR_REFRESH_TOKEN_HERE",
    "SenderEmail": "nguyenluonghoangthien5605@gmail.com"
  }
}
```

**Thay thế:**
- `YOUR_CLIENT_ID_HERE` → Client ID từ Bước 4
- `YOUR_CLIENT_SECRET_HERE` → Client Secret từ Bước 4
- `YOUR_REFRESH_TOKEN_HERE` → Refresh Token từ Bước 5

**⚠️ QUAN TRỌNG:** Không commit `appsettings.json` có thông tin thực vào Git!

## ✅ Bước 7: Test Email Service

1. Chạy ứng dụng Backend
2. Gọi API gửi email (ví dụ: forgot password → gửi OTP)
3. Kiểm tra email đến đúng địa chỉ người dùng

**Ví dụ:**
```
User.Email = "customer@example.com"
→ Email gửi từ: nguyenluonghoangthien5605@gmail.com
→ Email gửi đến: customer@example.com ✅
```

## 🔒 Security Best Practices

1. **Refresh Token**: Thông tin nhạy cảm, bảo mật cẩn thận
2. **Environment Variables**: Không hardcode credentials
3. **Git Ignore**: Không commit `appsettings.json` có thông tin thực
4. **Production**: Sử dụng Azure Key Vault hoặc AWS Secrets Manager

## 📝 Troubleshooting

### ❌ Lỗi: "Invalid grant"
**Nguyên nhân:** Refresh token đã hết hạn hoặc bị revoke  
**Giải pháp:** Lấy refresh token mới từ OAuth Playground

### ❌ Lỗi: "Access denied" hoặc "Error 403: access_denied"
**Nguyên nhân:** Chưa thêm email vào Test users trong OAuth Consent Screen  
**Giải pháp:** 
1. Vào Google Cloud Console → API & Services → OAuth consent screen
2. Cuộn xuống phần **Test users**
3. Nhấn **Add Users** và thêm email `nguyenluonghoangthien5605@gmail.com`
4. Nhấn **Save**
5. Quay lại OAuth Playground và thử lại

### ❌ Lỗi: "Invalid client"
**Nguyên nhân:** Client ID hoặc Client Secret sai  
**Giải pháp:** Kiểm tra lại credentials trong Google Cloud Console

### ❌ Lỗi: "RefreshToken is not configured"
**Nguyên nhân:** Chưa cấu hình RefreshToken trong appsettings.json  
**Giải pháp:** Làm lại Bước 6

## 📚 Tài Liệu Tham Khảo

- [Gmail API Documentation](https://developers.google.com/gmail/api)
- [Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)
- [Google Cloud Console](https://console.cloud.google.com/)
- [OAuth 2.0 Playground](https://developers.google.com/oauthplayground)

## 🎯 Checklist Hoàn Thành

- [ ] Đã tạo project trên Google Cloud Console
- [ ] Đã kích hoạt Gmail API
- [ ] Đã cấu hình OAuth Consent Screen với scope `gmail.send`
- [ ] Đã tạo OAuth Client ID và Client Secret
- [ ] Đã lấy Refresh Token từ OAuth Playground
- [ ] Đã cấu hình appsettings.json với credentials
- [ ] Đã test gửi email thành công

---

**Chúc bạn setup thành công! 🚀**
