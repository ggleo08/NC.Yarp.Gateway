using StackExchange.Redis;
using Yarp.Gateway.EntityFrameworkCore;

namespace Yarp.Gateway.RedisPubSub
{
    public class YarpConfigurationChangeSubscriber : BackgroundService
    {
        private readonly IYarpConfigurationStore _yarpStore;
        public YarpConfigurationChangeSubscriber(IYarpConfigurationStore yarpStore,
                                                 IConnectionMultiplexer connectionMultiplexer)
        {
            this._yarpStore = yarpStore;
            var subscriber = connectionMultiplexer.GetSubscriber();
            subscriber.Subscribe("YarpConfigChanged", (channel, value) => {
                _yarpStore.ReloadConfig();
            });
            _yarpStore.ConfigurationChange -= _yarpStore.ReloadConfig;
            _yarpStore.ConfigurationChange += () => subscriber.Publish("YarpConfigChanged", string.Empty);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
