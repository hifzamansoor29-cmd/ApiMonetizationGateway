namespace ApiMonetizationGateway.Services
{
    public interface IUsageTracker
    {
        ValueTask LogRequestAsync(int customerId, int apiId);
    }
}
