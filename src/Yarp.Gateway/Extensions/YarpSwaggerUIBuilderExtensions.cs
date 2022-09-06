using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.Extensions
{
    public static class YarpSwaggerUIBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerUIWithYarp(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                // 获取 Yarp 的 Routes 配置
                var proxyConfigProvider = serviceProvider.GetRequiredService<IProxyConfigProvider>();
                var yarpConfig = proxyConfigProvider.GetConfig();

                var clusters = yarpConfig.Clusters.SelectMany(t => t.Destinations, (clusterId, destination) => new { clusterId.ClusterId, destination.Value });

                foreach (var cluster in clusters)
                {
                    // Dapr sidercar swagger.json 地址配置
                    var serviceName = cluster.ClusterId == "dapr-sidercar" ? "gateway" : cluster.ClusterId;
                    options.SwaggerEndpoint($"{cluster.Value.Address}/v1.0/invoke/{serviceName}/method/swagger/v1/swagger.json", $"{serviceName} API");
                }


                // 添加当前网关项目 API
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway v1.0");
                //if (options.ConfigObject.Urls == null)
                //{
                //    var hostingEnv = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                //    options.ConfigObject.Urls = new[] { new UrlDescriptor { Name = $"{hostingEnv.ApplicationName} v1", Url = "v1/swagger.json" } };
                //}

                #region Yarp 各服务 swagger.json 地址配置
                //// 获取 Yarp 的 Routes 配置
                //var proxyConfigProvider = serviceProvider.GetRequiredService<IProxyConfigProvider>();
                //var yarpConfig = proxyConfigProvider.GetConfig();

                //var routedClusters = yarpConfig.Clusters.SelectMany(t => t.Destinations, (clusterId, destination) => new { clusterId.ClusterId, destination.Value });

                //var groupedClusters = routedClusters.GroupBy(q => q.Value.Address)
                //                                    .Select(t => t.First())
                //                                    .Distinct()
                //                                    .ToList();

                //foreach (var clusterGroup in groupedClusters)
                //{
                //    var routeConfig = yarpConfig.Routes.FirstOrDefault(q => q.ClusterId == clusterGroup.ClusterId);
                //    if (routeConfig == null)
                //    {
                //        logger.LogWarning($"Swagger UI: Couldn't find route configuration for {clusterGroup.ClusterId}...");
                //        continue;
                //    }

                //    // 添加Swagger终点，根据 Routes 配置拼接出内部服务的 Swagger 地址
                //    options.SwaggerEndpoint($"{clusterGroup.Value.Address}/swagger/v1/swagger.json", $"{routeConfig.RouteId} API");
                //    ////options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                //    ////options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                //}
                #endregion
            });

            return app;
        }
    }
}
