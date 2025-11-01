# 🎯 VNPay Test - READY!

## ✅ **SẴN SÀNG TEST NGAY!**

---

## 🔌 **Current Setup**

| Service | URL/Info | Status |
|---------|----------|--------|
| Backend | http://localhost:5000 | ✅ Running |
| ngrok | https://fca7f250a1f4.ngrok-free.app | ✅ Active |
| Frontend | http://localhost:3000 | ⏳ Waiting |
| VNPay TmnCode | MWLVVNSI | ✅ Configured |
| VNPay HashSecret | EKIAHBF7RO6RMX1CQSXZTDYSAFOBC9E1 | ✅ Configured |

---

## 🚀 **Test Steps**

### **1. Start Frontend** (if not running)
```powershell
cd C:\Users\ASUS\source\repos\TheGrind5_EventManagement_FrontEnd
npm start
```

### **2. Create Order**
1. Login → Chọn Event → Chọn vé → **Payment: VNPay**
2. Click "Đặt vé"
3. QR code sẽ hiển thị

### **3. Make Payment**
1. Click payment URL hoặc quét QR
2. Redirect đến VNPay Sandbox
3. Dùng test card → Complete payment

### **4. Verify**
- ✅ ngrok dashboard: http://localhost:4040
- ✅ Check webhook received
- ✅ Check payment = "SUCCESS"

---

## 📊 **Monitor**

### **ngrok Dashboard**
```
URL: http://localhost:4040
→ Tab "Requests"
→ Tìm: /api/Payment/vnpay/webhook
→ Click để xem payload
```

### **Backend Logs**
```
Payment created → TxnRef generated
Webhook called → Signature validated
Order updated → Status: Paid
```

### **Database Check**
```sql
-- Recent VNPay payments
SELECT TOP 5 * FROM Payment 
WHERE Method = 'VNPay' 
ORDER BY CreatedAt DESC

-- Check order status
SELECT OrderId, Status, PaymentMethod 
FROM [Order] 
WHERE PaymentMethod = 'VNPay'
ORDER BY CreatedAt DESC
```

---

## 🔍 **Quick Debug**

| Issue | Fix |
|-------|-----|
| ❌ Webhook không gọi | Check ngrok đang chạy, IpnUrl đúng |
| ❌ Webhook = "FAIL" | Check HashSecret, TmnCode đúng |
| ❌ No response | Check backend logs, signature validation |
| ❌ Order không update | Check database, webhook processing |

---

## 🎊 **Success Indicators**

✅ QR code hiển thị  
✅ Redirect đến VNPay Sandbox  
✅ Webhook received (ngrok dashboard)  
✅ Response = "SUCCESS"  
✅ Database updated  
✅ Order status = "Paid"  

---

## 📝 **API Test**

### **Create Payment**
```bash
POST http://localhost:5000/api/Payment/vnpay/create
Authorization: Bearer YOUR_TOKEN
Body: {
  "orderId": 1,
  "returnUrl": "http://localhost:3000/payment/vnpay/return"
}
```

### **Check Status**
```bash
GET http://localhost:5000/api/Payment/1/status
Authorization: Bearer YOUR_TOKEN
```

---

## ✅ **All Systems GO!**

**Backend**: ✅ Ready  
**ngrok**: ✅ Active  
**Credentials**: ✅ Configured  
**Code**: ✅ Complete  

**→ CHỈ CẦN TEST!** 🚀

