/**
 * AI Generated Test Code - Shopping Cart addItem() Function
 * Written exactly according to the provided template
 */

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

    test('should throw error for zero quantity', () => {
      const product = { id: 1, name: 'Laptop', price: 1000 };
      expect(() => cart.addItem(product, 0))
        .toThrow('Invalid input');
    });

    test('should throw error for undefined product', () => {
      expect(() => cart.addItem(undefined, 1))
        .toThrow('Invalid input');
    });
  });

  describe('Edge Cases', () => {
    test('should handle multiple different products', () => {
      const product1 = { id: 1, name: 'Laptop', price: 1000 };
      const product2 = { id: 2, name: 'Mouse', price: 25 };
      
      cart.addItem(product1, 1);
      cart.addItem(product2, 2);
      
      expect(cart.items).toHaveLength(2);
      expect(cart.items[0]).toEqual({ id: 1, name: 'Laptop', price: 1000, quantity: 1 });
      expect(cart.items[1]).toEqual({ id: 2, name: 'Mouse', price: 25, quantity: 2 });
    });

    test('should handle adding same product multiple times', () => {
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      cart.addItem(product, 1);
      cart.addItem(product, 2);
      cart.addItem(product, 3);
      
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(6);
    });

    test('should handle default quantity of 1', () => {
      const product = { id: 1, name: 'Laptop', price: 1000 };
      
      cart.addItem(product);
      
      expect(cart.items).toHaveLength(1);
      expect(cart.items[0].quantity).toBe(1);
    });
  });
});