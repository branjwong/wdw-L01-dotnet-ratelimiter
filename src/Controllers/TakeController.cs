using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimpleRateLimiter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TakeController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> Index()
        {
            return "hi";
        }
    }
}
