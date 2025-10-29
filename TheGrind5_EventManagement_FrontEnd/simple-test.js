/**
 * Simple test to verify ShoppingCart functionality
 * Run with: node simple-test.js
 */

// ShoppingCart class implementation
class ShoppingCart {
  constructor(userId) {
    this.userId = userId;
    this.items = [];
  }

  addItem(product, quantity = 1) {
    // Input validation
    if (!product || product === null) {
      throw new Error('Invalid input');
    }
    
    if (quantity <= 0) {
      throw new Error('Invalid input');
    }

    // Check if product already exists
    const existingItemIndex = this.items.findIndex(item => item.id === product.id);
    
    if (existingItemIndex !== -1) {
      // Update existing item quantity
      this.items[existingItemIndex].quantity += quantity;
    } else {
      // Add new item
      this.items.push({
        id: product.id,
        name: product.name,
        price: product.price,
        quantity: quantity
      });
    }
    
    return this.items;
  }
}

// Simple test runner
function runTests() {
  console.log('🧪 Running Shopping Cart Tests...\n');
  
  let passed = 0;
  let failed = 0;
  
  function test(name, testFn) {
    try {
      testFn();
      console.log(`✅ ${name}`);
      passed++;
    } catch (error) {
      console.log(`❌ ${name}: ${error.message}`);
      failed++;
    }
  }
  
  function expect(actual) {
    return {
      toHaveLength: (expected) => {
        if (actual.length !== expected) {
          throw new Error(`Expected length ${expected}, got ${actual.length}`);
        }
      },
      toEqual: (expected) => {
        if (JSON.stringify(actual) !== JSON.stringify(expected)) {
          throw new Error(`Expected ${JSON.stringify(expected)}, got ${JSON.stringify(actual)}`);
        }
      },
      toBe: (expected) => {
        if (actual !== expected) {
          throw new Error(`Expected ${expected}, got ${actual}`);
        }
      },
      toThrow: (expectedError) => {
        try {
          actual();
          throw new Error('Expected function to throw, but it did not');
        } catch (error) {
          if (error.message !== expectedError) {
            throw new Error(`Expected error "${expectedError}", got "${error.message}"`);
          }
        }
      }
    };
  }
  
  // Test 1: Add new item to empty cart
  test('should add new item to empty cart', () => {
    const cart = new ShoppingCart('user123');
    const product = { id: 1, name: 'Laptop', price: 1000 };
    
    cart.addItem(product, 2);
    
    expect(cart.items).toHaveLength(1);
    expect(cart.items[0]).toEqual({ id: 1, name: 'Laptop', price: 1000, quantity: 2 });
  });
  
  // Test 2: Update quantity for existing item
  test('should update quantity for existing item', () => {
    const cart = new ShoppingCart('user123');
    const product = { id: 1, name: 'Laptop', price: 1000 };
    
    cart.addItem(product, 2);
    cart.addItem(product, 3);
    
    expect(cart.items).toHaveLength(1);
    expect(cart.items[0].quantity).toBe(5);
  });
  
  // Test 3: Throw error for negative quantity
  test('should throw error for negative quantity', () => {
    const cart = new ShoppingCart('user123');
    const product = { id: 1, name: 'Laptop', price: 1000 };
    
    expect(() => cart.addItem(product, -5)).toThrow('Invalid input');
  });
  
  // Test 4: Throw error for null product
  test('should throw error for null product', () => {
    const cart = new ShoppingCart('user123');
    
    expect(() => cart.addItem(null, 1)).toThrow('Invalid input');
  });
  
  // Test 5: Handle multiple different products
  test('should handle multiple different products', () => {
    const cart = new ShoppingCart('user123');
    const product1 = { id: 1, name: 'Laptop', price: 1000 };
    const product2 = { id: 2, name: 'Mouse', price: 25 };
    
    cart.addItem(product1, 1);
    cart.addItem(product2, 2);
    
    expect(cart.items).toHaveLength(2);
    expect(cart.items[0]).toEqual({ id: 1, name: 'Laptop', price: 1000, quantity: 1 });
    expect(cart.items[1]).toEqual({ id: 2, name: 'Mouse', price: 25, quantity: 2 });
  });
  
  // Test 6: Handle default quantity of 1
  test('should handle default quantity of 1', () => {
    const cart = new ShoppingCart('user123');
    const product = { id: 1, name: 'Laptop', price: 1000 };
    
    cart.addItem(product);
    
    expect(cart.items).toHaveLength(1);
    expect(cart.items[0].quantity).toBe(1);
  });
  
  // Summary
  console.log(`\n📊 Test Results:`);
  console.log(`✅ Passed: ${passed}`);
  console.log(`❌ Failed: ${failed}`);
  console.log(`📈 Total: ${passed + failed}`);
  console.log(`🎯 Success Rate: ${((passed / (passed + failed)) * 100).toFixed(1)}%`);
  
  if (failed === 0) {
    console.log(`\n🎉 All tests passed! Shopping Cart is working correctly!`);
  } else {
    console.log(`\n⚠️  Some tests failed. Please check the implementation.`);
  }
}

// Run the tests
runTests();
