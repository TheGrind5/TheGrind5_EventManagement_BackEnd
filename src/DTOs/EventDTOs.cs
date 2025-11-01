using TheGrind5_EventManagement.Models;
using System.Text.Json.Serialization;

namespace TheGrind5_EventManagement.DTOs
{
    public record CreateEventRequest(
        string Title,
        string? Description,
        DateTime StartTime,
        DateTime EndTime,
        string? Location,
        string? Category
    );

    public record UpdateEventRequest(
        string Title,
        string? Description,
        DateTime StartTime,
        DateTime EndTime,
        string? Location,
        string? Category
    );

    // DTOs mới cho Event Creation theo mẫu Ticket Box
    public record CreateEventStep1Request(
        string Title,
        string? Description,
        string? EventMode, // Online/Offline
        string? Location,  // Thêm Location field
        string? VenueName,
        string? Province,
        string? District,
        string? Ward,
        string? StreetAddress,
        string? EventType,
        string? Category,
        string? Campus, // Thêm Campus field (tên campus như "Hà Nội", "Đà Nẵng")
        string? EventImage,
        string? BackgroundImage,
        string? EventIntroduction,
        string? EventDetails,
        string? SpecialGuests,
        string? SpecialExperience,
        string? TermsAndConditions,
        string? ChildrenTerms,
        string? VATTerms,
        string? OrganizerLogo,
        string? OrganizerName,
        string? OrganizerInfo
    );

    public record CreateEventStep2Request(
        DateTime StartTime,
        DateTime EndTime,
        List<CreateTicketTypeRequest> TicketTypes
    );

    public record CreateEventStep3Request(
        VenueLayoutData? VenueLayout
    );

    public record CreateEventStep4Request(
        string PaymentMethod,
        string BankAccount,
        string TaxInfo
    );

    // DTO để tạo event hoàn chỉnh với tất cả 5 bước cùng lúc
    public record CreateCompleteEventRequest(
        // Step 1
        string Title,
        string? Description,
        string? EventMode,
        string? Location,
        string? VenueName,
        string? Province,
        string? District,
        string? Ward,
        string? StreetAddress,
        string? EventType,
        string? Category,
        string? Campus, // Thêm Campus field (tên campus như "Hà Nội", "Đà Nẵng")
        string? EventImage,
        string? BackgroundImage,
        string? EventIntroduction,
        string? EventDetails,
        string? SpecialGuests,
        string? SpecialExperience,
        string? TermsAndConditions,
        string? ChildrenTerms,
        string? VATTerms,
        string? OrganizerLogo,
        string? OrganizerName,
        string? OrganizerInfo,
        // Step 2
        DateTime StartTime,
        DateTime EndTime,
        List<CreateTicketTypeRequest> TicketTypes,
        // Step 3
        VenueLayoutData? VenueLayout,
        // Step 4 - Settings (optional)
        string? EventSettings,
        // Step 5
        string PaymentMethod,
        string BankAccount,
        string TaxInfo
    );

    public record CreateTicketTypeRequest(
        string TypeName,
        decimal Price,
        int Quantity,
        int MinOrder,
        int MaxOrder,
        DateTime SaleStart,
        DateTime SaleEnd,
        string? Status = null
    );

    public record EventCreationResponse(
        int EventId,
        string Message,
        bool Success
    );

    public record EventDetailResponse(
        int EventId,
        string Title,
        string? Description,
        DateTime StartTime,
        DateTime EndTime,
        string? Location,
        string? VenueName,
        string? Province,
        string? District,
        string? Ward,
        string? StreetAddress,
        string? EventMode,
        string? EventType,
        string? Category,
        string? EventImage,
        string? BackgroundImage,
        string? EventIntroduction,
        string? EventDetails,
        string? SpecialGuests,
        string? SpecialExperience,
        string? TermsAndConditions,
        string? ChildrenTerms,
        string? VATTerms,
        string? OrganizerLogo,
        string? OrganizerName,
        string? OrganizerInfo,
        string? Status,
        DateTime CreatedAt,
        int HostId,
        string HostName,
        List<TicketTypeResponse> TicketTypes
    );

    public record TicketTypeResponse(
        int TicketTypeId,
        string TypeName,
        decimal Price,
        int Quantity,
        int MinOrder,
        int MaxOrder,
        DateTime SaleStart,
        DateTime SaleEnd,
        string Status
    );
}
