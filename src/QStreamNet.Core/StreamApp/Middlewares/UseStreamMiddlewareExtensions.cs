using Microsoft.Extensions.DependencyInjection;

namespace QStreamNet.Core.StreamApp.Middlewares
{
    public static class UseStreamMiddlewareExtensions
    {
        public static IStreamApplicationBuilder UseMiddleware<TMiddleware>(this IStreamApplicationBuilder app, params object[]? args) where TMiddleware : IStreamMiddleware
        {
            return app.Use(async (context, next) => {
                var middlewareFactory = context.Services!.GetService<IStreamMiddlewareFactory>();
                if (middlewareFactory == null) {
                    // No middleware factory
                    throw new InvalidOperationException($"no {nameof(IStreamMiddlewareFactory)}");
                }

                var middleware = middlewareFactory.Create<TMiddleware>(args);
                if (middleware == null)
                {
                    // The factory returned null, it's a broken implementation
                    throw new InvalidOperationException($"no {typeof(TMiddleware)} could be created by {typeof(IStreamMiddlewareFactory)}");
                }

                // try
                // {
                    await middleware.InvokeAsync(context, next);
                // }
                // finally
                // {
                //     middlewareFactory.Release(middleware);
                // }
            });
        }
    }
}