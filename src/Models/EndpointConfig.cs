namespace SimpleRateLimiter.Models
{
    public class EndpointConfig
    {
        public string? Endpoint { get; set; }
        public int Burst { get; set; }
        public int Sustained { get; set; }
    }
}
