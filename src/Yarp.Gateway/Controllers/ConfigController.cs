using Dapr;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Yarp.Gateway.EntityFrameworkCore;
using Yarp.Gateway.Services;

namespace Yarp.Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        private readonly IYarpConfigurationStore _yarpStore;
        private readonly IYarpRouteAppService _yarpRouteAppService;
        private readonly ILogger<ConfigController> _logger;

        public ConfigController(IYarpConfigurationStore yarpStore,
                                IYarpRouteAppService yarpRouteAppService,
                                ILogger<ConfigController> logger)
        {
            this._yarpStore = yarpStore;
            this._yarpRouteAppService = yarpRouteAppService;
            this._logger = logger;
        }

        [HttpGet("reload")]
        public async Task<IActionResult> Reload()
        {
            await Task.Factory.StartNew(() => _yarpStore.Reload());
            return Ok("OK");
        }

        [HttpPut("updateRouteId")]
        public async Task<IActionResult> UpdateRouteId(Guid id, string routeId)
        {
            await _yarpRouteAppService.UpdateRouteName(id, routeId);
            return Ok();
        }

        [HttpPost("YarpConfigChangedSubscribe")]
        [Topic("pubsub", "YarpConfigChanged")]
        public async Task<IActionResult> ConfigChangedSubscribe()
        {
            // var stream = Request.Body;
            // var buffer = new byte[Request.ContentLength.Value];
            // stream.Position = 0L;
            // stream.ReadAsync(buffer, 0, buffer.Length);
            // var content = Encoding.UTF8.GetString(buffer);
            await Task.Run(() => _yarpStore.ReloadConfig());
            _logger.LogInformation("YarpConfigChanged event Subscribe...");
            return Ok("ok");
        }
    }
}
