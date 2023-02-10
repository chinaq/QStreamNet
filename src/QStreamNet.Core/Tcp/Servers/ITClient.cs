using System.Net.Sockets;
using TcpNet.Pipelines;

namespace TcpNet.Tcps
{
    public interface ITClient
    {
        bool Connected { get; }
        void Close();
        NetworkStream GetStream();
        Task ListeningAsync(StreamPipeDelegate app, CancellationToken cancellationToken, int readTimeoutSec);
    }
}