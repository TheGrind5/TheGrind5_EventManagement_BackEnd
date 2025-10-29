/**
 * ShoppingCart Class Implementation
 * For testing purposes
 */

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

module.exports = ShoppingCart;
