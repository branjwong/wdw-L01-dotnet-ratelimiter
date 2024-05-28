using SimpleRateLimiter.Models;

namespace SimpleRateLimiter.Services
{
    public interface IBucketManager
    {
        Task<EndpointBucket?> GetBucket(string endpoint);
        Task TakeFromBucket(string endpoint);
        Task RefillBucket(string endpoint, decimal amount, int maximum);
    }

    public class BucketManager(BucketContext context) : IBucketManager
    {
        // Todo: Add Lock
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock
        private readonly BucketContext _context = context;

        public async Task<EndpointBucket?> GetBucket(string endpoint)
        {
            return await _context.EndpointBuckets.FindAsync(endpoint);
        }

        public async Task TakeFromBucket(string endpoint)
        {
            var bucket = await _context.EndpointBuckets.FindAsync(endpoint) ?? throw new InvalidOperationException($"Bucket not found for endpoint {endpoint}");
            bucket.Tokens--;
            await _context.SaveChangesAsync();
        }

        public async Task RefillBucket(string endpoint, decimal amount, int maximum)
        {
            var bucket = await _context.EndpointBuckets.FindAsync(endpoint) ?? throw new InvalidOperationException($"Bucket not found for endpoint {endpoint}");
            bucket.Tokens = Math.Min(bucket.Tokens + amount, maximum);
            await _context.SaveChangesAsync();
        }
    }
}
