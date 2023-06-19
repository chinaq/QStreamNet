using QStreamNet.Core.StreamApp.Contexts;

namespace QStreamNet.Core.StreamApp.Middlewares
{
    public interface IStreamMiddleware
    {
        Task InvokeAsync(StreamContext context, StreamPipeDelegate next);
    }
}