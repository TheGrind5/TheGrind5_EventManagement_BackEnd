using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories;

public class WishlistItemRepository : IWishlistItemRepository
{
    private readonly EventDBContext _context;

    public WishlistItemRepository(EventDBContext context)
    {
        _context = context;
    }

    public async Task<List<WishlistItem>> GetByUserIdAsync(int userId)
    {
        return await _context.WishlistItems
            .Include(wi => wi.TicketType)
                .ThenInclude(tt => tt.Event)
            .Where(wi => wi.UserId == userId)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync();
    }

    public async Task<WishlistItem?> GetByUserIdAndTicketTypeIdAsync(int userId, int ticketTypeId)
    {
        return await _context.WishlistItems
            .Include(wi => wi.TicketType)
                .ThenInclude(tt => tt.Event)
            .FirstOrDefaultAsync(wi => wi.UserId == userId && wi.TicketTypeId == ticketTypeId);
    }

    public async Task<WishlistItem?> GetByIdAsync(int id)
    {
        return await _context.WishlistItems
            .Include(wi => wi.TicketType)
                .ThenInclude(tt => tt.Event)
            .FirstOrDefaultAsync(wi => wi.Id == id);
    }

    public async Task<WishlistItem> CreateAsync(WishlistItem wishlistItem)
    {
        _context.WishlistItems.Add(wishlistItem);
        await _context.SaveChangesAsync();
        return wishlistItem;
    }

    public async Task<WishlistItem> UpdateAsync(WishlistItem wishlistItem)
    {
        _context.WishlistItems.Update(wishlistItem);
        await _context.SaveChangesAsync();
        return wishlistItem;
    }

    public async Task DeleteAsync(int id)
    {
        var wishlistItem = await _context.WishlistItems.FindAsync(id);
        if (wishlistItem != null)
        {
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllByUserIdAndIdsAsync(int userId, List<int> ids)
    {
        var wishlistItems = await _context.WishlistItems
            .Where(wi => wi.UserId == userId && ids.Contains(wi.Id))
            .ToListAsync();

        if (wishlistItems.Any())
        {
            _context.WishlistItems.RemoveRange(wishlistItems);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.WishlistItems.AnyAsync(wi => wi.Id == id);
    }
}
