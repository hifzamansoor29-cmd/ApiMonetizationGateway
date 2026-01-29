using ApiMonetizationGateway.Data;

namespace ApiMonetizationGateway.Services
{
    public class UsageProcessingService : BackgroundService
    {
        private readonly IUsageTracker _tracker;
        private readonly IServiceProvider _serviceProvider;

        public UsageProcessingService(IUsageTracker tracker, IServiceProvider serviceProvider)
        {
            _tracker = tracker;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var reader = ((UsageTracker)_tracker).Reader;

            await foreach (var log in reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MonetizationDbContext>();

                db.UsageLogs.Add(log);
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
