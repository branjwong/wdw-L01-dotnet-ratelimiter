using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleRateLimiter.Models;

namespace SimpleRateLimiter.Controllers
{
    [Route("[controller]")]
    public class TakeController : ControllerBase
    {
        private readonly ILogger _logger;

        public TakeController(ILogger<TakeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ObjectResult> Index(TakeItem takeItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Received request to take token from endpoint {Endpoint}", takeItem.Endpoint);
            return StatusCode(200, "hi");
        }
    }
}
