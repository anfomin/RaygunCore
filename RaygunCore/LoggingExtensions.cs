using RaygunCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging;

/// <summary>
/// <see cref="ILoggingBuilder"/> extension methods for Raygun logger provider registration.
/// </summary>
public static class RaygunLoggingExtensions
{
	/// <summary>
	/// Register <see cref="RaygunLoggerProvider"/> and required Raygun services.
	/// </summary>
	/// <param name="builder">Action to configure Raygun services.</param>
	public static ILoggingBuilder AddRaygun(this ILoggingBuilder logging, Action<IRaygunBuilder> builder)
	{
		var b = logging.Services.AddRaygun();
		((RaygunBuilder)b).IsLogging = true;
		logging.Services.AddSingleton<ILoggerProvider, RaygunLoggerProvider>();
		builder(b);
		return logging;
	}
}