namespace QStreamNet.Core.StreamApp.Contexts
{
    public interface IStreamClient
    {
        Task WriteAsync(byte[] data);
        Task<byte[]> ReadAsync(CancellationToken cancellationToken = default);

        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        void Open();
        void Close();
    }
}