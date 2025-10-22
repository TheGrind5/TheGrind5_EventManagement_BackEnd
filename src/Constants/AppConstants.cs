namespace TheGrind5_EventManagement.Constants
{
    public static class AppConstants
    {
        public const string JWT_ISSUER = "TheGrind5_EventManagement";
        public const string JWT_AUDIENCE = "TheGrind5_EventManagement_Users";
        public const string JWT_SECRET_KEY = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        public const int JWT_EXPIRY_DAYS = 7;
        
        public const string DEFAULT_ROLE = "User";
        public const string ADMIN_ROLE = "Admin";
        
        public const string CORS_POLICY_NAME = "AllowFrontend";
        public const string CORS_FRONTEND_URL = "http://localhost:3000";
        public const string CORS_FRONTEND_URL_ALT = "http://localhost:3001";
        public const string CORS_FRONTEND_URL_HTTPS = "https://localhost:3000";
        public const string CORS_FRONTEND_URL_HTTP = "http://127.0.0.1:3000";
    }
}

