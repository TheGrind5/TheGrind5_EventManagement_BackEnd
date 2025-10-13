using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs;

// Request DTOs
public class AddWishlistItemRequest
{
    [Required]
    public int TicketTypeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; } = 1;
}

public class UpdateWishlistItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}

public class BulkDeleteWishlistRequest
{
    [Required]
    public List<int> Ids { get; set; } = new();
}

public class WishlistCheckoutRequest
{
    [Required]
    public List<int> Ids { get; set; } = new();
}

// Response DTOs
public class WishlistItemDto
{
    public int Id { get; set; }
    public int TicketTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int Quantity { get; set; }
    public int MaxQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WishlistTotalsDto
{
    public int Count { get; set; }
    public decimal Sum { get; set; }
}

public class WishlistResponse
{
    public List<WishlistItemDto> Items { get; set; } = new();
    public WishlistTotalsDto Totals { get; set; } = new();
}

public class WishlistCheckoutResponse
{
    public string OrderDraftId { get; set; } = string.Empty;
    public string Next { get; set; } = string.Empty;
}
