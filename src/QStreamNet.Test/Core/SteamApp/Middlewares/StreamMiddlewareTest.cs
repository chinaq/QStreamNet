using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Middlewares;
using TcpNet.Pipelines;

namespace QStreamNet.Test.Core.SteamApp.Middlewares
{
    [TestClass]
    public class StreamMiddlewareTest
    {
        [TestMethod]
        public async Task No_Milldeware_Factory()
        {
            var services = new ServiceCollection();
            var mockServiceProvider = services.BuildServiceProvider();
            var builder = new StreamApplicationBuilder(mockServiceProvider);
            // no factory
            builder.UseMiddleware<Middleware>();
            var app = builder.Build();
            // run
            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                var context = new StreamContext();
                var sp = mockServiceProvider.CreateScope().ServiceProvider;
                context.Services = sp;
                await app(context);
            });
        }



        [TestMethod]
        // public async Task Null_Middleware_in_Factory()
        public async Task Not_Add_In_Collection_Middleware_in_Factory_Still_Works()
        {
            var services = new ServiceCollection();
            // factory
            services.AddSingleton<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
            // builder
            var mockServiceProvider = services.BuildServiceProvider();
            var builder = new StreamApplicationBuilder(mockServiceProvider);
            // no factory
            builder.UseMiddleware<Middleware>();
            var app = builder.Build();
            // run it
            // var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            // {
                var context = new StreamContext();
                var sp = mockServiceProvider.CreateScope().ServiceProvider;
                context.Services = sp;
                await app(context);
            // });
            // exception.Message.Should().Contain("No service for type");

            context.Messages.Count.Should().Be(1);
            context.Messages.First().Should().BeEquivalentTo("middle it");
            // Assert.Fail();
        }


        [TestMethod]
        public async Task Do_Middleware()
        {
            var services = new ServiceCollection();
            // factory
            services.AddSingleton<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
            services.AddScoped<Middleware>();
            // builder
            var mockServiceProvider = services.BuildServiceProvider();
            var builder = new StreamApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware<Middleware>();
            // app
            var app = builder.Build();
            var context = new StreamContext();
            var sp = mockServiceProvider.CreateScope().ServiceProvider;
            context.Services = sp;
            await app(context);
            // assert
            context.Messages.Count.Should().Be(1);
            context.Messages.First().Should().BeEquivalentTo("middle it");
        }


        [TestMethod]
        public async Task Create_Middleware_with_Params()
        {
            var services = new ServiceCollection();
            // factory
            services.AddSingleton<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
            // services.AddSingleton<MiddlewareWithParams>();
            // builder
            var mockServiceProvider = services.BuildServiceProvider();
            var builder = new StreamApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware<MiddlewareWithParams>("this is cs50");
            builder.UseMiddleware<MiddlewareWithParams>("this is cs51");
            // app
            var app = builder.Build();
            var context = new StreamContext();
            var sp = mockServiceProvider.CreateScope().ServiceProvider;
            context.Services = sp;
            await app(context);
            // assert
            context.Messages.Count.Should().Be(2);
            context.Messages[0].Should().BeEquivalentTo("this is cs50");
            context.Messages[1].Should().BeEquivalentTo("this is cs51");
        }


        class Middleware : IStreamMiddleware
        {
            public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
            {
                context.Messages.Add("middle it");
                await next(context);
            }
        }


        class MiddlewareWithParams : IStreamMiddleware
        {
            private readonly string _whoami;

            public MiddlewareWithParams(string whoami)
            {
                _whoami = whoami;
            }

            public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
            {
                context.Messages.Add(_whoami);
                await next(context);
            }
        }
    }
}