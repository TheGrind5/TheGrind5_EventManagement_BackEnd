using Microsoft.AspNetCore.Diagnostics;
using TheGrind5_EventManagement.DTOs;
using System.Net;

namespace TheGrind5_EventManagement.Middleware
{
    /// <summary>
    /// Global exception handler to catch and standardize all exceptions
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var response = CreateErrorResponse(exception);
            
            httpContext.Response.StatusCode = response.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }

        private ApiResponse<object> CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                // More specific exceptions first
                ArgumentNullException nullEx => ApiResponse<object>.ErrorResponse(
                    nullEx.Message,
                    (int)HttpStatusCode.BadRequest
                ),

                ArgumentException argEx => ApiResponse<object>.ErrorResponse(
                    argEx.Message,
                    (int)HttpStatusCode.BadRequest
                ),

                InvalidOperationException invalidEx => ApiResponse<object>.ErrorResponse(
                    invalidEx.Message,
                    (int)HttpStatusCode.BadRequest
                ),

                KeyNotFoundException notFoundEx => ApiResponse<object>.ErrorResponse(
                    notFoundEx.Message,
                    (int)HttpStatusCode.NotFound
                ),

                UnauthorizedAccessException => ApiResponse<object>.ErrorResponse(
                    "Unauthorized access",
                    (int)HttpStatusCode.Unauthorized
                ),

                // Default case
                _ => ApiResponse<object>.ErrorResponse(
                    "An internal server error occurred. Please try again later.",
                    (int)HttpStatusCode.InternalServerError
                )
            };
        }
    }
}

