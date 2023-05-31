using QStreamNet.Core.StreamApp.MiddlePoints;
using TcpNet.Pipelines;

namespace MiddlewareRoutes.Pipes
{
    // public class TimeWriteMiddleware : IStreamMiddleware, ICmdMiddlepoint
    public class TimeWriteMiddleware : IStreamMiddleware, ICmdMiddlepoint
    {
        public static string CmdName => "写时间";
        public static byte[] CmdData => new byte[] { 0x0A };

        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine(CmdName);
            await next(context);
        }
    }
}