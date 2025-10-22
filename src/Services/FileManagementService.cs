using System.Text.RegularExpressions;
using TheGrind5_EventManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Services
{
    public interface IFileManagementService
    {
        Task<string> SaveEventImageAsync(IFormFile file);
        Task<bool> DeleteEventImageAsync(string imageUrl);
        Task<bool> DeleteEventImagesAsync(List<string> imageUrls);
        Task<List<string>> GetUnusedEventImagesAsync();
        Task<int> CleanupUnusedImagesAsync();
        Task<bool> ImageExistsAsync(string imageUrl);
    }

    public class FileManagementService : IFileManagementService
    {
        private readonly string _uploadsFolder;
        private readonly string _eventsFolder;
        private readonly ILogger<FileManagementService> _logger;
        private readonly EventDBContext _context;

        public FileManagementService(ILogger<FileManagementService> logger, EventDBContext context)
        {
            _logger = logger;
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            _eventsFolder = Path.Combine(_uploadsFolder, "events");
            
            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(_eventsFolder))
                Directory.CreateDirectory(_eventsFolder);
        }

        public async Task<string> SaveEventImageAsync(IFormFile file)
        {
            try
            {
                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    throw new ArgumentException("Chỉ chấp nhận file ảnh (JPG, PNG, GIF, WEBP)");

                // Kiểm tra kích thước file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("File quá lớn, tối đa 5MB");

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_eventsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/events/{fileName}";
                _logger.LogInformation($"Event image saved: {imageUrl}");
                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving event image");
                throw;
            }
        }

        public async Task<bool> DeleteEventImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return false;

                var fileName = ExtractFileNameFromUrl(imageUrl);
                if (string.IsNullOrEmpty(fileName))
                    return false;

                var filePath = Path.Combine(_eventsFolder, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"Event image deleted: {imageUrl}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting event image: {imageUrl}");
                return false;
            }
        }

        public async Task<bool> DeleteEventImagesAsync(List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any())
                return true;

            var successCount = 0;
            foreach (var imageUrl in imageUrls)
            {
                if (await DeleteEventImageAsync(imageUrl))
                    successCount++;
            }

            _logger.LogInformation($"Deleted {successCount}/{imageUrls.Count} event images");
            return successCount == imageUrls.Count;
        }

        public async Task<List<string>> GetUnusedEventImagesAsync()
        {
            try
            {
                // Lấy tất cả ảnh đang được sử dụng từ database
                var usedImages = new HashSet<string>();
                var events = await _context.Events.ToListAsync();
                
                foreach (var eventItem in events)
                {
                    var eventDetails = eventItem.GetEventDetails();
                    if (!string.IsNullOrEmpty(eventDetails.EventImage))
                        usedImages.Add(eventDetails.EventImage);
                    if (!string.IsNullOrEmpty(eventDetails.BackgroundImage))
                        usedImages.Add(eventDetails.BackgroundImage);
                }

                // Lấy tất cả file ảnh trong thư mục
                var allImageFiles = Directory.GetFiles(_eventsFolder, "*.*", SearchOption.TopDirectoryOnly)
                    .Select(f => Path.GetFileName(f))
                    .ToList();

                // Tìm ảnh không sử dụng
                var unusedFiles = new List<string>();
                foreach (var file in allImageFiles)
                {
                    var imageUrl = $"/uploads/events/{file}";
                    if (!usedImages.Contains(imageUrl))
                    {
                        unusedFiles.Add(file);
                    }
                }

                return unusedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unused event images");
                return new List<string>();
            }
        }

        public async Task<int> CleanupUnusedImagesAsync()
        {
            try
            {
                var unusedImages = await GetUnusedEventImagesAsync();
                var deletedCount = 0;

                foreach (var imageFile in unusedImages)
                {
                    var filePath = Path.Combine(_eventsFolder, imageFile);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        deletedCount++;
                    }
                }

                _logger.LogInformation($"Cleaned up {deletedCount} unused event images");
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up unused images");
                return 0;
            }
        }

        public async Task<bool> ImageExistsAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return false;

                var fileName = ExtractFileNameFromUrl(imageUrl);
                if (string.IsNullOrEmpty(fileName))
                    return false;

                var filePath = Path.Combine(_eventsFolder, fileName);
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if image exists: {imageUrl}");
                return false;
            }
        }

        private string ExtractFileNameFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // Extract filename from URL like "/uploads/events/filename.jpg"
            var match = Regex.Match(imageUrl, @"/uploads/events/([^/]+)$");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
