using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.MiddlePoints;
using QStreamNet.Core.StreamApp.Middlewares;

namespace MiddlewareRoutes.Pipes
{
    public class TimeWriteCheckMiddleware : IStreamMiddleware, ICmdMiddlepoint
    {
        public static string CmdName => "检查写时间";

        public static byte[] CmdData => new byte[] { 0x0B };

        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine(CmdName);
            await next(context);
        }
    }
}