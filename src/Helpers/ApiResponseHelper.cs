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
    }
}

