using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories;

public interface IWishlistItemRepository
{
    Task<List<WishlistItem>> GetByUserIdAsync(int userId);
    Task<WishlistItem?> GetByUserIdAndTicketTypeIdAsync(int userId, int ticketTypeId);
    Task<WishlistItem?> GetByIdAsync(int id);
    Task<WishlistItem> CreateAsync(WishlistItem wishlistItem);
    Task<WishlistItem> UpdateAsync(WishlistItem wishlistItem);
    Task DeleteAsync(int id);
    Task DeleteAllByUserIdAndIdsAsync(int userId, List<int> ids);
    Task<bool> ExistsAsync(int id);
}
