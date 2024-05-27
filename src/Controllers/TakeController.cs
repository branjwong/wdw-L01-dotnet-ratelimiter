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

        public TakeController(ILogger<TakeController> logger)
        {
            _logger = logger;
            _config = JsonConvert.DeserializeObject<IList<EndpointConfig>>(System.IO.File.ReadAllText(@"endpoint.config.json"));

            if (_config != null)
            {
                foreach (var endpoint in _config)
                {
                    _logger.LogInformation("Loaded config: {Endpoint}: burst={Burst}, sustained={Sustained}", endpoint.Endpoint, endpoint.Burst, endpoint.Sustained);
                }
            }
        }

        [HttpPost]
        public ObjectResult Index(TakeItem takeItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Received request to take token from endpoint {Endpoint}", takeItem.Endpoint);
            return StatusCode(200, new { message = "hi" });
        }
    }
}
