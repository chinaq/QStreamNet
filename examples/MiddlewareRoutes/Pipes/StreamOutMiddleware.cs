using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;

namespace MiddlewareRoutes.Pipes
{
    public class StreamOutMiddleware : IStreamMiddleware
    {
        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine("Data Out");
            await next(context);
        }
    }
}