# Kế Hoạch Chi Tiết Triển Khai Webhook VNPay Sandbox

## 📋 Mục Tiêu
Triển khai chức năng Webhook VNPay Sandbox để nhận và xử lý thông báo thanh toán từ VNPay, đảm bảo đơn hàng được cập nhật trạng thái tự động khi thanh toán thành công/thất bại.

**Nguồn tham khảo chính thức**: [VNPay Sandbox Documentation](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html)

---

## 🚀 Quick Start

> **LƯU Ý**: File này là kế hoạch tổng quan. Để xem **CODE MẪU CHI TIẾT** đầy đủ, vui lòng xem file:
> 
> **[VNPAY_IMPLEMENTATION_CODE_GUIDE.md](./VNPAY_IMPLEMENTATION_CODE_GUIDE.md)**
> 
> File đó chứa toàn bộ code ready-to-use có thể copy-paste trực tiếp!

---

## 🔍 Phân Tích Hiện Trạng

### Đã Có Sẵn:
1. **Frontend**: 
   - `VNPayPaymentPage.jsx` - đã có UI để hiển thị QR code
   - API calls: `paymentAPI.createVNPayQR()`, `paymentAPI.getStatus()`, `paymentAPI.cancelPayment()`
   - Route: `/payment/vnpay/:orderId`

2. **Backend**:
   - Model `Payment.cs` - đã có cấu trúc database
   - Model `Order.cs` - đã có field `PaymentMethod`
   - `OrderController.cs` - có endpoint `/payment` nhưng chỉ hỗ trợ Wallet

3. **Database**:
   - Table `Payments` với các field: PaymentId, OrderId, Amount, Method, Status, PaymentDate

### Chưa Có:
1. **PaymentController** - Controller xử lý VNPay payment
2. **VNPayService** - Service tích hợp với VNPay API
3. **VNPay DTOs** - Data Transfer Objects cho VNPay requests/responses
4. **VNPay Configuration** - Cấu hình trong appsettings.json
5. **Webhook Endpoint** - Endpoint nhận IPN từ VNPay
6. **VNPay Helper** - Utilities để tạo hash, validate signature

---

## 📦 Các Thành Phần Cần Triển Khai

### 1. **Cấu Hình VNPay (Configuration)**

#### 1.1. Cập nhật `appsettings.json`
```json
{
  "VNPay": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://yourdomain.com/payment/vnpay/return",
    "IpnUrl": "https://yourdomain.com/api/Payment/vnpay/webhook",
    "QueryUrl": "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction",
    "Command": "pay",
    "CurrCode": "VND",
    "Version": "2.1.0",
    "Locale": "vn",
    "TimeZoneId": "SE Asia Standard Time",
    "OrderType": "other"
  }
}
```

**Nguồn tham khảo**: [VNPay Sandbox Payment API](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html)

#### 1.2. NuGet Packages Cần Thiết

Trước khi bắt đầu, cần cài đặt các packages sau:

```bash
# QR Code generation
dotnet add package QRCoder

# HMAC/SHA512 cryptography (đã có sẵn trong .NET)
# Không cần install thêm
```

#### 1.3. Tạo VNPay Configuration Class
- **File**: `src/Constants/VNPayConstants.cs`
- **Mục đích**: Lưu các hằng số và helper methods cho VNPay

---

### 2. **DTOs (Data Transfer Objects)**

