# 🎫 HỆ THỐNG ĐẶT VÉ - KẾ HOẠCH TRIỂN KHAI CHI TIẾT

**Mục tiêu:** Xây dựng hệ thống đặt vé hoàn chỉnh theo flow từ hình ảnh tham khảo, với đầy đủ các tính năng: chọn vé (có/không virtual stage), câu hỏi sự kiện, thông tin nhận vé, và thanh toán VNPay QR.

**Ngày bắt đầu:** 01/11/2024

---

## 📋 TỔNG QUAN FLOW

### Flow chính:
1. **TicketSelectionPage** → Chọn vé (Virtual Stage hoặc danh sách)
2. **OrderInformationPage** → Điền câu hỏi hoặc chọn phương thức thanh toán
3. **RecipientInformationPage** → Điền thông tin nhận vé
4. **VNPayPaymentPage** → Thanh toán QR Code
5. **OrderConfirmationPage** → Xác nhận thành công

### Dữ liệu cần xử lý:
- EventQuestion (câu hỏi sự kiện)
- OrderAnswers (JSON: đáp án người dùng)
- VenueLayout với hasVirtualStage
- Countdown Timer cho các bước
- Voucher/Discount
- Payment Gateway VNPay

---

## ✅ PHẦN 1: BACKEND - DATABASE & MODELS

### 1.1 Models ✅
- [x] **EventQuestion.cs**
  - Tạo model trong `src/Models/EventQuestion.cs`
  - Fields: QuestionId, EventId, QuestionText, QuestionType, IsRequired, Options (JSON), ValidationRules, DisplayOrder
  - Navigation: Event

- [x] **Order.cs**
  - Thêm field: `OrderAnswers` (string, nullable) - JSON lưu đáp án: `{questionId: answer}`

- [x] **Event.cs**
  - Thêm navigation: `ICollection<EventQuestion> EventQuestions`
  - Đã có: `VenueLayout` với `hasVirtualStage`

### 1.2 Database Context ✅
- [x] **EventDBContext.cs**
  - Add DbSet: `EventQuestions`
  - Add table mapping: `EventQuestion`
  - Add relationship method: `ConfigureEventQuestionRelationships`
  - Config: Cascade delete khi xóa Event

### 1.3 Database Migration ✅
- [x] **Migration File** ✅
  - ✅ Created: `20251101000000_AddEventQuestionAndOrderAnswers.cs`
  - ✅ Designer file created
  - ⏳ Pending: Apply to database when backend is stopped
  ```bash
  # Apply migration (run this when backend is stopped)
  dotnet ef database update
  ```
  - ✅ Tạo table EventQuestion với đầy đủ fields
  - ✅ Add column OrderAnswers vào table Order

### 1.4 DTOs ✅
- [x] **EventQuestionDTOs.cs** ✅
  - ✅ Created: `src/DTOs/EventQuestionDTOs.cs`
  - ✅ DTOs: CreateEventQuestionDTO, UpdateEventQuestionDTO, EventQuestionDTO, GetEventQuestionsByEventIdDTO

- [x] **OrderDTOs.cs** ✅
  - ✅ OrderAnswers field đã được thêm vào Order model
  - ⏳ Order DTOs sẽ được update khi cần hiển thị answers

---

## ✅ PHẦN 2: BACKEND - API ENDPOINTS

### 2.1 EventQuestion Controller ✅
- [x] **EventQuestionController.cs** ✅
  - ✅ Created: `src/Controllers/EventQuestionController.cs`
  - ✅ Route: /api/EventQuestion
  - ✅ GET /by-event/{eventId}: Get danh sách câu hỏi
  - ✅ GET /{questionId}: Get chi tiết 1 câu hỏi
  - ✅ POST /: Create câu hỏi mới (chỉ Host)
  - ✅ PUT /{questionId}: Update câu hỏi (chỉ Host)
  - ✅ DELETE /{questionId}: Delete câu hỏi (chỉ Host)

