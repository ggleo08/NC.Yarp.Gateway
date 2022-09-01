using Dapr.Client;
using Yarp.ReverseProxy.Forwarder;

namespace Yarp.Gateway.Extensions
{
    public class ___YarpForwarderHttpClientFactory // : IForwarderHttpClientFactory
    {
        private readonly DaprClient _daprClient;
        public ___YarpForwarderHttpClientFactory(DaprClient daprClient)
        {
            this._daprClient = daprClient;
        }
        public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
        {
            return null;
        }
    }
}
