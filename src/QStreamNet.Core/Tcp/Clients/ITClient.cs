using System.Net.Sockets;
using QStreamNet.Core.StreamApp.Middlewares;

namespace QStreamNet.Core.Tcp.Clients
{
    public interface ITClient
    {
        bool Connected { get; }
        void Close();
        NetworkStream GetStream();
        Task ListeningAsync(StreamPipeDelegate app, CancellationToken cancellationToken, int readTimeoutSec);
    }
}