### 2.2 EventQuestion Service & Repository ✅
- [x] **IEventQuestionService.cs** ✅
  - ✅ Created interface with full CRUD methods

- [x] **EventQuestionService.cs** ✅
  - ✅ Implement IEventQuestionService
  - ✅ Inject IEventQuestionRepository, IEventRepository
  - ✅ Validate quyền Host, validate data
  - ✅ Handle JSON serialization cho Options

- [x] **IEventQuestionRepository.cs** ✅
  - ✅ Created interface with full repository methods

- [x] **EventQuestionRepository.cs** ✅
  - ✅ Implement IEventQuestionRepository
  - ✅ Query với Include(Event)

### 2.3 Order Controller Updates ⏳
- [ ] **OrderController.cs** (update existing)
  - ⏳ Update POST /api/Order để support OrderAnswers
  - Logic validation sẽ được thêm sau

### 2.4 Register Services ✅
- [x] **ServiceCollectionExtensions.cs** ✅
  - ✅ Added IEventQuestionRepository registration
  - ✅ Added IEventQuestionService registration

---

## ✅ PHẦN 3: FRONTEND - COMPONENTS

### 3.1 Reusable Components ✅
- [x] **CountdownTimer.jsx** ✅
  - ✅ Created: `components/common/CountdownTimer.jsx`
  - ✅ Props: duration, onExpire, size, format
  - ✅ Features: Real-time countdown, visual warning colors, auto onExpire
  - ✅ Design: Red/Yellow/Green based on time left

- [x] **OrderSummaryCard.jsx** ✅
  - ✅ Created: `components/common/OrderSummaryCard.jsx`
  - ✅ Props: orderItems, subtotal, discount, total
  - ✅ Display: List tickets, pricing, voucher discount, total

- [x] **QuestionnaireForm.jsx** ✅
  - ✅ Created: `components/common/QuestionnaireForm.jsx`
  - ✅ Props: questions, onAnswersChange, initialAnswers
  - ✅ Supports: Text, Number, Email, Phone, Date, Radio, Checkbox, Dropdown
  - ✅ Validation real-time với error handling
  - ✅ Required field indicators

### 3.2 Page Components ⏳

#### 3.2.1 TicketSelectionPage ✅
- [x] **TicketSelectionPage.jsx** ✅
  - ✅ Created: `pages/TicketSelectionPage.jsx`
  - ✅ Layout có Virtual Stage và không Virtual Stage
  - ✅ Render StageViewer khi có virtual stage
  - ✅ Ticket type selector với quantity controls
  - ✅ Navigation to /order-information/:orderId
  - ✅ Responsive design

#### 3.2.2 OrderInformationPage ✅
- [x] **OrderInformationPage.jsx** ✅
  - ✅ Created: `pages/OrderInformationPage.jsx`
  - ✅ Route: /order-information/:orderId
  - ✅ Countdown Timer 15 minutes
  - ✅ Conditional rendering: QuestionnaireForm hoặc PaymentMethods
  - ✅ OrderSummaryCard bên phải
  - ✅ API calls: GET Order, GET EventQuestions, PUT Order
  - ✅ Navigation to /recipient-info/:orderId

#### 3.2.3 RecipientInformationPage ⏳
- [ ] **RecipientInformationPage.jsx** (refactor từ PaymentPage)
  ```jsx
  Route: /recipient-info/:orderId
  
  Layout:
  - Left: Form
      - Recipient Name
      - Phone Number
      - Email
      - Delivery Address (optional, cho physical tickets)
  - Right: OrderSummaryCard
  
  Features:
  - Pre-fill từ user profile
  - Validate phone, email
  - Address selector cho Vietnam provinces
  
  Actions:
  - handleContinue: update order → navigate to /payment/vnpay/:orderId
  ```

