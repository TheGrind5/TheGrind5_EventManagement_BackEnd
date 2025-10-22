namespace TheGrind5_EventManagement.Helpers
{
    public static class ApiResponseHelper
    {
        public static object Success<T>(T data, string message = "Success")
        {
            return new
            {
                success = true,
                message = message,
                data = data
            };
        }

        public static object Error(string message, object? errors = null)
        {
            return new
            {
                success = false,
                message = message,
                errors = errors
            };
        }

        public static object ValidationError(Dictionary<string, string[]> errors)
        {
            return new
            {
                success = false,
                message = "Validation failed",
                errors = errors
            };
        }

        public static object InternalServerError(string message = "Internal server error")
        {
            return new
            {
                success = false,
                message = message,
                statusCode = 500
            };
        }

        public static object BadRequest(string message = "Bad request")
        {
            return new
            {
                success = false,
                message = message,
                statusCode = 400
            };
        }

        public static object NotFound(string message = "Not found")
        {
            return new
            {
                success = false,
                message = message,
                statusCode = 404
            };
        }
    }
}

