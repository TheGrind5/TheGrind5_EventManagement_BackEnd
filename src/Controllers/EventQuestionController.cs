using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Business;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TheGrind5_EventManagement.Controllers
{
    [Route("api/EventQuestion")]
    [ApiController]
    public class EventQuestionController : ControllerBase
    {
        private readonly IEventQuestionService _questionService;

        public EventQuestionController(IEventQuestionService questionService)
        {
            _questionService = questionService;
        }

        /// <summary>
        /// Get all questions for a specific event
        /// </summary>
        [HttpGet("by-event/{eventId}")]
        public async Task<IActionResult> GetByEventId(int eventId)
        {
            try
            {
                if (eventId <= 0)
                    return BadRequest(new { message = "ID sự kiện không hợp lệ" });

                var questions = await _questionService.GetByEventIdAsync(eventId);
                return Ok(new { data = questions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy danh sách câu hỏi", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific question by ID
        /// </summary>
        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetById(int questionId)
        {
            try
            {
                if (questionId <= 0)
                    return BadRequest(new { message = "ID câu hỏi không hợp lệ" });

                var question = await _questionService.GetByIdAsync(questionId);
                return Ok(new { data = question });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi lấy thông tin câu hỏi", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new question for an event
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEventQuestionDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

                var hostId = int.Parse(User.FindFirstValue("userId") ?? "0");
                if (hostId == 0)
                    return Unauthorized(new { message = "Không thể xác định người dùng" });

                var question = await _questionService.CreateAsync(request, hostId);
                return CreatedAtAction(nameof(GetById), new { questionId = question.QuestionId }, new { data = question });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi tạo câu hỏi", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing question
        /// </summary>
        [HttpPut("{questionId}")]
        [Authorize]
        public async Task<IActionResult> Update(int questionId, [FromBody] UpdateEventQuestionDTO request)
        {
            try
            {
                if (questionId <= 0)
                    return BadRequest(new { message = "ID câu hỏi không hợp lệ" });

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });

                var hostId = int.Parse(User.FindFirstValue("userId") ?? "0");
                if (hostId == 0)
                    return Unauthorized(new { message = "Không thể xác định người dùng" });

                var question = await _questionService.UpdateAsync(questionId, request, hostId);
                return Ok(new { data = question });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi cập nhật câu hỏi", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a question
        /// </summary>
        [HttpDelete("{questionId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int questionId)
        {
            try
            {
                if (questionId <= 0)
                    return BadRequest(new { message = "ID câu hỏi không hợp lệ" });

                var hostId = int.Parse(User.FindFirstValue("userId") ?? "0");
                if (hostId == 0)
                    return Unauthorized(new { message = "Không thể xác định người dùng" });

                await _questionService.DeleteAsync(questionId, hostId);
                return Ok(new { message = "Xóa câu hỏi thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Có lỗi xảy ra khi xóa câu hỏi", error = ex.Message });
            }
        }
    }
}