#### 3.2.4 VNPayPaymentPage ⏳
- [ ] **VNPayPaymentPage.jsx** (tạo mới)
  ```jsx
  Route: /payment/vnpay/:orderId
  
  Layout:
  - Top: Countdown Timer (10 minutes)
  - Left: Order Info
      - Order total
      - Order ID
      - Event info
  - Center: QR Code
      - VNPay QR code
      - Scan to Pay instruction
      - Cancel button
  - Right: Payment Guide
  
  Features:
  - Generate VNPay QR từ order details
  - Poll payment status mỗi 3 seconds
  - Auto redirect khi paid
  - Warning messages
  
  Countdown:
  - 10 minutes cho payment
  - onExpire: auto cancel, redirect to home
  
  API calls:
  - POST /api/Payment/vnpay/create (generate QR)
  - GET /api/Payment/status/:paymentId (poll status)
  ```

#### 3.2.5 Update existing pages ⏳
- [ ] **OrderConfirmationPage.jsx** (update nếu cần)
  - Thêm hiển thị OrderAnswers nếu có
  - Show QR code ticket

---

## ✅ PHẦN 4: FRONTEND - SERVICES & UTILITIES

### 4.1 API Client Updates ⏳
- [ ] **apiClient.js** (update)
  ```javascript
  // Add EventQuestion API
  export const eventQuestionsAPI = {
      getByEventId: (eventId) => apiClient.get(`/EventQuestion/by-event/${eventId}`),
      getById: (questionId) => apiClient.get(`/EventQuestion/${questionId}`),
      create: (data) => apiClient.post('/EventQuestion', data),
      update: (questionId, data) => apiClient.put(`/EventQuestion/${questionId}`, data),
      delete: (questionId) => apiClient.delete(`/EventQuestion/${questionId}`)
  };
  
  // Update Order API
  ordersAPI.create: Include OrderAnswers field
  ordersAPI.updateAnswers: (orderId, answers) => apiClient.patch(`/Order/${orderId}/answers`, { answers })
  
  // Add Payment API
  export const paymentAPI = {
      createVNPayQR: (orderId) => apiClient.post(`/Payment/vnpay/create`, { orderId }),
      getStatus: (paymentId) => apiClient.get(`/Payment/status/${paymentId}`),
      cancelPayment: (paymentId) => apiClient.post(`/Payment/${paymentId}/cancel`)
  };
  ```

### 4.2 Utilities ⏳
- [ ] **questionHelpers.js** (tạo mới)
  ```javascript
  // Validation helpers
  export const validateAnswer = (question, answer) => { ... }
  export const validateAllAnswers = (questions, answers) => { ... }
  
  // Format helpers
  export const formatQuestionOptions = (options) => { ... }
  ```

---

## ✅ PHẦN 5: ROUTING & NAVIGATION

### 5.1 Routes ⏳
- [ ] **App.js** (update)
  ```javascript
  Routes to add/update:
  - /ticket-selection/:eventId (rename từ /create-order/:id)
  - /order-information/:orderId (new)
  - /recipient-info/:orderId (rename từ /payment/:orderId)
  - /payment/vnpay/:orderId (new)
  - /order-confirmation/:orderId (existing)
  ```

### 5.2 Navigation Flow ⏳
- [ ] Update all navigate calls:
  - EventDetailsPage → /ticket-selection/:eventId
  - TicketSelectionPage → /order-information/:orderId
  - OrderInformationPage → /recipient-info/:orderId
  - RecipientInformationPage → /payment/vnpay/:orderId
  - VNPayPaymentPage → /order-confirmation/:orderId

---

## ✅ PHẦN 6: TESTING & VALIDATION

### 6.1 Backend Testing ⏳
- [ ] Unit Tests:
  - EventQuestionService tests
  - EventQuestionRepository tests
  - OrderAnswers validation tests

- [ ] Integration Tests:
  - Create order với answers
  - Fetch questions by event
  - Update order answers

