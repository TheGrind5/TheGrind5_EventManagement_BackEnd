using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;

namespace TheGrind5_EventManagement.Services
{
    public class EventQuestionService : IEventQuestionService
    {
        private readonly IEventQuestionRepository _questionRepository;
        private readonly IEventRepository _eventRepository;

        public EventQuestionService(
            IEventQuestionRepository questionRepository,
            IEventRepository eventRepository)
        {
            _questionRepository = questionRepository;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventQuestionDTO>> GetByEventIdAsync(int eventId)
        {
            var questions = await _questionRepository.GetByEventIdAsync(eventId);
            return questions.Select(q => MapToDTO(q));
        }

        public async Task<EventQuestionDTO> GetByIdAsync(int questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException($"Question with ID {questionId} not found");

            return MapToDTO(question);
        }

        public async Task<EventQuestionDTO> CreateAsync(CreateEventQuestionDTO dto, int hostId)
        {
            // Validate event exists and user is host
            var eventData = await _eventRepository.GetEventByIdAsync(dto.EventId);
            if (eventData == null)
                throw new KeyNotFoundException($"Event with ID {dto.EventId} not found");

            if (eventData.HostId != hostId)
                throw new UnauthorizedAccessException("Only the event host can create questions");

            // Create question
            var question = new EventQuestion
            {
                EventId = dto.EventId,
                QuestionText = dto.QuestionText,
                QuestionType = dto.QuestionType,
                IsRequired = dto.IsRequired,
                Options = dto.Options,
                Placeholder = dto.Placeholder,
                ValidationRules = dto.ValidationRules,
                DisplayOrder = dto.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _questionRepository.CreateAsync(question);
            return MapToDTO(created);
        }

        public async Task<EventQuestionDTO> UpdateAsync(int questionId, UpdateEventQuestionDTO dto, int hostId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException($"Question with ID {questionId} not found");

            // Validate user is host
            var eventData = await _eventRepository.GetEventByIdAsync(question.EventId);
            if (eventData == null || eventData.HostId != hostId)
                throw new UnauthorizedAccessException("Only the event host can update questions");

            // Update only provided fields
            if (dto.QuestionText != null)
                question.QuestionText = dto.QuestionText;

            if (dto.QuestionType != null)
                question.QuestionType = dto.QuestionType;

            if (dto.IsRequired.HasValue)
                question.IsRequired = dto.IsRequired.Value;

            if (dto.Options != null)
                question.Options = dto.Options;

            if (dto.Placeholder != null)
                question.Placeholder = dto.Placeholder;

            if (dto.ValidationRules != null)
                question.ValidationRules = dto.ValidationRules;

            if (dto.DisplayOrder.HasValue)
                question.DisplayOrder = dto.DisplayOrder.Value;

            var updated = await _questionRepository.UpdateAsync(question);
            return MapToDTO(updated);
        }

        public async Task DeleteAsync(int questionId, int hostId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException($"Question with ID {questionId} not found");

            // Validate user is host
            var eventData = await _eventRepository.GetEventByIdAsync(question.EventId);
            if (eventData == null || eventData.HostId != hostId)
                throw new UnauthorizedAccessException("Only the event host can delete questions");

            await _questionRepository.DeleteAsync(questionId);
        }

        private EventQuestionDTO MapToDTO(EventQuestion question)
        {
            return new EventQuestionDTO
            {
                QuestionId = question.QuestionId,
                EventId = question.EventId,
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType,
                IsRequired = question.IsRequired,
                Options = question.Options,
                Placeholder = question.Placeholder,
                ValidationRules = question.ValidationRules,
                DisplayOrder = question.DisplayOrder,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt
            };
        }
    }
}

