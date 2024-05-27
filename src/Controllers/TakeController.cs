using Microsoft.AspNetCore.Mvc;
using SimpleRateLimiter.Models;
using Newtonsoft.Json;

namespace SimpleRateLimiter.Controllers
{
    [Route("[controller]")]
    public class TakeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IList<EndpointConfig>? _config;

        private readonly IDictionary<string, decimal> _buckets = new Dictionary<string, decimal>();

        public TakeController(ILogger<TakeController> logger)
        {
            _logger = logger;
            _config = JsonConvert.DeserializeObject<IList<EndpointConfig>>(System.IO.File.ReadAllText(@"endpoint.config.json"));

            if (_config != null)
            {
                foreach (var endpoint in _config)
                {
                    _logger.LogInformation("Loaded config: {Endpoint}: burst={Burst}, sustained={Sustained}", endpoint.Endpoint, endpoint.Burst, endpoint.Sustained);

                    _buckets.Add(endpoint.Endpoint, endpoint.Burst);
                }
            }
        }

        [HttpPost]
        public ObjectResult Index(TakeItem takeItem)
        {
            _logger.LogInformation("Received request to take token from endpoint {Endpoint}", takeItem.Endpoint);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state invalid for endpoint {Endpoint}", takeItem.Endpoint);
                return BadRequest(ModelState);
            }

            if (!_buckets.ContainsKey(takeItem.Endpoint))
            {
                _logger.LogError("Endpoint not found in config: {Endpoint}", takeItem.Endpoint);
                return StatusCode(400, new { message = "Endpoint not found in config" });
            }

            var tokens = _buckets[takeItem.Endpoint];
            if (tokens >= 1)
            {
                _buckets[takeItem.Endpoint] = tokens - 1;
                _logger.LogError("Token taken for endpoint {Endpoint}", takeItem.Endpoint);
                return StatusCode(200, new { message = "Token taken", tokensAvailable = _buckets[takeItem.Endpoint] });
            }
            else
            {
                _logger.LogError("No tokens available for endpoint {Endpoint}", takeItem.Endpoint);
                return StatusCode(429, new { message = "Rate limit exceeded", tokensAvailable = 0 });
            }
        }
    }
}
