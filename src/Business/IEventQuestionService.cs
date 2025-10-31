using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Business
{
    public interface IEventQuestionService
    {
        Task<IEnumerable<EventQuestionDTO>> GetByEventIdAsync(int eventId);
        Task<EventQuestionDTO> GetByIdAsync(int questionId);
        Task<EventQuestionDTO> CreateAsync(CreateEventQuestionDTO dto, int hostId);
        Task<EventQuestionDTO> UpdateAsync(int questionId, UpdateEventQuestionDTO dto, int hostId);
        Task DeleteAsync(int questionId, int hostId);
    }
}

