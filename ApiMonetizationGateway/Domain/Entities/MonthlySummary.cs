using ApiMonetizationGateway.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApiMonetizationGateway.Domain.Entities
{
    public class MonthlySummary
    {
        [Key]
        public int SummaryId { get; set; }
        public int CustomerId { get; set; }
        public string Month { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int BillableRequests { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal OverageAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Customer? Customer { get; set; }
    }
}
