# 🎯 Unit Testing Prompts Guide - TheGrind5 Event Management

## 📋 **Tổng quan**
File này liệt kê các prompt được sử dụng để tạo ra hệ thống unit testing hoàn chỉnh cho TheGrind5 Event Management System.

---

## 🚀 **Phase 1: Phân tích và Lập kế hoạch**

### **Prompt 1: Feature Analysis**
```
Hãy làm 1 file md để phân tích và chọn feature, như trên ảnh
```

**Kết quả:** Tạo `Feature_Analysis_Selection.md` - Phân tích các tính năng chính của hệ thống

### **Prompt 2: Shopping Cart Analysis**
```
Analyze this Shopping Cart class and identify all functions that need unit testing: 
For each function, identify: 
1. Main functionality 
2. Input parameters and types 
3. Expected return values 
4. Potential edge cases 
5. Dependencies that need mocking
```

**Kết quả:** Tạo `Shopping_Cart_Unit_Testing_Analysis.md` - Phân tích chi tiết các function cần test

---

## 🧪 **Phase 2: Tạo Unit Tests cho Backend (.NET)**

### **Prompt 3: Test Requirements Specification**
```
MEDIUM 👤 Người 4 - TicketService + Controller 
Trách nhiệm: TicketService extended + OrderController - User experience và API endpoints 
Lớp: TicketService + OrderController 
Số test cases: 10 

Methods cần test: 
public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(int userId) 
✅ GetUserTicketsAsync_ValidUser_ReturnsTickets Test case thành công: Lấy vé của user 
✅ GetUserTicketsAsync_Empty_ReturnsEmptyList Test case thành công: User chưa có vé 
❌ GetUserTicketsAsync_InvalidUser_ThrowsException Test case lỗi: User không hợp lệ 

[HttpPost] public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request) 
✅ CreateOrder_Success_ReturnsOk Test case thành công: Tạo order thành công 
❌ CreateOrder_Unauthorized_ReturnsUnauthorized Test case lỗi: Không có quyền truy cập 
❌ CreateOrder_InvalidRequest_ReturnsBadRequest Test case lỗi: Dữ liệu request không hợp lệ 
⚠️ CreateOrder_ConcurrentPurchase_HandlesRaceCondition Test case đặc biệt: Xử lý mua vé đồng thời 

[HttpPost("{id}/payment")] public async Task<IActionResult> ProcessPayment(int id, [FromBody] PaymentRequest request) 
✅ ProcessPayment_Wallet_Success Test case thành công: Thanh toán qua ví thành công 
❌ ProcessPayment_InsufficientBalance_ReturnsBadRequest Test case lỗi: Số dư ví không đủ 

Hãy hướng dẫn tôi làm những cái này
```

**Kết quả:** Tạo test project và implement các test cases

### **Prompt 4: Fix Failing Tests**
```
Fix 2 cái test bị lỗi đã
```

**Kết quả:** Sửa lỗi trong `TicketService` và test cases

### **Prompt 5: Run Tests**
```
Giờ muốn test thì mở file nào?
```

**Kết quả:** Hướng dẫn cách chạy tests

### **Prompt 6: Execute Tests**
```
Không ấy thì bạn mở cho mình test luôn đi
```

**Kết quả:** Chạy tests và hiển thị kết quả

---

## 🎨 **Phase 3: Tạo Unit Tests cho Frontend (JavaScript/React)**

### **Prompt 7: Jest Test Template Request**
```
"Create Jest unit tests for Shopping Cart's addItem() function with these test cases:

1. Add new item successfully
2. Add to existing item (update quantity)
3. Reject negative quantity
4. Reject null product

Requirements:
- Use Jest framework
- Include setup/teardown
- Use proper assertions (toEqual, toThrow)
- Add descriptive test names
- Mock any external dependencies"
```

**Kết quả:** Tạo Jest test suite cho Shopping Cart

### **Prompt 8: Template-based Test Code**
```
// shoppingCart.test.js
const ShoppingCart = require('./shoppingCart');
describe('ShoppingCart - addItem', () => {
  let cart;
  beforeEach(() => {
    cart = new ShoppingCart('user123');
  });
  describe('Happy Path', () => {
    test('should add new item to empty cart', () => {
      const product = { id: 1, name: 'Laptop', price: 1000 };
      cart.addItem(product, 2);
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0]).toEqual({ id: 1, name: 'Laptop', price: 1000, quantity: 2 });
    });
    test('should update quantity for existing item', () => {
      const product = { id: 1, name: 'Laptop', price: 1000 };
      cart.addItem(product, 2);
      cart.addItem(product, 3);
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(5);
    });
  });
  describe('Error Cases', () => {
    test('should throw error for negative quantity', () => {
      const product = { id: 1, name: 'Laptop', price: 1000 };
      expect(() => cart.addItem(product, -5))
        .toThrow('Invalid input');
    });
    test('should throw error for null product', () => {
      expect(() => cart.addItem(null, 1))
        .toThrow('Invalid input');
    });
  });
});
Viết theo mẫu này nhé
```

