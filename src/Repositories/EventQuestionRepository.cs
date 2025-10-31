using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Repositories
{
    public class EventQuestionRepository : IEventQuestionRepository
    {
        private readonly EventDBContext _context;

        public EventQuestionRepository(EventDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventQuestion>> GetByEventIdAsync(int eventId)
        {
            return await _context.EventQuestions
                .Where(eq => eq.EventId == eventId)
                .OrderBy(eq => eq.DisplayOrder)
                .ThenBy(eq => eq.QuestionId)
                .ToListAsync();
        }

        public async Task<EventQuestion?> GetByIdAsync(int questionId)
        {
            return await _context.EventQuestions
                .Include(eq => eq.Event)
                .FirstOrDefaultAsync(eq => eq.QuestionId == questionId);
        }

        public async Task<EventQuestion> CreateAsync(EventQuestion question)
        {
            _context.EventQuestions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<EventQuestion> UpdateAsync(EventQuestion question)
        {
            question.UpdatedAt = DateTime.UtcNow;
            _context.EventQuestions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task DeleteAsync(int questionId)
        {
            var question = await _context.EventQuestions.FindAsync(questionId);
            if (question != null)
            {
                _context.EventQuestions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
    }
}

