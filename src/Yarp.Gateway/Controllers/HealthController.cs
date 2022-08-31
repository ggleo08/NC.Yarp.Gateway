using Microsoft.AspNetCore.Mvc;

namespace Yarp.Gateway.Controllers
{
    [ApiController]
    public class HealthController : Controller
    {
        [HttpGet]
        [Route("/api/health")]
        public IActionResult Index()
        {
            return Ok("OK");
        }
    }
}
