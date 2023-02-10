using System.Net.Sockets;
using QStreamNet.Core.StreamApp.Contexts;
using TcpNet.Tcps;

namespace QStreamNet.Core.Tcp.Servers
{
    // public class TClientFactory : ITClientFactory
    // {
    //     private IStreamContextFactory _contextFactory;
    //     private ILoggerFactory _loggerFactory;

    //     public TClientFactory(IStreamContextFactory contextFactory, ILoggerFactory loggerFactory)
    //     {
    //         _contextFactory = contextFactory;
    //         _loggerFactory = loggerFactory;
    //     }
    //     public ITClient CreateOn(TcpClient tcpClient)
    //     {
    //         var client = new TClient(tcpClient, _contextFactory, _loggerFactory.CreateLogger<TClient>());
    //         return client;
    //     }
    // }
}