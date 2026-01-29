using System.ComponentModel.DataAnnotations;

namespace ApiMonetizationGateway.Domain.Entities
{
    public class ApiEndpoint
    {
        [Key]
        public int ApiId { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string Path { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
