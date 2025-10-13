using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;

namespace TheGrind5_EventManagement.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistItemRepository _wishlistItemRepository;
    private readonly IWishlistMapper _wishlistMapper;
    private readonly EventDBContext _context;

    public WishlistService(
        IWishlistItemRepository wishlistItemRepository,
        IWishlistMapper wishlistMapper,
        EventDBContext context)
    {
        _wishlistItemRepository = wishlistItemRepository;
        _wishlistMapper = wishlistMapper;
        _context = context;
    }

    public async Task<WishlistResponse> GetWishlistAsync(int userId)
    {
        var wishlistItems = await _wishlistItemRepository.GetByUserIdAsync(userId);
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
        var existingItem = await _wishlistItemRepository.GetByUserIdAndTicketTypeIdAsync(userId, request.TicketTypeId);

        if (existingItem != null)
        {
            // Upsert: increase quantity
            var newQuantity = Math.Min(existingItem.Quantity + request.Quantity, ticketType.Quantity);
            existingItem.Quantity = newQuantity;
            
            var updatedItem = await _wishlistItemRepository.UpdateAsync(existingItem);
            return _wishlistMapper.MapToDto(updatedItem);
        }
        else
        {
            // Create new item
            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                TicketTypeId = request.TicketTypeId,
                Quantity = Math.Min(request.Quantity, ticketType.Quantity),
                CreatedAt = DateTime.UtcNow
            };

            var createdItem = await _wishlistItemRepository.CreateAsync(wishlistItem);
            return _wishlistMapper.MapToDto(createdItem);
        }
    }

    public async Task<WishlistItemDto> UpdateQuantityAsync(int userId, int itemId, UpdateWishlistItemRequest request)
    {
        var wishlistItem = await _wishlistItemRepository.GetByIdAsync(itemId);
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

        var updatedItem = await _wishlistItemRepository.UpdateAsync(wishlistItem);
        return _wishlistMapper.MapToDto(updatedItem);
    }

    public async Task DeleteItemAsync(int userId, int itemId)
    {
        var wishlistItem = await _wishlistItemRepository.GetByIdAsync(itemId);
        if (wishlistItem == null)
            throw new ArgumentException("Wishlist item not found", nameof(itemId));

        if (wishlistItem.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own wishlist items");

        await _wishlistItemRepository.DeleteAsync(itemId);
    }

    public async Task DeleteItemsAsync(int userId, BulkDeleteWishlistRequest request)
    {
        await _wishlistItemRepository.DeleteAllByUserIdAndIdsAsync(userId, request.Ids);
    }

    public async Task<WishlistCheckoutResponse> CheckoutAsync(int userId, WishlistCheckoutRequest request)
    {
        if (!request.Ids.Any())
            throw new ArgumentException("No items selected for checkout");

        var wishlistItems = await _context.WishlistItems
            .Include(wi => wi.TicketType)
                .ThenInclude(tt => tt.Event)
            .Where(wi => wi.UserId == userId && request.Ids.Contains(wi.Id))
            .ToListAsync();

        if (!wishlistItems.Any())
            throw new ArgumentException("No valid items found for checkout");

        // Validate all items are still available
        foreach (var item in wishlistItems)
        {
            var ticketType = item.TicketType;
            if (ticketType == null || ticketType.Status != "Active")
                throw new InvalidOperationException($"Ticket type {ticketType?.TypeName} is no longer available");

            if (DateTime.UtcNow < ticketType.SaleStart || DateTime.UtcNow > ticketType.SaleEnd)
                throw new InvalidOperationException($"Ticket type {ticketType.TypeName} is not available for sale at this time");

            if (item.Quantity > ticketType.Quantity)
                throw new InvalidOperationException($"Insufficient quantity for ticket type {ticketType.TypeName}");
        }

        // For now, return a mock order draft ID
        // In a real implementation, this would create an actual order draft
        var orderDraftId = Guid.NewGuid().ToString();
        
        return new WishlistCheckoutResponse
        {
            OrderDraftId = orderDraftId,
            Next = $"/checkout/{orderDraftId}"
        };
    }
}
