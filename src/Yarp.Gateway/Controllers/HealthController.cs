using Microsoft.AspNetCore.Mvc;

namespace Yarp.Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet("health")]
        public IActionResult Index()
        {
            return Ok("OK");
        }
    }
}
