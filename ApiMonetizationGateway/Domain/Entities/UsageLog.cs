using System.ComponentModel.DataAnnotations;

namespace ApiMonetizationGateway.Domain.Entities
{
    public class UsageLog
    {
        [Key]
        public int LogId { get; set; }
        public int CustomerId { get; set; }
        public int ApiId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Year { get; set; } = DateTime.UtcNow.Year;
        public int Month { get; set; } = DateTime.UtcNow.Month;
        public ApiEndpoint? ApiEndpoint { get; set; }
    }
}
