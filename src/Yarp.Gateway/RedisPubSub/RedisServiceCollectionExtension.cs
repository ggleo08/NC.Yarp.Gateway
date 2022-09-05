using Dapr.Client;
using StackExchange.Redis;
using Yarp.Gateway.EntityFrameworkCore;

namespace Yarp.Gateway.RedisPubSub
{
    public static class RedisServiceCollectionExtension
    {
        public static IReverseProxyBuilder AddRedis(this IReverseProxyBuilder builder, string connetionString)
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connetionString));
            builder.Services.AddHostedService(sp =>
            {
                var daperClient = sp.GetRequiredService<DaprClient>();
                var logger = sp.GetRequiredService<ILogger<YarpConfigurationChangeSubscriber>>();
                return new YarpConfigurationChangeSubscriber(sp.GetRequiredService<IYarpConfigurationStore>(), ConnectionMultiplexer.Connect(connetionString), daperClient, logger);
            });
            return builder;
        }
    }
}
