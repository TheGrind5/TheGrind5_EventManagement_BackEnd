/**
 * AI Generated Test Code for ShoppingCart Class
 * Based on the provided template - Pure JavaScript Class Implementation
 */

// Mock ShoppingCart class implementation for testing
class ShoppingCart {
  constructor(userId) {
    this.userId = userId;
    this.items = [];
  }

  addItem(product, quantity = 1) {
    // Input validation
    if (!product || product === null) {
      throw new Error('Invalid input: Product cannot be null');
    }
    
    if (quantity <= 0) {
      throw new Error('Invalid input: Quantity must be greater than 0');
    }
    
    if (!Number.isInteger(quantity)) {
      throw new Error('Invalid input: Quantity must be a whole number');
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

  removeItem(productId) {
    this.items = this.items.filter(item => item.id !== productId);
    return this.items;
  }

  getTotalPrice() {
    return this.items.reduce((total, item) => total + (item.price * item.quantity), 0);
  }

  getTotalItems() {
    return this.items.reduce((total, item) => total + item.quantity, 0);
  }

  clear() {
    this.items = [];
    return this.items;
  }
}

describe('ShoppingCart - addItem', () => {
  let cart;

  beforeEach(() => {
    cart = new ShoppingCart('user123');
  });

  describe('Happy Path', () => {
    test('should add new item to empty cart', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act
      cart.addItem(product, 2);
      
      // Assert
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0]).toEqual({ 
        id: 1, 
        name: 'Laptop', 
        price: 1000, 
        quantity: 2 
      });
    });

    test('should update quantity for existing item', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act
      cart.addItem(product, 2);
      cart.addItem(product, 3);
      
      // Assert
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(5);
    });

    test('should add multiple different items', () => {
      // Arrange
      const product1 = { id: 1, name: 'Laptop', price: 1000 };
      const product2 = { id: 2, name: 'Mouse', price: 25 };
      
      // Act
      cart.addItem(product1, 1);
      cart.addItem(product2, 2);
      
      // Assert
      expect(cart.items).toHaveLength(2);
      expect(cart.items[0]).toEqual({ id: 1, name: 'Laptop', price: 1000, quantity: 1 });
      expect(cart.items[1]).toEqual({ id: 2, name: 'Mouse', price: 25, quantity: 2 });
    });

    test('should use default quantity of 1 when not specified', () => {
      // Arrange
      const product = { id: 1, name: 'Keyboard', price: 75 };
      
      // Act
      cart.addItem(product);
      
      // Assert
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(1);
    });
  });

  describe('Error Cases', () => {
    test('should throw error for negative quantity', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act & Assert
      expect(() => cart.addItem(product, -5))
        .toThrow('Invalid input: Quantity must be greater than 0');
    });

    test('should throw error for null product', () => {
      // Act & Assert
      expect(() => cart.addItem(null, 1))
        .toThrow('Invalid input: Product cannot be null');
    });

    test('should throw error for zero quantity', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act & Assert
      expect(() => cart.addItem(product, 0))
        .toThrow('Invalid input: Quantity must be greater than 0');
    });

    test('should throw error for undefined product', () => {
      // Act & Assert
      expect(() => cart.addItem(undefined, 1))
        .toThrow('Invalid input: Product cannot be null');
    });

    test('should throw error for decimal quantity', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act & Assert
      expect(() => cart.addItem(product, 2.5))
        .toThrow('Invalid input: Quantity must be a whole number');
    });

    test('should throw error for empty string product', () => {
      // Act & Assert
      expect(() => cart.addItem('', 1))
        .toThrow('Invalid input: Product cannot be null');
    });
  });

  describe('Edge Cases', () => {
    test('should handle very large quantity', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act
      cart.addItem(product, 999999);
      
      // Assert
      expect(cart.items[0].quantity).toBe(999999);
    });

    test('should handle product with zero price', () => {
      // Arrange
      const product = { id: 1, name: 'Free Item', price: 0 };
      
      // Act
      cart.addItem(product, 5);
      
      // Assert
      expect(cart.items[0].price).toBe(0);
      expect(cart.items[0].quantity).toBe(5);
    });

    test('should handle product with negative price', () => {
      // Arrange
      const product = { id: 1, name: 'Discounted Item', price: -100 };
      
      // Act
      cart.addItem(product, 2);
      
      // Assert
      expect(cart.items[0].price).toBe(-100);
      expect(cart.items[0].quantity).toBe(2);
    });

    test('should handle adding same product multiple times', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      // Act
      cart.addItem(product, 1);
      cart.addItem(product, 2);
      cart.addItem(product, 3);
      
      // Assert
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(6); // 1 + 2 + 3
    });
  });

  describe('Integration with other methods', () => {
    test('should work with getTotalPrice method', () => {
      // Arrange
      const product1 = { id: 1, name: 'Laptop', price: 1000 };
      const product2 = { id: 2, name: 'Mouse', price: 25 };
      
      // Act
      cart.addItem(product1, 2); // 2 * 1000 = 2000
      cart.addItem(product2, 3); // 3 * 25 = 75
      
      // Assert
      expect(cart.getTotalPrice()).toBe(2075);
    });

    test('should work with getTotalItems method', () => {
      // Arrange
      const product1 = { id: 1, name: 'Laptop', price: 1000 };
      const product2 = { id: 2, name: 'Mouse', price: 25 };
      
      // Act
      cart.addItem(product1, 2);
      cart.addItem(product2, 3);
      
      // Assert
      expect(cart.getTotalItems()).toBe(5); // 2 + 3
    });

    test('should work with removeItem method', () => {
      // Arrange
      const product1 = { id: 1, name: 'Laptop', price: 1000 };
      const product2 = { id: 2, name: 'Mouse', price: 25 };
      
      cart.addItem(product1, 2);
      cart.addItem(product2, 3);
      
      // Act
      cart.removeItem(1);
      
      // Assert
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].id).toBe(2);
    });

    test('should work with clear method', () => {
      // Arrange
      const product1 = { id: 1, name: 'Laptop', price: 1000 };
      const product2 = { id: 2, name: 'Mouse', price: 25 };
      
      cart.addItem(product1, 2);
      cart.addItem(product2, 3);
      
      // Act
      cart.clear();
      
      // Assert
      expect(cart.items).toHaveLength(0);
    });
  });

  describe('Performance Tests', () => {
    test('should handle adding many items efficiently', () => {
      // Arrange
      const startTime = performance.now();
      
      // Act - Add 1000 different items
      for (let i = 0; i < 1000; i++) {
        const product = { id: i, name: `Product ${i}`, price: Math.random() * 1000 };
        cart.addItem(product, Math.floor(Math.random() * 10) + 1);
      }
      
      const endTime = performance.now();
      const executionTime = endTime - startTime;
      
      // Assert
      expect(cart.items).toHaveLength(1000);
      expect(executionTime).toBeLessThan(1000); // Should complete in less than 1 second
    });

    test('should handle updating same item many times', () => {
      // Arrange
      const product = { id: 1, name: 'Laptop', price: 1000 };
      const startTime = performance.now();
      
      // Act - Add same item 1000 times
      for (let i = 0; i < 1000; i++) {
        cart.addItem(product, 1);
      }
      
      const endTime = performance.now();
      const executionTime = endTime - startTime;
      
      // Assert
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(1000);
      expect(executionTime).toBeLessThan(100); // Should complete in less than 100ms
    });
  });
});
