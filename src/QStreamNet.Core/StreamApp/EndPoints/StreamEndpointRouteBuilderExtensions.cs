using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.EndPoints
{
    public static class StreamEndpointRouteBuilderExtensions
    {
        public static IStreamEndpointRouteBuilder Map(
            this IStreamEndpointRouteBuilder builder,
            byte[] cmd,
            string name,
            StreamPipeDelegate requestDelegate)
        {
            var endpointRouteBuilder = builder;
            if (endpointRouteBuilder is null){
                throw new InvalidOperationException($"{nameof(IStreamEndpointRouteBuilder)} is null.");
            }
            var endpoints = endpointRouteBuilder.DataSource.Endpoints;
            endpoints.Add(new StreamEndpoint(requestDelegate, cmd, name));
            return builder;
        }
    }
}
