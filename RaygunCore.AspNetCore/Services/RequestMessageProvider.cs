using System;
using System.Collections.Generic;
using System.Linq;
using RaygunCore.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace RaygunCore.Services
{
	/// <summary>
	/// Provides <see cref="RaygunMessageDetails"/> with request information from <see cref="HttpContext"/>.
	/// </summary>
	public class RequestMessageProvider : IRaygunMessageProvider
	{
		readonly IHttpContextAccessor _httpContextAccessor;
		readonly RaygunOptions _options;

		public RequestMessageProvider(IHttpContextAccessor httpContextAccessor, IOptions<RaygunOptions> options)
		{
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_options = options.Value;
		}

		/// <inheritdoc/>
		public void Apply(RaygunMessageDetails details)
		{
			var context = _httpContextAccessor.HttpContext;
			if (context == null)
				return;

			var request = context.Request;
			details.Request = new RaygunRequestMessage
			{
				HostName = request.Host.Value,
				Url = request.GetDisplayUrl(),
				HttpMethod = request.Method,
				IPAddress = GetIpAddress(context.Connection),
				QueryString = GetQueryString(request),
				Headers = GetHeaders(request),
				Cookies = GetCookies(request),
				Form = GetForm(request)
			};
		}

		string? GetIpAddress(ConnectionInfo connection)
		{
			var ip = connection.RemoteIpAddress ?? connection.LocalIpAddress;
			if (ip == null)
				return null;

			int? port = connection.RemotePort == 0 ? connection.LocalPort : connection.RemotePort;
			if (port != 0)
				return ip + ":" + port.Value;
			return ip.ToString();
		}

		Dictionary<string, object> GetQueryString(HttpRequest request)
		{
			return request.Query.ToDictionary(q => q.Key, q => (object)q.Value.ToString());
		}

		Dictionary<string, object>? GetHeaders(HttpRequest request)
		{
			if (_options.IgnoreHeaders)
				return null;
			return request.Headers
				.Where(h => !_options.IgnoreHeaderNames.Contains(h.Key))
				.ToDictionary(h => h.Key, h => (object)h.Value.ToString());
		}

		Dictionary<string, object>? GetCookies(HttpRequest request)
		{
			if (_options.IgnoreCookies)
				return null;
			return request.Cookies
				.Where(c => !_options.IgnoreCookieNames.Contains(c.Key))
				.ToDictionary(c => c.Key, c => (object)c.Value);
		}

		Dictionary<string, object>? GetForm(HttpRequest request)
		{
			if (_options.IgnoreForm || !request.HasFormContentType)
				return null;

			IFormCollection form;
			try { form = request.Form; }
			catch (InvalidOperationException) { return null; }

			return request.Form
				.Where(h => !_options.IgnoreFormFields.Contains(h.Key))
				.ToDictionary(h => h.Key, h => (object)h.Value.ToString());
		}
	}
}