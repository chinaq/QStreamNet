using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.EndPoints
{
    public class StreamEndpoint
    {
        public string? Name { get; }
        public byte[] Metadata { get; }
        public StreamPipeDelegate? RequestDelegate { get; }


        public StreamEndpoint(
            StreamPipeDelegate? requestDelegate,
            byte[] metadata,
            string? name)
        {
            RequestDelegate = requestDelegate;
            Metadata = metadata;
            Name = name;
        }


    }
}
