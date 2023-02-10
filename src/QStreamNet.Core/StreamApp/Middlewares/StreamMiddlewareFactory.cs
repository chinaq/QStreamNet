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


        public IStreamMiddleware? Create<TMiddleware>() where TMiddleware : IStreamMiddleware
        {
            return _serviceProvider.GetRequiredService<TMiddleware>();
        }
    }
}