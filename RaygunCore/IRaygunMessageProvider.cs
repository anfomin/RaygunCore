using RaygunCore.Messages;

namespace RaygunCore
{
	/// <summary>
	/// Provides additional data for <see cref="RaygunMessageDetails"/>.
	/// </summary>
	public interface IRaygunMessageProvider
	{
		/// <summary>
		/// Fills <see cref="RaygunMessageDetails"/> with additional data.
		/// </summary>
		void Apply(RaygunMessageDetails details);
	}
}