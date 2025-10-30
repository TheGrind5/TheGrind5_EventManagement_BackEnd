namespace TheGrind5_EventManagement.DTOs
{
    /// <summary>
    /// Cấu hình cho việc export sample data
    /// </summary>
    public class SampleDataExportOptions
    {
        /// <summary>
        /// Output file path (SQL file)
        /// </summary>
        public string OutputFilePath { get; set; } = "SampleData_Export.sql";

        /// <summary>
        /// Include events trong export
        /// </summary>
        public bool IncludeEvents { get; set; } = true;

        /// <summary>
        /// Include users trong export
        /// </summary>
        public bool IncludeUsers { get; set; } = true;

        /// <summary>
        /// Include tickets trong export
        /// </summary>
        public bool IncludeTickets { get; set; } = true;

        /// <summary>
        /// Include orders trong export
        /// </summary>
        public bool IncludeOrders { get; set; } = false;

        /// <summary>
        /// Đổi tên ảnh thành friendly names
        /// </summary>
        public bool UseFriendlyImageNames { get; set; } = true;

        /// <summary>
        /// Copy ảnh vào assets/images
        /// </summary>
        public bool CopyImagesToAssets { get; set; } = true;

        /// <summary>
        /// Thư mục nguồn ảnh (mặc định: assets/images)
        /// </summary>
        public string? SourceImageFolder { get; set; }

        /// <summary>
        /// Thư mục đích cho ảnh (mặc định: assets/images)
        /// </summary>
        public string? TargetImageFolder { get; set; }
    }

    /// <summary>
    /// Kết quả export
    /// </summary>
    public class SampleDataExportResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string OutputFilePath { get; set; } = string.Empty;
        public int EventsExported { get; set; }
        public int UsersExported { get; set; }
        public int ImagesProcessed { get; set; }
        public Dictionary<string, string> ImageMappings { get; set; } = new();
        public string? GeneratedSql { get; set; }
    }

    /// <summary>
    /// Cấu hình xử lý ảnh
    /// </summary>
    public class ImageProcessingOptions
    {
        public bool UseFriendlyNames { get; set; } = true;
        public string? SourceFolder { get; set; }
        public string? TargetFolder { get; set; }
        public Dictionary<string, string>? NameMappings { get; set; }
    }

    /// <summary>
    /// Kết quả mapping ảnh
    /// </summary>
    public class ImageMappingResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int ImagesCopied { get; set; }
        public Dictionary<string, string> Mappings { get; set; } = new(); // oldPath -> newPath
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// Request DTO cho API export
    /// </summary>
    public class ExportSampleDataRequest
    {
        public bool IncludeEvents { get; set; } = true;
        public bool IncludeUsers { get; set; } = true;
        public bool IncludeTickets { get; set; } = true;
        public bool IncludeOrders { get; set; } = false;
        public bool UseFriendlyImageNames { get; set; } = true;
        public bool CopyImagesToAssets { get; set; } = true;
        public string? CustomOutputFileName { get; set; }
    }
}

