using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp
{
    public interface IStreamApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; set; }
        IDictionary<string, object?> Properties { get; }

        IStreamApplicationBuilder Use(Func<StreamPipeDelegate, StreamPipeDelegate> middleware);
        void UseEnd(StreamPipeDelegate handler);
        StreamPipeDelegate Build();
        // IStreamApplicationBuilder New();
    }
}