#### 2.1. PaymentDTOs.cs - Thêm các DTOs:
```csharp
// Request để tạo VNPay payment
public record CreateVNPayPaymentRequest
{
    public int OrderId { get; init; }
    public string? ReturnUrl { get; init; }
}

// Response sau khi tạo payment
public record CreateVNPayPaymentResponse
{
    public int PaymentId { get; init; }
    public string PaymentUrl { get; init; }
    public string QrCodeUrl { get; init; }
    public DateTime ExpiredAt { get; init; }
}

// Webhook data từ VNPay (IPN URL và ReturnURL)
// Tham khảo: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
public record VNPayWebhookData
{
    // Tham số bắt buộc
    public string vnp_TmnCode { get; init; }              // Mã website của merchant
    public long vnp_Amount { get; init; }                 // Số tiền (đã nhân 100, VD: 1000000 = 10,000 VND)
    public string vnp_BankCode { get; init; }            // Mã ngân hàng thanh toán
    public string? vnp_BankTranNo { get; init; }         // Mã giao dịch tại ngân hàng
    public string? vnp_CardType { get; init; }           // Loại thẻ thanh toán
    public string vnp_PayDate { get; init; }             // Thời gian thanh toán (yyyyMMddHHmmss)
    public string vnp_OrderInfo { get; init; }           // Thông tin đơn hàng
    public string vnp_TransactionNo { get; init; }       // Mã giao dịch tại VNPay
    public string vnp_ResponseCode { get; init; }         // Mã phản hồi kết quả thanh toán
    public string vnp_TransactionStatus { get; init; }    // Mã kết quả giao dịch tại VNPay
    public string vnp_TxnRef { get; init; }              // Mã tham chiếu giao dịch merchant
    public string vnp_SecureHash { get; init; }          // Mã kiểm tra (checksum) để đảm bảo dữ liệu không bị thay đổi
    public string? vnp_SecureHashType { get; init; }     // Loại mã kiểm tra (SHA256 hoặc MD5)
    
    // Các tham số tùy chọn khác (nếu có)
    public string? vnp_CreateDate { get; init; }         // Thời gian tạo giao dịch
    public string? vnp_IpAddr { get; init; }            // IP khách hàng
    public string? vnp_CurrCode { get; init; }          // Mã tiền tệ
}

// Payment status response
public record PaymentStatusResponse
{
    public int PaymentId { get; init; }
    public string Status { get; init; }
    public string? TransactionId { get; init; }
}
```

---

### 3. **VNPay Service**

#### 3.1. Interface: `IVNPayService.cs`
**File**: `src/Business/IVNPayService.cs`

```csharp
public interface IVNPayService
{
    // Tạo payment URL và QR code
    Task<CreateVNPayPaymentResponse> CreatePaymentAsync(int orderId, string returnUrl);
    
    // Xử lý webhook từ VNPay
    Task<bool> ProcessWebhookAsync(VNPayWebhookData webhookData);
    
    // Lấy trạng thái payment
    Task<PaymentStatusResponse?> GetPaymentStatusAsync(int paymentId);
    
    // Hủy payment
    Task<bool> CancelPaymentAsync(int paymentId);
    
    // Validate VNPay signature
    bool ValidateSignature(VNPayWebhookData webhookData);
}
```

#### 3.2. Implementation: `VNPayService.cs`
**File**: `src/Services/VNPayService.cs`

**Các chức năng chính:**
1. **CreatePaymentAsync**: 
   - Tạo payment record trong DB với status "Initiated"
   - Tạo payment URL với VNPay parameters
   - Generate QR code image từ payment URL (dùng QRCoder)
   - Convert QR code sang base64 hoặc lưu file
   - Return payment data (URL và QR code)

2. **ProcessWebhookAsync**:
   - Validate signature từ VNPay
   - Kiểm tra `vnp_ResponseCode` (00 = success)
   - Cập nhật Payment status trong DB
   - Cập nhật Order status (Paid/Failed)
   - Log transaction

3. **ValidateSignature**:
   - Loại bỏ `vnp_SecureHash` và `vnp_SecureHashType` khỏi parameters
   - Sắp xếp các tham số theo thứ tự alphabet (a-z)
   - Tạo query string từ các tham số đã sắp xếp
   - Tạo HMAC SHA512 hash với `HashSecret`
   - So sánh với `vnp_SecureHash` nhận được
   - **Lưu ý**: Sử dụng HMAC SHA512, không phải SHA512 thuần

4. **Helper Methods**:
   - `CreatePaymentUrl()` - Tạo payment URL với đầy đủ parameters
   - `CreateHash()` - Tạo HMAC SHA512 hash
   - `SortAndBuildQueryString()` - Sắp xếp params và tạo query string
   - `ExtractOrderIdFromTxnRef()` - Parse orderId từ vnp_TxnRef
   - `ConvertAmountToVnPayFormat()` - Chuyển đổi amount (nhân 100)
   - `ConvertAmountFromVnPayFormat()` - Chuyển đổi amount (chia 100)

---

### 4. **Payment Controller**

#### 4.1. `PaymentController.cs`
**File**: `src/Controllers/PaymentController.cs`

**Các Endpoints:**

