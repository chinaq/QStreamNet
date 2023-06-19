using Microsoft.Extensions.DependencyInjection;
using QStreamNet.Core.StreamApp.Middlewares;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public static class StreamMiddlepointRouteBuilderExtensions
    {

        public static IStreamMiddlepointRouteBuilder MapMiddle<TCmdMiddleware>(this IStreamMiddlepointRouteBuilder builder)
            where TCmdMiddleware : IStreamMiddleware, ICmdPoint
        {
            var middlepointRouteBuilder = builder;
            if (middlepointRouteBuilder is null){
                throw new InvalidOperationException($"{nameof(IStreamMiddlepointRouteBuilder)} is null.");
            }
            // delegate
            var appBuilder = builder.CreateApplicationBuilder();
            appBuilder.UseMiddleware<TCmdMiddleware>();
            var middle = appBuilder.ApplicationServices.GetRequiredService<TCmdMiddleware>();
            var requestDelegate = appBuilder.Build();
            // points
            var middlepoints = middlepointRouteBuilder.DataSource.Middlepoints;
            middlepoints.Add(new StreamMiddlepoint(requestDelegate, middle.CmdData, middle.CmdName));
            return builder;
        }


        public static IStreamMiddlepointRouteBuilder Map<TCmdMiddleware>(this IStreamMiddlepointRouteBuilder builder)
            where TCmdMiddleware : IStreamMiddleware, ICmdMiddlepoint
        {
            var middlepointRouteBuilder = builder;
            if (middlepointRouteBuilder is null){
                throw new InvalidOperationException($"{nameof(IStreamMiddlepointRouteBuilder)} is null.");
            }
            // delegate
            var requestDelegate = builder.CreateApplicationBuilder().UseMiddleware<TCmdMiddleware>().Build();
            // points
            var middlepoints = middlepointRouteBuilder.DataSource.Middlepoints;
            middlepoints.Add(new StreamMiddlepoint(requestDelegate, TCmdMiddleware.CmdData, TCmdMiddleware.CmdName));
            return builder;
        }


        public static IStreamMiddlepointRouteBuilder Map<TCmdMiddleware>(this IStreamMiddlepointRouteBuilder builder, byte[] cmdData, string cmdName)
            where TCmdMiddleware : IStreamMiddleware
        {
            var middlepointRouteBuilder = builder;
            if (middlepointRouteBuilder is null){
                throw new InvalidOperationException($"{nameof(IStreamMiddlepointRouteBuilder)} is null.");
            }
            // delegate
            var requestDelegate = builder.CreateApplicationBuilder().UseMiddleware<TCmdMiddleware>().Build();
            // points
            var middlepoints = middlepointRouteBuilder.DataSource.Middlepoints;
            middlepoints.Add(new StreamMiddlepoint(requestDelegate, cmdData, cmdName));
            return builder;
        }


        public static IStreamMiddlepointRouteBuilder Map(
            this IStreamMiddlepointRouteBuilder builder,
            byte[] cmd,
            string name,
            StreamPipeDelegate requestDelegate)
        {
            var middlepointRouteBuilder = builder;
            if (middlepointRouteBuilder is null){
                throw new InvalidOperationException($"{nameof(IStreamMiddlepointRouteBuilder)} is null.");
            }
            var middlepoints = middlepointRouteBuilder.DataSource.Middlepoints;
            middlepoints.Add(new StreamMiddlepoint(requestDelegate, cmd, name));
            return builder;
        }

    }
}