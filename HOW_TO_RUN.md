# 🚀 Cách Chạy Dự Án VNPay

## ⚡ Quick Start

Chỉ cần **1 lệnh duy nhất**:

```bash
run.bat
```

Script sẽ tự động:
1. ✅ Start ngrok tunnel
2. ✅ Lấy ngrok URL mới
3. ✅ Update IpnUrl trong appsettings.json
4. ✅ Start backend (http://localhost:5000)
5. ✅ Start frontend (http://localhost:3000)

---

## 📋 Chi Tiết

### Ngrok URL Tự Động

Mỗi lần chạy `run.bat`, script sẽ:
- Đợi ngrok khởi động (6 giây)
- Lấy public URL từ ngrok API
- Tự động update `IpnUrl` trong `src/appsettings.json`
- Không cần config thủ công!

### Services

| Service | URL | Window |
|---------|-----|--------|
| **ngrok** | https://*.ngrok-free.app | Separate |
| **Backend** | http://localhost:5000 | Separate |
| **Frontend** | http://localhost:3000 | Separate |

---

## 🛑 Stop Services

**Ctrl+C** trong mỗi window service, hoặc đóng các windows.

---

## 🔍 Verify

Sau khi chạy `run.bat`:

1. **Check ngrok**: http://localhost:4040
2. **Check backend**: http://localhost:5000/swagger
3. **Check config**: `src/appsettings.json` → IpnUrl đã được update

---

## ⚠️ Lưu Ý

- **Database migration**: Chạy `dotnet ef database update` lần đầu
- **Frontend**: Cần chạy `npm install` trước lần đầu
- **ngrok**: Mỗi lần restart, URL thay đổi (script tự động xử lý)

---

## 🧪 Test VNPay

1. Chạy `run.bat`
2. Login frontend
3. Tạo order → Chọn VNPay
4. Thanh toán bằng VNPay Sandbox
5. Check ngrok dashboard: http://localhost:4040

---

**Done! 🎉**

