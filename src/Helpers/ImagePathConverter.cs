using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TheGrind5_EventManagement.Helpers
{
    /// <summary>
    /// Helper để chuyển đổi đường dẫn ảnh và tạo tên friendly
    /// </summary>
    public static class ImagePathConverter
    {
        /// <summary>
        /// Tạo tên file friendly từ EventId và Title
        /// </summary>
        public static string GenerateFriendlyEventImageName(int eventId, string eventTitle, int index = 1)
        {
            var slug = ConvertToSlug(eventTitle);
            var extension = ".jpg"; // Default extension
            return $"event_{eventId}_{slug}_{index}{extension}";
        }

        /// <summary>
        /// Tạo tên file friendly cho avatar
        /// </summary>
        public static string GenerateFriendlyAvatarName(int userId)
        {
            return $"user_{userId}.jpg";
        }

        /// <summary>
        /// Chuyển đổi tiếng Việt có dấu sang không dấu và tạo slug
        /// </summary>
        public static string ConvertToSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "event";

            // Remove diacritics (Vietnamese)
            var normalized = RemoveVietnameseDiacritics(text.ToLowerInvariant());

            // Replace spaces with underscores
            normalized = normalized.Replace(" ", "_");

            // Remove special characters, keep only letters, numbers, and underscores
            normalized = Regex.Replace(normalized, @"[^a-z0-9_]", string.Empty);

            // Remove multiple underscores
            normalized = Regex.Replace(normalized, @"_+", "_");

            // Trim underscores from start and end
            normalized = normalized.Trim('_');

            // Ensure not empty
            if (string.IsNullOrEmpty(normalized))
                return "event";

            // Limit length
            if (normalized.Length > 50)
                normalized = normalized.Substring(0, 50).TrimEnd('_');

            return normalized;
        }

        /// <summary>
        /// Remove Vietnamese diacritics
        /// </summary>
        private static string RemoveVietnameseDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var result = new StringBuilder(text.Length);
            
            foreach (var c in text)
            {
                switch (c)
                {
                    // a, ă, â
                    case 'à': case 'á': case 'ả': case 'ã': case 'ạ':
                    case 'ắ': case 'ằ': case 'ẳ': case 'ẵ': case 'ặ':
                    case 'ấ': case 'ầ': case 'ẩ': case 'ẫ': case 'ậ':
                        result.Append('a');
                        break;
                    case 'ă':
                        result.Append('a');
                        break;
                    case 'â':
                        result.Append('a');
                        break;

                    // e, ê
                    case 'è': case 'é': case 'ẻ': case 'ẽ': case 'ẹ':
                    case 'ề': case 'ế': case 'ể': case 'ễ': case 'ệ':
                        result.Append('e');
                        break;
                    case 'ê':
                        result.Append('e');
                        break;

                    // i
                    case 'ì': case 'í': case 'ỉ': case 'ĩ': case 'ị':
                        result.Append('i');
                        break;

                    // o, ô, ơ
                    case 'ò': case 'ó': case 'ỏ': case 'õ': case 'ọ':
                    case 'ồ': case 'ố': case 'ổ': case 'ỗ': case 'ộ':
                    case 'ờ': case 'ớ': case 'ở': case 'ỡ': case 'ợ':
                        result.Append('o');
                        break;
                    case 'ô':
                        result.Append('o');
                        break;
                    case 'ơ':
                        result.Append('o');
                        break;

                    // u, ư
                    case 'ù': case 'ú': case 'ủ': case 'ũ': case 'ụ':
                    case 'ừ': case 'ứ': case 'ử': case 'ữ': case 'ự':
                        result.Append('u');
                        break;
                    case 'ư':
                        result.Append('u');
                        break;

                    // y
                    case 'ỳ': case 'ý': case 'ỷ': case 'ỹ': case 'ỵ':
                        result.Append('y');
                        break;

                    // đ
                    case 'đ':
                        result.Append('d');
                        break;

                    // Đ
                    case 'Đ':
                        result.Append('d');
                        break;

                    default:
                        result.Append(c);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Extract filename from URL path
        /// </summary>
        public static string ExtractFileNameFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // Extract from /assets/images/events/filename.jpg
            var match = Regex.Match(imageUrl, @"/assets/images/(events|avatars)/([^/]+)$");
            return match.Success ? match.Groups[2].Value : string.Empty;
        }

        /// <summary>
        /// Get image type from URL (events or avatars)
        /// </summary>
        public static string GetImageTypeFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return "events";

            if (imageUrl.Contains("/avatars/"))
                return "avatars";

            return "events";
        }

        /// <summary>
        /// Get file extension from filename or URL
        /// </summary>
        public static string GetFileExtension(string fileNameOrUrl)
        {
            if (string.IsNullOrEmpty(fileNameOrUrl))
                return ".jpg";

            var extension = Path.GetExtension(fileNameOrUrl);
            return string.IsNullOrEmpty(extension) ? ".jpg" : extension;
        }
    }
}

