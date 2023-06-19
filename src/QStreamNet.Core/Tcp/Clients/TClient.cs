using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nito.AsyncEx;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;
using QStreamNet.Core.Util;

namespace QStreamNet.Core.Tcp.Clients
{
    public class TClient : ITClient
    {

        private class CallbackArgs
        {
            public NetworkStream Stream { get; }
            public StreamPipeDelegate StreamApplication { get; }
            public byte[] Buffer { get; }
            public AsyncAutoResetEvent AsyncAutoResetEvent { get; }

            public CallbackArgs(NetworkStream stream, StreamPipeDelegate application)
            {
                Stream = stream;
                StreamApplication = application;
                Buffer = new byte[1024];
                AsyncAutoResetEvent = new AsyncAutoResetEvent(false);
            }
        }


        private TcpClient _tcpClient;
        private IList<ITClient> _tClients;
        private NetworkStream _netStream;
        private IStreamContextFactory _contextFactory;
        private ILogger<TClient> _logger;
        private QDataHandler _dh;

        public bool Connected => _tcpClient.Connected;

        public TClient(TcpClient tcpClient, IList<ITClient> tClients, IStreamContextFactory contextFactory, ILogger<TClient>? logger = null)
        {
            _logger = logger?? new NullLogger<TClient>();
            _tcpClient = tcpClient;
            _tClients = tClients;
            lock(_tClients){
                _tClients.Add(this);
                _logger.LogInformation($"new tcp client {tcpClient.GetHashCode()} added to list on count: {_tClients.Count}");
            }
            _netStream = _tcpClient.GetStream();
            _contextFactory = contextFactory;
            _dh = new QDataHandler();
        }

        public void Close()
        {
            // _netStream?.Close();
            _tcpClient?.Close();
            lock(_tClients){
                _tClients.Remove(this);
                _logger.LogInformation($"tcp client {_tcpClient?.GetHashCode()} removed from list on count: {_tClients.Count}");
            }
        }

        public NetworkStream GetStream()
        {
            return _tcpClient.GetStream();
        }

        public async Task ListeningAsync(StreamPipeDelegate app, CancellationToken cancellationToken, int RecTimeoutSec = 0)
        {
            while (_tcpClient.Connected && _netStream.CanRead && !cancellationToken.IsCancellationRequested) {
                var args = new CallbackArgs(_netStream, app);
                // begin read
                _netStream.BeginRead(args.Buffer, 0, args.Buffer.Length,
                    new AsyncCallback(ReadCallBack),
                    args);

                if (RecTimeoutSec > 0) {
                    await ReadEndOrTimeout(args.AsyncAutoResetEvent, cancellationToken, RecTimeoutSec);
                } else {
                    await args.AsyncAutoResetEvent.WaitAsync(cancellationToken);
                }
            }
            if (_tcpClient.Connected) {
                Close();
                _logger.LogInformation($"tcpclient {_tcpClient.GetHashCode()} closed on Listening stopped");
            }
        }

        private async Task ReadEndOrTimeout(AsyncAutoResetEvent asyncAutoResetEvent, CancellationToken cancellationToken, int recTimeoutSec)
        {
            Task delayTask = Task.Delay(recTimeoutSec * 1000);
            Task readTask = asyncAutoResetEvent.WaitAsync(cancellationToken);
            Task task = await Task.WhenAny(delayTask, readTask);
            if (task == delayTask) {
                Close();
                _logger.LogInformation($"tcpclient {_tcpClient.GetHashCode()} closed on read timeout");
            }
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            CallbackArgs args = (CallbackArgs)ar.AsyncState!;
            NetworkStream networkStream = args.Stream;
            byte[] receiveBuffer = args.Buffer;
            int bytesReceived = 0;

            // read
            try {
                bytesReceived = networkStream.EndRead(ar);
            } catch (ObjectDisposedException) {
                _logger.LogInformation($"tcpclient {_tcpClient.GetHashCode()} EndRead Timeout.");
                args.AsyncAutoResetEvent.Set();
                return;
            } catch (Exception e) {
                _logger.LogInformation($"tcpclient {_tcpClient.GetHashCode()} EndRead error: {e.ToString()}");
                args.AsyncAutoResetEvent.Set();
                return;
            }

            // if closed
            if (bytesReceived <= 0) {
                Close();
                _logger.LogInformation($"tcp client {_tcpClient.GetHashCode()} closed on byte rev 0");
                args.AsyncAutoResetEvent.Set();
                return;
            }

            var start = DateTime.Now;
            // set request to context
            var data = new byte[bytesReceived];
            Array.Copy(receiveBuffer, data, bytesReceived);
            var context = _contextFactory.Create();
            context.DataIn = data;
            context.Set<ITClient>(this);
            _logger.LogInformation(context.GetHashCode(), $"tcp client {_tcpClient.GetHashCode()} requested: {_dh.BytesToStrHex(data)}");

            // run app
            try {
                args.StreamApplication(context).Wait();
            } catch (Exception e) {
                _logger.LogError(context.GetHashCode(), $"tcp client {_tcpClient.GetHashCode()} on app running error: {e.ToString()}");
                context?.Dispose();
                return;
            }

            // response
            try {
                if (context.DataOut is null || context.DataOut.Length < 1) {
                    _logger.LogInformation(context.GetHashCode(), $"tcp client {_tcpClient.GetHashCode()} did not response");
                } else {
                    _logger.LogInformation(context.GetHashCode(), $"tcp client {_tcpClient.GetHashCode()} to response: {_dh.BytesToStrHex(context.DataOut!)}");
                    networkStream.WriteAsync(context.DataOut).GetAwaiter().GetResult();
                    _logger.LogDebug(context.GetHashCode(), $"tcp client {_tcpClient.GetHashCode()} responsed");
                }
            } catch (Exception e) {
                _logger.LogError(context.GetHashCode(), $"tcp client {_tcpClient.GetHashCode()} on response error: {e.ToString()}");
            }
            context?.Dispose();
            
            var end = DateTime.Now;
            var roundDiff = end.Subtract(start).TotalSeconds;
            _logger.LogInformation($"tcp client {_tcpClient.GetHashCode()} req at {start.ToString("HH:mm:ss")}, res at {end.ToString("HH:mm:ss")}, used {string.Format("{0:00.000}",roundDiff)} sec");

            // has recevied
            args.AsyncAutoResetEvent.Set();
        }

        public override int GetHashCode()
        {
            return _tcpClient.GetHashCode();
        }
    }
}
