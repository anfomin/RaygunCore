using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaygunCore.Messages;

namespace RaygunCore.Services;

/// <summary>
/// Default implementation for <see cref="IRaygunClient"/>.
/// </summary>
public class DefaultRaygunClient : IRaygunClient
{
	readonly ILogger _logger;
	readonly IHttpClientFactory _clientFactory;
	readonly IRaygunMessageBuilder _messageBuilder;
	readonly IEnumerable<IRaygunValidator> _validators;
	readonly RaygunOptions _options;

	public DefaultRaygunClient(
		ILogger<DefaultRaygunClient> logger,
		IHttpClientFactory clientFactory,
		IRaygunMessageBuilder messageBuilder,
		IEnumerable<IRaygunValidator> validators,
		IOptions<RaygunOptions> options)
	{
		_logger = logger;
		_clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
		_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
		_validators = validators ?? throw new ArgumentNullException(nameof(validators));;
		_options = options.Value;
	}

	/// <inheritdoc/>
	public async Task SendAsync(string message,
		Exception? exception,
		RaygunSeverity? severity = null,
		IEnumerable<string>? tags = null,
		IDictionary<string, object>? customData = null
	) {
		if (_options.Tags.Count > 0)
			tags = tags == null ? _options.Tags : tags.Concat(_options.Tags).Distinct();

		IEnumerable<Exception?> exceptions = exception?.StripWrapperExceptions(_options.WrapperExceptions) ?? Enumerable.Repeat<Exception?>(null, 1);
		foreach (var ex in exceptions)
		{
			if (ShouldSend(message, ex))
			{
				var msg = _messageBuilder.Build(message, ex, severity, tags, customData);
				await TransmitMessageAsync(msg);
				if (ex != null)
					ex.MarkSent();
			}
		}
	}

	/// <summary>
	/// Creates <see cref="HttpClient"/> with default configuration.
	/// </summary>
	protected virtual HttpClient CreateClient()
	{
		if (string.IsNullOrEmpty(_options.ApiKey))
			throw new InvalidOperationException("API key is required");

		var client = _clientFactory.CreateClient();
		client.DefaultRequestHeaders.Add("X-ApiKey", _options.ApiKey);
		return client;
	}

	/// <summary>
	/// Determines if message and exception should be sent to Raygun.
	/// </summary>
	/// <param name="message">Custom text.</param>
	/// <param name="exception">The exception to deliver.</param>
	protected virtual bool ShouldSend(string message, Exception? exception)
		=> (exception == null || !exception.IsSent())
			&& exception is not RaygunException
			&& _validators.All(v => v.ShouldSend(message, exception));

	/// <summary>
	/// Transmits a message to Raygun.
	/// </summary>
	/// <param name="message">The RaygunMessage to send. This needs its OccurredOn property
	/// set to a valid DateTime and as much of the Details property as is available.</param>
	protected async Task TransmitMessageAsync(RaygunMessage message)
	{
		try
		{
			var content = JsonContent.Create(message);
			await content.LoadIntoBufferAsync();
			var result = await CreateClient().PostAsync(_options.ApiEndpoint, content);
			result.EnsureSuccessStatusCode();
		}
		catch (Exception ex)
		{
			if (_options.ThrowOnError)
				throw new RaygunException(ex);
			else
				_logger.LogError(ex, "Raygun transmission failed");
		}
	}
}