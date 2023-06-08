using Microsoft.Extensions.DependencyInjection;
using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.Middlewares
{
    public class StreamMiddlewareFactory : IStreamMiddlewareFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StreamMiddlewareFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IStreamMiddleware? Create<TMiddleware>(params object[]? args) where TMiddleware : IStreamMiddleware
        {
            // return _serviceProvider.GetRequiredService<TMiddleware>();
            args ??= new object[0];
            return ActivatorUtilities.CreateInstance<TMiddleware>(_serviceProvider, args);
        }
    }
}