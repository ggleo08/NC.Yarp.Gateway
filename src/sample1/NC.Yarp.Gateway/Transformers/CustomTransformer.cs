using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

namespace NC.Yarp.Gateway.Transformers
{
    public class CustomTransformer : HttpTransformer
    {
        /// <summary>
        /// 在转发代理请求之前的回调。
        /// 除了 RequestUri 外，所有 HttpRequestMessage 信息都会初始化，如果没有提供值，则会在回调之后初始化 RequestUri。
        /// destinationPrefix 参数表示在构造 RequestUri 时应使用的目标 URI 前缀。 
        /// 报文头被父类实现了复制，但是排除了一些协议信息，如 HTTP/2 伪标头 (':authority') 。
        /// </summary>
        /// <param name="httpContext">收到的请求上下文</param>
        /// <param name="proxyRequest">发出的代理请求信息</param>
        /// <param name="destinationPrefix">可选的目标服务器 uri 前缀，可用于创建 RequestUri </param>
        /// <returns></returns>
        public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix)
        {
            // 复制所有请求报文头信息
            await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix);

            // 自定义 QueryString
            var queryContext = new QueryTransformContext(httpContext.Request);
            queryContext.Collection.Remove("param1");
            queryContext.Collection["area"] = "xx2";

            // 设置自定义 URI
            // 注意：这里拼接地址时需要注意斜杠问题
            proxyRequest.RequestUri = new Uri(destinationPrefix + httpContext.Request.Path + queryContext.QueryString);

            // 清除原始请求主机，将会改用来自目标 Uri 的目标主机（proxyRequest.RequestUri ）。
            proxyRequest.Headers.Host = null;
        }
    }
}
