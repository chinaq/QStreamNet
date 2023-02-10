namespace QStreamNet.Core.StreamApp.EndPoints
{
    public interface IStreamEndpointRouteBuilder
    {
         IStreamApplicationBuilder CreateApplicationBuilder();

        IServiceProvider ServiceProvider { get; }

        StreamEndpointDataSource DataSource { get; }
    }
}