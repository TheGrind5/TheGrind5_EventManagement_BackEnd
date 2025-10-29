using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs
{
    public record ProductDto(
        int ProductId,
        string ProductName,
        decimal Price,
        string? ProductImage,
        string? Description,
        int EventId,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );

    public record CreateProductRequest(
        [Required]
        [StringLength(200)]
        string ProductName,
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
        decimal Price,
        
        [StringLength(500)]
        string? ProductImage,
        
        [StringLength(1000)]
        string? Description,
        
        [Required]
        int EventId
    );

    public record UpdateProductRequest(
        [StringLength(200)]
        string? ProductName,
        
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
        decimal? Price,
        
        [StringLength(500)]
        string? ProductImage,
        
        [StringLength(1000)]
        string? Description,
        
        bool? IsActive
    );

    public record ProductSelectionDto(
        int ProductId,
        string ProductName,
        decimal Price,
        string? ProductImage,
        string? Description,
        bool IsSelected = false,
        int Quantity = 1
    );
}
