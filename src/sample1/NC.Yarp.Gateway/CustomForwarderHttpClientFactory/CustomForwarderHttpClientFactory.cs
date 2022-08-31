using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace NC.Yarp.Gateway
{
    /// <summary>
    /// 自定义转发 HttpClientFactory
    /// https://microsoft.github.io/reverse-proxy/articles/http-client-config.html
    /// </summary>
    public class CustomForwarderHttpClientFactory : IForwarderHttpClientFactory
    {
        public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
        {
            // 建议任何自定义工厂将以下 SocketsHttpHandler 属性设置为与默认工厂相同的值，以保留正确的反向代理行为并避免不必要的开销。
            var handler = new SocketsHttpHandler
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                UseCookies = false
            };

            return new HttpMessageInvoker(handler, disposeHandler: true);
        }
    }
}
