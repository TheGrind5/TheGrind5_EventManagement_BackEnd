using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsByEventAsync(int eventId);
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<ProductDto> CreateProductAsync(CreateProductRequest request);
        Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductSelectionDto>> GetProductsForSelectionAsync(int eventId);
    }

    public class ProductService : IProductService
    {
        private readonly EventDBContext _context;

        public ProductService(EventDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByEventAsync(int eventId)
        {
            var products = await _context.Products
                .Where(p => p.EventId == eventId && p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.ProductName))
                {
                    throw new ArgumentException("Tên sản phẩm không được để trống");
                }

                if (request.Price < 0)
                {
                    throw new ArgumentException("Giá sản phẩm không được âm");
                }

                if (request.EventId <= 0)
                {
                    throw new ArgumentException("ID sự kiện không hợp lệ");
                }

                // Check if event exists
                var eventExists = await _context.Events.AnyAsync(e => e.EventId == request.EventId);
                if (!eventExists)
                {
                    throw new ArgumentException("Sự kiện không tồn tại");
                }

                var product = new Product
                {
                    ProductName = request.ProductName.Trim(),
                    Price = request.Price,
                    ProductImage = request.ProductImage?.Trim(),
                    Description = request.Description?.Trim(),
                    EventId = request.EventId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateProductAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductRequest request)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return null;

            if (request.ProductName != null)
                product.ProductName = request.ProductName;

            if (request.Price.HasValue)
                product.Price = request.Price.Value;

            if (request.ProductImage != null)
                product.ProductImage = request.ProductImage;

            if (request.Description != null)
                product.Description = request.Description;

            if (request.IsActive.HasValue)
                product.IsActive = request.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ProductSelectionDto>> GetProductsForSelectionAsync(int eventId)
        {
            var products = await _context.Products
                .Where(p => p.EventId == eventId && p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return products.Select(p => new ProductSelectionDto(
                p.ProductId,
                p.ProductName,
                p.Price,
                p.ProductImage,
                p.Description,
                false, // IsSelected
                1      // Quantity
            ));
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto(
                product.ProductId,
                product.ProductName,
                product.Price,
                product.ProductImage,
                product.Description,
                product.EventId,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt
            );
        }
    }
}
