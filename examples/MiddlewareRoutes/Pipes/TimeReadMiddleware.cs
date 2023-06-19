using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.MiddlePoints;
using QStreamNet.Core.StreamApp.Middlewares;

namespace MiddlewareRoutes.Pipes
{
    // public class TimeReadMiddleware : IStreamMiddleware, ICmdMiddlepoint
    public class TimeReadMiddleware : IStreamMiddleware, ICmdMiddlepoint
    {
        public static string CmdName => "读时间";

        public static byte[] CmdData => new byte[] { 0x0C };

        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine(CmdName);
            await next(context);
        }
    }
}