using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;

namespace QStreamNet.Core.StreamApp
{
    public static class UseExtension
    {
        public static IStreamApplicationBuilder Use(this IStreamApplicationBuilder app, Func<StreamContext, StreamPipeDelegate, Task> middleware)
        {
            return app.Use(next => context => middleware(context, next));
        }
    }
}