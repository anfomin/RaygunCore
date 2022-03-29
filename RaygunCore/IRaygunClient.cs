namespace RaygunCore;

/// <summary>
/// Entiry point to send messages to Raygun.
/// </summary>
public interface IRaygunClient
{
	/// <summary>
	/// Transmits message or exception to Raygun.
	/// </summary>
	/// <param name="message">Custom text.</param>
	/// <param name="exception">The exception to deliver.</param>
	/// <param name="severity">Message severity.</param>
	/// <param name="tags">A list of strings associated with the message.</param>
	/// <param name="customData">A key-value collection of custom data that will be added to the payload.</param>
	Task SendAsync(string message, Exception? exception, RaygunSeverity? severity = null, IEnumerable<string>? tags = null, IDictionary<string, object>? customData = null);
}

/// <summary>
/// <see cref="IRaygunClient"/> extension methods.
/// </summary>
public static class RaygunExtensions
{
	/// <summary>
	/// Transmits message to Raygun.
	/// </summary>
	/// <param name="message">Custom text.</param>
	/// <param name="severity">Message severity.</param>
	/// <param name="tags">A list of strings associated with the message.</param>
	/// <param name="customData">A key-value collection of custom data that will be added to the payload.</param>
	public static Task SendAsync(this IRaygunClient client, string message, RaygunSeverity? severity = null, IEnumerable<string>? tags = null, IDictionary<string, object>? customData = null)
		=> client.SendAsync(message, null, severity, tags, customData);

	/// <summary>
	/// Transmits exception to Raygun.
	/// </summary>
	/// <param name="exception">The exception to deliver.</param>
	/// <param name="severity">Exception severity.</param>
	/// <param name="tags">A list of strings associated with the message.</param>
	/// <param name="customData">A key-value collection of custom data that will be added to the payload.</param>
	public static Task SendAsync(this IRaygunClient client, Exception exception, RaygunSeverity? severity = null, IEnumerable<string>? tags = null, IDictionary<string, object>? customData = null)
		=> client.SendAsync(exception.Message, exception, severity, tags, customData);
}