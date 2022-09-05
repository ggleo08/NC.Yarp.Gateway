using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public interface IYarpConfigurationStore
    {
        public event YarpConfigurationChangeHandler ConfigurationChange;
        Task<IProxyConfig> GetConfigAsync();

        void Reload();

        Task ReloadConfigAsync();
        
        IChangeToken GetReloadToken();
    }
}
