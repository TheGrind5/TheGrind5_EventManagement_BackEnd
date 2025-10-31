using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public interface IEventQuestionRepository
    {
        Task<IEnumerable<EventQuestion>> GetByEventIdAsync(int eventId);
        Task<EventQuestion?> GetByIdAsync(int questionId);
        Task<EventQuestion> CreateAsync(EventQuestion question);
        Task<EventQuestion> UpdateAsync(EventQuestion question);
        Task DeleteAsync(int questionId);
    }
}

