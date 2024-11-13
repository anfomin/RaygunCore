# Raygun provider for .NET

Standart [Raygun4Net library](https://github.com/MindscapeHQ/raygun4net) does not work well with .NET. So I've created library for new architecture:

- Built with interfaces and works via default dependency injection;
- As result, services can be easily extended or replaced;
- You can work via `IRaygunClient` directly or use `RaygunLogger` or intergrate handler into ASP.NET Core pipeline.

## Installation via NuGet

Package               | Description                               | NuGet
----------------------|-------------------------------------------|-------
RaygunCore            | Raygun client and logger                  | [![NuGet](https://img.shields.io/nuget/v/RaygunCore.svg)](https://www.nuget.org/packages/RaygunCore)
RaygunCore.AspNetCore | Handler for ASP.NET Core request pipeline | [![NuGet](https://img.shields.io/nuget/v/RaygunCore.AspNetCore.svg)](https://www.nuget.org/packages/RaygunCore.AspNetCore)

## How to use with ASP.NET Core

Register services in `Startup` class:

```C#
public void ConfigureServices(IServiceCollection services)
{
    // configure with API KEY
    services.AddRaygun("_API_KEY_")
        .WithHttp();

    // or with options
    services.AddRaygun(opt => opt.ApiKey = "_API_KEY_")
        .WithHttp();

    // or with configuration section
    services.AddRaygun(configuration)
        .WithHttp();
}
```

Method `AddRaygun()` registers only minimal required services. So then you can request `IRaygunClient` service and send errors:

```C#
public async Task<string> ActionInController([FromServices]IRaygunClient raygun)
{
    try
    {
        // some code
    }
    catch (Exception ex)
    {
        await raygun.SendAsync(ex);
    }
    return "OK";
}
```

Method `WithHttp()` in application services registration adds pipeline handler so any exception in request is automatically sent to Raygun.

## How to use with logging

You can register Raygun logger provider:

```C#
WebHost.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .ConfigureLogging((context, logging) => logging
        .AddRaygun(r => r
            .Configure(opt => opt.ApiKey = "_API_KEY_")
            .WithHttp()
        )
    );
```

Then just use logger:
```C#
public async Task<string> ActionInController([FromServices]ILogger<MyController> logger)
{
    logger.LogError(0, "My error message");
}
```

## License

This package has MIT license. Refer to the [LICENSE](LICENSE) for detailed information.
