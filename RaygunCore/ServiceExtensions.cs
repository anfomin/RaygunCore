using RaygunCore;
using RaygunCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods for Raygun services registration.
/// </summary>
public static class RaygunServiceExtensions
{
	/// <summary>
	/// Register Raygun main services.
	/// </summary>
	public static IRaygunBuilder AddRaygun(this IServiceCollection services)
	{
		services.AddOptions();
		services.AddHttpClient();
		services.TryAddTransient<IRaygunClient, DefaultRaygunClient>();
		services.TryAddTransient<IRaygunMessageBuilder, DefaultRaygunMessageBuilder>();
		services.TryAddEnumerable(ServiceDescriptor.Transient<IRaygunMessageProvider, MainMessageProvider>());
		services.TryAddEnumerable(ServiceDescriptor.Transient<IRaygunMessageProvider, EnvironmentMessageProvider>());
		services.AddSingleton<IHostedService, RaygunLoggerWaiter>();
		return new RaygunBuilder(services);
	}

	/// <summary>
	/// Register Raygun main services.
	/// </summary>
	/// <param name="apiKey">Raygun API key.</param>
	public static IRaygunBuilder AddRaygun(this IServiceCollection services, string apiKey)
		=> services.AddRaygun().Configure(opt => opt.ApiKey = apiKey);

	/// <summary>
	/// Register Raygun main services.
	/// </summary>
	/// <param name="options">Action to configure Raygun options.</param>
	public static IRaygunBuilder AddRaygun(this IServiceCollection services, Action<RaygunOptions> options)
		=> services.AddRaygun().Configure(options);

	/// <summary>
	/// Register Raygun main services.
	/// </summary>
	/// <param name="configuration">Raygun options configuration section.</param>
	public static IRaygunBuilder AddRaygun(this IServiceCollection services, IConfiguration configuration)
		=> services.AddRaygun().Configure(configuration);

	/// <summary>
	/// Congifures Raygun options.
	/// </summary>
	/// <param name="options">Action to configure Raygun options.</param>
	public static IRaygunBuilder Configure(this IRaygunBuilder builder, Action<RaygunOptions> options)
	{
		builder.Services.Configure(options);
		return builder;
	}

	/// <summary>
	/// Congifures Raygun options.
	/// </summary>
	/// <param name="configuration">Raygun options configuration section.</param>
	public static IRaygunBuilder Configure(this IRaygunBuilder builder, IConfiguration configuration)
	{
		builder.Services.Configure<RaygunOptions>(configuration);
		return builder;
	}
}