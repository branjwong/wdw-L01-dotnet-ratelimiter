
using Microsoft.AspNetCore.Mvc;
using SimpleRateLimiter.Models;
using SimpleRateLimiter.Services;

namespace SimpleRateLimiter.Controllers
{
    [Route("api/[controller]")]
    public class TakeController(ILogger<TakeController> logger, IBucketManager bucketManager) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly IBucketManager _bucketManager = bucketManager;

        // POST: api/Take
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ObjectResult> PostEndpointBucket(TakeItem takeItem)
        {
            _logger.LogInformation("Received request to take token from endpoint {Endpoint}", takeItem.Endpoint);

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model state invalid for endpoint {Endpoint}", takeItem.Endpoint);
                return BadRequest(ModelState);
            }

            var endpointBucket = await _bucketManager.GetBucket(takeItem.Endpoint);

            if (endpointBucket == null)
            {
                _logger.LogInformation("Endpoint not found: {Endpoint}", takeItem.Endpoint);
                return StatusCode(400, new { message = "Endpoint not found in config" });
            }

            var tokens = endpointBucket.Tokens;
            if (tokens >= 1)
            {
                await _bucketManager.TakeFromBucket(takeItem.Endpoint);
                _logger.LogInformation("Token taken for endpoint {Endpoint}", takeItem.Endpoint);
                return StatusCode(200, new { message = "Token taken", tokensAvailable = endpointBucket.Tokens });
            }
            else
            {
                _logger.LogWarning("No tokens available for endpoint {Endpoint}", takeItem.Endpoint);
                return StatusCode(429, new { message = "Rate limit exceeded", tokensAvailable = endpointBucket.Tokens });
            }
        }
    }
}
