using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Helpers;

namespace TheGrind5_EventManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampusController : ControllerBase
{
    private readonly EventDBContext _context;
    private readonly ILogger<CampusController> _logger;

    public CampusController(EventDBContext context, ILogger<CampusController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/campus
    [HttpGet]
    public IActionResult GetAllCampuses()
    {
        try
        {
            var campuses = _context.Campuses.ToList();
            return Ok(ApiResponseHelper.Success(campuses));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all campuses");
            return StatusCode(500, ApiResponseHelper.Error("Lỗi khi lấy danh sách campus"));
        }
    }

    // GET: api/campus/{id}
    [HttpGet("{id}")]
    public IActionResult GetCampusById(int id)
    {
        try
        {
            var campus = _context.Campuses.Find(id);
            if (campus == null)
            {
                return NotFound(ApiResponseHelper.Error("Campus không tồn tại"));
            }
            return Ok(ApiResponseHelper.Success(campus));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campus by id: {Id}", id);
            return StatusCode(500, ApiResponseHelper.Error("Lỗi khi lấy thông tin campus"));
        }
    }

    // POST: api/campus/seed
    [HttpPost("seed")]
    public IActionResult SeedCampuses()
    {
        try
        {
            // Check if campuses already exist
            if (_context.Campuses.Any())
            {
                return BadRequest(ApiResponseHelper.Error("Campuses đã tồn tại trong database"));
            }

            var campuses = new List<Campus>
            {
                new Campus { Name = "Hà Nội", Code = "HN", CreatedAt = DateTime.Now },
                new Campus { Name = "TP. Hồ Chí Minh", Code = "HCM", CreatedAt = DateTime.Now },
                new Campus { Name = "Đà Nẵng", Code = "DN", CreatedAt = DateTime.Now },
                new Campus { Name = "Quy Nhơn", Code = "QN", CreatedAt = DateTime.Now },
                new Campus { Name = "Cần Thơ", Code = "CT", CreatedAt = DateTime.Now }
            };

            _context.Campuses.AddRange(campuses);
            _context.SaveChanges();

            return Ok(ApiResponseHelper.Success(campuses, "Đã thêm thành công 5 campus FPT"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding campuses");
            return StatusCode(500, ApiResponseHelper.Error("Lỗi khi seed dữ liệu campus"));
        }
    }
}

