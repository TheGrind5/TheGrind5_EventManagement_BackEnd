using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services;

public interface IWishlistService
{
    Task<WishlistResponse> GetWishlistAsync(int userId);
    Task<WishlistItemDto> AddItemAsync(int userId, AddWishlistItemRequest request);
    Task<WishlistItemDto> UpdateQuantityAsync(int userId, int itemId, UpdateWishlistItemRequest request);
    Task DeleteItemAsync(int userId, int itemId);
    Task DeleteItemsAsync(int userId, BulkDeleteWishlistRequest request);
    Task<WishlistCheckoutResponse> CheckoutAsync(int userId, WishlistCheckoutRequest request);
}
