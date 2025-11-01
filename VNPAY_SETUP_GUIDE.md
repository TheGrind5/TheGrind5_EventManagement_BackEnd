# Hướng Dẫn Setup VNPay với ngrok

## 📋 Bước 1: Cài Đặt ngrok

1. **Download ngrok**: https://ngrok.com/download
2. Giải nén vào thư mục bất kỳ (ví dụ: `C:\ngrok`)
3. (Optional) Add vào PATH để chạy từ bất kỳ đâu

---

## 📋 Bước 2: Đăng Ký VNPay Sandbox

1. Truy cập: **https://sandbox.vnpayment.vn/devreg/**
2. Đăng ký tài khoản merchant
3. Kiểm tra email để lấy:
   - **TmnCode**: 8 ký tự (ví dụ: `2QXUI4J4`)
   - **HashSecret**: Secret key dài

---

## 📋 Bước 3: Cập Nhật Credentials

### Cách 1: User Secrets (Khuyến nghị)

```powershell
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_BackEnd\src

# Init user secrets
dotnet user-secrets init

# Set credentials
dotnet user-secrets set "VNPay:TmnCode" "YOUR_TMN_CODE"
dotnet user-secrets set "VNPay:HashSecret" "YOUR_HASH_SECRET"
```

### Cách 2: appsettings.json (Không an toàn)

Mở `src/appsettings.json` và cập nhật:
```json
"VNPay": {
  "TmnCode": "YOUR_TMN_CODE",
  "HashSecret": "YOUR_HASH_SECRET",
  ...
}
```

---

## 📋 Bước 4: Start Backend

```powershell
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_BackEnd\src
dotnet run
```

Backend sẽ chạy tại: **http://localhost:5000**

---

## 📋 Bước 5: Start ngrok

**Mở Terminal/CMD mới** (giữ Backend đang chạy):

```powershell
ngrok http 5000
```

Output sẽ hiển thị:
```
Session Status                online
Account                       ...
Version                       3.x.x
Region                        United States (us)
Latency                       12ms
Web Interface                 http://127.0.0.1:4040
Forwarding                    https://abc123-def456.ngrok.io -> http://localhost:5000
```

**Copy URL**: `https://abc123-def456.ngrok.io`

---

## 📋 Bước 6: Cập Nhật IpnUrl trong Config

Mở `src/appsettings.json` và cập nhật:

```json
"VNPay": {
  "TmnCode": "YOUR_TMN_CODE",
  "HashSecret": "YOUR_HASH_SECRET",
  "PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
  "ReturnUrl": "http://localhost:3000/payment/vnpay/return",
  "IpnUrl": "https://abc123-def456.ngrok.io/api/Payment/vnpay/webhook",
  "QueryUrl": "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction",
  ...
}
```

**Thay đổi**: `IpnUrl` = ngrok URL + `/api/Payment/vnpay/webhook`

⚠️ **Lưu ý**: Mỗi lần restart ngrok, URL sẽ thay đổi. Cần update lại IpnUrl!

---

## 📋 Bước 7: Start Frontend

```powershell
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_FrontEnd
npm start
```

Frontend sẽ chạy tại: **http://localhost:3000**

---

## 📋 Bước 8: Test Payment Flow

### 1. Tạo Order
- Đăng nhập frontend
- Chọn event
- Chọn vé và số lượng
- Chọn VNPay payment method
- Submit order

### 2. Kiểm Tra QR Code
- Page `/payment/vnpay/:orderId` hiển thị QR code
- Copy payment URL để test

### 3. Thanh Toán
- Quét QR hoặc click vào payment URL
- Redirect đến VNPay Sandbox
- Dùng test card từ VNPay
- Complete payment

### 4. Kiểm Tra Kết Quả
- User được redirect về `/payment/vnpay/return`
- Webhook được trigger (check ngrok dashboard: http://127.0.0.1:4040)
- Check backend logs
- Check database: `Payment` table và `Order` table

---

## 📋 Bước 9: Verify Webhook

### Xem ngrok Dashboard
1. Mở: http://127.0.0.1:4040
2. Click tab **"Requests"**
3. Tìm request đến `/api/Payment/vnpay/webhook`
4. Xem:
   - Request payload (VNPay gửi gì)
   - Response (backend trả về "SUCCESS" hay "FAIL")

### Check Backend Logs
```powershell
# Trong terminal chạy dotnet run
# Sẽ thấy logs như:
Creating VNPay payment for order 123
VNPay payment created: PaymentId=1, TxnRef=ORDER_123_20250131123000
Processing VNPay webhook for TxnRef=ORDER_123_20250131123000
Payment succeeded for Order 123
```

### Check Database
```sql
-- Check Payment
SELECT * FROM Payment WHERE OrderId = 123

-- Check Order
SELECT * FROM [Order] WHERE OrderId = 123
```

---

## 🔧 Troubleshooting

### ❌ Webhook không được gọi
- **Nguyên nhân**: ngrok URL thay đổi hoặc sai
- **Giải pháp**: Copy URL mới từ ngrok, update IpnUrl trong config, restart backend

### ❌ Webhook trả về "FAIL"
- **Nguyên nhân**: Signature validation failed hoặc HashSecret sai
- **Giải pháp**: Check HashSecret trong config khớp với VNPay Sandbox

### ❌ Return URL không hoạt động
- **Nguyên nhân**: Route chưa được add hoặc user chưa login
- **Giải pháp**: Check `src/App.js` đã có route `/payment/vnpay/return`

### ❌ Order status không cập nhật
- **Nguyên nhân**: Webhook xử lý lỗi hoặc order không tồn tại
- **Giải pháp**: Check backend logs, verify orderId đúng

---

## 📝 Checklist Hoàn Thành

- [ ] ngrok đã cài đặt và chạy
- [ ] VNPay Sandbox account đã đăng ký
- [ ] Credentials đã set (User Secrets hoặc appsettings.json)
- [ ] IpnUrl đã update với ngrok URL
- [ ] Backend đang chạy (localhost:5000)
- [ ] ngrok đang forward (http://localhost:5000)
- [ ] Frontend đang chạy (localhost:3000)
- [ ] Đã test tạo order và thanh toán
- [ ] Webhook được gọi (check ngrok dashboard)
- [ ] Database đã cập nhật đúng

---

## 🚀 Ready for Production

Khi deploy production:
1. Thay VNPay Sandbox → VNPay Production
2. Update `PaymentUrl`, `QueryUrl` sang production URLs
3. Update `ReturnUrl`, `IpnUrl` sang domain thật
4. Dùng credentials production
5. Không cần ngrok (dùng SSL certificate cho HTTPS)

---

## 📚 Resources

- VNPay Sandbox: https://sandbox.vnpayment.vn/apis/
- VNPay Payment API: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
- ngrok Docs: https://ngrok.com/docs

