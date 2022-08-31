using Microsoft.AspNetCore.Mvc;
using Yarp.Gateway.EntityFrameworkCore;
using Yarp.Gateway.Services;

namespace Yarp.Gateway.Controllers
{
    [ApiController]
    public class ConfigController : Controller
    {
        private readonly IYarpConfigurationStore _yarpStore;
        private readonly IYarpRouteAppService  _yarpRouteAppService;
        public ConfigController(IYarpConfigurationStore yarpStore,
                                IYarpRouteAppService yarpRouteAppService)
        {
            this._yarpStore = yarpStore;
            this._yarpRouteAppService = yarpRouteAppService;
        }

        [HttpGet]
        [Route("/api/config/reload")]
        public async Task<IActionResult> Reload()
        {
            await Task.Factory.StartNew(() => _yarpStore.Reload());
            return Ok("OK");
        }


        [HttpPut]
        [Route("/api/config/updateRouteId")]
        public async Task<IActionResult> UpdateRouteId(Guid id, string routeId)
        {
            await _yarpRouteAppService.UpdateRouteName(id, routeId);
            return Ok();
        }
    }
}
