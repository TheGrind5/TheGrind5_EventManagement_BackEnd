using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business;

public interface IAIChatbotService
{
    Task<ChatbotResponse> GetChatbotResponseAsync(string question, int? eventId = null, int? userId = null);
}

