using System;
using RaygunCore.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace RaygunCore.Services
{
	/// <summary>
	/// Provides <see cref="RaygunMessageDetails"/> with response information from <see cref="HttpContext"/>.
	/// </summary>
	public class ResponseMessageProvider : IRaygunMessageProvider
	{
		readonly IHttpContextAccessor _httpContextAccessor;

		public ResponseMessageProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		}

		/// <inheritdoc/>
		public void Apply(RaygunMessageDetails details)
		{
			var context = _httpContextAccessor.HttpContext;
			if (context == null || !context.Response.HasStarted)
				return;

			var responseFeature = context.Features.Get<IHttpResponseFeature>();
			details.Response = new RaygunResponseMessage
			{
				StatusCode = context.Response.StatusCode,
				StatusDescription = responseFeature?.ReasonPhrase
			};
		}
	}
}