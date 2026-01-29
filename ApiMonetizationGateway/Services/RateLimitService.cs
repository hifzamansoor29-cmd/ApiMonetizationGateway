using ApiMonetizationGateway.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ApiMonetizationGateway.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache;
        private readonly MonetizationDbContext _db;

        public RateLimitService(IMemoryCache cache, MonetizationDbContext db)
        {
            _cache = cache;
            _db = db;
        }

        public async Task<(bool IsAllowed, int CustomerId)> CheckAccessAsync(string apiKey)
        {
            var customer = await _db.Customers
                .Include(c => c.Subscription)
                .ThenInclude(s => s.Tier)
                .FirstOrDefaultAsync(c => c.ApiKey == apiKey);

            if (customer == null || customer.Subscription?.Tier == null)
                return (false, 0);

            var secondKey = $"RL_{customer.CustomerId}_{DateTime.UtcNow:ss}";
            var requestCount = _cache.GetOrCreate(secondKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                return 0;
            });

            if (requestCount >= customer.Subscription.Tier.RateLimit)
                return (false, customer.CustomerId);

            _cache.Set(secondKey, requestCount + 1);

            return (true, customer.CustomerId);
        }
    }
}