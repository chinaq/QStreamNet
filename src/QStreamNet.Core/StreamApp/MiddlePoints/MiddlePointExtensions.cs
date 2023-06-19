using QStreamNet.Core.StreamApp.Contexts;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public static class MiddlePointExtensions
    {
        public static IStreamApplicationBuilder UseMiddleRouting(
            this IStreamApplicationBuilder builder, 
            Func<StreamContext, StreamMiddlepoint, bool> pointCheck,
            Func<IStreamMiddlepointRouteBuilder, IStreamMiddlepointRouteBuilder> middleBuilderConfig)
        {
            var middlepointRouteBuilder = new DefaultStreamMiddlepointRouteBuilder(builder);
            middleBuilderConfig(middlepointRouteBuilder);

            builder.Use(async (context, next) => {
                var middlepoints = middlepointRouteBuilder.DataSource.Middlepoints;
                // var theEndpoint = endpoints.FirstOrDefault(e => e.Metadata.SequenceEqual(cmdMatch(context.DataIn!)));
                var theMiddlepoint = middlepoints.FirstOrDefault(e => pointCheck(context, e));
                // context.Endpoint = theEndpoint;
                if (theMiddlepoint is null) { return; }
                await theMiddlepoint.RequestDelegate(context);
                await next(context);
            });
            return builder;
        }
    }
}