# QStreamNet

QStreamNet is a dotnet library for streaming data though pipelines. It is inspired by ASP.NET Core's middleware pipeline. It is a simple, lightweight and fast library that allows you to build a pipeline for processing data. It is also flexible enough to allow you to build a pipeline with a single step.

## Installation
``` bash
dotnet add package QStreamNet
```

## Features
### Stream Client
It works with Stream Clients like 
- TCP
- UDP
- Serial Port 

It can be used to build a pipeline for processing data.

## Piplines
It works with
- Services
- Middlewares
- Routes

## Server & Client
It works on both 
- server side
- client side

## Usage
pipeline example:
``` cs
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

// OUTPUT:
//
// Hello, QStream!
// SimpleMiddleware
// Easy Middleware
// End Middleware


// a simple class
class SimpleMiddleware : IStreamMiddleware
{
    public async Task InvokeAsync(StreamContext context, StreamPipeDelegate next)
    {
        Console.WriteLine("SimpleMiddleware");
        await next(context);
    }
}
```