using System.Runtime.Versioning;
using QStreamNet.Core.StreamApp.Middlewares;
using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public static class StreamMiddlepointRouteBuilderExtensions
    {
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
            // var data = TCmdMiddleware.CmdData;
            // Console.Write("TCmd: ");
            // Console.Write(TCmdMiddleware.CmdData);
            // Console.Write(TCmdMiddleware.CmdName);
            // Console.WriteLine();
            middlepoints.Add(new StreamMiddlepoint(requestDelegate, TCmdMiddleware.CmdData, TCmdMiddleware.CmdName));
            return builder;
        }


        public static IStreamMiddlepointRouteBuilder Map<TCmdMiddleware>(this IStreamMiddlepointRouteBuilder builder, byte[] cmdData, string cmdName)
            where TCmdMiddleware : IStreamMiddleware
            // where TCmdMiddleware : IStreamMiddleware, ICmdMiddlepoint
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
            // builder.CreateApplicationBuilder().UseMiddleware<>().Build()
            return builder;
        }


        // public static IStreamMiddlepointRouteBuilder MapEnd(
        //     this IStreamMiddlepointRouteBuilder builder,
        //     StreamPipeDelegate requestDelegate)
        // {
        //     var middlepointRouteBuilder = builder;
        //     if (middlepointRouteBuilder is null){
        //         throw new InvalidOperationException($"{nameof(IStreamMiddlepointRouteBuilder)} is null.");
        //     }
        //     // middlepointRouteBuilder.DataSource.MiddleEndpoint = new StreamMiddlepoint(requestDelegate, null, null);
        //     return builder;
        // }
    }
}