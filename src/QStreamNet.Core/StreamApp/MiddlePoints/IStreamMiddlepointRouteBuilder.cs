namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public interface IStreamMiddlepointRouteBuilder
    {
         IStreamApplicationBuilder CreateApplicationBuilder();

        IServiceProvider ServiceProvider { get; }

        StreamMiddlepointDataSource DataSource { get; }
    }
}