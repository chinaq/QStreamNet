using System.IO.Ports;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using QStreamNet.Core.SerialPorts;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.Util;

namespace QStreamNet.Test.Core.Spclients
{
    [TestClass]
    public class SpClientTest
    {
        private readonly string _port0;
        private readonly string _port1;
        private readonly QDataHandler _dh;

        public SpClientTest()
        {
            var path = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.Development.json")
                .Build();
            _port0 = configuration["Port0"];
            _port1 = configuration["Port1"];
            _dh = new QDataHandler();    
        }

        [TestMethod]
        public async Task Write_Read()
        {
            // Arrange
            // ports
            var port0 = new SerialPort(_port0);
            var port1 = new SerialPort(_port1);
            port0.Open(); port0.BaseStream.Flush(); port0.Close();
            port1.Open(); port1.BaseStream.Flush(); port1.Close();
            // client 0
            IStreamClient client0 = new SpClient(port0);
            client0.Open();
            client0.ReadTimeout = 100;
            client0.WriteTimeout = 100;
            // client 1
            IStreamClient client1 = new SpClient(port1);
            client1.Open();
            client1.ReadTimeout = 100;
            client1.WriteTimeout = 100;

            // Act
            await client0.WriteAsync(new byte [] {0x00, 0x01});
            var resultC1 = await client1.ReadAsync();
            await client1.WriteAsync(new byte[] {0x02, 0x03});
            var task = client0.ReadAsync();
            var resultC0 = await task;

            // clear
            client0.Close();
            client1.Close();

            // Assert
            resultC1.Should().BeEquivalentTo(new byte[] {0x00, 0x01});
            resultC0.Should().BeEquivalentTo(new byte[] {0x02, 0x03});
        }


        [TestMethod]
        public void ReadLine_Timeout()
        {
            // Arrange
            // ports
            var port0 = new SerialPort(_port0);
            port0.Open(); port0.BaseStream.Flush(); port0.Close();
            // client 0
            IStreamClient client0 = new SpClient(port0);
            client0.Open();
            client0.ReadTimeout = 100;

            // Act
            // Assert
            client0.Invoking(c => c.ReadLine()).Should().Throw<TimeoutException>();
            // clear
            client0.Close();
        }

        [TestMethod]
        public async Task WriteLine_ReadLine_Sync()
        {
            // Arrange
            // ports
            var port0 = new SerialPort(_port0);
            var port1 = new SerialPort(_port1);
            port0.Open(); port0.BaseStream.Flush(); port0.Close();
            port1.Open(); port1.BaseStream.Flush(); port1.Close();
            // client 0
            IStreamClient client0 = new SpClient(port0);
            client0.Open();
            client0.ReadTimeout = 100;
            client0.WriteTimeout = 100;
            // client 1
            IStreamClient client1 = new SpClient(port1);
            client1.Open();
            client1.ReadTimeout = 100;
            client1.WriteTimeout = 100;

            // Act
            client0.WriteLine("Hello");
            var resultC1 = client1.ReadLine();
            await client1.WriteAsync(_dh.StrCharsToBytes("World\n"));
            var resultC0 = client0.ReadLine();

            // clear
            client0.Close();
            client1.Close();

            // Assert
            resultC1.Should().BeEquivalentTo("Hello");
            resultC0.Should().BeEquivalentTo("World");
        }


        [TestMethod, Timeout(1000)]
        public async Task WriteLine_ReadLine_Async()
        {
            // Arrange
            // ports
            var port0 = new SerialPort(_port0);
            var port1 = new SerialPort(_port1);
            port0.Open(); port0.BaseStream.Flush(); port0.Close();
            port1.Open(); port1.BaseStream.Flush(); port1.Close();
            // client 0
            IStreamClient client0 = new SpClient(port0);
            client0.Open();
            client0.ReadTimeout = 200;
            client0.WriteTimeout = 100;
            // client 1
            IStreamClient client1 = new SpClient(port1);
            client1.Open();
            client1.ReadTimeout = 200;
            client1.WriteTimeout = 100;

            // Act
            await client1.WriteAsync(_dh.StrCharsToBytes("World\n"));
            var resultC0 = await client0.ReadLineAsync();

            await client0.WriteLineAsync("Hello");
            var resultC1 = await client1.ReadLineAsync();

            // clear
            client0.Close();
            client1.Close();

            // Assert
            resultC1.Should().BeEquivalentTo("Hello");
            resultC0.Should().BeEquivalentTo("World");
            // Assert.Fail();
        }
    }
}
