namespace QStreamNet.Core.StreamApp.Contexts
{
    public interface IStreamClient
    {
        Task WriteAsync(byte[] data);
        Task<byte[]> ReadAsync(CancellationToken cancellationToken = default);

        void WriteLine(string message);
        String ReadLine();

        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        void Open();
        void Close();
    }
}