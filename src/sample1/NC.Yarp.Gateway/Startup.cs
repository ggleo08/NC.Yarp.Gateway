using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace NC.Yarp.Gateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region 基本代理服务
            //// 添加 Yarp 反向代理
            //var proxyBuilder = services.AddReverseProxy();

            //// 初始化反向代理，配置文件 appsettins.json 中的 ReverseProxy 节点
            //proxyBuilder.LoadFromConfig(Configuration.GetSection("ReverseProxy"));
            #endregion

            #region 直接转发服务 Direct Forwading
            //services.AddHttpForwarder();
            #endregion

            #region 代码方式配置路由、集群
            // 配置文件方式参见 appsettings.Development.json
            services.AddReverseProxy()
                    .LoadFromMemory(GetRoutes(), GetClusters());

            #endregion

            #region 配置 HttpClient
            //services.AddReverseProxy()
            //        .ConfigureHttpClient((context, handler) =>
            //        {
            //            handler.SslOptions.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate("cert file path"));
            //        });

            #region 自定义 IForwarderHttpClientFactory 控制 HttpClient 对象的创建
            //services.AddSingleton<IForwarderHttpClientFactory, CustomForwarderHttpClientFactory>();
            #endregion

            #endregion

            #region 认证和授权
            services.AddAuthorization(options =>
             {
                 options.AddPolicy("authPolicy", policy =>
                  {
                      policy.RequireAuthenticatedUser();
                  });
                 // 回退策略（备用策略）
                 // 将用于任何未配置策略的请求或路由的策略
                 // 默认情况下，FallbackPolicy 没有值，任何请求都将被允许
                 // options.FallbackPolicy = null;
             });
            #endregion

            #region 跨域策略
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("customPolicy", builder =>
            //    {
            //        builder.AllowAnyOrigin();
            //    });
            //});
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpForwarder forwarder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region  直接转发服务配置
            //// 为代理操作的出站调用配置我们自己的 HttpMessageInvoker
            //var messageInvoker = new HttpMessageInvoker(new SocketsHttpHandler()
            //{
            //    UseProxy = false,
            //    AllowAutoRedirect = false,
            //    AutomaticDecompression = DecompressionMethods.None,
            //    UseCookies = false
            //});

            //// 设置请求转换类
            //var transformer = new CustomTransformer(); // or HttpTransformer.Default;
            //// 请求配置
            //var requestOptions = new ForwarderRequestConfig() { 
            //    Timeout = TimeSpan.FromSeconds(100)
            //};
            #endregion

            // 启用端点路由
            app.UseRouting();

            // 启用跨域
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});

                #region 基本代理服务
                //// 注册反向代理路由
                //endpoints.MapReverseProxy();
                #endregion

                #region 直接转发服务
                //// 当使用 IHttpForwarder 进行直接转发时，你要负责路由、目标发现、负载平衡、关联等。
                //endpoints.Map("/{**catch-all}", async httpContext =>
                //{
                //    var error = await forwarder.SendAsync(httpContext, "https://baidu.com", messageInvoker, requestOptions, transformer);
                //    // 校验代理操作是否成功
                //    // IHttpForwarder 从 HTTP 客户端捕获异常和超时，记录并将它们转换为 5xx 状态码或中止响应
                //    // SendAsync 返回错误代码，错误详情可以从 IForwarderErrorFeature 获取
                //    if (error != ForwarderError.None)
                //    {
                //        var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                //        var exception = errorFeature.Exception;
                //    }
                //});
                #endregion

                #region 代码方式配置路由、集群
                // 注册反向代理路由
                endpoints.MapReverseProxy();
                #endregion

            });
        }

        #region 代码方式配置路由、集群

        private RouteConfig[] GetRoutes()
        {
            return new[]
            {
              new RouteConfig()
              {
                  RouteId = "RouteId_1",
                  ClusterId = "ClusterId_1",

                  #region 授权策略
                  //AuthorizationPolicy = "authPolicy", // 授权策略
                  #endregion

                  #region 跨域策略
                  //CorsPolicy = "customPolicy",
                  #endregion

                  Match = new RouteMatch
                  {
                      // 每条路由都需要指定路径或主机。 这种 “catch-all” 模式匹配所有请求路径。  
                      Path = "{**catch-all}",

                      #region Header Routing, 配置文件方式参见 sppsettings.HttpClient.json
                      //Headers = new []
                      //{
                      //    new RouteHeader()
                      //    {
                      //        // 要在请求中检查的表头名称，非空，不区分大小写
                      //        Name = "header1",
                      //        // 匹配列表，头必须根据指定的模式匹配这些值中的至少一个，除非 Model 设置为'NotContains'
                      //        // 头必须根据指定的模式匹配这些值中的至少一个，除非 Mode 设置为'NotContains'
                      //        // 除非 Mode 设置为 'Exists'，只需要 header 名称存在即可，否则至少需要一个值
                      //        Values = new [] { "1prefix", "2prefix" },
                      //        // 匹配模式
                      //        // HeaderMatchMode 用来指定如何将匹配列表与请求标头进行匹配，默认值为完全匹配 'ExactHeader'
                      //        Mode = HeaderMatchMode.HeaderPrefix,
                      //        // 是否区分大小写，默认false
                      //        IsCaseSensitive = true
                      //    }
                      //}
                      #endregion
                  }
              }
            };
        }

        private const string DEBUG_HEADER = "Debug";
        private const string DEBUG_METADATA_KEY = "debug";
        private const string DEBUG_VALUE = "true";
        private ClusterConfig[] GetClusters()
        {
            var debugMetadata = new Dictionary<string, string>();
            debugMetadata.Add(DEBUG_METADATA_KEY, DEBUG_VALUE);

            return new[] {
              new ClusterConfig{
                  ClusterId = "ClusterId_1",
                  // 会话关联
                  SessionAffinity = new SessionAffinityConfig
                  {
                      Enabled = true,
                      Policy = "Cookie",
                      AffinityKeyName = ".Yarp.ReverseProxy.Affinity"
                  },
                  Destinations = new Dictionary<string, DestinationConfig> (StringComparer.OrdinalIgnoreCase)
                  {
                      { "destination1", new DestinationConfig() { Address = "https://gateway-sit.aidoin.com/api/operatetoolkit/" } },
                      //{ "debugdestination1", new DestinationConfig() {
                      //    Address = "https://www.baidu.com",
                      //    Metadata = debugMetadata  }
                      //},
                  },
                  HttpClient = new HttpClientConfig
                  {
                      MaxConnectionsPerServer = 10,
                      SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12,
                  }
              }
            };
        }

        #endregion
    }
}
