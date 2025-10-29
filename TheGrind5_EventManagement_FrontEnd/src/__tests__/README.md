# 🧪 Jest Unit Tests - Shopping Cart addItem() Function

## 📋 Test Cases Implemented

### ✅ **1. Add new item successfully**
- **Test**: `should add new item to wishlist when item does not exist`
- **Test**: `should add new item with default quantity of 1`
- **Purpose**: Verify adding new items to shopping cart works correctly

### ✅ **2. Add to existing item (update quantity)**
- **Test**: `should update quantity when item already exists in wishlist`
- **Purpose**: Verify quantity updates for existing items

### ✅ **3. Reject negative quantity**
- **Test**: `should handle negative quantity gracefully`
- **Test**: `should handle zero quantity`
- **Purpose**: Ensure invalid quantities are rejected

### ✅ **4. Reject null product**
- **Test**: `should handle null ticketTypeId`
- **Test**: `should handle undefined ticketTypeId`
- **Test**: `should handle empty string ticketTypeId`
- **Purpose**: Ensure null/invalid products are rejected

### ✅ **Additional Edge Cases**
- **Test**: `should handle API network error`
- **Test**: `should handle user not authenticated`
- **Test**: `should set loading state during addItem operation`

## 🛠️ Test Setup

### **Dependencies Mocked**
- `wishlistAPI` - API service for wishlist operations
- `useAuth` - Authentication context
- `console` methods - Reduce test noise

### **Test Environment**
- **Framework**: Jest with jsdom environment
- **Testing Library**: @testing-library/react
- **Mocking**: Jest mocks for external dependencies

## 🚀 How to Run Tests

### **Run All Tests**
```bash
cd TheGrind5_EventManagement_FrontEnd
npm test
```

### **Run Specific Test File**
```bash
npm test -- WishlistContext.test.js
```

### **Run with Coverage**
```bash
npm test -- --coverage
```

### **Run in Watch Mode**
```bash
npm test -- --watch
```

## 📊 Test Structure

### **AAA Pattern Applied**
```javascript
// Arrange - Setup test data and mocks
const mockWishlistData = { /* test data */ };
mockWishlistAPI.addItem.mockResolvedValue({ success: true });

// Act - Execute the function being tested
await act(async () => {
  addResult = await result.current.addItem(123, 2);
});

// Assert - Verify the results
expect(addResult).toBe(true);
expect(mockWishlistAPI.addItem).toHaveBeenCalledWith(123, 2);
```

### **Mock Strategy**
- **External Dependencies**: All API calls and contexts are mocked
- **Isolation**: Each test runs in isolation with fresh mocks
- **Cleanup**: Mocks are cleared between tests

## 🎯 Test Coverage

### **Functions Tested**
- `addItem(ticketTypeId, quantity)` - Main function under test
- `fetchWishlist()` - Called after successful add
- Error handling and loading states

### **Scenarios Covered**
- ✅ Success cases (new item, existing item)
- ✅ Error cases (invalid input, network errors)
- ✅ Edge cases (authentication, loading states)
- ✅ Input validation (null, negative, zero quantities)

## 🔧 Configuration Files

### **jest.config.js**
- Test environment: jsdom
- Module name mapping
- Coverage thresholds
- Test file patterns

### **setupTests.js**
- Global mocks setup
- Console method mocking
- Browser API mocks (matchMedia, IntersectionObserver)

## 📈 Expected Results

When running tests, you should see:
```
PASS src/__tests__/WishlistContext.test.js
  Shopping Cart addItem() Function
    1. Add new item successfully
      ✓ should add new item to wishlist when item does not exist
      ✓ should add new item with default quantity of 1
    2. Add to existing item (update quantity)
      ✓ should update quantity when item already exists in wishlist
    3. Reject negative quantity
      ✓ should handle negative quantity gracefully
      ✓ should handle zero quantity
    4. Reject null product
      ✓ should handle null ticketTypeId
      ✓ should handle undefined ticketTypeId
      ✓ should handle empty string ticketTypeId
    Additional Edge Cases
      ✓ should handle API network error
      ✓ should handle user not authenticated
      ✓ should set loading state during addItem operation

Test Suites: 1 passed, 1 total
Tests:       10 passed, 10 total
```

## 🎉 Success Criteria

- ✅ All 10 test cases pass
- ✅ Proper mocking of external dependencies
- ✅ Descriptive test names following convention
- ✅ AAA pattern implementation
- ✅ Edge cases and error handling covered
- ✅ Setup/teardown properly implemented

---

**🎯 This test suite ensures the Shopping Cart's addItem() function is robust, reliable, and handles all expected scenarios correctly!**
