using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Middlewares;
using QStreamNet.Core.Tcp.Servers;

namespace QStreamNet.Core.Tcp.Hosts
{
    public class TcpHostService : IHostedService
    {
        public IStreamApplicationBuilderFactory ApplicationBuilderFactory { get; }
        public ILogger Logger { get; }

        private TcpHostServiceOptions _tcpHostServcieOptions;
        private IHostApplicationLifetime _applicationLifetime;
        private ITServer _tServer;


        public TcpHostService(
            IStreamApplicationBuilderFactory applicationBuilderFactory,
            IOptions<TcpHostServiceOptions> tcpHostServiceOptions,
            ITServer tServer,
            IHostApplicationLifetime applicationLifetime,
            ILoggerFactory loggerFactory)
        {
            ApplicationBuilderFactory = applicationBuilderFactory;
            Logger = loggerFactory.CreateLogger<TcpHostService>();           
            _tcpHostServcieOptions = tcpHostServiceOptions.Value;
            _applicationLifetime = applicationLifetime;
            _tServer = tServer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting");
            var applicationBuilder = ApplicationBuilderFactory.CreateBuilder();

            // config
            var configure = _tcpHostServcieOptions.ConfigureApplication;
            if (configure == null) {
                throw new InvalidOperationException($"No application configured.");
            }
            configure(applicationBuilder);

            // run
            var app = applicationBuilder.Build();
            // tServer
            _ = TryStartServerAsync(_tServer, app, cancellationToken);

            Logger.LogInformation("Started");
            await Task.CompletedTask;
        }


        private async Task TryStartServerAsync(ITServer tServer, StreamPipeDelegate app, CancellationToken cancellationToken)
        {
            try {
                await _tServer.StartAsync(app, cancellationToken);
            } catch (Exception ex) {
                Logger.LogCritical(ex.ToString());
                _applicationLifetime.StopApplication();
            }
        }



        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Stopped");
            _tServer.Stop();
            await Task.CompletedTask;
        }
    }
}