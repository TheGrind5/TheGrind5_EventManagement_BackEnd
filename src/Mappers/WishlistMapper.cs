using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Mappers;

public class WishlistMapper : IWishlistMapper
{
    public WishlistItemDto MapToDto(Wishlist wishlist)
    {
        return new WishlistItemDto
        {
            Id = wishlist.Id,
            TicketTypeId = wishlist.TicketTypeId,
            EventId = wishlist.TicketType?.EventId ?? 0,
            Title = wishlist.TicketType?.TypeName ?? string.Empty,
            EventName = wishlist.TicketType?.Event?.Title ?? string.Empty,
            Price = wishlist.TicketType?.Price ?? 0,
            ThumbnailUrl = null, // Event model doesn't have ImageUrl property
            Quantity = wishlist.Quantity,
            MaxQuantity = wishlist.TicketType?.Quantity ?? 0,
            CreatedAt = wishlist.AddedAt
        };
    }

    public WishlistResponse MapToWishlistResponse(List<Wishlist> wishlistItems)
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