1. **POST `/api/Payment/vnpay/create`**
   - Input: `CreateVNPayPaymentRequest`
   - Output: `CreateVNPayPaymentResponse`
   - Authentication: Required
   - Logic:
     - Validate order tồn tại và thuộc về user
     - Check order status = "Pending"
     - Call `VNPayService.CreatePaymentAsync()`
     - Return payment URL và QR code

2. **POST `/api/Payment/vnpay/webhook`** ⚠️ **QUAN TRỌNG**
   - Input: Query parameters từ VNPay (có thể là GET hoặc POST với query string)
   - Output: HTTP 200 OK (text/plain) với message **"SUCCESS"** hoặc **"FAIL"**
   - Authentication: **KHÔNG CẦN** (public endpoint, VNPay sẽ gọi từ internet)
   - Logic:
     - Parse query parameters thành `VNPayWebhookData`
     - **Bắt buộc**: Validate signature (bỏ qua nếu signature không hợp lệ)
     - Check `vnp_ResponseCode`:
       - "00" = Giao dịch thành công
       - Khác = Giao dịch thất bại (xem bảng mã lỗi bên dưới)
     - Check `vnp_TransactionStatus`:
       - "00" = Giao dịch thành công tại VNPay
       - Khác = Kiểm tra chi tiết mã lỗi
     - Call `VNPayService.ProcessWebhookAsync()`
     - Return **"SUCCESS"** nếu xử lý thành công, **"FAIL"** nếu lỗi
   - **Lưu ý**: 
     - VNPay yêu cầu response là plain text "SUCCESS" hoặc "FAIL" (KHÔNG phải JSON)
     - VNPay sẽ retry nếu không nhận được "SUCCESS"
     - Nên xử lý async và return response ngay lập tức

3. **GET `/api/Payment/{paymentId}/status`**
   - Input: paymentId
   - Output: `PaymentStatusResponse`
   - Authentication: Required
   - Logic:
     - Check user có quyền xem payment (qua Order)
     - Return payment status

4. **POST `/api/Payment/{paymentId}/cancel`**
   - Input: paymentId
   - Output: Success/Error message
   - Authentication: Required
   - Logic:
     - Check user có quyền hủy payment
     - Update payment status = "Failed"
     - Có thể cancel order nếu cần

---

### 5. **Repository Layer**

#### 5.1. Tạo mới `IPaymentRepository.cs`
**File**: `src/Repositories/IPaymentRepository.cs` (CẦN TẠO MỚI)

```csharp
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null);
        Task<List<Payment>> GetPaymentsByOrderIdAsync(int orderId);
    }
}
```

#### 5.2. Implementation: `PaymentRepository.cs`
**File**: `src/Repositories/PaymentRepository.cs` (CẦN TẠO MỚI)

```csharp
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EventDBContext _context;

        public PaymentRepository(EventDBContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = status;
            if (transactionId != null)
            {
                payment.TransactionId = transactionId;
            }
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}
```

---

### 6. **Database Migrations**

#### 6.1. Kiểm tra Payment Model

**Payment Model hiện tại (từ codebase)**:
```csharp
public partial class Payment
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; }  // "Wallet", "VNPay", etc.
    public string Status { get; set; }  // "Initiated", "Succeeded", "Failed", "Refunded"
    public DateTime PaymentDate { get; set; }
    public virtual Order Order { get; set; }
}
```

**Cần thêm các fields cho VNPay integration**:
- `TransactionId` (string, nullable) - Mã giao dịch từ VNPay (vnp_TransactionNo)
- `VnpTxnRef` (string, nullable) - Mã tham chiếu VNPay (vnp_TxnRef)
- `ResponseCode` (string, nullable) - Mã phản hồi từ VNPay (vnp_ResponseCode)
- `TransactionStatus` (string, nullable) - Mã trạng thái VNPay (vnp_TransactionStatus)
- `CreatedAt` (DateTime) - Thời gian tạo payment
- `UpdatedAt` (DateTime, nullable) - Thời gian cập nhật

#### 6.2. Migration (nếu cần)

**Tạo migration mới để thêm các fields VNPay**:
```bash
dotnet ef migrations add AddVNPayFieldsToPayment
dotnet ef database update
```

