using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Scripts
{
    public class CleanupUnusedImages
    {
        private readonly EventDBContext _context;
        private readonly IFileManagementService _fileManagementService;

        public CleanupUnusedImages(EventDBContext context, IFileManagementService fileManagementService)
        {
            _context = context;
            _fileManagementService = fileManagementService;
        }

        public async Task<int> CleanupAllUnusedImagesAsync()
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
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events");
                if (!Directory.Exists(uploadsFolder))
                    return 0;

                var allImageFiles = Directory.GetFiles(uploadsFolder, "*.*", SearchOption.TopDirectoryOnly)
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

                // Xóa ảnh không sử dụng
                var deletedCount = 0;
                foreach (var file in unusedFiles)
                {
                    var filePath = Path.Combine(uploadsFolder, file);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        deletedCount++;
                        Console.WriteLine($"Deleted unused image: {file}");
                    }
                }

                Console.WriteLine($"Cleanup completed. Deleted {deletedCount} unused images.");
                return deletedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex.Message}");
                return 0;
            }
        }
    }
}
