using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService, IUsageTracker usageTracker, MonetizationDbContext dbContext)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

            if (path.Contains("/swagger") || path.Contains("/scalar") || path.Contains("/openapi"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key Missing");
                return;
            }

            var (allowed, customerId) = await rateLimitService.CheckAccessAsync(apiKey);

            if (!allowed)
            {
                context.Response.StatusCode = 429;
                context.Response.Headers.Append("Retry-After", "1");
                await context.Response.WriteAsync("Rate limit exceeded or monthly quota exceeded");
                return;
            }

            await _next(context);

            if (context.Response.StatusCode < 400)
            {
                var normalizedPath = path.Trim('/');

                var api = await dbContext.ApiEndpoints
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Path.ToLower().Trim('/') == normalizedPath);

                int apiId = api?.ApiId ?? 1;
                
                await usageTracker.LogRequestAsync(customerId, apiId);
            }
        }
    }
}