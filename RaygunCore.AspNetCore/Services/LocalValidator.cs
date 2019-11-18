using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace RaygunCore.Services
{
	/// <summary>
	/// Rejects errors for local requests if <see cref="RaygunOptions.IgnoreLocalErrors"/>.
	/// </summary>
	public class LocalValidator : IRaygunValidator
	{
		readonly IHttpContextAccessor _httpContextAccessor;
		readonly RaygunOptions _options;

		public LocalValidator(IHttpContextAccessor httpContextAccessor, IOptions<RaygunOptions> options)
		{
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_options = options.Value;
		}

		/// <inheritdoc/>
		public bool ShouldSend(string message, Exception? ex)
		{
			var context = _httpContextAccessor.HttpContext;
			return context == null || !_options.IgnoreLocalErrors || !context.Request.IsLocal();
		}
	}

	internal static class HttpRequestExtensions
	{
		/// <summary>
		/// Returns true if the IP address of the request originator was 127.0.0.1 or if the IP address of the request was the same as the server's IP address.
		/// </summary>
		/// <remarks>
		/// Credit to Filip W for the initial implementation of this method.
		/// See http://www.strathweb.com/2016/04/request-islocal-in-asp-net-core/
		/// </remarks>
		public static bool IsLocal(this HttpRequest request)
		{
			var connection = request.HttpContext.Connection;
			if (connection.RemoteIpAddress != null)
				return connection.RemoteIpAddress.Equals(connection.LocalIpAddress) || IPAddress.IsLoopback(connection.RemoteIpAddress);

			// for in memory TestServer or when dealing with default connection info
			return connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
		}
	}
}