using RaygunCore.Messages;
using Microsoft.AspNetCore.Http;

namespace RaygunCore.Services;

/// <summary>
/// Provides <see cref="RaygunMessageDetails"/> with user information from <see cref="HttpContext"/>.
/// </summary>
public class UserMessageProvider : IRaygunMessageProvider
{
	readonly IHttpContextAccessor _httpContextAccessor;

	public UserMessageProvider(IHttpContextAccessor httpContextAccessor)
		=> _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

	/// <inheritdoc/>
	public void Apply(RaygunMessageDetails details)
	{
		var context = _httpContextAccessor.HttpContext;
		if (context == null)
			return;
		if (context.User.Identity?.IsAuthenticated == true)
			details.User = new RaygunUserMessage(context.User.Identity.Name!);
	}
}