using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Business;

namespace TheGrind5_EventManagement.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistMapper _wishlistMapper;
    private readonly EventDBContext _context;
    private readonly IOrderService _orderService;

    public WishlistService(
        IWishlistMapper wishlistMapper,
        EventDBContext context,
        IOrderService orderService)
    {
        _wishlistMapper = wishlistMapper;
        _context = context;
        _orderService = orderService;
    }

    public async Task<WishlistResponse> GetWishlistAsync(int userId)
    {
        var wishlistItems = await _context.Wishlists
            .Include(w => w.TicketType)
                .ThenInclude(tt => tt.Event)
            .Where(w => w.UserId == userId)
            .ToListAsync();
        
        return _wishlistMapper.MapToWishlistResponse(wishlistItems);
    }

    public async Task<WishlistItemDto> AddItemAsync(int userId, AddWishlistItemRequest request)
    {
        // Validate ticket type exists and is available
        var ticketType = await _context.TicketTypes
            .Include(tt => tt.Event)
            .FirstOrDefaultAsync(tt => tt.TicketTypeId == request.TicketTypeId);

        if (ticketType == null)
            throw new ArgumentException("Ticket type not found", nameof(request.TicketTypeId));

        if (ticketType.Status != "Active")
            throw new InvalidOperationException("Ticket type is not active");

        if (DateTime.UtcNow < ticketType.SaleStart || DateTime.UtcNow > ticketType.SaleEnd)
            throw new InvalidOperationException("Ticket is not available for sale at this time");

        // Check if item already exists in wishlist
        var existingItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.TicketTypeId == request.TicketTypeId);

        if (existingItem != null)
        {
            // Update quantity if item exists
            var newQuantity = Math.Min(existingItem.Quantity + request.Quantity, ticketType.Quantity);
            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return _wishlistMapper.MapToDto(existingItem);
        }
        else
        {
            // Create new item
            var wishlistItem = new Wishlist
            {
                UserId = userId,
                TicketTypeId = request.TicketTypeId,
                Quantity = Math.Min(request.Quantity, ticketType.Quantity),
                AddedAt = DateTime.UtcNow
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return _wishlistMapper.MapToDto(wishlistItem);
        }
    }

    public async Task<WishlistItemDto> UpdateQuantityAsync(int userId, int itemId, UpdateWishlistItemRequest request)
    {
        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == itemId);
        
        if (wishlistItem == null)
            throw new ArgumentException("Wishlist item not found", nameof(itemId));

        if (wishlistItem.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own wishlist items");

        // Validate ticket type is still available
        var ticketType = await _context.TicketTypes
            .Include(tt => tt.Event)
            .FirstOrDefaultAsync(tt => tt.TicketTypeId == wishlistItem.TicketTypeId);

        if (ticketType == null)
            throw new ArgumentException("Ticket type not found");

        if (ticketType.Status != "Active")
            throw new InvalidOperationException("Ticket type is not active");

        // Clamp quantity to available amount
        var newQuantity = Math.Min(request.Quantity, ticketType.Quantity);
        wishlistItem.Quantity = newQuantity;
        wishlistItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _wishlistMapper.MapToDto(wishlistItem);
    }

    public async Task DeleteItemAsync(int userId, int itemId)
    {
        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == itemId);
        
        if (wishlistItem == null)
            throw new ArgumentException("Wishlist item not found", nameof(itemId));

        if (wishlistItem.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own wishlist items");

        _context.Wishlists.Remove(wishlistItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemsAsync(int userId, BulkDeleteWishlistRequest request)
    {
        var itemsToDelete = await _context.Wishlists
            .Where(w => w.UserId == userId && request.Ids.Contains(w.Id))
            .ToListAsync();
        
        if (itemsToDelete.Count != request.Ids.Count)
            throw new ArgumentException("Some wishlist items not found");

        _context.Wishlists.RemoveRange(itemsToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<WishlistCheckoutResponse> CheckoutAsync(int userId, WishlistCheckoutRequest request)
    {
        if (!request.Ids.Any())
            throw new ArgumentException("No items selected for checkout");

        // Get all wishlist items for user first
        var userWishlistItems = await _context.Wishlists
            .Include(w => w.TicketType)
                .ThenInclude(tt => tt.Event)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        // Filter by requested IDs in memory
        var wishlistItems = userWishlistItems
            .Where(w => request.Ids.Contains(w.Id))
            .ToList();

        if (!wishlistItems.Any())
            throw new ArgumentException("No valid items found for checkout");

        // Tạo orders thật từ wishlist items
        var createdOrders = await CreateOrdersFromWishlistAsync(userId, request);
        
        if (!createdOrders.Any())
            throw new InvalidOperationException("No orders were created from wishlist items");

        // Trả về order đầu tiên (hoặc có thể trả về tất cả)
        var firstOrder = createdOrders.First();
        
        return new WishlistCheckoutResponse
        {
            OrderDraftId = firstOrder.OrderId.ToString(),
            Next = $"/payment/{firstOrder.OrderId}"
        };
    }

    /// <summary>
    /// Tạo orders thật từ wishlist items
    /// </summary>
    public async Task<List<CreateOrderResponseDTO>> CreateOrdersFromWishlistAsync(int userId, WishlistCheckoutRequest request)
    {
        if (!request.Ids.Any())
            throw new ArgumentException("No items selected for checkout");

        // Get all wishlist items for user first
        var userWishlistItems = await _context.Wishlists
            .Include(w => w.TicketType)
                .ThenInclude(tt => tt.Event)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        // Filter by requested IDs in memory
        var wishlistItems = userWishlistItems
            .Where(w => request.Ids.Contains(w.Id))
            .ToList();

        if (!wishlistItems.Any())
            throw new ArgumentException("No valid items found for checkout");

        var createdOrders = new List<CreateOrderResponseDTO>();

        // Tạo order cho từng wishlist item
        foreach (var wishlistItem in wishlistItems)
        {
            try
            {
                // Tạo CreateOrderRequestDTO từ wishlist item
                var orderRequest = new CreateOrderRequestDTO
                {
                    EventId = wishlistItem.TicketType.EventId,
                    TicketTypeId = wishlistItem.TicketTypeId,
                    Quantity = wishlistItem.Quantity,
                    SeatNo = null
                };

                // Gọi OrderService để tạo order thật
                var orderResponse = await _orderService.CreateOrderAsync(orderRequest, userId);
                createdOrders.Add(orderResponse);
            }
            catch (Exception ex)
            {
                // Log error nhưng tiếp tục với items khác
                Console.WriteLine($"Error creating order for wishlist item {wishlistItem.Id}: {ex.Message}");
            }
        }

        return createdOrders;
    }
}
