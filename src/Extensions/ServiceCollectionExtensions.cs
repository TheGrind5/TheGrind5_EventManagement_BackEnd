using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Services;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Constants;

namespace TheGrind5_EventManagement.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EventDBContext>(options =>
            {
                var conn = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(conn))
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                
                options.UseSqlServer(conn, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                });
            });
            
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEmailService, EmailService>();
            // services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserMapper, UserMapper>();
            services.AddScoped<IEventMapper, EventMapper>();
            services.AddScoped<IOrderMapper, OrderMapper>();
            services.AddScoped<IWishlistMapper, WishlistMapper>();
            
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWalletService, WalletService>();

            services.AddScoped<IWishlistService, WishlistService>();

            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IVoucherService, VoucherService>();

            
            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AppConstants.CORS_POLICY_NAME, policy =>
                {
                    policy.WithOrigins(
                            AppConstants.CORS_FRONTEND_URL,
                            AppConstants.CORS_FRONTEND_URL_ALT,
                            AppConstants.CORS_FRONTEND_URL_HTTPS,
                            AppConstants.CORS_FRONTEND_URL_HTTP)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            
            return services;
        }
    }
}


