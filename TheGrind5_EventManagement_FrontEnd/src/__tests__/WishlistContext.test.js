/**
 * Jest Unit Tests for Shopping Cart's addItem() function
 * Tests the WishlistContext addItem functionality
 */

import { renderHook, act } from '@testing-library/react';
import { WishlistProvider, useWishlist } from '../contexts/WishlistContext';
import { wishlistAPI } from '../services/apiClient';
import { useAuth } from '../contexts/AuthContext';

// Mock external dependencies
jest.mock('../services/apiClient');
jest.mock('../contexts/AuthContext');

describe('Shopping Cart addItem() Function', () => {
  let mockWishlistAPI;
  let mockUseAuth;

  beforeEach(() => {
    // Setup mocks before each test
    mockWishlistAPI = {
      getWishlist: jest.fn(),
      addItem: jest.fn(),
      updateItem: jest.fn(),
      deleteItem: jest.fn(),
    };
    
    mockUseAuth = {
      user: { id: 1, name: 'Test User' }
    };

    // Configure mocks
    wishlistAPI.getWishlist = mockWishlistAPI.getWishlist;
    wishlistAPI.addItem = mockWishlistAPI.addItem;
    wishlistAPI.updateItem = mockWishlistAPI.updateItem;
    wishlistAPI.deleteItem = mockWishlistAPI.deleteItem;
    
    useAuth.mockReturnValue(mockUseAuth);

    // Clear all mocks
    jest.clearAllMocks();
  });

  afterEach(() => {
    // Cleanup after each test
    jest.resetAllMocks();
  });

  describe('1. Add new item successfully', () => {
    test('should add new item to wishlist when item does not exist', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [
            {
              id: 1,
              ticketTypeId: 123,
              quantity: 2,
              ticketName: 'VIP Ticket',
              price: 100
            }
          ],
          totalItems: 1,
          totalPrice: 200
        }
      };

      mockWishlistAPI.addItem.mockResolvedValue({ success: true });
      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(123, 2);
      });

      // Assert
      expect(addResult).toBe(true);
      expect(mockWishlistAPI.addItem).toHaveBeenCalledWith(123, 2);
      expect(mockWishlistAPI.getWishlist).toHaveBeenCalled();
      expect(result.current.wishlist).toEqual(mockWishlistData.data);
    });

    test('should add new item with default quantity of 1', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [
            {
              id: 1,
              ticketTypeId: 456,
              quantity: 1,
              ticketName: 'Standard Ticket',
              price: 50
            }
          ],
          totalItems: 1,
          totalPrice: 50
        }
      };

      mockWishlistAPI.addItem.mockResolvedValue({ success: true });
      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(456); // No quantity specified
      });

      // Assert
      expect(addResult).toBe(true);
      expect(mockWishlistAPI.addItem).toHaveBeenCalledWith(456, 1);
    });
  });

  describe('2. Add to existing item (update quantity)', () => {
    test('should update quantity when item already exists in wishlist', async () => {
      // Arrange
      const existingWishlistData = {
        data: {
          items: [
            {
              id: 1,
              ticketTypeId: 123,
              quantity: 1,
              ticketName: 'VIP Ticket',
              price: 100
            }
          ],
          totalItems: 1,
          totalPrice: 100
        }
      };

      const updatedWishlistData = {
        data: {
          items: [
            {
              id: 1,
              ticketTypeId: 123,
              quantity: 3, // Updated quantity
              ticketName: 'VIP Ticket',
              price: 100
            }
          ],
          totalItems: 1,
          totalPrice: 300
        }
      };

      // Mock initial wishlist fetch
      mockWishlistAPI.getWishlist.mockResolvedValueOnce(existingWishlistData);
      // Mock addItem call
      mockWishlistAPI.addItem.mockResolvedValue({ success: true });
      // Mock updated wishlist fetch
      mockWishlistAPI.getWishlist.mockResolvedValueOnce(updatedWishlistData);

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Wait for initial wishlist load
      await act(async () => {
        await result.current.fetchWishlist();
      });

      // Act - Add more quantity to existing item
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(123, 2);
      });

      // Assert
      expect(addResult).toBe(true);
      expect(mockWishlistAPI.addItem).toHaveBeenCalledWith(123, 2);
      expect(mockWishlistAPI.getWishlist).toHaveBeenCalledTimes(2); // Initial + after add
    });
  });

  describe('3. Reject negative quantity', () => {
    test('should handle negative quantity gracefully', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [],
          totalItems: 0,
          totalPrice: 0
        }
      };

      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);
      // Mock addItem to throw error for negative quantity
      mockWishlistAPI.addItem.mockRejectedValue(new Error('Quantity must be positive'));

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(123, -1);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(result.current.error).toBe('Quantity must be positive');
      expect(mockWishlistAPI.addItem).toHaveBeenCalledWith(123, -1);
    });

    test('should handle zero quantity', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [],
          totalItems: 0,
          totalPrice: 0
        }
      };

      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);
      mockWishlistAPI.addItem.mockRejectedValue(new Error('Quantity must be greater than 0'));

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(123, 0);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(result.current.error).toBe('Quantity must be greater than 0');
    });
  });

  describe('4. Reject null product', () => {
    test('should handle null ticketTypeId', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [],
          totalItems: 0,
          totalPrice: 0
        }
      };

      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);
      mockWishlistAPI.addItem.mockRejectedValue(new Error('Ticket type ID is required'));

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(null, 1);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(result.current.error).toBe('Ticket type ID is required');
      expect(mockWishlistAPI.addItem).toHaveBeenCalledWith(null, 1);
    });

    test('should handle undefined ticketTypeId', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [],
          totalItems: 0,
          totalPrice: 0
        }
      };

      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);
      mockWishlistAPI.addItem.mockRejectedValue(new Error('Ticket type ID is required'));

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(undefined, 1);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(result.current.error).toBe('Ticket type ID is required');
    });

    test('should handle empty string ticketTypeId', async () => {
      // Arrange
      const mockWishlistData = {
        data: {
          items: [],
          totalItems: 0,
          totalPrice: 0
        }
      };

      mockWishlistAPI.getWishlist.mockResolvedValue(mockWishlistData);
      mockWishlistAPI.addItem.mockRejectedValue(new Error('Invalid ticket type ID'));

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem('', 1);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(result.current.error).toBe('Invalid ticket type ID');
    });
  });

  describe('Additional Edge Cases', () => {
    test('should handle API network error', async () => {
      // Arrange
      mockWishlistAPI.addItem.mockRejectedValue(new Error('Network error'));

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(123, 1);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(result.current.error).toBe('Network error');
    });

    test('should handle user not authenticated', async () => {
      // Arrange
      useAuth.mockReturnValue({ user: null });

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      let addResult;
      await act(async () => {
        addResult = await result.current.addItem(123, 1);
      });

      // Assert
      expect(addResult).toBe(false);
      expect(mockWishlistAPI.addItem).not.toHaveBeenCalled();
    });

    test('should set loading state during addItem operation', async () => {
      // Arrange
      let resolveAddItem;
      const addItemPromise = new Promise((resolve) => {
        resolveAddItem = resolve;
      });
      
      mockWishlistAPI.addItem.mockReturnValue(addItemPromise);
      mockWishlistAPI.getWishlist.mockResolvedValue({ data: { items: [], totalItems: 0, totalPrice: 0 } });

      const wrapper = ({ children }) => (
        <WishlistProvider>{children}</WishlistProvider>
      );

      const { result } = renderHook(() => useWishlist(), { wrapper });

      // Act
      act(() => {
        result.current.addItem(123, 1);
      });

      // Assert - Check loading state
      expect(result.current.loading).toBe(true);

      // Complete the operation
      await act(async () => {
        resolveAddItem({ success: true });
        await addItemPromise;
      });

      expect(result.current.loading).toBe(false);
    });
  });
});
