using Microsoft.Extensions.DependencyInjection;
using QStreamNet.Core.StreamApp;
using QStreamNet.Core.StreamApp.Contexts;
using QStreamNet.Core.StreamApp.Middlewares;

Console.WriteLine("Hello, QStream!");

// services 
var sc = new ServiceCollection();
sc.AddSingleton<IStreamMiddlewareFactory, StreamMiddlewareFactory>();
var services = sc.BuildServiceProvider();
// pipes
var builder = new StreamApplicationBuilder(services);
builder
    .UseMiddleware<SimpleMiddleware>()
    .Use(async (context, next) => { Console.WriteLine("Easy Middleware"); await next(context); })
    .UseEnd(async context => { Console.WriteLine("End Middleware"); await Task.CompletedTask; });
// app
var context = new StreamContext() { Services = services.CreateScope().ServiceProvider };
var app = builder.Build();
// run it
await app(context);


// a simple class
class SimpleMiddleware : IStreamMiddleware
{
    public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
    {
        Console.WriteLine("SimpleMiddleware");
        await next(context);
    }
}
