using UrlShortener.API.Services;

namespace UrlShortener.API.BackgroundServices
{
    public class InactivateExpiredUrlService : BackgroundService
    {
        private readonly ILogger<InactivateExpiredUrlService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        public InactivateExpiredUrlService(
            IServiceProvider serviceProvider,
            ILogger<InactivateExpiredUrlService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UrlExpirationBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var urlService = scope.ServiceProvider.GetRequiredService<IUrlShortenerService>();

                try
                {
                    await urlService.DeactivateExpiredUrlsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deactivate expired URLs.");
                }

                await Task.Delay(Interval, stoppingToken);
            }

            _logger.LogInformation("UrlExpirationBackgroundService stopped.");
        }
    }
}
