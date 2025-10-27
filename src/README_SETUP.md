# Setup Instructions

## Configuration

1. Copy `appsettings.Example.json` to `appsettings.json`
2. Update the following values in `appsettings.json`:
   - **ConnectionStrings.DefaultConnection**: Your SQL Server connection string
   - **Jwt.Key**: Generate a secure random key (at least 32 characters)
   - **Jwt.Issuer**: Your application issuer name (can keep default)
   - **Jwt.Audience**: Your application audience (can keep default)

## Security Notes

⚠️ **IMPORTANT**: 
- **NEVER** commit `appsettings.json` to version control
- Keep your JWT secret key secure
- Use strong, randomly generated keys in production
- The example file contains placeholder values only

## Database Setup

1. Ensure SQL Server is running
2. Update the connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Optionally run the sample data SQL script

