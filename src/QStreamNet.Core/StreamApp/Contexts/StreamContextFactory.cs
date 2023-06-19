using Microsoft.Extensions.DependencyInjection;

namespace QStreamNet.Core.StreamApp.Contexts
{
    public class StreamContextFactory : IStreamContextFactory
    {
        private IServiceProvider _serviceProvider;

        public StreamContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider  = serviceProvider;
        }

        public StreamContext Create()
        {
            var context = new StreamContext();
            var scope = _serviceProvider.CreateScope();
            var requestServcies = scope.ServiceProvider;
            context.Scope = scope;
            context.Services = requestServcies;
            return context;
        }
    }
}