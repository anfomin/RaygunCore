using System;

namespace RaygunCore
{
	/// <summary>
	/// Provides method to check if message and exception should be sent to Raygun.
	/// </summary>
	public interface IRaygunValidator
	{
		/// <summary>
		/// Determines if message and exception should be sent to Raygun.
		/// </summary>
		/// <param name="message">Custom text.</param>
		/// <param name="exception">The exception to deliver.</param>
		bool ShouldSend(string message, Exception exception);
	}
}