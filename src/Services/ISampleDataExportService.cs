using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services
{
    /// <summary>
    /// Service để export sample data kèm ảnh ra SQL script
    /// </summary>
    public interface ISampleDataExportService
    {
        /// <summary>
        /// Export tất cả events, users, tickets kèm ảnh ra SQL script
        /// </summary>
        /// <param name="options">Cấu hình export</param>
        /// <returns>Kết quả export</returns>
        Task<SampleDataExportResult> ExportSampleDataAsync(SampleDataExportOptions options);

        /// <summary>
        /// Export chỉ events kèm ảnh
        /// </summary>
        Task<SampleDataExportResult> ExportEventsOnlyAsync(SampleDataExportOptions options);

        /// <summary>
        /// Copy và đổi tên ảnh theo quy tắc friendly naming
        /// </summary>
        Task<ImageMappingResult> ProcessAndCopyImagesAsync(ImageProcessingOptions options);
    }
}

