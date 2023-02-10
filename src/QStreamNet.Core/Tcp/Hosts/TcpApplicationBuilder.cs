using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;
using TcpNet.Tcps;

namespace TcpNet.Pipelines
{
    public class TcpApplicationBuilder
    {
        public IServiceCollection Services { get; }
        // private readonly HostBuilder _hostBuilder = new();
        public HostBuilder HostBuilder { get; }
        private TcpApplication? _tcpApplication;

        public TcpApplicationBuilder(string[] args)
        {
            this.Services = new ServiceCollection();
            this.HostBuilder = new();
            // this.Configuration = new(_hostBuilder.ho.context);
            // _hostBuilder.Configuration;
            HostBuilder
                .ConfigureDefaults(args)
                .ConfigureServices(services => {
                    services.Configure<TcpHostServiceOptions>(option => {
                        option.ConfigureApplication = UseAppMiddlewares;
                    });
                }).ConfigureServices((hostContext,services) => {
                    var tPort = hostContext.Configuration.GetRequiredSection(nameof(TPort)).Get<TPort>();;
                    services.AddSingleton<TPort>(tPort);
                    services.AddSingleton<ITServer, TServer>();
                }).ConfigureServices((hostContext,services) => {
                    services.TryAddSingleton<IStreamApplicationBuilderFactory, StreamApplicationBuilderFactory>();
                    services.AddSingleton<IStreamContextFactory, StreamContextFactory>();
                    services.AddScoped<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
                });
        }

        private void UseAppMiddlewares(IStreamApplicationBuilder app)
        {
            Debug.Assert(_tcpApplication is not null);
            app.Use(next => {
                _tcpApplication.UseEnd(next);
                return _tcpApplication.BuildRequestDelegate();
            });
        }

        public TcpApplication Build()
        {
            HostBuilder.ConfigureServices(service => {
                foreach(var s in Services) {
                    service.Add(s);
                }
                service.AddHostedService<TcpHostService>();
            });
            _tcpApplication = new TcpApplication(HostBuilder.Build());
            return _tcpApplication;
        }
    }
}