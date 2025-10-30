using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.DTOs;
using TheGrind5_EventManagement.Helpers;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Services
{
    public class SampleDataExportService : ISampleDataExportService
    {
        private readonly EventDBContext _context;
        private readonly ILogger<SampleDataExportService> _logger;
        private readonly string _assetsFolder;
        private readonly string _eventsFolder;
        private readonly string _avatarsFolder;

        public SampleDataExportService(
            EventDBContext context,
            ILogger<SampleDataExportService> logger)
        {
            _context = context;
            _logger = logger;
            _assetsFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "assets", "images");
            _eventsFolder = Path.Combine(_assetsFolder, "events");
            _avatarsFolder = Path.Combine(_assetsFolder, "avatars");

            // Tạo thư mục nếu chưa có
            Directory.CreateDirectory(_eventsFolder);
            Directory.CreateDirectory(_avatarsFolder);
        }

        public async Task<SampleDataExportResult> ExportSampleDataAsync(SampleDataExportOptions options)
        {
            try
            {
                var result = new SampleDataExportResult
                {
                    OutputFilePath = options.OutputFilePath
                };

                // Step 1: Process và copy images
                Dictionary<string, string> imageMappings = new();
                if (options.CopyImagesToAssets)
                {
                    var imageOptions = new ImageProcessingOptions
                    {
                        UseFriendlyNames = options.UseFriendlyImageNames,
                        SourceFolder = options.SourceImageFolder ?? _assetsFolder,
                        TargetFolder = options.TargetImageFolder ?? _assetsFolder
                    };

                    var imageResult = await ProcessAndCopyImagesAsync(imageOptions);
                    if (!imageResult.Success)
                    {
                        result.Success = false;
                        result.ErrorMessage = imageResult.ErrorMessage;
                        return result;
                    }

                    imageMappings = imageResult.Mappings;
                    result.ImageMappings = imageMappings;
                    result.ImagesProcessed = imageResult.ImagesCopied;
                }

                // Step 2: Query data từ database với LINQ
                var sqlBuilder = new StringBuilder();
                
                // Export Users
                if (options.IncludeUsers)
                {
                    var users = await _context.Users
                        .Where(u => !string.IsNullOrEmpty(u.Avatar))
                        .ToListAsync();

                    result.UsersExported = users.Count;

                    sqlBuilder.AppendLine("-- ===========================================");
                    sqlBuilder.AppendLine("-- USERS");
                    sqlBuilder.AppendLine("-- ===========================================");
                    sqlBuilder.AppendLine();

                    foreach (var user in users)
                    {
                        var avatarPath = imageMappings.ContainsKey(user.Avatar ?? "")
                            ? imageMappings[user.Avatar ?? ""]
                            : user.Avatar ?? "NULL";

                        sqlBuilder.AppendLine($"UPDATE [User] SET Avatar = '{EscapeSqlString(avatarPath)}' WHERE UserID = {user.UserId};");
                    }
                    sqlBuilder.AppendLine();
                }

                // Export Events
                if (options.IncludeEvents)
                {
                    var events = await _context.Events
                        .Include(e => e.TicketTypes)
                        .Include(e => e.Host)
                        .OrderBy(e => e.EventId)
                        .ToListAsync();

                    result.EventsExported = events.Count;

                    sqlBuilder.AppendLine("-- ===========================================");
                    sqlBuilder.AppendLine("-- EVENTS");
                    sqlBuilder.AppendLine("-- ===========================================");
                    sqlBuilder.AppendLine();

                    foreach (var evt in events)
                    {
                        // Process EventDetails JSON
                        var eventDetails = evt.GetEventDetails();
                        var updatedEventDetails = UpdateImagePathsInEventDetails(eventDetails, imageMappings);
                        evt.SetEventDetails(updatedEventDetails);

                        // Process OrganizerInfo JSON
                        var organizerInfo = evt.GetOrganizerInfo();
                        if (!string.IsNullOrEmpty(organizerInfo.OrganizerLogo))
                        {
                            organizerInfo.OrganizerLogo = imageMappings.ContainsKey(organizerInfo.OrganizerLogo)
                                ? imageMappings[organizerInfo.OrganizerLogo]
                                : organizerInfo.OrganizerLogo;
                        }
                        evt.SetOrganizerInfo(organizerInfo);

                        // Generate UPDATE SQL
                        sqlBuilder.AppendLine($"-- Event: {evt.Title}");
                        sqlBuilder.AppendLine($"UPDATE [Event] SET");
                        sqlBuilder.AppendLine($"  EventDetails = '{EscapeSqlString(evt.EventDetails ?? "NULL")}',");
                        sqlBuilder.AppendLine($"  OrganizerInfo = '{EscapeSqlString(evt.OrganizerInfo ?? "NULL")}'");
                        sqlBuilder.AppendLine($"WHERE EventId = {evt.EventId};");
                        sqlBuilder.AppendLine();
                    }
                }

                // Step 3: Write SQL to file
                result.GeneratedSql = sqlBuilder.ToString();
                await File.WriteAllTextAsync(options.OutputFilePath, result.GeneratedSql, Encoding.UTF8);

                result.Success = true;
                _logger.LogInformation($"Export completed: {result.EventsExported} events, {result.UsersExported} users, {result.ImagesProcessed} images");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting sample data");
                return new SampleDataExportResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<SampleDataExportResult> ExportEventsOnlyAsync(SampleDataExportOptions options)
        {
            options.IncludeUsers = false;
            options.IncludeTickets = false;
            options.IncludeOrders = false;
            return await ExportSampleDataAsync(options);
        }

        public async Task<ImageMappingResult> ProcessAndCopyImagesAsync(ImageProcessingOptions options)
        {
            try
            {
                var result = new ImageMappingResult();
                var mappings = new Dictionary<string, string>();

                // Get all events với LINQ
                var events = await _context.Events
                    .Where(e => !string.IsNullOrEmpty(e.EventDetails))
                    .ToListAsync();

                var eventImageCounts = new Dictionary<int, int>();

                foreach (var evt in events)
                {
                    var eventDetails = evt.GetEventDetails();

                    // Process EventImage
                    if (!string.IsNullOrEmpty(eventDetails.EventImage))
                    {
                        var newName = options.UseFriendlyNames
                            ? ImagePathConverter.GenerateFriendlyEventImageName(
                                evt.EventId,
                                evt.Title,
                                eventImageCounts.GetValueOrDefault(evt.EventId, 0) + 1)
                            : ImagePathConverter.ExtractFileNameFromUrl(eventDetails.EventImage);

                        if (!eventImageCounts.ContainsKey(evt.EventId))
                            eventImageCounts[evt.EventId] = 0;
                        eventImageCounts[evt.EventId]++;

                        var copyResult = await CopyImageAsync(
                            eventDetails.EventImage,
                            newName,
                            "events",
                            options);

                        if (copyResult.success)
                        {
                            mappings[eventDetails.EventImage] = copyResult.newPath;
                        }
                        else
                        {
                            result.Errors.Add($"Failed to copy: {eventDetails.EventImage}");
                        }
                    }

                    // Process BackgroundImage
                    if (!string.IsNullOrEmpty(eventDetails.BackgroundImage))
                    {
                        var newName = options.UseFriendlyNames
                            ? ImagePathConverter.GenerateFriendlyEventImageName(
                                evt.EventId,
                                evt.Title,
                                eventImageCounts.GetValueOrDefault(evt.EventId, 0) + 1)
                            : ImagePathConverter.ExtractFileNameFromUrl(eventDetails.BackgroundImage);

                        if (!eventImageCounts.ContainsKey(evt.EventId))
                            eventImageCounts[evt.EventId] = 0;
                        eventImageCounts[evt.EventId]++;

                        var copyResult = await CopyImageAsync(
                            eventDetails.BackgroundImage,
                            newName,
                            "events",
                            options);

                        if (copyResult.success)
                        {
                            mappings[eventDetails.BackgroundImage] = copyResult.newPath;
                        }
                        else
                        {
                            result.Errors.Add($"Failed to copy: {eventDetails.BackgroundImage}");
                        }
                    }

                    // Process OrganizerLogo
                    var organizerInfo = evt.GetOrganizerInfo();
                    if (!string.IsNullOrEmpty(organizerInfo.OrganizerLogo))
                    {
                        var newName = options.UseFriendlyNames
                            ? ImagePathConverter.GenerateFriendlyAvatarName(evt.HostId)
                            : ImagePathConverter.ExtractFileNameFromUrl(organizerInfo.OrganizerLogo);

                        var copyResult = await CopyImageAsync(
                            organizerInfo.OrganizerLogo,
                            newName,
                            "events", // Logo organizer cũng lưu trong events folder
                            options);

                        if (copyResult.success)
                        {
                            mappings[organizerInfo.OrganizerLogo] = copyResult.newPath;
                        }
                        else
                        {
                            result.Errors.Add($"Failed to copy: {organizerInfo.OrganizerLogo}");
                        }
                    }
                }

                // Process user avatars với LINQ
                var users = await _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.Avatar))
                    .ToListAsync();

                foreach (var user in users)
                {
                    var newName = options.UseFriendlyNames
                        ? ImagePathConverter.GenerateFriendlyAvatarName(user.UserId)
                        : ImagePathConverter.ExtractFileNameFromUrl(user.Avatar ?? "");

                    var copyResult = await CopyImageAsync(
                        user.Avatar ?? "",
                        newName,
                        "avatars",
                        options);

                    if (copyResult.success)
                    {
                        mappings[user.Avatar ?? ""] = copyResult.newPath;
                    }
                    else
                    {
                        result.Errors.Add($"Failed to copy: {user.Avatar}");
                    }
                }

                result.Success = true;
                result.ImagesCopied = mappings.Count;
                result.Mappings = mappings;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing images");
                return new ImageMappingResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<(bool success, string newPath)> CopyImageAsync(
            string sourceImageUrl,
            string newFileName,
            string imageType,
            ImageProcessingOptions options)
        {
            try
            {
                var sourceFileName = ImagePathConverter.ExtractFileNameFromUrl(sourceImageUrl);
                if (string.IsNullOrEmpty(sourceFileName))
                    return (false, "");

                var sourceFolder = imageType == "events" ? _eventsFolder : _avatarsFolder;
                var targetFolder = options.TargetFolder ?? 
                    (imageType == "events" ? _eventsFolder : _avatarsFolder);

                var sourcePath = Path.Combine(sourceFolder, sourceFileName);
                if (!File.Exists(sourcePath))
                {
                    // Thử tìm trong wwwroot/uploads
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (imageType == "events")
                        wwwrootPath = Path.Combine(wwwrootPath, "events");
                    else
                        wwwrootPath = Path.Combine(wwwrootPath, "avatars");

                    sourcePath = Path.Combine(wwwrootPath, sourceFileName);
                    if (!File.Exists(sourcePath))
                    {
                        _logger.LogWarning($"Image not found: {sourceFileName}");
                        return (false, "");
                    }
                }

                var targetPath = Path.Combine(targetFolder, newFileName);

                // Copy file
                File.Copy(sourcePath, targetPath, overwrite: true);

                var newPath = $"/assets/images/{imageType}/{newFileName}";
                return (true, newPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error copying image: {sourceImageUrl}");
                return (false, "");
            }
        }

        private EventDetailsData UpdateImagePathsInEventDetails(
            EventDetailsData eventDetails,
            Dictionary<string, string> imageMappings)
        {
            if (eventDetails == null)
                return new EventDetailsData();

            if (!string.IsNullOrEmpty(eventDetails.EventImage) &&
                imageMappings.ContainsKey(eventDetails.EventImage))
            {
                eventDetails.EventImage = imageMappings[eventDetails.EventImage];
            }

            if (!string.IsNullOrEmpty(eventDetails.BackgroundImage) &&
                imageMappings.ContainsKey(eventDetails.BackgroundImage))
            {
                eventDetails.BackgroundImage = imageMappings[eventDetails.BackgroundImage];
            }

            // Update images array if exists
            if (eventDetails.images != null && eventDetails.images.Length > 0)
            {
                eventDetails.images = eventDetails.images
                    .Select(img => imageMappings.ContainsKey(img) ? imageMappings[img] : img)
                    .ToArray();
            }

            return eventDetails;
        }

        private string EscapeSqlString(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "NULL")
                return "NULL";

            return value.Replace("'", "''").Replace("\r\n", " ").Replace("\n", " ");
        }
    }
}

