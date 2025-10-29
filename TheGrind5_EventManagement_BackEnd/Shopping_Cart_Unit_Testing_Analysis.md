# Shopping Cart (WishlistService) - Unit Testing Analysis

## 🛒 Class Overview: WishlistService

**File**: `src/Services/WishlistService.cs`  
**Purpose**: Manages user wishlist/shopping cart functionality for event tickets  
**Dependencies**: `IWishlistMapper`, `EventDBContext`, `IOrderService`

---

## 📋 Functions Requiring Unit Testing

### 1. **GetWishlistAsync(int userId)**

#### Main Functionality
- Retrieves all wishlist items for a specific user
- Includes related TicketType and Event data
- Maps to WishlistResponse DTO

#### Input Parameters
- `userId` (int): ID of the user whose wishlist to retrieve

#### Expected Return Values
- `Task<WishlistResponse>`: Contains list of WishlistItemDto and totals

#### Edge Cases
- ✅ User with no wishlist items (empty list)
- ✅ User with multiple wishlist items
- ✅ User with expired/inactive ticket types
- ✅ Database connection failure
- ✅ Invalid userId (negative/zero)

#### Dependencies to Mock
- `_context.Wishlists` (DbSet<Wishlist>)
- `_wishlistMapper.MapToWishlistResponse()`

---

### 2. **AddItemAsync(int userId, AddWishlistItemRequest request)**

#### Main Functionality
- Adds a new item to user's wishlist
- Validates ticket type availability and status
- Handles existing items by updating quantity
- Enforces quantity limits based on available stock

#### Input Parameters
- `userId` (int): ID of the user
- `request` (AddWishlistItemRequest): Contains TicketTypeId and Quantity

#### Expected Return Values
- `Task<WishlistItemDto>`: The added/updated wishlist item
- Throws `ArgumentException` if ticket type not found
- Throws `InvalidOperationException` if ticket not available

#### Edge Cases
- ✅ Adding item that doesn't exist in database
- ✅ Adding item with inactive status
- ✅ Adding item outside sale period
- ✅ Adding item that already exists (quantity update)
- ✅ Adding item with quantity exceeding available stock
- ✅ Adding item with zero/negative quantity
- ✅ Database save failure

#### Dependencies to Mock
- `_context.TicketTypes` (DbSet<TicketType>)
- `_context.Wishlists` (DbSet<Wishlist>)
- `_wishlistMapper.MapToDto()`
- `DateTime.UtcNow` (for time validation)

---

### 3. **UpdateQuantityAsync(int userId, int itemId, UpdateWishlistItemRequest request)**

#### Main Functionality
- Updates quantity of existing wishlist item
- Validates user ownership and ticket availability
- Clamps quantity to available stock

#### Input Parameters
- `userId` (int): ID of the user
- `itemId` (int): ID of the wishlist item to update
- `request` (UpdateWishlistItemRequest): Contains new quantity

#### Expected Return Values
- `Task<WishlistItemDto>`: Updated wishlist item
- Throws `ArgumentException` if item not found
- Throws `UnauthorizedAccessException` if user doesn't own item
- Throws `InvalidOperationException` if ticket not active

#### Edge Cases
- ✅ Updating non-existent item
- ✅ Updating item owned by different user
- ✅ Updating item with inactive ticket type
- ✅ Updating quantity to exceed available stock
- ✅ Updating with zero/negative quantity
- ✅ Database save failure

#### Dependencies to Mock
- `_context.Wishlists` (DbSet<Wishlist>)
- `_context.TicketTypes` (DbSet<TicketType>)
- `_wishlistMapper.MapToDto()`
- `DateTime.UtcNow` (for time validation)

---

### 4. **DeleteItemAsync(int userId, int itemId)**

#### Main Functionality
- Removes a single item from user's wishlist
- Validates user ownership before deletion

#### Input Parameters
- `userId` (int): ID of the user
- `itemId` (int): ID of the wishlist item to delete

#### Expected Return Values
- `Task`: Completes successfully when item deleted
- Throws `ArgumentException` if item not found
- Throws `UnauthorizedAccessException` if user doesn't own item

