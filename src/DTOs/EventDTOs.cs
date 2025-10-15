namespace TheGrind5_EventManagement.DTOs
{
    public record CreateEventRequest(
        string Title,
        string Description,
        DateTime StartTime,
        DateTime EndTime,
        string Location,
        string Category
    );

    public record UpdateEventRequest(
        string Title,
        string Description,
        DateTime StartTime,
        DateTime EndTime,
        string Location,
        string Category
    );

    // DTOs mới cho Event Creation theo mẫu Ticket Box
    public record CreateEventStep1Request(
        string Title,
        string Description,
        string EventMode, // Online/Offline
        string VenueName,
        string Province,
        string District,
        string Ward,
        string StreetAddress,
        string EventType,
        string Category,
        string EventImage,
        string BackgroundImage,
        string EventIntroduction,
        string EventDetails,
        string SpecialGuests,
        string SpecialExperience,
        string TermsAndConditions,
        string ChildrenTerms,
        string VATTerms,
        string OrganizerLogo,
        string OrganizerName,
        string OrganizerInfo
    );

    public record CreateEventStep2Request(
        DateTime StartTime,
        DateTime EndTime,
        List<CreateTicketTypeRequest> TicketTypes
    );

    public record CreateEventStep3Request(
        string EventSettings,
        bool AllowRefund,
        int RefundDaysBefore,
        bool RequireApproval
    );

    public record CreateEventStep4Request(
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
        string Status
    );

    public record EventCreationResponse(
        int EventId,
        string Message,
        bool Success
    );

    public record EventDetailResponse(
        int EventId,
        string Title,
        string Description,
        DateTime StartTime,
        DateTime EndTime,
        string Location,
        string VenueName,
        string Province,
        string District,
        string Ward,
        string StreetAddress,
        string EventMode,
        string EventType,
        string Category,
        string EventImage,
        string BackgroundImage,
        string EventIntroduction,
        string EventDetails,
        string SpecialGuests,
        string SpecialExperience,
        string TermsAndConditions,
        string ChildrenTerms,
        string VATTerms,
        string OrganizerLogo,
        string OrganizerName,
        string OrganizerInfo,
        string Status,
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
