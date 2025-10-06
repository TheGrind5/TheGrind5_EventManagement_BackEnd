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
}
