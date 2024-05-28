
using Microsoft.AspNetCore.Mvc;
using SimpleRateLimiter.Models;

namespace SimpleRateLimiter.Controllers
{
    [Route("api/[controller]")]
    public class TakeController : ControllerBase
    {
        private readonly ILogger _logger;

        // TODO: Refactor to use TokenManager.cs
        private readonly BucketContext _context;

        public TakeController(ILogger<TakeController> logger, BucketContext context)
        {
            _logger = logger;
            _context = context;
        }

        // POST: api/Take
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ObjectResult> PostEndpointBucket(TakeItem takeItem)
        {
            _logger.LogInformation("Received request to take token from endpoint {Endpoint}", takeItem.Endpoint);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state invalid for endpoint {Endpoint}", takeItem.Endpoint);
                return BadRequest(ModelState);
            }

            var endpointBucket = await _context.EndpointBuckets.FindAsync(takeItem.Endpoint);

            if (endpointBucket == null)
            {
                _logger.LogError("Endpoint not found: {Endpoint}", takeItem.Endpoint);
                return StatusCode(400, new { message = "Endpoint not found in config" });
            }

            var tokens = endpointBucket.Tokens;
            if (tokens >= 1)
            {
                endpointBucket.Tokens = tokens - 1;
                await _context.SaveChangesAsync();
                _logger.LogError("Token taken for endpoint {Endpoint}", takeItem.Endpoint);
                return StatusCode(200, new { message = "Token taken", tokensAvailable = endpointBucket.Tokens });
            }
            else
            {
                _logger.LogError("No tokens available for endpoint {Endpoint}", takeItem.Endpoint);
                return StatusCode(429, new { message = "Rate limit exceeded", tokensAvailable = 0 });
            }
        }
    }
}
