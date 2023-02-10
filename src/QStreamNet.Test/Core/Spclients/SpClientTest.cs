using System.IO.Ports;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using QStreamNet.Core.SerialPorts;
using QStreamNet.Core.StreamApp.Contexts;

namespace QStreamNet.Test.Core.Spclients
{
    [TestClass]
    public class SpClientTest
    {
        private string _port0;
        private string _port1;

        public SpClientTest()
        {
            var path = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.Development.json")
                .Build();
            _port0 = configuration["Port0"];
            _port1 = configuration["Port1"];
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
            var task = client0.ReadAsync();
            await client1.WriteAsync(new byte[] {0x02, 0x03});
            var resultC0 = await task;

            // clear
            client0.Close();
            client1.Close();

            // Assert
            resultC1.Should().BeEquivalentTo(new byte[] {0x00, 0x01});
            resultC0.Should().BeEquivalentTo(new byte[] {0x02, 0x03});
        }
    }
}