**Migration SQL sẽ thêm**:
```sql
ALTER TABLE Payments
ADD TransactionId NVARCHAR(100) NULL,
    VnpTxnRef NVARCHAR(100) NULL,
    ResponseCode NVARCHAR(10) NULL,
    TransactionStatus NVARCHAR(10) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL;
```

---

### 7. **Dependency Injection**

#### 7.1. Cập nhật `ServiceCollectionExtensions.cs`
**File**: `src/Extensions/ServiceCollectionExtensions.cs`

**Cập nhật `AddRepositories()` trong ServiceCollectionExtensions.cs**:
```csharp
public static IServiceCollection AddRepositories(this IServiceCollection services)
{
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IEventRepository, EventRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IEventQuestionRepository, EventQuestionRepository>();
    services.AddScoped<IPaymentRepository, PaymentRepository>(); // ← Thêm dòng này
    return services;
}
```

**Thêm vào `AddApplicationServices()`**:
```csharp
services.AddScoped<IVNPayService, VNPayService>(); // ← Thêm dòng này
```

---

## 🔄 Quy Trình Xử Lý Webhook

### Flow Webhook VNPay (theo tài liệu chính thức):

```
1. User tạo order → Chọn VNPay payment
2. Frontend gọi POST /api/Payment/vnpay/create
3. Backend:
   a. Tạo Payment record (Status: "Initiated")
   b. Tạo vnp_TxnRef (format: ORDER_{OrderId}_{Timestamp})
   c. Build payment URL với các parameters:
      - vnp_Amount (đã nhân 100)
      - vnp_Command = "pay"
      - vnp_CreateDate (yyyyMMddHHmmss)
      - vnp_CurrCode = "VND"
      - vnp_IpAddr (IP khách hàng)
      - vnp_Locale = "vn"
      - vnp_OrderInfo (mô tả đơn hàng)
      - vnp_OrderType = "other"
      - vnp_ReturnUrl (URL merchant nhận kết quả)
      - vnp_TmnCode
      - vnp_TxnRef
      - vnp_Version = "2.1.0"
      - vnp_SecureHash (HMAC SHA512)
   d. Return payment URL và QR code
4. Frontend redirect user đến payment URL hoặc hiển thị QR code
5. User thanh toán trên VNPay
6. VNPay xử lý thanh toán tại Ngân hàng
7. VNPay thực hiện 2 hành động đồng thời:
   a. Chuyển hướng user về ReturnURL với query params
   b. Gọi IPN URL (webhook) với query params
8. Backend xử lý Webhook (IPN URL):
   a. Parse query parameters
   b. Validate signature (BẮT BUỘC):
      - Loại bỏ vnp_SecureHash và vnp_SecureHashType
      - Sắp xếp params theo alphabet
      - Tạo query string
      - Tính HMAC SHA512 với HashSecret
      - So sánh với vnp_SecureHash
   c. Kiểm tra vnp_ResponseCode:
      - "00" = Giao dịch thành công
      - "07" = Trừ tiền thành công nhưng bị nghi ngờ gian lận
      - Khác = Giao dịch thất bại (xem bảng mã lỗi)
   d. Kiểm tra vnp_TransactionStatus:
      - "00" = Giao dịch thành công tại VNPay
      - Khác = Xem bảng mã TransactionStatus
   e. Cập nhật Payment:
      - Status = "Succeeded" (nếu ResponseCode="00") hoặc "Failed"
      - TransactionId = vnp_TransactionNo
      - ResponseCode = vnp_ResponseCode
      - TransactionStatus = vnp_TransactionStatus
   f. Cập nhật Order:
      - Status = "Paid" (nếu thành công) hoặc "Failed"
      - PaymentMethod = "VNPay"
   g. Return "SUCCESS" (nếu xử lý thành công) hoặc "FAIL"
9. VNPay nhận response "SUCCESS" và xác nhận đã gửi thông báo
10. Frontend nhận kết quả từ ReturnURL và hiển thị cho user
```

**Tham khảo**: [VNPay Payment Flow](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html)

---

## 🔐 Bảo Mật Webhook

### 1. Signature Validation
- **Bắt buộc**: Validate `vnp_SecureHash` trong mọi webhook request
- Sử dụng `HashSecret` từ config để tạo hash và so sánh

### 2. IP Whitelist (Optional nhưng khuyến nghị)
- Có thể thêm middleware để check IP của VNPay
- VNPay IP ranges (cần cập nhật từ tài liệu chính thức)

