using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public class StreamMiddlepoint
    {
        public string? Name { get; }
        public byte[]? Metadata { get; }
        public StreamPipeDelegate RequestDelegate { get; }


        public StreamMiddlepoint(
            StreamPipeDelegate requestDelegate,
            byte[]? metadata,
            string? name)
        {
            RequestDelegate = requestDelegate;
            Metadata = metadata;
            Name = name;
        }
    }
}
