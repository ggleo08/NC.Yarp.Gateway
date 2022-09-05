using Dapr.Client;
using StackExchange.Redis;
using Yarp.Gateway.EntityFrameworkCore;

namespace Yarp.Gateway.RedisPubSub
{
    public class YarpConfigurationChangeSubscriber : BackgroundService
    {
        private readonly IYarpConfigurationStore _yarpStore;
        private readonly DaprClient _daprClient;
        private readonly ILogger<YarpConfigurationChangeSubscriber> _logger;

        public YarpConfigurationChangeSubscriber(IYarpConfigurationStore yarpStore,
                                                 IConnectionMultiplexer connectionMultiplexer,
                                                 DaprClient daprClient,
                                                 ILogger<YarpConfigurationChangeSubscriber> logger)
        {
            this._yarpStore = yarpStore;
            this._daprClient = daprClient;
            this._logger = logger;

            //var subscriber = connectionMultiplexer.GetSubscriber();
            //subscriber.Subscribe("YarpConfigChanged", (channel, value) =>
            //{
            //    _yarpStore.ReloadConfig();
            //    _logger.LogInformation("YarpConfigChanged Subscribe...");
            //});

            _yarpStore.ConfigurationChange -= async () =>
            {
                await _yarpStore.ReloadConfigAsync();
            };
            _yarpStore.ConfigurationChange += async () =>
            {
                // subscriber.Publish("YarpConfigChanged", string.Empty);
                await _daprClient.PublishEventAsync("pubsub", "YarpConfigChanged");

                _logger.LogInformation("YarpConfigChanged event publish...");
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
