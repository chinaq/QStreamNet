using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TcpNet.Pipelines;

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