using Microsoft.EntityFrameworkCore;
using Yarp.Gateway.Entities;
using Yarp.Gateway.Extensions;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public static class YarpDbContextModelCreating
    {
        public static void ConfigYarpGateway(this ModelBuilder modelBuilder)
        {
            // Check.NotNull(builder, nameof(builder));
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }
            //BuildCluster(modelBuilder);
            //BuildDestination(modelBuilder);
            //BuildProxyHttpClientOptions(modelBuilder);
            //BuildSessionAffinityOptions(modelBuilder);
            //BuildHealthCheckOptions(modelBuilder);
            //BuildProxyRoute(modelBuilder);
            //BuildProxyMatch(modelBuilder);

            // BuildProxyMetadata(modelBuilder);

            // 初始化种子数据
            InitSeedingData(modelBuilder);
        }

        #region 配置 Entity
        private static void BuildCluster(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpCluster>(builder =>
            {
                builder.HasMany(p => p.Destinations)
                       .WithOne(p => p.Cluster)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(p => p.Metadata)
                       .WithOne()
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(p => p.SessionAffinity)
                       .WithOne(s => s.Cluster)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(p => p.HealthCheck)
                       .WithOne(s => s.Cluster)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(p => p.HttpClient)
                       .WithOne(s => s.Cluster)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(p => p.HttpRequest)
                       .WithOne(s => s.Cluster)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void BuildDestination(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpDestination>(builder =>
            {
                builder.HasMany(p => p.Metadata)
                       .WithOne()
                       .OnDelete(DeleteBehavior.NoAction);
            });
        }

        private static void BuildSessionAffinityOptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpSessionAffinityConfig>(builder =>
            {
                builder.HasOne(p => p.Cookie)
                       .WithOne(c => c.SessionAffinityConfig)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void BuildProxyHttpClientOptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpHttpClientOption>(builder =>
            {
                builder.HasOne(h => h.WebProxy)
                       .WithOne(w => w.HttpClientOption)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void BuildHealthCheckOptions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpHealthCheckOption>(builder =>
            {
                builder.HasOne(p => p.Passive)
                       .WithOne(s => s.HealthCheckOption)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(p => p.Active)
                       .WithOne(s => s.HealthCheckOption)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void BuildProxyRoute(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpRoute>(builder =>
            {
                builder.HasMany(p => p.Metadata)
                       .WithOne()
                       .OnDelete(DeleteBehavior.Cascade).IsRequired(false);
                builder.HasMany(p => p.Transforms)
                       .WithOne()
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(p => p.Match)
                       .WithOne(m => m.Route)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void BuildProxyMatch(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpMatch>(builder =>
            {
                builder.HasMany(p => p.Headers)
                       .WithOne(h => h.Match)
                       .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(p => p.QueryParameters)
                       .WithOne(q => q.Match)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void BuildProxyMetadata(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YarpMetadata>(builder =>
            {
                builder.HasOne(p => p.Route).WithMany().OnDelete(DeleteBehavior.NoAction).HasForeignKey(p => p.RouteId).IsRequired(false);
                builder.HasOne(p => p.Cluster).WithMany().OnDelete(DeleteBehavior.NoAction).HasForeignKey(p => p.ClusterId).IsRequired(false);
                builder.HasOne(p => p.Destination).WithMany().OnDelete(DeleteBehavior.NoAction).HasForeignKey(p => p.DestinationId).IsRequired(false);
            });
        }
        #endregion

        #region  种子数据
        public static void InitSeedingData(ModelBuilder modelBuilder)
        {
            var clusters = new List<YarpCluster>()
            {
                //new YarpCluster(){ Id = Guid.NewGuid(), ClusterId = "cluster1" },
                //new YarpCluster(){ Id = Guid.NewGuid(), ClusterId = "cluster2" },
                new YarpCluster(){ Id = Guid.NewGuid(), ClusterId = "dapr-sidercar" }
            };

            var destinations = new List<YarpDestination>()
            {
                //new YarpDestination(){ Id = Guid.NewGuid(), Name="Cluster1/Destination1", ClusterId = clusters[0].Id , Address = "http://localhost:5251" },
                //new YarpDestination(){ Id = Guid.NewGuid(), Name="Cluster2/Destination2", ClusterId = clusters[1].Id , Address = "http://localhost:5252" }
                new YarpDestination(){ Id = Guid.NewGuid(), Name="dapr-sidercar/destination1", ClusterId = clusters[0].Id , Address = DaprDefaults.GetDefaultHttpEndpoint() }
            };

            var routes = new List<YarpRoute>()
            {
                new YarpRoute(){ Id = Guid.NewGuid(), RouteId = "first-service", ClusterId = clusters[0].Id, AuthorizationPolicy = "Default" },
                new YarpRoute(){ Id = Guid.NewGuid(), RouteId = "second-service", ClusterId = clusters[0].Id, /*AuthorizationPolicy = "Default"*/ },
            };

            var matches = new List<YarpMatch>()
            {
                new YarpMatch(){ Id = Guid.NewGuid(), RouteId = routes[0].Id, Path = "/api/first/{**catch-all}" },
                new YarpMatch(){ Id = Guid.NewGuid(), RouteId = routes[1].Id, Path = "/api/second/{**catch-all}" },
            };

            //var transformType = EnumTransformType.PathPattern;
            //var transforms = new List<YarpTransform>()
            //{
            //    new YarpTransform(){ Id = Guid.NewGuid(), RouteId = routes[0].Id, Type = transformType, Key = transformType.ToString(), Value = "/api/{**catch-all}" },
            //    new YarpTransform(){ Id = Guid.NewGuid(), RouteId = routes[1].Id, Type = transformType, Key = transformType.ToString(), Value = "/api/{**catch-all}" },
            //};

            var metadatas = new List<YarpMetadata>() {
                new YarpMetadata(){ Id = Guid.NewGuid(), RouteId = routes[0].Id, Key = DaprYarpConstants.MetaKeys.Dapr, Value = DaprYarpConstants.DaprAct.Method },
                new YarpMetadata(){ Id = Guid.NewGuid(), RouteId = routes[1].Id, Key = DaprYarpConstants.MetaKeys.Dapr, Value = DaprYarpConstants.DaprAct.Method },
            };

            modelBuilder.Entity<YarpCluster>().HasData(clusters);
            modelBuilder.Entity<YarpDestination>().HasData(destinations);
            modelBuilder.Entity<YarpRoute>().HasData(routes);
            modelBuilder.Entity<YarpMatch>().HasData(matches);
            //modelBuilder.Entity<YarpTransform>().HasData(transforms);
            modelBuilder.Entity<YarpMetadata>().HasData(metadatas);
        }
        #endregion
    }
}
