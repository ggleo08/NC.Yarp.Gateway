using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Yarp.Gateway.EntityFrameworkCore;
using Yarp.Gateway.Services;

namespace Yarp.Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {

        private readonly DaprClient _daprClient;
        private readonly ILogger<ConfigController> _logger;

        public TestController(DaprClient daprClient,
                              ILogger<ConfigController> logger)
        {
            this._daprClient = daprClient;
            this._logger = logger;
        }

        [HttpGet]
        [Route("GetFirst")]
        public async Task<IActionResult> First()
        {
            try
            {
                var result = await _daprClient.InvokeMethodAsync<List<WeatherForecast>>("first", "api/WeatherForecast");
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetSecond")]
        public async Task<IActionResult> Second()
        {
            var result = await _daprClient.InvokeMethodAsync<object>("second", "api/WeatherForecast");
            return Ok(result);
        }
    }
}