**Kết quả:** Tạo test code theo đúng template yêu cầu

---

## 🔧 **Phase 4: Troubleshooting và Optimization**

### **Prompt 9: Test Execution**
```
Giờ hãy chạy test suite
```

**Kết quả:** Chạy và debug test suite

### **Prompt 10: Error Analysis**
```
Phân tích xem có lỗi gì không
```

**Kết quả:** Phân tích và sửa lỗi trong test configuration

### **Prompt 11: File Analysis**
```
Có những file cs nào được tạo mới hay chỉnh sửa không
```

**Kết quả:** Liệt kê các file C# đã được tạo/sửa

---

## 📝 **Phase 5: Documentation**

### **Prompt 12: Documentation Request**
```
Bây giờ bạn hãy viết tài liệu vào file md đi
```

**Kết quả:** Tạo `Unit_Testing_Guide.md` với hướng dẫn chi tiết

### **Prompt 13: Prompts Documentation**
```
Hãy liệt kê những promPT để tạo ra được sản phẩm như thế này, viết vào file md
```

**Kết quả:** Tạo file này - `Unit_Testing_Prompts_Guide.md`

---

## 🎯 **Prompt Patterns hiệu quả**

### **1. Specific Requirements Pattern**
```
"Create [Technology] unit tests for [Component] with these test cases:
1. [Specific case 1]
2. [Specific case 2]
3. [Error case 1]
4. [Error case 2]

Requirements:
- Use [Framework]
- Include [Specific features]
- Use proper [Assertions]
- Add [Documentation requirements]
- Mock [Dependencies]"
```

### **2. Template-based Pattern**
```
"[Code template]
Viết theo mẫu này nhé"
```

### **3. Problem-solving Pattern**
```
"Fix [Specific issue]"
"Phân tích xem có lỗi gì không"
"Giờ hãy chạy [Action]"
```

### **4. Documentation Pattern**
```
"Bây giờ bạn hãy viết tài liệu vào file md đi"
"Hãy liệt kê những [Specific items]"
```

---

## 📊 **Kết quả đạt được**

### **Backend (.NET)**
- ✅ **21/21 tests PASSED** (100% success rate)
- ✅ **3 test files** được tạo
- ✅ **1 source file** được sửa
- ✅ **Comprehensive coverage** cho TicketService và OrderController

### **Frontend (JavaScript)**
- ✅ **6/6 tests PASSED** (100% success rate)
- ✅ **Multiple test files** được tạo
- ✅ **Jest configuration** hoàn chỉnh
- ✅ **Template-based implementation**

### **Documentation**
- ✅ **4 markdown files** với hướng dẫn chi tiết
- ✅ **README files** cho từng component
- ✅ **Troubleshooting guides**
- ✅ **Best practices documentation**

---

## 🚀 **Best Practices cho Prompts**

### **1. Be Specific**
- ✅ "Create Jest unit tests for Shopping Cart's addItem() function"
- ❌ "Create some tests"

### **2. Provide Examples**
- ✅ Include code templates
- ✅ Show expected output format
- ✅ Give specific test cases

### **3. Set Clear Requirements**
- ✅ List specific frameworks to use
- ✅ Define test coverage requirements
- ✅ Specify error handling needs

### **4. Use Iterative Approach**
- ✅ Start with analysis
- ✅ Build incrementally
- ✅ Fix issues as they arise
- ✅ Document everything

### **5. Ask for Documentation**
- ✅ Request markdown files
- ✅ Ask for README files
- ✅ Get troubleshooting guides

---

## 🎉 **Tóm tắt**

**13 prompts chính** đã tạo ra một hệ thống unit testing hoàn chỉnh với:
- **27+ test cases** (Backend + Frontend)
- **100% success rate**
- **Comprehensive documentation**
- **Production-ready code**

**🎯 Kết luận: Với các prompt patterns này, bạn có thể tạo ra bất kỳ hệ thống unit testing nào một cách hiệu quả!** 🚀
