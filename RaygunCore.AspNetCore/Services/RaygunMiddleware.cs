using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace RaygunCore.Services
{
	/// <summary>
	/// Catches pipeline errors and sends them to Raygun.
	/// </summary>
	public class RaygunMiddleware
	{
		readonly RequestDelegate _next;

		public RaygunMiddleware(RequestDelegate next)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next.Invoke(httpContext);
			}
			catch (Exception ex)
			{
				var client = httpContext.RequestServices.GetRequiredService<IRaygunClient>();
				await client.SendAsync(ex, RaygunSeverity.Critical);
				throw;
			}
		}
	}
}