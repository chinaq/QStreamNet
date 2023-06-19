using QStreamNet.Core.StreamApp;

namespace QStreamNet.Core.Tcp.Hosts
{
    public class TcpHostServiceOptions
    {
        public Action<IStreamApplicationBuilder>? ConfigureApplication { get; set; }
    }
}