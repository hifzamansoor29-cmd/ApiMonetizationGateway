using ApiMonetizationGateway.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApiMonetizationGateway.Domain.Entities
{
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CustomerId { get; set; }
        public int TierId { get; set; }
        public Tier? Tier { get; set; }
    }
}
