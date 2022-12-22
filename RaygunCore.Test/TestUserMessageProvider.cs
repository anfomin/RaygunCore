using RaygunCore.Messages;

namespace RaygunCore.Test;

class TestUserMessageProvider : IRaygunMessageProvider
{
	public const string Email = "user@example.net";

	public void Apply(RaygunMessageDetails details)
	{
		details.User = new(Email)
		{
			Email = Email,
			IsAnonymous = false
		};
	}
}