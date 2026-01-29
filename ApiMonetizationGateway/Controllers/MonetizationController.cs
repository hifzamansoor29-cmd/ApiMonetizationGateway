using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Domain.DTos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Controllers
{
    [ApiController]
    [Route("api/usage")]
    public class MonetizationController : ControllerBase
    {
        private readonly MonetizationDbContext _db;

        public MonetizationController(MonetizationDbContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<UsageSummaryDto>> GetMyUsage([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var customer = await _db.Customers
                .Include(c => c.Subscription)
                .ThenInclude(s => s.Tier)
                .FirstOrDefaultAsync(c => c.ApiKey == apiKey);

            if (customer == null) return Unauthorized(new { message = "Invalid API Key" });

            var now = DateTime.UtcNow;
            var currentMonthLabel = now.ToString("yyyy-MM");

            var summary = await _db.MonthlySummaries
                .FirstOrDefaultAsync(s => s.CustomerId == customer.CustomerId && s.Month == currentMonthLabel);

            int actualUsage = summary?.TotalRequests ?? await _db.UsageLogs.CountAsync(l =>
                l.CustomerId == customer.CustomerId &&
                l.Year == now.Year &&
                l.Month == now.Month);

            return Ok(new UsageSummaryDto
            {
                CustomerName = customer.Name,
                TierName = customer.Subscription?.Tier?.Name ?? "N/A",
                RequestsUsed = actualUsage,
                MonthlyQuota = customer.Subscription?.Tier?.MonthlyQuota ?? 0,
                AmountDue  = summary?.TotalAmount ?? 0,
                Currency = "USD"
            });
        }
    }
}