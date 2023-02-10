using QStreamNet.Core.StreamApp;

namespace TcpNet.Pipelines
{
    public class TcpHostServiceOptions
    {
        public Action<IStreamApplicationBuilder>? ConfigureApplication { get; set; }
    }
}