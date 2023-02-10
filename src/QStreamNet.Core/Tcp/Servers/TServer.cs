using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.Tcp.Servers;
using TcpNet.Pipelines;

namespace TcpNet.Tcps
{
    public class TServer : ITServer
    {
        public int PortNum { get; }

        private int _limitConnections;
        private int _readTimeoutSec;
        private TcpListener _tcpListener;
        private IList<ITClient> _tClients;
        private IStreamContextFactory _contextFactory;
        private ILoggerFactory _loggerFactory;
        private ILogger<TServer> _logger;

        public TServer(
            TPort tPort,
            IStreamContextFactory streamContextFactory,
            ILoggerFactory loggerFactory)
        {
            PortNum = tPort.Num;
            _limitConnections = tPort.LimitConnections;
            _readTimeoutSec = tPort.ReadTimeoutSec;
            _tcpListener = new TcpListener(IPAddress.Any, tPort.Num);
            _tClients = new List<ITClient>();
            _contextFactory = streamContextFactory;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TServer>();
            _logger.LogInformation($"TPort: {JsonSerializer.Serialize(tPort)}");
        }


        public async Task StartAsync(StreamPipeDelegate application, CancellationToken cancellationToken)
        {
            try {
                _tcpListener.Start();
                _logger.LogInformation($"Listening on {_tcpListener.LocalEndpoint}");
                while (!cancellationToken.IsCancellationRequested) {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
                    _logger.LogDebug($"new tcp client {tcpClient.GetHashCode()} connected");
                    // check & clean clients
                    lock(_tClients) {
                        if (_limitConnections > 0 && _tClients.Count >= _limitConnections) {
                            for (int i = _tClients.Count - 1; i >= 0; i--) {
                                if (_tClients[i] is null || _tClients[i].Connected == false) {
                                    _tClients.RemoveAt(i);
                                }
                            }
                            _logger.LogInformation($"cleaned tcp client list on count: {_tClients.Count}");
                        }
                        // check again
                        if (_limitConnections > 0 && _tClients.Count >= _limitConnections) {
                            tcpClient.Close();
                            _logger.LogDebug($"new tcp client {tcpClient.GetHashCode()} closed for list maxed {_tClients.Count}");
                            continue;
                        }
                    }
                    // client listening
                    var tClient = new TClient(tcpClient, _tClients, _contextFactory, _loggerFactory.CreateLogger<TClient>());
                    var task = tClient.ListeningAsync(application, cancellationToken, _readTimeoutSec);
                }
            } catch (OperationCanceledException e) {
                _logger.LogInformation(e.Message);  
            } catch (Exception e) {  
                _logger.LogInformation(e.ToString());  
                throw;
            }  
        }


        public void Stop()
        {
            _tcpListener.Stop();
        }
    }
}