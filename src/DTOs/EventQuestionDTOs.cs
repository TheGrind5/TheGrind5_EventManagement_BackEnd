using System.ComponentModel.DataAnnotations;

namespace TheGrind5_EventManagement.DTOs
{
    // Request DTO để tạo câu hỏi mới
    public record CreateEventQuestionDTO
    {
        [Required]
        public int EventId { get; init; }

        [Required]
        [MaxLength(500)]
        public string QuestionText { get; init; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string QuestionType { get; init; } = "Text"; // "Text", "Number", "Email", "Phone", "Date", "Radio", "Checkbox", "Dropdown"

        public bool IsRequired { get; init; } = true;

        public string? Options { get; init; } // JSON array for Radio/Checkbox/Dropdown

        [MaxLength(500)]
        public string? Placeholder { get; init; }

        public string? ValidationRules { get; init; } // JSON

        public int DisplayOrder { get; init; } = 0;
    }

    // Request DTO để update câu hỏi
    public record UpdateEventQuestionDTO
    {
        [MaxLength(500)]
        public string? QuestionText { get; init; }

        [MaxLength(50)]
        public string? QuestionType { get; init; }

        public bool? IsRequired { get; init; }

        public string? Options { get; init; }

        [MaxLength(500)]
        public string? Placeholder { get; init; }

        public string? ValidationRules { get; init; }

        public int? DisplayOrder { get; init; }
    }

    // Response DTO để trả về câu hỏi
    public record EventQuestionDTO
    {
        public int QuestionId { get; init; }
        public int EventId { get; init; }
        public string QuestionText { get; init; } = string.Empty;
        public string QuestionType { get; init; } = "Text";
        public bool IsRequired { get; init; }
        public string? Options { get; init; }
        public string? Placeholder { get; init; }
        public string? ValidationRules { get; init; }
        public int DisplayOrder { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    // Response DTO để trả về danh sách câu hỏi theo event
    public record GetEventQuestionsByEventIdDTO
    {
        public int EventId { get; init; }
        public List<EventQuestionDTO> Questions { get; init; } = new();
    }
}

