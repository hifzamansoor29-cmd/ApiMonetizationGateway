namespace ApiMonetizationGateway.Domain.DTos
{
    public class UsageSummaryDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string TierName { get; set; } = string.Empty;
        public int RequestsUsed { get; set; }
        public int MonthlyQuota { get; set; }
        public double RemainingPercentage => MonthlyQuota > 0
            ? Math.Max(0, (double)(MonthlyQuota - RequestsUsed) / MonthlyQuota * 100)
            : 0;
        public decimal AmountDue { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
