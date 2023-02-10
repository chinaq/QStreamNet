using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.EndPoints;

namespace TcpNet.Pipelines
{
    public class TcpApplication : IHost, IStreamApplicationBuilder, IStreamEndpointRouteBuilder, IAsyncDisposable
    {
        internal const string EndpointRouteBuilderKey = "__EndpointRouteBuilder";

        private readonly IHost _host;
        // private readonly List<StreamEndpointDataSource> _dataSources = new();
        private readonly StreamEndpointDataSource _dataSources = new();

        public ILogger Logger { get; }
        internal IStreamApplicationBuilder ApplicationBuilder { get; }

        public TcpApplication(IHost host)
        {
            _host = host;
            ApplicationBuilder = new StreamApplicationBuilder(host.Services);
            Logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger<TcpApplication>();

            Properties[EndpointRouteBuilderKey] = this;
        }

        public IServiceProvider Services => _host.Services;

        public IServiceProvider ServiceProvider => Services;

        // public ICollection<StreamEndpointDataSource> DataSources => _dataSources;
        public StreamEndpointDataSource DataSource => _dataSources;

        public IDictionary<string, object?> Properties => ApplicationBuilder.Properties;


        public IServiceProvider ApplicationServices
        {
            get => ApplicationBuilder.ApplicationServices;
            set => ApplicationBuilder.ApplicationServices = value;
        }

        // public IFeatureCollection ServerFeatures =>  _host.Services.GetRequiredService<IServer>().Features;

        public void Dispose() => _host.Dispose();

        public Task StartAsync(CancellationToken cancellationToken = default) => 
            _host.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken = default) =>
            _host.StopAsync(cancellationToken);


        public void Run(string? url = null)
        {
            Logger.LogInformation("Run Application");
            HostingAbstractionsHostExtensions.Run(this);
        }



        public IStreamApplicationBuilder Use(Func<StreamPipeDelegate, StreamPipeDelegate> middleware)
        {
            ApplicationBuilder.Use(middleware);
            return this;
        }

        // public IApplicationBuilder New() => ApplicationBuilder.New();


        public static TcpApplicationBuilder CreateBuilder(string[] args)
        {
            return new TcpApplicationBuilder(args);
        }

        public void UseEnd(StreamPipeDelegate handler)
        {
            ApplicationBuilder.UseEnd(handler);
        }

        void IDisposable.Dispose() => _host.Dispose();
        public ValueTask DisposeAsync() => ((IAsyncDisposable)_host).DisposeAsync();

        internal StreamPipeDelegate BuildRequestDelegate() => ApplicationBuilder.Build();
        public StreamPipeDelegate Build() => ApplicationBuilder.Build();

        public IStreamApplicationBuilder CreateApplicationBuilder()
        {
            return new StreamApplicationBuilder(this.ApplicationServices);
        }
    }
}