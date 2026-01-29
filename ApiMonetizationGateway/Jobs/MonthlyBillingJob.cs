using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Jobs
{
    public class MonthlyBillingJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MonthlyBillingJob> _logger;

        public MonthlyBillingJob(IServiceProvider serviceProvider, ILogger<MonthlyBillingJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monthly Billing Job is initialized.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MonetizationDbContext>();

                    await GenerateMonthlySummaries(db);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during monthly billing summarization.");
                }
            }
        }

        private async Task GenerateMonthlySummaries(MonetizationDbContext db)
        {
            _logger.LogInformation("Starting Monthly Billing Summarization...");

            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var month = lastMonth.Month;
            var year = lastMonth.Year;
            var monthKey = $"{year}-{month:D2}";

            var customers = await db.Customers
                .Include(c => c.Subscription)
                .ThenInclude(s => s.Tier)
                .ToListAsync();

            foreach (var customer in customers)
            {
                var exists = await db.MonthlySummaries
                    .AnyAsync(s => s.CustomerId == customer.CustomerId && s.Month == monthKey);

                if (exists) continue;

                var tier = customer.Subscription.Tier;

                var totalRequests = await db.UsageLogs.CountAsync(l =>
                    l.CustomerId == customer.CustomerId &&
                    l.Timestamp.Month == month &&
                    l.Timestamp.Year == year);

                int billableRequests = Math.Max(0, totalRequests - tier.MonthlyQuota);

                decimal baseAmount = tier.Price;
                decimal overageAmount = billableRequests * tier.OveragePricePerRequest;
                decimal totalAmount = baseAmount + overageAmount;

                var summary = new MonthlySummary
                {
                    CustomerId = customer.CustomerId,
                    Month = monthKey,
                    TotalRequests = totalRequests,
                    BillableRequests = billableRequests,
                    BaseAmount = baseAmount,
                    OverageAmount = overageAmount,
                    TotalAmount = totalAmount,
                    GeneratedAt = DateTime.UtcNow
                };

                db.MonthlySummaries.Add(summary);
            }

            await db.SaveChangesAsync();
            _logger.LogInformation("Monthly Billing Summarization completed successfully.");
        }

    }
}