using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Helpers
{
    // Kế thừa BackgroundService của .NET
    public class ExpiredBookingHelper : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredBookingHelper> _logger;

        public ExpiredBookingHelper(IServiceProvider serviceProvider, ILogger<ExpiredBookingHelper> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🤖 Bot Dọn dẹp Hóa đơn quá hạn đã khởi động!");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                        await bookingService.CancelExpiredBookingsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Lỗi khi chạy luồng dọn dẹp vé.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}