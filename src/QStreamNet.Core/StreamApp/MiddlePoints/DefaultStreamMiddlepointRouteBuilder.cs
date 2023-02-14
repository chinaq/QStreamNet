using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public class DefaultStreamMiddlepointRouteBuilder : IStreamMiddlepointRouteBuilder
    {
        public DefaultStreamMiddlepointRouteBuilder(IStreamApplicationBuilder applicationBuilder)
        {
            ApplicationBuilder = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            // DataSources = new List<EndpointDataSource>();
            DataSource = new StreamMiddlepointDataSource();
        }

        public IStreamApplicationBuilder ApplicationBuilder { get; }

        public IStreamApplicationBuilder CreateApplicationBuilder() => new StreamApplicationBuilder(ApplicationBuilder.ApplicationServices);

        public IServiceProvider ServiceProvider => ApplicationBuilder.ApplicationServices;

        public StreamMiddlepointDataSource DataSource { get; }
    }
}
