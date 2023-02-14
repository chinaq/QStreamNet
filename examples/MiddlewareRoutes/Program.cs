// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using MiddlewareRoutes.Middlepoints;
using MiddlewareRoutes.Pipes;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.MiddlePoints;
using QStreamNet.Core.StreamApp.Middlewares;
using TcpNet.Pipelines;

Console.WriteLine("Hello, Middleware Routes!");

// set service
var sc = new ServiceCollection();
// sc.AddLogging(c => c.AddSerilog());
sc.AddSingleton<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
sc.AddScoped<TimeWriteMiddleware>();
sc.AddScoped<TimeReadMiddleware>();
sc.AddScoped<StreamOutMiddleware>();
sc.AddScoped<StreamInMiddleware>();
sc.AddScoped<TimeWriteCheckMiddleware>();
sc.AddScoped<TimeReadCheckMiddleware>();
var services = sc.BuildServiceProvider();

// pipes
var builder = new StreamApplicationBuilder(services);
builder
    .UseMiddleRouting(
        (context, point) => context.Get<OutPoint>().Name == point.Name,
        middleBuilder =>  middleBuilder
            // .Map<TimeWriteMiddleware>(TimeWriteMiddleware.CmdData, TimeWriteMiddleware.CmdName)
            // .Map<TimeReadMiddleware>(TimeReadMiddleware.CmdData, TimeReadMiddleware.CmdName)
            .Map<TimeWriteMiddleware>()
            .Map<TimeReadMiddleware>()
    )
    .UseMiddleware<StreamOutMiddleware>()
    .UseMiddleware<StreamInMiddleware>()
    .UseMiddleRouting(
        (context, point) => context.Get<InPoint>().Name == point.Name,
        middleBuilder => middleBuilder
            // .Map<TimeWriteCheckMiddleware>(TimeWriteCheckMiddleware.CmdData, TimeWriteCheckMiddleware.CmdName)
            // .Map<TimeReadCheckMiddleware>(TimeReadCheckMiddleware.CmdData, TimeReadCheckMiddleware.CmdName)
            .Map<TimeWriteCheckMiddleware>()
            .Map<TimeReadCheckMiddleware>()
    );
var app = builder.Build();



Console.WriteLine();
Console.WriteLine("### WRITE TIME ###");
// set context
var context = new StreamContext();
context.Services = services.CreateScope().ServiceProvider;
context.Set(new OutPoint {Name = TimeWriteMiddleware.CmdName});
context.Set(new InPoint {Name = TimeWriteCheckMiddleware.CmdName});
// run
await app(context);


Console.WriteLine();
Console.WriteLine("### READ TIME ###");
context = new StreamContext();
context.Services = services.CreateScope().ServiceProvider;
context.Set(new OutPoint {Name = TimeReadMiddleware.CmdName});
context.Set(new InPoint {Name = TimeReadCheckMiddleware.CmdName});
// run
await app(context);


Console.WriteLine();
Console.WriteLine("### UNKNOWN ###");
context = new StreamContext();
context.Services = services.CreateScope().ServiceProvider;
context.Set(new OutPoint {Name = TimeReadMiddleware.CmdName});
context.Set(new InPoint {Name = "UNKNOWN"});
// run
await app(context);
