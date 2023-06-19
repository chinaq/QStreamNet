using QStreamNet.Core.StreamApp.Middlewares;

namespace QStreamNet.Core.Tcp.Servers
{
    public interface ITServer
    {
        public int PortNum { get; }

        Task StartAsync(StreamPipeDelegate application, CancellationToken cancellationToken);
        // void Listen();
        void Stop();
    }
}
