namespace QStreamNet.Core.Tcp.Hosts
{
    // public class TcpBootstrapHostBuilder : IHostBuilder
    // {
    //     private readonly IServiceCollection _services;
    //     private readonly List<Action<IConfigurationBuilder>> _configureHostActions = new();
    //     private readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new();
    //     private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppActions = new();

    //     public IDictionary<object, object> Properties => throw new NotImplementedException();

    //     public TcpBootstrapHostBuilder(IServiceCollection services)
    //     {
    //         _services = services;
    //     }

    //     public IHost Build()
    //     {
    //         throw new NotImplementedException();
    //     }

    //     public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    //     {
    //         _configureAppActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
    //         return this;
    //     }

    //     public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    //     {
    //         _configureHostActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
    //         return this;
    //     }

    //     public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    //     {
    //         // HostingHostBuilderExtensions.ConfigureDefaults calls this via ConfigureLogging
    //         _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
    //         return this;
    //     }

    //     public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
}
