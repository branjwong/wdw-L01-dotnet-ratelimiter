using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SimpleRateLimiter.Models
{
    public class EndpointBucket
    {
        [Key]
        public required string Endpoint { get; set; }
        public decimal Tokens { get; set; }
    }
}
