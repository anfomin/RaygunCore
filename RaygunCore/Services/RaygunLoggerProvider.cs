using System;
using Microsoft.Extensions.Logging;

namespace RaygunCore.Services
{
	/// <summary>
	/// Creates <see cref="RaygunLogger"/>.
	/// </summary>
	public class RaygunLoggerProvider : ILoggerProvider
	{
		readonly IRaygunClient _client;

		public RaygunLoggerProvider(IRaygunClient client)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}

		/// <inheritdoc/>
		public ILogger CreateLogger(string categoryName)
		{
			return new RaygunLogger(_client);
		}

		/// <inheritdoc/>
		public void Dispose() { }
	}
}