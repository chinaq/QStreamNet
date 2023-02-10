using System.IO.Ports;
using QStreamNet.Core.StreamApp.Contexts;

namespace QStreamNet.Core.SerialPorts
{
    public class SpClient : IStreamClient
    {
        private SerialPort _serialPort;

        // public int ReadInterval { get; set; } = 10;
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
            // var readSize = 0;
            // var readSize = await stream.ReadAsync(readBuffer, 0, readBuffer.Length, cancellationToken).WaitAsync(timeout);
            var readSize = await stream.ReadAsync(readBuffer, 0, readBuffer.Length).WaitAsync(timeout, cancellationToken);
            var result = new byte[readSize];
            Array.Copy(readBuffer, result, readSize);
            return result;
        }
    }
}
