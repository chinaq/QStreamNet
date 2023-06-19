using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.EndPoints;
using QStreamNet.Core.StreamApp.Middlewares;

namespace QStreamNet.Test.Core.SteamApp.Endpoints
{
    [TestClass]
    public class StreamEndpointRouteBuilderTest
    {
        internal const string EndpointRouteBuilderKey = "__EndpointRouteBuilder";


        [TestMethod]
        public async Task Set_Use_Routing()
        {
            var are = new AutoResetEvent(false);
            // Arrange
            var appBuilder = new StreamApplicationBuilder(Mock.Of<IServiceProvider>());
            appBuilder.UseRouting(req => req.Skip(1).Take(1).ToArray());
            appBuilder.UseEndpoints();
            var routeBuilder = (IStreamEndpointRouteBuilder)appBuilder.Properties[EndpointRouteBuilderKey]!;
            routeBuilder.Map(
                new byte[] { 0x01 },
                "name001",
                context => {
                    are.Set();
                    return Task.CompletedTask;
                }
            );
            var app = appBuilder.Build();
            // Act
            await app(new StreamContext() { DataIn = new byte[] {0x00, 0x01}});
            are.WaitOne(10).Should().BeTrue();
        }


        [TestMethod]
        public async Task Routing_Null()
        {
            var are = new AutoResetEvent(false);
            // Arrange
            var appBuilder = new StreamApplicationBuilder(Mock.Of<IServiceProvider>());
            appBuilder.UseRouting(req => req);
            appBuilder.UseEndpoints();
            appBuilder.Use((context, next) => {
                are.Set();
                return Task.CompletedTask;
            });
            var app = appBuilder.Build();
            // Act
            await app(new StreamContext() { DataIn = new byte[] {0x00, 0x01}});
            // assert
            are.WaitOne(10).Should().BeTrue();
        }


        [TestMethod]
        public async Task Routing_In_Middleware()
        {
            // Arrange
            // services
            var services = new ServiceCollection();
            services.AddSingleton<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
            services.AddScoped<Middleware>();
            // builder
            var serviceProvider = services.BuildServiceProvider();
            var appBuilder = new StreamApplicationBuilder(serviceProvider);
            appBuilder.UseRouting(req => req.Skip(1).Take(1).ToArray());
            appBuilder.UseEndpoints();
            var routeBuilder = (IStreamEndpointRouteBuilder)appBuilder.Properties[EndpointRouteBuilderKey]!;
            routeBuilder.Map(
                new byte[] { 0x01 },
                "name001",
                routeBuilder.CreateApplicationBuilder().UseMiddleware<Middleware>().Build()
            );
            var app = appBuilder.Build();
            // Act
            var context = new StreamContext() { DataIn = new byte[] {0x00, 0x01} };
            context.Services = serviceProvider.CreateScope().ServiceProvider;
            await app(context);
            // Assert
            context.Messages.Count.Should().Be(1);
            context.Messages.First().Should().BeEquivalentTo("middle it");
        }


        class Middleware : IStreamMiddleware
        {
            public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
            {
                context.Messages.Add("middle it");
                await Task.CompletedTask;
            }
        }
    }
}
