using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp
{

    public class StreamApplicationBuilder : IStreamApplicationBuilder
    {
        private const string ApplicationServicesKey = "application.Services";
        private readonly List<Func<StreamPipeDelegate, StreamPipeDelegate>> _middlewares = new List<Func<StreamPipeDelegate, StreamPipeDelegate>>();

        public IDictionary<string, object?> Properties { get; }

        public IServiceProvider ApplicationServices
        {
            get { return GetProperty<IServiceProvider>(ApplicationServicesKey)!; }
            set { SetProperty<IServiceProvider>(ApplicationServicesKey, value); }
        }

        private ILogger<StreamApplicationBuilder> _logger;

        public StreamApplicationBuilder(IServiceProvider serviceProvider, ILogger<StreamApplicationBuilder>? logger = null)
        {
            Properties = new Dictionary<string, object?>(StringComparer.Ordinal);
            ApplicationServices = serviceProvider;
            _logger = logger?? new NullLogger<StreamApplicationBuilder>();
        }


        public StreamPipeDelegate Build()
        {
            StreamPipeDelegate next = _ => { 
                _logger.LogInformation("middleware flowed to 404");
                return Task.CompletedTask;
            };
            for (var c = _middlewares.Count - 1; c >= 0; c--) {
                next = _middlewares[c](next);
            }
            return next;
        }

        public void UseEnd(StreamPipeDelegate handler)
        {
            Use(_ => handler);
        }

        public IStreamApplicationBuilder Use(Func<StreamPipeDelegate, StreamPipeDelegate> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }



        private T? GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out var value) ? (T?)value : default(T);
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

    }
}