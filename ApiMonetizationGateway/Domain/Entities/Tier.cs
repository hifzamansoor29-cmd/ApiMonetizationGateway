using ApiMonetizationGateway.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApiMonetizationGateway.Domain.Entities
{
    public class Tier
    {
        [Key]
        public int TierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MonthlyQuota { get; set; }
        public double RateLimit { get; set; }
        public decimal Price { get; set; }
        public decimal OveragePricePerRequest { get; set; }
    }
}
