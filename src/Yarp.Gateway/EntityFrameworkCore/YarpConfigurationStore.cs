using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Security.Authentication;
//using System.Security.Cryptography.X509Certificates;
using Dapr.Client;
using Yarp.Gateway.Entities;
using Yarp.Gateway.Extensions;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public class YarpConfigurationStore : IYarpConfigurationStore
    {

        private YarpConfigurationReloadToken _reloadToken = new YarpConfigurationReloadToken();
        public event YarpConfigurationChangeHandler ConfigurationChange;

        private IServiceProvider _serviceProvider;
        //private IMemoryCache _cache;
        private readonly DaprClient _daprClient;
        private readonly ILogger<YarpConfigurationStore> _logger;

        public YarpConfigurationStore(IServiceProvider serviceProvider,
                                      IMemoryCache cache,
                                      DaprClient daprClient,
                                      ILogger<YarpConfigurationStore> logger)
        {
            this._serviceProvider = serviceProvider;
            //this._cache = cache;
            this._daprClient = daprClient;
            this._logger = logger;

            this.ConfigurationChange += this.ReloadConfig;
        }

        // Used by tests
        //internal LinkedList<WeakReference<X509Certificate2>> Certificates { get; } = new LinkedList<WeakReference<X509Certificate2>>();

        public async Task<IProxyConfig> GetConfigAsync()
        {
            _logger.LogInformation("GetConfig");

            var cacheConfig = await _daprClient.GetStateAsync<YarpProxyConfig>("statestore", "ReverseProxyConfig");
            if (cacheConfig != null)
            {
                return cacheConfig;
            }
            else
            {
                cacheConfig = await GetFromDbAsync() as YarpProxyConfig;
                await SetConfigAsync(cacheConfig);

                return cacheConfig;
            }

            #region MemoryCache
            //var exist = _cache.TryGetValue<IProxyConfig>("ReverseProxyConfig", out IProxyConfig config);
            //if (exist)
            //{
            //    return config;
            //}
            //else
            //{
            //    config = GetFromDb();
            //    SetConfig(config);

            //    return config;
            //}
            #endregion

        }

        public IChangeToken GetReloadToken()
        {
            return _reloadToken;
        }

        public void Reload()
        {
            _logger.LogInformation("ChangeConfig");
            if (ConfigurationChange != null)
                ConfigurationChange();
        }

        public void ReloadConfig()
        {
            ReloadConfigAsync().Wait();
        }

        public async Task ReloadConfigAsync()
        {
            _logger.LogInformation("SetConfig");
            await SetConfigAsync();
            Interlocked.Exchange<YarpConfigurationReloadToken>(ref this._reloadToken, new YarpConfigurationReloadToken()).OnReload();
        }

        private async Task SetConfigAsync()
        {
            var config = await GetFromDbAsync();
            await SetConfigAsync(config);
        }

        private async Task SetConfigAsync(IProxyConfig config)
        {
            await _daprClient.SaveStateAsync("statestore", "ReverseProxyConfig", config);
            // _cache.Set("ReverseProxyConfig", config);
        }

        private async Task<IProxyConfig> GetFromDbAsync()
        {
            var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<YarpDbContext>();
            var routers = await dbContext.Set<YarpRoute>()
                                         .Include(r => r.Match).ThenInclude(m => m.Headers)
                                         .Include(r => r.Metadata)
                                         .Include(r => r.Transforms)
                                         .Include(r => r.Cluster)
                                         .AsNoTracking().ToListAsync();
            var clusters = await dbContext.Set<YarpCluster>()
                                          .Include(c => c.Metadata)
                                          .Include(c => c.Destinations)
                                          .Include(c => c.SessionAffinity).ThenInclude(s => s.Cookie)
                                          .Include(c => c.HttpRequest)
                                          .Include(c => c.HttpClient)
                                          .Include(c => c.HealthCheck).ThenInclude(h => h.Active)
                                          .Include(c => c.HealthCheck).ThenInclude(h => h.Passive)
                                          .AsNoTracking().ToListAsync();

            var newConfig = new YarpProxyConfig();
            foreach (var cluster in clusters)
            {
                newConfig.Clusters.Add(CreateCluster(cluster));
            }

            foreach (var section in routers)
            {
                newConfig.Routes.Add(CreateRoute(section));
            }

            //var configString = JsonSerializer.Serialize(newConfig);
            //Logger.LogInformation(JsonSerializer.Serialize(newConfig));

            return newConfig;
        }

        private ClusterConfig CreateCluster(YarpCluster cluster)
        {
            var destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);
            foreach (var destination in cluster.Destinations)
            {
                destinations.Add(destination.Name, CreateDestination(destination));
            }

            return new ClusterConfig
            {
                ClusterId = cluster.ClusterId,
                LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                SessionAffinity = CreateSessionAffinityOptions(cluster.SessionAffinity),
                HealthCheck = CreateHealthCheckOptions(cluster.HealthCheck),
                HttpClient = CreateHttpClientConfig(cluster.HttpClient),
                HttpRequest = CreateProxyRequestConfig(cluster.HttpRequest),
                Metadata = cluster.Metadata.ReadStringDictionary(),
                Destinations = destinations,
            };
        }

        private static RouteConfig CreateRoute(YarpRoute proxyRoute)
        {
            if (string.IsNullOrEmpty(proxyRoute.RouteId))
            {
                throw new Exception("The route config format has changed, routes are now objects instead of an array. The route id must be set as the object name, not with the 'RouteId' field.");
            }
            return new RouteConfig
            {
                RouteId = proxyRoute.RouteId,
                Order = proxyRoute.Order,
                ClusterId = proxyRoute.Cluster.ClusterId, //.ClusterId,
                AuthorizationPolicy = proxyRoute.AuthorizationPolicy,
                CorsPolicy = proxyRoute.CorsPolicy,
                Metadata = proxyRoute.Metadata.ReadStringDictionary(),
                Transforms = CreateTransforms(proxyRoute.Transforms),
                Match = CreateProxyMatch(proxyRoute.Match),
            };
        }

        private static IReadOnlyList<IReadOnlyDictionary<string, string>> CreateTransforms(List<YarpTransform> transforms)
        {
            if (transforms is null || transforms.Count == 0)
            {
                return null;
            }
            var groupTransforms = transforms.OrderBy(t => t.Id).GroupBy(t => t.Type);
            var list = new List<IReadOnlyDictionary<string, string>>();
            foreach (var group in groupTransforms)
            {
                var key = group.Key.ToString();
                Dictionary<string, string> dir = new Dictionary<string, string>();
                foreach (var transform in group)
                {
                    if (transform.Key == key)
                    {
                        if (dir.Count != 0)
                            list.Add(dir);
                        dir = new Dictionary<string, string>();
                    }
                    dir.Add(transform.Key, transform.Value);
                }
                if (dir.Count != 0)
                    list.Add(dir);
            }
            return list;
        }

        private static RouteMatch CreateProxyMatch(YarpMatch match)
        {
            if (match is null)
            {
                return null;
            }

            return new RouteMatch()
            {
                Methods = match.Methods.ReadStringArray(),
                Hosts = match.Hosts.ReadStringArray(),
                Path = match.Path,
                Headers = CreateRouteHeaders(match.Headers),
                QueryParameters = CreateRouteQueryParameters(match.QueryParameters)
            };
        }

        private static IReadOnlyList<RouteHeader>? CreateRouteHeaders(List<YarpRouteHeader> routeHeaders)
        {
            if (routeHeaders is null || routeHeaders.Count == 0)
            {
                return null;
            }

            return routeHeaders.Select(data => CreateRouteHeader(data)).ToArray();
        }

        private static RouteHeader CreateRouteHeader(YarpRouteHeader routeHeader)
        {
            return new RouteHeader()
            {
                Name = routeHeader.Name,
                Values = routeHeader.Values.ReadStringArray(),
                Mode = routeHeader.Mode,
                IsCaseSensitive = routeHeader.IsCaseSensitive,
            };
        }

        private static IReadOnlyList<RouteQueryParameter>? CreateRouteQueryParameters(IReadOnlyList<YarpRouteQueryParameter> routeQueryParameters)
        {
            if (routeQueryParameters is null)
            {
                return null;
            }

            return routeQueryParameters.Select(data => CreateRouteQueryParameter(data)).ToArray();
        }

        private static RouteQueryParameter CreateRouteQueryParameter(YarpRouteQueryParameter routeQueryParameter)
        {
            return new RouteQueryParameter()
            {
                Name = routeQueryParameter.Name,
                Values = routeQueryParameter.Values.ReadStringArray(),
                Mode = routeQueryParameter.Mode,
                IsCaseSensitive = routeQueryParameter.IsCaseSensitive,
            };
        }

        private static SessionAffinityConfig? CreateSessionAffinityOptions(YarpSessionAffinityConfig sessionAffinityOptions)
        {
            if (sessionAffinityOptions is null)
            {
                return null;
            }

            return new SessionAffinityConfig
            {
                Policy = sessionAffinityOptions.Policy,
                FailurePolicy = sessionAffinityOptions.FailurePolicy,
                AffinityKeyName = sessionAffinityOptions.AffinityKeyName,
                Enabled = sessionAffinityOptions.Enabled ?? false,
                Cookie = CreateSessionAffinityCookieConfig(sessionAffinityOptions.Cookie)
            };
        }

        private static SessionAffinityCookieConfig? CreateSessionAffinityCookieConfig(YarpSessionAffinityCookie sessionAffinityCookie)
        {
            if (sessionAffinityCookie is null)
            {
                return null;
            }

            return new SessionAffinityCookieConfig
            {
                Path = sessionAffinityCookie.Path,
                SameSite = sessionAffinityCookie.SameSite,
                HttpOnly = sessionAffinityCookie.HttpOnly,
                MaxAge = sessionAffinityCookie.MaxAge.ReadTimeSpan(),
                Domain = sessionAffinityCookie.Domain,
                IsEssential = sessionAffinityCookie.IsEssential,
                SecurePolicy = sessionAffinityCookie.SecurePolicy,
                Expiration = sessionAffinityCookie.Expiration.ReadTimeSpan()
            };
        }

        private static HealthCheckConfig? CreateHealthCheckOptions(YarpHealthCheckOption healthCheckOptions)
        {
            if (healthCheckOptions is null)
            {
                return null;
            }

            return new HealthCheckConfig
            {
                Passive = CreatePassiveHealthCheckOptions(healthCheckOptions.Passive),
                Active = CreateActiveHealthCheckOptions(healthCheckOptions.Active),
                AvailableDestinationsPolicy = healthCheckOptions.AvailableDestinationsPolicy
            };
        }

        private static PassiveHealthCheckConfig? CreatePassiveHealthCheckOptions(YarpPassiveHealthCheckOption passiveHealthCheckOptions)
        {
            if (passiveHealthCheckOptions is null)
            {
                return null;
            }

            return new PassiveHealthCheckConfig
            {
                Enabled = passiveHealthCheckOptions.Enabled,
                Policy = passiveHealthCheckOptions.Policy,
                ReactivationPeriod = passiveHealthCheckOptions.ReactivationPeriod.ReadTimeSpan()
            };
        }

        private static ActiveHealthCheckConfig? CreateActiveHealthCheckOptions(YarpActiveHealthCheckOption activeHealthCheckOptions)
        {
            if (activeHealthCheckOptions is null)
            {
                return null;
            }

            return new ActiveHealthCheckConfig
            {
                Enabled = activeHealthCheckOptions.Enabled,
                Interval = activeHealthCheckOptions.Interval.ReadTimeSpan(),
                Timeout = activeHealthCheckOptions.Timeout.ReadTimeSpan(),
                Policy = activeHealthCheckOptions.Policy,
                Path = activeHealthCheckOptions.Path
            };
        }

        private static HttpClientConfig? CreateHttpClientConfig(YarpHttpClientOption proxyHttpClientOptions)
        {
            if (proxyHttpClientOptions is null)
            {
                return null;
            }

            SslProtocols? sslProtocols = null;
            if (!string.IsNullOrWhiteSpace(proxyHttpClientOptions?.SslProtocols))
            {

                foreach (var protocolConfig in proxyHttpClientOptions?.SslProtocols?.Split(",").Select(s => Enum.Parse<SslProtocols>(s, ignoreCase: true)))
                {
                    sslProtocols = sslProtocols == null ? protocolConfig : sslProtocols | protocolConfig;
                }
            }
            else
            {
                sslProtocols = SslProtocols.None;
            }

            WebProxyConfig? webProxy;
            var webProxySection = proxyHttpClientOptions.WebProxy;
            if (webProxySection != null)
            {
                webProxy = new WebProxyConfig()
                {
                    Address = string.IsNullOrWhiteSpace(webProxySection.Address) ? null : new Uri(webProxySection.Address),
                    BypassOnLocal = webProxySection.BypassOnLocal,
                    UseDefaultCredentials = webProxySection.UseDefaultCredentials
                };
            }
            else
            {
                webProxy = null;
            }
            return new HttpClientConfig
            {
                SslProtocols = sslProtocols,
                DangerousAcceptAnyServerCertificate = proxyHttpClientOptions.DangerousAcceptAnyServerCertificate,
                MaxConnectionsPerServer = proxyHttpClientOptions.MaxConnectionsPerServer,
#if NET
                EnableMultipleHttp2Connections = proxyHttpClientOptions.EnableMultipleHttp2Connections,
                RequestHeaderEncoding = proxyHttpClientOptions.RequestHeaderEncoding,
#endif
                WebProxy = webProxy
            };
        }

        private static ForwarderRequestConfig? CreateProxyRequestConfig(YarpForwarderRequest requestProxyOptions)
        {
            if (requestProxyOptions is null)
            {
                return null;
            }

            return new ForwarderRequestConfig
            {
                ActivityTimeout = requestProxyOptions.ActivityTimeout.ReadTimeSpan(),
                Version = requestProxyOptions.Version.ReadVersion(),
#if NET
                VersionPolicy = requestProxyOptions.VersionPolicy.ReadEnum<HttpVersionPolicy>(),
#endif
                AllowResponseBuffering = requestProxyOptions.AllowResponseBuffering
            };
        }

        private static DestinationConfig CreateDestination(YarpDestination destination)
        {
            if (destination is null)
            {
                return null;
            }

            return new DestinationConfig
            {
                Address = destination.Address,
                Health = destination.Health,
                Metadata = destination.Metadata.ReadStringDictionary(),
            };
        }

    }
}
