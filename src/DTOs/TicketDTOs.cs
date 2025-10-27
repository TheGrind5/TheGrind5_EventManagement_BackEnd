using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheGrind5_EventManagement.DTOs
{
    public class TicketDTO
    {
        public int TicketId { get; set; }
        public required string SerialNumber { get; set; }
        public required string Status { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public required TicketTypeInfoDTO TicketType { get; set; }
        public required EventInfoDTO Event { get; set; }
        public required OrderInfoDTO Order { get; set; }
    }

    public class TicketTypeInfoDTO
    {
        public int TicketTypeId { get; set; }
        public required string TypeName { get; set; }
        public decimal Price { get; set; }
    }

    public class TicketTypeDTO
    {
        public int TicketTypeId { get; set; }
        public int EventId { get; set; }
        public required string TypeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? MinOrder { get; set; }
        public int? MaxOrder { get; set; }
        public DateTime SaleStart { get; set; }
        public DateTime SaleEnd { get; set; }
        public required string Status { get; set; }
        public int AvailableQuantity { get; set; }
    }

    public class EventInfoDTO
    {
        public int EventId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }
    }

    public class OrderInfoDTO
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CheckInTicketRequestDTO
    {
        [Required]
        public int TicketId { get; set; }
    }

    public class TicketListResponseDTO
    {
        public required IEnumerable<TicketDTO> Tickets { get; set; }
        public int TotalCount { get; set; }
        public int AvailableCount { get; set; }
        public int UsedCount { get; set; }
        public int RefundedCount { get; set; }
    }
}
