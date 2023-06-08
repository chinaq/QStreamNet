using System.IO.Ports;
using QStreamNet.Core.StreamApp.Contexts;

namespace QStreamNet.Core.SerialPorts
{
    public class SpClient : IStreamClient
    {
        private readonly SerialPort _serialPort;

        public int ReadTimeout { get; set; } = -1;
        public int WriteTimeout { get; set; } = -1;

        public SpClient(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void Open()
        {
            _serialPort.Open();
        }

        public async Task WriteAsync(byte[] data)
        {
            var stream = _serialPort.BaseStream;
            var timeout = TimeSpan.FromMilliseconds(WriteTimeout);
            await stream.WriteAsync(data).AsTask().WaitAsync(timeout);
        }

        public async Task<byte[]> ReadAsync(CancellationToken cancellationToken)
        {
            var stream = _serialPort.BaseStream;
            var bufferSize = _serialPort.ReadBufferSize;
            var timeout = TimeSpan.FromMilliseconds(ReadTimeout);
            // read
            byte[] readBuffer = new byte[bufferSize];
            var readSize = await stream.ReadAsync(readBuffer, 0, readBuffer.Length)
                .WaitAsync(timeout, cancellationToken);
            var result = new byte[readSize];
            Array.Copy(readBuffer, result, readSize);
            return result;
        }

        public String ReadLine()
        {
            _serialPort.ReadTimeout = ReadTimeout;
            return _serialPort.ReadLine();
        }

        public void WriteLine(string message)
        {
            _serialPort.WriteTimeout = WriteTimeout;
            _serialPort.WriteLine(message);
        }

        public async Task WriteLineAsync(string message)
        {
            // try {
                var stream = _serialPort.BaseStream;
                var timeout = TimeSpan.FromMilliseconds(WriteTimeout);
                var writerStream = new StreamWriter(stream);
                await writerStream.WriteLineAsync(message).WaitAsync(timeout);
                await writerStream.FlushAsync();
            // } catch (Exception e) {
            //     Console.WriteLine(e);
            //     throw;
            // }
        }

        public async Task<string?> ReadLineAsync(CancellationToken cancellationToken = default)
        {
            var stream = _serialPort.BaseStream;
            var readerStream = new StreamReader(stream);
            var timeout = TimeSpan.FromMilliseconds(ReadTimeout);
            string? result = await readerStream.ReadLineAsync().WaitAsync(timeout, cancellationToken);
            return result;
        }

    }
}
