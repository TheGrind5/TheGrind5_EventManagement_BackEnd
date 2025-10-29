namespace TheGrind5_EventManagement.DTOs
{
    /// <summary>
    /// Standard API Response wrapper for all endpoints
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400, object? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Pagination request parameters
    /// </summary>
    public class PagedRequest
    {
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                const int maxPageSize = 100;
                _pageSize = value switch
                {
                    < 1 => 10,
                    > maxPageSize => maxPageSize,
                    _ => value
                };
            }
        }

        public int Skip => (Page - 1) * PageSize;
    }

    /// <summary>
    /// Pagination response wrapper
    /// </summary>
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

        public PagedResponse()
        {
        }

        public PagedResponse(List<T> data, int totalCount, int page, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }

    /// <summary>
    /// Extension methods for pagination
    /// </summary>
    public static class PaginationExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, PagedRequest request)
        {
            return source
                .Skip(request.Skip)
                .Take(request.PageSize);
        }
    }
}

