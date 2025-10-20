using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs
{
    // Request DTO cho tạo order mới
    public record CreateOrderRequestDTO
    {
        [Required]
        public int EventId { get; init; }

        [Required]
        public int TicketTypeId { get; init; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; init; }

        public string? SeatNo { get; init; }
    }

    // Response DTO sau khi tạo order thành công
    public record CreateOrderResponseDTO
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public int EventId { get; init; }
        public string EventTitle { get; init; } = string.Empty;
        public string TicketTypeName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public string Message { get; init; } = string.Empty;
    }

    // DTO để hiển thị thông tin order
    public record OrderDTO
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Status { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<OrderItemDTO> OrderItems { get; init; } = new();
    }

    // DTO cho order item details
    public record OrderItemDTO
    {
        public int OrderItemId { get; init; }
        public int TicketTypeId { get; init; }
        public string TicketTypeName { get; init; } = string.Empty;
        public string EventTitle { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
        public string? SeatNo { get; init; }
        public string Status { get; init; } = string.Empty;
    }

    // DTO cho order detail (có thêm payments)
    public record OrderDetailDto
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Status { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public List<OrderItemDTO> OrderItems { get; init; } = new();
        public List<PaymentDTO> Payments { get; init; } = new();
    }

    // DTO cho payment details
    public record PaymentDTO
    {
        public int PaymentId { get; init; }
        public int OrderId { get; init; }
        public decimal Amount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public string TransactionId { get; init; } = string.Empty;
    }
}