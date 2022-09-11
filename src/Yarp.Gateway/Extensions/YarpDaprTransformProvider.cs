using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Yarp.Gateway.Extensions
{
    public class YarpDaprTransformProvider : ITransformProvider
    {
        private readonly ILogger<YarpDaprTransformProvider> _logger;
        public YarpDaprTransformProvider(ILogger<YarpDaprTransformProvider> logger)
        {
            this._logger = logger;
            _logger.LogInformation("add YarpDaprTransformProvider");
        }

        public void Apply(TransformBuilderContext context)
        {
            _logger.LogInformation("YarpDaprTransformProvider Apply...");
            string daprAction = null;
            if (context.Route.Metadata?.TryGetValue(DaprYarpConstants.MetaKeys.Dapr, out daprAction) ?? false)
            {
                switch (daprAction)
                {
                    case DaprYarpConstants.DaprAct.Method:
                        context.AddRequestTransform(transformContext =>
                        {
                            var index = transformContext.Path.Value!.IndexOf('/', 5); // format: /api/xxxx
                            var appId = transformContext.Path.Value.Substring(5, index - 5);
                            var newPath = transformContext.Path.Value.Substring(index);
                            transformContext.ProxyRequest.RequestUri = new Uri($"{transformContext.DestinationPrefix}/v1.0/invoke/{appId}/method/api{newPath}");
                            return ValueTask.CompletedTask;
                        });
                        break;
                }
            }
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {
        }

        public void ValidateRoute(TransformRouteValidationContext context)
        {
        }
    }
}
