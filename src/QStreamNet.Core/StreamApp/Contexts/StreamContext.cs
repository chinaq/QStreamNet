using Microsoft.Extensions.DependencyInjection;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.EndPoints;

namespace TcpNet.Pipelines
{
    public class StreamContext : IDisposable
    {
        public static class Statuses
        {
            public static readonly int Null_0 = 0;
            public static readonly int Ok_200 = 200;
            public static readonly int BadIn_400 = 400;
            public static readonly int NotFound_404 = 404;
            public static readonly int InternalError_500 = 500;
        }

        private IDictionary<Type, object> _properties;
        // public int ClientId { get; set; }
        public IServiceScope? Scope { get; set; }
        public IServiceProvider? Services { get; set; }
        public byte[]? DataIn { get; set; }
        public byte[]? DataOut { get; set; }
        public IStreamClient? StreamClient { get; set; }
        public int Status { get; set; }
        public StreamEndpoint? Endpoint { get; set; }
        public IList<string> Messages { get; private set; }

        public StreamContext()
        {
            Messages = new List<string>();
            _properties = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            Scope?.Dispose();
        }

        public TProperty Get<TProperty>()
        {
            return (TProperty)_properties[typeof(TProperty)];
        }

        public void Set<TProperty>(TProperty property)
        {
            _properties[typeof(TProperty)] = property!;
        }
    }
}
