using Microsoft.EntityFrameworkCore;
using Yarp.Gateway.Entities;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public class YarpDbContext : DbContext, IYarpDbContext
    {
        public YarpDbContext(DbContextOptions<YarpDbContext> options)
                : base(options)
        {

        }

        public DbSet<YarpCluster> YarpClusters { get; set; }
        public DbSet<YarpRoute> YarpRoutes { get; set; }
        public DbSet<YarpDestination> YarpDestinations { get; set; }
        public DbSet<YarpActiveHealthCheckOption> YarpActiveHealthCheckOptions { get; set; }
        public DbSet<YarpHealthCheckOption> YarpHealthCheckOptions { get; set; }
        public DbSet<YarpMetadata> YarpMetadatas { get; set; }
        public DbSet<YarpPassiveHealthCheckOption> YarpPassiveHealthCheckOptions { get; set; }
        public DbSet<YarpHttpClientOption> YarpProxyHttpClientOptions { get; set; }
        public DbSet<YarpMatch> YarpMatches { get; set; }
        public DbSet<YarpForwarderRequest> YarpRequestProxyOptions { get; set; }// RequestProxyOptions { get; set; }
        public DbSet<YarpRouteHeader> YarpRouteHeaders { get; set; }
        public DbSet<YarpSessionAffinityConfig> YarpSessionAffinityOptions { get; set; }
        public DbSet<YarpSessionAffinityOptionSetting> YarpSessionAffinityOptionSettings { get; set; }
        public DbSet<YarpTransform> YarpTransforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigYarpGateway();
        }

    }
}
