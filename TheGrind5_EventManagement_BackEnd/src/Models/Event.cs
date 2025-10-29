using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TheGrind5_EventManagement.Models;

public partial class Event
{
    public int EventId { get; set; }

    public int HostId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public string? Location { get; set; }

    public string? EventType { get; set; }

    [RegularExpression("^(Online|Offline)$", ErrorMessage = "EventMode must be Online or Offline")]
    public string? EventMode { get; set; } = "Offline";

    public string? Category { get; set; }

    [RegularExpression("^(Draft|Open|Closed|Cancelled)$", ErrorMessage = "Status must be Draft, Open, Closed, or Cancelled")]
    public string? Status { get; set; }

    // JSON fields for additional information
    public string? EventDetails { get; set; } // JSON: venue, images, introduction, special guests, etc.
    
    public string? TermsAndConditions { get; set; } // JSON: terms, children terms, VAT terms
    
    public string? OrganizerInfo { get; set; } // JSON: logo, name, info

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Host { get; set; }

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    // Helper methods for JSON properties
    public EventDetailsData GetEventDetails()
    {
        if (string.IsNullOrEmpty(EventDetails))
            return new EventDetailsData();
        
        try
        {
            return JsonSerializer.Deserialize<EventDetailsData>(EventDetails) ?? new EventDetailsData();
        }
        catch
        {
            return new EventDetailsData();
        }
    }

    public void SetEventDetails(EventDetailsData data)
    {
        EventDetails = JsonSerializer.Serialize(data);
    }

    public TermsAndConditionsData GetTermsAndConditions()
    {
        if (string.IsNullOrEmpty(TermsAndConditions))
            return new TermsAndConditionsData();
        
        try
        {
            return JsonSerializer.Deserialize<TermsAndConditionsData>(TermsAndConditions) ?? new TermsAndConditionsData();
        }
        catch
        {
            return new TermsAndConditionsData();
        }
    }

    public void SetTermsAndConditions(TermsAndConditionsData data)
    {
        TermsAndConditions = JsonSerializer.Serialize(data);
    }

    public OrganizerInfoData GetOrganizerInfo()
    {
        if (string.IsNullOrEmpty(OrganizerInfo))
            return new OrganizerInfoData();
        
        try
        {
            return JsonSerializer.Deserialize<OrganizerInfoData>(OrganizerInfo) ?? new OrganizerInfoData();
        }
        catch
        {
            return new OrganizerInfoData();
        }
    }

    public void SetOrganizerInfo(OrganizerInfoData data)
    {
        OrganizerInfo = JsonSerializer.Serialize(data);
    }
}

// Helper classes for JSON serialization
public class EventDetailsData
{
    public string? VenueName { get; set; }
    public string? StreetAddress { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? EventImage { get; set; }
    public string? BackgroundImage { get; set; }
    public string? EventIntroduction { get; set; }
    public string? EventDetails { get; set; }
    public string? SpecialGuests { get; set; }
    public string? SpecialExperience { get; set; }
}

public class TermsAndConditionsData
{
    public string? TermsAndConditions { get; set; }
    public string? ChildrenTerms { get; set; }
    public string? VATTerms { get; set; }
}

public class OrganizerInfoData
{
    public string? OrganizerLogo { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerInfo { get; set; }
}