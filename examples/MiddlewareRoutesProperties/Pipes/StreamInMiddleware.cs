using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TcpNet.Pipelines;

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