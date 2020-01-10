using System;
using System.Collections.Generic;
using RaygunCore.Messages;

namespace RaygunCore
{
	/// <summary>
	/// Responsible for creating <see cref="RaygunMessage"/>.
	/// </summary>
	public interface IRaygunMessageBuilder
	{
		/// <summary>
		/// Creates <see cref="RaygunMessage"/> from text message and exception.
		/// </summary>
		/// <param name="message">Custom text.</param>
		/// <param name="exception">The exception to deliver.</param>
		/// <param name="severity">Message severity.</param>
		/// <param name="tags">A list of strings associated with the message.</param>
		/// <param name="customData">A key-value collection of custom data that will be added to the payload.</param>
		RaygunMessage Build(string message, Exception? exception, RaygunSeverity? severity, IEnumerable<string>? tags, IDictionary<string, object>? customData);
	}
}