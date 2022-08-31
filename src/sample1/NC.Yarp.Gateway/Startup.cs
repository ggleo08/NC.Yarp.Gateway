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
            #region �����������
            //// ��� Yarp �������
            //var proxyBuilder = services.AddReverseProxy();

            //// ��ʼ��������������ļ� appsettins.json �е� ReverseProxy �ڵ�
            //proxyBuilder.LoadFromConfig(Configuration.GetSection("ReverseProxy"));
            #endregion

            #region ֱ��ת������ Direct Forwading
            //services.AddHttpForwarder();
            #endregion

            #region ���뷽ʽ����·�ɡ���Ⱥ
            // �����ļ���ʽ�μ� appsettings.Development.json
            services.AddReverseProxy()
                    .LoadFromMemory(GetRoutes(), GetClusters());

            #endregion

            #region ���� HttpClient
            //services.AddReverseProxy()
            //        .ConfigureHttpClient((context, handler) =>
            //        {
            //            handler.SslOptions.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate("cert file path"));
            //        });

            #region �Զ��� IForwarderHttpClientFactory ���� HttpClient ����Ĵ���
            //services.AddSingleton<IForwarderHttpClientFactory, CustomForwarderHttpClientFactory>();
            #endregion

            #endregion

            #region ��֤����Ȩ
            services.AddAuthorization(options =>
             {
                 options.AddPolicy("authPolicy", policy =>
                  {
                      policy.RequireAuthenticatedUser();
                  });
                 // ���˲��ԣ����ò��ԣ�
                 // �������κ�δ���ò��Ե������·�ɵĲ���
                 // Ĭ������£�FallbackPolicy û��ֵ���κ����󶼽�������
                 // options.FallbackPolicy = null;
             });
            #endregion

            #region �������
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

            #region  ֱ��ת����������
            //// Ϊ��������ĳ�վ�������������Լ��� HttpMessageInvoker
            //var messageInvoker = new HttpMessageInvoker(new SocketsHttpHandler()
            //{
            //    UseProxy = false,
            //    AllowAutoRedirect = false,
            //    AutomaticDecompression = DecompressionMethods.None,
            //    UseCookies = false
            //});

            //// ��������ת����
            //var transformer = new CustomTransformer(); // or HttpTransformer.Default;
            //// ��������
            //var requestOptions = new ForwarderRequestConfig() { 
            //    Timeout = TimeSpan.FromSeconds(100)
            //};
            #endregion

            // ���ö˵�·��
            app.UseRouting();

            // ���ÿ���
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});

                #region �����������
                //// ע�ᷴ�����·��
                //endpoints.MapReverseProxy();
                #endregion

                #region ֱ��ת������
                //// ��ʹ�� IHttpForwarder ����ֱ��ת��ʱ����Ҫ����·�ɡ�Ŀ�귢�֡�����ƽ�⡢�����ȡ�
                //endpoints.Map("/{**catch-all}", async httpContext =>
                //{
                //    var error = await forwarder.SendAsync(httpContext, "https://baidu.com", messageInvoker, requestOptions, transformer);
                //    // У���������Ƿ�ɹ�
                //    // IHttpForwarder �� HTTP �ͻ��˲����쳣�ͳ�ʱ����¼��������ת��Ϊ 5xx ״̬�����ֹ��Ӧ
                //    // SendAsync ���ش�����룬����������Դ� IForwarderErrorFeature ��ȡ
                //    if (error != ForwarderError.None)
                //    {
                //        var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                //        var exception = errorFeature.Exception;
                //    }
                //});
                #endregion

                #region ���뷽ʽ����·�ɡ���Ⱥ
                // ע�ᷴ�����·��
                endpoints.MapReverseProxy();
                #endregion

            });
        }

        #region ���뷽ʽ����·�ɡ���Ⱥ

        private RouteConfig[] GetRoutes()
        {
            return new[]
            {
              new RouteConfig()
              {
                  RouteId = "RouteId_1",
                  ClusterId = "ClusterId_1",

                  #region ��Ȩ����
                  //AuthorizationPolicy = "authPolicy", // ��Ȩ����
                  #endregion

                  #region �������
                  //CorsPolicy = "customPolicy",
                  #endregion

                  Match = new RouteMatch
                  {
                      // ÿ��·�ɶ���Ҫָ��·���������� ���� ��catch-all�� ģʽƥ����������·����  
                      Path = "{**catch-all}",

                      #region Header Routing, �����ļ���ʽ�μ� sppsettings.HttpClient.json
                      //Headers = new []
                      //{
                      //    new RouteHeader()
                      //    {
                      //        // Ҫ�������м��ı�ͷ���ƣ��ǿգ������ִ�Сд
                      //        Name = "header1",
                      //        // ƥ���б�ͷ�������ָ����ģʽƥ����Щֵ�е�����һ�������� Model ����Ϊ'NotContains'
                      //        // ͷ�������ָ����ģʽƥ����Щֵ�е�����һ�������� Mode ����Ϊ'NotContains'
                      //        // ���� Mode ����Ϊ 'Exists'��ֻ��Ҫ header ���ƴ��ڼ��ɣ�����������Ҫһ��ֵ
                      //        Values = new [] { "1prefix", "2prefix" },
                      //        // ƥ��ģʽ
                      //        // HeaderMatchMode ����ָ����ν�ƥ���б��������ͷ����ƥ�䣬Ĭ��ֵΪ��ȫƥ�� 'ExactHeader'
                      //        Mode = HeaderMatchMode.HeaderPrefix,
                      //        // �Ƿ����ִ�Сд��Ĭ��false
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
                  // �Ự����
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
