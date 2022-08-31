using StackExchange.Redis;
using Yarp.Gateway.EntityFrameworkCore;

namespace Yarp.Gateway.RedisPubSub
{
    public static class RedisServiceCollectionExtension
    {
        public static IReverseProxyBuilder AddRedis(this IReverseProxyBuilder builder, string connetionString)
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connetionString));
            builder.Services.AddHostedService(sp => new YarpConfigurationChangeSubscriber(sp.GetRequiredService<IYarpConfigurationStore>(), ConnectionMultiplexer.Connect(connetionString)));
            return builder;
        }
    }
}
