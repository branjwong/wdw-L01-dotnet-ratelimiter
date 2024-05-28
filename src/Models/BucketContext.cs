using Microsoft.EntityFrameworkCore;

namespace SimpleRateLimiter.Models;

public class BucketContext : DbContext
{
    public BucketContext(DbContextOptions<BucketContext> options)
        : base(options)
    {
    }

    public DbSet<EndpointBucket> EndpointBuckets { get; set; } = null!;
}
