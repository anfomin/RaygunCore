using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RaygunCore.Services
{
	/// <summary>
	/// Creates <see cref="RaygunLogger"/>.
	/// </summary>
	public class RaygunLoggerProvider : ILoggerProvider
	{
		readonly IServiceProvider _services;

		public RaygunLoggerProvider(IServiceProvider services)
		{
			_services = services ?? throw new ArgumentNullException(nameof(services));
		}

		/// <inheritdoc/>
		public ILogger CreateLogger(string categoryName)
		{
			// using Lazy because IHttpClientFactory requires logger and DI fails with dependency recursion
			return new RaygunLogger(new Lazy<IRaygunClient>(() => _services.GetRequiredService<IRaygunClient>()));
		}

		/// <inheritdoc/>
		public void Dispose() { }
	}
}