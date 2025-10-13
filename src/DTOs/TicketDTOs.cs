using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs
{
    public class TicketDTO
    {
        public int TicketId { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public TicketTypeInfoDTO TicketType { get; set; }
        public EventInfoDTO Event { get; set; }
        public OrderInfoDTO Order { get; set; }
    }

    public class TicketTypeInfoDTO
    {
        public int TicketTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
    }

    public class TicketTypeDTO
    {
        public int TicketTypeId { get; set; }
        public int EventId { get; set; }
        public string TypeName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? MinOrder { get; set; }
        public int? MaxOrder { get; set; }
        public DateTime SaleStart { get; set; }
        public DateTime SaleEnd { get; set; }
        public string Status { get; set; }
        public int AvailableQuantity { get; set; }
    }

    public class EventInfoDTO
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
    }

    public class OrderInfoDTO
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CheckInTicketRequestDTO
    {
        [Required]
        public int TicketId { get; set; }
    }

    public class TicketListResponseDTO
    {
        public IEnumerable<TicketDTO> Tickets { get; set; }
        public int TotalCount { get; set; }
        public int AvailableCount { get; set; }
        public int UsedCount { get; set; }
        public int RefundedCount { get; set; }
    }
}
