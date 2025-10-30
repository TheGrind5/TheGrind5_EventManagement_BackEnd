using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Services;

namespace TheGrind5_EventManagement.Controllers
{
    /// <summary>
    /// Controller để export sample data kèm ảnh ra SQL script
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly ISampleDataExportService _exportService;
        private readonly ILogger<ExportController> _logger;

        public ExportController(
            ISampleDataExportService exportService,
            ILogger<ExportController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        /// <summary>
        /// Export tất cả sample data kèm ảnh ra SQL script
        /// </summary>
        [HttpPost("sample-data")]
        [Authorize] // Chỉ admin hoặc authorized user mới được export
        public async Task<IActionResult> ExportSampleData([FromBody] ExportSampleDataRequest request)
        {
            try
            {
                var options = new SampleDataExportOptions
                {
                    OutputFilePath = request.CustomOutputFileName ?? 
                        $"SampleData_Export_{DateTime.Now:yyyyMMdd_HHmmss}.sql",
                    IncludeEvents = request.IncludeEvents,
                    IncludeUsers = request.IncludeUsers,
                    IncludeTickets = request.IncludeTickets,
                    IncludeOrders = request.IncludeOrders,
                    UseFriendlyImageNames = request.UseFriendlyImageNames,
                    CopyImagesToAssets = request.CopyImagesToAssets
                };

                var result = await _exportService.ExportSampleDataAsync(options);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                return Ok(new
                {
                    success = true,
                    message = "Export thành công!",
                    outputFile = result.OutputFilePath,
                    statistics = new
                    {
                        eventsExported = result.EventsExported,
                        usersExported = result.UsersExported,
                        imagesProcessed = result.ImagesProcessed
                    },
                    imageMappings = result.ImageMappings
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExportSampleData");
                return StatusCode(500, new { message = "Có lỗi xảy ra khi export", error = ex.Message });
            }
        }

        /// <summary>
        /// Export chỉ events kèm ảnh
        /// </summary>
        [HttpPost("events-only")]
        [Authorize]
        public async Task<IActionResult> ExportEventsOnly([FromBody] ExportSampleDataRequest request)
        {
            try
            {
                var options = new SampleDataExportOptions
                {
                    OutputFilePath = request.CustomOutputFileName ?? 
                        $"Events_Export_{DateTime.Now:yyyyMMdd_HHmmss}.sql",
                    IncludeEvents = true,
                    IncludeUsers = false,
                    IncludeTickets = false,
                    IncludeOrders = false,
                    UseFriendlyImageNames = request.UseFriendlyImageNames,
                    CopyImagesToAssets = request.CopyImagesToAssets
                };

                var result = await _exportService.ExportEventsOnlyAsync(options);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                return Ok(new
                {
                    success = true,
                    message = "Export events thành công!",
                    outputFile = result.OutputFilePath,
                    statistics = new
                    {
                        eventsExported = result.EventsExported,
                        imagesProcessed = result.ImagesProcessed
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExportEventsOnly");
                return StatusCode(500, new { message = "Có lỗi xảy ra khi export", error = ex.Message });
            }
        }

        /// <summary>
        /// Download file SQL đã export
        /// </summary>
        [HttpGet("download/{fileName}")]
        [Authorize]
        public IActionResult DownloadFile(string fileName)
        {
            try
            {
                // Validate fileName để tránh path traversal
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest(new { message = "Tên file không hợp lệ" });
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File không tồn tại" });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = "application/sql";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tải file" });
            }
        }
    }
}

