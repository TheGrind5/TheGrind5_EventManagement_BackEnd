using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;
using TheGrind5_EventManagement.Extensions;
using TheGrind5_EventManagement.Constants;
using TheGrind5_EventManagement.Middleware;
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services using extension methods
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddGenericServices(); // Thêm Generic Services
builder.Services.AddCorsPolicy();

// Add Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add background services
builder.Services.AddHostedService<TheGrind5_EventManagement.Services.OrderCleanupService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? AppConstants.JWT_ISSUER,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? AppConstants.JWT_AUDIENCE,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? AppConstants.JWT_SECRET_KEY))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
// Use Exception Handler (must be early in pipeline)
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

// Only use HTTPS redirection in production or when HTTPS is configured
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Enable static files to serve uploaded files (phải đặt trước CORS)
app.UseStaticFiles();

// Enable static files for assets/images directory (resolve from ContentRootPath to avoid bin/ path issues)
var contentRoot = builder.Environment.ContentRootPath; // typically .../TheGrind5/src
var assetsPath = Path.Combine(contentRoot, "..", "assets", "images");
var absoluteAssetsPath = Path.GetFullPath(assetsPath);
Console.WriteLine($"[Static Files] ContentRoot: {contentRoot}");
Console.WriteLine($"[Static Files] Assets Path: {absoluteAssetsPath}");
Console.WriteLine($"[Static Files] Assets Directory Exists: {Directory.Exists(absoluteAssetsPath)}");

if (Directory.Exists(absoluteAssetsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(absoluteAssetsPath),
        RequestPath = "/assets/images",
        OnPrepareResponse = ctx =>
        {
            // Detect file type by content, not just extension
            var filePath = ctx.File.PhysicalPath;
            var isSvg = false;
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                var bytes = System.IO.File.ReadAllBytes(filePath);
                
                // Check if it's actually SVG by content
                if (bytes.Length > 0 && bytes.Length < 100000) // SVG files are usually small
                {
                    var header = System.Text.Encoding.ASCII.GetString(bytes.Take(Math.Min(100, bytes.Length)).ToArray());
                    if (header.TrimStart().StartsWith("<svg", StringComparison.OrdinalIgnoreCase) ||
                        header.TrimStart().StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                    {
                        // It's SVG, set correct content type with charset
                        ctx.Context.Response.ContentType = "image/svg+xml; charset=utf-8";
                        isSvg = true;
                        Console.WriteLine($"[Static Files] Detected SVG file: {ctx.File.Name}, setting content-type to image/svg+xml");
                    }
                }
                
                // For other files, let ASP.NET Core auto-detect from extension
            }
            
            // Set cache headers - more aggressive for SVG to prevent stale cache
            if (isSvg)
            {
                // For SVG files, use no-cache to force browser to always check for updates
                ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers.Pragma = "no-cache";
            }
            else
            {
                // For regular images, allow caching but with revalidation
                ctx.Context.Response.Headers.CacheControl = "public, max-age=3600, must-revalidate";
            }
        }
    });
    Console.WriteLine($"[Static Files] Configured to serve from: {absoluteAssetsPath} at /assets/images");
}
else
{
    Console.WriteLine($"[Static Files] WARNING: Assets directory not found at {absoluteAssetsPath}");
}

app.UseCors(AppConstants.CORS_POLICY_NAME);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Auto-apply migrations on startup to ensure database exists/updated
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EventDBContext>();
    var conn = db.Database.GetDbConnection();
    Console.WriteLine($"[DB] Provider: {conn.GetType().Name}");
    Console.WriteLine($"[DB] Connection: {conn.ConnectionString}");
    
    // Check if database was created from TheGrind5_Query.sql
    try
    {
        var hasUserTable = db.Database.ExecuteSqlRaw(@"
            SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'User'
        ") > 0;
        
        if (hasUserTable)
        {
            Console.WriteLine("[Migration] Database tables already exist from TheGrind5_Query.sql");
            Console.WriteLine("[Migration] Attempting to apply pending migrations...");
            
            // Try to apply migrations, but catch and ignore errors about existing objects
            try
            {
                db.Database.Migrate();
                Console.WriteLine("[Migration] Migrations applied successfully");
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // Ignore errors about existing objects (tables, indexes, etc.)
                if (sqlEx.Message.Contains("already an object named") || 
                    sqlEx.Message.Contains("There is already an object") ||
                    sqlEx.Number == 2714 || // SQL error: There is already an object named
                    sqlEx.Number == 1913 || // SQL error: Cannot create index because it already exists
                    sqlEx.Number == 1750)   // SQL error: Could not create constraint
                {
                    Console.WriteLine($"[Migration] Skipped existing objects - this is normal if database was created from SQL script");
                }
                else
                {
                    Console.WriteLine($"[Migration] SQL Error ({sqlEx.Number}): {sqlEx.Message.Split('\n').FirstOrDefault()}");
                }
            }
            catch (Exception migrateEx)
            {
                Console.WriteLine($"[Migration] Migration error (non-SQL): {migrateEx.Message}");
            }
        }
        else
        {
            // Fresh database, apply all migrations
            Console.WriteLine("[Migration] Fresh database detected, applying all migrations...");
            db.Database.Migrate();
            Console.WriteLine("[Migration] All migrations applied successfully");
        }
    }
    catch (Exception checkEx)
    {
        Console.WriteLine($"[Migration] Error checking database: {checkEx.Message}");
        // Try to apply migrations anyway
        try
        {
            db.Database.Migrate();
        }
        catch (Exception migrateEx2)
        {
            Console.WriteLine($"[Migration] Migration failed: {migrateEx2.Message}");
        }
    }
    
    try
    {
        var eventsCount = db.Events.Count();
        Console.WriteLine($"[DB] Events count = {eventsCount}");
    }
    catch (Exception countEx)
    {
        Console.WriteLine($"[DB] Count error: {countEx.Message}");
    }
}
catch (Exception migrateEx)
{
    Console.WriteLine($"[Migration] WARNING: {migrateEx.Message}");
    // Don't fail the app, just log the error
}

app.Run();
