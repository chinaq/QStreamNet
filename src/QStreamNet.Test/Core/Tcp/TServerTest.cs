using System.Net.Sockets;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;
using QStreamNet.Core.Tcp.Servers;

namespace QStreamNet.Test.Core.Tcp
{
    [TestClass]
    public class TServerTest
    {
        [TestMethod]
        public void ResponseData_Once()
        {
            var server = new TServer(
                new TPort() { Num = 667 },
                // new TClientFactory(Mock.Of<IStreamContextFactory>(), 
                new StreamContextFactory(new ServiceCollection().BuildServiceProvider()),
                new NullLoggerFactory()
            );
            // await server.StartAsync(null, new CancellationToken());
            var respData = new byte[] { 0x01 };
            StreamPipeDelegate app = async context => {
                context.DataOut = respData;
                await Task.CompletedTask;
            };
            var task = server.StartAsync(app, new CancellationToken());
            var tcpClient = new TcpClient("127.0.0.1", server.PortNum);
            var ns = tcpClient.GetStream();
            // write
            var reqData = new byte[] { 0x01 };
            ns.Write(reqData);
            // rev
            var buffer = new byte[8];
            var len = ns.Read(buffer, 0, buffer.Length);
            var result = new byte[len];
            Array.Copy(buffer, result, len);
            // assert
            result.Should().BeEquivalentTo(respData);
            server.Stop();
        }



        [TestMethod]
        public void ResponseData_Twice()
        {
            var server = new TServer(
                new TPort() { Num = 667 },
                new StreamContextFactory(new ServiceCollection().BuildServiceProvider()),
                new NullLoggerFactory()
            );
            var respData = new byte[] { 0x01 };
            StreamPipeDelegate app = async context => {
                context.DataOut = respData;
                await Task.CompletedTask;
            };
            var task = server.StartAsync(app, new CancellationToken());
            var tcpClient = new TcpClient("127.0.0.1", server.PortNum);
            var ns = tcpClient.GetStream();
            // twice
            for(int i = 0; i < 2; i++){
                var buffer = new byte[8];

                // write
                var reqData0 = new byte[] { 0x01, 0x02 };
                ns.Write(reqData0);
                // rev
                var len0 = ns.Read(buffer, 0, buffer.Length);
                var result0 = new byte[len0];
                Array.Copy(buffer, result0, len0);
                // assert
                result0.Should().BeEquivalentTo(respData);
            }
            server.Stop();
        }


        [TestMethod]
        public void ResponseData_Twice_By_AppBuilder()
        {
            // arrange
            // Set app            
            int useCount = 0;
            int useEndCount = 0;
            var resp = new byte[] { 0x01 };
            // builder
            var services = new ServiceCollection().BuildServiceProvider();
            var builder = new StreamApplicationBuilder(services);
            builder.Use(async (context, next) => {
                useCount++;
                await next(context);
            });
            builder.UseEnd(context => {
                useEndCount++;
                context.DataOut = resp;
                return Task.CompletedTask;
            });
            var app = builder.Build();

            // server
            var server = new TServer(
                new TPort() { Num = 667 },
                new StreamContextFactory(new ServiceCollection().BuildServiceProvider()),
                new NullLoggerFactory()
            );
            // await server.StartAsync(null, new CancellationToken());
            var task = server.StartAsync(app, new CancellationToken());
            var tcpClient = new TcpClient("127.0.0.1", server.PortNum);
            var ns = tcpClient.GetStream();
            // twice
            for(int i = 0; i < 2; i++){
                var buffer = new byte[8];
                // write
                var reqData0 = new byte[] { 0x01, 0x02 };
                ns.Write(reqData0);
                // rev
                var len0 = ns.Read(buffer, 0, buffer.Length);
                var result0 = new byte[len0];
                Array.Copy(buffer, result0, len0);
                // assert
                result0.Should().BeEquivalentTo(resp);
            }
            server.Stop();
            // assert
            useCount.Should().Be(2);
            useEndCount.Should().Be(2);
        }


        [TestMethod]
        public void Limited_Connectionst_Only_One()
        {
            // Arrange
            var server = new TServer(
                new TPort() { Num = 667, ReadTimeoutSec = 3, LimitConnections = 1 },
                new StreamContextFactory(new ServiceCollection().BuildServiceProvider()),
                new NullLoggerFactory()
            );
            var respData = new byte[] { 0x01 };
            StreamPipeDelegate app = async context => {
                context.DataOut = respData;
                await Task.CompletedTask;
            };
            // Act
            var task = server.StartAsync(app, new CancellationToken());
            using var tcpClient0 = new TcpClient("127.0.0.1", server.PortNum);
            using var tcpClient1 = new TcpClient("127.0.0.1", server.PortNum);
            Thread.Sleep(300);
            // do it
            tcpClient0.GetStream().WriteByte(0x00);
            tcpClient1.GetStream().WriteByte(0x01);
            // Assert
            // try again
            tcpClient0.GetStream().WriteByte(0x00);
            tcpClient1.Invoking(t1 => t1.GetStream().WriteByte(0x01)).Should().Throw<IOException>();
            // clean
            server.Stop();
        }


        [TestMethod]
        public void Limited_Connections_If_Current_Closed_Next_GetTheChance_To_Connect()
        {
            // Arrange
            var server = new TServer(
                new TPort() { Num = 667, ReadTimeoutSec = 3, LimitConnections = 1 },
                new StreamContextFactory(new ServiceCollection().BuildServiceProvider()),
                new NullLoggerFactory()
            );
            var respData = new byte[] { 0x01 };
            StreamPipeDelegate app = async context => {
                context.DataOut = respData;
                await Task.CompletedTask;
            };
            // Act
            var task = server.StartAsync(app, new CancellationToken());
            // do it
            using var tcpClient0 = new TcpClient("127.0.0.1", server.PortNum);
            Thread.Sleep(100);
            tcpClient0.GetStream().WriteByte(0x00);

            // client 1
            using (var tcpClient1 = new TcpClient("127.0.0.1", server.PortNum)) {
                Thread.Sleep(100);
                tcpClient1.GetStream().WriteByte(0x01);
                Thread.Sleep(300);
                tcpClient1.Invoking(t1 => t1.GetStream().WriteByte(0x01)).Should().Throw<IOException>();
            }

            Thread.Sleep(100);
            tcpClient0.GetStream().WriteByte(0x00);
            tcpClient0.Close();

            // client 2
            Thread.Sleep(100);
            using (var tcpClient2 = new TcpClient("127.0.0.1", server.PortNum)) {
                // new conncetion worked
                Thread.Sleep(100);
                tcpClient2.GetStream().WriteByte(0x01);
                Thread.Sleep(300);
                tcpClient2.GetStream().WriteByte(0x01);
            }
            // clean
            server.Stop();
        }


    }
}