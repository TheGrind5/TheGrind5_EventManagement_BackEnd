using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetProductsByEvent(int eventId)
        {
            try
            {
                var products = await _productService.GetProductsByEventAsync(eventId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sản phẩm", error = ex.Message });
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin sản phẩm", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    return BadRequest(new { message = "Dữ liệu sản phẩm không hợp lệ" });
                }

                if (string.IsNullOrWhiteSpace(request.ProductName))
                {
                    return BadRequest(new { message = "Tên sản phẩm không được để trống" });
                }

                if (request.Price < 0)
                {
                    return BadRequest(new { message = "Giá sản phẩm không được âm" });
                }

                if (request.EventId <= 0)
                {
                    return BadRequest(new { message = "ID sự kiện không hợp lệ" });
                }

                var product = await _productService.CreateProductAsync(request);
                return CreatedAtAction(nameof(GetProduct), new { productId = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo sản phẩm", error = ex.Message });
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] UpdateProductRequest request)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(productId, request);
                if (product == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật sản phẩm", error = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(productId);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy sản phẩm" });

                return Ok(new { message = "Xóa sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi xóa sản phẩm", error = ex.Message });
            }
        }

        [HttpGet("event/{eventId}/selection")]
        public async Task<IActionResult> GetProductsForSelection(int eventId)
        {
            try
            {
                var products = await _productService.GetProductsForSelectionAsync(eventId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách sản phẩm để chọn", error = ex.Message });
            }
        }
    }
}
