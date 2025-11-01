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
                    // Disable retry strategy to avoid conflict with manual transactions
                    // sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                });
            });
            
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IEventQuestionRepository, EventQuestionRepository>();
            services.AddScoped<IAISuggestionRepository, AISuggestionRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileManagementService, FileManagementService>();
            // services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserMapper, UserMapper>();
            services.AddScoped<IEventMapper, EventMapper>();
            services.AddScoped<IOrderMapper, OrderMapper>();
            services.AddScoped<IWishlistMapper, WishlistMapper>();
            
            // AI Services
            services.AddHttpClient<IHuggingFaceService, HuggingFaceService>();
            services.AddScoped<IAIRecommendationService, AIRecommendationService>();
            services.AddScoped<IAIChatbotService, AIChatbotService>();
            services.AddScoped<IAIPricingService, AIPricingService>();
            services.AddScoped<IAIContentGenerationService, AIContentGenerationService>();
            
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
            services.AddScoped<ISampleDataExportService, SampleDataExportService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEventQuestionService, EventQuestionService>();
            
            // Admin service
            services.AddScoped<IAdminService, AdminService>();
            
            // VNPay service
            services.AddScoped<IVNPayService, VNPayService>();

            
            return services;
        }

        /// <summary>
        /// Register Generic Services để giảm code duplication
        /// Thay thế manual service registration
        /// TODO: Implement Generic Services when needed
        /// </summary>
        public static IServiceCollection AddGenericServices(this IServiceCollection services)
        {
            // TODO: Implement Generic Services
            // Commented out until Generic Services are implemented
            
            // Register Generic Services cho Event
            // services.AddScoped<IGenericService<Event, EventDTO, CreateEventRequest, UpdateEventRequest>, 
            //     GenericService<Event, EventDTO, CreateEventRequest, UpdateEventRequest>>();
            // services.AddScoped<IGenericMapper<Event, EventDTO, CreateEventRequest, UpdateEventRequest>, 
            //     GenericMapper<Event, EventDTO, CreateEventRequest, UpdateEventRequest>>();

            // Register Generic Services cho Order
            // services.AddScoped<IGenericService<Order, OrderDTO, CreateOrderRequestDTO, UpdateOrderRequestDTO>, 
            //     GenericService<Order, OrderDTO, CreateOrderRequestDTO, UpdateOrderRequestDTO>>();
            // services.AddScoped<IGenericMapper<Order, OrderDTO, CreateOrderRequestDTO, UpdateOrderRequestDTO>, 
            //     GenericMapper<Order, OrderDTO, CreateOrderRequestDTO, UpdateOrderRequestDTO>>();

            // Register Generic Services cho Ticket
            // services.AddScoped<IGenericService<Ticket, TicketDTO, CreateTicketRequestDTO, UpdateTicketRequestDTO>, 
            //     GenericService<Ticket, TicketDTO, CreateTicketRequestDTO, UpdateTicketRequestDTO>>();
            // services.AddScoped<IGenericMapper<Ticket, TicketDTO, CreateTicketRequestDTO, UpdateTicketRequestDTO>, 
            //     GenericMapper<Ticket, TicketDTO, CreateTicketRequestDTO, UpdateTicketRequestDTO>>();

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


