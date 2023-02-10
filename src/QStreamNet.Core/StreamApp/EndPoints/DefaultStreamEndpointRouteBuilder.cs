namespace QStreamNet.Core.StreamApp.EndPoints
{
    public class DefaultStreamEndpointRouteBuilder : IStreamEndpointRouteBuilder
    {
        public DefaultStreamEndpointRouteBuilder(IStreamApplicationBuilder applicationBuilder)
        {
            ApplicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            // DataSources = new List<EndpointDataSource>();
            DataSource = new StreamEndpointDataSource();
        }

        public IStreamApplicationBuilder ApplicationBuilder { get; }

        public IStreamApplicationBuilder CreateApplicationBuilder() => new StreamApplicationBuilder(ApplicationBuilder.ApplicationServices);

        public IServiceProvider ServiceProvider => ApplicationBuilder.ApplicationServices;

        public StreamEndpointDataSource DataSource { get; }
    }
}