# 🚀 Quick Start VNPay - Sẵn Sàng Test!

## ✅ Đã Hoàn Thành

- ✅ ngrok đang chạy: https://bd61899cc277.ngrok-free.app
- ✅ IpnUrl đã được cập nhật
- ✅ Backend đang chạy tại http://localhost:5000
- ✅ All code đã implement xong

---

## 📋 Bước Tiếp Theo: Đăng Ký VNPay Sandbox

### 1. Truy Cập VNPay Sandbox
**URL**: https://sandbox.vnpayment.vn/devreg/

### 2. Đăng Ký Merchant Account
- Điền form đăng ký
- Verify email
- Đợi approval

### 3. Lấy Credentials
Check email hoặc login dashboard để lấy:
- **TmnCode**: 8 ký tự (ví dụ: `2QXUI4J4`)
- **HashSecret**: String dài (ví dụ: `ZUWNJYDRFOQIPUCAEKXOHFGIUPFBMXFO`)

---

## 🔐 Cập Nhật Credentials

### Cách Nhanh (Cách 2):
Mở file: `src/appsettings.json`

```json
"VNPay": {
  "TmnCode": "PASTE_TMN_CODE_HERE",     // ← Thay bằng TmnCode thật
  "HashSecret": "PASTE_HASH_SECRET_HERE", // ← Thay bằng HashSecret thật
  "PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
  "ReturnUrl": "http://localhost:3000/payment/vnpay/return",
  "IpnUrl": "https://bd61899cc277.ngrok-free.app/api/Payment/vnpay/webhook", ✅
  ...
}
```

**Lưu ý**: ngrok URL sẽ thay đổi mỗi lần restart ngrok. Cần update lại IpnUrl!

### Cách An Toàn (Cách 1):
```powershell
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_BackEnd\src

# Init user secrets (chỉ cần chạy 1 lần)
dotnet user-secrets init

# Set credentials
dotnet user-secrets set "VNPay:TmnCode" "YOUR_TMN_CODE"
dotnet user-secrets set "VNPay:HashSecret" "YOUR_HASH_SECRET"
```

---

## 🔄 Restart Backend

Sau khi update credentials, restart backend:

```powershell
# Stop backend hiện tại (Ctrl+C)
# Sau đó start lại:
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_BackEnd\src
dotnet run
```

---

## 🧪 Test Flow

### 1. Start Frontend
```powershell
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_FrontEnd
npm start
```

### 2. Tạo Order với VNPay
1. Login frontend
2. Vào trang event details
3. Chọn vé và số lượng
4. Chọn payment method: **VNPay**
5. Click "Đặt vé"

### 3. Kiểm Tra
- ✅ QR code hiển thị
- ✅ Payment URL được tạo
- ✅ Redirect đến VNPay Sandbox

### 4. Thanh Toán
1. Click vào payment URL hoặc quét QR
2. Dùng test card từ VNPay Sandbox
3. Complete payment

### 5. Verify Webhook
**Mở**: http://localhost:4040 (ngrok dashboard)
- Tìm request đến `/api/Payment/vnpay/webhook`
- Check request payload
- Check response = "SUCCESS"

### 6. Check Database
```sql
SELECT * FROM Payment WHERE OrderId = YOUR_ORDER_ID
SELECT * FROM [Order] WHERE OrderId = YOUR_ORDER_ID
```

---

## 🔍 Debug Checklist

| Issue | Solution |
|-------|----------|
| ❌ Webhook không được gọi | - Check ngrok đang chạy<br>- Check IpnUrl đúng format<br>- Check backend đang chạy |
| ❌ Webhook trả "FAIL" | - Check HashSecret đúng<br>- Check TmnCode đúng<br>- Check signature validation |
| ❌ Order không cập nhật | - Check backend logs<br>- Check database<br>- Check webhook response |

---

## 📝 Important Notes

### ⚠️ ngrok URL Thay Đổi
Mỗi lần restart ngrok, URL mới. Cần:
1. Lấy URL mới: `Invoke-RestMethod http://localhost:4040/api/tunnels`
2. Update IpnUrl trong appsettings.json
3. Restart backend

### ⚠️ VNPay Sandbox Test Cards
Check email từ VNPay để lấy test card numbers

### ✅ Webhook Response
Webhook **PHẢI** return plain text: "SUCCESS" hoặc "FAIL" (không phải JSON)

### ✅ Idempotency
Backend đã implement check duplicate webhook qua TransactionId

---

## 🎯 Summary

Bạn chỉ cần:
1. ✅ Đăng ký VNPay Sandbox → Lấy credentials
2. ✅ Update credentials vào appsettings.json
3. ✅ Restart backend
4. ✅ Test payment flow

**Code đã sẵn sàng, chỉ cần credentials!** 🚀

