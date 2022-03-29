using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace RaygunCore.Services;

/// <summary>
/// Catches pipeline errors and sends them to Raygun.
/// </summary>
public class RaygunMiddleware
{
	readonly RequestDelegate _next;
	readonly IRaygunClient _client;
	readonly bool _ignoreCanceledErros;

	public RaygunMiddleware(RequestDelegate next, IRaygunClient client, IOptions<RaygunOptions> options)
	{
		_next = next ?? throw new ArgumentNullException(nameof(next));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_ignoreCanceledErros = options.Value.IgnoreCanceledErrors;
	}

	public async Task Invoke(HttpContext httpContext)
	{
		try
		{
			await _next.Invoke(httpContext);
		}
		catch (Exception ex)
		{
			if (!_ignoreCanceledErros || ex is not OperationCanceledException)
				await _client.SendAsync(ex, RaygunSeverity.Critical);
			throw;
		}
	}
}