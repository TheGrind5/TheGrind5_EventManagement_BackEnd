using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

// Enable static files for assets/images directory (sample images committed to git)
var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "assets", "images");
if (Directory.Exists(assetsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(assetsPath),
        RequestPath = "/assets/images"
    });
}

app.UseCors(AppConstants.CORS_POLICY_NAME);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