#### Edge Cases
- ✅ Deleting non-existent item
- ✅ Deleting item owned by different user
- ✅ Database save failure
- ✅ Invalid itemId (negative/zero)

#### Dependencies to Mock
- `_context.Wishlists` (DbSet<Wishlist>)

---

### 5. **DeleteItemsAsync(int userId, BulkDeleteWishlistRequest request)**

#### Main Functionality
- Removes multiple items from user's wishlist
- Validates all items exist before deletion

#### Input Parameters
- `userId` (int): ID of the user
- `request` (BulkDeleteWishlistRequest): Contains list of item IDs to delete

#### Expected Return Values
- `Task`: Completes successfully when all items deleted
- Throws `ArgumentException` if some items not found

#### Edge Cases
- ✅ Deleting empty list of items
- ✅ Deleting some non-existent items
- ✅ Deleting items owned by different user
- ✅ Database save failure
- ✅ Mixed valid/invalid item IDs

#### Dependencies to Mock
- `_context.Wishlists` (DbSet<Wishlist>)

---

### 6. **CheckoutAsync(int userId, WishlistCheckoutRequest request)**

#### Main Functionality
- Converts wishlist items to actual orders
- Validates selected items exist and belong to user
- Creates orders through OrderService

#### Input Parameters
- `userId` (int): ID of the user
- `request` (WishlistCheckoutRequest): Contains list of item IDs to checkout

#### Expected Return Values
- `Task<WishlistCheckoutResponse>`: Contains order draft ID and next step
- Throws `ArgumentException` if no items selected or no valid items found
- Throws `InvalidOperationException` if no orders created

#### Edge Cases
- ✅ Empty selection list
- ✅ Non-existent item IDs
- ✅ Items owned by different user
- ✅ Order creation failure
- ✅ Partial order creation success
- ✅ All order creations fail

#### Dependencies to Mock
- `_context.Wishlists` (DbSet<Wishlist>)
- `_orderService.CreateOrderAsync()`

---

### 7. **CreateOrdersFromWishlistAsync(int userId, WishlistCheckoutRequest request)**

#### Main Functionality
- Helper method to create orders from wishlist items
- Handles individual order creation failures gracefully

#### Input Parameters
- `userId` (int): ID of the user
- `request` (WishlistCheckoutRequest): Contains list of item IDs

#### Expected Return Values
- `Task<List<CreateOrderResponseDTO>>`: List of successfully created orders
- Throws `ArgumentException` if no items selected or no valid items found

#### Edge Cases
- ✅ Empty selection list
- ✅ Non-existent item IDs
- ✅ Items owned by different user
- ✅ Individual order creation failures
- ✅ All order creations fail
- ✅ Partial success scenarios

#### Dependencies to Mock
- `_context.Wishlists` (DbSet<Wishlist>)
- `_orderService.CreateOrderAsync()`

---

## 🧪 Test Strategy Recommendations

### Test Categories
1. **Happy Path Tests**: Normal successful operations
2. **Validation Tests**: Input validation and business rules
3. **Authorization Tests**: User ownership and access control
4. **Edge Case Tests**: Boundary conditions and error scenarios
5. **Integration Tests**: Database and external service interactions

### Mocking Strategy
- **Database Context**: Use InMemory database or mock DbSet
- **External Services**: Mock IOrderService and IWishlistMapper
- **Time Dependencies**: Mock DateTime.UtcNow for consistent testing

### Test Data Setup
- Create test users with different scenarios
- Create ticket types with various statuses and availability
- Create wishlist items with different quantities and ownership

---

## 🎯 Priority Testing Order

1. **High Priority**: `AddItemAsync`, `UpdateQuantityAsync`, `DeleteItemAsync`
2. **Medium Priority**: `GetWishlistAsync`, `DeleteItemsAsync`
3. **Low Priority**: `CheckoutAsync`, `CreateOrdersFromWishlistAsync`

**Reasoning**: Core CRUD operations are most critical for basic functionality, while checkout operations depend on order service integration.
