using Microsoft.Extensions.DependencyInjection;
using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.Middlewares
{
    public class StreamMiddlewareFactory : IStreamMiddlewareFactory
    {
        private IServiceProvider _serviceProvider;

        public StreamMiddlewareFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IStreamMiddleware? Create<TMiddleware>(params object[]? args) where TMiddleware : IStreamMiddleware
        {
            // return _serviceProvider.GetRequiredService<TMiddleware>();
            if (args == null) {
                args = new object[0];
            }
            return ActivatorUtilities.CreateInstance<TMiddleware>(_serviceProvider, args);
        }
    }
}