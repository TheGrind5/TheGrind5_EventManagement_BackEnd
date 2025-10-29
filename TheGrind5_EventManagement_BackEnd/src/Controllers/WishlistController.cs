using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var wishlist = await _wishlistService.GetWishlistAsync(userId.Value);
            return Ok(wishlist);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách wishlist", error = ex.Message });
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddWishlistItemRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var item = await _wishlistService.AddItemAsync(userId.Value, request);
            return Ok(item);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi thêm item vào wishlist", error = ex.Message });
        }
    }

    [HttpPatch("items/{itemId}")]
    public async Task<IActionResult> UpdateItem(int itemId, [FromBody] UpdateWishlistItemRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var item = await _wishlistService.UpdateQuantityAsync(userId.Value, itemId, request);
            return Ok(item);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật item", error = ex.Message });
        }
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> DeleteItem(int itemId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            await _wishlistService.DeleteItemAsync(userId.Value, itemId);
            return Ok(new { message = "Xóa item thành công" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi xóa item", error = ex.Message });
        }
    }

    [HttpPost("bulk-delete")]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteWishlistRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            await _wishlistService.DeleteItemsAsync(userId.Value, request);
            return Ok(new { message = "Xóa các item thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi xóa các item", error = ex.Message });
        }
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] WishlistCheckoutRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _wishlistService.CheckoutAsync(userId.Value, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Có lỗi xảy ra khi checkout", error = ex.Message });
        }
    }

    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : null;
    }
}
