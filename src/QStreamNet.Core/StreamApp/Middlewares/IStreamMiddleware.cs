namespace TcpNet.Pipelines
{
    public interface IStreamMiddleware
    {
        Task InvokeAsync(StreamContext context, StreamPipeDelegate next);
    }
}