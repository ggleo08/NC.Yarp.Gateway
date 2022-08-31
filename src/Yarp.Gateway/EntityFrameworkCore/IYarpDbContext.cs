using Microsoft.EntityFrameworkCore;
using Yarp.Gateway.Entities;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public interface IYarpDbContext
    {
        DbSet<YarpCluster> YarpClusters { get; set; }
        DbSet<YarpRoute> YarpRoutes { get; set; }
        DbSet<YarpDestination> YarpDestinations { get; set; }
        DbSet<YarpActiveHealthCheckOption> YarpActiveHealthCheckOptions { get; set; }
        DbSet<YarpHealthCheckOption> YarpHealthCheckOptions { get; set; }
        DbSet<YarpMetadata> YarpMetadatas { get; set; }
        DbSet<YarpPassiveHealthCheckOption> YarpPassiveHealthCheckOptions { get; set; }
        DbSet<YarpHttpClientOption> YarpProxyHttpClientOptions { get; set; }
        DbSet<YarpMatch> YarpMatches { get; set; }
        DbSet<YarpForwarderRequest> YarpRequestProxyOptions { get; set; }
        DbSet<YarpRouteHeader> YarpRouteHeaders { get; set; }
        DbSet<YarpSessionAffinityConfig> YarpSessionAffinityOptions { get; set; }
        DbSet<YarpSessionAffinityOptionSetting> YarpSessionAffinityOptionSettings { get; set; }
        DbSet<YarpTransform> YarpTransforms { get; set; }
    }
}
