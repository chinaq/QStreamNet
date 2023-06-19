using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Contexts;

namespace QStreamNet.Test.Core.SteamApp.Apps
{
    [TestClass]
    public class StreamApplicationBuilderTest
    {
        [TestMethod]
        public void Do_App_Twice()
        {
            // arrange
            // init
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
            // act
            var app = builder.Build();
            var context = new StreamContext();
            app(context).Wait();
            app(context).Wait();
            // assert
            useCount.Should().Be(2);
            useEndCount.Should().Be(2);
            context.DataOut.Should().BeEquivalentTo(resp);
        }
        
    }
}