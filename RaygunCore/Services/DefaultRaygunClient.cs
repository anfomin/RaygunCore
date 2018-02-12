using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RaygunCore.Messages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace RaygunCore.Services
{
	/// <summary>
	/// Default implementation for <see cref="IRaygunClient"/>.
	/// </summary>
	public class DefaultRaygunClient : IRaygunClient, IDisposable
	{
		readonly IRaygunMessageBuilder _messageBuilder;
		readonly IEnumerable<IRaygunValidator> _validators;
		readonly RaygunOptions _options;
		HttpClient _client;

		public DefaultRaygunClient(IRaygunMessageBuilder messageBuilder, IEnumerable<IRaygunValidator> validators, IOptions<RaygunOptions> options)
		{
			_messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
			_validators = validators;
			_options = options.Value;
			_client = CreateClient();
		}

		/// <inheritdoc/>
		public virtual void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
				_client = null;
			}
		}

		/// <summary>
		/// Creates <see cref="HttpClient"/> with default configuration.
		/// </summary>
		protected virtual HttpClient CreateClient()
		{
			if (string.IsNullOrEmpty(_options.ApiKey))
				throw new InvalidOperationException("API key is required");

			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("X-ApiKey", _options.ApiKey);
			return client;
		}

		/// <summary>
		/// Determines if message and exception should be sent to Raygun.
		/// </summary>
		/// <param name="message">Custom text.</param>
		/// <param name="exception">The exception to deliver.</param>
		protected virtual bool ShouldSend(string message, Exception exception)
		{
			return (exception == null || !exception.IsSent())
				&& !(exception is RaygunException)
				&& (_validators == null || _validators.All(v => v.ShouldSend(message, exception)));
		}

		/// <summary>
		/// Transmits a message to Raygun.
		/// </summary>
		/// <param name="message">The RaygunMessage to send. This needs its OccurredOn property
		/// set to a valid DateTime and as much of the Details property as is available.</param>
		protected async Task SendMessageAsync(RaygunMessage message)
		{
			try
			{
				var result = await _client.PostAsync(_options.ApiEndpoint, new JsonContent(message));
				result.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error sending exception to Raygun {ex.Message}");
				if (_options.ThrowOnError)
					throw new RaygunException(ex);
			}
		}

		/// <inheritdoc/>
		public async Task SendAsync(string message, Exception exception, RaygunSeverity? severity = null, IList<string> tags = null, IDictionary<string, object> customData = null)
		{
			foreach (var ex in StripWrapperExceptions(exception))
			{
				if (ShouldSend(message, ex))
				{
					var msg = _messageBuilder.Build(message, ex, severity, tags, customData);
					await SendMessageAsync(msg);
					if (ex != null)
						ex.MarkSent();
				}
			}
		}

		IEnumerable<Exception> StripWrapperExceptions(Exception exception)
		{
			if (exception != null && _options.WrapperExceptions.Any(ex => exception.GetType() == ex && exception.InnerException != null))
			{
				if (exception is AggregateException aggregate)
				{
					foreach (var inner in aggregate.InnerExceptions)
					foreach (var ex in StripWrapperExceptions(inner))
						yield return ex;
				}
				else
				{
					foreach (Exception ex in StripWrapperExceptions(exception.InnerException))
						yield return ex;
				}
			}
			else
			{
				yield return exception;
			}
		}

		class JsonContent : StringContent
		{
			public JsonContent(object obj) : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json") { }
		}
	}
}