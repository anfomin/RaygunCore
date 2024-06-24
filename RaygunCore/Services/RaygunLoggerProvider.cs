using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RaygunCore.Services;

/// <summary>
/// Creates <see cref="RaygunLogger"/>.
/// </summary>
public class RaygunLoggerProvider(IServiceProvider services) : ILoggerProvider
{
	// using Lazy because IHttpClientFactory requires logger and DI fails with dependency recursion
	readonly Lazy<IRaygunClient> _clientLazy = new(() => services.GetRequiredService<IRaygunClient>());

	/// <inheritdoc/>
	public ILogger CreateLogger(string categoryName)
		=> new RaygunLogger(_clientLazy, categoryName);

	/// <inheritdoc/>
	public void Dispose() { }
}