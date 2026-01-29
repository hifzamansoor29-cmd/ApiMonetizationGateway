using ApiMonetizationGateway.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApiMonetizationGateway.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ApiKey { get; set; } = Guid.NewGuid().ToString();
        public Subscription? Subscription { get; set; }
    }
}
