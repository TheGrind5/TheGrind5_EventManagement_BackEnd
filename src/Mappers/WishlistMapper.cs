using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Mappers;

public class WishlistMapper : IWishlistMapper
{
    public WishlistItemDto MapToDto(WishlistItem wishlistItem)
    {
        return new WishlistItemDto
        {
            Id = wishlistItem.Id,
            TicketTypeId = wishlistItem.TicketTypeId,
            Title = wishlistItem.TicketType?.TypeName ?? string.Empty,
            EventName = wishlistItem.TicketType?.Event?.Title ?? string.Empty,
            Price = wishlistItem.TicketType?.Price ?? 0,
            ThumbnailUrl = null, // Event model doesn't have ImageUrl property
            Quantity = wishlistItem.Quantity,
            MaxQuantity = wishlistItem.TicketType?.Quantity ?? 0,
            CreatedAt = wishlistItem.CreatedAt
        };
    }

    public WishlistResponse MapToWishlistResponse(List<WishlistItem> wishlistItems)
    {
        var items = wishlistItems.Select(MapToDto).ToList();
        
        return new WishlistResponse
        {
            Items = items,
            Totals = new WishlistTotalsDto
            {
                Count = items.Count,
                Sum = items.Sum(item => item.Price * item.Quantity)
            }
        };
    }
}
