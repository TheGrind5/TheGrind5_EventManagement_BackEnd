using TheGrind5_EventManagement.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TheGrind5_EventManagement.Services
{
    /// <summary>
    /// Background service để tự động cleanup expired orders
    /// Chạy mỗi 5 phút để kiểm tra và cancel orders hết hạn
    /// Sử dụng OrderService.CleanupExpiredOrdersAsync() để thực hiện cleanup
    /// </summary>
    public class OrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5); // Chạy mỗi 5 phút

        public OrderCleanupService(IServiceProvider serviceProvider, ILogger<OrderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCleanupService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    var cleanedCount = await orderService.CleanupExpiredOrdersAsync();
                    
                    if (cleanedCount > 0)
                    {
                        _logger.LogInformation("Cleaned up {Count} expired orders", cleanedCount);
                    }

                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired orders");
                    
                    // Nếu có lỗi, đợi 1 phút trước khi thử lại
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }

            _logger.LogInformation("OrderCleanupService stopped");
        }
    }
}
