using Microsoft.Extensions.Primitives;

namespace Yarp.Gateway.EntityFrameworkCore
{
    public class YarpConfigurationReloadToken : IChangeToken
    {
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public void OnReload() => tokenSource.Cancel();
        
        public bool ActiveChangeCallbacks { get; } = true;

        public bool HasChanged { get { return tokenSource.IsCancellationRequested; } }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return tokenSource.Token.Register(callback, state);
        }
    }
}
