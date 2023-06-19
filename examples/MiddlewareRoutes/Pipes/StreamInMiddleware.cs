using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;

namespace MiddlewareRoutes.Pipes
{
    public class StreamInMiddleware : IStreamMiddleware
    {
        public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
        {
            Console.WriteLine("Data In");
            await next(context);
        }
    }
}