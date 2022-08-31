using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public interface IYarpConfigurationStore
    {
        public event YarpConfigurationChangeHandler ConfigurationChange;
        IProxyConfig GetConfig();

        void Reload();

        void ReloadConfig();
        
        IChangeToken GetReloadToken();
    }
}
