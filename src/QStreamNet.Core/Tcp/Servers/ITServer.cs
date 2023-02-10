using TcpNet.Pipelines;

namespace TcpNet.Tcps
{
    public interface ITServer
    {
        public int PortNum { get; }

        Task StartAsync(StreamPipeDelegate application, CancellationToken cancellationToken);
        // void Listen();
        void Stop();
    }
}
