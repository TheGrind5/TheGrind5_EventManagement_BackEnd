using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Mappers;

public interface IWishlistMapper
{
    WishlistItemDto MapToDto(WishlistItem wishlistItem);
    WishlistResponse MapToWishlistResponse(List<WishlistItem> wishlistItems);
}
