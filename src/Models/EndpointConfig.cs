using System.ComponentModel.DataAnnotations;

namespace SimpleRateLimiter.Models
{
    public class EndpointConfig
    {
        public required string Endpoint { get; set; }
        public int Burst { get; set; }
        public decimal Sustained { get; set; }
    }
}
