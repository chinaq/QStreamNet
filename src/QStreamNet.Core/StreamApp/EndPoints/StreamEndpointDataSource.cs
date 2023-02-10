namespace QStreamNet.Core.StreamApp.EndPoints
{
    public class StreamEndpointDataSource
    {
        public ICollection<StreamEndpoint> Endpoints { get; }

        public StreamEndpointDataSource()
        {
            Endpoints = new List<StreamEndpoint>();
        }
    }
}