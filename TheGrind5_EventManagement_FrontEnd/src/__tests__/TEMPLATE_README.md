# 🧪 AI Generated Test Code - Template Version

## 📁 **Files theo template:**

### ✅ **1. Test File chính**
- `shoppingCart.test.js` - Test file theo đúng template bạn cung cấp
- `shoppingCart.js` - Class implementation để test

## 🎯 **Test Structure theo template:**

```javascript
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
```

## 🚀 **Cách chạy test theo template:**

### **Method 1: Chạy test template**
```bash
cd TheGrind5_EventManagement_FrontEnd
npm run test:template
```

### **Method 2: Chạy trực tiếp**
```bash
npx jest src/__tests__/shoppingCart.test.js --verbose
```

### **Method 3: Chạy với watch mode**
```bash
npx jest src/__tests__/shoppingCart.test.js --watch
```

## 📊 **Test Cases theo template:**

### **Happy Path** ✅
- ✅ Add new item to empty cart
- ✅ Update quantity for existing item

### **Error Cases** ✅
- ✅ Throw error for negative quantity
- ✅ Throw error for null product
- ✅ Throw error for zero quantity
- ✅ Throw error for undefined product

### **Edge Cases** ✅
- ✅ Handle multiple different products
- ✅ Handle adding same product multiple times
- ✅ Handle default quantity of 1

## 🎯 **Features của template version:**

- ✅ **Exact match** với template bạn cung cấp
- ✅ **CommonJS require** syntax
- ✅ **Jest testing framework**
- ✅ **beforeEach** setup
- ✅ **describe/test** structure
- ✅ **expect assertions**
- ✅ **Error throwing** tests
- ✅ **Clean, simple** code

## 📈 **Expected Results:**

```
PASS src/__tests__/shoppingCart.test.js
  ShoppingCart - addItem
    Happy Path
      ✓ should add new item to empty cart
      ✓ should update quantity for existing item
    Error Cases
      ✓ should throw error for negative quantity
      ✓ should throw error for null product
      ✓ should throw error for zero quantity
      ✓ should throw error for undefined product
    Edge Cases
      ✓ should handle multiple different products
      ✓ should handle adding same product multiple times
      ✓ should handle default quantity of 1

Test Suites: 1 passed, 1 total
Tests:       9 passed, 9 total
```

## 🎉 **Tóm tắt:**

**Template version** này được viết **chính xác** theo mẫu bạn cung cấp:
- ✅ Cùng cấu trúc code
- ✅ Cùng syntax
- ✅ Cùng test cases
- ✅ Cùng assertions
- ✅ Cùng error handling

**Chạy ngay bây giờ:**
```bash
npm run test:template
```

**🎯 Đây chính là AI Generated Test Code theo đúng template của bạn!** 🚀
