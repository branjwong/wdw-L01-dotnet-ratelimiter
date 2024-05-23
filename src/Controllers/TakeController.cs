using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleRateLimiter.Models;

namespace SimpleRateLimiter.Controllers
{
    [Route("[controller]")]
    public class TakeController : ControllerBase
    {
        private readonly ILogger _logger;
        // private readonly IList<EndpointConfig> _config;

        public TakeController(ILogger<TakeController> logger)
        {
            _logger = logger;
            // _config = JsonConvert.DeserializeObject<IList<EndpointConfig>>(File.ReadAllText(@"config.json"));

            // _logger.LogInformation("Loaded config: {Config}", _config);
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
