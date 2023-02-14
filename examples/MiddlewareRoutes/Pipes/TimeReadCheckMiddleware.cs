using QStreamNet.Core.StreamApp.MiddlePoints;
using TcpNet.Pipelines;

namespace MiddlewareRoutes.Pipes
{
    // public class TimeReadCheckMiddleware : IStreamMiddleware, ICmdMiddlepoint
    public class TimeReadCheckMiddleware : IStreamMiddleware, ICmdMiddlepoint
    {
        public static string CmdName => "检查读时间";
        public static byte[] CmdData => new byte[] { 0x0D };

        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine(CmdName);
            await next(context);
        }
    }
}