### 3. Idempotency
- Check transaction đã được xử lý chưa (qua TransactionId)
- Tránh duplicate processing

### 4. Logging
- Log tất cả webhook requests (kể cả invalid)
- Lưu raw query string để debug

---

## 📝 Chi Tiết Implementation

### Phase 1: Setup và Configuration
- [ ] 1.1. Đăng ký tài khoản VNPay Sandbox tại [sandbox.vnpayment.vn/devreg](https://sandbox.vnpayment.vn/devreg)
- [ ] 1.2. Lấy TmnCode và HashSecret từ email đăng ký
- [ ] 1.3. Cài đặt NuGet package: `dotnet add package QRCoder`
- [ ] 1.4. Cập nhật appsettings.json với VNPay config
- [ ] 1.5. Tạo VNPayConstants.cs
- [ ] 1.6. Tạo VNPay Configuration class

### Phase 2: Database và Models
- [ ] 2.1. Review Payment model hiện tại
- [ ] 2.2. Thêm các fields mới: TransactionId, VnpTxnRef, ResponseCode, TransactionStatus, CreatedAt, UpdatedAt
- [ ] 2.3. Tạo migration `AddVNPayFieldsToPayment`
- [ ] 2.4. Chạy `dotnet ef database update`
- [ ] 2.5. Tạo IPaymentRepository interface
- [ ] 2.6. Implement PaymentRepository
- [ ] 2.7. Test PaymentRepository với Unit Tests

### Phase 3: DTOs và Business Logic
- [ ] 3.1. Tạo PaymentDTOs.cs với các DTOs cần thiết
- [ ] 3.2. Tạo IVNPayService interface
- [ ] 3.3. Implement VNPayService:
  - [ ] 3.3.1. CreatePaymentAsync()
  - [ ] 3.3.2. ProcessWebhookAsync()
  - [ ] 3.3.3. ValidateSignature()
  - [ ] 3.3.4. Helper methods (CreateHash, CreatePaymentUrl, etc.)

### Phase 4: API Endpoints
- [ ] 4.1. Tạo PaymentController.cs
- [ ] 4.2. Implement POST /api/Payment/vnpay/create
- [ ] 4.3. Implement POST /api/Payment/vnpay/webhook ⚠️
- [ ] 4.4. Implement GET /api/Payment/{id}/status
- [ ] 4.5. Implement POST /api/Payment/{id}/cancel

### Phase 5: Dependency Injection và Testing
- [ ] 5.1. Đăng ký services trong ServiceCollectionExtensions
- [ ] 5.2. Unit tests cho VNPayService
- [ ] 5.3. Integration tests cho PaymentController
- [ ] 5.4. Test webhook với VNPay Sandbox

### Phase 6: Error Handling và Logging
- [ ] 6.1. Error handling trong VNPayService
- [ ] 6.2. Logging webhook requests
- [ ] 6.3. Retry mechanism cho failed webhooks (optional)

---

## 🧪 Testing Checklist

### Unit Tests
- [ ] Test `CreatePaymentAsync()` - tạo payment URL đúng format
- [ ] Test `ValidateSignature()` - validate đúng và sai signature
- [ ] Test `ProcessWebhookAsync()` - xử lý webhook thành công/thất bại
- [ ] Test helper methods (CreateHash, CreatePaymentUrl)

### Integration Tests
- [ ] Test POST /api/Payment/vnpay/create (authenticated)
- [ ] Test POST /api/Payment/vnpay/webhook (unauthenticated, với valid/invalid signature)
- [ ] Test GET /api/Payment/{id}/status
- [ ] Test POST /api/Payment/{id}/cancel

### Manual Testing với VNPay Sandbox
- [ ] Tạo order và chọn VNPay
- [ ] Verify QR code hiển thị đúng
- [ ] Thanh toán thành công → Verify order status = "Paid"
- [ ] Thanh toán thất bại → Verify order status = "Failed"
- [ ] Test webhook với invalid signature → Verify rejected
- [ ] Test duplicate webhook → Verify idempotency

---

## 📚 Tài Liệu Tham Khảo Chính Thức

### 1. Tài Liệu VNPay Sandbox:
- **Thanh toán Pay**: [https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html)
- **Thanh toán trả góp**: [https://sandbox.vnpayment.vn/apis/docs/thanh-toan-installment/installment.html](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-installment/installment.html)
- **Thanh toán bằng mã Token**: [https://sandbox.vnpayment.vn/apis/docs/thanh-toan-token/token.html](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-token/token.html)
- **Truy vấn và hoàn tiền**: [https://sandbox.vnpayment.vn/apis/docs/truy-van-hoan-tien/querydr&refund.html](https://sandbox.vnpayment.vn/apis/docs/truy-van-hoan-tien/querydr&refund.html)
- **Chuyển đổi thuật toán mã hóa**: [https://sandbox.vnpayment.vn/apis/docs/chuyen-doi-thuat-toan/changeTypeHash.html](https://sandbox.vnpayment.vn/apis/docs/chuyen-doi-thuat-toan/changeTypeHash.html)

### 2. Các Tham Số Quan Trọng:

#### 2.1. vnp_ResponseCode (Mã phản hồi kết quả thanh toán):
- **"00"**: Giao dịch thành công
- **"07"**: Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường)
- **"09"**: Thẻ/Tài khoản chưa đăng ký dịch vụ InternetBanking
- **"10"**: Xác thực thông tin thẻ/tài khoản không đúng quá 3 lần
- **"11"**: Đã hết hạn chờ thanh toán
- **"12"**: Thẻ/Tài khoản bị khóa
- **"13"**: Nhập sai mật khẩu xác thực giao dịch (OTP)
- **"24"**: Khách hàng hủy giao dịch
- **"51"**: Tài khoản không đủ số dư để thực hiện giao dịch
- **"65"**: Tài khoản đã vượt quá hạn mức giao dịch trong ngày
- **"75"**: Ngân hàng thanh toán đang bảo trì
- **"79"**: Nhập sai mật khẩu thanh toán quá số lần quy định
- **"99"**: Các lỗi khác

#### 2.2. vnp_TransactionStatus (Mã kết quả giao dịch tại VNPay):
- **"00"**: Giao dịch thành công
- **"01"**: Giao dịch chưa hoàn tất
- **"02"**: Giao dịch bị lỗi
- **"04"**: Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPay)
- **"05"**: VNPay đang xử lý giao dịch này (GD hoàn tiền)
- **"06"**: VNPay đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)
- **"07"**: Giao dịch bị nghi ngờ gian lận
- **"09"**: GD Hoàn trả bị từ chối

#### 2.3. Các Tham Số Bắt Buộc Khi Tạo Payment URL:
- `vnp_Version`: "2.1.0"
- `vnp_Command`: "pay"
- `vnp_TmnCode`: Mã website merchant (8 ký tự)
- `vnp_Amount`: Số tiền (đã nhân 100, không có ký tự phân cách)
- `vnp_CreateDate`: yyyyMMddHHmmss (GMT+7)
- `vnp_CurrCode`: "VND"
- `vnp_IpAddr`: IP khách hàng
- `vnp_Locale`: "vn"
- `vnp_OrderInfo`: Thông tin đơn hàng
- `vnp_OrderType`: "other" hoặc mã loại hàng hóa
- `vnp_ReturnUrl`: URL merchant nhận kết quả
- `vnp_TxnRef`: Mã tham chiếu giao dịch (unique, max 100 chars)
- `vnp_SecureHash`: HMAC SHA512 hash

#### 2.4. Format vnp_TxnRef:
- Tối đa 100 ký tự, phải unique
- Nên dùng format: `ORDER_{OrderId}_{Timestamp}` để dễ parse lại OrderId
- Ví dụ: `ORDER_123_20250127153000`

#### 2.5. vnp_SecureHash (Signature):
- **Thuật toán**: HMAC SHA512 (không phải SHA512 thuần)
- **Cách tính**:
  1. Loại bỏ `vnp_SecureHash` và `vnp_SecureHashType` khỏi parameters
  2. Sắp xếp các tham số theo thứ tự alphabet (a-z)
  3. Tạo query string: `key1=value1&key2=value2&...`
  4. Tính HMAC SHA512 với `HashSecret`
  5. Convert sang hex string (lowercase)

---

## ⚠️ Lưu Ý Quan Trọng

1. **Webhook Endpoint (IPN URL)**:
   - Phải là public URL (không cần auth)
   - VNPay chỉ gọi được nếu server accessible từ internet
   - Development: Dùng ngrok, localtunnel, hoặc ssh tunnel để test
   - Format URL: `https://yourdomain.com/api/Payment/vnpay/webhook`
   - VNPay gọi qua HTTP POST hoặc GET với query string
   - Không gửi body, chỉ query parameters

2. **Response Format**:
   - VNPay yêu cầu response là plain text: "SUCCESS" hoặc "FAIL"
   - Không phải JSON

3. **Timeout**:
   - VNPay có timeout cho webhook response (thường 30s)
   - Nên xử lý async và return response ngay, sau đó process
   - Nếu timeout, VNPay sẽ retry webhook

4. **ReturnURL vs IPN URL**:
   - **ReturnURL**: User được redirect về sau khi thanh toán (dùng để hiển thị kết quả cho user)
   - **IPN URL (Webhook)**: VNPay gọi từ server để cập nhật kết quả thanh toán (dùng để xử lý logic nghiệp vụ)
   - **Quan trọng**: Nên xử lý logic nghiệp vụ ở IPN URL, ReturnURL chỉ để hiển thị kết quả

5. **Testing**:
   - VNPay Sandbox có thể test với test cards
   - Đăng ký tài khoản tại: [https://sandbox.vnpayment.vn/devreg/](https://sandbox.vnpayment.vn/devreg/)
   - Cần verify với real payments trước khi production

6. **Security**:
   - **Bắt buộc**: Validate signature trong mọi webhook request
   - Không bao giờ bỏ qua signature validation
   - Log tất cả webhook requests để audit
   - Kiểm tra `vnp_TmnCode` phải khớp với config

7. **Error Handling**:
   - Webhook có thể retry nhiều lần nếu không nhận được "SUCCESS"
   - Implement idempotency để tránh duplicate processing
   - Check transaction đã được xử lý chưa (qua `vnp_TransactionNo`) trước khi xử lý lại

8. **Amount Format**:
   - **Khi gửi đến VNPay**: Số tiền phải nhân 100 (VD: 10,000 VND → 1000000)
   - **Khi nhận từ VNPay**: Số tiền đã nhân 100, cần chia 100 để hiển thị
   - Lưu ý: Không có dấu phẩy, chấm, hoặc ký tự phân cách

9. **Date Format**:
   - `vnp_CreateDate`: yyyyMMddHHmmss (GMT+7, không có dấu phân cách)
   - `vnp_PayDate`: yyyyMMddHHmmss (GMT+7)
   - Ví dụ: 20250127153000 = 27/01/2025 15:30:00 (GMT+7)

10. **QR Code Generation**:
    - VNPay không tự động tạo QR code image
    - Cần dùng thư viện generate QR code từ payment URL
    - Suggestion: Dùng `QRCoder` NuGet package cho .NET
    - Hoặc: Dùng API bên thứ 3 như QR Server, Google Charts API

11. **Idempotency**:
    - Check `vnp_TransactionNo` đã tồn tại trong DB chưa
    - Nếu đã xử lý → return "SUCCESS" ngay, không xử lý lại
    - Tránh duplicate notifications từ VNPay

12. **ReturnURL Handling**:
    - ReturnURL KHÔNG nên chứa logic nghiệp vụ
    - Chỉ dùng để hiển thị kết quả cho user
    - Tất cả logic update DB nên ở IPN URL (webhook)
    - ReturnURL có thể redirect đến OrderConfirmationPage với orderId

13. **Error Response**:
    - Webhook phải return ngay "SUCCESS" hoặc "FAIL"
    - Nếu return "FAIL", VNPay sẽ retry nhiều lần
    - Nên xử lý async sau khi return "SUCCESS"
    - Log errors để debug

---

## 📊 Timeline Ước Tính

- **Phase 1**: 1-2 giờ (Setup, Configuration, Đăng ký Sandbox)
- **Phase 2**: 2-3 giờ (Database, Migrations, PaymentRepository)
- **Phase 3**: 4-6 giờ (DTOs, Business Logic, VNPayService)
- **Phase 4**: 3-4 giờ (API Endpoints, PaymentController)
- **Phase 5**: 2-3 giờ (DI, Unit Tests, Integration Tests)
- **Phase 6**: 1-2 giờ (Error Handling, Logging, Documentation)

**Tổng**: ~13-20 giờ (tùy vào kinh nghiệm và mức độ chi tiết)

---

## 🚀 Deployment Checklist

- [ ] Cập nhật appsettings với production VNPay credentials
- [ ] Cập nhật IpnUrl và ReturnUrl với production domain
- [ ] Test webhook endpoint với VNPay Sandbox production
- [ ] Setup monitoring cho webhook endpoint
- [ ] Setup alerts cho failed webhooks
- [ ] Document API endpoints cho team
- [ ] Update API documentation (Swagger)

---

---

## 📋 Danh Sách Tham Số Đầy Đủ

### Tham Số Gửi Đến VNPay (Create Payment URL):

| Tham số | Kiểu dữ liệu | Bắt buộc | Mô tả | Ví dụ |
|---------|--------------|----------|-------|-------|
| vnp_Version | Alphanumeric[1,8] | ✅ | Phiên bản API | "2.1.0" |
| vnp_Command | Alpha[1,16] | ✅ | Mã API | "pay" |
| vnp_TmnCode | Alphanumeric[8] | ✅ | Mã website merchant | "2QXUI4J4" |
| vnp_Amount | Numeric[1,12] | ✅ | Số tiền (đã nhân 100) | "1000000" (10,000 VND) |
| vnp_CreateDate | Numeric[14] | ✅ | Thời gian tạo (yyyyMMddHHmmss GMT+7) | "20250127153000" |
| vnp_CurrCode | Alpha[3] | ✅ | Mã tiền tệ | "VND" |
| vnp_IpAddr | Alphanumeric | ✅ | IP khách hàng | "127.0.0.1" |
| vnp_Locale | Alpha[2,5] | ✅ | Ngôn ngữ | "vn" |
| vnp_OrderInfo | Alphanumeric[0,255] | ✅ | Thông tin đơn hàng | "Thanh toan don hang #123" |
| vnp_OrderType | Alphanumeric[0,100] | ✅ | Loại hàng hóa | "other" |
| vnp_ReturnUrl | Alphanumeric[0,255] | ✅ | URL nhận kết quả | "https://domain.com/return" |
| vnp_TxnRef | Alphanumeric[1,100] | ✅ | Mã tham chiếu | "ORDER_123_20250127153000" |
| vnp_BankCode | Alphanumeric[3,20] | ⚪ | Mã ngân hàng | "VNPAYQR", "VNBANK", "INTCARD" |
| vnp_ExpireDate | Numeric[14] | ⚪ | Thời gian hết hạn | "20250127163000" |
| vnp_SecureHash | Alphanumeric | ✅ | Chữ ký điện tử (HMAC SHA512) | "abc123..." |

### Tham Số Nhận Từ VNPay (Webhook/IPN):

Tất cả tham số trên + các tham số bổ sung:
- `vnp_BankCode`: Mã ngân hàng thanh toán
- `vnp_BankTranNo`: Mã giao dịch tại ngân hàng
- `vnp_CardType`: Loại thẻ
- `vnp_PayDate`: Thời gian thanh toán
- `vnp_TransactionNo`: Mã giao dịch tại VNPay
- `vnp_ResponseCode`: Mã phản hồi kết quả
- `vnp_TransactionStatus`: Mã kết quả giao dịch
- `vnp_SecureHashType`: Loại mã kiểm tra (SHA256)

**Nguồn tham khảo**: [VNPay Payment API Documentation](https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html)

---

---

## 🎯 Quick Start Checklist

### Trước Khi Bắt Đầu:
1. [ ] Đăng ký VNPay Sandbox account
2. [ ] Lấy credentials (TmnCode, HashSecret)
3. [ ] Setup ngrok/localtunnel cho local development
4. [ ] Install QRCoder NuGet package

### Testing Webhook Locally:
```bash
# Dùng ngrok (Windows)
ngrok http 5000

# Hoặc localtunnel (Node.js)
npx localtunnel --port 5000

# Copy public URL vào IpnUrl config
```

---

**Tác giả**: AI Assistant  
**Ngày tạo**: 2025-01-27  
**Cập nhật**: 2025-01-27 (Thêm chi tiết từ tài liệu chính thức + code examples)  
**Version**: 2.0

