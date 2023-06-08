using Microsoft.Extensions.Logging;

namespace QStreamNet.Core.StreamApp
{
    public class StreamApplicationBuilderFactory : IStreamApplicationBuilderFactory
    {
        private readonly IServiceProvider _services;
        private readonly ILoggerFactory _loggerFactory;

        public StreamApplicationBuilderFactory(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            _services = services;
            _loggerFactory = loggerFactory;
        }

        public IStreamApplicationBuilder CreateBuilder()
        {
            return new StreamApplicationBuilder(_services, _loggerFactory.CreateLogger<StreamApplicationBuilder>());
        }
    }
}