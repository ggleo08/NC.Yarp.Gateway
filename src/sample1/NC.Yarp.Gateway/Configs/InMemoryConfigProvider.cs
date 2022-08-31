using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using Yarp.ReverseProxy.Configuration;

namespace NC.Yarp.Gateway
{

    public static class InMemoryConfigProviderExtension
    {
        public static IReverseProxyBuilder LoadFromMemory(this IReverseProxyBuilder builder, IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            builder.Services.AddSingleton<IProxyConfigProvider>(new InMemoryConfigProvider(routes, clusters));
            return builder;
        }
    }

    /// <summary>
    /// 提供IProxyConfigProvider的实现，以支持由代码生成的配置。  
    /// </summary>
    public class InMemoryConfigProvider : IProxyConfigProvider
    {
        // 标记为volatile，以便更新是原子的
        private volatile InMemoryConfig _config;

        public InMemoryConfigProvider(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            _config = new InMemoryConfig(routes, clusters);
        }

        /// <summary>
        /// 实现IProxyConfigProvider.GetConfig方法，提供当前配置的快照  
        /// </summary>
        /// <returns></returns>
        public IProxyConfig GetConfig() => _config;

        /// <summary>
        /// 使用配置的新快照交换配置状态，然后发出更改信号
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="clusters"></param>
        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            var oldConfig = _config;
            _config = new InMemoryConfig(routes, clusters);
            oldConfig.SignalChange();
        }
    }

    /// <summary>
    /// IProxyConfig的实现，它是当前配置状态的快照。这个类的数据应不可变。
    /// </summary>
    public class InMemoryConfig : IProxyConfig
    {

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        /// <summary>
        /// 代理路由列表的快照  
        /// </summary>
        public IReadOnlyList<RouteConfig> Routes { get; }

        /// <summary>
        /// 集群列表的快照，这些集群是可互换目标端点的集合
        /// </summary>
        public IReadOnlyList<ClusterConfig> Clusters { get; }

        /// <summary>
        /// 触发以指示代理状态已更改，且该快照现在已过时  
        /// </summary>
        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }
}
