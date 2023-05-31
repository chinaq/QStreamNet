using QStreamNet.Core.StreamApp.MiddlePoints;
using TcpNet.Pipelines;

namespace MiddlewareRoutes.Pipes
{
    // public class TimeReadCheckMiddleware : IStreamMiddleware, ICmdMiddlepoint
    public class TimeReadCheckMiddleware : IStreamMiddleware, ICmdPoint
    {
        public static readonly string CMD_NAME__TIME_READ_CHECK = "检查读时间";
        public string CmdName => CMD_NAME__TIME_READ_CHECK;
        public byte[] CmdData => new byte[] { 0x0D };

        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine(CmdName);
            await next(context);
        }
    }
}
