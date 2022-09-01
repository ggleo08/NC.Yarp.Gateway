using Yarp.Gateway.EntityFrameworkCore;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.Extensions
{
    public static class IReverseProxyBuilderExtensions
    {
        public static IReverseProxyBuilder AddEntityFrameworkProvider(this IReverseProxyBuilder builder)
        {
            builder.Services.AddSingleton<IProxyConfigProvider>(provider =>
            {
                return new YarpEntityFrameworkProvider(provider.GetService<ILogger<YarpEntityFrameworkProvider>>(), provider.GetRequiredService<IYarpConfigurationStore>());
            });
            return builder;
        }

        public static IReverseProxyBuilder LoadFromEntityFramework(this IReverseProxyBuilder builder)
        {
            builder.Services.AddSingleton<IYarpConfigurationStore, YarpConfigurationStore>();
            builder.AddEntityFrameworkProvider();
            return builder;
        }

        public static IConfigurationBuilder AddDaprConfig(this IConfigurationBuilder configurationBuilder)
        {
            var httpEndpoint = DaprDefaults.GetDefaultHttpEndpoint(); //参考Dapr.Client，获取到dapr-sidecar的url
            return configurationBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Yarp:Clusters:dapr-sidecar:Destinations:d1:Address", httpEndpoint),
            });
        }
    }
}