### 6.2 Frontend Testing ⏳
- [ ] Component Tests:
  - CountdownTimer: countdown logic
  - QuestionnaireForm: validation, rendering
  - OrderSummaryCard: calculations

- [ ] E2E Tests:
  - Complete booking flow without questions
  - Complete booking flow with questions
  - Virtual stage selection flow
  - VNPay QR payment flow
  - Countdown expiration handling

### 6.3 Manual Testing Checklist ⏳
- [ ] **Scenario 1:** Event KHÔNG có Virtual Stage, KHÔNG có questions
  1. Browse events → Click event
  2. Chọn vé → Chọn số lượng → Apply voucher
  3. Click "Tạo đơn hàng"
  4. See payment methods → Chọn method
  5. Điền thông tin nhận vé
  6. See VNPay QR → Pay
  7. See confirmation

- [ ] **Scenario 2:** Event CÓ Virtual Stage, CÓ questions
  1. Browse events → Click event với stage
  2. Chọn khu vực trên Virtual Stage
  3. Chọn số lượng
  4. See order info → Điền answers cho questions
  5. Điền thông tin nhận vé
  6. Pay via QR
  7. See confirmation with answers

- [ ] **Scenario 3:** Countdown expires
  1. Start booking flow
  2. Wait for countdown to expire
  3. Auto redirect + order canceled

---

## 📊 TIẾN ĐỘ TỔNG QUAN

**Backend:** 4/15 tasks completed (27%)
- ✅ Models created
- ✅ Database context configured
- ⏳ Migration pending
- ⏳ DTOs pending
- ⏳ Controllers pending
- ⏳ Services pending

**Frontend:** 0/15 tasks completed (0%)
- ⏳ Components pending
- ⏳ Pages pending
- ⏳ Services pending
- ⏳ Routing pending

**Overall:** 4/30 tasks completed (13%)

---

## 🎯 ƯU TIÊN TRIỂN KHAI

### Phase 1: Backend Foundation (HIGH PRIORITY)
1. ✅ Create EventQuestion model
2. ✅ Update Order model
3. ✅ Configure database context
4. ⏳ Create migration
5. ⏳ Create DTOs
6. ⏳ Basic CRUD API cho EventQuestion
7. ⏳ Update Order API

### Phase 2: Frontend Components (MEDIUM PRIORITY)
8. ⏳ CountdownTimer component
9. ⏳ QuestionnaireForm component
10. ⏳ OrderSummaryCard component

### Phase 3: Page Integration (HIGH PRIORITY)
11. ⏳ TicketSelectionPage (refactor existing)
12. ⏳ OrderInformationPage (new)
13. ⏳ RecipientInformationPage (refactor existing)
14. ⏳ VNPayPaymentPage (new)

### Phase 4: Polish & Testing (LOW PRIORITY)
15. ⏳ Routing updates
16. ⏳ Error handling improvements
17. ⏳ Unit tests
18. ⏳ E2E tests
19. ⏳ Manual testing
20. ⏳ Documentation

---

## 📝 NOTES

### Design Decisions:
1. **OrderAnswers as JSON:** Lưu flexible, không cần table phức tạp
2. **Countdown Timer:** 15 min cho order info, 10 min cho payment
3. **Conditional Rendering:** Show questions OR payment methods (not both)
4. **Virtual Stage:** Reuse existing StageViewer component

### Technical Debt:
- ⚠️ Order table thiếu EventId (cần migration thêm)
- ⚠️ Payment Gateway VNPay integration chưa có
- ⚠️ Address selector cho Vietnam provinces

### Future Enhancements:
- Multi-language support cho questions
- Rich text questions với images
- Conditional questions (show/hide based on answers)
- Payment method preferences
- Saved addresses cho users

---

**Last Updated:** 01/11/2024
**Status:** In Progress
**Assigned To:** [TBD]

