using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;
using TheGrind5_EventManagement.Extensions;
using TheGrind5_EventManagement.Constants;
using TheGrind5_EventManagement.Middleware;

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

app.Run();
