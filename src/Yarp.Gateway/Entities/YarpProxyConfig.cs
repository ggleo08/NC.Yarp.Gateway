using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.Entities
{
    public class YarpProxyConfig : IProxyConfig
    {
        public List<RouteConfig> Routes { get; set; } = new List<RouteConfig>();

        public List<ClusterConfig> Clusters { get; set; } = new List<ClusterConfig>();

        public IChangeToken ChangeToken { get; set; }

        IReadOnlyList<RouteConfig> IProxyConfig.Routes => Routes;

        IReadOnlyList<ClusterConfig> IProxyConfig.Clusters => Clusters;
    }
}
