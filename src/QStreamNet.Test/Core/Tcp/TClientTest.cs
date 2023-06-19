using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.Tcp.Clients;

namespace QStreamNet.Test.Core.Tcp
{
    [TestClass]
    public class TClientTest
    {
        [TestMethod]
        public void ListeningAsync_Check_Request()
        {
            var are = new AutoResetEvent(false);
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 6666);
            tcpListener.Start();
            // act
            // server
            Task taskTCli = Task.Run(() =>
            {
                TcpClient tcpCli = tcpListener.AcceptTcpClient();
                var tCli = new TClient(tcpCli,
                    new List<ITClient>(),
                    new StreamContextFactory(new ServiceCollection().BuildServiceProvider())
                );
                // var result = tCli.Rev();
                var task = tCli.ListeningAsync(async context => {
                    // assert
                    context.DataIn.Should().BeEquivalentTo(new byte[] { 0x11, 0x22 });
                    are.Set();
                    await Task.CompletedTask;
                }, new CancellationToken());
            });
            // client
            TcpClient tcpSend = new TcpClient("127.0.0.1", 6666);
            NetworkStream ns = tcpSend.GetStream();
            var send = new byte[] { 0x11, 0x22 };
            ns.Write(send, 0, send.Length);
            // wait for
            are.WaitOne(500).Should().BeTrue();
            tcpSend.Close();
            ns.Close();
            tcpListener.Stop();
        }


        [TestMethod]
        public void ListeningAsync_Check_Response()
        {
            var are = new AutoResetEvent(false);
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 6666);
            tcpListener.Start();
            // act
            // server
            Task taskTCli = Task.Run(() => {
                TcpClient tcpCli = tcpListener.AcceptTcpClient();
                // var tCli = new TClient(tcpCli, Mock.Of<IStreamContextFactory>());
                var tCli = new TClient(tcpCli,
                    new List<ITClient>(),
                    new StreamContextFactory(new ServiceCollection().BuildServiceProvider())
                );
                var task = tCli.ListeningAsync(async context => {
                    // assert
                    context.DataOut = new byte[] { 0x03, 0x04 };
                    are.Set();
                    await Task.CompletedTask;
                }, new CancellationToken());
            });
            // client
            TcpClient tcpSend = new TcpClient("127.0.0.1", 6666);
            NetworkStream ns = tcpSend.GetStream();
            // send
            var send = new byte[] { 0x01 };
            ns.Write(send, 0, send.Length);
            // rev
            var revBuffer = new byte[8];
            var revLen = ns.Read(revBuffer, 0, revBuffer.Length);
            var result = new byte[revLen];
            Array.Copy(revBuffer, result, revLen);
            result.Should().BeEquivalentTo(new byte[] { 0x03, 0x04 });
            // wait for
            are.WaitOne(500).Should().BeTrue();
            tcpSend.Close();
            ns.Close();
            tcpListener.Stop();
        }
    }
}
