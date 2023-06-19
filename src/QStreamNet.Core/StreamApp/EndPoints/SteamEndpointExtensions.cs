using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace QStreamNet.Core.StreamApp.EndPoints
{
    public static class SteamEndpointExtensions
    {

        private const string EndpointRouteBuilder = "__EndpointRouteBuilder";


        public static IStreamApplicationBuilder UseRouting(this IStreamApplicationBuilder builder, Func<byte[], byte[]> cmdMatch)
        {
            IStreamEndpointRouteBuilder endpointRouteBuilder;
            if (builder.Properties.TryGetValue(EndpointRouteBuilder, out var obj)) {
                endpointRouteBuilder = (IStreamEndpointRouteBuilder)obj!;
            } else {
                endpointRouteBuilder = new DefaultStreamEndpointRouteBuilder(builder);
                builder.Properties[EndpointRouteBuilder] = endpointRouteBuilder;
            }

            builder.Use(async (context, next) => {
                var endpoints = endpointRouteBuilder.DataSource.Endpoints;
                var theEndpoint = endpoints.FirstOrDefault(e => e.Metadata.SequenceEqual(cmdMatch(context.DataIn!)));
                context.Endpoint = theEndpoint;
                await next(context);
            });
            return builder;
        }


        public static IStreamApplicationBuilder UseEndpoints(this IStreamApplicationBuilder builder)
        {
            var logger = builder.ApplicationServices.GetService<ILogger<IStreamApplicationBuilder>>()
                ?? new NullLogger<IStreamApplicationBuilder>();

            builder.Use(async (context, next) => {
                var endpoint = context.Endpoint!;
                if (endpoint?.RequestDelegate != null) {
                    logger.LogInformation(context.GetHashCode(), $"route to {endpoint.Name}.");
                    await endpoint.RequestDelegate(context);
                } else {
                    logger.LogInformation(context.GetHashCode(), $"no endpoint to route.");
                    await next(context);
                };
            });
            return builder;
        }

    }
}