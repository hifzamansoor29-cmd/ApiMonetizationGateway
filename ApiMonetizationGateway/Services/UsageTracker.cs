using ApiMonetizationGateway.Domain.Entities;
using System.Threading.Channels;

namespace ApiMonetizationGateway.Services
{
    public class UsageTracker : IUsageTracker
    {
        private readonly Channel<UsageLog> _channel;
        private readonly ILogger<UsageTracker> _logger;

        public UsageTracker(ILogger<UsageTracker> logger)
        {
            _logger = logger;

            var options = new BoundedChannelOptions(5000)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            };

            _channel = Channel.CreateBounded<UsageLog>(options);
        }

        // Removed userId to match your Entity
        public async ValueTask LogRequestAsync(int customerId, int apiId)
        {
            var now = DateTime.UtcNow;

            var log = new UsageLog
            {
                CustomerId = customerId,
                ApiId = apiId,
                Timestamp = now,
                Year = now.Year,
                Month = now.Month
            };

            if (!_channel.Writer.TryWrite(log))
            {
                await _channel.Writer.WriteAsync(log);
            }
        }

        public ChannelReader<UsageLog> Reader => _channel.Reader;
    }
}