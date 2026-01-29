namespace ApiMonetizationGateway.Services
{
    public interface IRateLimitService
    {
        Task<(bool IsAllowed, int CustomerId)> CheckAccessAsync(string apiKey);
    }
}